using CommandLine;

namespace BotController;

public class Options
{
	[Option("token", Required = true)]
	public string GitHubToken { get; set; } = string.Empty;

	[Option("test-mode", Default = true)]
	public bool? TestMode { get; set; }
}
