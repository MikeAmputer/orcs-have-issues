namespace Engine;

[Serializable]
public class CharacterStatistics
{
	public int CyclesPlayed { get; set; }
	public int Level { get; set; }
	public int Sieges { get; set; }
	public int SiegesWon { get; set; }
	public int SiegeContribution { get; set; }
	public int GoldEarned { get; set; }
	public Dictionary<Enemy, int> Kills { get; set; } = new();
}
