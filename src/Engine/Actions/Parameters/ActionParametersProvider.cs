namespace Engine;

public abstract class ActionParametersProvider<TParameters>
{
	public abstract bool TryParseParameters(string[] parameters, out TParameters typedParameters);
}
