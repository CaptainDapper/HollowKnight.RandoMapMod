using ModCommon;
using ModCommon.Util;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RandoMapMod
{
	static class LogicManager
	{
		private static readonly DebugLog logger = new DebugLog(nameof(LogicManager));

		private static readonly Dictionary<string, string> shopItems = new Dictionary<string, string>()
		{
			{"Gathering Swarm", "Sly"},
			{"Stalwart Shell", "Sly"},
			{"Lumafly Lantern", "Sly"},
			{"Simple Key-Sly", "Sly"},
			{"Mask Shard-Sly1", "Sly"},
			{"Mask Shard-Sly2", "Sly"},
			{"Vessel Fragment-Sly1", "Sly"},
			{"Rancid Egg-Sly", "Sly"},
			{"Heavy Blow", "Sly (Key)" },
			{"Sprintmaster", "Sly (Key)" },
			{"Elegant Key", "Sly (Key)" },
			{"Wayward Compass", "Iselda" },
			{"Quick Focus", "Salubra" },
			{"Lifeblood Heart", "Salubra" },
			{"Steady Body", "Salubra" },
			{"Long Nail", "Salubra" },
			{"Shaman Stone", "Salubra" },
			{"Fragile Heart", "Leg Eater" },
			{"Fragile Greed", "Leg Eater" },
			{"Fragile Strength", "Leg Eater" },
			{"Mask Shard-5 Grubs", "Grubfather" },
			{"Pale Ore-Grubs", "Grubfather" },
			{"Rancid Egg-Grubs", "Grubfather" },
			{"Hallownest Seal-Grubs", "Grubfather" },
			{"King's Idol-Grubs", "Grubfather" },
			{"Grubsong", "Grubfather" },
			{"Grubberfly's Elegy", "Grubfather" },
			{"Arcane Egg-Seer", "Seer" },
			{"Vessel Fragment-Seer", "Seer" },
			{"Pale Ore-Seer", "Seer" },
			{"Hallownest Seal-Seer", "Seer" },
			{"Dream Gate", "Seer" },
			{"Awoken Dream Nail", "Seer" },
			{"Dream Wielder", "Seer" },
		};

		public static HelperData helperData;
		
		public static bool ItemIsChecked(string itemName) 
		{
			if (helperData == null)
			{
				return false;
			}
			return helperData.checkedd.Values.Any(location => location.items.Contains(itemName));
		}

		public static bool ItemIsReachable(string itemName) 
		{
			if (helperData == null)
			{
				return false;
			}
			if (helperData.reachable.Values.Any(location => {
				return location.items.Contains(itemName);
				}))
			{
				return true;
			}
			if (shopItems.ContainsKey(itemName))
			{
				/*
				 * If this is a shop item, we need to say it's reachable whether the item
				 * is in HelperData's "checked" or "reachable", or else after the player
				 * checks the shop once, the pins will all shrink.
				 */
				string shopName = shopItems[itemName];
				if (helperData.checkedd.ContainsKey("Shops"))
				{
					Location location = helperData.checkedd["Shops"];
					if (location.items.Contains(shopName)) {
						return true;
					}
				}
				if (helperData.reachable.ContainsKey("Shops"))
				{
					Location location = helperData.reachable["Shops"];
					if (location.items.Contains(shopName)) {
						return true;
					}
				}
			}
			return false;
		}

		public static bool isGrubFatherItem(string itemName)
		{
			return "Grubfather".Equals(shopItems[itemName]);
		}

		public static bool isSeerItem(string itemName)
		{
			return "Seer".Equals(shopItems[itemName]);
		}
	}
}