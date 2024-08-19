﻿namespace Engine;

public class SiegeFight
{
	public readonly Dictionary<Race, List<Character>> Participants = new()
	{
		{ Race.Human, [] },
		{ Race.Orc, [] },
	};

	public Race Simulate(Race currentHolder)
	{
		if (Participants[Race.Orc].Count + Participants[Race.Human].Count == 0)
		{
			return currentHolder;
		}

		var orcContribution = Participants[Race.Orc].Sum(p => p.PrepareForSiege());
		var humanContribution = Participants[Race.Human].Sum(p => p.PrepareForSiege());

		var orcFighters = new List<Fighter>(Participants[Race.Orc]);
		var humanFighters = new List<Fighter>(Participants[Race.Human]);

		orcFighters.AddRange(GetOrcReinforcement(orcContribution));
		humanFighters.AddRange(GetHumanReinforcement(humanContribution));

		var orderByLevel = true;

		while (true)
		{
			Func<Fighter, int> orderKeySelector = orderByLevel
				? fighter => fighter.Level
				: fighter => fighter.GetHashCode();

			orderByLevel = !orderByLevel;

			var orcs = orcFighters
				.Where(orc => orc.CanFight)
				.OrderByDescending(orderKeySelector)
				.ToList();

			var humans = humanFighters
				.Where(hum => hum.CanFight)
				.OrderByDescending(orderKeySelector)
				.ToList();

			if (orcs.Count == 0)
			{
				return Race.Human;
			}

			if (humans.Count == 0)
			{
				return Race.Orc;
			}

			foreach (var fight in BatchFights(GetNextAttacker(currentHolder), orcs, humans))
			{
				fight.Simulate();
			}
		}
	}

	private const int MaxFightBatchSize = 5;

	private IEnumerable<Fight> BatchFights(
		Race firstAttacker,
		List<Fighter> orcs,
		List<Fighter> humans)
	{
		var fightersLeft = Math.Min(orcs.Count, humans.Count);
		var fightersTaken = 0;
		var attacker = firstAttacker;

		while (fightersLeft > 0)
		{
			var batchSize = Math.Min(MaxFightBatchSize, fightersLeft);
			if (fightersLeft is > 5 and <= 8)
			{
				batchSize = 4;
			}

			var orcFighters = orcs.Skip(fightersTaken).Take(batchSize).ToArray();
			var humanFighters = humans.Skip(fightersTaken).Take(batchSize).ToArray();

			if (attacker == Race.Orc)
			{
				yield return new Fight(orcFighters, humanFighters);
			}

			if (attacker == Race.Human)
			{
				yield return new Fight(humanFighters, orcFighters);
			}

			fightersLeft -= batchSize;
			fightersTaken += batchSize;
			attacker = GetNextAttacker(attacker);
		}
	}

	private Race _defaultAttacker = Race.Human;

	private Race NextDefaultAttacker
	{
		get
		{
			_defaultAttacker = GetNextAttacker(_defaultAttacker);
			return _defaultAttacker;
		}
	}

	private Race GetNextAttacker(Race forRace) => forRace switch
	{
		Race.None => NextDefaultAttacker,
		Race.Human => Race.Orc,
		Race.Orc => Race.Human,
		_ => throw new ArgumentOutOfRangeException(nameof(forRace), forRace, null)
	};

	private static readonly List<(int pointsRequirement, Func<Mercenary> orcs, Func<Mercenary> humans)> Mercs =
	[
		(2, () => Mercenary.OrcRaider, () => Mercenary.HumanMilitia),
		(4, () => Mercenary.OrcHeadhunter, () => Mercenary.HumanFootman),
		(7, () => Mercenary.OrcBerserker, () => Mercenary.HumanKnight),
		(12, () => Mercenary.OrcBlademaster, () => Mercenary.HumanPaladin),
		(20, () => Mercenary.OrcChieftain, () => Mercenary.HumanCaptain),
	];

	private IEnumerable<Fighter> GetOrcReinforcement(int contributionPoints)
	{
		foreach (var tuple in Mercs)
		{
			for (var i = 0; i < contributionPoints / tuple.pointsRequirement; i++)
			{
				yield return tuple.orcs();
			}
		}
	}

	private IEnumerable<Fighter> GetHumanReinforcement(int contributionPoints)
	{
		foreach (var tuple in Mercs)
		{
			for (var i = 0; i < contributionPoints / tuple.pointsRequirement; i++)
			{
				yield return tuple.humans();
			}
		}
	}
}