namespace Engine;

public partial class ActionLaborInstance
{
	public static readonly ActionLaborInstance Mill = new(
		"Mill",
		requiredLevel: 0, actionPointsCost: 6, goldReward: 40, materialsReward: 0);
}
