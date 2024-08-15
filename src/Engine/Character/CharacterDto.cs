using Octokit;

namespace Engine;

[Serializable]
public class CharacterDto
{
	public Race Race { get; set; } = Race.None;

	public int Exp { get; set; } = 0;
	public int Gold { get; set; } = 0;
	public int Materials { get; set; } = 0;

	public int ArmorRank { get; set; } = 0;
	public int WeaponRank { get; set; } = 0;

	public LevelUpSelection[] LevelUps { get; set; } = [];
}
