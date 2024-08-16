namespace Engine;

public abstract class SubAction : IAction
{
	protected abstract Dictionary<string, IAction> SubCommands { get; }

	public ActionReport Execute(string[] parameters, Character character)
	{
		if (parameters.Length == 0)
		{
			return ActionReport.Empty;
		}

		if (!SubCommands.TryGetValue(parameters[0], out var action))
		{
			return ActionReport.Empty;
		}

		return action.Execute(parameters[1..], character);
	}

	protected ActionReport Execute(string command, Character character)
	{
		var parameters = command
			.Trim()
			.ToLower()
			.Split(' ', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);

		return Execute(parameters, character);
	}
}
