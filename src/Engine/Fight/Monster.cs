namespace Engine;

public class Monster : Fighter
{
	public static Monster SpiderWhelp => new(hp: 2, damage: 2, def: 0, expReward: 1);
	public static Monster SpiderScout => new(hp: 6, damage: 4, def: 0, expReward: 2);
	public static Monster SpiderGuardian => new(hp: 24, damage: 4, def: 2, expReward: 4);
	public static Monster SpiderAmbusher => new(hp: 12, damage: 9, def: 1, expReward: 7);

	public static Monster Goblin => new(hp: 15, damage: 5, def: 0, expReward: 3);
	public static Monster GoblinSoldier => new(hp: 20, damage: 7, def: 1, expReward: 6);

	protected override int BaseDamage => _baseDamage;
	public override int ExpReward => _expReward;

	private readonly int _baseDamage;
	private readonly int _expReward;

	private Monster(int hp, int damage, int def, int expReward)
	{
		CurrentHp = hp;
		_baseDamage = damage;
		Defence = def;
		_expReward = expReward;
	}
}
