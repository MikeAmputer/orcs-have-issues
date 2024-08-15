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
