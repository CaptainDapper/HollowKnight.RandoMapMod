using ModCommon;
using System;
using UnityEngine;

namespace RandoMapMod
{
	class PinData
	{
		public enum Types
		{
			SceneData,
			PlayerBool
		}

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

		public Types CheckType
		{
			get;
			internal set;
		}
		public string CheckBool
		{
			get;
			internal set;
		}
		public string PrereqRaw
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

		public bool Possible
		{
			get
			{
				bool test = LogicManager.reachableItems.Contains(this.ID.Replace('_', ' '));
				//Dev.Log(this.ID + " Possible? " + test);
				return test;
			}
		}
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
					if (cost == 0) return true;
					if (RandoMapMod.grubfatherItems.Contains(this.ID.Replace('_', ' ')))
					{
						//Dev.Log("Grub Cost for " + this.ID + " is " + cost + ". You have " + PlayerData.instance.grubsCollected);
						return PlayerData.instance.grubsCollected > cost;
					}
					if (RandoMapMod.seerItems.Contains(this.ID.Replace('_', ' ')))
					{
						//Dev.Log("Essence Cost for " + this.ID + " is " + cost + ". You have " + PlayerData.instance.dreamOrbs);
						return PlayerData.instance.dreamOrbs > cost;
					}
					return false;
				}
				//if ( this.PrereqRaw == null ) {
				//	return true;
				//}
				//return LogicManager.ParseLogic( this.Prereq, LogicManager.ParsePrereqNode );
			}
		}
		public string ObjectName
		{
			get
			{
				return ObjectNames.Get(this);
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