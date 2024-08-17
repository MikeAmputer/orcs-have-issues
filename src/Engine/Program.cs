using System.Text;
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
var utcNow = DateTimeOffset.UtcNow;
var since = utcNow.AddHours(-options.PeriodHours);


var ghClient = new GitHubClient(productInfo, credentialStore);

var repository = await ghClient.Repository.Get(owner, repositoryName);

Logging.RateLimitProvider = () => ghClient.GetLastApiInfo()?.RateLimit;

var issueRepository = await PlayerDataRepository.Create(repository, ghClient, since);

var characters = issueRepository.GetCharacters().ToList();

foreach (var (character, commands) in characters)
{
	var logs = new StringBuilder($"Time of processing: `{utcNow}`");
	logs.AppendLine();
	foreach (var command in commands)
	{
		var actionReport = ActionRunner.Execute(command, character);
		if (actionReport.IsExecuted)
		{
			logs.AppendLine(actionReport.LogMessage);
		}
	}

	var stateBody = character.ToStateCommentBody(logs.ToString());

	if (options.TestMode)
	{
		Logging.LogInfo(stateBody);
		continue;
	}

	if (character.PlayerInfo.StateCommentId != null)
	{
		await ghClient.Issue.Comment.Update(repository.Id, character.PlayerInfo.StateCommentId.Value, stateBody);
	}
	else
	{
		await ghClient.Issue.Comment.Create(repository.Id, character.PlayerInfo.IssueNumber, stateBody);
	}
}

Logging.LogGitHubClientState();
Logging.LogInfo("Complete");
