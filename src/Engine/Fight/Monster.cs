namespace Engine;

public class Monster : Fighter
{
	public static Monster Spiderling => new(hp: 2, damage: 2, def: 0, expReward: 1);
	public static Monster SpiderScout => new(hp: 6, damage: 4, def: 0, expReward: 2);
	public static Monster SpiderGuardian => new(hp: 24, damage: 4, def: 2, expReward: 4);
	public static Monster SpiderAmbusher => new(hp: 12, damage: 9, def: 1, expReward: 7);
	public static Monster SpiderStinger => new(hp: 30, damage: 10, def: 2, expReward: 13);
	public static Monster SpiderTemplar => new(hp: 42, damage: 14, def: 3, expReward: 21);
	// public static Monster SpiderMatriarch => new(hp: 0, damage: 0, def: 0, expReward: 0);

	public static Monster GoblinScavenger => new(hp: 15, damage: 5, def: 0, expReward: 3);
	public static Monster GoblinHunter => new(hp: 20, damage: 7, def: 1, expReward: 6);
	public static Monster GoblinBrute => new(hp: 29, damage: 7, def: 2, expReward: 9);

	public static Monster BanditMugger => new(hp: 21, damage: 6, def: 1, expReward: 6);
	public static Monster BanditMarauder => new(hp: 30, damage: 8, def: 2, expReward: 10);
	public static Monster BanditEnforcer => new(hp: 32, damage: 11, def: 3, expReward: 14);

	public static Monster UndeadSkeleton => new(hp: 29, damage: 7, def: 1, expReward: 8);
	public static Monster UndeadGhoul => new(hp: 37, damage: 11, def: 3, expReward: 16);
	public static Monster UndeadWraith => new(hp: 48, damage: 15, def: 4, expReward: 24);
	public static Monster UndeadLich => new(hp: 70, damage: 20, def: 4, expReward: 32);
	// public static Monster UndeadVampire => new(hp: 0, damage: 0, def: 0, expReward: 0);
	// public static Monster UndeadDeathKnight => new(hp: 0, damage: 0, def: 0, expReward: 0);
	// public static Monster UndeadBoneGolem => new(hp: 0, damage: 0, def: 0, expReward: 0);

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
		ArmorRank = def / 3;
	}
}
