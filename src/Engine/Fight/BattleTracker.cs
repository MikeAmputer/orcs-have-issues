namespace Engine;

public class BattleTracker
{
	public int DamageProduced { get; set; }
	public int DamageDealt { get; set; }

	public int DamageTaken { get; set; }
	public int DamageMitigated { get; set; }
	public int HealthDelta { get; set; }

	public int ExpEarned { get; set; }
	public int Kills { get; set; }
}
