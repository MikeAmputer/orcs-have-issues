using System.Text.Json;
using System.Text.RegularExpressions;
using Octokit;

namespace Engine;

public static class IssueExtensions
{
	private const string CharacterDtoRegexPattern = @"
```json
(.+)
```
";

	private static readonly Regex CharacterDtoRegex = new(CharacterDtoRegexPattern, RegexOptions.Compiled);

	public static CharacterDto ToCharacterDto(this IssueComment? comment)
	{
		if (comment == null)
		{
			return new CharacterDto();
		}

		var jsonString = CharacterDtoRegex.Match(comment.Body).TryGetGroupValue(1);

		if (jsonString.IsNullOrWhiteSpace())
		{
			return new CharacterDto();
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
		};
	}
}
