using System.Text;

namespace Engine;

public class Character : Fighter
{
	public override Enemy Type => Enemy.Player;
	public PlayerInfo PlayerInfo { get; }

	public Race Race { get; private set; }

	public CharacterLevelInfo LevelInfo { get; }
	public override int Level => LevelInfo.Level;

	public int Gold { get; private set; }
	public int Materials { get; private set; }

	public List<LevelUpSelection> LevelUps { get; } = new();

	public int MaxHp { get; private set; } = 100;
	public int MaxAp { get; private set; } = 10;

	public int CurrentAp { get; private set; }

	protected override int BaseDamage => 4;

	public override int ExpReward => 5 + Level * 3;

	private FortressBuff? _fortressBuff;

	public bool IsSiegeParticipant { get; private set; }

	public int SiegeContributionPoints { get; private set; }

	public StringBuilder Logs { get; } = new();

	public CharacterStatistics Statistics { get; }

	public Character(PlayerInfo playerInfo, CharacterDto dto, DateTimeOffset? utcNow = null)
	{
		if (utcNow != null)
		{
			Logs.AppendLine($"Processed at: `{utcNow}`");
		}

		PlayerInfo = playerInfo;
		Statistics = dto.Statistics;

		Race = playerInfo.IssueLabels.Contains(Race.Orc.ToString().ToLower())
			? Race.Orc
			: playerInfo.IssueLabels.Contains(Race.Human.ToString().ToLower())
				? Race.Human
				: Race.None;

		ApplyRaceBonuses();

		LevelInfo = new CharacterLevelInfo(dto.Exp);
		Gold = dto.Gold;
		Materials = dto.Materials;

		foreach (var levelUp in dto.LevelUps.Take(LevelInfo.Level))
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

	public bool IsLevelUpAvailable => LevelInfo.Level > LevelUps.Count;

	public void AddReward(int exp, int gold, int materials, int siegeContribution = 0)
	{
		LevelInfo.AddExp(exp);
		Gold += gold;
		Materials += materials;
		SiegeContributionPoints += siegeContribution;

		Statistics.GoldEarned += gold;
		Statistics.SiegeContribution += siegeContribution;
	}

	public void TradeMaterials(int price, int amount)
	{
		Gold -= price * amount;
		Materials += amount;
	}

	public void SpendAp(int ap)
	{
		CurrentAp -= ap;
	}

	public int ApplyLevelUpSelection(LevelUpSelection levelUp)
	{
		if (levelUp == LevelUpSelection.None)
		{
			return 0;
		}

		LevelUps.Add(levelUp);

		switch (levelUp)
		{
			case LevelUpSelection.Hp:
				MaxHp += 10;
				CurrentHp += 10;

				return 10;
			case LevelUpSelection.Atk:
				Attack += 1;

				return 1;
			case LevelUpSelection.Def:
				Defence += 1;

				return 1;
			default:
				throw new ArgumentOutOfRangeException(nameof(levelUp), levelUp, null);
		}
	}

	public void MarkSiegeParticipant() => IsSiegeParticipant = true;

	public int PrepareForSiege()
	{
		var startingHp = CurrentHp;

		var hpLvlUps = LevelUps.Count(l => l == LevelUpSelection.Hp);
		CurrentHp = Math.Min(MaxHp, CurrentHp + MaxHp / 5 + hpLvlUps * 4);
		StartBattleTracker();

		var hpRegained = CurrentHp - startingHp;

		Logs.Append("You are taking a short rest before the siege.");
		if (hpRegained > 0)
		{
			Logs.Append($" HP regained: `+{hpRegained} HP`.");
		}

		Logs.AppendLine($" Current HP: {CurrentHp}/{MaxHp}.");

		return SiegeContributionPoints;
	}

	public string? GetFortressBuffDescription(string title)
	{
		if (_fortressBuff == null || _fortressBuff.IsEmpty)
		{
			return null;
		}

		var sb = new StringBuilder(title);

		if (_fortressBuff.MaxHp > 0)
		{
			sb.Append($" `+{_fortressBuff.MaxHp} HP`");
		}

		if (_fortressBuff.Attack > 0)
		{
			sb.Append($" `+{_fortressBuff.Attack} ATK`");
		}

		if (_fortressBuff.Defence > 0)
		{
			sb.Append($" `+{_fortressBuff.Defence} DEF`");
		}

		return sb.ToString();
	}

	public void PrepareForSave()
	{
		DisableFortressBuff();
		_fortressBuff = ServerState.Instance.GetFortressBuffFor(Race);
		Statistics.CyclesPlayed++;
		Statistics.Level = Level;

		if (IsSiegeParticipant)
		{
			Statistics.Sieges++;
		}
	}

	protected override void ScoreFrag(Fighter target)
	{
		LevelInfo.AddExp(target.ExpReward);

		Statistics.Kills.TryAdd(target.Type, 0);
		Statistics.Kills[target.Type]++;
	}

	public void RewardSiegeWinner()
	{
		const int expReward = 5;

		Statistics.SiegesWon++;
		LevelInfo.AddExp(expReward);
		Track(t => t.ExpEarned += expReward);
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

	private void ApplyRaceBonuses()
	{
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

		MaxHp += Math.Min(reactions, 20) + 2 * (reactions / 10);
		Attack += reactions / 30;
		Defence += (reactions + 10) / 30;
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

		var buffsDescription = GetFortressBuffDescription("Fortress buffs applied:");

		if (buffsDescription != null)
		{
			Logs.AppendLine(buffsDescription);
		}
	}

	private void DisableFortressBuff()
	{
		Attack -= _fortressBuff?.Attack ?? 0;
		Defence -= _fortressBuff?.Defence ?? 0;
		MaxHp -= _fortressBuff?.MaxHp ?? 0;
	}
}
