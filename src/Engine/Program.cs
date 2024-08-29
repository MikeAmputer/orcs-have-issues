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

var ghClient = new GitHubClient(productInfo, credentialStore);

var repository = await ghClient.Repository.Get(owner, repositoryName);

Logging.RateLimitProvider = () => ghClient.GetLastApiInfo()?.RateLimit;

var since = await ServerState.Instance.Initialize(ghClient, repository, options, utcNow);

var (playerData, shouldSimulate) = await PlayerDataRepository.Create(ghClient, repository, since);

if (!shouldSimulate)
{
	Logging.LogInfo("Aborted");
	return;
}

var characters = playerData.GetCharacters(utcNow).ToList();

if (characters.Count == 0)
{
	Logging.LogInfo("No characters to process");
	Logging.LogInfo("Aborted");
	return;
}

await Logging.LogCharactersToFile(characters);

Logging.LogInfo("Executing commands");

foreach (var (character, commands) in characters)
{
	foreach (var command in commands)
	{
		var actionReport = ActionRunner.Execute(command, character);
		if (actionReport.IsExecuted)
		{
			character.Logs.AppendLine(actionReport.LogMessage);
		}
	}
}

Logging.LogInfo("Commands executed");
Logging.LogInfo("Simulating sieges");

ServerState.Instance.SimulateSiege();

Logging.LogInfo("Sieges simulated");
Logging.LogInfo("Saving characters");

foreach (var (character, _) in characters)
{
	character.PrepareForSave();

	var stateBody = character.ToStateCommentBody();

	if (options.TestMode.GetValueOrDefault(true))
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

Logging.LogInfo("Characters saved");
Logging.LogGitHubClientState();
Logging.LogInfo("Saving server state");

ServerState.Instance.PrepareForSave(characters.Select(t => t.Character));
var stateIssueBody = ServerState.Instance.ToStateIssueBody();

if (!options.TestMode.GetValueOrDefault(true))
{
	if (ServerState.Instance.IssueNumber != null)
	{
		await ghClient.Issue.Update(
			repository.Id,
			ServerState.Instance.IssueNumber!.Value,
			new IssueUpdate
			{
				Body = stateIssueBody,
			});
	}
	else
	{
		var issue = await ghClient.Issue.Create(
			repository.Id,
			new NewIssue("Server State")
			{
				Body = stateIssueBody,
				Labels = { "server" },
			});

		Logging.LogInfo($"New server state issue created #{issue.Number}");
	}
}
else
{
	Logging.LogInfo(stateIssueBody);
}

Logging.LogGitHubClientState();
Logging.LogInfo("Complete");
