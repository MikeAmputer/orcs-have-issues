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

		var fortresses = new StringBuilder();
		foreach (var fortress in state.Fortresses.OrderBy(f => f.Id))
		{
			fortresses.AppendLine($"1. {fortress.ToString()}");
		}

		var args = new object[]
		{
			leaderboard.ToString(),
			fortresses.ToString(),
			logs,
			dto.ToString(),
		};

		return string.Format(StateBodyTemplate, args);
	}

	private const string StateBodyTemplate = @"
### Leaderboard
{0}
### Fortresses
{1}
___
<details><summary>Logs</summary>
<p>

{2}

</p>
</details>

<details><summary>DTO</summary>
<p>

```json
{3}
```

</p>
</details>";
}
