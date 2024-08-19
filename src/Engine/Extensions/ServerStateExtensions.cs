namespace Engine;

public static class ServerStateExtensions
{
	public static string ToStateIssueBody(this ServerState state)
	{
		var logs = state.Logs.ToString();

		var dto = ServerStateDto.FromServerState(state);

		var args = new object[]
		{
			logs,
			dto.ToString(),
		};

		return string.Format(StateBodyTemplate, args);
	}

	private const string StateBodyTemplate = @"
### Logs
{0}
___
<details><summary>DTO</summary>
<p>

```json
{1}
```

</p>
</details>";
}
