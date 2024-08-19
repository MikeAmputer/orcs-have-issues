using System.Text;
using System.Text.RegularExpressions;
using Octokit;

namespace Engine;

public class ServerState
{
	public static readonly ServerState Instance = new();

	public IEnumerable<Fortress> FortressesUnderControl(Race race) =>
		_fortresses.Where(f => f.Holder == race);

	public Dictionary<FortressId, Race> FortressHolders =>
		_fortresses.ToDictionary(f => f.Id, f => f.Holder);

	public int? IssueNumber => _stateIssueNumber;

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
			var winner = siege.Simulate(fortress.Holder);

			result.Add(fortressId, winner);

			if (winner != fortress.Holder)
			{
				Logs.AppendLine($"`{fortress.Name}` has been captured by the `{winner.ToString()}s`");
			}
		}

		ApplySiegeWinners(result);

		return result;
	}

	public async Task Initialize(IGitHubClient gitHubClient, Repository gitHubRepository, DateTimeOffset? utcNow = null)
	{
		if (utcNow != null)
		{
			Logs.AppendLine($"Processed at: `{utcNow}`");
		}

		var stateIssue = await LoadServerStateIssue(gitHubClient.Issue, gitHubRepository);
		_stateIssueNumber = stateIssue?.Number;

		var dto = stateIssue.ToServerStateDto();

		foreach (var (fortressId, race) in dto.FortressHolders)
		{
			_fortresses.Single(f => f.Id == fortressId).Holder = race;
		}

		RevaluateBuffs();
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

	private static readonly Regex ServerStateIssueRegex = new(
		"^server state",
		RegexOptions.IgnoreCase | RegexOptions.Compiled);

	private async Task<Issue?> LoadServerStateIssue(IIssuesClient issuesClient, Repository repository)
	{
		Logging.LogInfo("Requesting server state issue");

		var issues = await issuesClient.GetAllForRepository(
			repository.Id,
			new RepositoryIssueRequest
			{
				Filter = IssueFilter.Created,
				State = ItemStateFilter.Open,
				SortProperty = IssueSort.Updated,
				SortDirection = SortDirection.Descending,
			});

		Logging.LogInfo("Issues retrieved");
		Logging.LogGitHubClientState();

		if (issues.Count != 0)
		{
			foreach (var issue in issues)
			{
				if (!ServerStateIssueRegex.IsMatch(issue.Title))
				{
					continue;
				}

				Logging.LogInfo($"Server state issue #{issue.Number}");

				return issue;
			}
		}

		Logging.LogInfo($"Server state issue not found");

		return null;
	}
}
