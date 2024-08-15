namespace Engine.Tests;

[TestClass]
public class CharacterLevelInfoTests
{
	[TestMethod]
	[DataRow(0, 0)]
	[DataRow(24, 0)]
	[DataRow(25, 1)]
	[DataRow(64, 1)]
	[DataRow(65, 2)]
	[DataRow(int.MaxValue, 20)]
	public void ExpToLevel(int exp, int level)
	{
		var levelInfo = new CharacterLevelInfo(exp);

		Assert.AreEqual(level, levelInfo.Level);
		Assert.AreEqual(exp, levelInfo.Exp);
	}

	[TestMethod]
	[DataRow(0, 0)]
	[DataRow(24, 24)]
	[DataRow(25, 0)]
	[DataRow(26, 1)]
	public void ExpToCurrentLevelExp(int exp, int currentLevelExp)
	{
		var levelInfo = new CharacterLevelInfo(exp);

		Assert.AreEqual(currentLevelExp, levelInfo.CurrentLevelExp);
	}
}
