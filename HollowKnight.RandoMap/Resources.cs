using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Xml;
using UnityEngine;

namespace RandoMapMod {
	class Resources {
		private static readonly DebugLog _logger = new DebugLog(nameof(Resources));

		public enum SpriteId {
			/// <summary>
			/// The pin to use for a normal check (e.g. a red question mark).
			/// </summary>
			Rando,
			/// <summary>
			/// The pin to use for shop checks (e.g. a dollar symbol)
			/// </summary>
			Shop,
			/// <summary>
			/// The pin to use for a check with an unmet prereq, which usually
			/// means a Grubfather check but you don't have enough grubs, or a Seer
			/// check but you don't have enough essence.
			/// </summary>
			MissingPrereq,
		}

		private readonly Dictionary<SpriteId, Sprite> _pSprites;
		private readonly Dictionary<string, PinData> _pPinData;

		public Dictionary<string, PinData> PinData() {
			return _pPinData;
		}

		public Sprite Sprite(SpriteId pSpriteName) {
			if (_pSprites.TryGetValue(pSpriteName, out Sprite sprite)) {
				return sprite;
			}

			_logger.Error("Failed to load sprite named '" + pSpriteName + "'");
			return null;
		}

		public Resources() {
			Assembly theDLL = typeof(RandoMapMod).Assembly;
			_pSprites = new Dictionary<SpriteId, Sprite>();
			foreach (string resource in theDLL.GetManifestResourceNames()) {
				if (resource.EndsWith(".png")) {
					//Load up all the one sprites!
					Stream img = theDLL.GetManifestResourceStream(resource);
					byte[] buff = new byte[img.Length];
					img.Read(buff, 0, buff.Length);
					img.Dispose();

					Texture2D texture = new Texture2D(1, 1);
					texture.LoadImage(buff, true);
					SpriteId? key = resource switch
					{
						"RandoMapMod.Resources.Map.prereqPin.png" => SpriteId.MissingPrereq,
						"RandoMapMod.Resources.Map.randoPin.png" => SpriteId.Rando,
						"RandoMapMod.Resources.Map.shopPin.png" => SpriteId.Shop,
						_ => null
					};
					if (key == null) {
						_logger.Warn($"Found unrecognized sprite {resource}. Ignoring.");
					} else {
						_pSprites.Add(
							(SpriteId)key,
							UnityEngine.Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f)));
					}

				} else if (resource.EndsWith("pindata.xml")) {
					//Load the pin-specific data; we'll follow up with the direct rando info later, so we don't duplicate defs...
					try {
						using (Stream stream = theDLL.GetManifestResourceStream(resource)) {
							_pPinData = _LoadPinData(stream);
						}
					} catch (Exception e) {
						_logger.Error("pindata.xml Load Failed!");
						_logger.Error(e.ToString());
					}
				}
			}

			Assembly randoDLL = typeof(RandomizerMod.RandomizerMod).Assembly;
			Dictionary<String, Action<XmlDocument>> resourceProcessors = new Dictionary<String, Action<XmlDocument>>
			{
				{
					"items.xml", (xml) => {
						_LoadItemData(xml.SelectNodes("randomizer/item"));
					}
				}
			};
			foreach (string resource in randoDLL.GetManifestResourceNames()) {
				foreach (string resourceEnding in resourceProcessors.Keys) {
					if (resource.EndsWith(resourceEnding)) {
						_logger.Log($"Loading data from {nameof(RandomizerMod)}'s {resource} file.");
						try {
							using (Stream stream = randoDLL.GetManifestResourceStream(resource)) {
								XmlDocument xml = new XmlDocument();
								xml.Load(stream);
								resourceProcessors[resourceEnding].Invoke(xml);
							}
						} catch (Exception e) {
							_logger.Error($"{resourceEnding} Load Failed!");
							_logger.Error(e.ToString());
						}
						break;
					}
				}
			}
		}

		/// <summary>
		/// Loads items from RandomizerMod's item list, and merges it with our own pindata.xml
		/// </summary>
		/// <param name="nodes">The item nodes from Randomizer Mod's items.xml file</param>
		private void _LoadItemData(XmlNodeList nodes) {
			foreach (XmlNode node in nodes) {
				string itemName = node.Attributes["name"].Value;
				if (!_pPinData.ContainsKey(itemName)) {
					//logger.Log($"RandomizerMod has an item.xml entry for {itemName} but there is no matching entry in RandoMapMod's pindata.xml. This is probably intentional to avoid pins that would otherwise mislead the player.");
					continue;
				}

				PinData pinD = _pPinData[itemName];
				foreach (XmlNode chld in node.ChildNodes) {
					if (chld.Name == "sceneName") {
						pinD.SceneName = chld.InnerText;
						continue;
					}

					if (chld.Name == "objectName") {
						pinD.OriginalName = chld.InnerText;
						continue;
					}

					if (chld.Name == "itemLogic") {
						pinD.LogicRaw = chld.InnerText;
						continue;
					}

					if (chld.Name == "boolName") {
						pinD.ObtainedBool = chld.InnerText;
						continue;
					}

					if (chld.Name == "inChest") {
						pinD.InChest = true;
						continue;
					}

					if (chld.Name == "newShiny") {
						pinD.NewShiny = true;
						continue;
					}

					if (chld.Name == "x") {
						pinD.NewX = (int)XmlConvert.ToDouble(chld.InnerText);
						continue;
					}

					if (chld.Name == "y") {
						pinD.NewY = (int)XmlConvert.ToDouble(chld.InnerText);
						continue;
					}

					if (chld.Name == "pool") {
						pinD.Pool = chld.InnerText;
						continue;
					}

				}
			}
		}

		/// <summary>
		/// Parses our `pindata.xml` file.
		/// </summary>
		private static Dictionary<string, PinData> _LoadPinData(Stream stream) {
			Dictionary<string, PinData> retVal = new Dictionary<string, PinData>();

			XmlDocument xml = new XmlDocument();
			xml.Load(stream);
			foreach (XmlNode node in xml.SelectNodes("randomap/pin")) {
				PinData newPin = new PinData {
					ID = node.Attributes["name"].Value
				};
				foreach (XmlNode chld in node.ChildNodes) {
					if (chld.NodeType == XmlNodeType.Comment) {
						continue;
					}
					switch (chld.Name) {
						case "pinScene":
							newPin.PinScene = chld.InnerText;
							break;
						case "checkBool":
							newPin.CheckBool = chld.InnerText;
							break;
						case "offsetX":
							newPin.OffsetX = XmlConvert.ToSingle(chld.InnerText);
							break;
						case "offsetY":
							newPin.OffsetY = XmlConvert.ToSingle(chld.InnerText);
							break;
						case "offsetZ":
							newPin.OffsetZ = XmlConvert.ToSingle(chld.InnerText);
							break;
						case "hasPrereq":
							newPin.HasPrereq = XmlConvert.ToBoolean(chld.InnerText);
							break;
						case "isShop":
							newPin.IsShop = XmlConvert.ToBoolean(chld.InnerText);
							break;
						default:
							_logger.Error($"Pin '{newPin.ID}' in XML had node '{chld.Name}' not parsable!");
							break;
					}
				}
				retVal.Add(newPin.ID, newPin);
			}
			return retVal;
		}
	}
}