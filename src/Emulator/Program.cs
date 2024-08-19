using System.Text.RegularExpressions;
using Engine;

var playerInfo = new PlayerInfo
{
	IssueReactions = 2,
	IsStargazer = true,
};

var dto = new CharacterDto
{
	Exp = 0,
	Gold = 0,
	Materials = 0,
};

var character = new Character(playerInfo, dto);

var statsRegex = new Regex("### Stats(?:[\\s\\S]*?)### Logs");
var cycles = 0;

while (true)
{
	var command = Console.ReadLine();

	if (command == "show")
	{
		var fullText = character.ToStateCommentBody();
		var state = statsRegex
			.Match(fullText)
			.Value
			.Replace("### Stats", "")
			.Replace("### Logs", "")
			.Trim();
		Console.WriteLine(state);
		continue;
	}

	var reports = command!
		.Split('|')
		.Select(c => ActionRunner.Execute(c.Trim(), character))
		.Where(report => report.IsExecuted)
		.Select(report => report.LogMessage)
		.ToList();

	if (reports.Count != 0)
	{
		Console.WriteLine($"Cycles: {++cycles}");
		reports.ForEach(Console.WriteLine);
	}
	else
	{
		Console.WriteLine("Nothing to execute");
	}

	character = new Character(playerInfo, CharacterDto.FromCharacter(character));
}
