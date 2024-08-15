namespace Engine;

public class CharacterLevelInfo
{
	public int Level { get; private set; } = 0;
	public int Exp { get; private set; } = 0;
	public int CurrentLevelExp { get; private set; } = 0;

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

	private static readonly Dictionary<int, int> ExpRequirements = new()
	{
		{ 1, 25 },
		{ 2, 40 },
		{ 3, 55 },
		{ 4, 75 },
		{ 5, 100 },
		{ 6, 150 },
		{ 7, 250 },
		{ 8, 400 },
		{ 9, 600 },
		{ 10, 900 },
		{ 11, 1400 },
		{ 12, 2000 },
		{ 13, 3000 },
		{ 14, 5000 },
		{ 15, 9000 },
		{ 16, 15000 },
		{ 17, 25000 },
		{ 18, 45000 },
		{ 19, 75000 },
		{ 20, 120000 },
	};
}
