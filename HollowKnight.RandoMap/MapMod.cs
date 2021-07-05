using HutongGames.PlayMaker;
using ModCommon;
using Modding;
//using RandoMapMod.BoringInternals;
using RandoMapMod.UnityComponents;
using RandoMapMod.VersionDiffs;
using SereCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace RandoMapMod {
	public class MapMod : Mod {
		#region Meta
		public override string GetVersion() {
			string ver = "0.5.1"; //If you update this, please also update the README.
			int minAPI = 45;

			bool apiTooLow = Convert.ToInt32(ModHooks.Instance.ModVersion.Split('-')[1]) < minAPI;
			if (apiTooLow) {
				return ver + " (Update your API)";
			}

			return ver;
		}
		#endregion


		#region Static Stuff
		private const int SAFE = 3;
		private const float MAP_MIN_X = -24.16f;
		private const float MAP_MAX_X = 17.3f;
		private const float MAP_MIN_Y = -12.58548f;
		private const float MAP_MAX_Y = 15.6913f;

		private static int _convoCheck = 0;
		private static bool _locked = false;

		public static MapMod Instance {
			get; private set;
		}

		private static IVersionController _vc = null;
		public static IVersionController VersionController {
			get {
				if (_vc == null) {
					DebugLog.Log("Finding Version");
					if (RandomizerMod.RandomizerMod.Instance.GetVersion().Contains("MW")) {
						//Multiworld
						DebugLog.Log("MultiWorld Detected");
						_vc = new MultiWorldRando3();
					} else {
						//Standard
						DebugLog.Log("Standard Rando Detected");
						_vc = new StandardRando3();
					}
				}

				return _vc;
			}
		}

		public static bool IsRando => RandomizerMod.RandomizerMod.Instance.Settings.Randomizer;

		internal static void TogglePins() {
			Instance._PinGroup.MainGroup.SetActive(!Instance._PinGroup.MainGroup.activeSelf);
		}

		public static void ToggleResourceHelpers() {
			Instance._PinGroup.HelperGroup.SetActive(!Instance._PinGroup.HelperGroup.activeSelf);
		}

		public static bool AllMapsGiven { get; private set; } = false;
		public static void GiveAllMaps(string from) {
			DebugLog.Log($"Maps granted from {from}");

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
			//GiveCollectorsMap();

			// Set cornifer as having left all the areas. This could be condensed into the previous foreach for one less GetFields(), but I value the clarity more.
			foreach (FieldInfo field in playerData.GetFields().Where(field => field.Name.StartsWith("corn") && field.Name.EndsWith("Left"))) {
				pd.SetBool(field.Name, true);
			}

			// Set Cornifer as sleeping at home
			pd.SetBool(nameof(pd.corniferAtHome), true);

			//Make sure both groups are activated
			Instance._PinGroup.MainGroup.SetActive(true);
			Instance._PinGroup.HelperGroup.SetActive(true);

			AllMapsGiven = true;
		}

		public enum PinStyles {
			Normal,
			Afraid,
			AlsoAfraid
		}

		public static PinStyles PinStyle { get; private set; } = PinStyles.Normal;

		public static void SetPinStyleOrReturnToNormal(PinStyles style) {
			DebugLog.Log($"SetPins: {PinStyle} => {style}");
			if (PinStyle == style) {
				PinStyle = PinStyles.Normal;
				DebugLog.Log($"Back to Normal: {PinStyle}");
				return;
			}
			PinStyle = style;
			DebugLog.Log($"New Stuff: {PinStyle}");
		}
		#endregion

		#region Private Non-Methods
		private GameObject _pinGroupGO = null;

		private PinGroup _PinGroup => _pinGroupGO?.GetComponent<PinGroup>();
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
				DebugLog.Warn("Initialized twice... Stop that.");
				return;
			}
			Instance = this;
			DebugLog.Log("RandoMapMod Initializing...");

			On.GameMap.Start += this._GameMap_Start;                        //Set up custom pins
			On.GameMap.WorldMap += this._GameMap_WorldMap;                  //Set big map boundaries
			On.GameMap.SetupMapMarkers += this._GameMap_SetupMapMarkers;    //Enable the custom pins
			On.GameMap.DisableMarkers += this._GameMap_DisableMarkers;      //Disable the custom pins

			On.GrubPin.OnEnable += this._GrubPin_Enable;                    //Disable all grub pins so we can use our own. (Only if we were given maps.)

			//Giveaway time
			UnityEngine.SceneManagement.SceneManager.activeSceneChanged += _HandleSceneChanges;
			ModHooks.Instance.LanguageGetHook += _HandleLanguageGet;

			DebugLog.Log("RandoMapMod Initialize complete!");
		}
		#endregion

		#region Private Methods
		private void _GameMap_Start(On.GameMap.orig_Start orig, GameMap self) {
			DebugLog.Log("GameMap_Start");
			if (!IsRando) {
				orig(self);
				return;
			}
			try {
				DebugLog.Log("Emptying out HelperData on game start.");
				HelperLog.NewGame();

				//Create the custom pin group, and add all the new pins
				if (this._pinGroupGO == null) {
					DebugLog.Log("First Setup. Adding Pin Group and Populating...");
					MapMod.SetPinStyleOrReturnToNormal(MapMod.PinStyles.Normal);
					this._pinGroupGO = new GameObject("RandoMap Pins");
					this._pinGroupGO.AddComponent<PinGroup>();
					this._pinGroupGO.AddComponent<MapTextOverlay>();
					this._pinGroupGO.transform.SetParent(self.transform);
					this._pinGroupGO.transform.position = new Vector3(0f, 0f, 0f);

					foreach (PinData pin in ResourceHelper.PinData.Values) {
						if (pin.CreationRequired) {
							this._PinGroup.AddPinToRoom(pin, self);
						}
					}

					InputListener.InstantiateSingleton();
				}
			} catch (Exception e) {
				DebugLog.Error($"Error: {e}");
			}

			DebugLog.Log("Finished.");
			orig(self);

			//_DebugPins(self);
		}

		private void _GameMap_WorldMap(On.GameMap.orig_WorldMap orig, GameMap self) {
			orig(self);
			if (!IsRando)
				return;

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
			this._DeleteErrantLifebloodPin(self);
#if DEBUG
			//DEBUG STUFF
#endif

			if (!IsRando) {
				return;
			}
			this._PinGroup.Show();
		}

		private void _DeleteErrantLifebloodPin(GameMap gameMap) {
			//DebugLog.Log("DeleteErrant");

			GameObject go = gameMap.transform.Find("Deepnest")?.Find("Deepnest_26")?.Find("pin_blue_health")?.gameObject;
			if (go == null) {
				DebugLog.Log("Couldn't find the pin!");
				return;
			}

			go.SetActive(false);
		}

		private void _GameMap_DisableMarkers(On.GameMap.orig_DisableMarkers orig, GameMap self) {
			if (!IsRando) {
				orig(self);
				return;
			}
			try {
				this._PinGroup.Hide();
			} catch (Exception e) {
				DebugLog.Error($"Failed to DisableMarkers {e.Message}");
			}
			orig(self);
		}

		private void _GrubPin_Enable(On.GrubPin.orig_OnEnable orig, GrubPin self) {
			if (AllMapsGiven) {
				if (self.gameObject.activeSelf) self.gameObject.SetActive(false);
			}

			orig(self);
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
			}
			if (from.name == SceneNames.Menu_Title) {
				//Nothing to clean up since PinGroup deletes itself when you go to the menu. Neat!
			}
		}

		private string _HandleLanguageGet(string key, string sheetTitle) {
			if (IsRando && _convoCheck < SAFE) {
				if (string.IsNullOrEmpty(key) || string.IsNullOrEmpty(sheetTitle)) {
					return string.Empty;
				}

				if (sheetTitle == "Elderbug") {
					DebugLog.Log("Map Check! Safety: " + _convoCheck + ", locked: " + _locked + ", Key: " + key + ", " + Settings.MapsGiven);

					string message = string.Empty;
					if (key == "ELDERBUG_INTRO_VISITEDCROSSROAD") {
						DebugLog.Log("returning original");
						return Language.Language.GetInternal(key, sheetTitle);
					} else if (_convoCheck == 0) {
						string talk = "Welcome to RandoMapMod!" +
							"\nA BIG pin means look there for progression. LITTLE means the next key item won't be there. \"!\" means you need something else, maybe grubs or a key? \"$\" indicates a shop that may have items." +
							"\nTalk to me 2 more times, and I'll give you all the maps." +
							"\nIf you're playing BINGO, you should probably not do that.";
						talk += "I will also go ahead and give you some special PINK pins." +
							"\nThese will show you the locations of Grubs and/or Essence Locations if they are NOT randomized." +
							"\nSome locations require either grubs or essence to unlock them, so these Resource Helpers should help.";
						talk += "<page>And also, instead of talking to me, you can simply do the following:" +
							"\"Ctrl + M\" - Gives you all the maps\n" +
							"\"Ctrl + G\" - Reveals all grub locations (if they aren't randomized)\n" +
							"\"Ctrl + P\" - Toggles the pins";
						talk += "<page>Okay, so finally if you type in 'afraidofchange' at any point, the pins will show in the old style. Big ol' \"?\" circles. Just for you Colette! <3" +
							"\nBig thanks to everyone in the HK Discord, the HK Racing Discord, other coders, and everyone who helped me test." +
							"\nI really appreciate all that test ease!";

						if (Settings.MapsGiven) _convoCheck = 3; //Skip the rest of the conversation; just wanted to give the people a refresher at least.

						message = talk;
					} else if (_convoCheck == 1 && !Settings.MapsGiven) {
						//Seriously? Trying to cover up Dirtmouth's scandal, are you? Tell you what, I'll tone it down a little bit but come on man; you can't tell me Elder Bug is 100% innocent.
						//  And besides,A) Who is Iselda longingly staring and sighing at all day if not Elder Bug
						//  and B) What else is Elder Bug going to do but "talk to" literally the only resident in town before you arrive
						//  and C) He's called "Elder Bug" because he's obviously the alpha male. ;)
						message = "I frequently *ahem* \"visit\" Cornifer's wife... She tells me he lies to travelers to get money for an inferior product... The jerk. I've taken his completed originals. Maybe once they're bankrupt she'll run off with me.<page>I'll let you have the maps, the quill, and a compass since you're new around here if you talk to me 1 more time.";

					} else if (_convoCheck == 2 && !Settings.MapsGiven) {
						string maps = "Okay hang on";
						System.Random random = new System.Random(RandomizerMod.RandomizerMod.Instance.Settings.Seed);
						for (int i = 0; i < random.Next(3, 10); i++) {
							maps += "...\n...\n...\n...\n";
						}
						maps += "<page>...Here you go! Now, if you'd keep my personal business to yourself, I won't have to get my hands dirty. Hm, interesting how the Pale King died, don't you think...?";

						message = maps;
					}

					if (_convoCheck < SAFE) {
						_convoCheck++;
					} else {
						if (!_locked) {
							GiveAllMaps("Conversation Stack");

							_locked = true;
						}
					}

					return message;
				} else if (!_locked && sheetTitle == "Titles" && key == "DIRTMOUTH_MAIN") {
					return "FREE MAPS";
				} else if (!_locked && sheetTitle == "Titles" && key == "DIRTMOUTH_SUB") {
					return "Talk to Elderbug";
				}
			}

			return Language.Language.GetInternal(key, sheetTitle);
		}

#if DEBUG && false
		//These are nice but I don't need them right now
		private void _DebugPins(GameMap gameMap) {
			for (int i = 0; i < gameMap.transform.childCount; i++) {
				GameObject areaObj = gameMap.transform.GetChild(i).gameObject;
				for (int j = 0; j < areaObj.transform.childCount; j++) {
					GameObject roomObj = areaObj.transform.GetChild(j).gameObject;
					for (int k = 0; k < roomObj.transform.childCount; k++) {
						GameObject pinObj = roomObj.transform.GetChild(k).gameObject;
							DebugLog.Log($"{gameMap.name} => {areaObj.name} => {roomObj.name} => {pinObj.name}");
						if (pinObj.name == "pin_blue_health") {
						}
					}
				}
			}
		}

		private void _LogAllComps(GameObject go) {
			DebugLog.Log($"All Components for `{go.name}`");
			foreach (Component component in go.GetComponents<Component>()) {
				DebugLog.Log($"---{component.GetType()}");
			}
		}
#endif
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
