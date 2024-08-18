namespace Engine;

public class Character : Fighter
{
	public const int MaxHpLevelUps = 10;

	public PlayerInfo PlayerInfo { get; }

	public Race Race { get; private set; } = Race.None;

	public CharacterLevelInfo LevelInfo { get; }
	public int Gold { get; private set; }
	public int Materials { get; private set; }

	public List<LevelUpSelection> LevelUps { get; } = new();

	public int MaxHp { get; private set; } = 100;
	public int MaxAp { get; private set; } = 10;

	public int CurrentAp { get; private set; }

	protected override int BaseDamage => 4;

	public override int ExpReward => LevelInfo.Level * 2 + 5;

	private FortressBuff? _fortressBuff;

	public override void ScoreFrag(Fighter target)
	{
		LevelInfo.AddExp(target.ExpReward);
	}

	public Character(PlayerInfo playerInfo, CharacterDto dto)
	{
		PlayerInfo = playerInfo;

		SelectRace(dto.Race);

		LevelInfo = new CharacterLevelInfo(dto.Exp);
		Gold = dto.Gold;
		Materials = dto.Materials;

		foreach (var levelUp in dto.LevelUps)
		{
			ApplyLevelUpSelection(levelUp);
		}

		ApplyArmorRanks(dto.ArmorRank);
		ApplyWeaponRanks(dto.WeaponRank);

		ApplyReactions();
		ApplyStargazing();

		CurrentHp = MaxHp;
		CurrentAp = MaxAp;
	}

	public void SelectRace(Race race)
	{
		if (Race != Race.None)
		{
			return;
		}

		DisableFortressBuff();
		Race = race;
		_fortressBuff = ServerState.Instance.GetFortressBuffFor(Race);

		switch (Race)
		{
			case Race.None:
				return;
			case Race.Human:
				Defence++;

				break;
			case Race.Orc:
				Attack++;

				break;
			default:
				throw new ArgumentOutOfRangeException();
		}

		EnableFortressBuff();
	}

	public bool IsLevelUpAvailable => LevelInfo.Level > LevelUps.Count;

	public bool CanLevelUpHp => LevelUps.Count(selection => selection == LevelUpSelection.Hp) < MaxHpLevelUps;

	public void AddReward(int exp, int gold, int materials)
	{
		LevelInfo.AddExp(exp);
		Gold += gold;
		Materials += materials;
	}

	public void SpendAp(int ap)
	{
		CurrentAp -= ap;
	}

	private int _lvlUpHp = 0;

	public void ApplyLevelUpSelection(LevelUpSelection levelUp)
	{
		switch (levelUp)
		{
			case LevelUpSelection.None:
				break;
			case LevelUpSelection.Hp:
				_lvlUpHp++;
				if (_lvlUpHp <= MaxHpLevelUps)
				{
					var delta = MaxHpLevelUps + 1 - _lvlUpHp;
					MaxHp += delta;
					CurrentHp += delta;;
				}

				break;
			case LevelUpSelection.Atk:
				Attack += 1;

				break;
			case LevelUpSelection.Def:
				Defence += 1;

				break;
			default:
				throw new ArgumentOutOfRangeException();
		}

		LevelUps.Add(levelUp);
	}

	public bool CraftWeapon(int gold, int mats) =>
		Craft(gold, mats, character => character.ApplyWeaponRanks(1));

	public bool CraftArmor(int gold, int mats) =>
		Craft(gold, mats, character => character.ApplyArmorRanks(1));

	private bool Craft(int gold, int mats, Action<Character> applyAction)
	{
		if (Gold < gold || Materials < mats)
		{
			return false;
		}

		Gold -= gold;
		Materials -= mats;
		applyAction(this);

		return true;
	}

	private void ApplyArmorRanks(int increment)
	{
		ArmorRank += increment;
		Defence += increment;
	}

	private void ApplyWeaponRanks(int increment)
	{
		WeaponRank += increment;
		Attack += increment;
	}

	private void ApplyReactions()
	{
		var reactions = PlayerInfo.IssueReactions > 100 ? 100 : PlayerInfo.IssueReactions;

		MaxHp += Math.Min(reactions, 20);
		Attack += reactions / 20;
		Defence += (reactions + 10) / 20;
		MaxAp += reactions / 50;
	}

	private void ApplyStargazing()
	{
		if (PlayerInfo.IsStargazer)
		{
			Attack++;
			Defence++;
		}
	}

	private void EnableFortressBuff()
	{
		Attack += _fortressBuff?.Attack ?? 0;
		Defence += _fortressBuff?.Defence ?? 0;
		MaxHp += _fortressBuff?.MaxHp ?? 0;
		CurrentHp += _fortressBuff?.MaxHp ?? 0;
	}

	private void DisableFortressBuff()
	{
		Attack -= _fortressBuff?.Attack ?? 0;
		Defence -= _fortressBuff?.Defence ?? 0;
		MaxHp -= _fortressBuff?.MaxHp ?? 0;
	}
}
