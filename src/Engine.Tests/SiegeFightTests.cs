using System.Text;

namespace Engine.Tests;

[TestClass]
public class SiegeFightTests
{
	[TestMethod]
	[DataRow(Race.None), DataRow(Race.Human), DataRow(Race.Orc)]
	public void NoParticipants_SameHolder(Race race)
	{
		var siege = new SiegeFight();

		var winner = siege.Simulate(race, new StringBuilder());

		Assert.AreEqual(race, winner);
	}

	[TestMethod]
	[DataRow(Race.Human, Race.None), DataRow(Race.Orc, Race.None)]
	[DataRow(Race.Human, Race.Human), DataRow(Race.Orc, Race.Human)]
	[DataRow(Race.Human, Race.Orc), DataRow(Race.Orc, Race.Orc)]
	public void OnlyOneSideParticipants_Wins(Race expectedWinner, Race holder)
	{
		var siege = new SiegeFight();

		switch (expectedWinner)
		{
			case Race.Human:
				siege.Participants[Race.Human].AddRange(CreateCharacters(1, Race.Human));

				break;
			case Race.Orc:
				siege.Participants[Race.Orc].AddRange(CreateCharacters(1, Race.Orc));

				break;
			default:
				throw new ArgumentOutOfRangeException(nameof(expectedWinner), expectedWinner, null);
		}

		var winner = siege.Simulate(holder, new StringBuilder());

		Assert.AreEqual(expectedWinner, winner);
	}

	[TestMethod]
	[DataRow(1, 1), DataRow(1, 6), DataRow(6, 1), DataRow(6, 6)]
	public void BothSideParticipants_HasWinner(int orcs, int humans)
	{
		var siege = new SiegeFight();

		siege.Participants[Race.Human].AddRange(CreateCharacters(humans, Race.Human));
		siege.Participants[Race.Orc].AddRange(CreateCharacters(orcs, Race.Orc));

		var winner = siege.Simulate(Race.None, new StringBuilder());

		Assert.AreNotEqual(Race.None, winner);
	}

	private IEnumerable<Character> CreateCharacters(int count, Race race)
	{
		var dto = new CharacterDto
		{
			Race = race,
		};

		for (var i = 0; i < count; i++)
		{
			yield return new Character(new PlayerInfo(), dto);
		}
	}
}
