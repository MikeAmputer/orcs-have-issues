namespace Engine;

public class RaceActionParametersProvider : ActionParametersProvider<Race>
{
	public override bool TryParseParameters(string[] parameters, out Race typedParameters)
	{
		typedParameters = default;

		if (parameters.Length != 1)
		{
			return false;
		}

		if (!Enum.TryParse(parameters[0], true, out typedParameters))
		{
			return false;
		}

		if (!Enum.IsDefined(typedParameters))
		{
			typedParameters = default;
			return false;
		}

		return typedParameters != Race.None;
	}
}
