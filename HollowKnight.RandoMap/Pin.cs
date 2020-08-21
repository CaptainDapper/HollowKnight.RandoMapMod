using ModCommon;
using RandoMapMod;
using System;
using System.Collections.Generic;
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
            string pool = PinData_S.All[this.pinData.ID].Pool;
            bool isRandomized = false;
            switch (pool)
            {
                case "Dreamer":
                    isRandomized = RandomizerMod.RandomizerMod.Instance.Settings.RandomizeDreamers;
                    break;

                case "Skill":
                    isRandomized = RandomizerMod.RandomizerMod.Instance.Settings.RandomizeSkills;
                    break;

                case "Charm":
                    isRandomized = RandomizerMod.RandomizerMod.Instance.Settings.RandomizeCharms;
                    break;

                case "Key":
                    isRandomized = RandomizerMod.RandomizerMod.Instance.Settings.RandomizeKeys;
                    break;

                case "Geo":
                    isRandomized = RandomizerMod.RandomizerMod.Instance.Settings.RandomizeGeoChests;
                    break;

                case "Mask":
                    isRandomized = RandomizerMod.RandomizerMod.Instance.Settings.RandomizeMaskShards;
                    break;

                case "Vessel":
                    isRandomized = RandomizerMod.RandomizerMod.Instance.Settings.RandomizeVesselFragments;
                    break;

                case "Ore":
                    isRandomized = RandomizerMod.RandomizerMod.Instance.Settings.RandomizePaleOre;
                    break;

                case "Notch":
                    isRandomized = RandomizerMod.RandomizerMod.Instance.Settings.RandomizeCharmNotches;
                    break;

                case "Egg":
                    isRandomized = RandomizerMod.RandomizerMod.Instance.Settings.RandomizeRancidEggs;
                    break;

                case "Relic":
                    isRandomized = RandomizerMod.RandomizerMod.Instance.Settings.RandomizeRelics;
                    break;

                case "Map":
                    isRandomized = RandomizerMod.RandomizerMod.Instance.Settings.RandomizeMaps;
                    break;

                case "Stag":
                    isRandomized = RandomizerMod.RandomizerMod.Instance.Settings.RandomizeStags;
                    break;

                case "Grub":
                    isRandomized = RandomizerMod.RandomizerMod.Instance.Settings.RandomizeGrubs;
                    break;

                case "Root":
                    isRandomized = RandomizerMod.RandomizerMod.Instance.Settings.RandomizeWhisperingRoots;
                    break;

				case "Rock":
					isRandomized = RandomizerMod.RandomizerMod.Instance.Settings.RandomizeRocks;
					break;

				case "DupeRock":
					isRandomized = RandomizerMod.RandomizerMod.Instance.Settings.RandomizeDupeRocks;
					break;

				default:
                    isRandomized = true;
                    break;
            }
            if (!isRandomized)
            {
                //Dev.Log(this.pinData.ID + " is not randomized");
                this.disableSelf();
            }
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