namespace Engine;

public class Monster : Fighter
{
	public static Monster Spiderling => new(Enemy.Spider, hp: 2, damage: 2, def: 0, expReward: 1);
	public static Monster SpiderScout => new(Enemy.Spider, hp: 6, damage: 4, def: 0, expReward: 2);
	public static Monster SpiderGuardian => new(Enemy.Spider, hp: 24, damage: 4, def: 2, expReward: 4);
	public static Monster SpiderAmbusher => new(Enemy.Spider, hp: 12, damage: 9, def: 1, expReward: 7);
	public static Monster SpiderStinger => new(Enemy.Spider, hp: 30, damage: 10, def: 2, expReward: 13);
	public static Monster SpiderTemplar => new(Enemy.Spider, hp: 42, damage: 14, def: 3, expReward: 21);
	// public static Monster SpiderMatriarch => new(Enemy.Spider, hp: 0, damage: 0, def: 0, expReward: 0);

	public static Monster GoblinScavenger => new(Enemy.Goblin, hp: 15, damage: 5, def: 0, expReward: 3);
	public static Monster GoblinHunter => new(Enemy.Goblin, hp: 20, damage: 7, def: 1, expReward: 6);
	public static Monster GoblinBrute => new(Enemy.Goblin, hp: 29, damage: 7, def: 2, expReward: 9);

	public static Monster BanditMugger => new(Enemy.Bandit, hp: 21, damage: 6, def: 1, expReward: 6);
	public static Monster BanditMarauder => new(Enemy.Bandit, hp: 30, damage: 8, def: 2, expReward: 10);
	public static Monster BanditEnforcer => new(Enemy.Bandit, hp: 32, damage: 11, def: 3, expReward: 14);

	public static Monster UndeadSkeleton => new(Enemy.Undead, hp: 29, damage: 7, def: 1, expReward: 8);
	public static Monster UndeadGhoul => new(Enemy.Undead, hp: 37, damage: 11, def: 3, expReward: 16);
	public static Monster UndeadWraith => new(Enemy.Undead, hp: 48, damage: 15, def: 4, expReward: 24);
	public static Monster UndeadLich => new(Enemy.Undead, hp: 70, damage: 20, def: 4, expReward: 32);
	// public static Monster UndeadVampire => new(Enemy.Undead, hp: 0, damage: 0, def: 0, expReward: 0);
	// public static Monster UndeadDeathKnight => new(Enemy.Undead, hp: 0, damage: 0, def: 0, expReward: 0);
	// public static Monster UndeadBoneGolem => new(Enemy.Undead, hp: 0, damage: 0, def: 0, expReward: 0);

	public override Enemy Type => _enemy;
	protected override int BaseDamage => _baseDamage;
	public override int ExpReward => _expReward;

	private readonly int _baseDamage;
	private readonly int _expReward;
	private readonly Enemy _enemy;

	private Monster(Enemy type, int hp, int damage, int def, int expReward)
	{
		_enemy = type;
		CurrentHp = hp;
		_baseDamage = damage;
		Defence = def;
		_expReward = expReward;
		ArmorRank = def / 3;
	}
}
