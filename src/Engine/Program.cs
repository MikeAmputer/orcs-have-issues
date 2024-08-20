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

await ServerState.Instance.Initialize(ghClient, repository, utcNow);

var playerData = await PlayerDataRepository.Create(ghClient, repository, since);

var characters = playerData.GetCharacters(utcNow).ToList();
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

Logging.LogInfo("Characters saved");
Logging.LogGitHubClientState();
Logging.LogInfo("Saving server state");

ServerState.Instance.UpdateLeaderboard(characters.Select(t => t.Character));
var stateIssueBody = ServerState.Instance.ToStateIssueBody();

if (!options.TestMode)
{

	if (ServerState.Instance.IssueNumber != null)
	{
		await ghClient.Issue.Update(
			repository.Id,
			ServerState.Instance.IssueNumber!.Value,
			new IssueUpdate
			{
				Title = "Server State",
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
