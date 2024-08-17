namespace Engine;

public abstract class Fighter
{
	protected abstract int BaseDamage { get; }

	public int Attack { get; protected set; } = 0;
	public int Defence { get; protected set; } = 0;

	public int ArmorRank { get; protected set; } = 0;
	public int WeaponRank { get; protected set; } = 0;

	public int CurrentHp { get; protected set; }

	public bool CanFight => CurrentHp > 0;

	public int Damage => (int) Math.Round(Attack * (1.6 + WeaponRank * 0.1) + BaseDamage);

	public virtual int ExpReward => 0;

	public int TakeDamage(int damage)
	{
		if (damage <= 0)
		{
			return 0;
		}

		var postMitigation = (int) (damage * damage / (damage + Math.Pow(Defence, 1.2 + ArmorRank * 0.1)));

		CurrentHp -= postMitigation;

		CurrentHp = Math.Max(0, CurrentHp);

		return postMitigation;
	}

	public virtual void ScoreFrag(Fighter target) { }
}
