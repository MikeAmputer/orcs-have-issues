namespace Engine;

public class PlayerInfo
{
	public int IssueNumber { get; init; }
	public string UserLogin { get; init; } = string.Empty;
	public long? StateCommentId { get; init; }
	public string IssueTitle { get; init; } = string.Empty;

	public int IssueReactions { get; init; }
	public bool IsStargazer { get; init; }

	public bool IsBot { get; init; }

	public HashSet<string> IssueLabels { get; init; } = new();
}
