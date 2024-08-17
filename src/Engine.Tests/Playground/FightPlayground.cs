namespace Engine.Tests.Playground;

[TestClass]
[Ignore]
public class FightPlayground
{
	[TestMethod]
	public void Player_Vs_Monsters()
	{
		var player = new TestCharacter(74, 4, 1, 0, 0);
		Fighter[] monsters = [Monster.SpiderGuardian, Monster.SpiderGuardian, Monster.SpiderScout, Monster.SpiderAmbusher];
		//Fighter[] monsters = [Monster.Goblin, Monster.GoblinSoldier];

		new Fight([player], monsters).Simulate();

		Logging.LogInfo($"HP: {player.CurrentHp} | EXP: {player.LevelInfo.Exp}");
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
