using System.Text.RegularExpressions;
using CommandLine;
using Engine;
using Octokit;
using Octokit.Internal;

const string owner = "MikeAmputer";
const string repositoryName = "orcs-have-issues";
const string version = "1";

var productInfo = new ProductHeaderValue(owner, version);
var options = Parser.Default.ParseArguments<Options>(args).Value;
var credentialStore = new InMemoryCredentialStore(new Credentials(options.GitHubToken));
var since = DateTimeOffset.UtcNow.AddHours(-options.PeriodHours);

var batchPagination = new ApiOptions
{
	PageSize = 100
};

var ghClient = new GitHubClient(productInfo, credentialStore);

var repository = await ghClient.Repository.Get(owner, repositoryName);

Logging.LogGitHubClientState(ghClient);
Logging.LogInfo("Requesting issues");

var issues = await ghClient.Issue.GetAllForRepository(
	repository.Id,
	new RepositoryIssueRequest
	{
		Filter = IssueFilter.All,
		State = ItemStateFilter.Open,
		SortProperty = IssueSort.Updated,
		SortDirection = SortDirection.Ascending,
		Since = since,
	},
	batchPagination);

Logging.LogGitHubClientState(ghClient);
Logging.LogInfo("Requesting comments");

var comments = await ghClient.Issue.Comment.GetAllForRepository(
	repository.Id,
	new IssueCommentRequest
	{
		Sort = IssueCommentSort.Updated,
		Direction = SortDirection.Ascending,
		Since = since,
	},
	batchPagination);

Logging.LogGitHubClientState(ghClient);

var characterIssueRegex = new Regex("^character", RegexOptions.IgnoreCase | RegexOptions.Compiled);
var characterRepository = new CharacterRepository();

foreach (var issue in issues)
{
	var userId = issue.User.Id;

	if (!characterIssueRegex.IsMatch(issue.Title))
	{
		continue;
	}

	if (characterRepository.ContainsUser(userId))
	{
		continue;
	}

	characterRepository.Add(
		userId,
		new Character
		{
			UserLogin = issue.User.Login
		});
}
