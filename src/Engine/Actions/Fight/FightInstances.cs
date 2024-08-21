namespace Engine;

public sealed partial class ActionFightInstance
{
	public static readonly ActionFightInstance SpiderLair = new(
		"Spider Lair",
		actionPointsCost: 4, goldReward: 2, materialsReward: 1,
		() => [Monster.Spiderling, Monster.SpiderGuardian, Monster.Spiderling, Monster.Spiderling]);

	public static readonly ActionFightInstance SpiderLairElite = new(
		"Spider Lair (Elite)",
		actionPointsCost: 4, goldReward: 4, materialsReward: 2,
		() => [Monster.SpiderGuardian, Monster.SpiderGuardian, Monster.SpiderScout, Monster.SpiderAmbusher]);

	public static readonly ActionFightInstance GoblinCamp = new(
		"Goblin Camp",
		actionPointsCost: 4, goldReward: 14, materialsReward: 0,
		() => [Monster.GoblinScavenger, Monster.GoblinHunter]);

	public static readonly ActionFightInstance GoblinCampElite = new(
		"Goblin Camp (Elite)",
		actionPointsCost: 4, goldReward: 21, materialsReward: 1,
		() => [Monster.GoblinBrute, Monster.GoblinHunter, Monster.GoblinHunter]);

	public static readonly ActionFightInstance BanditHideout = new(
		"Bandit Hideout",
		actionPointsCost: 4, goldReward: 19, materialsReward: 0,
		() => [Monster.BanditMugger, Monster.BanditMarauder]);

	public Fighter[] CreateEnemies => _enemies();
}
