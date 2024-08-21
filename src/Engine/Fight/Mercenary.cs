namespace Engine;

public class Mercenary : Fighter
{
	public static Mercenary OrcRaider => new(level: 1, hp: 80, damage: 6, def: 1);
	public static Mercenary OrcHeadhunter => new(level: 3, hp: 90, damage: 11, def: 3);
	public static Mercenary OrcBerserker => new(level: 5, hp: 100, damage: 16, def: 5);
	public static Mercenary OrcBlademaster => new(level: 7, hp: 110, damage: 20, def: 7);
	public static Mercenary OrcChieftain => new(level: 10, hp: 130, damage: 26, def: 10);

	public static Mercenary HumanMilitia => new(level: 1, hp: 80, damage: 6, def: 1);
	public static Mercenary HumanFootman => new(level: 3, hp: 95, damage: 9, def: 4);
	public static Mercenary HumanKnight => new(level: 5, hp: 105, damage: 14, def: 6);
	public static Mercenary HumanPaladin => new(level: 7, hp: 115, damage: 18, def: 8);
	public static Mercenary HumanCaptain => new(level: 10, hp: 135, damage: 24, def: 11);

	protected override int BaseDamage => _baseDamage;
	public override int ExpReward => 5 + Level * 3;
	public override int Level => _level;

	private readonly int _baseDamage;
	private readonly int _level;

	private Mercenary(int level, int hp, int damage, int def)
	{
		CurrentHp = hp;
		_baseDamage = damage;
		Defence = def;
		_level = level;
	}
}
