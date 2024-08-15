namespace Engine;

public abstract class Fighter
{
	public int Attack { get; protected set; } = 0;
	public int Defence { get; protected set; } = 0;

	public int ArmorRank { get; protected set; } = 0;
	public int WeaponRank { get; protected set; } = 0;

	public int CurrentHp { get; protected set; }

	public bool CanFight => CurrentHp > 0;

	public int Damage => (int) (Attack * (2 + WeaponRank * 0.1) + 5);

	public void TakeDamage(int damage)
	{
		if (damage <= 0)
		{
			return;
		}

		CurrentHp -= (int) (damage * damage / (damage + Math.Pow(Defence, 1 + ArmorRank * 0.1)));

		CurrentHp = Math.Max(0, CurrentHp);
	}
}
