using System.Collections.Generic;
using Vintagestory.API.Common;
using Vintagestory.API.Server;
using Newtonsoft.Json;

namespace SmithingPlusTweaks;

public class PlayerStatsFix : ModSystem
{
	internal const string SmithingQuality = Core.SP_SORT_ID+":smithingQuality";
	internal const string ToolRepairPenaltyModifier = Core.SP_SORT_ID+":toolRepairPenaltyModifier";
	
	public override void StartServerSide(ICoreServerAPI api)
	{
		// Load traits.json
		var asset = api.Assets.TryGet(Core.SP_ID + ":config/traits.json");
		if (asset == null) return;

		var traits = asset.ToObject<List<TraitConfig>>();

		api.Event.PlayerJoin += player =>
		{
			var stats = player.Entity.Stats;
			if (stats == null) return;
			
			foreach (var trait in traits)
			{
				SetTrait(stats, trait);
			}
		};
	}
	
	private static void SetTrait(EntityStats stats, TraitConfig t)
	{
		foreach (var attribute in t.Attributes)
		{
			if (attribute.Key == SmithingQuality)
			{
				stats.Set("traits",SmithingQuality, attribute.Value + SmithingPlus.Config.ConfigLoader.Config.HelveHammerSmithingQualityModifier);
			}
			else if (attribute.Key == ToolRepairPenaltyModifier)
			{
				stats.Set("traits",ToolRepairPenaltyModifier, attribute.Value + SmithingPlus.Config.ConfigLoader.Config.RepairableToolDurabilityMultiplier);
			}
		}
	}

	private class TraitConfig
	{
		public string Code { get; set; }
		public string Type { get; set; }
		public Dictionary<string, float> Attributes { get; set; }
	}
}