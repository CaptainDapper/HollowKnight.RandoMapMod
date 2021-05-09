using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RandoMapMod {
	[DebugName(nameof(PinGroup))]
	class PinGroup : MonoBehaviour {
		#region Private Non-Methods
		private readonly List<Pin> _pins = new List<Pin>();
		private MapTextOverlay _mapTextOverlay = null;
		private MapTextOverlay _MapTextOverlay {
			get {
				if (_mapTextOverlay == null) _mapTextOverlay = gameObject.GetComponent<MapTextOverlay>();
				return _mapTextOverlay;
			}
		}
		#endregion

		#region Non-Private Non-Methods
		public GameObject MainGroup { get; private set; } = null;
		public GameObject HelperGroup { get; private set; } = null;
		public bool Hidden { get; private set; } = false;
		#endregion

		#region MonoBehaviour "Overrides"
		protected void Start() {
			this.Hide();
		}
		#endregion

		#region Non-Private Methods
		public void Hide(bool force = false) {
			if (force) Hidden = true;

			this.gameObject.SetActive(false);
			this._MapTextOverlay.Hide();
		}

		public void Show(bool force = false) {
			if (force) Hidden = false;

			if (!Hidden) {
				this.gameObject.SetActive(true);
				this._MapTextOverlay.Show();
			}
		}

		public void AddPinToRoom(PinData pinData, GameMap gameMap) {
			if (_pins.Any(pin => pin.PinData.ID == pinData.ID)) {
				//Already in the list... Probably shouldn't add them.
				DebugLog.Warn($"Duplicate pin found for group: {pinData.ID} - Skipped.");
				return;
			}

			string roomName = pinData.PinScene ?? ResourceHelper.PinData[pinData.ID].SceneName;
			Sprite pinSprite = pinData.IsShop ?
				pinSprite = ResourceHelper.FetchSprite(ResourceHelper.Sprites.Shop) :
				pinSprite = ResourceHelper.FetchSpriteByPool(pinData.Pool);

			GameObject newPin = new GameObject("pin_rando");
			if (pinSprite.name.StartsWith("req")) {
				if (HelperGroup == null) {
					HelperGroup = new GameObject("Resource Helpers");
					HelperGroup.transform.SetParent(this.transform);
					//default to off
					HelperGroup.SetActive(false);
				}

				newPin.transform.SetParent(HelperGroup.transform);
			} else {
				if (MainGroup == null) {
					MainGroup = new GameObject("Main Group");
					MainGroup.transform.SetParent(this.transform);
					//default to off
					MainGroup.SetActive(false);
				}

				newPin.transform.SetParent(MainGroup.transform);
			}
			newPin.layer = 30;
			newPin.transform.localScale *= 1.2f;

			SpriteRenderer sr = newPin.AddComponent<SpriteRenderer>();
			sr.sprite = pinSprite;
			sr.sortingLayerName = "HUD";
			sr.size = new Vector2(1f, 1f);

			Vector3 vec = __GetRoomPos() + pinData.Offset;
			newPin.transform.localPosition = new Vector3(vec.x, vec.y, vec.z - 1f + (vec.y / 100) + (vec.x / 100));

			//Disable to avoid the Pin component's OnEnable before setting the pindata...
			//   Yay Constructorless Components...
			newPin.SetActive(false);

			Pin pinC = newPin.AddComponent<Pin>();
			pinC.SetPinData(pinData);

			newPin.SetActive(true);

			_pins.Add(pinC);

			Vector3 __GetRoomPos() {
				//@@OPTIMIZE: Should be indexed or hard-coded but it runs once per game session. Small gain.
				Vector3 pos = new Vector3(-30f, -30f, -0.5f);
				bool exitLoop = false;

				for (int index1 = 0; index1 < gameMap.transform.childCount; ++index1) {
					GameObject areaObj = gameMap.transform.GetChild(index1).gameObject;
					for (int index2 = 0; index2 < areaObj.transform.childCount; ++index2) {
						GameObject roomObj = areaObj.transform.GetChild(index2).gameObject;
						if (roomObj.name == roomName) {
							pos = roomObj.transform.position;
							exitLoop = true;
							break;
						}
					}
					if (exitLoop) {
						break;
					}
				}

				return pos;
			}
		}
		#endregion
	}
}