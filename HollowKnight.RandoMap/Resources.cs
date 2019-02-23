using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Xml;
using UnityEngine;

namespace RandoMapMod {
	class Resources {
		private static Dictionary<string, Sprite> pSprites = null;
		private static List<PinData> pPinData = null;

		internal static List<PinData> PinData() {
			return pPinData;
		}

		internal static Sprite Sprite( string pSpriteName ) {
			if ( pSprites != null && pSprites.TryGetValue( pSpriteName, out Sprite sprite ) ) {
				return sprite;
			}

			DebugLog.Write( "Failed to load sprite named '" + pSpriteName + "'" );
			return null;
		}

		internal static void Initialize() {
			Assembly theDLL = typeof( RandoMapMod ).Assembly;
			pSprites = new Dictionary<string, Sprite>();
			foreach ( string resource in theDLL.GetManifestResourceNames() ) {
				if ( resource.EndsWith( ".png" ) ) {
					Stream img = theDLL.GetManifestResourceStream( resource );
					byte[] buff = new byte[img.Length];
					img.Read( buff, 0, buff.Length );
					img.Dispose();

					Texture2D texture = new Texture2D( 1, 1 );
					texture.LoadImage( buff, true );

					pSprites.Add(
						Path.GetFileNameWithoutExtension( resource.Replace( "RandoMapMod.Resources.", string.Empty ) ),
						UnityEngine.Sprite.Create( texture, new Rect( 0, 0, texture.width, texture.height ), new Vector2( 0.5f, 0.5f ) ) );
				} else if ( resource.EndsWith( "pindata.xml" ) ) {
					try {
						using ( Stream stream = theDLL.GetManifestResourceStream( resource ) ) {
							pLoadPinData( stream );
						}
					} catch ( Exception e ) {
						DebugLog.Write( "XML Load Failed!" );
						DebugLog.Write( e.ToString() );
					}
				}
			}
		}

		private static void pLoadPinData( Stream stream ) {
			//DebugLog.Write( "pLoadPinData" );

			pPinData = new List<PinData>();

			XmlDocument xml = new XmlDocument();
			xml.Load( stream );
			foreach ( XmlNode node in xml.SelectNodes( "randomap/check" ) ) {
				PinData newPin = new PinData();
				newPin.Item = node.Attributes["name"].Value;
				//DebugLog.Write( "  " + node.Name + " " + newPin.Item );

				foreach ( XmlNode chld in node.ChildNodes ) {
					//DebugLog.Write( "    " + chld.Name + " " + chld.InnerText );
					bool found = false;

					if ( chld.Name == "pinScene" ) {
						found = true;
						newPin.PinScene = chld.InnerText;
					}

					if ( chld.Name == "checkType" ) {
						found = true;
						newPin.CheckType = pSelectCheckType( chld.InnerText );
					}

					if ( chld.Name == "sceneName" ) {
						found = true;
						newPin.SceneName = chld.InnerText;
					}

					if ( chld.Name == "objectName" ) {
						found = true;
						newPin.ObjectName = chld.InnerText;
					}

					if ( chld.Name == "rawName" ) {
						found = true;
						newPin.PDName = chld.InnerText;
					}

					if ( chld.Name == "rawValue" ) {
						found = true;
						newPin.PDValue = chld.InnerText;
					}

					if ( chld.Name == "logic" ) {
						found = true;
						newPin.LogicRaw = chld.InnerText;
					}

					if ( found == false ) {
						DebugLog.Write( "Item '" + newPin.Item + "' in XML had node '" + chld.Name + "' not parsable!" );
					}
				}

				pPinData.Add( newPin );
			}
		}

		private static PinData.Types pSelectCheckType( string text ) {
			switch ( text ) {
				case "sceneData.persistentBoolItems":
					return global::RandoMapMod.PinData.Types.SceneData;
				case "playerData.scenesVisited":
					return global::RandoMapMod.PinData.Types.PlayerSceneVisited;
				case "playerData.raw":
					return global::RandoMapMod.PinData.Types.PlayerBool;
				case "playerData.rawGT":
					return global::RandoMapMod.PinData.Types.PlayerGT;
				default:
					DebugLog.Write( "WOW something went wrong with the thingy. '" + text + "'" );
					return global::RandoMapMod.PinData.Types.PlayerBool;
			}
		}
	}
}
