using HutongGames.PlayMaker;
using ModCommon;
using Modding;
using SeanprCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.SceneManagement;
using static RandoMapMod.Resources;

namespace RandoMapMod
{
	public class RandoMapMod : Mod
	{
		//TODO:
		//		With skips enabled, set a special Pin sprite for each type of skip needed
		//		Pretty up the sprites
		//		Add a pin-sprite for Sequence Breaking
		//		HOLD - I need RandomizerMod to be elsewise in a few places, in order to ultimately clean this all up
		//			LogicManager all public'd, and maybe change the parser to accept a callback function instead of the list of 'obtained' items.
		//			Either the SaveSettings needs to hang onto the StringValues a bit longer before removing the actions, or all the action types need to be public'd.
		//		HOLD - Give the randomizer a custom end screen (
		//			Skip credits, 
		//			add hash [of randomizer actions?],
		//			location check count percentage [as opposed to masks, and other stuff...]
		//			checks per hour
		//		)
		//		HOLD - Add pins to the Legend??
		const float MAP_MIN_X = -24.16f;
		const float MAP_MAX_X = 17.3f;
		const float MAP_MIN_Y = -12.58548f;
		const float MAP_MAX_Y = 15.6913f;

		private static readonly DebugLog logger = new DebugLog(nameof(RandoMapMod));

		private GameObject custPinGroup = null;
		private GameMap theMap;
		private Resources resources = null;
		private MapTextOverlay mapTextOverlay = null; //TODO

		public SaveSettings Settings { get; set; } = new SaveSettings();
		public override ModSettings SaveSettings
		{
			get => Settings = Settings ?? new SaveSettings();
			set => Settings = value is SaveSettings saveSettings ? saveSettings : Settings;
		}

		public static RandoMapMod Instance
		{
			get; private set;
		}

		public static bool IsRando
		{
			get
			{
				//return true;
				return RandomizerMod.RandomizerMod.Instance.Settings.Randomizer;
			}
		}

		public override string GetVersion()
		{
			string ver = "0.5.0"; //If you update this, please also update the README.
			int minAPI = 45;

			bool apiTooLow = Convert.ToInt32(ModHooks.Instance.ModVersion.Split('-')[1]) < minAPI;
			if (apiTooLow)
			{
				return ver + " (Update API)";
			}

			return ver;
		}

		public override void Initialize()
		{
			if (Instance != null)
			{
				logger.Warn("Initialized twice... Stop that.");
				return;
			}
			Instance = this;
			logger.Log("RandoMapMod Initializing...");

			this.resources = new Resources();

			On.GameMap.Start += this.GameMap_Start;             //Set up custom pins
			On.GameMap.WorldMap += this.GameMap_WorldMap;         //Set big map boundaries
			On.GameMap.SetupMapMarkers += this.GameMap_SetupMapMarkers;   //Enable the custom pins
			On.GameMap.DisableMarkers += this.GameMap_DisableMarkers;   //Disable the custom pins

			//Giveaway time
			UnityEngine.SceneManagement.SceneManager.activeSceneChanged += HandleSceneChanges;
			ModHooks.Instance.LanguageGetHook += HandleLanguageGet;

			logger.Log("RandoMapMod Initialize complete!");
		}

		private void GameMap_Start(On.GameMap.orig_Start orig, GameMap self)
		{
			if (!IsRando)
			{
				orig(self);
				return;
			}
			try
			{
				logger.Log("Emptying out HelperData on game start.");
				LogicManager.helperData = new HelperData();
				if (this.mapTextOverlay == null)
				{
					logger.Log("Creating MapTextOverlay");
					this.mapTextOverlay = new MapTextOverlay();
				}
				//Create the custom pin group, and add all the new pins
				if (this.custPinGroup == null)
				{
					this.theMap = self;
					this.custPinGroup = new GameObject("Custom Pins");
					this.custPinGroup.AddComponent<PinGroup>();
					this.custPinGroup.transform.parent = self.transform;
					this.custPinGroup.transform.position = new Vector3(0f, 0f, 0f);
					this.custPinGroup.SetActive(false);

					foreach (PinData pin in this.resources.PinData().Values)
					{
						this.addPinToRoom(pin);
					}
				}
			} catch (Exception e)
			{
				logger.Error($"Error: {e}");
			}

			orig(self);
		}

		private void GameMap_WorldMap(On.GameMap.orig_WorldMap orig, GameMap self)
		{
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

		private void GameMap_SetupMapMarkers(On.GameMap.orig_SetupMapMarkers orig, GameMap self)
		{
			orig(self);
			if (!IsRando) {
				return;
			}
			try
			{
				using StreamReader file =
						new StreamReader(Path.Combine(Application.persistentDataPath, "RandomizerHelperLog.txt"));
				Either<String, HelperData> parseResults = new RandomizerHelperParser().parse(file);
				parseResults.Case(errorMessage =>
				{
					logger.Error($"Could not parse RandomizeHelperLog.txt: {errorMessage}");
				}, helperData =>
				{
					LogicManager.helperData = helperData;
					this.custPinGroup.SetActive(true);
					this.mapTextOverlay.Show(helperData);
				});
			} catch (Exception e)
			{
				logger.Error($"Failed to parse RandomizerHelper data: {e}");
			}
		}

		private void GameMap_DisableMarkers(On.GameMap.orig_DisableMarkers orig, GameMap self)
		{
			if (!IsRando)
			{
				orig(self);
				return;
			}
			try
			{
				//Dev.Log("UNMARK THE MAP!");
				this.custPinGroup.SetActive(false);
				this.mapTextOverlay.Hide();
			} catch (Exception e)
			{
				logger.Error($"Failed to DisableMarkers {e}");
			}
			orig(self);
		}

		private void addPinToRoom(PinData pin)
		{
			string roomName = pin.PinScene ?? this.resources.PinData()[pin.ID].SceneName;

			GameObject newPin = new GameObject("pin_rando");
			newPin.transform.parent = this.custPinGroup.transform;
			newPin.layer = 30;
			newPin.transform.localScale *= 1.2f;

			SpriteRenderer sr = newPin.AddComponent<SpriteRenderer>();
			if (pin.isShop)
			{
				sr.sprite = this.resources.Sprite(SpriteId.Shop);
			}
			else
			{
				sr.sprite = this.resources.Sprite(SpriteId.Rando);
			}
			sr.sortingLayerName = "HUD";
			sr.size = new Vector2(1f, 1f);

			Pin pinC = newPin.AddComponent<Pin>();
			pinC.PinData = pin;
			pinC.Resources = resources;

			Vector3 vec = this.getRoomPos(roomName) + pin.Offset;
			newPin.transform.localPosition = new Vector3(vec.x, vec.y, (vec.z - 0.5f));
		}

		private Vector3 getRoomPos(string prmRoomName)
		{
			//TODO: I should probably just remove this stupid thing; it's a waste of cycles. All
			//		I'd have to do is print out the .localPosition of each Pin, then update the XML
			//		with THOSE offsets instead... On the other hand, this way simplifies things...
			Vector3 pos = new Vector3(-30f, -30f, -0.5f);
			bool exitLoop = false;

			for (int index1 = 0; index1 < this.theMap.transform.childCount; ++index1)
			{
				GameObject gameObject1 = this.theMap.transform.GetChild(index1).gameObject;
				for (int index2 = 0; index2 < gameObject1.transform.childCount; ++index2)
				{
					GameObject gameObject2 = gameObject1.transform.GetChild(index2).gameObject;
					if (gameObject2.name == prmRoomName)
					{
						pos = gameObject2.transform.position;
						exitLoop = true;
						break;
					}
				}
				if (exitLoop)
				{
					break;
				}
			}

			return pos;
		}

		//Give compass, quill, maps
		private static int _SAFETY = 0;
		private const int SAFE = 3;
		private static bool _locked = false;

		private void HandleSceneChanges(Scene from, Scene to)
		{
			if (!IsRando)
			{
				return;
			}

			if (to.name == "Town")
			{
				_SAFETY = 0;
				_locked = false;
				PlayMakerFSM elder = FSMUtility.LocateFSM(GameObject.Find("Elderbug"), "npc_control");
				FsmState target = null;
				foreach (FsmState state in elder.FsmStates)
				{
					if (state.Name == "Convo End")
					{
						target = state;
						break;
					}
				}

				if (target != null)
				{
					List<FsmStateAction> actions = target.Actions.ToList();
					actions.Add(new ElderbugIsACoolDude());
					target.Actions = actions.ToArray();
				}
			}
			if (to.name == SceneNames.Menu_Title)
			{
				Settings.MapsGiven = false;
			}
			//throw new NotImplementedException();
		}

		private static void giveCollectorsMapIfGrubsAreNotRandomized()
		{
			if (RandomizerMod.RandomizerMod.Instance.Settings.RandomizeGrubs)
			{
				// Do nothing
			}
			else
			{
				PlayerData.instance.SetBool(nameof(PlayerData.instance.hasPinGrub), true);
			}
		}

		private string HandleLanguageGet(string key, string sheetTitle)
		{
			if (IsRando && _SAFETY < SAFE)
			{
				if (string.IsNullOrEmpty(key) || string.IsNullOrEmpty(sheetTitle))
				{
					return string.Empty;
				}

				if (sheetTitle == "Elderbug")
				{
					//if (_SAFETY == 0 && !Settings.MapsGiven)
					Dev.Log("Map Check! Safety: " + _SAFETY + ", locked: " + _locked + ", Key: " + key + ", " + Settings.MapsGiven);
					if (_SAFETY == 0 && key == "ELDERBUG_INTRO_MAIN" && !Settings.MapsGiven)
					{
						_SAFETY++;
						return "Welcome to RandoMapMod!\nA BIG pin means look there for progression. LITTLE means the next key item won't be there. \"!\" means you need something else, maybe grubs or a key? \"$\" indicates a shop that may have items.\nTalk to me 2 more times, and I'll give you all the maps.\nIf you're playing BINGO, you should probably not do that.";
					}
					else if (_SAFETY == 1 && !Settings.MapsGiven)
					{
						_SAFETY++;
						//return "I frequently *ahem* \"visit\" Cornifer's wife... She tells me he lies to travelers to get money for an inferior product... The ass. I've taken his completed originals. Maybe once they're bankrupt she'll run off with me.<page>I'll let you have the maps, the quill, and a compass since you're new around here if you talk to me 1 more time.";
						return "I'll let you have the maps, the quill, and a compass since you're new around here if you talk to me 1 more time.";

					}
					else if (_SAFETY == 2 && !Settings.MapsGiven)
					{
						string maps = "Okay hang on";
						for (int i = 0; i < 10; i++)
						{
							maps += "...\n...\n...\n...\n";
						}
						_SAFETY++;

						if (_SAFETY >= SAFE & !_locked)
						{
							PlayerData pd = PlayerData.instance;
							Type playerData = typeof(PlayerData);

							// Give the maps to the player
							pd.SetBool(nameof(pd.hasMap), true);

							foreach (FieldInfo field in playerData.GetFields().Where(field => field.Name.StartsWith("map") && field.FieldType == typeof(bool)))
							{
								pd.SetBool(field.Name, true);
							}

							//Give them compass and Quill
							pd.SetBool(nameof(pd.gotCharm_2), true);
							pd.SetBool(nameof(pd.hasQuill), true);
							giveCollectorsMapIfGrubsAreNotRandomized();

							// Set cornifer as having left all the areas. This could be condensed into the previous foreach for one less GetFields(), but I value the clarity more.
							foreach (FieldInfo field in playerData.GetFields().Where(field => field.Name.StartsWith("corn") && field.Name.EndsWith("Left")))
							{
								pd.SetBool(field.Name, true);
							}

							// Set Cornifer as sleeping at home
							pd.SetBool(nameof(pd.corniferAtHome), true);

							_locked = true;
						}
						maps += "<page> ...Here you go!";
						return maps;
					}
					else
					{
						return Language.Language.GetInternal(key, sheetTitle);
					}
				}
				else if (!_locked && sheetTitle == "Titles" && key == "DIRTMOUTH_MAIN")
				{
					return "FREE MAPS";
				}
				else if (!_locked && sheetTitle == "Titles" && key == "DIRTMOUTH_SUB")
				{
					return "Talk to Elderbug";
				}
			}
			return Language.Language.GetInternal(key, sheetTitle);
		}

		private class ElderbugIsACoolDude : FsmStateAction
		{

			public override void OnEnter()
			{
				//_SAFETY++;

				if (_SAFETY >= SAFE & !_locked)
				{

					PlayerData pd = PlayerData.instance;
					Type playerData = typeof(PlayerData);

					// Give the maps to the player
					pd.SetBool(nameof(pd.hasMap), true);

					foreach (FieldInfo field in playerData.GetFields().Where(field => field.Name.StartsWith("map") && field.FieldType == typeof(bool)))
					{
						pd.SetBool(field.Name, true);
					}

					//Give them compass and Quill
					pd.SetBool(nameof(pd.gotCharm_2), true);
					pd.SetBool(nameof(pd.hasQuill), true);
					giveCollectorsMapIfGrubsAreNotRandomized();

					// Set cornifer as having left all the areas. This could be condensed into the previous foreach for one less GetFields(), but I value the clarity more.
					foreach (FieldInfo field in playerData.GetFields().Where(field => field.Name.StartsWith("corn") && field.Name.EndsWith("Left")))
					{
						pd.SetBool(field.Name, true);
					}

					// Set Cornifer as sleeping at home
					pd.SetBool(nameof(pd.corniferAtHome), true);

					_locked = true;
				}

				Finish();
			}
		}
	}
}