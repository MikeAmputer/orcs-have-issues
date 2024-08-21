namespace Engine.Tests.Playground;

[TestClass]
[Ignore]
public class FightPlayground
{
	[TestMethod]
	public void Player_Vs_Monsters()
	{
		var player = new TestCharacter(100, 4, 3, 0, 1);
		var monsters = ActionFightInstance.GoblinCampElite.CreateEnemies;
		//Fighter[] monsters = [Monster.BanditEnforcer, Monster.BanditEnforcer, Monster.BanditEnforcer];

		player.StartBattleTracker();
		new Fight([player], monsters).Simulate();
		var tracker = player.StopBattleTracker();

		Logging.LogInfo($"Damage Dealt: {tracker.DamageDealt}");
		Logging.LogInfo($"Damage Taken: {tracker.DamageTaken}");
		Logging.LogInfo($"Damage Mitigated: {tracker.DamageMitigated}");
		Logging.LogInfo($"Health Lost: {tracker.HealthDelta}");
		Logging.LogInfo($"Exp Earned: {tracker.ExpEarned}");
		Logging.LogInfo($"Kills: {tracker.Kills}");
	}

	[TestMethod]
	public void Player_Vs_Player()
	{
		var attacker = new TestCharacter(100, 15, 9, 4, 4);
		var defender = new TestCharacter(119, 12, 10, 4, 4);

		new Fight([attacker], [defender]).Simulate();

		Logging.LogInfo($"HP: {attacker.CurrentHp} | HP2: {defender.CurrentHp}");
	}

	private class TestCharacter : Character
	{
		public TestCharacter(int hp, int atk, int def, int weapon, int armor)
			: base(new PlayerInfo(), new CharacterDto())
		{
			CurrentHp = hp;
			Attack = atk;
			Defence = def;
			WeaponRank = weapon;
			ArmorRank = armor;
		}
	}
}
