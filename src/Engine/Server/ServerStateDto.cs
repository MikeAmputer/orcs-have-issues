using System.Text.Json;

namespace Engine;

[Serializable]
public class ServerStateDto
{
	public Dictionary<FortressId, Race> FortressHolders { get; set; } = new();

	public static ServerStateDto FromServerState(ServerState state)
	{
		return new()
		{
			FortressHolders = state.FortressHolders,
		};
	}

	public override string ToString()
	{
		return JsonSerializer.Serialize(this);
	}
}
