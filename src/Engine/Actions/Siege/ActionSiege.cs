namespace Engine;

public class ActionSiege : ActionBase<FortressId, EnumActionParametersProvider<FortressId>>
{
	protected override bool IsAvailable(Character character)
	{
		return !character.IsSiegeParticipant
			&& character.Race != Race.None;
	}

	protected override ActionReport ExecuteCore(FortressId fortressId, Character character)
	{
		ServerState.Instance.AddSiegeParticipant(fortressId, character);

		var holdsFortress = ServerState.Instance
			.FortressesUnderControl(character.Race)
			.Any(f => f.Id == fortressId);

		var siegeNoun = holdsFortress ? "defence" : "siege";

		return ActionReport.FromMessage(
			$"You signed up for the **_{Fortress.GetName(fortressId)}_** {siegeNoun}");
	}
}
