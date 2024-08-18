namespace Engine;

public class ServerState
{
	public static readonly ServerState Instance = new();

	public IEnumerable<Fortress> FortressesUnderControl(Race race) =>
		_fortresses.Where(f => f.Holder == race);

	public int MaterialsPriceFor(Race race) =>
		MaterialsBasePrice - _fortresses.Count(f => f.Holder == race);

	public FortressBuff? GetFortressBuffFor(Race race) => race switch
	{
		Race.None => null,
		Race.Human => _humanBuff,
		Race.Orc => _orcBuff,
		_ => throw new ArgumentOutOfRangeException(nameof(race), race, null)
	};

	public void Initialize()
	{
		// get dto and set _fortresses
		_orcBuff = new();
		FortressesUnderControl(Race.Orc).ToList().ForEach(f => f.Buff(_orcBuff));

		_humanBuff = new();
		FortressesUnderControl(Race.Human).ToList().ForEach(f => f.Buff(_humanBuff));
	}

	private const int MaterialsBasePrice = 10;

	private FortressBuff _orcBuff = new();
	private FortressBuff _humanBuff = new();

	private readonly IReadOnlyList<Fortress> _fortresses = new[]
	{
		Fortress.Southern,
		Fortress.Northern,
		Fortress.Western
	};
}
