using HutongGames.PlayMaker;
using ModCommon;
using Modding;
using SeanprCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.SceneManagement;
using static RandoMapMod.Resources;

namespace RandoMapMod {
	public class RandoMapMod : Mod {
		#region Static Stuff
		private const int SAFE = 3;
		private const float MAP_MIN_X = -24.16f;
		private const float MAP_MAX_X = 17.3f;
		private const float MAP_MIN_Y = -12.58548f;
		private const float MAP_MAX_Y = 15.6913f;

		private static readonly DebugLog _logger = new DebugLog(nameof(RandoMapMod));
		private static int _convoCheck = 0;
		private static bool _locked = false;

		public static List<string> shopNames = new List<string>()
		{
			"Sly",
			"Sly (Key)",
			"Iselda",
			"Salubra",
			"Leg Eater",
			"Grubfather",
			"Seer"
		};

		public static List<string> slyItems = new List<string>()
		{
			"Gathering Swarm",
			"Stalwart Shell",
			"Lumafly Lantern",
			"Simple Key-Sly",
			"Mask Shard-Sly1",
			"Mask Shard-Sly2",
			"Vessel Fragment-Sly1",
			"Rancid Egg-Sly",
		};

		public static List<string> slyKeyItems = new List<string>()
		{
			"Gathering Swarm",
			"Stalwart Shell",
			"Lumafly Lantern",
			"Simple Key-Sly",
			"Mask Shard-Sly1",
			"Mask Shard-Sly2",
			"Mask Shard-Sly3",
			"Mask Shard-Sly4",
			"Vessel Fragment-Sly1",
			"Vessel Fragment-Sly2",
			"Rancid Egg-Sly",
			"Heavy Blow",
			"Sprintmaster",
			"Elegant Key",
		};

		public static List<string> iseldaItems = new List<string>()
		{
			"Wayward Compass",
		};

		public static List<string> salubraItems = new List<string>()
		{
			"Quick Focus",
			"Lifeblood Heart",
			"Steady Body",
			"Long Nail",
			"Shaman Stone",
		};

		public static List<string> legEaterItems = new List<string>()
		{
			"Fragile Heart",
			"Fragile Greed",
			"Fragile Strength",
		};

		public static List<string> grubfatherItems = new List<string>()
		{
			"Mask Shard-5 Grubs",
			"Pale Ore-Grubs",
			"Rancid Egg-Grubs",
			"Hallownest Seal-Grubs",
			"King's Idol-Grubs",
			"Grubsong",
			"Grubberfly's Elegy"
		};

		public static List<string> seerItems = new List<string>()
		{
			"Arcane Egg-Seer",
			"Vessel Fragment-Seer",
			"Pale Ore-Seer",
			"Hallownest Seal-Seer",
			"Dream Gate",
			"Awoken Dream Nail",
			"Dream Wielder"
		};


		public static RandoMapMod Instance {
			get; private set;
		}

		public static bool IsRando => RandomizerMod.RandomizerMod.Instance.Settings.Randomizer;

		public static void GiveCollectorsMap(bool toggle = false) {
			if (RandomizerMod.RandomizerMod.Instance.Settings.RandomizeGrubs) {
				// Do nothing
			} else {
				if (toggle) {
					//Toggle
					PlayerData.instance.SetBool(nameof(PlayerData.instance.hasPinGrub), !PlayerData.instance.hasPinGrub);
				} else {
					PlayerData.instance.SetBool(nameof(PlayerData.instance.hasPinGrub), true);
				}
			}
		}

		public static void GiveAllMaps(string from) {
			_logger.Log($"Maps granted through ElderBug from {from}");

			PlayerData pd = PlayerData.instance;
			Type playerData = typeof(PlayerData);

			// Give the maps to the player
			pd.SetBool(nameof(pd.hasMap), true);

			foreach (FieldInfo field in playerData.GetFields().Where(field => field.Name.StartsWith("map") && field.FieldType == typeof(bool))) {
				pd.SetBool(field.Name, true);
			}

			//Give them compass and Quill
			pd.SetBool(nameof(pd.gotCharm_2), true);
			pd.SetBool(nameof(pd.hasQuill), true);
			GiveCollectorsMap();

			// Set cornifer as having left all the areas. This could be condensed into the previous foreach for one less GetFields(), but I value the clarity more.
			foreach (FieldInfo field in playerData.GetFields().Where(field => field.Name.StartsWith("corn") && field.Name.EndsWith("Left"))) {
				pd.SetBool(field.Name, true);
			}

			// Set Cornifer as sleeping at home
			pd.SetBool(nameof(pd.corniferAtHome), true);
		}
		#endregion


		#region Private Non-Methods
		private GameObject _pinGroup = null;
		private GameMap _theMap;
		private Resources _resources = null;

		private PinGroup _PinGroup => _pinGroup?.GetComponent<PinGroup>();
		#endregion


		#region Non-Private Non-Methods
		public SaveSettings Settings { get; private set; } = new SaveSettings();
		#endregion


		#region <Mod> Overrides
		public override ModSettings SaveSettings {
			get => Settings ??= new SaveSettings();
			set => Settings = value is SaveSettings saveSettings ? saveSettings : Settings;
		}
		
		public override void Initialize() {
			if (Instance != null) {
				_logger.Warn("Initialized twice... Stop that.");
				return;
			}
			Instance = this;
			_logger.Log("RandoMapMod Initializing...");

			this._resources = new Resources();

			On.GameMap.Start += this._GameMap_Start;             //Set up custom pins
			On.GameMap.WorldMap += this._GameMap_WorldMap;         //Set big map boundaries
			On.GameMap.SetupMapMarkers += this._GameMap_SetupMapMarkers;   //Enable the custom pins
			On.GameMap.DisableMarkers += this._GameMap_DisableMarkers;   //Disable the custom pins

			//Giveaway time
			UnityEngine.SceneManagement.SceneManager.activeSceneChanged += _HandleSceneChanges;
			ModHooks.Instance.LanguageGetHook += _HandleLanguageGet;

			_logger.Log("RandoMapMod Initialize complete!");
		}

		public override string GetVersion() {
			string ver = "0.5.1"; //If you update this, please also update the README.
			int minAPI = 45;

			bool apiTooLow = Convert.ToInt32(ModHooks.Instance.ModVersion.Split('-')[1]) < minAPI;
			if (apiTooLow) {
				return ver + " (Update API)";
			}

			return ver;
		}
		#endregion


		#region Private Methods

		private void _GameMap_Start(On.GameMap.orig_Start orig, GameMap self) {
			if (!IsRando) {
				orig(self);
				return;
			}
			try {
				_logger.Log("Emptying out HelperData on game start.");
				HelperLog.NewGame();

				//Create the custom pin group, and add all the new pins
				if (this._pinGroup == null) {
					this._theMap = self;
					this._pinGroup = new GameObject("Custom Pins");
					this._pinGroup.AddComponent<PinGroup>();
					this._pinGroup.AddComponent<MapTextOverlay>();
					this._pinGroup.transform.parent = self.transform;
					this._pinGroup.transform.position = new Vector3(0f, 0f, 0f);
					this._PinGroup.Hide();

					foreach (PinData pin in this._resources.PinData().Values) {
						this._AddPinToRoom(pin);
					}
				}
			} catch (Exception e) {
				_logger.Error($"Error: {e}");
			}

			orig(self);
		}

		private void _GameMap_WorldMap(On.GameMap.orig_WorldMap orig, GameMap self) {
			orig(self);
			if (!IsRando)
				return;
			//Dev.Log("WORLD THE MAP!");
			//Set the maximum scroll boundaries, so we can scroll the entire map, even if we don't have the maps unlocked.
			if (self.panMinX > MAP_MIN_X)
				self.panMinX = MAP_MIN_X;
			if (self.panMaxX < MAP_MAX_X)
				self.panMaxX = MAP_MAX_X;
			if (self.panMinY > MAP_MIN_Y)
				self.panMinY = MAP_MIN_Y;
			if (self.panMaxY < MAP_MAX_Y)
				self.panMaxY = MAP_MAX_Y;
		}

		private void _GameMap_SetupMapMarkers(On.GameMap.orig_SetupMapMarkers orig, GameMap self) {
			orig(self);
			if (!IsRando) {
				return;
			}
			this._PinGroup.Show();
		}

		private void _GameMap_DisableMarkers(On.GameMap.orig_DisableMarkers orig, GameMap self) {
			if (!IsRando) {
				orig(self);
				return;
			}
			try {
				//Dev.Log("UNMARK THE MAP!");
				this._PinGroup.Hide();
			} catch (Exception e) {
				_logger.Error($"Failed to DisableMarkers {e}");
			}
			orig(self);
		}

		private void _AddPinToRoom(PinData pin) {
			string roomName = pin.PinScene ?? this._resources.PinData()[pin.ID].SceneName;

			GameObject newPin = new GameObject("pin_rando");
			newPin.transform.parent = this._pinGroup.transform;
			newPin.layer = 30;
			newPin.transform.localScale *= 1.2f;

			SpriteRenderer sr = newPin.AddComponent<SpriteRenderer>();
			if (pin.isShop)
				sr.sprite = Resources.Sprite("Map.shopPin");
			else
				if (pin.Pool == "Rock")
				sr.sprite = Resources.Sprite("Map.rockPin");
			else if (pin.Pool == "Soul" || pin.Pool == "PalaceSoul")
				sr.sprite = Resources.Sprite("Map.totemPin");
			else if (pin.Pool == "Cocoon")
				sr.sprite = Resources.Sprite("Map.lifebloodPin");
			else if (pin.Pool == "Grub" && !Pin.IsRandomizedInSettings["Grub"])
				sr.sprite = Resources.Sprite("Map.actualGrub");
			else if ((pin.Pool == "Root" && !Pin.IsRandomizedInSettings["Root"]) || (pin.Pool == "Essence_Boss" && (!Pin.IsRandomizedInSettings.ContainsKey("Essence_Boss") || !Pin.IsRandomizedInSettings["Essence_Boss"])))
			{
				//Dev.Log("Pin " + pin.ID + " in room " + pin.SceneName);
				sr.sprite = Resources.Sprite("Map.actualEssence");
			}
			else
				sr.sprite = Resources.Sprite("Map.randoPin");
			sr.sortingLayerName = "HUD";
			sr.size = new Vector2(1f, 1f);

			Pin pinC = newPin.AddComponent<Pin>();
			pinC.PinData = pin;
			pinC.Resources = _resources;

			Vector3 vec = this._GetRoomPos(roomName) + pin.Offset;
			newPin.transform.localPosition = new Vector3(vec.x, vec.y, (vec.z - 0.5f));
		}

		private Vector3 _GetRoomPos(string prmRoomName) {
			Vector3 pos = new Vector3(-30f, -30f, -0.5f);
			bool exitLoop = false;

			for (int index1 = 0; index1 < this._theMap.transform.childCount; ++index1) {
				GameObject gameObject1 = this._theMap.transform.GetChild(index1).gameObject;
				for (int index2 = 0; index2 < gameObject1.transform.childCount; ++index2) {
					GameObject gameObject2 = gameObject1.transform.GetChild(index2).gameObject;
					if (gameObject2.name == prmRoomName) {
						pos = gameObject2.transform.position;
						exitLoop = true;
						break;
					}
				}
				if (exitLoop) {
					break;
				}
			}

			return pos;
		}

		private void _HandleSceneChanges(Scene from, Scene to) {
			if (!IsRando) {
				return;
			}

			if (to.name == "Town") {
				_convoCheck = 0;
				_locked = false;
				PlayMakerFSM elder = FSMUtility.LocateFSM(GameObject.Find("Elderbug"), "npc_control");
				FsmState target = null;
				foreach (FsmState state in elder.FsmStates) {
					if (state.Name == "Convo End") {
						target = state;
						break;
					}
				}

				if (target != null) {
					List<FsmStateAction> actions = target.Actions.ToList();
					actions.Add(new ElderbugIsACoolDude());
					target.Actions = actions.ToArray();
				}
			}
			if (to.name == SceneNames.Menu_Title) {
				Settings.MapsGiven = false;
				Pin.Reset();
			}
  			if (from.name == SceneNames.Menu_Title) {
				Pin.Reset();
				Pin.Setup();
			}
		}

		private string _HandleLanguageGet(string key, string sheetTitle) {
			if (IsRando && _convoCheck < SAFE) {
				if (string.IsNullOrEmpty(key) || string.IsNullOrEmpty(sheetTitle)) {
					return string.Empty;
				}

				if (sheetTitle == "Elderbug") {
					Dev.Log("Map Check! Safety: " + _convoCheck + ", locked: " + _locked + ", Key: " + key + ", " + Settings.MapsGiven);
					if (_convoCheck == 0 && key == "ELDERBUG_INTRO_MAIN") {
						_convoCheck++;
						string talk = "Welcome to RandoMapMod!" +
							"\nA BIG pin means look there for progression. LITTLE means the next key item won't be there. \"!\" means you need something else, maybe grubs or a key? \"$\" indicates a shop that may have items." +
							"\nTalk to me 2 more times, and I'll give you all the maps." +
							"\nIf you're playing BINGO, you should probably not do that.";
						talk += "<page>And also, instead of talking to me, you can simply do the following:" +
							"\"Ctrl + M\" - Gives you all the maps\n" +
							"\"Ctrl + G\" - Reveals all grub locations (if they aren't randomized)\n" +
							"\"Ctrl + P\" - Toggles the pins";

						if (Settings.MapsGiven) _convoCheck = 3; //Skip the rest of the conversation; just wanted to give the people a refresher at least.

						return talk;
					} else if (_convoCheck == 1 && !Settings.MapsGiven) {
						_convoCheck++;

						//Seriously? Trying to cover up Dirtmouth's scandal, are you? Tell you what, I'll tone it down a little bit but come on man; you can't tell me Elder Bug is 100% innocent. And besides, A) Who is Iselda longingly staring and sighing at all day if not Elder Bug and B) What else is Elder Bug going to do but "talk to" literally the only resident in town before you arrive and C) He's called "Elder Bug" because he's obviously the alpha male. ;)
						return "I frequently *ahem* \"visit\" Cornifer's wife... She tells me he lies to travelers to get money for an inferior product... The jerk. I've taken his completed originals. Maybe once they're bankrupt she'll run off with me.<page>I'll let you have the maps, the quill, and a compass since you're new around here if you talk to me 1 more time.";
						//return "I'll let you have the maps, the quill, and a compass since you're new around here if you talk to me 1 more time.";

					} else if (_convoCheck == 2 && !Settings.MapsGiven) {
						string maps = "Okay hang on";
						for (int i = 0; i < 10; i++) {
							maps += "...\n...\n...\n...\n";
						}
						maps += "<page>...Here you go! Now, if you'd keep my personal business to yourself, I won't have to get my hands dirty. Hm, interesting how the Pale King disappeared, don't you think...?";
						_convoCheck++;

						if (_convoCheck >= SAFE & !_locked) {
							GiveAllMaps("Conversation Stack");

							_locked = true;
						}
						maps += "<page> ...Here you go!";
						return maps;
					} else {
						return Language.Language.GetInternal(key, sheetTitle);
					}
				} else if (!_locked && sheetTitle == "Titles" && key == "DIRTMOUTH_MAIN") {
					return "FREE MAPS";
				} else if (!_locked && sheetTitle == "Titles" && key == "DIRTMOUTH_SUB") {
					return "Talk to Elderbug";
				}
			}
			return Language.Language.GetInternal(key, sheetTitle);
		}
		#endregion


		#region Mastercard
		private class ElderbugIsACoolDude : FsmStateAction {

			public override void OnEnter() {
				//_SAFETY++;

				if (_convoCheck >= SAFE & !_locked) {
					GiveAllMaps("FSMAction");

					_locked = true;
				}

				Finish();
			}
		}
		#endregion
	}
}
