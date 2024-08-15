namespace Engine;

public abstract class ActionBase
{
	public ActionReport Execute(string[] parameters, Character character)
	{
		if (SubCommands == null)
		{
			return ExecuteCore(parameters, character);
		}

		if (parameters.Length == 0)
		{
			return ActionReport.Empty;
		}

		if (!SubCommands.TryGetValue(parameters[0], out var action))
		{
			return ActionReport.Empty;
		}

		if (!action.IsAvailable(character))
		{
			return ActionReport.Empty;
		}

		return action.Execute(parameters[1..], character);
	}

	protected virtual Dictionary<string, ActionBase>? SubCommands { get; } = null;

	protected virtual ActionReport ExecuteCore(string[] parameters, Character character)
	{
		return ActionReport.Empty;
	}

	protected virtual bool IsAvailable(Character character)
	{
		return true;
	}
}
