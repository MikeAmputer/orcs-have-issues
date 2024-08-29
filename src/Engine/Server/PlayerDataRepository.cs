using System.Text.RegularExpressions;
using Octokit;

namespace Engine;

public class PlayerDataRepository
{
	private static readonly Regex CommentIssueNumberRegex = new(@"/issues/(\d+)", RegexOptions.Compiled);

	private static readonly ApiOptions BatchPagination = new()
	{
		PageSize = 100
	};

	private const int CommandMaxLength = 100;
	private const int CommandsMaxQuantity = 20;

	private readonly Repository _gitHubRepository;
	private readonly Dictionary<int, Issue> _issues = new();
	private readonly HashSet<long> _players = new();
	private readonly Dictionary<int, IssueComment?> _characterStates = new();
	private readonly Dictionary<int, IssueComment> _characterActions = new();
	private HashSet<long> _stargazers = null!;

	private PlayerDataRepository(Repository gitHubRepository)
	{
		_gitHubRepository = gitHubRepository;
	}

	public static async Task<(PlayerDataRepository PlayerData, bool shouldSimulate)> Create(
		IGitHubClient gitHubClient,
		Repository gitHubRepository,
		DateTimeOffset since)
	{
		var result = new PlayerDataRepository(gitHubRepository);

		var anyEditedIssues = await result.LoadIssues(gitHubClient.Issue, since);

		if (!anyEditedIssues)
		{
			Logging.LogInfo($"No edited issues since {since}");

			return (result, false);
		}

		var anyNewComments = await result.LoadComments(gitHubClient.Issue.Comment, since);

		if (!anyNewComments)
		{
			Logging.LogInfo($"No new comments since {since}");

			return (result, false);
		}

		await result.LoadStargazers(gitHubClient.Activity.Starring, gitHubRepository);

		return (result, true);
	}

	public IEnumerable<(Character Character, string[] Commands)> GetCharacters(DateTimeOffset utcNow)
	{
		foreach (var (key, issue) in _issues)
		{
			var dto = _characterStates.TryGetValue(key, out var state)
				? state.ToCharacterDto()
				: new CharacterDto();

			var playerInfo = issue.ToPlayerInfo(_stargazers.Contains(issue.User.Id), state?.Id);
			var commands = GetCommands(key);

			if (commands.Length != 0)
			{
				yield return (new Character(playerInfo, dto, utcNow), commands);
			}
		}
	}

	private string[] GetCommands(int issueNumber)
	{
		if (!_characterActions.TryGetValue(issueNumber, out var comment))
		{
			return [];
		}

		return comment.Body
			.Split(["\r\n", "\r", "\n"], StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)
			.Where(command => command.Length <= CommandMaxLength)
			.Take(CommandsMaxQuantity)
			.ToArray();
	}

	private async Task<bool> LoadIssues(IIssuesClient issuesClient, DateTimeOffset since)
	{
		Logging.LogInfo("Requesting issues");

		var issues = await issuesClient.GetAllForRepository(
			_gitHubRepository.Id,
			new RepositoryIssueRequest
			{
				Filter = IssueFilter.All,
				Labels = { "character" },
				State = ItemStateFilter.Open,
				SortProperty = IssueSort.Updated,
				SortDirection = SortDirection.Descending,
				Since = since,
			},
			BatchPagination);

		Logging.LogInfo("Issues retrieved");
		Logging.LogGitHubClientState();

		if (issues.Count == 0)
		{
			return false;
		}

		foreach (var issue in issues)
		{
			if (issue.Comments == 0 || _players.Contains(issue.User.Id))
			{
				continue;
			}

			if (_issues.TryAdd(issue.Number, issue))
			{
				_players.Add(issue.User.Id);
			}
		}

		return true;
	}

	private async Task<bool> LoadComments(IIssueCommentsClient commentsClient, DateTimeOffset since)
	{
		Logging.LogInfo("Requesting comments");

		var comments = await commentsClient.GetAllForRepository(
			_gitHubRepository.Id,
			new IssueCommentRequest
			{
				Sort = IssueCommentSort.Updated,
				Direction = SortDirection.Descending,
				Since = since,
			},
			BatchPagination);

		Logging.LogInfo("Comments retrieved");
		Logging.LogGitHubClientState();

		var commentsByIssue = comments
			.GroupBy(comment => Convert.ToInt32(
				CommentIssueNumberRegex.Match(comment.HtmlUrl).TryGetGroupValue(1, "0")))
			.Where(group => group.Key > 0 && _issues.ContainsKey(group.Key))
			.ToList();

		if (commentsByIssue.Count == 0)
		{
			return false;
		}

		foreach (var group in commentsByIssue)
		{
			if (!_issues.TryGetValue(group.Key, out var issue))
			{
				throw new InvalidOperationException();
			}

			var actionsComment = FindActionsComment(group, issue.User.Id);

			if (actionsComment == null)
			{
				_issues.Remove(issue.Number);

				continue;
			}

			var stateComment = FindStateComment(group, _gitHubRepository.Owner.Id);

			if (stateComment == null)
			{
				Logging.LogInfo($"Scanning issue #{issue.Number} for old states");

				var issueComments = await commentsClient.GetAllForIssue(
					_gitHubRepository.Id,
					issue.Number,
					BatchPagination);

				stateComment = FindStateComment(issueComments, _gitHubRepository.Owner.Id);

				if (stateComment == null)
				{
					Logging.LogInfo($"Issue #{issue.Number} state not found");
				}
			}

			_characterActions.Add(issue.Number, actionsComment);
			_characterStates.Add(issue.Number, stateComment);
		}

		Logging.LogInfo("Comments attributed");
		Logging.LogGitHubClientState();

		return true;
	}

	private static IssueComment? FindActionsComment(IEnumerable<IssueComment> comments, long issueAuthor)
	{
		return comments
			.Where(comment => comment.User.Id == issueAuthor)
			.MaxBy(comment => comment.CreatedAt);
	}

	private static IssueComment? FindStateComment(IEnumerable<IssueComment> comments, long repositoryOwner)
	{
		return comments
			.Where(comment => comment.User.Id == repositoryOwner)
			.Where(comment => comment.Body.StartsWith("### Stats"))
			.MinBy(comment => comment.CreatedAt);
	}

	private async Task LoadStargazers(IStarredClient starredClient, Repository gitHubRepository)
	{
		Logging.LogInfo("Requesting stargazers");

		var stars = await starredClient
			.GetAllStargazersWithTimestamps(gitHubRepository.Id, BatchPagination);

		Logging.LogInfo("Stargazers retrieved");
		Logging.LogGitHubClientState();

		_stargazers = stars.Select(s => s.User.Id).ToHashSet();
	}
}
