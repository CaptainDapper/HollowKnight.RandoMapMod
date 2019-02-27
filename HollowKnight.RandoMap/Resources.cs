using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Xml;
using UnityEngine;

namespace RandoMapMod {
	class Resources {
		private static Dictionary<string, Sprite> pSprites = null;
		private static Dictionary<string, PinData> pPinData = null;

		internal static Dictionary<string, PinData> PinData() {
			return pPinData;
		}

		internal static Sprite Sprite( string pSpriteName ) {
			if ( pSprites != null && pSprites.TryGetValue( pSpriteName, out Sprite sprite ) ) {
				return sprite;
			}

			DebugLog.Error( "Failed to load sprite named '" + pSpriteName + "'" );
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
						DebugLog.Error( "XML Load Failed!" );
						DebugLog.Error( e.ToString() );
					}
				}
			}

			Assembly randoDLL = typeof( RandomizerMod.RandomizerMod ).Assembly;
			foreach ( string resource in randoDLL.GetManifestResourceNames() ) {
				if ( resource.EndsWith( "items.xml" ) ) {
					try {
						using ( Stream stream = theDLL.GetManifestResourceStream( resource ) ) {
							pLoadItemData( stream );
						}
					} catch ( Exception e ) {
						DebugLog.Error( "XML Load Failed!" );
						DebugLog.Error( e.ToString() );
					}
				}
			}
		}

		private static void pLoadPinData( Stream stream ) {
			//DebugLog.Write( "pLoadPinData" );
			pPinData = new Dictionary<string, PinData>();

			XmlDocument xml = new XmlDocument();
			xml.Load( stream );
			foreach ( XmlNode node in xml.SelectNodes( "randomap/pin" ) ) {
				PinData newPin = new PinData();
				newPin.ID = node.Attributes["name"].Value;
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

					if ( chld.Name == "boolName" ) {
						found = true;
						newPin.CheckBool = chld.InnerText;
					}

					if ( found == false ) {
						DebugLog.Error( "Pin '" + newPin.ID + "' in XML had node '" + chld.Name + "' not parsable!" );
					}
				}

				pPinData.Add( newPin.ID, newPin );
			}
		}

		private static void pLoadItemData( Stream stream ) {
			XmlDocument xml = new XmlDocument();
			xml.Load( stream );
			foreach ( XmlNode node in xml.SelectNodes( "randomizer/item" ) ) {
				string itemName = node.Attributes["name"].Value;
				if ( !pPinData.ContainsKey( itemName ) ) {
					DebugLog.Error( "Could not find item '" + itemName + "' in PinData Dict!" );
					continue;
				}

				PinData pinD = pPinData[itemName];
				foreach ( XmlNode chld in node.ChildNodes ) {
					if ( chld.Name == "sceneName" ) {
						pinD.SceneName = chld.InnerText;
						continue;
					}

					if ( chld.Name == "objectName" ) {
						pinD.OriginalName = chld.InnerText;
						continue;
					}

					if ( chld.Name == "logic" ) {
						pinD.LogicRaw = chld.InnerText;
						continue;
					}

					if ( chld.Name == "boolName" ) {
						pinD.LogicBool = chld.InnerText;
						continue;
					}

					if ( chld.Name == "newShiny" ) {
						pinD.NewShiny = true;
						continue;
					}

					if ( chld.Name == "x" ) {
						pinD.NewX = XmlConvert.ToInt32( chld.InnerText );
					}

					if ( chld.Name == "y" ) {
						pinD.NewY = XmlConvert.ToInt32( chld.InnerText );
					}
				}
			}
		}

		private static PinData.Types pSelectCheckType( string text ) {
			switch ( text ) {
				case "sceneData":
					return global::RandoMapMod.PinData.Types.SceneData;
				case "playerData.bool":
					return global::RandoMapMod.PinData.Types.PlayerBool;
				default:
					DebugLog.Error( "Error parsing Pin Check Type. '" + text + "'" );
					return global::RandoMapMod.PinData.Types.PlayerBool;
			}
		}
	}
}
