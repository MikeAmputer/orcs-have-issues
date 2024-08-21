namespace Engine;

public class EnumActionParametersProvider<TEnum> : ActionParametersProvider<TEnum>
	where TEnum : struct, Enum
{
	public override bool TryParseParameters(string[] parameters, out TEnum typedParameters)
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

		return !EqualityComparer<TEnum>.Default.Equals(typedParameters, default);
		// return (int)(object)typedParameters != 0;
	}
}
