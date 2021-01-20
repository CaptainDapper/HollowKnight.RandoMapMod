using ModCommon;
using System;
using UnityEngine;

namespace RandoMapMod
{
	class PinData
	{
		private static readonly DebugLog logger = new DebugLog(nameof(PinData));

		//Assigned with pindata.xml
		public string ID
		{
			get;
			internal set;
		}
		public string PinScene
		{
			get;
			internal set;
		}

		public string CheckBool
		{
			get;
			internal set;
		}
		public float OffsetX
		{
			get;
			internal set;
		}
		public float OffsetY
		{
			get;
			internal set;
		}
		public float OffsetZ
		{
			get;
			internal set;
		}

		//Assigned with Randomizer's items.xml:
		public string SceneName
		{
			get;
			internal set;
		}
		public string OriginalName
		{
			get;
			internal set;
		}
		public string LogicRaw
		{
			get;
			internal set;
		}
		public string ObtainedBool
		{
			get;
			internal set;
		}
		public bool InChest
		{
			get;
			internal set;
		}
		public bool NewShiny
		{
			get;
			internal set;
		}
		public int NewX
		{
			get;
			internal set;
		}
		public int NewY
		{
			get;
			internal set;
		}
		public string Pool
		{
			get;
			internal set;
		}

		/// <summary>
		/// Returns true if `pindata.xml` has the `hasPrereq` flag set to true.
		/// This seems to indicate that the item belongs to either the Grubfather
		/// or Seer and thus has a prerequisite in the number of grubs rescued or
		/// essence collected. This is used to control whether we use the
		/// `prereqPin.png` instead of the standard pins.
		/// </summary>
		public bool hasPrereq
		{
			get;
			internal set;
		}

		public bool isShop
		{
			get;
			internal set;
		}

		/// <summary>
		/// Returns true if the item is reachable based on current randomizer logic; false otherwise.
		/// </summary>
		public bool Possible
		{
			get
			{
				bool test = LogicManager.ItemIsReachable(this.ID.Replace('_', ' '));
				//Dev.Log(this.ID + " Possible? " + test);
				return test;
			}
		}

		/// <summary>
		/// Returns true if the "preReq"s are met. Most pins have preReq met by
		/// default. Grubfather and Seer pins only have preReq met if you have
		/// enough grub/essence.
		/// </summary>
		public bool PreReqMet
		{
			get
			{
				if (!this.hasPrereq)
				{
					return true;
				}
				else
				{
					logger.Log($"Checking if {this.ID} has its prereqs met...");
					int cost = 0;
					(string, int)[] costs = RandomizerMod.RandomizerMod.Instance.Settings.VariableCosts;
					for (int i = 0; i < costs.Length; i++)
					{
						if (costs[i].Item1 == this.ID)
						{
							cost = costs[i].Item2;
							break;
						}
					}
					if (cost == 0)
					{
						logger.Log($"Cost for {this.ID} was zero, so marking as prereqs met.");
						return true;
					}
					if (LogicManager.isGrubFatherItem(this.ID.Replace('_', ' ')))
					{
						var retVal = PlayerData.instance.grubsCollected > cost;
						logger.Log($"{this.ID} is a grubfather item, and  {PlayerData.instance.grubsCollected} > {cost} == {retVal}.");
						return retVal;
					}
					if (LogicManager.isSeerItem(this.ID.Replace('_', ' ')))
					{
						var retVal = PlayerData.instance.dreamOrbs > cost;
						logger.Log($"{this.ID} is a Seer item, and  {PlayerData.instance.dreamOrbs} > {cost} == {retVal}.");
						return retVal;
					}
					logger.Log($"{this.ID} returning false by default.");
					return false;
				}
			}
		}

		public Vector3 Offset
		{
			get
			{
				return new Vector3(this.OffsetX, this.OffsetY, this.OffsetZ);
			}
		}

		public PinData()
		{
			//Some of these things don't appear in the items.xml file, so I'll just set some defaults...
			this.SceneName = "";
			this.OriginalName = "";
			this.LogicRaw = "";
			this.ObtainedBool = "";
			this.NewShiny = false;
		}

		public override string ToString()
		{
			return "Pin_" + this.ID;
		}
	}
}