namespace Engine;

public class ActionLabor : SubAction
{
	protected override Dictionary<string, IAction> SubCommands => new()
	{
		{ "mill", ActionLaborInstance.Mill },
	};
}
