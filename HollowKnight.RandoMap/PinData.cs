using RandoMapMod.VersionDiffs;
using System.Security.AccessControl;
using UnityEngine;

namespace RandoMapMod {

	[DebugName(nameof(PinData))]
	public class PinData {
		#region Constructors
		public PinData() {
			//Some of these things don't appear in the items.xml file, so I'll just set some defaults...
			this.SceneName = "";
			this.OriginalName = "";
			this.LogicRaw = "";
			this.ObtainedBool = "";
			this.NewShiny = false;
		}
		#endregion

		#region Private Non-Methods
		//Assigned with pindata.xml
		public string ID {
			get;
			internal set;
		}
		public string PinScene {
			get;
			internal set;
		}

		public string CheckBool {
			get;
			internal set;
		}
		public float OffsetX {
			get;
			internal set;
		}
		public float OffsetY {
			get;
			internal set;
		}
		public float OffsetZ {
			get;
			internal set;
		}

		//Assigned with Randomizer's items.xml:
		public string SceneName {
			get;
			internal set;
		}
		public string OriginalName {
			get;
			internal set;
		}
		public string LogicRaw {
			get;
			internal set;
		}
		public string ObtainedBool {
			get;
			internal set;
		}
		public bool InChest {
			get;
			internal set;
		}
		public bool NewShiny {
			get;
			internal set;
		}
		public int NewX {
			get;
			internal set;
		}
		public int NewY {
			get;
			internal set;
		}
		public string Pool {
			get;
			internal set;
		}

		/// <summary>
		/// Returns true if `pindata.xml` has the `hasPrereq` flag set to true. This
		/// indicates that the item belongs to either the Grubfather or Seer and thus
		/// has a prerequisite cost. This is used to control whether we add a "!" to a pin.
		/// </summary>
		public bool HasPrereq {
			get;
			internal set;
		}

		public bool IsShop {
			get;
			internal set;
		}

		public Vector3 Offset => new Vector3(this.OffsetX, this.OffsetY, this.OffsetZ);
		public bool CreationRequired {
			get {
				bool? isRand = this.Pool switch {
					"Dreamer" => RandomizerMod.RandomizerMod.Instance.Settings.RandomizeDreamers,
					"Skill" => RandomizerMod.RandomizerMod.Instance.Settings.RandomizeSkills,
					"Charm" => RandomizerMod.RandomizerMod.Instance.Settings.RandomizeCharms,
					"Key" => RandomizerMod.RandomizerMod.Instance.Settings.RandomizeKeys,
					"Geo" => RandomizerMod.RandomizerMod.Instance.Settings.RandomizeGeoChests,
					"Mask" => RandomizerMod.RandomizerMod.Instance.Settings.RandomizeMaskShards,
					"Vessel" => RandomizerMod.RandomizerMod.Instance.Settings.RandomizeVesselFragments,
					"Ore" => RandomizerMod.RandomizerMod.Instance.Settings.RandomizePaleOre,
					"Notch" => RandomizerMod.RandomizerMod.Instance.Settings.RandomizeCharmNotches,
					"Egg" => RandomizerMod.RandomizerMod.Instance.Settings.RandomizeRancidEggs,
					"Relic" => RandomizerMod.RandomizerMod.Instance.Settings.RandomizeRelics,
					"Map" => RandomizerMod.RandomizerMod.Instance.Settings.RandomizeMaps,
					"Stag" => RandomizerMod.RandomizerMod.Instance.Settings.RandomizeStags,
					"Flame" => MapMod.VersionController.RandomizeGrimmkinFlames(),
					"Soul" => RandomizerMod.RandomizerMod.Instance.Settings.RandomizeSoulTotems,
					"Lore" => RandomizerMod.RandomizerMod.Instance.Settings.RandomizeLoreTablets,
					"Cocoon" => RandomizerMod.RandomizerMod.Instance.Settings.RandomizeLifebloodCocoons,
					"PalaceSoul" => RandomizerMod.RandomizerMod.Instance.Settings.RandomizePalaceTotems,
					"Rock" => RandomizerMod.RandomizerMod.Instance.Settings.RandomizeRocks,
					"Boss_Geo" => RandomizerMod.RandomizerMod.Instance.Settings.RandomizeBossGeo,
					"PalaceLore" => RandomizerMod.RandomizerMod.Instance.Settings.RandomizePalaceTablets,

					"Cursed" => RandomizerMod.RandomizerMod.Instance.Settings.Cursed,
					"SplitCloak" => RandomizerMod.RandomizerMod.Instance.Settings.RandomizeCloakPieces,
					"SplitCloakLocation" => RandomizerMod.RandomizerMod.Instance.Settings.RandomizeCloakPieces,
					"SplitClaw" => RandomizerMod.RandomizerMod.Instance.Settings.RandomizeClawPieces,
					"CursedNail" => RandomizerMod.RandomizerMod.Instance.Settings.CursedNail,

					//The following will always need a pin. If they aren't randomized, they have ResourceHelper pins.
					"Grub" => true,
					"Root" => true,
					"Essence_Boss" => true,
					_ => null,
				};

#if DEBUG
				if (this.Pool == "Cocoon") {
					//DebugLog.Log($"`{this.Pool}` => `{this.ID}`");
				}
#endif

				if (isRand == null) {
					if (this.ID.Contains("Grimmkin") && MapMod.VersionController is MultiWorldRando3) {
						//No need to warn
					} else {
						DebugLog.Warn($"Undefined Pool Type: `{this.Pool}` from PinData `{this.ID}`");
					}
					return true;
				} else {
					return (bool) isRand;
				}
			}
		}
		#endregion

		#region <> Overrides
		public override string ToString() {
			return "Pin_" + this.ID;
		}
		#endregion
	}
}