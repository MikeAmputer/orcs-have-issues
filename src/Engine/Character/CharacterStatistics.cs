namespace Engine;

[Serializable]
public class CharacterStatistics
{
	public int CyclesPlayed { get; set; }
	public int Level { get; set; }
	public Dictionary<Enemy, int> Kills { get; set; } = new();
}
