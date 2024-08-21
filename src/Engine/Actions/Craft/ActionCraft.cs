namespace Engine;

public class ActionCraft : SubAction
{
	protected override Dictionary<string, IAction> SubCommands => new()
	{
		{ "weapon", new ActionCraftWeapon() },
		{ "armor", new ActionCraftArmor() },
	};
}
