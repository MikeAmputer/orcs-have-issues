namespace Engine;

public sealed class ActionRaceSelection : ActionBase<Race, RaceActionParametersProvider>
{
	protected override bool IsAvailable(Character character)
	{
		return character.Race == Race.None;
	}

	protected override ActionReport ExecuteCore(Race race, Character character)
	{
		character.SelectRace(race);

		return ActionReport.FromMessage($"Race selected: `{race}`");
	}
}
