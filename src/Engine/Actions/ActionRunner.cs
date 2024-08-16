namespace Engine;

public sealed class ActionRunner : SubAction
{
	protected override Dictionary<string, IAction> SubCommands => new()
	{
		{ "select-race", new ActionRaceSelection() },
		{ "fight", new ActionFight() },
	};

	private static readonly ActionRunner Instance = new();

	public new static ActionReport Execute(string command, Character character)
	{
		var parameters = command
			.Trim()
			.ToLower()
			.Split(' ', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);

		return Instance.Execute(parameters, character);
	}
}
