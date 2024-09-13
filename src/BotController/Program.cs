using BotController;
using CommandLine;
using Engine;
using Octokit;
using Octokit.Internal;
using Options = BotController.Options;

const string owner = "MikeAmputer";
const string repositoryName = "orcs-have-issues";
const string version = "1";

var productInfo = new ProductHeaderValue(owner, version);
var options = Parser.Default.ParseArguments<Options>(args).Value;
var credentialStore = new InMemoryCredentialStore(new Credentials(options.GitHubToken));

var ghClient = new GitHubClient(productInfo, credentialStore);

var repository = await ghClient.Repository.Get(owner, repositoryName);

Logging.RateLimitProvider = () => ghClient.GetLastApiInfo()?.RateLimit;

await ServerState.Instance.Initialize(ghClient, repository, null, DateTimeOffset.UtcNow);

var botRepository = await BotRepository.Create(ghClient, repository);

var bots = botRepository.GetBotSettings().ToList();

if (bots.Count == 0)
{
	Logging.LogInfo("Aborted");

	return;
}

foreach (var (character, issue, comment) in bots)
{
	var actions = ActionsSelector.NextBestActions(character);
	var actionsContent = string.Join('\n', actions);

	if (options.TestMode.GetValueOrDefault(true))
	{
		Logging.LogInfo($"Bot {issue.Title} #{issue.Number}\n{actionsContent}");
	}
	else
	{
		if (comment == null)
		{
			await ghClient.Issue.Comment.Create(repository.Id, issue.Number, actionsContent);
		}
		else
		{
			actionsContent = $"{actionsContent}\nguid {Guid.NewGuid()}";
			await ghClient.Issue.Comment.Update(repository.Id, comment.Id, actionsContent);
		}
	}
}

Logging.LogGitHubClientState();
Logging.LogInfo("Completed");
