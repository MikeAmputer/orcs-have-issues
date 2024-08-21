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
			LevelUpSelection.Hp => LevelUp(LevelUpSelection.Hp, character),
			LevelUpSelection.Atk => LevelUp(LevelUpSelection.Atk, character),
			LevelUpSelection.Def => LevelUp(LevelUpSelection.Def, character),
			_ => throw new ArgumentOutOfRangeException(nameof(parameters), parameters, null)
		};

	private ActionReport LevelUp(LevelUpSelection selection, Character character)
	{
		var delta = character.ApplyLevelUpSelection(selection);
		return ActionReport.FromMessage(ComposeLogMessage(selection, delta));
	}

	private string ComposeLogMessage(LevelUpSelection selection, int delta)
	{
		return $"Level-up selection made: `+{delta} {selection.ToString().ToUpper()}`";
	}
}
