namespace Engine;

public class Fortress
{
	public static readonly Fortress Southern = new()
	{
		Id = FortressId.South,
		Name = "Southern Fortress",
		Buff = buff => buff.MaxHp += 15,
	};

	public static readonly Fortress Northern = new()
	{
		Id = FortressId.North,
		Name = "Northern Fortress",
		Buff = buff => buff.Attack += 1,
	};

	public static readonly Fortress Western = new()
	{
		Id = FortressId.West,
		Name = "Western Fortress",
		Buff = buff => buff.Defence += 1,
	};

	public required FortressId Id { get; init; }
	public required string Name { get; init; }
	public required Action<FortressBuff> Buff { get; init; }

	public Race Holder { get; set; } = Race.None;
}
