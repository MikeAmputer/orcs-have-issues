﻿namespace Engine;

public class ActionBuyMaterials : ActionBase<int, QuantityActionParametersProvider>
{
	protected override ActionReport ExecuteCore(int quantity, Character character)
	{
		var price = ServerState.Instance.MaterialsPriceFor(character.Race);
		var toBuy = Math.Min(quantity, character.Gold / price);

		character.AddReward(0, -price * toBuy, toBuy);

		return ActionReport.FromMessage($"Materials trade: `-{price * toBuy} Gold` `+{toBuy} Materials`");
	}
}