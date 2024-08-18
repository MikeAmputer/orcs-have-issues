namespace Engine;

public class ActionCraftArmor : ActionBase<object?, EmptyActionParametersProvider>
{
	protected override bool IsAvailable(Character character)
	{
		return character.ArmorRank < 5;
	}

	protected override ActionReport ExecuteCore(object? _, Character character)
	{
		var rankToCraft = character.ArmorRank + 1;
		var (gold, mats) = Requirements[rankToCraft];

		if (!character.CraftArmor(gold, mats))
		{
			return ActionReport.FromMessage(
				$"Insufficient resources to craft next armor: `Gold: {gold}` `Materials: {mats}`");
		}

		return ActionReport.FromMessage(
			$"Crafted new armor: `Armor Rank: {character.ArmorRank}` `-{gold} Gold` `-{mats} Materials`");
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
