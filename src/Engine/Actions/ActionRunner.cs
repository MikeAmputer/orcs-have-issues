namespace Engine;

public static class ActionRunner
{
	private static readonly Dictionary<string, ActionBase> _actions = new()
	{
		{ "fight", new ActionFight() },
	};

	public static ActionReport Execute(string command, Character character)
	{
		var parameters = command
			.Trim()
			.ToLower()
			.Split(' ', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);

		if (parameters.Length == 0)
		{
			return ActionReport.Empty;
		}

		if (!_actions.TryGetValue(parameters[0], out var action))
		{
			return ActionReport.Empty;
		}

		return action.Execute(parameters[1..], character);
	}
}
