namespace Engine.Tests.ActionParameters;

[TestClass]
public class RaceActionParametersProviderTests
{
	[TestMethod]
	public void EmptyArray_Unsuccessful()
	{
		string[] parameters = [];

		var provider = new RaceActionParametersProvider();
		var success = provider.TryParseParameters(parameters, out var parameter);

		Assert.IsFalse(success);
	}

	[TestMethod]
	[DataRow("human", Race.Human)]
	[DataRow("orc", Race.Orc)]
	[DataRow("1", Race.Human)]
	[DataRow("2", Race.Orc)]
	public void SingleValidValueArray_RaceParsedWithSuccess(string stringValue, Race parsedValue)
	{
		string[] parameters = [stringValue];

		var provider = new RaceActionParametersProvider();
		var success = provider.TryParseParameters(parameters, out var parameter);

		Assert.IsTrue(success);
		Assert.AreEqual(parsedValue, parameter);
	}

	[TestMethod]
	public void TwoElementArray_Unsuccessful()
	{
		string[] parameters = ["human", "orc"];

		var provider = new RaceActionParametersProvider();
		var success = provider.TryParseParameters(parameters, out _);

		Assert.IsFalse(success);
	}

	[TestMethod]
	[DataRow("none")]
	[DataRow("0")]
	[DataRow("3")]
	[DataRow("nonexistent")]
	[DataRow("10000000000")]
	public void InvalidValue_Unsuccessful(string parameter)
	{
		string[] parameters = [parameter];

		var provider = new RaceActionParametersProvider();
		var success = provider.TryParseParameters(parameters, out _);

		Assert.IsFalse(success);
	}
}
