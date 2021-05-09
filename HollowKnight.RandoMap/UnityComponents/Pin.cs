using RandoMapMod;
//using RandoMapMod.BoringInternals;
using System;
using UnityEngine;
using UnityEngine.UI;

[DebugName(nameof(Pin))]
class Pin : MonoBehaviour {
	#region Private Non-Methods
	internal readonly Color InactiveColor = Color.gray;

	private bool? _isPossible = null;
	private bool _updateTrigger = false;

	internal Vector3 OrigScale;
	internal Color OrigColor;
	internal Sprite OrigSprite;
	internal Vector3 OrigPosition;

	private bool _preReqTrueLock = false;
	private GameObject _prereqLayer = null;

	private MapMod.PinStyles _currentPinStyle = MapMod.PinStyle;

	private SpriteRenderer SR => this.gameObject.GetComponent<SpriteRenderer>();
	#endregion

	#region Public Non-Methods
	public PinData PinData { get; private set; } = null;
	public void SetPinData(PinData pd) {
		this.PinData = pd;

		this.OrigScale = this.transform.localScale;
		this.OrigColor = this.SR.color;
		this.OrigSprite = this.SR.sprite;
		this.OrigPosition = this.transform.localPosition;

		this._UpdateState();
	} // As a setter, this totally counts as a non-method >_>
	#endregion

	#region MonoBehaviour "Overrides"
	protected void OnEnable() {
		_updateTrigger = true;
	}
	protected void Update() {
		if (_updateTrigger) {
			this._UpdateState();

			this._UpdatePinType();
		}
	}
	#endregion

	#region Private Methods
	private void _UpdatePinType() {
		if (this.OrigSprite.name.StartsWith("req")) //Grub pin; don't change it!
			return;

		if (this._currentPinStyle != MapMod.PinStyle) {
			switch (MapMod.PinStyle) {
				case MapMod.PinStyles.Afraid:
				case MapMod.PinStyles.AlsoAfraid:
					__SetNewPinStyle(MapMod.PinStyle);
					break;
				case MapMod.PinStyles.Normal:
				default: {
					// Need to change to the normal pins
					if (_prereqLayer != null && _prereqLayer.activeSelf == true) {
						SpriteRenderer sr = _prereqLayer.GetComponent<SpriteRenderer>();
						sr.color = new Color(1, 1, 1, 1);
					}

					this.SR.sprite = this.OrigSprite;
				}
				break;
			}

			this._currentPinStyle = MapMod.PinStyle;
		}






		void __SetNewPinStyle(MapMod.PinStyles pinStyle) {
			// Change to old pins
			bool prereq = false;
			if (_prereqLayer != null && _prereqLayer.activeSelf == true) {
				SpriteRenderer sr = _prereqLayer.GetComponent<SpriteRenderer>();
				sr.color = new Color(0, 0, 0, 0);
				prereq = true;
			}

			string ogName = this.SR.sprite.name;
			ResourceHelper.Sprites oldSprite;
			if (pinStyle == MapMod.PinStyles.Afraid) {
				oldSprite = this.PinData.Pool switch {
					"Rock" => ResourceHelper.Sprites.oldGeoRockInv,
					"Grub" => ResourceHelper.Sprites.oldGrubInv,
					"Cocoon" => ResourceHelper.Sprites.oldLifebloodInv,
					"Soul" => ResourceHelper.Sprites.oldTotemInv,
					_ => ResourceHelper.Sprites.Unknown,
				};
			} else {
				oldSprite = this.PinData.Pool switch {
					"Rock" => ResourceHelper.Sprites.oldGeoRock,
					"Grub" => ResourceHelper.Sprites.oldGrub,
					"Cocoon" => ResourceHelper.Sprites.oldLifeblood,
					"Soul" => ResourceHelper.Sprites.oldTotem,
					_ => ResourceHelper.Sprites.Unknown,
				};
			}

			this.SR.sprite = ResourceHelper.FetchSprite(prereq ? ResourceHelper.Sprites.old_prereq : oldSprite);
			this.SR.sprite.name = ogName + "_OLD";
		}
	}

	private void _UpdateState() {
		try {
			if (this.PinData == null) {
				throw new Exception("Cannot enable pin with null pindata. Ensure game object is disabled before adding as component, then call SetPinData(<pd>) before enabling.");
			}
			//Set Pin state according to prereqs.
			this._UpdatePrereqState();

			//Otherwise, check if it's reachable and/or if it has any prerequisites.
			this._UpdateReachableState();

			//Disable Pin if we've already obtained / checked this location.
			if (GameStatus.ItemIsChecked(this.PinData.ID)) {
				this._DisableSelf();
			} else {

			}
		} catch (Exception e) {
			DebugLog.Error($"Failed to enable pin! ID: {this.PinData.ID}", e);
		}
	}

	private void _DisableSelf() {
		this.gameObject.SetActive(false);
	}

	private void _UpdateReachableState() {
		bool newValue = GameStatus.ItemIsReachable(this.PinData.ID);

		if (newValue == this._isPossible) {
			return;
		}

		if (newValue == true) {
			// We can reach this item now!
			this.transform.localScale = this.OrigScale;
			this.SR.color = this.OrigColor;
			this._isPossible = true;
		} else {
			// We can't reach this item.
			this.transform.localScale = this.OrigScale * 0.5f;
			this.SR.color = this.InactiveColor;
			this._isPossible = false;
		}
	}

	private void _UpdatePrereqState() {
		if (!this.PinData.HasPrereq || _preReqTrueLock) return;

		//No need to set one up unless we need it.
		if (GameStatus.ItemPrereqsAreMet(this.PinData.ID) == true) {
			//We've got all the prereqs; hide the "!"
			if (_prereqLayer != null && _prereqLayer.activeSelf == true) {
				_prereqLayer.SetActive(false);
			}
			_preReqTrueLock = true;
		} else if (_prereqLayer == null || _prereqLayer.activeSelf == false) {
			if (_prereqLayer == null) {
				_SetupPrereqLayer();
			}
			_prereqLayer.SetActive(true);
		}
	}

	private void _SetupPrereqLayer() {
		_prereqLayer = new GameObject();
		_prereqLayer.transform.SetParent(this.transform);
		_prereqLayer.layer = 30;
		_prereqLayer.transform.localScale *= 1.3f;

		SpriteRenderer sr = _prereqLayer.AddComponent<SpriteRenderer>();
		sr.sprite = ResourceHelper.FetchSprite(ResourceHelper.Sprites.Prereq);
		sr.sortingLayerName = "HUD";
		sr.size = new Vector2(1f, 1f);

		_prereqLayer.transform.localPosition = new Vector3(0, 0, -0.001f);
	}
	#endregion
}