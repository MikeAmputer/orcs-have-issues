namespace Engine;

public class Monster : Fighter
{
	public static Monster Goblin => new(hp: 15, atk: 1, def: 1, weaponRank: 0, armorRank: 0);
	public static Monster GoblinSoldier => new(hp: 20, atk: 2, def: 1, weaponRank: 1, armorRank: 1);

	private Monster(int hp, int atk, int def, int weaponRank, int armorRank)
	{
		CurrentHp = hp;
		Attack = atk;
		Defence = def;
		WeaponRank = weaponRank;
		ArmorRank = armorRank;
	}
}
