namespace Engine;

public sealed partial class ActionFightInstance
{
	public static readonly ActionFightInstance SpiderLair = new(
		"Spider Lair",
		actionPointsCost: 4, goldReward: 2, materialsReward: 1,
		() => [Monster.Spiderling, Monster.SpiderGuardian, Monster.Spiderling, Monster.Spiderling]);

	public static readonly ActionFightInstance SpiderLairElite = new(
		"Spider Lair (Elite)",
		actionPointsCost: 4, goldReward: 2, materialsReward: 2,
		() => [Monster.SpiderGuardian, Monster.SpiderGuardian, Monster.SpiderScout, Monster.SpiderAmbusher]);

	public static readonly ActionFightInstance SpiderLairHeroic = new(
		"Spider Lair (Heroic)",
		actionPointsCost: 4, goldReward: 2, materialsReward: 4,
		() => [Monster.SpiderGuardian, Monster.SpiderStinger, Monster.SpiderTemplar, Monster.SpiderGuardian]);

	public static readonly ActionFightInstance GoblinCamp = new(
		"Goblin Camp",
		actionPointsCost: 4, goldReward: 14, materialsReward: 0,
		() => [Monster.GoblinScavenger, Monster.GoblinHunter]);

	public static readonly ActionFightInstance GoblinCampElite = new(
		"Goblin Camp (Elite)",
		actionPointsCost: 4, goldReward: 21, materialsReward: 0,
		() => [Monster.GoblinBrute, Monster.GoblinHunter, Monster.GoblinHunter]);

	public static readonly ActionFightInstance BanditHideout = new(
		"Bandit Hideout",
		actionPointsCost: 4, goldReward: 19, materialsReward: 0,
		() => [Monster.BanditMugger, Monster.BanditMarauder]);

	public static readonly ActionFightInstance BanditHideoutElite = new(
		"Bandit Hideout (Elite)",
		actionPointsCost: 4, goldReward: 34, materialsReward: 0,
		() => [Monster.BanditEnforcer, Monster.BanditEnforcer, Monster.BanditMarauder]);

	public static readonly ActionFightInstance Crypt = new(
		"Crypt",
		actionPointsCost: 4, goldReward: 25, materialsReward: 0,
		() => [Monster.UndeadSkeleton, Monster.UndeadGhoul]);

	public static readonly ActionFightInstance CryptElite = new(
		"Crypt (Elite)",
		actionPointsCost: 4, goldReward: 52, materialsReward: 0,
		() => [Monster.UndeadSkeleton, Monster.UndeadLich, Monster.UndeadGhoul]);

	public Fighter[] CreateEnemies => _enemies();
}
