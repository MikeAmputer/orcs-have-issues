﻿namespace Engine;

public class Fight
{
	private readonly Dictionary<int, Fighter> _fighters = new();
	private readonly List<int> _attacksOrder = new();

	private readonly HashSet<int> _aliveAttackers = new();
	private readonly HashSet<int> _aliveDefenders = new();

	private int _attackersAlive;
	private int _defendersAlive;

	private int _lastAttackedAttacker = -1;
	private int _lastAttackedDefender = -1;

	public Fight(Fighter[] attackers, Fighter[] defenders)
	{
		var attackersCount = attackers.Length;
		var defendersCount = defenders.Length;

		if (attackersCount == 0 || defendersCount == 0)
		{
			throw new ArgumentException("One of teams is empty");
		}

		var fighterId = 0;

		for (var i = 0; i < Math.Max(attackersCount, defendersCount); i++)
		{
			if (i < attackersCount)
			{
				var fighter = attackers[i];

				if (fighter.CanFight)
				{
					_fighters.Add(fighterId, fighter);
					_aliveAttackers.Add(fighterId);
					_attacksOrder.Add(fighterId);
					fighterId++;
				}
			}

			if (i < defendersCount)
			{
				var fighter = defenders[i];

				if (fighter.CanFight)
				{
					_fighters.Add(fighterId, fighter);
					_aliveDefenders.Add(fighterId);
					_attacksOrder.Add(fighterId);
					fighterId++;
				}
			}
		}

		_attackersAlive = _aliveAttackers.Count;
		_defendersAlive = _aliveDefenders.Count;
	}

	public bool Simulate()
	{
		foreach (var fighterId in _attacksOrder)
		{
			var fightFinished = false;

			if (_aliveAttackers.Contains(fighterId))
			{
				_lastAttackedDefender = GetTargetId(_aliveDefenders, _lastAttackedDefender);
				fightFinished = SimulateHit(fighterId, _lastAttackedDefender, false);
			}
			if (_aliveDefenders.Contains(fighterId))
			{
				_lastAttackedAttacker = GetTargetId(_aliveAttackers, _lastAttackedAttacker);
				fightFinished = SimulateHit(fighterId, _lastAttackedAttacker, true);
			}

			if (fightFinished)
			{
				return _attackersAlive > 0;
			}
		}

		return Simulate();
	}

	private bool FightFinished => _attackersAlive == 0 || _defendersAlive == 0;

	private bool SimulateHit(int sourceId, int targetId, bool targetIsAttacker)
	{
		var source = _fighters[sourceId];
		var target = _fighters[targetId];

		target.TakeDamage(source);

		if (target.CanFight)
		{
			return false;
		}

		if (targetIsAttacker)
		{
			_aliveAttackers.Remove(targetId);
			_attackersAlive--;
		}
		else
		{
			_aliveDefenders.Remove(targetId);
			_defendersAlive--;
		}

		return FightFinished;

	}

	private static int GetTargetId(HashSet<int> targets, int lastAttacked)
	{
		foreach (var target in targets)
		{
			if (target > lastAttacked)
			{
				return target;
			}
		}

		return targets.First();
	}
}
