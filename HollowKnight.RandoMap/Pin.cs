using System;
using System.Collections.Generic;
using RandoMapMod;
using UnityEngine;

class Pin : MonoBehaviour {
	public PinData PinData {
		set {
			this.pinData = value;
			this.pinData.gameObject = this.gameObject;
		}
	}
	private PinData pinData;

	void OnEnable() {
		bool disable = false;

		PlayerData pd = PlayerData.instance;
		SceneData sd = SceneData.instance;

		switch ( pinData.CheckType ) {
			case PinData.Types.PlayerBool:
				if ( pd.GetBool( pinData.PDName ) ) {
					DebugLog.Write( "  PDBool disable" );
					disable = true;
				}
				break;
			case PinData.Types.PlayerGT:
				if ( pd.GetInt( pinData.PDName ) > int.Parse(pinData.PDValue) ) {
					DebugLog.Write( "  PDGT disable" );
					disable = true;
				}
				break;
			case PinData.Types.PlayerSceneVisited:
				if ( pd.scenesVisited.Contains( pinData.SceneName ) ) {
					DebugLog.Write( "  SceneVisited disable" );
					disable = true;
				}
				break;
			case PinData.Types.SceneData:
				if ( pSceneObjectActivated( pinData.SceneName, pinData.ObjectName ) ) {
					DebugLog.Write( "  SceneData disable" );
					disable = true;
				}
				break;
			default:
				DebugLog.Write( "Something seriously wrong with this PinMB's PinData..." );
				break;
		}

		if ( disable ) {
			//DebugLog.Write( "Disabling pin: " + pinData.Item + " " + pinData.PinScene + " " + pinData.CheckType.ToString() );
			this.gameObject.SetActive( false );
		} else {
			DebugLog.Write( "Pin remains enabled: " + pinData.Item + " " + pinData.PinScene + " " + pinData.CheckType.ToString() );
		}
	}

	private bool pSceneObjectActivated( string pSceneName, string pObjectName ) {
		List<PersistentBoolData> pbis = SceneData.instance.persistentBoolItems;

		foreach ( PersistentBoolData pbd in pbis ) {
			if ( pbd.sceneName == pSceneName && pbd.id == pObjectName ) {
				return pbd.activated;
			}
		}

		DebugLog.Write( "PBD NOT FOUND! PANIC! " + pSceneName + " - " + pObjectName );
		return false;
	}

	void OnDisable() {

	}
}