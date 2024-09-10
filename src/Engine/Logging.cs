using System.Text;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Logging.Console;
using Octokit;

namespace Engine;

public static class Logging
{
	public static Func<RateLimit?>? RateLimitProvider { get; set; }

	private static readonly ILogger Logger;

	static Logging()
	{
		Logger = LoggerFactory
			.Create(builder => builder
				.AddConsoleFormatter<CustomConsoleFormatter, CustomConsoleFormatterOptions>()
				.AddConsole(options => options.FormatterName = nameof(CustomConsoleFormatter)))
			.CreateLogger("General");
	}

	public static void LogInfo(string message) => Logger.LogInformation(message);

	public static void LogGitHubClientState()
	{
		var rateLimit = RateLimitProvider?.Invoke();

		if (rateLimit == null)
		{
			return;
		}

		LogInfo($"Remaining API calls: {rateLimit.Remaining} / {rateLimit.Limit}");
	}

	private const string CharactersArtifactPath = "./artifacts/characters.txt";

	public static Task LogCharactersToFile(List<(Character Character, string[] Commands)> characters)
	{
		var text = new StringBuilder();

		foreach (var (character, commands) in characters)
		{
			AppendCharacter(text, character, commands);
		}

		var directoryPath = Path.GetDirectoryName(CharactersArtifactPath);
		if (!Directory.Exists(directoryPath))
		{
			Directory.CreateDirectory(directoryPath!);
		}

		return File.WriteAllTextAsync(CharactersArtifactPath, text.ToString());
	}

	private static void AppendCharacter(StringBuilder sb, Character character, string[] commands)
	{
		var playerInfo = character.PlayerInfo;
		var name = playerInfo.IsBot ? playerInfo.IssueTitle : playerInfo.UserLogin;
		sb.AppendLine($"Character #{playerInfo.IssueNumber} {name}");
		sb.Append($"StateCommentId: {playerInfo.StateCommentId?.ToString() ?? "none"}; ");
		sb.AppendLine($"IsStargazer: {playerInfo.IsStargazer}; IssueReactions: {playerInfo.IssueReactions}");
		sb.AppendLine($"DTO: {CharacterDto.FromCharacter(character).ToString()}");

		sb.AppendLine("Commands:");

		foreach (var command in commands)
		{
			sb.AppendLine(command);
		}

		sb.AppendLine("___\n");
	}
}

public class CustomConsoleFormatter() : ConsoleFormatter(nameof(CustomConsoleFormatter))
{
	public override void Write<TState>(
		in LogEntry<TState> logEntry,
		IExternalScopeProvider? scopeProvider,
		TextWriter textWriter)
	{
		var color = logEntry.LogLevel switch
		{
			LogLevel.Information => ConsoleColor.White,
			LogLevel.Warning => ConsoleColor.Yellow,
			LogLevel.Error => ConsoleColor.Red,
			LogLevel.Critical => ConsoleColor.Magenta,
			_ => ConsoleColor.White
		};

		Console.ForegroundColor = color;

		Console.WriteLine(
			$"[{DateTime.Now:HH:mm:ss.fff}] {logEntry.LogLevel}: {logEntry.Formatter(logEntry.State, logEntry.Exception)}");

		Console.ResetColor();
	}
}

public class CustomConsoleFormatterOptions : ConsoleFormatterOptions;
