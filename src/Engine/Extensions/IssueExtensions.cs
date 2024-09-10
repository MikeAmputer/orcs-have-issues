using System.Text.Json;
using System.Text.RegularExpressions;
using Octokit;

namespace Engine;

public static class IssueExtensions
{
	private const string DtoRegexPattern = @"```json\r?\n(.+)\r?\n```";

	private static readonly Regex DtoRegex = new(DtoRegexPattern, RegexOptions.Compiled);

	private static readonly Regex CommentIssueNumberRegex = new(@"/issues/(\d+)", RegexOptions.Compiled);

	public static CharacterDto ToCharacterDto(this IssueComment? comment)
	{
		if (comment == null || comment.Body.IsNullOrWhiteSpace())
		{
			return new();
		}

		var jsonString = DtoRegex.Match(comment.Body).TryGetGroupValue(1);

		if (jsonString.IsNullOrWhiteSpace())
		{
			return new();
		}

		return JsonSerializer.Deserialize<CharacterDto>(jsonString)
			?? throw new InvalidOperationException($"Unable to deserialize character DTO: {comment.HtmlUrl}.");
	}

	public static PlayerInfo ToPlayerInfo(this Issue issue, bool isStargazer, long? stateCommentId, bool isBot = false)
	{
		return new()
		{
			IssueNumber = issue.Number,
			UserLogin = issue.User.Login,
			StateCommentId = stateCommentId,
			IssueTitle = issue.Title,
			IssueReactions = issue.Reactions.Heart,
			IsStargazer = isStargazer,
			IssueLabels = issue.Labels.Select(label => label.Name).ToHashSet(),
			IsBot = isBot,
		};
	}

	public static ServerStateDto ToServerStateDto(this Issue? issue)
	{
		if (issue == null || issue.Body.IsNullOrWhiteSpace())
		{
			return new();
		}

		var jsonString = DtoRegex.Match(issue.Body).TryGetGroupValue(1);

		if (jsonString.IsNullOrWhiteSpace())
		{
			return new();
		}

		return JsonSerializer.Deserialize<ServerStateDto>(jsonString)
			?? throw new InvalidOperationException($"Unable to deserialize server state DTO: {issue.HtmlUrl}.");
	}

	public static int GetIssueNumber(this IssueComment comment)
	{
		return Convert.ToInt32(CommentIssueNumberRegex.Match(comment.HtmlUrl).TryGetGroupValue(1, "0"));
	}

	public static IssueComment? FindActionsComment(this IEnumerable<IssueComment> comments, long issueAuthor)
	{
		return comments
			.Where(comment => comment.User.Id == issueAuthor)
			.Where(comment => !comment.Body.StartsWith("### Stats"))
			.MaxBy(comment => comment.CreatedAt);
	}

	public static IssueComment? FindStateComment(this IEnumerable<IssueComment> comments, long repositoryOwner)
	{
		return comments
			.Where(comment => comment.User.Id == repositoryOwner)
			.Where(comment => comment.Body.StartsWith("### Stats"))
			.MinBy(comment => comment.CreatedAt);
	}

	public static string[] GetCommands(this IssueComment comment)
	{
		const int commandMaxLength = 100;
		const int commandsMaxQuantity = 20;

		if (comment.Body.IsNullOrWhiteSpace())
		{
			return [];
		}

		return comment.Body
			.Split(["\r\n", "\r", "\n"], StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)
			.Where(command => command.Length <= commandMaxLength)
			.Take(commandsMaxQuantity)
			.ToArray();
	}
}
