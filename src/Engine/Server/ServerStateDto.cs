using System.Text.Json;

namespace Engine;

[Serializable]
public class ServerStateDto
{
	public Dictionary<FortressId, Race> FortressHolders { get; set; } = new();
	public List<LeaderboardEntryDto> Leaderboard { get; set; } = [];

	public static ServerStateDto FromServerState(ServerState state)
	{
		return new()
		{
			FortressHolders = state.FortressHolders,
			Leaderboard = state.GetLeaderboard(20).ToList(),
		};
	}

	public override string ToString()
	{
		return JsonSerializer.Serialize(this);
	}
}
