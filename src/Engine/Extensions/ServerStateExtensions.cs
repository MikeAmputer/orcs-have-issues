namespace Engine;

public static class ServerStateExtensions
{
	public static string ToStateIssueBody(this ServerState state)
	{
		var dto = ServerStateDto.FromServerState(state);

		var args = new object[]
		{
			dto.ToString(),
		};

		return string.Format(StateBodyTemplate, args);
	}

	private const string StateBodyTemplate = @"
<details><summary>DTO</summary>
<p>

```json
{0}
```

</p>
</details>";
}
