namespace Engine;

public class ActionReport
{
	public static ActionReport Empty => new();

	public static ActionReport FromMessage(string message) => new()
	{
		IsExecuted = true,
		LogMessage = message,
	};

	public bool IsExecuted { get; private init; }
	public string LogMessage { get; private init; } = string.Empty;
}
