namespace Engine;

public static class StringExtensions
{
	public static bool IsNullOrWhiteSpace(this string? value)
	{
		return string.IsNullOrWhiteSpace(value);
	}

	public static int TryConvertToInt32(this string value, int fallbackValue = 0)
	{
		return int.TryParse(value, out var result) ? result : fallbackValue;
	}
}
