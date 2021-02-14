using UnityEngine;

class PinGroup : Component {
	//Component so that Update still gets called when gameObject is not active.
	private bool _forcedHide = false;
	private MapTextOverlay _mapTextOverlay = null;
	private MapTextOverlay _MapTextOverlay {
		get {
			if (_mapTextOverlay == null) _mapTextOverlay = gameObject.GetComponent<MapTextOverlay>();
			return _mapTextOverlay;
		}
	}

	protected void Update() {
		if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)) {
			if (Input.GetKeyDown(KeyCode.P)) {
				this.Hide(true);
			}
			if (Input.GetKeyDown(KeyCode.G)) {
				RandoMapMod.RandoMapMod.GiveCollectorsMap(toggle: true);
			}
			if (Input.GetKeyDown(KeyCode.M)) {
				RandoMapMod.RandoMapMod.GiveAllMaps("Hotkey");
			}
		}
	}

	public void Hide(bool force = false) {
		if (force) _forcedHide = true;

		this.gameObject.SetActive(false);
		this._MapTextOverlay.Hide();
	}

	internal void Show(bool force = false) {
		if (force) _forcedHide = false;

		if (!_forcedHide) {
			this.gameObject.SetActive(true);
			this._MapTextOverlay.Show();
		}
	}
}
