namespace Engine;

public partial class ActionLaborInstance
{
	public static readonly ActionLaborInstance Mill = new(
		"Mill", requiredLevel: 0,
		actionPointsCost: 6, goldReward: 40, goldPerLevel: 2, materialsReward: 0, siegeContribution: 0);

	public static readonly ActionLaborInstance Workshop = new(
		"Workshop", requiredLevel: 1,
		actionPointsCost: 6, goldReward: 34, goldPerLevel: 1, materialsReward: 0, siegeContribution: 1);

	public static readonly ActionLaborInstance Farm = new(
		"Farm", requiredLevel: 2,
		actionPointsCost: 5, goldReward: 38, goldPerLevel: 1, materialsReward: 0, siegeContribution: 0);

	public static readonly ActionLaborInstance Lumberyard = new(
		"Lumberyard", requiredLevel: 5,
		actionPointsCost: 6, goldReward: 54, goldPerLevel: 2, materialsReward: 2, siegeContribution: 1);

	public static readonly ActionLaborInstance Quarry = new(
		"Quarry", requiredLevel: 6,
		actionPointsCost: 7, goldReward: 78, goldPerLevel: 3, materialsReward: 3, siegeContribution: 1);

	public static readonly ActionLaborInstance Forge = new(
		"Forge", requiredLevel: 7,
		actionPointsCost: 6, goldReward: 61, goldPerLevel: 2, materialsReward: 0, siegeContribution: 2);
}
