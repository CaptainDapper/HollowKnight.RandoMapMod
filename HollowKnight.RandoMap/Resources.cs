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
						DebugLog.Error( "pindata.xml Load Failed!" );
						DebugLog.Error( e.ToString() );
					}
				}
			}

			Assembly randoDLL = typeof( RandomizerMod.RandomizerMod ).Assembly;
			foreach ( string resource in randoDLL.GetManifestResourceNames() ) {
				if ( resource.EndsWith( "items.xml" ) ) {
					try {
						using ( Stream stream = randoDLL.GetManifestResourceStream( resource ) ) {
							XmlDocument xml = new XmlDocument();
							xml.Load( stream );
							pLoadItemData( xml.SelectNodes( "randomizer/item" ) );
							pLoadMacroData( xml.SelectNodes( "randomizer/macro" ), xml.SelectNodes( "randomizer/additiveItemSet" ) );
						}
					} catch ( Exception e ) {
						DebugLog.Error( "items.xml Load Failed!" );
						DebugLog.Error( e.ToString() );
					}
				}
			}
		}

		private static void pLoadMacroData( XmlNodeList nodes, XmlNodeList additiveItems ) {
			DebugLog.Write( "pLoadMacroData" );
			foreach ( XmlNode node in nodes ) {
				string name = node.Attributes["name"].Value;
				LogicManager.AddMacro( name, node.InnerText );
			}

			foreach ( XmlNode node in additiveItems ) {
				string name = node.Attributes["name"].Value;

				string[] additiveSet = new string[node.ChildNodes.Count];
				for ( int i = 0; i < additiveSet.Length; i++ ) {
					additiveSet[i] = node.ChildNodes[i].InnerText;
				}

				LogicManager.AddMacro( name, string.Join( " | ", additiveSet ));
			}
		}

		private static void pLoadItemData( XmlNodeList nodes ) {
			DebugLog.Write( "pLoadItemData" );
			foreach ( XmlNode node in nodes ) {
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

		private static void pLoadPinData( Stream stream ) {
			DebugLog.Write( "pLoadPinData" );
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

					switch ( chld.Name ) {
						case "pinScene":
							newPin.PinScene = chld.InnerText;
							break;
						case "checkType":
							newPin.CheckType = pSelectCheckType( chld.InnerText );
							break;
						case "checkBool":
							newPin.CheckBool = chld.InnerText;
							break;
						case "offsetX":
							newPin.OffsetX = XmlConvert.ToSingle( chld.InnerText );
							break;
						case "offsetY":
							newPin.OffsetY = XmlConvert.ToSingle( chld.InnerText );
							break;
						case "offsetZ":
							newPin.OffsetZ = XmlConvert.ToSingle( chld.InnerText );
							break;
						default:
							DebugLog.Error( "Pin '" + newPin.ID + "' in XML had node '" + chld.Name + "' not parsable!" );
							break;
					}
				}

				pPinData.Add( newPin.ID, newPin );
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
