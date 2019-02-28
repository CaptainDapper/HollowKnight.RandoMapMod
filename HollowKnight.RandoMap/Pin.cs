using RandoMapMod;
using System.Collections.Generic;
using UnityEngine;

class Pin : MonoBehaviour {
	private PinData pinData;
	private bool isPossible = true;

	private SpriteRenderer sr = null;
	private Vector3 origScale;
	private Color origColor;

	public PinData PinData {
		set {
			this.pinData = value;
		}
	}



	void Awake() {
		this.sr = this.gameObject.GetComponent<SpriteRenderer>();
		this.origScale = this.transform.localScale;
		this.origColor = this.sr.color;
	}

	void OnEnable() {
		//Set Pin's display state according to location's logic.
		setLogicState( this.pinData.Possible );

		//Disable Pin if we've already obtained / checked this location.
		switch ( this.pinData.CheckType ) {
			case PinData.Types.PlayerBool:
				if ( PlayerData.instance.GetBool( this.pinData.CheckBool ) ) {
					disableSelf();
				}
				break;
			case PinData.Types.SceneData:
				if ( this.checkSceneData( this.pinData.SceneName, this.pinData.ObjectName ) ) {
					disableSelf();
				}
				break;
			default:
				DebugLog.Warn( "Pin CheckType not defined?" );
				break;
		}
	}



	private void disableSelf() {
		this.gameObject.SetActive( false );
	}

	private void setLogicState( bool val ) {
		if ( val == true && this.isPossible == false ) {
			this.transform.localScale = this.origScale;
			this.sr.color = this.origColor;
			this.isPossible = true;
		} else if ( val == false && this.isPossible == true ) {
			this.transform.localScale = this.origScale * 0.5f;
			this.sr.color = Color.gray;
			this.isPossible = false;
		}
	}

	private bool checkSceneData( string pSceneName, string pObjectName ) {
		List<PersistentBoolData> pbis = SceneData.instance.persistentBoolItems;

		foreach ( PersistentBoolData pbd in pbis ) {
			if ( pbd.sceneName == pSceneName && pbd.id == pObjectName ) {
				return pbd.activated;
			}
		}

		return false;
	}
}