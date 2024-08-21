namespace Engine;

public class ActionCraftWeapon : ActionBase<object?, EmptyActionParametersProvider>
{
	protected override bool IsAvailable(Character character)
	{
		return character.WeaponRank < 5;
	}

	protected override ActionReport ExecuteCore(object? _, Character character)
	{
		var rankToCraft = character.WeaponRank + 1;
		var (gold, mats) = Requirements[rankToCraft];

		if (!character.CraftWeapon(gold, mats))
		{
			return ActionReport.FromMessage(
				$"Insufficient resources to craft next weapon: `Gold: {gold}` `Materials: {mats}`");
		}

		return ActionReport.FromMessage(
			$"Crafted new weapon: `Weapon Rank: {character.WeaponRank}` `-{gold} Gold` `-{mats} Materials`");
	}

	private static readonly Dictionary<int, (int gold, int mats)> Requirements = new()
	{
		{ 1, (50, 10) },
		{ 2, (150, 30) },
		{ 3, (600, 90) },
		{ 4, (1500, 180) },
		{ 5, (5000, 400) },
		{ 6, (100000, 100000) },
	};
}
