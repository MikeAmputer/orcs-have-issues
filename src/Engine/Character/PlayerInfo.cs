namespace Engine;

public class PlayerInfo
{
	public int IssueNumber { get; init; }
	public string UserLogin { get; init; } = string.Empty;
	public long? StateCommentId { get; init; }

	public int IssueReactions { get; init; }
	public bool IsStargazer { get; init; }

	public HashSet<string> IssueLabels { get; init; } = new();
}
