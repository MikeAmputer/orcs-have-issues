using System.Text.Json;
using Octokit;

namespace Engine;

[Serializable]
public class CharacterDto
{
	public int Exp { get; set; } = 0;
	public int Gold { get; set; } = 0;
	public int Materials { get; set; } = 0;

	public int ArmorRank { get; set; } = 0;
	public int WeaponRank { get; set; } = 0;

	public LevelUpSelection[] LevelUps { get; set; } = [];

	public static CharacterDto FromCharacter(Character character)
	{
		return new()
		{
			Exp = character.LevelInfo.Exp,
			Gold = character.Gold,
			Materials = character.Materials,
			ArmorRank = character.ArmorRank,
			WeaponRank = character.WeaponRank,
			LevelUps = character.LevelUps.ToArray(),
		};
	}

	public override string ToString()
	{
		return JsonSerializer.Serialize(this);
	}
}
