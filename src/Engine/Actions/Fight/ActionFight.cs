namespace Engine;

public sealed class ActionFight : SubAction
{
	protected override Dictionary<string, IAction> SubCommands => new()
	{
		{ "spider-lair", ActionFightInstance.SpiderLair },
		{ "spider-lair-elite", ActionFightInstance.SpiderLairElite },
		{ "spider-lair-heroic", ActionFightInstance.SpiderLairHeroic },
		{ "goblin-camp", ActionFightInstance.GoblinCamp },
		{ "goblin-camp-elite", ActionFightInstance.GoblinCampElite },
		{ "bandit-hideout", ActionFightInstance.BanditHideout },
		{ "bandit-hideout-elite", ActionFightInstance.BanditHideoutElite },
		{ "crypt", ActionFightInstance.Crypt },
		{ "crypt-elite", ActionFightInstance.CryptElite },
	};
}
