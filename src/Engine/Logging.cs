using Microsoft.Extensions.Logging;
using Octokit;

namespace Engine;

public static class Logging
{
	private static readonly ILogger Logger;

	static Logging()
	{
		Logger = LoggerFactory.Create(builder => builder.AddConsole()).CreateLogger("General");
	}

	public static void LogInfo(string message) => Logger.LogInformation(message);

	public static void LogGitHubClientState(GitHubClient client)
	{
		var rateLimit = client.GetLastApiInfo()?.RateLimit;

		if (rateLimit == null)
		{
			return;
		}

		LogInfo($"Remaining API calls: {rateLimit.Remaining} / {rateLimit.Limit}");
	}
}
