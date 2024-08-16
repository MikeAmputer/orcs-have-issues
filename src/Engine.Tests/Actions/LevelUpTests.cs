﻿namespace Engine.Tests.Actions;

[TestClass]
public class LevelUpTests
{
	[TestMethod]
	public void LevelUpUnavailable_Unsuccessful()
	{
		const int level = 0;
		var levelUpSelections = new Dictionary<LevelUpSelection, int>();
		var character = CreateCharacter(level, levelUpSelections);
		string[] parameters = [];

		var action = new ActionLevelUp();
		var report = action.Execute(parameters, character);

		Assert.IsFalse(report.IsExecuted);
		Assert.AreEqual(0, character.LevelUps.Count);
	}

	[TestMethod]
	public void LevelUpAvailable_LevelUpHp_Success()
	{
		const int level = 1;
		var levelUpSelections = new Dictionary<LevelUpSelection, int>();
		var character = CreateCharacter(level, levelUpSelections);
		string[] parameters = ["hp"];

		var action = new ActionLevelUp();
		var report = action.Execute(parameters, character);

		Assert.IsTrue(report.IsExecuted);
		Assert.AreEqual(1, character.LevelUps.Count);
		Assert.AreEqual(110, character.MaxHp);
		Assert.AreEqual(110, character.CurrentHp);
	}

	[TestMethod]
	public void TwoLevelUpsAvailable_LevelUpHpTwoTimes_HpDeltaScales()
	{
		const int level = 2;
		var levelUpSelections = new Dictionary<LevelUpSelection, int>();
		var character = CreateCharacter(level, levelUpSelections);
		string[] parameters = ["hp"];

		var action = new ActionLevelUp();
		action.Execute(parameters, character);
		action.Execute(parameters, character);

		Assert.AreEqual(2, character.LevelUps.Count);
		Assert.AreEqual(119, character.MaxHp);
		Assert.AreEqual(119, character.CurrentHp);
	}

	[TestMethod]
	public void LevelUpAvailable_MaxHpLevelUpsReached_LevelUpHp_NotApplied()
	{
		const int level = Character.MaxHpLevelUps + 1;
		var levelUpSelections = new Dictionary<LevelUpSelection, int>
		{
			{ LevelUpSelection.Hp, Character.MaxHpLevelUps },
		};
		var character = CreateCharacter(level, levelUpSelections);
		string[] parameters = ["hp"];

		var action = new ActionLevelUp();
		var report = action.Execute(parameters, character);

		Assert.IsTrue(report.IsExecuted);
		Assert.IsFalse(report.LogMessage.IsNullOrWhiteSpace());
		Assert.AreEqual(10, character.LevelUps.Count);
	}

	[TestMethod]
	public void LevelUpAvailable_LevelUpAtk_Success()
	{
		const int level = 1;
		var levelUpSelections = new Dictionary<LevelUpSelection, int>();
		var character = CreateCharacter(level, levelUpSelections);
		string[] parameters = ["atk"];

		var action = new ActionLevelUp();
		var report = action.Execute(parameters, character);

		Assert.IsTrue(report.IsExecuted);
		Assert.AreEqual(1, character.LevelUps.Count);
		Assert.AreEqual(1, character.Attack);
	}

	[TestMethod]
	public void LevelUpAvailable_LevelUpDef_Success()
	{
		const int level = 1;
		var levelUpSelections = new Dictionary<LevelUpSelection, int>();
		var character = CreateCharacter(level, levelUpSelections);
		string[] parameters = ["def"];

		var action = new ActionLevelUp();
		var report = action.Execute(parameters, character);

		Assert.IsTrue(report.IsExecuted);
		Assert.AreEqual(1, character.LevelUps.Count);
		Assert.AreEqual(1, character.Defence);
	}

	private Character CreateCharacter(int level, Dictionary<LevelUpSelection, int> levelUpSelections)
	{
		var dto = new CharacterDto
		{
			Exp = CharacterLevelInfo.TotalExpForLevel(level),
			LevelUps = levelUpSelections
				.SelectMany(kvp => Enumerable.Repeat(kvp.Key, kvp.Value))
				.ToArray()
		};

		return new Character(new PlayerInfo(), dto);
	}
}