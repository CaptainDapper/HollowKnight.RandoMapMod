using HutongGames.PlayMaker;
using ModCommon;
using Modding;
using On;
using SeanprCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace RandoMapMod {
	public class RandoMapMod : Mod {
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

		private GameObject custPinGroup = null;
		private GameMap theMap;

		public SaveSettings Settings { get; set; } = new SaveSettings();
		public override ModSettings SaveSettings
		{
			get => Settings = Settings ?? new SaveSettings();
			set => Settings = value is SaveSettings saveSettings ? saveSettings : Settings;
		}


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

		public static bool IsRando {
			get {
				//return true;
				return RandomizerMod.RandomizerMod.Instance.Settings.Randomizer;
			}
		}

		public override string GetVersion() {
			string ver = "0.3.11";
			int minAPI = 45;

			bool apiTooLow = Convert.ToInt32(ModHooks.Instance.ModVersion.Split('-')[1]) < minAPI;
			if (apiTooLow) {
				return ver + " (Update API)";
			}

			return ver;
		}

		public override void Initialize() {
			if (Instance != null) {
				DebugLog.Warn("Initialized twice... Stop that.");
				return;
			}
			Instance = this;
			DebugLog.Log("RandoMapMod Initializing...");

			Resources.Initialize();

			On.GameMap.OnEnable += this.GameMap_OnEnable;
			On.GameMap.Start += this.GameMap_Start;                         //Set up custom pins
			On.GameMap.WorldMap += this.GameMap_WorldMap;                   //Set big map boundaries
			//On.GameMap.Update += this.GameMap_Update;
			On.GameMap.SetupMapMarkers += this.GameMap_SetupMapMarkers;     //Enable the custom pins
			On.GameMap.DisableMarkers += this.GameMap_DisableMarkers;       //Disable the custom pins

			//ModHooks.Instance.NewGameHook += this.NewGameHook;
			ModHooks.Instance.SavegameLoadHook += this.SavegameLoadHook;    //Load object name changes
			ModHooks.Instance.SavegameSaveHook += this.SavegameSaveHook;    //Load object name changes

			//Giveaway time
			UnityEngine.SceneManagement.SceneManager.activeSceneChanged += HandleSceneChanges;
			ModHooks.Instance.LanguageGetHook += HandleLanguageGet;

			DebugLog.Log("RandoMapMod Initialize complete!");
		}

		//private void NewGameHook()
		//{
		//	Pin.Reset();
		//	Pin.Setup();
		//}

		private void SavegameLoadHook(int slot) {
			ObjectNames.Load(slot);
		}

		private void SavegameSaveHook(int slot) {
			ObjectNames.Load(slot);
		}

		private List<string> InteriorNameList = new List<string>()
		{
			"Fungus1_35",
			"Fungus1_36",
			"Mines_35",
			"Room_Fungus_Shaman",
			"Ruins_Elevator",
			"Crossroads_ShamanTemple",
			"Room_GG_Shortcut"
		};
		private bool isInInterior = false;
		private string interiorName = "";
		private void GameMap_Start(On.GameMap.orig_Start orig, GameMap self) {
			//self.areaGreenpath.transform.Translate(new Vector3(200, 200));
			//self.GetComponent<Renderer>().enabled = false;
			//self.enabled = false;
			//self.areaGreenpath.
			//var roomMaterial = UnityEngine.Object.Instantiate(self.areaCliffs.transform.GetChild(1).GetComponent<SpriteRenderer>().material);

			//GameObject testRoom = new GameObject("Fungus1_35", typeof(SpriteRenderer), typeof(RoughMapRoom));
			//testRoom.transform.SetParent(self.areaGreenpath.transform);
			//testRoom.layer = 31;
			//testRoom.transform.localScale = Vector3.one;
			//testRoom.SetActive(true);
			//var spriteRenderer = testRoom.GetComponent<SpriteRenderer>();
			//spriteRenderer.material = roomMaterial;
			//spriteRenderer.sprite = Resources.Sprite("Map.TestRoom");
			////spriteRenderer.sortingLayerID = 
			//spriteRenderer.sortingOrder = 1337;
			//var roughMapRoom = testRoom.GetComponent<RoughMapRoom>();
			//roughMapRoom.fullSprite = spriteRenderer.sprite;
			//testRoom.transform.localPosition = self.areaGreenpath.transform.Find("Fungus1_34").transform.localPosition;

			//GameObject testRoomSprite = new GameObject("RF134", typeof(SpriteRenderer));
			//testRoomSprite.transform.SetParent(testRoom.transform);
			//testRoomSprite.transform.localPosition = Vector3.zero;
			//testRoomSprite.layer = 31;
			//testRoomSprite.transform.localScale = Vector3.one;
			//testRoomSprite.SetActive(true);
			//var spriteSpriteRenderer = testRoomSprite.GetComponent<SpriteRenderer>();
			//spriteSpriteRenderer.material = roomMaterial;
			//spriteSpriteRenderer.sprite = Resources.Sprite("Map.TestRoom");
			//spriteSpriteRenderer.sortingOrder = 1337;
			//if (PlayerData.instance.GetVariable<List<string>>(nameof(PlayerData.instance.scenesMapped)).Contains(testRoom.name))
			//	PlayerData.instance.GetVariable<List<string>>(nameof(PlayerData.instance.scenesMapped)).Add(testRoom.name);

			//self.areaGreenpath.transform.Find("Fungus1_34").transform.localPosition = new Vector3(2000, 2000);

			if (!IsRando) {
				orig(self);
				return;
			}
			Dev.Log("START THE MAP!");

			LogicManager.reachableItems = new List<string>();
			LogicManager.checkedItems = new List<string>();
			//Create the custom pin group, and add all the new pins
			if (this.custPinGroup == null) {
				this.theMap = self;

				this.custPinGroup = new GameObject("Custom Pins");
				this.custPinGroup.AddComponent<PinGroup>();
				this.custPinGroup.transform.parent = self.transform;
				this.custPinGroup.transform.position = new Vector3(0f, 0f, 0f);
				this.custPinGroup.SetActive(false);

				foreach (PinData pin in PinData_S.All.Values) {
					this.addPinToRoom(pin);
				}
				//PinData test = new PinData();
				//test.NewX = 
			}

			orig(self);
		}

		private void GameMap_OnEnable(On.GameMap.orig_OnEnable orig, GameMap self)
		{
			//Dev.Log("ENABLE ZE MAP! Is it Interior? " + isInInterior);
			//if (isInInterior)
			//{

			//}
			//else
			orig(self);


		}


		private void GameMap_WorldMap(On.GameMap.orig_WorldMap orig, GameMap self) {
			//string sceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
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

		private void GameMap_Update(On.GameMap.orig_Update orig, GameMap self)
		{
			orig(self);
		}
		//List<Vector3> blueMarkers = new List<Vector3>();
		private void GameMap_SetupMapMarkers( On.GameMap.orig_SetupMapMarkers orig, GameMap self ) {
			bool hasCollectorsMap = PlayerData.instance.GetBoolInternal("hasPinGrub");
			if (IsRando && hasCollectorsMap)
				PlayerData.instance.SetBoolInternal("hasPinGrub", false);
			orig( self );
			if (IsRando && hasCollectorsMap)
				PlayerData.instance.SetBoolInternal("hasPinGrub", true);
			if ( !IsRando )
				return;

			//Dev.Log("MARK THE MAP!");
			// Parse Helper Log
			//File.AppendAllText(Path.Combine(Application.persistentDataPath, "RandomizerHelperLog.txt"), message + Environment.NewLine);
			string line;
            bool hitReachables = false;
            bool hitChecked = false;
            System.IO.StreamReader file =
                new System.IO.StreamReader(Path.Combine(Application.persistentDataPath, "RandomizerHelperLog.txt"));
            //Dev.Log("Sly MS1" + PlayerData.instance.slyShellFrag1);
            //Dev.Log("Sly MS2" + PlayerData.instance.slyShellFrag2);
            //Dev.Log("Sly MS3" + PlayerData.instance.slyShellFrag3);
            //Dev.Log("Sly MS4" + PlayerData.instance.slyShellFrag4);
            //Dev.Log("Sly SK" + PlayerData.instance.slySimpleKey);
            //Dev.Log("Sly VF1" + PlayerData.instance.slyVesselFrag1);
            //Dev.Log("Sly VF2" + PlayerData.instance.slyVesselFrag2);
            //Dev.Log("Sly VF3" + PlayerData.instance.slyVesselFrag3);
            //Dev.Log("Sly VF4" + PlayerData.instance.slyVesselFrag4);
            //Dev.Log("Sly CN1" + PlayerData.instance.slyNotch1);
            //Dev.Log("Sly CN2" + PlayerData.instance.slyNotch2);
            //Dev.Log("Sly RE" + PlayerData.instance.slyRancidEgg);
            //Dev.Log("Dream Reward1: " + PlayerData.instance.dreamReward1);
            //Dev.Log("Dream Reward2: " + PlayerData.instance.dreamReward2);
            //Dev.Log("Dream Reward3: " + PlayerData.instance.dreamReward3);
            //Dev.Log("Dream Reward4: " + PlayerData.instance.dreamReward4);
            //Dev.Log("Dream Reward5: " + PlayerData.instance.dreamReward5);
            //Dev.Log("Dream Reward5b: " + PlayerData.instance.dreamReward5b);
            //Dev.Log("Dream Reward6: " + PlayerData.instance.dreamReward6);
            //Dev.Log("Dream Reward7: " + PlayerData.instance.dreamReward7);
            //Dev.Log("Dream Reward8: " + PlayerData.instance.dreamReward8);
            //Dev.Log("Dream Reward9: " + PlayerData.instance.dreamReward9);

            //Dev.Log("Shop Costs");
            //foreach((string, int) cost in RandomizerMod.RandomizerMod.Instance.Settings.ShopCosts)
            //{
            //    Dev.Log(cost.Item1 + ":" + cost.Item2);
            //    //Dev.Log(PlayerData.instance.)
            //    //ShopMenuStock stock = GameObject.Find("Shop Menu").GetComponent<ShopMenuStock>();
            //}
            //Dev.Log("Variable Costs");
            //foreach ((string, int) cost in RandomizerMod.RandomizerMod.Instance.Settings.VariableCosts)
            //{
            //    Dev.Log(cost.Item1 + ":" + cost.Item2);
            //}
            //Dev.Log("Loaded Helper Log? " + (file != null));
            while ((line = file.ReadLine()) != null)
            {
                //Dev.Log("File Line: " + line);
                if (hitChecked)
                {
                    if (line.StartsWith(" - "))
                    {
                        // if shop is in checked, the pins all shrink, so this fixes that
                        if (shopNames.Contains(line.Substring(3)))
                        {
                            List<string> shopItems = new List<string>();
                            switch (line.Substring(3))
                            {
                                case "Sly":
                                    //Dev.Log("Sly is open!");
                                    shopItems = slyItems.ToList();
                                    break;
                                case "Sly (Key)":
                                    //Dev.Log("Sly (Key) is open!");
                                    shopItems = slyKeyItems.ToList();
                                    break;
                                case "Iselda":
                                    //Dev.Log("Iselda is open!");
                                    shopItems = iseldaItems.ToList();
                                    break;
                                case "Salubra":
                                    //Dev.Log("Salubra is open!");
                                    shopItems = salubraItems.ToList();
                                    break;
                                case "Leg Eater":
                                    //Dev.Log("Leg Eater is open!");
                                    shopItems = legEaterItems.ToList();
                                    break;
                                case "Grubfather":
                                    //Dev.Log("Grubfater is open!");
                                    shopItems = grubfatherItems.ToList();
                                    break;
                                case "Seer":
                                    //Dev.Log("Seer is open!");
                                    shopItems = seerItems.ToList();
                                    break;
                            }
                            foreach (string item in shopItems)
                            {
                                //Dev.Log("Reachable: " + item);
                                LogicManager.reachableItems.Add(item);
                            }
                        }
                        else
                        {
                            //Dev.Log("Checked: " + line.Substring(3));
                            LogicManager.checkedItems.Add(line.Substring(3));
                        }
                    }
                } else
                if (hitReachables)
                {
                    if (line.StartsWith(" - "))
                    {
                        if (shopNames.Contains(line.Substring(3)))
                        {
                            List<string> shopItems = new List<string>();
                            switch (line.Substring(3))
                            {
                                case "Sly":
                                    //Dev.Log("Sly is open!");
                                    shopItems = slyItems.ToList();
                                    break;
                                case "Sly (Key)":
                                    //Dev.Log("Sly (Key) is open!");
                                    shopItems = slyKeyItems.ToList();
                                    break;
                                case "Iselda":
                                    //Dev.Log("Iselda is open!");
                                    shopItems = iseldaItems.ToList();
                                    break;
                                case "Salubra":
                                    //Dev.Log("Salubra is open!");
                                    shopItems = salubraItems.ToList();
                                    break;
                                case "Leg Eater":
                                    //Dev.Log("Leg Eater is open!");
                                    shopItems = legEaterItems.ToList();
                                    break;
                                case "Grubfather":
                                    //Dev.Log("Grubfater is open!");
                                    shopItems = grubfatherItems.ToList();
                                    break;
                                case "Seer":
                                    //Dev.Log("Seer is open!");
                                    shopItems = seerItems.ToList();
                                    break;
                            }
                            foreach (string item in shopItems)
                            {
                                //Dev.Log("Reachable: " + item);
                                LogicManager.reachableItems.Add(item);
                            }
                        }
                        else
                        {
                            //Dev.Log("Reachable: " + line.Substring(3));
                            LogicManager.reachableItems.Add(line.Substring(3));
                        }
                    }
                }
                if (line.Contains("REACHABLE ITEM LOCATIONS"))
                {
                    LogicManager.reachableItems = new List<string>();
                    hitReachables = true;
                }
                if (line.Contains("CHECKED ITEM LOCATIONS"))
                {
                    LogicManager.checkedItems = new List<string>();
                    hitChecked = true;
                }
            }

            file.Close();
			//for (int i = 1; i >= 40; i++)
			//{
			//    PlayerData.instance.GetType().GetProperty("charmCost_" + i).SetValue(PlayerData.instance, 1, null);
			//}

			//foreach(string reachable in LogicManager.reachableItems)
			//{
			//    Dev.Log("REACHABLE ITEM: +" + reachable + "+");
			//}

			//foreach (string check in LogicManager.checkedItems)
			//{
			//    Dev.Log("CHECKED ITEM: +" + check + "+");
			//}
			//Dev.Log("Variable Costs: ");
			//foreach((string, int) shopCost in RandomizerMod.RandomizerMod.Instance.Settings.VariableCosts)
			//{
			//    Dev.Log(shopCost.Item1 + ": " + shopCost.Item2);
			//}
			this.custPinGroup.SetActive( true );
		}

		private void GameMap_DisableMarkers( On.GameMap.orig_DisableMarkers orig, GameMap self ) {
			if ( !IsRando ) {
				orig( self );
				return;
			}
			
			orig(self);
			this.custPinGroup.SetActive(false);
			//blueMarkers = PlayerData.instance.placedMarkers_b;
			//PlayerData.instance.placedMarkers_b = new List<Vector3>();
			//foreach (GameObject marker in self.mapMarkersBlue)
			//{
			//	marker.SetActive(false);
			//}
			//foreach (GameObject marker in self.mapMarkersRed)
			//{
			//	marker.SetActive(false);
			//}
			//foreach (GameObject marker in self.mapMarkersWhite)
			//{
			//	marker.SetActive(false);
			//}
			//foreach (GameObject marker in self.mapMarkersYellow)
			//{
			//	marker.SetActive(false);
			//}
			//Dev.Log("UNMARK THE MAP!");
			//Dev.Log("Current Hero Map Position: " + (this.custPinGroup.transform.position.x- self.compassIcon.transform.position.x) + ", " + (this.custPinGroup.transform.position.y- self.compassIcon.transform.position.y));


			//Dev.Log("Current Hero Map Position: " + self.compassIcon.transform.position.x + ", " + self.compassIcon.transform.position.y);
			//Dev.Log("Current Hero Map Position: " + self.mapMarkersBlue[0].transform.position.x + ", " + self.mapMarkersBlue[0].transform.position.y);
			//self.

			//orig( self );
		}

		private void addPinToRoom( PinData pin ) {
			string roomName = (pin.PinScene != null) ? pin.PinScene : PinData_S.All[pin.ID].SceneName;

			GameObject newPin = new GameObject( "pin_rando" );
			newPin.transform.parent = this.custPinGroup.transform;
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
			sr.size = new Vector2( 1f, 1f );

			Pin pinC = newPin.AddComponent<Pin>();
			pinC.PinData = pin;

			Vector3 vec = this.getRoomPos( roomName ) + pin.Offset;
			newPin.transform.localPosition = new Vector3( vec.x, vec.y, (vec.z - 0.5f) );
		}

		private Vector3 getRoomPos( string prmRoomName ) {
			//TODO: I should probably just remove this stupid thing; it's a waste of cycles. All
			//		I'd have to do is print out the .localPosition of each Pin, then update the XML
			//		with THOSE offsets instead... On the other hand, this way simplifies things...
			Vector3 pos = new Vector3( -30f, -30f, -0.5f );
			bool exitLoop = false;

			for ( int index1 = 0; index1 < this.theMap.transform.childCount; ++index1 ) {
				GameObject gameObject1 = this.theMap.transform.GetChild( index1 ).gameObject;
				for ( int index2 = 0; index2 < gameObject1.transform.childCount; ++index2 ) {
					GameObject gameObject2 = gameObject1.transform.GetChild( index2 ).gameObject;
					if ( gameObject2.name == prmRoomName ) {
						pos = gameObject2.transform.position;
						exitLoop = true;
						break;
					}
				}
				if ( exitLoop ) {
					break;
				}
			}

			return pos;
		}







		//Give compass, quill, maps
		private static int _SAFETY = 0;
		private const int SAFE = 3;
		private static bool _locked = false;

		private void HandleSceneChanges( Scene from, Scene to ) {
			if ( !IsRando ) {
				return;
			}

			if ( to.name == "Town" ) {
				_SAFETY = 0;
				_locked = false;
				PlayMakerFSM elder = FSMUtility.LocateFSM( GameObject.Find( "Elderbug" ), "npc_control" );
				FsmState target = null;
				foreach ( FsmState state in elder.FsmStates ) {
					if ( state.Name == "Convo End" ) {
						target = state;
						break;
					}
				}

				if ( target != null ) {
					List<FsmStateAction> actions = target.Actions.ToList();
					actions.Add( new ElderbugIsACoolDude() );
					target.Actions = actions.ToArray();
				}
			}
			if (to.name == SceneNames.Menu_Title)
			{
				Settings.MapsGiven = false;
				Pin.Reset();
			}
			if (from.name == SceneNames.Menu_Title)
			{
				Pin.Reset();
				Pin.Setup();
			}
			//if (InteriorNameList.Contains(to.name) || to.name.Contains("White_Palace"))
			//{
			//	isInInterior = true;
			//	if (to.name.Contains("White_Palace"))
			//		interiorName = "White Palace";
			//	else
			//		switch (to.name)
			//		{
			//			case "Fungus1_35":
			//			case "Fungus1_36":
			//				Dev.Log("MAP Stone Sancturary");
			//				interiorName = "Stone Sancturary";
			//				break;

			//			case "Mines_35":
			//				Dev.Log("MAP Crystalized Mound");
			//				interiorName = "Crystalized Mound";
			//				break;

			//			case "Room_Fungus_Shaman":
			//				Dev.Log("MAP Overgrown Mound");
			//				interiorName = "Overgrown Mound";
			//				break;

			//			case "Ruins_Elevator":
			//				Dev.Log("MAP Pleasure House Elevator");
			//				interiorName = "Pleasure House Elevator";
			//				break;

			//			case "Crossroads_ShamanTemple":
			//				Dev.Log("MAP Ancestral Mound");
			//				interiorName = "Ancestral Mound";
			//				break;

			//			case "Room_GG_Shortcut":
			//				Dev.Log("MAP Fluke Hermit");
			//				interiorName = "Fluke Hermit";
			//				break;
			//		}
			//}
			//else
			//{
			//	isInInterior = true;
			//	interiorName = "";
			//}

			//throw new NotImplementedException();
		}
		
		private string HandleLanguageGet( string key, string sheetTitle ) {
			if ( IsRando && _SAFETY < SAFE ) {
				if ( string.IsNullOrEmpty( key ) || string.IsNullOrEmpty( sheetTitle ) ) {
					return string.Empty;
				}

				if ( sheetTitle == "Elderbug" ) {
					//if (_SAFETY == 0 && !Settings.MapsGiven)
					Dev.Log("Map Check! Safety: " + _SAFETY + ", locked: " + _locked + ", Key: " + key + ", " + Settings.MapsGiven);
					if ( _SAFETY == 0 && key == "ELDERBUG_INTRO_MAIN" && !Settings.MapsGiven)
					{
						_SAFETY++;
						return "Welcome to RandoMapMod!\n\n\n\nA large \"?\" pin means look there for progression. A small \"?\" means the next key item won't be there. A \"!\" means that you need additional Grubs or Essence. A large \"$\" indicates a shop is accessible and may have items remaining.\nIn addition, non-randomized Grub and Essence sources now have new pins that indicate whether or not they are currently accessible in logic! As with other pin types, large pins are accessible, small pins are not.\n\nTalk to me 2 more times, and I'll give you all the maps.\nIf you're playing BINGO, you should probably not do that.";
					} else if ( _SAFETY == 1 && !Settings.MapsGiven) {
						_SAFETY++;
						//return "I frequently *ahem* \"visit\" Cornifer's wife... She tells me he lies to travelers to get money for an inferior product... The ass. I've taken his completed originals. Maybe once they're bankrupt she'll run off with me.<page>I'll let you have the maps, the quill, and a compass since you're new around here if you talk to me 1 more time.";
						return "I'll let you have the maps, the quill, and a compass since you're new around here if you talk to me 1 more time.";

                    } else if ( _SAFETY == 2 && !Settings.MapsGiven) {
						string maps = "Okay hang on";
						for ( int i = 0; i < 10; i++ ) {
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

							// Set cornifer as having left all the areas. This could be condensed into the previous foreach for one less GetFields(), but I value the clarity more.
							foreach (FieldInfo field in playerData.GetFields().Where(field => field.Name.StartsWith("corn") && field.Name.EndsWith("Left")))
							{
								pd.SetBool(field.Name, true);
							}

							// Set Cornifer as sleeping at home
							pd.SetBool(nameof(pd.corniferAtHome), true);

							_locked = true;
						}
						//maps += "<page>...Here you go! Now, if you'd keep Iselda's infidelity to yourself, I won't have to kill you. Hm, don't you wonder how the King died...?";
						maps += "<page> ...Here you go!";
                        return maps;
					} else {
						return Language.Language.GetInternal( key, sheetTitle );
					}
				} else if ( sheetTitle == "Titles" && key == "DIRTMOUTH_MAIN" ) {
					return "FREE MAPS";
				} else if ( sheetTitle == "Titles" && key == "DIRTMOUTH_SUB" ) {
					return "Talk to Elderbug";
				}
			}

			return Language.Language.GetInternal( key, sheetTitle );
		}

		private class ElderbugIsACoolDude : FsmStateAction {

			public override void OnEnter() {
				//_SAFETY++;

				if ( _SAFETY >= SAFE & !_locked ) {
					
					PlayerData pd = PlayerData.instance;
					Type playerData = typeof(PlayerData);

					// Give the maps to the player
					pd.SetBool(nameof(pd.hasMap), true);
					
					foreach (FieldInfo field in playerData.GetFields().Where(field => field.Name.StartsWith("map") && field.FieldType == typeof(bool)))
					{
						pd.SetBool(field.Name, true);
					}
					
					//Give them compass and Quill
					pd.SetBool( nameof(pd.gotCharm_2), true );
					pd.SetBool( nameof(pd.hasQuill), true );
					
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
