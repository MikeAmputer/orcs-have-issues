namespace Engine;

public class FortressBuff
{
	public int MaxHp { get; set; }
	public int Attack { get; set; }
	public int Defence { get; set; }

	public bool IsEmpty => MaxHp + Attack + Defence == 0;
}
