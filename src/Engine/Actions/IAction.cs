namespace Engine;

public interface IAction
{
	public ActionReport Execute(string[] parameters, Character character);
}
