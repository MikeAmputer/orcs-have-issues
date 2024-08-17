using System.Text;

namespace Engine;

public sealed partial class ActionFightInstance : ActionBase<int, TimesActionParametersProvider>
{
	private readonly string _name;

	private readonly int _actionPointsCost;
	private readonly int _goldReward;
	private readonly int _materialsReward;

	private readonly Func<Fighter[]> _enemies;

	private ActionFightInstance(
		string name,
		int actionPointsCost,
		int goldReward,
		int materialsReward,
		Func<Fighter[]> enemies)
	{
		_name = name;
		_actionPointsCost = actionPointsCost;
		_goldReward = goldReward;
		_materialsReward = materialsReward;
		_enemies = enemies;
	}

	protected override bool IsAvailable(Character character)
	{
		return character.CurrentAp >= _actionPointsCost
			&& character.CanFight;
	}

	protected override ActionReport ExecuteCore(int times, Character character)
	{
		var startingHp = character.CurrentHp;
		var startingAp = character.CurrentAp;
		var startingExp = character.LevelInfo.Exp;

		var wins = 0;
		for (var i = 0; i < times; i++)
		{
			var fight = new Fight([character], _enemies());
			if (fight.Simulate())
			{
				wins++;
			}

			character.SpendAp(_actionPointsCost);

			if (!IsAvailable(character))
			{
				break;
			}
		}

		var logMessage = ProcessResults(character, wins, startingHp, startingAp, startingExp);
		return ActionReport.FromMessage(logMessage);
	}

	private string ProcessResults(Character character, int wins, int startingHp, int startingAp, int startingExp)
	{
		var deltaExp = character.LevelInfo.Exp - startingExp;
		var deltaGold = wins * _goldReward;
		var deltaMats = wins * _materialsReward;
		var deltaHp = startingHp - character.CurrentHp;
		var deltaAp = startingAp - character.CurrentAp;

		character.AddReward(0, deltaGold, deltaMats);

		var sb = new StringBuilder($"Fight {_name}");

		if (wins > 1)
		{
			sb.Append($" ({wins} wins)");
		}

		if (wins == 0)
		{
			sb.Append(" (defeat)");
		}

		if (deltaExp > 0)
		{
			sb.Append($" `+{deltaExp} Exp`");
		}

		if (deltaGold > 0)
		{
			sb.Append($" `+{deltaGold} Gold`");
		}

		if (deltaMats > 0)
		{
			sb.Append($" `+{deltaMats} Materials`");
		}

		sb.Append($" `-{deltaHp} HP`");
		sb.Append($" `-{deltaAp} AP`");

		return sb.ToString();
	}
}
