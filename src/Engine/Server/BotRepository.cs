using Octokit;

namespace Engine;

public class BotRepository
{
	private static readonly ApiOptions BatchPagination = new()
	{
		PageSize = 100
	};

	private readonly Repository _gitHubRepository;
	private readonly Dictionary<int, Issue> _issues = new();
	private readonly Dictionary<int, IssueComment?> _states = new();
	private readonly Dictionary<int, IssueComment?> _actions = new();

	private BotRepository(Repository gitHubRepository)
	{
		_gitHubRepository = gitHubRepository;
	}

	public static async Task<BotRepository> Create(IGitHubClient gitHubClient, Repository gitHubRepository)
	{
		var result = new BotRepository(gitHubRepository);

		var anyBots = await result.LoadIssues(gitHubClient.Issue);

		if (!anyBots)
		{
			Logging.LogInfo($"No bots found");

			return result;
		}

		await result.LoadComments(gitHubClient.Issue.Comment);

		return result;
	}

	public IEnumerable<(Character Character, string[] Commands)> GetBotPlayers(DateTimeOffset utcNow)
	{
		foreach (var (character, _, actionsComment) in GetBotSettings(utcNow))
		{
			if (actionsComment == null)
			{
				continue;
			}

			var commands = actionsComment.GetCommands();

			yield return (character, commands);
		}
	}

	public IEnumerable<(Character Character, Issue Issue, IssueComment? ActionsComment)> GetBotSettings(
		DateTimeOffset? utcNow = null)
	{
		foreach (var (issueNumber, issue) in _issues)
		{
			var dto = _states.TryGetValue(issueNumber, out var state)
				? state.ToCharacterDto()
				: new CharacterDto();

			var playerInfo = issue.ToPlayerInfo(false, state?.Id);

			_actions.TryGetValue(issueNumber, out var commandsComment);

			yield return (new Character(playerInfo, dto, utcNow), issue, commandsComment);
		}
	}

	private async Task<bool> LoadIssues(IIssuesClient issuesClient)
	{
		Logging.LogInfo("Requesting bot issues");

		var issues = await issuesClient.GetAllForRepository(
			_gitHubRepository.Id,
			new RepositoryIssueRequest
			{
				Filter = IssueFilter.All,
				Labels = { "bot" },
				State = ItemStateFilter.Open,
				SortProperty = IssueSort.Updated,
				SortDirection = SortDirection.Descending,
			},
			BatchPagination);

		Logging.LogInfo("Bot issues retrieved");
		Logging.LogGitHubClientState();

		if (issues.Count == 0)
		{
			return false;
		}

		foreach (var issue in issues)
		{
			_issues.Add(issue.Number, issue);
		}

		return true;
	}

	private async Task LoadComments(IIssueCommentsClient commentsClient)
	{
		Logging.LogInfo("Requesting bot comments");

		foreach (var (issueNumber, issue) in _issues)
		{
			var comments = await commentsClient.GetAllForIssue(
				_gitHubRepository.Id,
				issueNumber,
				BatchPagination);

			_states.Add(issueNumber, comments.FindStateComment(_gitHubRepository.Owner.Id));
			_actions.Add(issueNumber, comments.FindActionsComment(_gitHubRepository.Owner.Id));
		}

		Logging.LogInfo("Bot comments retrieved");
		Logging.LogGitHubClientState();
	}
}
