namespace Engine;

public sealed partial class ActionFightInstance
{
	public static readonly ActionFightInstance SpiderLair = new(
		"Spider Lair",
		actionPointsCost: 4, goldReward: 2, materialsReward: 1,
		() => [Monster.SpiderWhelp, Monster.SpiderGuardian, Monster.SpiderWhelp, Monster.SpiderWhelp]);

	public static readonly ActionFightInstance SpiderLairElite = new(
		"Spider Lair (Elite)",
		actionPointsCost: 4, goldReward: 4, materialsReward: 2,
		() => [Monster.SpiderGuardian, Monster.SpiderGuardian, Monster.SpiderScout, Monster.SpiderAmbusher]);

	public static readonly ActionFightInstance GoblinCamp = new(
		"Goblin Camp",
		actionPointsCost: 4, goldReward: 14, materialsReward: 0,
		() => [Monster.Goblin, Monster.GoblinSoldier]);
}
