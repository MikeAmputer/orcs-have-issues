namespace Engine;

public class ActionReport
{
	public static ActionReport Empty => new ActionReport();

	public static ActionReport FromMessage(string message) => new()
	{
		IsExecuted = true,
		LogMessage = message,
	};

	public bool IsExecuted { get; init; } = false;
	public string LogMessage { get; init; } = string.Empty;
}
