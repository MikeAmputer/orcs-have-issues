namespace Engine.Tests.Playground;

[TestClass]
[Ignore]
public class FightPlayground
{
	[TestMethod]
	public void Player_Vs_Monsters()
	{
		var player = new TestCharacter(100, 2, 1, 0, 0);
		//Fighter[] monsters = [Monster.SpiderGuardian, Monster.SpiderGuardian, Monster.SpiderScout, Monster.SpiderAmbusher];
		Fighter[] monsters = [Monster.Goblin, Monster.GoblinSoldier];

		new Fight([player], monsters).Simulate();

		Logging.LogInfo($"HP: {player.CurrentHp} | EXP: {player.LevelInfo.Exp}");
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
