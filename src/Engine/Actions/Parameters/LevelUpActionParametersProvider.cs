namespace Engine;

public class LevelUpActionParametersProvider : ActionParametersProvider<LevelUpSelection>
{
	public override bool TryParseParameters(string[] parameters, out LevelUpSelection typedParameters)
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

		return typedParameters != LevelUpSelection.None;
	}
}
