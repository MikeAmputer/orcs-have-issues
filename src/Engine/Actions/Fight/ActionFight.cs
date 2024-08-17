namespace Engine;

public sealed class ActionFight : SubAction
{
	protected override Dictionary<string, IAction> SubCommands => new()
	{
		{ "spider-lair", ActionFightInstance.SpiderLair },
		{ "spider-lair-elite", ActionFightInstance.SpiderLairElite },
		{ "goblin-camp", ActionFightInstance.GoblinCamp },
	};
}
