﻿namespace Engine;

public sealed class ActionRunner : SubAction
{
	protected override Dictionary<string, IAction> SubCommands => new()
	{
		{ "select-race", new ActionRaceSelection() },
		{ "level-up", new ActionLevelUp() },
		{ "fight", new ActionFight() },
		{ "craft", new ActionCraft() },
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