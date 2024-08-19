namespace Engine;

public class ActionLevelUp : ActionBase<LevelUpSelection, EnumActionParametersProvider<LevelUpSelection>>
{
	protected override bool IsAvailable(Character character)
	{
		return character.IsLevelUpAvailable;
	}

	protected override ActionReport ExecuteCore(LevelUpSelection parameters, Character character) =>
		parameters switch
		{
			LevelUpSelection.None => ActionReport.Empty,
			LevelUpSelection.Hp => LevelUpHp(character),
			LevelUpSelection.Atk => LevelUpPlusOne(LevelUpSelection.Atk, character),
			LevelUpSelection.Def => LevelUpPlusOne(LevelUpSelection.Def, character),
			_ => throw new ArgumentOutOfRangeException(nameof(parameters), parameters, null)
		};

	private ActionReport LevelUpHp(Character character)
	{
		if (!character.CanLevelUpHp)
		{
			return ActionReport.FromMessage(
				$"Unable to level-up HP value, max value (${Character.MaxHpLevelUps}) is reached");
		}

		var oldHp = character.MaxHp;
		character.ApplyLevelUpSelection(LevelUpSelection.Hp);
		return ActionReport.FromMessage(ComposeLogMessage(LevelUpSelection.Hp, character.MaxHp - oldHp));
	}

	private ActionReport LevelUpPlusOne(LevelUpSelection selection, Character character)
	{
		character.ApplyLevelUpSelection(selection);
		return ActionReport.FromMessage(ComposeLogMessage(selection, 1));
	}

	private string ComposeLogMessage(LevelUpSelection selection, int delta)
	{
		return $"Level-up selection made: `+{delta} {selection.ToString().ToUpper()}`";
	}
}
