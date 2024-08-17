using System.Text;

namespace Engine;

public partial class ActionLaborInstance : ActionBase<int, TimesActionParametersProvider>
{
	private readonly string _name;

	private readonly int _requiredLevel;
	private readonly int _actionPointsCost;
	private readonly int _goldReward;
	private readonly int _materialsReward;

	private ActionLaborInstance(
		string name,
		int requiredLevel,
		int actionPointsCost,
		int goldReward,
		int materialsReward)
	{
		_name = name;
		_requiredLevel = requiredLevel;
		_actionPointsCost = actionPointsCost;
		_goldReward = goldReward;
		_materialsReward = materialsReward;
	}

	protected override bool IsAvailable(Character character)
	{
		return character.CurrentAp >= _actionPointsCost
			&& character.LevelInfo.Level >= _requiredLevel;
	}

	protected override ActionReport ExecuteCore(int times, Character character)
	{
		var startingAp = character.CurrentAp;

		var workTimes = 0;
		for (var i = 0; i < times; i++)
		{
			character.SpendAp(_actionPointsCost);
			workTimes++;

			if (!IsAvailable(character))
			{
				break;
			}
		}

		return ActionReport.FromMessage(ProcessResults(character, workTimes, startingAp));
	}

	private string ProcessResults(Character character, int times, int startingAp)
	{
		var deltaGold = times * _goldReward;
		var deltaMats = times * _materialsReward;
		var deltaAp = startingAp - character.CurrentAp;

		character.AddReward(0, deltaGold, deltaMats);

		var sb = new StringBuilder($"Your hard work at the {_name} was paid off:");

		if (deltaGold > 0)
		{
			sb.Append($" `+{deltaGold} Gold`");
		}

		if (deltaMats > 0)
		{
			sb.Append($" `+{deltaMats} Materials`");
		}

		sb.Append($" `-{deltaAp} AP`");

		return sb.ToString();
	}
}
