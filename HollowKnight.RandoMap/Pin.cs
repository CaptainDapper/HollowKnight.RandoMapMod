using ModCommon;
using RandoMapMod;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

class Pin : MonoBehaviour {
	private PinData pinData;
	private bool isPossible = true;
	private bool isPrereqMet = true;

	private SpriteRenderer sr = null;
	private Sprite origSprite = null;
	private Vector3 origScale;
	private Color origColor;

	public PinData PinData {
		set {
			this.pinData = value;
		}
	}
	private static bool areSettingsLoaded = false;
	public static Dictionary<string, bool> IsRandomizedInSettings = new Dictionary<string, bool>();
	private static List<(string,string)> RandomizableItems = new List<(string,string)>
	{
		("RandomizeDreamers", "Dreamer"),
		("RandomizeSkills", "Skill"),
		("RandomizeCharms", "Charm"),
		("RandomizeKeys", "Key"),
		("RandomizeGeoChests", "Geo"),
		("RandomizeMaskShards", "Mask"),
		("RandomizeVesselFragments", "Vessel"),
		("RandomizeCharmNotches", "Notch"),
		("RandomizePaleOre", "Ore"),
		("RandomizeRancidEggs", "Egg"),
		("RandomizeRelics", "Relic"),
		("RandomizeMaps", "Map"),
		("RandomizeStags", "Stag"),
		("RandomizeGrubs", "Grub"),
		("RandomizeWhisperingRoots", "Root"),
		("RandomizeRocks", "Rock"),
		("RandomizeSoulTotems", "Soul"),
		("RandomizePalaceTotems", "PalaceSoul"),
		("RandomizeLoreTablets", "Lore"),
		("RandomizeLifebloodCocoons", "Cocoon"),
		("RandomizeGrimmkinFlames", "Flame"),
		("RandomizeBossEssence", "Essence_Boss")
		
	};
	//private static Type managerType = typeof(RandomizerMod.Randomization.ProgressionManager);

	public static void Setup() {
		//Dev.Log("Entering setup");
		//IsRandomizedInSettings = new Dictionary<string, bool>();
		Type settings = typeof(RandomizerMod.SaveSettings);
		foreach ((string, string) randomizable in RandomizableItems)
		{
			//Dev.Log(randomizable.Item1 + " " + randomizable.Item2);
			PropertyInfo property = settings.GetProperty(randomizable.Item1);
			//Dev.Log(" Property is " + property);
			if (property != null)
			{
				//Dev.Log(" Object randomizable " + randomizable.Item1 + " , " + randomizable.Item2);
				IsRandomizedInSettings.Add(randomizable.Item2, (bool)property.GetValue(RandomizerMod.RandomizerMod.Instance.Settings, null));
				//Dev.Log(" " + RandomizerMod.RandomizerMod.Instance.Settings.RandomizeDreamers);
				
			}
		}
		//Dev.Log("Completing setup");
		areSettingsLoaded = true;
	}

	public static void Reset()
	{
		IsRandomizedInSettings = new Dictionary<string, bool>();
		areSettingsLoaded = false;
	}


	void Awake() {
		this.sr = this.gameObject.GetComponent<SpriteRenderer>();
		this.origSprite = this.sr.sprite;
		this.origScale = this.transform.localScale;
		this.origColor = this.sr.color;
	}

	void OnEnable() {
		try {
			//Set Pin's display state according to location's logic.
            //this.setLogicState(RandomizerMod.Randomization)
			this.setLogicState( this.pinData.Possible );

			if ( this.isPossible ) {
				//Set Pin state according to prereqs.
				this.setPrereqState( this.pinData.PreReqMet );
			}
		} catch ( Exception e ) {
			DebugLog.Error( e.ToString() );
		}

        //Disable Pin if we've already obtained / checked this location.
        if (LogicManager.checkedItems.Contains(this.pinData.ID.Replace('_', ' ')))
        {
            //Dev.Log(this.pinData.ID + " is checked");
            this.disableSelf();
        }
        else
        {
			//Dev.Log("RAWR");
			string pool = PinData_S.All[this.pinData.ID].Pool;
			if (pool == null)
				return;
			//Dev.Log("Pin pool: " + pool);
			bool isRandomized = false;
			//if (!areSettingsLoaded)
			//	Setup();
			if (pool != null)
			{
				IsRandomizedInSettings.TryGetValue(pool, out isRandomized);
				//Dev.Log("Pool " + pool + " is randomized? " + isRandomized);
			}
            
            if (!isRandomized)
            {
				//Dev.Log(this.pinData.ID + " is reachable? " + RandomizerMod.RandoLogger.pm.CanGet(this.pinData.ID));
				if (pool == "Grub" || pool == "Root" || pool == "Essence_Boss")
				{
					if(RandomizerMod.RandoLogger.pm.CanGet(this.pinData.ID))
					{
						//if (RandomizerMod.RandoLogger.pm.Has(this.pinData.ID))
						//	this.disableSelf();
						this.transform.localScale = this.origScale;
						this.sr.color = this.origColor;
						this.isPossible = true;
						//this.origSprite = Resources.Sprite("Map.randoPin");
					}
					if (pool == "Grub")
					{
						string roomName = PinData_S.All[this.pinData.ID].SceneName;
						if (PlayerData.instance.scenesGrubRescued.Contains(roomName))
							this.disableSelf();
					}
					if (pool == "Root")
					{
						string roomName = PinData_S.All[this.pinData.ID].SceneName;
						if (PlayerData.instance.scenesEncounteredDreamPlantC.Contains(roomName))
							this.disableSelf();
					}

					if (pool == "Essence_Boss")
					{
						bool removePin = false;
						switch (this.pinData.ID)
						{
							case "Boss_Essence-Elder_Hu":
								removePin = HeroController.instance.playerData.elderHuDefeated == 2;
								break;
							case "Boss_Essence-Xero":
								removePin = HeroController.instance.playerData.xeroDefeated == 2;
								break;
							case "Boss_Essence-Gorb":
								removePin = HeroController.instance.playerData.aladarSlugDefeated == 2;
								break;
							case "Boss_Essence-Marmu":
								removePin = HeroController.instance.playerData.mumCaterpillarDefeated == 2;
								break;
							case "Boss_Essence-No_Eyes":
								removePin = HeroController.instance.playerData.noEyesDefeated == 2;
								break;
							case "Boss_Essence-Galien":
								removePin = HeroController.instance.playerData.galienDefeated == 2;
								break;
							case "Boss_Essence-Markoth":
								removePin = HeroController.instance.playerData.markothDefeated == 2;
								break;
							case "Boss_Essence-Failed_Champion":
								removePin = HeroController.instance.playerData.falseKnightOrbsCollected;
								break;
							case "Boss_Essence-Soul_Tyrant":
								removePin = HeroController.instance.playerData.mageLordOrbsCollected;
								break;
							case "Boss_Essence-Lost_Kin":
								removePin = HeroController.instance.playerData.infectedKnightOrbsCollected;
								break;
							case "Boss_Essence-White_Defender":
								removePin = HeroController.instance.playerData.whiteDefenderOrbsCollected;
								break;
							case "Boss_Essence-Grey_Prince_Zote":
								removePin = HeroController.instance.playerData.greyPrinceOrbsCollected;
								break;
						}
						if (removePin)
							this.disableSelf();
					}
					//Dev.Log(this.pinData.ID + " is reachable? " + RandomizerMod.RandoLogger.pm.CanGet(this.pinData.ID));
				}
				else
				{
					this.disableSelf();
				}
            }

			//if(this.pinData.isShop)
			//{
			//	string shopName = "";
			//	switch (this.pinData.ID)
			//	{
			//		case "Stalwart_Shell":
			//			break;

			//		case "Wayward_Compass":
			//			break;

			//		case "Quick_Focus":
			//			break;

			//		case "Fragile_Strength":
			//			break;

			//		case "Sprintmaster":
			//			break;
			//	}
				
			//	//RandomizerMod.RandoLogger.pm.
			//}
        }
        //switch ( this.pinData.CheckType ) {
        //	case PinData.Types.PlayerBool:
        //		if ( this.checkPlayerData( this.pinData.CheckBool ) ) {
        //			this.disableSelf();
        //		}
        //		break;
        //	case PinData.Types.SceneData:
        //		if ( this.checkSceneData( this.pinData.SceneName, this.pinData.ObjectName ) ) {
        //			this.disableSelf();
        //		}
        //		break;
        //	default:
        //		DebugLog.Warn( "Pin CheckType not defined?" );
        //		break;
        //}
    }

	private void disableSelf() {
		this.gameObject.SetActive( false );
	}

	private void setLogicState( bool val ) {
        //Dev.Log("Set Logic State: " + val);
		if ( val == true && this.isPossible == false ) {
			this.transform.localScale = this.origScale;
			this.sr.color = this.origColor;
			this.isPossible = true;
		} else if ( val == false && this.isPossible == true ) {
            //Dev.Log("Shrink!");
			this.transform.localScale = this.origScale * 0.5f;
			this.sr.color = Color.gray;
			this.isPossible = false;
		}
	}

	private void setPrereqState( bool val ) {
		if ( val == true && this.isPrereqMet == false ) {
			this.sr.sprite = this.origSprite;
			this.isPrereqMet = true;
		} else if ( val == false && this.isPrereqMet == true ) {
			this.sr.sprite = RandoMapMod.Resources.Sprite( "Map.prereqPin" );
			this.isPrereqMet = false;
		}
	}

	private bool checkPlayerData( string checkBool ) {
		bool ret = PlayerData.instance.GetBool( checkBool );

		return ret;
	}

	private bool checkSceneData( string pSceneName, string pObjectName ) {
		bool ret = false;
		List<PersistentBoolData> pbis = SceneData.instance.persistentBoolItems;

		foreach ( PersistentBoolData pbd in pbis ) {
			if ( pbd.sceneName == pSceneName && pbd.id == pObjectName ) {
				ret = pbd.activated;
				break;
			}
		}

		return ret;
	}
}