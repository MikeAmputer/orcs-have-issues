using System.Text;

namespace Engine;

public static class ServerStateExtensions
{
	public static string ToStateIssueBody(this ServerState state)
	{
		var logs = state.Logs.ToString();

		var dto = ServerStateDto.FromServerState(state);

		var leaderboard = new StringBuilder();
		foreach (var entry in dto.Leaderboard)
		{
			leaderboard.AppendLine($"1. [{entry.Login}]({entry.IssueNumber}) - `{entry.Exp} EXP`");
		}

		var args = new object[]
		{
			leaderboard.ToString(),
			logs,
			dto.ToString(),
		};

		return string.Format(StateBodyTemplate, args);
	}

	private const string StateBodyTemplate = @"
### Leaderboard
{0}
### Logs
{1}
___
<details><summary>DTO</summary>
<p>

```json
{2}
```

</p>
</details>";
}
