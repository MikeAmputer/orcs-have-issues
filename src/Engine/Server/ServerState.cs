using System.Collections.Concurrent;
using System.Text;
using Octokit;

namespace Engine;

public class ServerState
{
	public static readonly ServerState Instance = new();

	public IEnumerable<Fortress> FortressesUnderControl(Race race) =>
		_fortresses.Where(f => f.Holder == race);

	public Dictionary<FortressId, Race> FortressHolders =>
		_fortresses.ToDictionary(f => f.Id, f => f.Holder);

	public IReadOnlyList<Fortress> Fortresses => _fortresses;

	public int? IssueNumber => _stateIssueNumber;

	public ServerStatistics Statistics { get; private set; } = null!;

	public DateTime? LastCycleSimulation { get; private set; }

	public StringBuilder Logs { get; } = new();

	public int MaterialsPriceFor(Race race) =>
		MaterialsBasePrice - _fortresses.Count(f => f.Holder == race);

	public FortressBuff? GetFortressBuffFor(Race race) => race switch
	{
		Race.None => null,
		Race.Human => _humanBuff,
		Race.Orc => _orcBuff,
		_ => throw new ArgumentOutOfRangeException(nameof(race), race, null)
	};

	public void AddSiegeParticipant(FortressId fortressId, Character character)
	{
		if (character.Race == Race.None || character.IsSiegeParticipant)
		{
			return;
		}

		_sieges[fortressId].Participants[character.Race].Add(character);
		character.MarkSiegeParticipant();
	}

	public Dictionary<FortressId, Race> SimulateSiege()
	{
		var result = new Dictionary<FortressId, Race>();

		foreach (var (fortressId, siege) in _sieges)
		{
			var fortress = _fortresses.Single(f => f.Id == fortressId);

			if (fortress.Holder != Race.None)
			{
				Logs.AppendLine($"Siege of {fortress.Name}, held by {fortress.Holder}s, begins");
			}
			else
			{
				Logs.AppendLine($"Battle for {fortress.Name} begins");
			}

			var winner = siege.Simulate(fortress.Holder, Logs);

			result.Add(fortressId, winner);

			if (winner != fortress.Holder)
			{
				Logs.AppendLine($"{fortress.Name} has been captured by the {winner}s");
			}
			else if (fortress.Holder == Race.None)
			{
				Logs.AppendLine($"{fortress.Name} still belongs to nobody");
			}
			else
			{
				Logs.AppendLine($"{fortress.Name} still belongs to {fortress.Holder}s");
			}
		}

		ApplySiegeWinners(result);

		return result;
	}

	public async Task<DateTimeOffset> Initialize(
		IGitHubClient gitHubClient,
		Repository gitHubRepository,
		int? periodHours,
		DateTimeOffset utcNow)
	{
		Logs.AppendLine($"Processed at: `{utcNow}`");

		var stateIssue = await LoadServerStateIssue(gitHubClient.Issue, gitHubRepository);
		_stateIssueNumber = stateIssue?.Number;

		var dto = stateIssue.ToServerStateDto();

		foreach (var (fortressId, race) in dto.FortressHolders)
		{
			_fortresses.Single(f => f.Id == fortressId).Holder = race;
		}

		RevaluateBuffs();

		_leaderboard = new(
			dto.Leaderboard
				.ToDictionary(
					entry => entry.Login,
					entry => (entry.IssueNumber, entry.Exp)));

		Statistics = dto.Statistics;

		var since = periodHours == null
			? dto.LastCycleSimulation == null
				? utcNow.AddHours(-25)
				: new DateTimeOffset(dto.LastCycleSimulation.Value, TimeSpan.Zero)
			: utcNow.AddHours(-periodHours.Value);

		LastCycleSimulation = since.UtcDateTime;

		return since;
	}

	public IEnumerable<LeaderboardEntryDto> GetLeaderboard(int length)
	{
		return _leaderboard.Select(kvp => new LeaderboardEntryDto()
			{
				Login = kvp.Key,
				IssueNumber = kvp.Value.IssueNumber,
				Exp = kvp.Value.Exp,
			})
			.OrderByDescending(entry => entry.Exp)
			.Take(length);
	}

	public void PrepareForSave(IEnumerable<Character> characters)
	{
		UpdateLeaderboard(characters);
		Statistics.CyclesSimulated++;
		LastCycleSimulation = DateTime.UtcNow;
	}

	private ConcurrentDictionary<string, (int IssueNumber, int Exp)> _leaderboard = new();

	private void UpdateLeaderboard(IEnumerable<Character> characters)
	{
		foreach (var character in characters.Where(c => !c.PlayerInfo.IsBot))
		{
			_leaderboard.AddOrUpdate(
				character.PlayerInfo.UserLogin,
				(character.PlayerInfo.IssueNumber, character.LevelInfo.Exp),
				(_, _) => (character.PlayerInfo.IssueNumber, character.LevelInfo.Exp));
		}
	}

	private void ApplySiegeWinners(Dictionary<FortressId, Race> winners)
	{
		foreach (var (fortressId, race) in winners)
		{
			_fortresses.Single(f => f.Id == fortressId).Holder = race;
		}

		RevaluateBuffs();
	}

	private void RevaluateBuffs()
	{
		_orcBuff = new();
		FortressesUnderControl(Race.Orc).ToList().ForEach(f => f.Buff(_orcBuff));

		_humanBuff = new();
		FortressesUnderControl(Race.Human).ToList().ForEach(f => f.Buff(_humanBuff));
	}

	private const int MaterialsBasePrice = 10;

	private int? _stateIssueNumber;

	private FortressBuff _orcBuff = new();
	private FortressBuff _humanBuff = new();

	private readonly IReadOnlyList<Fortress> _fortresses = new List<Fortress>
	{
		Fortress.Southern,
		Fortress.Northern,
		Fortress.Western
	};

	private readonly IReadOnlyDictionary<FortressId, SiegeFight> _sieges = new Dictionary<FortressId, SiegeFight>
	{
		{ FortressId.South, new() },
		{ FortressId.North, new() },
		{ FortressId.West, new() },
	};

	private async Task<Issue?> LoadServerStateIssue(IIssuesClient issuesClient, Repository repository)
	{
		Logging.LogInfo("Requesting server state issue");

		var issues = await issuesClient.GetAllForRepository(
			repository.Id,
			new RepositoryIssueRequest
			{
				Filter = IssueFilter.Created,
				Labels = { "server" },
				State = ItemStateFilter.Open,
				SortProperty = IssueSort.Updated,
				SortDirection = SortDirection.Descending,
			});

		Logging.LogInfo("Issues retrieved");
		Logging.LogGitHubClientState();

		if (issues.Count == 0)
		{
			Logging.LogInfo("Server state issue not found");

			return null;
		}

		Logging.LogInfo($"Found {issues.Count} server state issues");

		return issues[0];
	}
}
