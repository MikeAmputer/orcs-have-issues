namespace Engine;

public class Fortress
{
	public static readonly Fortress Southern = new()
	{
		Id = FortressId.South,
		Name = "Southern Fortress",
		Buff = buff => buff.MaxHp += 15,
		BuffDescription = "+15 HP",
	};

	public static readonly Fortress Northern = new()
	{
		Id = FortressId.North,
		Name = "Northern Fortress",
		Buff = buff => buff.Attack += 1,
		BuffDescription = "+1 ATK",
	};

	public static readonly Fortress Western = new()
	{
		Id = FortressId.West,
		Name = "Western Fortress",
		Buff = buff => buff.Defence += 1,
		BuffDescription = "+1 DEF",
	};

	public static string GetName(FortressId id) => id switch
	{
		FortressId.South => Southern.Name,
		FortressId.North => Northern.Name,
		FortressId.West => Western.Name,
		_ => throw new ArgumentOutOfRangeException(nameof(id), id, null)
	};

	public required FortressId Id { get; init; }
	public required string Name { get; init; }
	public required Action<FortressBuff> Buff { get; init; }
	public required string BuffDescription { get; init; }

	public Race Holder { get; set; } = Race.None;

	public override string ToString()
	{
		return Holder != Race.None
			? $"**_{Name} - {Holder}s_** `{BuffDescription}` `-10% Costs`"
			: $"**_{Name} - Empty_**";
	}
}
