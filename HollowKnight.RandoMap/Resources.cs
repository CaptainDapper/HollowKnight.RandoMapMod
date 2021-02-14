using ModCommon;
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

		public static Dictionary<string, PinData> PinData() {
			return pPinData;
		}

		public static Sprite Sprite( string pSpriteName ) {
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
					//Load up all the one sprites!
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
					//Load the pin-specific data; we'll follow up with the direct rando info later, so we don't duplicate defs...
					try {
						using ( Stream stream = theDLL.GetManifestResourceStream( resource ) ) {
							loadPinData( stream );
						}
					} catch ( Exception e ) {
						DebugLog.Error( "pindata.xml Load Failed!" );
						DebugLog.Error( e.ToString() );
					}
				}
			}

            //Dev.Log("Initialize macros");
			Assembly randoDLL = typeof( RandomizerMod.RandomizerMod ).Assembly;
			foreach ( string resource in randoDLL.GetManifestResourceNames() ) {
                //Dev.Log("Macro 1");
				if ( resource.EndsWith( "items.xml" )) {

                    try {
                        //Dev.Log("Item 1");
                        using ( Stream stream = randoDLL.GetManifestResourceStream( resource ) ) {
							XmlDocument xml = new XmlDocument();
							xml.Load( stream );
                            //Dev.Log("Item 2");
                            loadItemData( xml.SelectNodes( "randomizer/item" ) );
                            //Dev.Log("Item 3");
                            //loadMacroData( xml.SelectNodes( "randomizer/macro" ), xml.SelectNodes( "randomizer/additiveItemSet" ) );
                        }
					} catch ( Exception e ) {
						DebugLog.Error( "items.xml Load Failed!" );
						DebugLog.Error( e.ToString() );
					}
				}
				if (resource.EndsWith("rocks.xml"))
				{

					try
					{
						//Dev.Log("Item 1");
						using (Stream stream = randoDLL.GetManifestResourceStream(resource))
						{
							XmlDocument xml = new XmlDocument();
							xml.Load(stream);
							//Dev.Log("Item 2");
							loadItemData(xml.SelectNodes("randomizer/item"));
							//Dev.Log("Item 3");
							//loadMacroData( xml.SelectNodes( "randomizer/macro" ), xml.SelectNodes( "randomizer/additiveItemSet" ) );
						}
					}
					catch (Exception e)
					{
						DebugLog.Error("rocks.xml Load Failed!");
						DebugLog.Error(e.ToString());
					}
				}
				if (resource.EndsWith("soul_lore.xml"))
				{

					try
					{
						//Dev.Log("Item 1");
						using (Stream stream = randoDLL.GetManifestResourceStream(resource))
						{
							XmlDocument xml = new XmlDocument();
							xml.Load(stream);
							//Dev.Log("Item 2");
							loadItemData(xml.SelectNodes("randomizer/item"));
							//Dev.Log("Item 3");
							//loadMacroData( xml.SelectNodes( "randomizer/macro" ), xml.SelectNodes( "randomizer/additiveItemSet" ) );
						}
					}
					catch (Exception e)
					{
						DebugLog.Error("soul_lore.xml Load Failed!");
						DebugLog.Error(e.ToString());
					}
				}
				if (resource.EndsWith("macros.xml"))
                {
                    try
                    {
                        //Dev.Log("Macro 1");
                        using (Stream stream = randoDLL.GetManifestResourceStream(resource))
                        {
                            XmlDocument xml = new XmlDocument();
                            xml.Load(stream);
                            //Dev.Log("Macro 2");
                            //loadItemData(xml.SelectNodes("randomizer/item"));
                            loadMacroData(xml.SelectNodes("randomizer/macro"));
                            //Dev.Log("Macro 3");
                        }
                    }
                    catch (Exception e)
                    {
                        DebugLog.Error("macros.xml Load Failed!");
                        DebugLog.Error(e.ToString());
                    }
                }
                //if (resource.EndsWith("waypoints.xml"))
                //{
                //    try
                //    {
                //        //Dev.Log("Macro 1");
                //        using (Stream stream = randoDLL.GetManifestResourceStream(resource))
                //        {
                //            XmlDocument xml = new XmlDocument();
                //            xml.Load(stream);
                //            //Dev.Log("Macro 2");
                //            //loadItemData(xml.SelectNodes("randomizer/item"));
                //            loadMacroWaypointData(xml.SelectNodes("randomizer/item"));
                //            //Dev.Log("Macro 3");
                //        }
                //    }
                //    catch (Exception e)
                //    {
                //        DebugLog.Error("waypoints.xml Load Failed!");
                //        DebugLog.Error(e.ToString());
                //    }
                //}
                if (resource.EndsWith("additive.xml"))
                {
                    try
                    {
                        //Dev.Log("Macro 1");
                        using (Stream stream = randoDLL.GetManifestResourceStream(resource))
                        {
                            XmlDocument xml = new XmlDocument();
                            xml.Load(stream);
                            //Dev.Log("Macro 2");
                            //loadItemData(xml.SelectNodes("randomizer/item"));
                            loadAdditiveMacroData(xml.SelectNodes("randomizer/additiveItemSet"));
                            //Dev.Log("Macro 3");
                        }
                    }
                    catch (Exception e)
                    {
                        DebugLog.Error("macros.xml Load Failed!");
                        DebugLog.Error(e.ToString());
                    }
                }
            }
		}


        private static void loadMacroData(XmlNodeList nodes)
        {
            foreach (XmlNode node in nodes)
            {
                string name = node.Attributes["name"].Value;
                //Dev.Log("Load Macro: " + name + " as " + node.InnerText);
                LogicManager.AddMacro(name, node.InnerText);
            }
        }
        private static void loadAdditiveMacroData(XmlNodeList additiveItems)
        {
            foreach (XmlNode node in additiveItems)
            {
                string name = node.Attributes["name"].Value;
                //Dev.Log("Load Macro: " + name);

                string[] additiveSet = new string[node.ChildNodes.Count];
                for (int i = 0; i < additiveSet.Length; i++)
                {
                    additiveSet[i] = node.ChildNodes[i].InnerText;
                    //Dev.Log("Load " + name + ": " + additiveSet[i]);
                }

                LogicManager.AddMacro(name, string.Join(" | ", additiveSet));
            }
        }

        private static void loadMacroData( XmlNodeList nodes, XmlNodeList additiveItems ) {
            //Dev.Log("Load Macros");
			foreach ( XmlNode node in additiveItems ) {
				string name = node.Attributes["name"].Value;
                //Dev.Log("Load Macro: " + name);

                string[] additiveSet = new string[node.ChildNodes.Count];
				for ( int i = 0; i < additiveSet.Length; i++ ) {
					additiveSet[i] = node.ChildNodes[i].InnerText;
                    //Dev.Log("Load " + name + ": " + additiveSet[i]);
                }

				LogicManager.AddMacro( name, string.Join( " | ", additiveSet ) );
			}

			foreach ( XmlNode node in nodes ) {
                string name = node.Attributes["name"].Value;
                //Dev.Log("Load Macro: " + name + " as " + node.InnerText);
                LogicManager.AddMacro( name, node.InnerText );
			}
		}

        private static void loadMacroWaypointData(XmlNodeList nodes)
        {
            foreach (XmlNode node in nodes)
            {
                string name = node.Attributes["name"].Value;
                //Dev.Log("Load Macro: " + name + " as " + node["itemLogic"].InnerText);
                LogicManager.AddMacro(name, node["itemLogic"].InnerText);
            }
        }

        private static void loadItemData( XmlNodeList nodes ) {
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

					if ( chld.Name == "itemLogic") {
						pinD.LogicRaw = chld.InnerText;
						continue;
					}

					if ( chld.Name == "boolName" ) {
						pinD.ObtainedBool = chld.InnerText;
						continue;
					}

					if ( chld.Name == "inChest" ) {
						pinD.InChest = true;
						continue;
					}

					if ( chld.Name == "newShiny" ) {
						pinD.NewShiny = true;
						continue;
					}

					if ( chld.Name == "x" ) {
						pinD.NewX = (int)XmlConvert.ToDouble( chld.InnerText );
						continue;
					}

					if ( chld.Name == "y" ) {
						pinD.NewY = (int)XmlConvert.ToDouble( chld.InnerText );
						continue;
					}

                    if ( chld.Name == "pool")
                    {
                        pinD.Pool = chld.InnerText;
                        continue;
                    }

                }
			}
		}

		private static void loadPinData( Stream stream ) {
			pPinData = new Dictionary<string, PinData>();

			XmlDocument xml = new XmlDocument();
			xml.Load( stream );
			foreach ( XmlNode node in xml.SelectNodes( "randomap/pin" ) ) {
				PinData newPin = new PinData();
				newPin.ID = node.Attributes["name"].Value;
                //Dev.Log("Load Pin Data: " + newPin.ID);
				string line = "";

				foreach ( XmlNode chld in node.ChildNodes ) {
					switch ( chld.Name ) {
						case "pinScene":
							line += ", pinScene = " + chld.InnerText;
							newPin.PinScene = chld.InnerText;
							break;
						case "checkType":
							line += ", checkType = " + chld.InnerText;
							newPin.CheckType = selectCheckType( chld.InnerText );
							break;
						case "checkBool":
							line += ", checkBool = " + chld.InnerText;
							newPin.CheckBool = chld.InnerText;
							break;
						case "prereq":
							line += ", prereq = " + chld.InnerText;
							newPin.PrereqRaw = chld.InnerText;
							break;
						case "offsetX":
							line += ", offsetX = " + chld.InnerText;
							newPin.OffsetX = XmlConvert.ToSingle( chld.InnerText );
							break;
						case "offsetY":
							line += ", offsetY = " + chld.InnerText;
							newPin.OffsetY = XmlConvert.ToSingle( chld.InnerText );
							break;
						case "offsetZ":
							line += ", offsetZ = " + chld.InnerText;
							newPin.OffsetZ = XmlConvert.ToSingle( chld.InnerText );
							break;
                        case "hasPrereq":
                            line += ", hasPrereq = " + chld.InnerText;
                            newPin.hasPrereq = XmlConvert.ToBoolean(chld.InnerText);
                            break;
                        case "isShop":
                            line += ", isShop = " + chld.InnerText;
                            newPin.isShop = XmlConvert.ToBoolean(chld.InnerText);
                            break;
						default:
							DebugLog.Error( "Pin '" + newPin.ID + "' in XML had node '" + chld.Name + "' not parsable!" );
							break;
					}
				}
                
				pPinData.Add( newPin.ID, newPin );
                //Dev.Log(newPin.ID + " Pin added: " + pPinData.ContainsKey(newPin.ID));
            }
		}

		private static PinData.Types selectCheckType( string text ) {
			//There used to be more of these things... This is probably useless now.
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
