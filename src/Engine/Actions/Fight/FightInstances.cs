namespace Engine;

public sealed partial class ActionFightInstance
{
	public static ActionFightInstance GoblinCamp => new(
		"Goblin Camp",
		actionPointsCost: 3, expReward: 10, goldReward: 5, materialsReward: 2,
		() => [Monster.Goblin, Monster.GoblinSoldier]);
}
