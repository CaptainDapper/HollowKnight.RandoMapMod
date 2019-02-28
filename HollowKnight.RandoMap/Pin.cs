using RandoMapMod;
using System;
using System.Collections.Generic;
using UnityEngine;

class Pin : MonoBehaviour {
	private PinData pinData;
	private readonly bool isPossible = true;

	private SpriteRenderer sr = null;
	private Vector3 origScale;
	private Color origColor;

	private bool selected = false;
	private Color selectedColor = new Color( 0f, 1f, 0f, 0.8f );
	private readonly float selectedSize = 1.2f;
	private bool blinking = false;
	private Color blinkingColor = new Color( 1f, 1f, 1f, 0.5f );
	private readonly float blinkingSize = 1f;
	private int blinkFrames = 0;

	public PinData PinData {
		set {
			this.pinData = value;
			this.pinData.Pin = this;
		}
	}



	void Awake() {
		this.sr = this.gameObject.GetComponent<SpriteRenderer>();
		this.origScale = this.transform.localScale;
		this.origColor = this.sr.color;
	}

	void OnEnable() {
		return;

		bool disable = false;

		PlayerData pd = PlayerData.instance;
		SceneData sd = SceneData.instance;

		switch ( this.pinData.CheckType ) {
			case PinData.Types.PlayerBool:
				if ( pd.GetBool( this.pinData.CheckBool ) ) {
					//DebugLog.Write( "  PDBool disable" );
					disable = true;
				}
				break;
			case PinData.Types.SceneData:
				if ( this.pSceneObjectActivated( this.pinData.SceneName, this.pinData.ObjectName ) ) {
					//DebugLog.Write( "  SceneData disable" );
					disable = true;
				}
				break;
			default:
				DebugLog.Warn( "Pin CheckType not defined?" );
				break;
		}

		if ( disable ) {
			this.gameObject.SetActive( false );
		} else {
			if ( this.pinData.Possible && !this.isPossible ) {
				this.transform.localScale = this.origScale;
				this.sr.color = this.origColor;
			} else if ( !this.pinData.Possible && this.isPossible ) {
				this.transform.localScale = this.origScale * 0.5f;
				this.sr.color = Color.gray;
			}
			//DebugLog.Write( "Pin remains enabled: " + pinData.ID + " " + pinData.PinScene + " " + pinData.CheckType.ToString() );
		}
	}

	void Update() {
		if ( this.selected ) {
			this.blinkFrames++;
			if ( this.blinkFrames % 300 == 0 ) {
				if ( this.blinking ) {
					this.blinking = false;
					this.sr.color = this.selectedColor;
					this.transform.localScale = this.origScale * this.selectedSize;
					this.blinkFrames += 100;
				} else {
					this.blinking = true;
					this.sr.color = this.blinkingColor;
					this.transform.localScale = this.origScale * this.blinkingSize;
				}
			}

			if ( Input.GetKeyDown( KeyCode.Keypad8 ) ) {
				pMovePin( new Vector3( 0f, 0.05f, 0f ) );
			} else if ( Input.GetKeyDown( KeyCode.Keypad4 ) ) {
				pMovePin( new Vector3( -0.05f, 0f, 0f ) );
			} else if ( Input.GetKeyDown( KeyCode.Keypad6 ) ) {
				pMovePin( new Vector3( 0.05f, 0f, 0f ) );
			} else if ( Input.GetKeyDown( KeyCode.Keypad2 ) ) {
				pMovePin( new Vector3( 0f, -0.05f, 0f ) );
			} else if ( Input.GetKeyDown( KeyCode.KeypadPlus ) ) {
				pMovePin( new Vector3( 0f, 0f, 0.01f ) );
			} else if ( Input.GetKeyDown( KeyCode.KeypadMinus ) ) {
				pMovePin( new Vector3( 0f, 0f, -0.01f ) );
			}
		}
	}

	private void pMovePin( Vector3 vector3 ) {
		this.transform.localPosition += vector3;
		this.pinData.OffsetX += vector3.x;
		this.pinData.OffsetY += vector3.y;
		this.pinData.OffsetZ += vector3.z;
	}

	public void Select() {
		if ( !this.selected ) {
			this.selected = true;
			DebugLog.Write( "Select '" + this.pinData.ID + "'" );
			this.sr.color = this.selectedColor;
			this.transform.localScale = this.origScale * this.selectedSize;
			this.blinking = false;
			this.blinkFrames = 0;
		}
	}

	public void Deselect() {
		if ( this.selected ) {
			this.selected = false;
			//DebugLog.Write( "Deselect '" + this.pinData.ID + "'" );
			this.sr.color = this.origColor;
			this.transform.localScale = this.origScale;
		}
	}



	private bool pSceneObjectActivated( string pSceneName, string pObjectName ) {
		List<PersistentBoolData> pbis = SceneData.instance.persistentBoolItems;

		foreach ( PersistentBoolData pbd in pbis ) {
			if ( pbd.sceneName == pSceneName && pbd.id == pObjectName ) {
				return pbd.activated;
			}
		}

		return false;
	}
}