namespace Engine;

public class EmptyActionParametersProvider : ActionParametersProvider<object?>
{
	public override bool TryParseParameters(string[] parameters, out object? typedParameters)
	{
		typedParameters = null;

		return parameters.Length == 0;
	}
}
