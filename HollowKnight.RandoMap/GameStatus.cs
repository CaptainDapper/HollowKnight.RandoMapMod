using System.Collections.Generic;
using System.Linq;

namespace RandoMapMod {
	[DebugName(nameof(GameStatus))]
	public static class GameStatus {
		#region Statics
		private static HelperLog.DataStore _HData => HelperLog.Data;

		private static readonly Dictionary<string, string> _shopItems = new Dictionary<string, string>() {
			{"Gathering Swarm", "Sly"},
			{"Stalwart Shell", "Sly"},
			{"Lumafly Lantern", "Sly"},
			{"Simple Key-Sly", "Sly"},
			{"Mask Shard-Sly1", "Sly"},
			{"Mask Shard-Sly2", "Sly"},
			{"Vessel Fragment-Sly1", "Sly"},
			{"Rancid Egg-Sly", "Sly"},

			{"Heavy Blow", "Sly (Key)"},
			{"Sprintmaster", "Sly (Key)"},
			{"Elegant Key", "Sly (Key)"},

			{"Wayward Compass", "Iselda"},

			{"Quick Focus", "Salubra"},
			{"Lifeblood Heart", "Salubra"},
			{"Steady Body", "Salubra"},
			{"Long Nail", "Salubra"},
			{"Shaman Stone", "Salubra"},

			{"Fragile Heart", "Leg Eater"},
			{"Fragile Greed", "Leg Eater"},
			{"Fragile Strength", "Leg Eater"},

			{"Mask Shard-5 Grubs", "Grubfather"},
			{"Pale Ore-Grubs", "Grubfather"},
			{"Rancid Egg-Grubs", "Grubfather"},
			{"Hallownest Seal-Grubs", "Grubfather"},
			{"King's Idol-Grubs", "Grubfather"},
			{"Grubsong", "Grubfather"},
			{"Grubberfly's Elegy", "Grubfather"},

			{"Arcane Egg-Seer", "Seer"},
			{"Vessel Fragment-Seer", "Seer"},
			{"Pale Ore-Seer", "Seer"},
			{"Hallownest Seal-Seer", "Seer"},
			{"Mask Shard-Seer", "Seer"},
			{"Dream Gate", "Seer"},
			{"Awoken Dream Nail", "Seer"},
			{"Dream Wielder", "Seer"},
		};

		public static bool ItemIsChecked(string itemName) {
			if (_HData == null) {
				return false;
			}
			return _HData.HasChecked(itemName);
		}

		public static bool ItemIsReachable(string itemName) {
			//return MapMod.VersionController.CanGet(itemName);

			//return false;
			string cleanName = itemName.Replace('_', ' ');
			if (_HData == null) {
				return false;
			}

			if (_HData.CanReach(cleanName)) {
				return true;
			}

			if (_shopItems.ContainsKey(cleanName)) {
				/*
				 * If this is a shop item, we need to say it's reachable whether the item
				 * is in HelperData's "checked" or "reachable", or else after the player
				 * checks the shop once, the pins will all shrink.
				 */
				string shopName = _shopItems[cleanName];
				if (_HData.CheckedShopItems != null && _HData.CheckedShopItems.Contains(shopName)) {
					return true;
				}
				if (_HData.ReachableShopItems != null && _HData.ReachableShopItems.Contains(shopName)) {
					return true;
				}
			}
			return false;
		}

		public static bool ItemPrereqsAreMet(string itemName) {
			string cleanName = itemName.Replace('_', ' ');

			//DebugLog.Log($"Checking if {this.ID} has its prereqs met...");
			int cost = 0;
			(string, int)[] costs = RandomizerMod.RandomizerMod.Instance.Settings.VariableCosts;
			for (int i = 0; i < costs.Length; i++) {
				if (costs[i].Item1 == itemName) {
					cost = costs[i].Item2;
					break;
				}
			}
			if (cost == 0) {
				DebugLog.Log($"Cost for {itemName} was zero, so marking as prereqs met.");
				return true;
			}
			if (GameStatus.IsGrubFatherItem(cleanName)) {
				bool retVal = PlayerData.instance.grubsCollected > cost;
				//DebugLog.Log($"{this.ID} is a grubfather item, and  {PlayerData.instance.grubsCollected} > {cost} == {retVal}.");
				return retVal;
			}
			if (GameStatus.IsSeerItem(cleanName)) {
				bool retVal = PlayerData.instance.dreamOrbs > cost;
				//DebugLog.Log($"{this.ID} is a Seer item, and  {PlayerData.instance.dreamOrbs} > {cost} == {retVal}.");
				return retVal;
			}
			DebugLog.Log($"{itemName} returning false by default.");
			return false;
		}

		public static bool IsGrubFatherItem(string itemName) {
			return "Grubfather".Equals(_shopItems[itemName]);
		}

		public static bool IsSeerItem(string itemName) {
			return "Seer".Equals(_shopItems[itemName]);
		}
		#endregion
	}
}