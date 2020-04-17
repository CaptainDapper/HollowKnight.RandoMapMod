using HutongGames.PlayMaker;
using ModCommon;
using Modding;
using On;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
			string ver = "0.3.5";
			int minAPI = 45;

			bool apiTooLow = Convert.ToInt32( ModHooks.Instance.ModVersion.Split( '-' )[1] ) < minAPI;
			if ( apiTooLow ) {
				return ver + " (Update API)";
			}

			return ver;
		}

		public override void Initialize() {
			if ( Instance != null ) {
				DebugLog.Warn( "Initialized twice... Stop that." );
				return;
			}
			Instance = this;
			DebugLog.Log( "RandoMapMod Initializing..." );

			Resources.Initialize();
			
			On.GameMap.Start += this.GameMap_Start;							//Set up custom pins
			On.GameMap.WorldMap += this.GameMap_WorldMap;					//Set big map boundaries
			On.GameMap.SetupMapMarkers += this.GameMap_SetupMapMarkers;		//Enable the custom pins
			On.GameMap.DisableMarkers += this.GameMap_DisableMarkers;		//Disable the custom pins

			ModHooks.Instance.SavegameLoadHook += this.SavegameLoadHook;	//Load object name changes
			ModHooks.Instance.SavegameSaveHook += this.SavegameSaveHook;    //Load object name changes

			//Giveaway time
			UnityEngine.SceneManagement.SceneManager.activeSceneChanged += HandleSceneChanges;
			ModHooks.Instance.LanguageGetHook += HandleLanguageGet;

			DebugLog.Log("RandoMapMod Initialize complete!");
		}

		private void SavegameLoadHook( int slot ) {
			ObjectNames.Load(slot);
		}

		private void SavegameSaveHook( int slot ) {
			ObjectNames.Load(slot);
		}

		private void GameMap_Start( On.GameMap.orig_Start orig, GameMap self ) {
			if ( !IsRando ) {
				orig( self );
				return;
			}
            //Dev.Log("START THE MAP!");
            LogicManager.reachableItems = new List<string>();
            LogicManager.checkedItems = new List<string>();
            //Create the custom pin group, and add all the new pins
            if ( this.custPinGroup == null ) {
				this.theMap = self;

				this.custPinGroup = new GameObject( "Custom Pins" );
				this.custPinGroup.AddComponent<PinGroup>();
				this.custPinGroup.transform.parent = self.transform;
				this.custPinGroup.transform.position = new Vector3( 0f, 0f, 0f );
				this.custPinGroup.SetActive( false );

				foreach ( PinData pin in PinData_S.All.Values ) {
					this.addPinToRoom( pin );
				}
			}

			orig( self );
		}

		private void GameMap_WorldMap( On.GameMap.orig_WorldMap orig, GameMap self ) {
			orig( self );
			if ( !IsRando )
				return;
            //Dev.Log("WORLD THE MAP!");
            //Set the maximum scroll boundaries, so we can scroll the entire map, even if we don't have the maps unlocked.
            if ( self.panMinX > MAP_MIN_X )
				self.panMinX = MAP_MIN_X;
			if ( self.panMaxX < MAP_MAX_X )
				self.panMaxX = MAP_MAX_X;
			if ( self.panMinY > MAP_MIN_Y )
				self.panMinY = MAP_MIN_Y;
			if ( self.panMaxY < MAP_MAX_Y )
				self.panMaxY = MAP_MAX_Y;
		}

		private void GameMap_SetupMapMarkers( On.GameMap.orig_SetupMapMarkers orig, GameMap self ) {
			orig( self );
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
                                Dev.Log("Reachable: " + item);
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
            //Dev.Log("UNMARK THE MAP!");
            this.custPinGroup.SetActive( false );

			orig( self );
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
			    sr.sprite = Resources.Sprite( "Map.randoPin" );
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

		private void HandleSceneChanges( Scene from, Scene to ) {
			if ( !IsRando ) {
				return;
			}

			//DebugLog.Log( "New Scene: " + to.name );
			if ( to.name == "Town" ) {
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
			//throw new NotImplementedException();
		}
		
		private string HandleLanguageGet( string key, string sheetTitle ) {
			if ( IsRando && _SAFETY < SAFE ) {
				if ( string.IsNullOrEmpty( key ) || string.IsNullOrEmpty( sheetTitle ) ) {
					return string.Empty;
				}

				if ( sheetTitle == "Elderbug" ) {
					if ( _SAFETY == 0 && key == "ELDERBUG_INTRO_MAIN" ) {
						return "Welcome to RandoMapMod!\nA BIG pin means look there for progression. LITTLE means the next key item won't be there. \"!\" means you need something else, maybe grubs or a key? \"$\" indicates a shop that may have items.\nTalk to me 2 more times, and I'll give you all the maps.\nIf you're playing BINGO, you should probably not do that.";
					} else if ( _SAFETY == 1 ) {
						//return "I frequently *ahem* \"visit\" Cornifer's wife... She tells me he lies to travelers to get money for an inferior product... The ass. I've taken his completed originals. Maybe once they're bankrupt she'll run off with me.<page>I'll let you have the maps, the quill, and a compass since you're new around here if you talk to me 1 more time.";
                        return "I'll let you have the maps, the quill, and a compass since you're new around here if you talk to me 1 more time.";

                    } else if ( _SAFETY == 2 ) {
						string maps = "Okay hang on";
						for ( int i = 0; i < 10; i++ ) {
							maps += "...\n...\n...\n...\n";
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
			private static bool locked = false;

			public override void OnEnter() {
				_SAFETY++;

				if ( _SAFETY >= SAFE & !locked ) {
					PlayerData pd = PlayerData.instance;
					pd.SetBool( "gotCharm_2", true );
					pd.SetBool( "hasMap", true );
					pd.SetBool( "hasQuill", true );
					pd.SetBool( "mapDirtmouth", true );
					pd.SetBool( "mapCrossroads", true );
					pd.SetBool( "mapGreenpath", true );
					pd.SetBool( "mapFogCanyon", true );
					pd.SetBool( "mapRoyalGardens", true );
					pd.SetBool( "mapFungalWastes", true );
					pd.SetBool( "mapCity", true );
					pd.SetBool( "mapWaterways", true );
					pd.SetBool( "mapMines", true );
					pd.SetBool( "mapDeepnest", true );
					pd.SetBool( "mapCliffs", true );
					pd.SetBool( "mapOutskirts", true );
					pd.SetBool( "mapRestingGrounds", true );
					pd.SetBool( "mapAbyss", true );
					pd.SetBool( "mapAllRooms", true );

					locked = true;
				}

				Finish();
			}
		}
	}
}
