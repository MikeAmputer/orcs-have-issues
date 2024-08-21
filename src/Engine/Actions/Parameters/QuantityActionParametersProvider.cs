namespace Engine;

public class QuantityActionParametersProvider : ActionParametersProvider<int>
{
	public override bool TryParseParameters(string[] parameters, out int typedParameters)
	{
		if (parameters.Length != 1)
		{
			typedParameters = 0;
			return false;
		}

		typedParameters = parameters.Length == 0 ? 1 : parameters[0].TryConvertToInt32();

		return typedParameters > 0;
	}
}
