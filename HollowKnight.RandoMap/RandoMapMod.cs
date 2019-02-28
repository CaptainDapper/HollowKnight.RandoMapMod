using Modding;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace RandoMapMod {
	// TO DO LIST
	//  Update all of the pin offsets.
	//  Fix blurry pins

	public class RandoMapMod : Mod {
		private GameObject custPinGroup = null;
		private GameMap theMap;

		public static RandoMapMod Instance {
			get; private set;
		}

		public static bool IsRando {
			get {
				//DebugLog.Write( "Rando? " + RandomizerMod.RandomizerMod.Instance.Settings.Randomizer );
				return true;
				//return RandomizerMod.RandomizerMod.Instance.Settings.Randomizer;
			}
		}

		public override string GetVersion() {
			string ver = "0.0.1";
			int minAPI = 45;

			bool apiTooLow = Convert.ToInt32( ModHooks.Instance.ModVersion.Split( '-' )[1] ) < minAPI;
			if ( apiTooLow ) {
				return ver + " (Update API)";
			}

			return ver;
		}

		public override void Initialize() {
			DebugLog.Write( "Initialize" );

			if ( Instance != null ) {
				LogWarn( "Initialized twice... Stop that." );
				return;
			}
			Instance = this;

			Resources.Initialize();

			//On.GameMap.Start += this.PrintDebug;
			On.GameMap.Start += this.GameMap_Start;
			//On.GameMap.SetupMapMarkers += this.PrintDebug;
			On.GameMap.SetupMapMarkers += this.GameMap_SetupMapMarkers;
			On.GameMap.DisableMarkers += this.GameMap_DisableMarkers;

			ModHooks.Instance.SavegameLoadHook += this.SavegameLoadHook;
			ModHooks.Instance.NewGameHook += this.NewGameHook;

			DebugLog.Log("RandoMapMod Initialize complete!");
		}

		private void SceneChanged( string targetScene ) {
			DebugLog.Write( "SceneChanged " + targetScene );
			if ( targetScene == "Tutorial_01" ) {
				if ( IsRando ) {
					ObjectNames.Load();
				}
				ModHooks.Instance.SceneChanged -= this.SceneChanged;
			}
		}

		private void NewGameHook() {
			DebugLog.Write( "NewGameHook" );

			ModHooks.Instance.SceneChanged += this.SceneChanged;
		}

		private void SavegameLoadHook( int slot ) {
			DebugLog.Write( "SavegameLoadHook " + slot );

			ObjectNames.Load();
		}

		private void GameMap_Start( On.GameMap.orig_Start orig, GameMap self ) {
			DebugLog.Write( "GameMap_Start" );

			if ( !IsRando ) {
				orig( self );
				return;
			}

			if ( this.custPinGroup == null ) {
				this.theMap = self;

				DebugLog.Write( "Creating Custom Pin Group." );

				this.custPinGroup = new GameObject( "Custom Pins" );
				CustomPinGroup cpg = this.custPinGroup.AddComponent<CustomPinGroup>();
				this.custPinGroup.transform.parent = self.transform;
				this.custPinGroup.transform.position = new Vector3( 0f, 0f, 0f );
				this.custPinGroup.SetActive( false );

				foreach ( PinData pin in PinData_S.All.Values ) {
					this.pAddPinToRoom( pin );
				}
			}
			orig( self );
		}

		private void GameMap_SetupMapMarkers( On.GameMap.orig_SetupMapMarkers orig, GameMap self ) {
			DebugLog.Write( "GameMap_SetupMapMarkers" );

			if ( !IsRando ) {
				orig( self );
				return;
			}
			orig( self );

			this.custPinGroup.SetActive( true );
		}

		private void GameMap_DisableMarkers( On.GameMap.orig_DisableMarkers orig, GameMap self ) {
			DebugLog.Write( "GameMap_DisableMarkers" );

			if ( !IsRando ) {
				orig( self );
				return;
			}
			this.custPinGroup.SetActive( false );

			orig( self );
		}

		private void pAddPinToRoom( PinData pPin ) {
			string roomName = pPin.PinScene;

			GameObject newPin = new GameObject( "pin_rando" );
			newPin.transform.parent = this.custPinGroup.transform;
			newPin.layer = 30;

			SpriteRenderer sr = newPin.AddComponent<SpriteRenderer>();
			sr.sprite = Resources.Sprite( "Map.randoPin" );
			sr.sortingLayerName = "HUD";
			sr.size = new Vector2( 1f, 1f );

			newPin.transform.localScale *= 1.2f;

			Pin pin = newPin.AddComponent<Pin>();
			pin.PinData = pPin;

			Vector3 vec = this.pGetRoomPos( roomName ) + pPin.Offset;

			newPin.transform.localPosition = new Vector3( vec.x, vec.y, (vec.z - 0.5f) );
		}

		private Vector3 pGetRoomPos( string prmRoomName ) {
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

		private void pMakeRandoPin( GameObject prmParent, Vector3 prmOffset ) {
			GameObject newPin = new GameObject( "pin_rando" );
			SpriteRenderer sr = newPin.AddComponent<SpriteRenderer>();
			sr.sprite = Resources.Sprite( "Map.randoPin" );

			newPin.transform.parent = prmParent.transform;
			newPin.transform.localPosition = prmOffset;

			newPin.GetComponent<SpriteRenderer>().enabled = true;
			newPin.SetActive( true );
		}













































		private void pPrintDebug( GameMap self ) {
			for ( int index1 = 0; index1 < self.transform.childCount; ++index1 ) {
				GameObject gameObject1 = self.transform.GetChild( index1 ).gameObject;
				this.pDebugObj( gameObject1, "" );
				for ( int index2 = 0; index2 < gameObject1.transform.childCount; ++index2 ) {
					GameObject gameObject2 = gameObject1.transform.GetChild( index2 ).gameObject;
					this.pDebugObj( gameObject2, "  " );
					for ( int index3 = 0; index3 < gameObject2.transform.childCount; ++index3 ) {
						GameObject gameObject3 = gameObject2.transform.GetChild( index3 ).gameObject;
						this.pDebugObj( gameObject3, "    " );
					}
				}
			}
		}

		private void pMakeOutline( GameObject pO ) {
			for ( int i = 0; i < 8; i++ ) {
				GameObject newT = GameObject.Instantiate( pO );
				newT.transform.parent = pO.transform;
				TextMesh tm = newT.GetComponent<TextMesh>();
				tm.color = Color.black;

				float pixelSize = 0.02f;
				newT.transform.localPosition = newT.transform.localPosition + ( this.pGetOffset( i ) * pixelSize );
			}
		}

		private Vector3 pGetOffset( int i ) {
			switch ( i % 8 ) {
				case 0:
					return new Vector3( 0, 1, 0.01f );
				case 1:
					return new Vector3( 1, 1, 0.01f );
				case 2:
					return new Vector3( 1, 0, 0.01f );
				case 3:
					return new Vector3( 1, -1, 0.01f );
				case 4:
					return new Vector3( 0, -1, 0.01f );
				case 5:
					return new Vector3( -1, -1, 0.01f );
				case 6:
					return new Vector3( -1, 0, 0.01f );
				case 7:
					return new Vector3( -1, 1, 0.01f );
				default:
					return Vector3.zero;
			}
		}

		private void pMakeRoomLabels() {
			for ( int index1 = 0; index1 < this.theMap.transform.childCount; ++index1 ) {
				GameObject gameObject1 = this.theMap.transform.GetChild( index1 ).gameObject;
				if ( gameObject1.name != "Dream_Gate_Pin" && gameObject1.name != "Grub Pins" ) {
					for ( int index2 = 0; index2 < gameObject1.transform.childCount; ++index2 ) {
						GameObject gameObject2 = gameObject1.transform.GetChild( index2 ).gameObject;
						if ( gameObject2.name != "Grub Pins" ) {
							GameObject canvasObj = new GameObject( "sceneLabel" );
							Canvas canvas = canvasObj.AddComponent<Canvas>();

							GameObject textObj = new GameObject( "text" );
							textObj.layer = 30;
							MeshRenderer mesh = textObj.AddComponent<MeshRenderer>();
							mesh.sortingOrder = 0;
							mesh.sortingLayerName = "HUD";
							TextMesh text = textObj.AddComponent<TextMesh>();
							text.text = gameObject2.name;
							text.color = Color.green;

							this.pMakeOutline( textObj );

							textObj.transform.parent = canvasObj.transform;
							textObj.transform.localPosition = new Vector3( 0f, 0f, 0f );
							textObj.transform.localEulerAngles = new Vector3( 0f, 0f, 20f );
							canvasObj.transform.parent = gameObject2.transform;
							canvasObj.transform.localScale = new Vector3( 0.18f, 0.18f, 0.18f );
							canvasObj.transform.localPosition = new Vector3( 0f, 0f, -0.6f );
						}
					}
				}
			}
		}

		private void pDebugSR( GameObject prmObj ) {
			List<string> lines = new List<string>();

			lines.Add( prmObj.name.ToUpper() );
			lines.Add( "activeInHierarchy: " + prmObj.activeInHierarchy );
			lines.Add( "activeSelf: " + prmObj.activeSelf );
			lines.Add( "layer: " + prmObj.layer );
			lines.Add( "scene: " + prmObj.scene );
			lines.Add( "tag: " + prmObj.tag );

			lines.Add( "TRANSFORM" );
			lines.Add( "position: " + prmObj.transform.position );
			lines.Add( "localPosition: " + prmObj.transform.localPosition );
			lines.Add( "childCount: " + prmObj.transform.childCount );
			lines.Add( "localScale: " + prmObj.transform.localScale );
			lines.Add( "lossyScale: " + prmObj.transform.lossyScale );
			lines.Add( "tag: " + prmObj.transform.tag );

			SpriteRenderer sr = prmObj.GetComponent<SpriteRenderer>();

			if ( sr != null ) {
				lines.Add( "SPRITE RENDERER" );
				lines.Add( "tileMode: " + sr.tileMode );
				lines.Add( "tag: " + sr.tag );
				lines.Add( "sortingOrder: " + sr.sortingOrder );
				lines.Add( "sortingLayerName: " + sr.sortingLayerName );
				lines.Add( "sortingLayerID: " + sr.sortingLayerID );
				lines.Add( "size: " + sr.size );
				lines.Add( "sharedMaterial: " + sr.sharedMaterial );
				lines.Add( "receiveShadows: " + sr.receiveShadows );
				lines.Add( "material: " + sr.material );
				lines.Add( "isVisible: " + sr.isVisible );
				lines.Add( "enabled: " + sr.enabled );
				lines.Add( "drawMode: " + sr.drawMode );
				lines.Add( "color: " + sr.color );
				lines.Add( "bounds: " + sr.bounds );
			}

			lines.Add( "" );

			DebugLog.Write( lines );
		}

		private void pDebugObj( GameObject go, string indent ) {
			string theString = "";
			bool thing = false;
			theString = go.name + ( ( go.activeSelf == false ) ? "(d)" : "" ) + ": ";

			foreach ( var comp in go.GetComponents<Component>() ) {
				if ( thing ) {
					theString += ",";
				} else {
					thing = true;
				}
				theString += comp.GetType().Name;
				if ( comp.GetType() == typeof( Transform ) ) {
					Transform trans = (Transform) comp;
					theString += "(" + trans.GetPositionX() + ", " + trans.GetPositionY() + ")";
				}
			}

			DebugLog.Write( indent + theString );
		}
	}
}
