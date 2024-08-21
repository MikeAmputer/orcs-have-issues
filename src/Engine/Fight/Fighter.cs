namespace Engine;

public abstract class Fighter
{
	protected abstract int BaseDamage { get; }

	public virtual int Level => 0;

	public int Attack { get; protected set; }
	public int Defence { get; protected set; }

	public int ArmorRank { get; protected set; }
	public int WeaponRank { get; protected set; }

	public int CurrentHp { get; protected set; }

	public bool CanFight => CurrentHp > 0;

	private BattleTracker? _battleTracker;

	public int Damage => (int) Math.Round(Attack * (1.6 + WeaponRank * 0.1) + BaseDamage);

	public virtual int ExpReward => 0;

	public void TakeDamage(Fighter source)
	{
		var damage = source.Damage;

		if (damage <= 0)
		{
			return;
		}

		var postMitigationDamage = (int) (damage * damage / (damage + Math.Pow(Defence, 1.2 + ArmorRank * 0.1)));
		var mitigated = damage - postMitigationDamage;

		var preHitHp = CurrentHp;
		CurrentHp -= postMitigationDamage;
		CurrentHp = Math.Max(0, CurrentHp);

		var hpDelta = preHitHp - CurrentHp;

		source.Track(t => t.DamageProduced += damage);
		source.Track(t => t.DamageDealt += postMitigationDamage);
		if (!CanFight)
		{
			source.Track(t => t.Kills++);
			source.Track(t => t.ExpEarned += ExpReward);
		}

		Track(t => t.DamageTaken += postMitigationDamage);
		Track(t => t.DamageMitigated += mitigated);
		Track(t => t.HealthDelta += hpDelta);
	}

	public void StartBattleTracker()
	{
		_battleTracker = new();
	}

	public BattleTracker StopBattleTracker()
	{
		var tracker = _battleTracker;
		_battleTracker = null;

		return tracker ?? new();
	}

	public virtual void ScoreFrag(Fighter target) { }

	private void Track(Action<BattleTracker> trackerAction)
	{
		if (_battleTracker != null)
		{
			trackerAction(_battleTracker);
		}
	}
}
