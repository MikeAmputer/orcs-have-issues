using System.Text.Json;
using System.Text.RegularExpressions;
using Octokit;

namespace Engine;

public static class IssueExtensions
{
	private const string DtoRegexPattern = @"```json\r?\n(.+)\r?\n```";

	private static readonly Regex DtoRegex = new(DtoRegexPattern, RegexOptions.Compiled);

	public static CharacterDto ToCharacterDto(this IssueComment? comment)
	{
		if (comment == null)
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

	public static PlayerInfo ToPlayerInfo(this Issue issue, bool isStargazer, long? stateCommentId)
	{
		return new()
		{
			IssueNumber = issue.Number,
			UserLogin = issue.User.Login,
			StateCommentId = stateCommentId,
			IssueReactions = issue.Reactions.Heart,
			IsStargazer = isStargazer,
			IssueLabels = issue.Labels.Select(label => label.Name).ToHashSet(),
		};
	}

	public static ServerStateDto ToServerStateDto(this Issue? issue)
	{
		if (issue == null)
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
}
