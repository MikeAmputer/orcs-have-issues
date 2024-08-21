using System.Text;

namespace Engine;

public static class CharacterExtensions
{
	public static string ToStateCommentBody(this Character character)
	{
		var logs = character.Logs.ToString();

		var dto = CharacterDto.FromCharacter(character);

		var state = new StringBuilder();

		state.AppendLine($"**_Level {character.LevelInfo.Level}, {character.Race}_**");

		if (character.IsLevelUpAvailable)
		{
			state.AppendLine(" `Level-up available`");
		}

		state.AppendLine($"Exp until next level: {character.LevelInfo.UntilNextLevel} (total: {dto.Exp})");
		state.AppendLine(
			$"Battle stats: `HP: {character.MaxHp}` `ATK: {character.Attack}` `DEF: {character.Defence}`");
		state.AppendLine($"Resources: `Gold: {dto.Gold}` `Materials: {dto.Materials}`");
		state.AppendLine($"Equipment: `Weapon Rank: {dto.WeaponRank}` `Armor Rank: {dto.ArmorRank}`");
		state.AppendLine($"Action points: `AP: {character.MaxAp}`");

		var fortressBuffs = character.GetFortressBuffDescription("Fortress buffs:");
		if (!fortressBuffs.IsNullOrWhiteSpace())
		{
			state.AppendLine(fortressBuffs);
		}

		var args = new object[]
		{
			state.ToString(),
			logs,
			dto.ToString(),
		};

		return string.Format(StateCommentBodyTemplate, args);
	}

	private const string StateCommentBodyTemplate = @"### Stats
{0}
### Logs
{1}
___
<details><summary>DTO</summary>
<p>

```json
{2}
```

</p>
</details>";
}
