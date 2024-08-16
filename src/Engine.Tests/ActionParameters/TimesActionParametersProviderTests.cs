namespace Engine.Tests.ActionParameters;

[TestClass]
public class TimesActionParametersProviderTests
{
	[TestMethod]
	public void EmptyArray_ReturnsOne()
	{
		string[] parameters = [];

		var provider = new TimesActionParametersProvider();
		var success = provider.TryParseParameters(parameters, out var parameter);

		Assert.IsTrue(success);
		Assert.AreEqual(1, parameter);
	}

	[TestMethod]
	[DataRow("1", 1)]
	[DataRow("10", 10)]
	public void SinglePositiveValueArray_IntParsedWithSuccess(string stringValue, int parsedValue)
	{
		string[] parameters = [stringValue];

		var provider = new TimesActionParametersProvider();
		var success = provider.TryParseParameters(parameters, out var parameter);

		Assert.IsTrue(success);
		Assert.AreEqual(parsedValue, parameter);
	}

	[TestMethod]
	[DataRow("-1")]
	[DataRow("0")]
	public void SingleNonPositiveValueArray_Unsuccessful(string stringValue)
	{
		string[] parameters = [stringValue];

		var provider = new TimesActionParametersProvider();
		var success = provider.TryParseParameters(parameters, out _);

		Assert.IsFalse(success);
	}

	[TestMethod]
	public void TwoElementArray_Unsuccessful()
	{
		string[] parameters = ["1", "2"];

		var provider = new TimesActionParametersProvider();
		var success = provider.TryParseParameters(parameters, out _);

		Assert.IsFalse(success);
	}

	[TestMethod]
	public void InvalidValue_Unsuccessful()
	{
		string[] parameters = ["ONE"];

		var provider = new TimesActionParametersProvider();
		var success = provider.TryParseParameters(parameters, out _);

		Assert.IsFalse(success);
	}
}
