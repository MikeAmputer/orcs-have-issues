namespace Engine.Tests.Actions;

[TestClass]
public class CraftTests
{
	[TestMethod]
	public void RequirementsMet_CraftWeapon_Crafted()
	{
		var character = CreateCharacter(1000, 1000, 0, 0);

		var report = ActionRunner.Execute("craft weapon", character);

		Assert.IsTrue(report.IsExecuted);
		Assert.AreEqual(1, character.WeaponRank);
		Assert.AreEqual(0, character.ArmorRank);
		Assert.AreEqual(950, character.Gold);
		Assert.AreEqual(970, character.Materials);
	}

	[TestMethod]
	public void RequirementsMet_CraftArmor_Crafted()
	{
		var character = CreateCharacter(1000, 1000, 0, 0);

		var report = ActionRunner.Execute("craft armor", character);

		Assert.IsTrue(report.IsExecuted);
		Assert.AreEqual(0, character.WeaponRank);
		Assert.AreEqual(1, character.ArmorRank);
		Assert.AreEqual(950, character.Gold);
		Assert.AreEqual(970, character.Materials);
	}

	[TestMethod]
	[DataRow("craft weapon")]
	[DataRow("craft armor")]
	public void RequirementsNotMet_Craft_NotCrafted(string command)
	{
		var character = CreateCharacter(0, 0, 0, 0);

		var report = ActionRunner.Execute(command, character);

		Assert.IsTrue(report.IsExecuted);
		Assert.AreEqual(0, character.WeaponRank);
		Assert.AreEqual(0, character.ArmorRank);
		Assert.AreEqual(0, character.Gold);
		Assert.AreEqual(0, character.Materials);
	}

	[TestMethod]
	[DataRow("craft weapon")]
	[DataRow("craft armor")]
	public void MaxLevelReached_Craft_NotExecuted(string command)
	{
		var character = CreateCharacter(1000000, 1000000, 5, 5);

		var report = ActionRunner.Execute(command, character);

		Assert.IsFalse(report.IsExecuted);
	}

	private Character CreateCharacter(int gold, int mats, int weaponRank, int armorRank)
	{
		var dto = new CharacterDto
		{
			Gold = gold,
			Materials = mats,
			WeaponRank = weaponRank,
			ArmorRank = armorRank,
		};

		return new Character(new PlayerInfo(), dto);
	}
}
