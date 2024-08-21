using CommandLine;

namespace Engine;

public class Options
{
	[Option("token", Required = true)]
	public string GitHubToken { get; set; } = string.Empty;

	[Option("test-mode", Default = true)]
	public bool TestMode { get; set; }

	[Option("period-hours", Default = 24)]
	public int PeriodHours { get; set; }
}
