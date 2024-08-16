namespace Engine;

public abstract class ActionBase<TParameters, TParametersProvider> : IAction
	where TParametersProvider : ActionParametersProvider<TParameters>, new()
{
	private readonly TParametersProvider _parametersProvider = new();

	public ActionReport Execute(string[] parameters, Character character)
	{
		if (!IsAvailable(character))
		{
			return ActionReport.Empty;
		}

		if (!_parametersProvider.TryParseParameters(parameters, out var typedParams))
		{
			return ActionReport.Empty;
		}

		return ExecuteCore(typedParams, character);
	}

	protected abstract ActionReport ExecuteCore(TParameters parameters, Character character);

	protected virtual bool IsAvailable(Character character)
	{
		return true;
	}
}
