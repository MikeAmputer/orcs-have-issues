namespace Engine.Tests.Actions;

[TestClass]
public class RaceSelectionTests
{
	[TestMethod]
	[DataRow("human", Race.Human)]
	[DataRow("orc", Race.Orc)]
	public void None_ToSpecified(string parameter, Race expected)
	{
		var character = CreateCharacter(Race.None);
		string[] parameters = [parameter];

		var action = new ActionRaceSelection();
		var report = action.Execute(parameters, character);

		Assert.IsTrue(report.IsExecuted);
		Assert.AreEqual(expected, character.Race);
	}

	[TestMethod]
	[DataRow("orc", Race.Human)]
	[DataRow("human", Race.Orc)]
	public void Specified_Unchanged(string parameter, Race specified)
	{
		var character = CreateCharacter(specified);
		string[] parameters = [parameter];

		var action = new ActionRaceSelection();
		var report = action.Execute(parameters, character);

		Assert.IsFalse(report.IsExecuted);
		Assert.AreEqual(specified, character.Race);
	}

	private Character CreateCharacter(Race race)
	{
		var dto = new CharacterDto
		{
			Race = race,
		};

		return new Character(new PlayerInfo(), dto);
	}
}
