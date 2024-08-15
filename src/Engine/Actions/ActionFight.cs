namespace Engine;

public class ActionFight : ActionBase
{
	protected override Dictionary<string, ActionBase> SubCommands => new()
	{
		{ "goblin-camp", ActionFightInstance.GoblinCamp },
	};
}
