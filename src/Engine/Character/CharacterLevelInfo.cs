namespace Engine;

public class CharacterLevelInfo
{
	public int Level { get; private set; }
	public int Exp { get; private set; }
	public int CurrentLevelExp { get; private set; }

	public int UntilNextLevel => ExpRequirements[Level + 1] - CurrentLevelExp;

	private int _nextLevelRequirement = ExpRequirements.First().Value;
	private readonly int _maxLevel = ExpRequirements.Last().Key;

	public CharacterLevelInfo(int exp)
	{
		AddExp(exp);
	}

	public void AddExp(int exp)
	{
		Exp += exp;
		CurrentLevelExp += exp;

		while (CurrentLevelExp >= _nextLevelRequirement && Level < _maxLevel)
		{
			Level++;
			CurrentLevelExp -= _nextLevelRequirement;
			_nextLevelRequirement = ExpRequirements.GetValueOrDefault(Level + 1, -1);
		}
	}

	public static int TotalExpForLevel(int level) => ExpRequirements.Take(level).Sum(kvp => kvp.Value);

	private static readonly Dictionary<int, int> ExpRequirements = new()
	{
		{ 1, 25 },
		{ 2, 40 },
		{ 3, 55 },
		{ 4, 80 },
		{ 5, 115 },
		{ 6, 170 },
		{ 7, 290 },
		{ 8, 450 },
		{ 9, 700 },
		{ 10, 1100 },
		{ 11, 1700 },
		{ 12, 2600 },
		{ 13, 4000 },
		{ 14, 6000 },
		{ 15, 12000 },
		{ 16, 24000 },
		{ 17, 50000 },
		{ 18, 90000 },
		{ 19, 150000 },
		{ 20, 250000 },
	};
}
