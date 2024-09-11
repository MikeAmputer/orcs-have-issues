using Engine;

namespace BotController;

public static class ActionsSelector
{
	private static readonly Random StaticRandom = new((int) DateTime.UtcNow.Ticks % int.MaxValue);

	public static List<string> NextBestActions(Character character)
	{
		var result = new List<string>();

		if (character.IsLevelUpAvailable)
		{
			result.Add($"level-up {NextLvlUpAction()}");
		}

		result.Add($"labor {NextLaborAction(character)}");
		result.Add($"fight {NextFightAction(character)}");
		result.Add($"siege {NextSiegeAction()}");

		return result;
	}

	private static string NextLaborAction(Character character)
	{
		switch (character.Level)
		{
			case 0:
				return "mill";
			case > 1:
				return "workshop";
			default:
				return "mill";
		}
	}

	private static string NextFightAction(Character character)
	{
		switch (character.Attack + character.Defence)
		{
			case < 3:
				return "spider-lair";
			case < 4:
				return "goblin-camp";
			case < 5:
				return "bandit-hideout";
			default:
				return "spider-lair-elite";
		}
	}

	private static string NextSiegeAction()
	{
		switch (StaticRandom.Next(3))
		{
			case 0:
				return "west";
			case 1:
				return "south";
			default:
				return "north";
		}
	}

	private static string NextLvlUpAction()
	{
		switch (StaticRandom.Next(3))
		{
			case 0:
				return "atk";
			case 1:
				return "def";
			default:
				return "hp";
		}
	}
}
