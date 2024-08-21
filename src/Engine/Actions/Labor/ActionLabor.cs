namespace Engine;

public class ActionLabor : SubAction
{
	protected override Dictionary<string, IAction> SubCommands => new()
	{
		{ "mill", ActionLaborInstance.Mill },
		{ "workshop", ActionLaborInstance.Workshop },
		{ "farm", ActionLaborInstance.Farm },
		{ "lumberyard", ActionLaborInstance.Lumberyard },
		{ "quarry", ActionLaborInstance.Quarry },
		{ "forge", ActionLaborInstance.Forge },
	};
}
