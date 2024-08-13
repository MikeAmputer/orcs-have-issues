namespace Engine;

public class CharacterRepository
{
	private readonly Dictionary<long, Character> _characters = new();

	public bool ContainsUser(long userId)
	{
		return _characters.ContainsKey(userId);
	}

	public void Add(long userId, Character character)
	{
		_characters.Add(userId, character);
	}
}
