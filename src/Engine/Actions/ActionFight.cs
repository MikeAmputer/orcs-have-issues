namespace Engine;

public sealed class ActionFight : SubAction
{
	protected override Dictionary<string, IAction> SubCommands => new()
	{
		{ "goblin-camp", ActionFightInstance.GoblinCamp },
	};
}
