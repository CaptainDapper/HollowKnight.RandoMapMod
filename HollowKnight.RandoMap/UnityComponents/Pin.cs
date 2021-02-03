using RandoMapMod;
using System;
using UnityEngine;
using static RandoMapMod.Resources;

class Pin : MonoBehaviour {
	#region Static
	private static readonly DebugLog _logger = new DebugLog(nameof(Pin));
	#endregion


	#region Private Non-Methods
	private PinData _pinData;
	private RandoMapMod.Resources _resources;
	private bool _isPossible = true;
	private bool _isPrereqMet = true;

	private SpriteRenderer _sr = null;
	/// <summary>
	/// This is the sprite to show if all the pre-requisites are met (this is usually either randoPin.png or shopPin.png).
	/// </summary>
	private Sprite _origSprite = null;
	private Vector3 _origScale;
	private Color _origColor;
	#endregion


	#region Non-Private Non-Methods
	public PinData PinData {
		set => this._pinData = value;
	}

	public RandoMapMod.Resources Resources {
		set => this._resources = value;
	}
	#endregion


	#region Unity Methods
	protected void Awake() {
		this._sr = this.gameObject.GetComponent<SpriteRenderer>();
		this._origSprite = this._sr.sprite;
		this._origScale = this.transform.localScale;
		this._origColor = this._sr.color;
	}

	protected void OnEnable() {
		try {
			//First off, if this item is not randomized, don't show it.
			string pool = _resources.PinData()[this._pinData.ID].Pool;
			bool isRandomized = pool switch
			{
				"Dreamer" => RandomizerMod.RandomizerMod.Instance.Settings.RandomizeDreamers,
				"Skill" => RandomizerMod.RandomizerMod.Instance.Settings.RandomizeSkills,
				"Charm" => RandomizerMod.RandomizerMod.Instance.Settings.RandomizeCharms,
				"Key" => RandomizerMod.RandomizerMod.Instance.Settings.RandomizeKeys,
				"Geo" => RandomizerMod.RandomizerMod.Instance.Settings.RandomizeGeoChests,
				"Mask" => RandomizerMod.RandomizerMod.Instance.Settings.RandomizeMaskShards,
				"Vessel" => RandomizerMod.RandomizerMod.Instance.Settings.RandomizeVesselFragments,
				"Ore" => RandomizerMod.RandomizerMod.Instance.Settings.RandomizePaleOre,
				"Notch" => RandomizerMod.RandomizerMod.Instance.Settings.RandomizeCharmNotches,
				"Egg" => RandomizerMod.RandomizerMod.Instance.Settings.RandomizeRancidEggs,
				"Relic" => RandomizerMod.RandomizerMod.Instance.Settings.RandomizeRelics,
				"Map" => RandomizerMod.RandomizerMod.Instance.Settings.RandomizeMaps,
				"Stag" => RandomizerMod.RandomizerMod.Instance.Settings.RandomizeStags,
				"Grub" => RandomizerMod.RandomizerMod.Instance.Settings.RandomizeGrubs,
				"Root" => RandomizerMod.RandomizerMod.Instance.Settings.RandomizeWhisperingRoots,
				_ => true,
			};
			if (!isRandomized) {
				this._DisableSelf();
				return;
			}
			//Otherwise, check if it's reachable and/or if it has any prerequisites.
			this._SetReachableStatus(this._pinData.Reachable);
			if (this._isPossible) {
				//Set Pin state according to prereqs.
				this._SetPrereqState(this._pinData.PreReqMet);
			}
			//Disable Pin if we've already obtained / checked this location.
			if (GameStatus.ItemIsChecked(this._pinData.ID.Replace('_', ' '))) {
				this._DisableSelf();
			} else {

			}
		} catch (Exception e) {
			_logger.Error($"Failed to enable pin: {e.Message} {e.StackTrace}");
		}
	}
	#endregion


	#region Private Methods
	private void _DisableSelf() {
		this.gameObject.SetActive(false);
	}

	/// <summary>
	/// Shrinks and/or greys out a pin if it is not possible to get the pin at the current time.
	/// </summary>
	private void _SetReachableStatus(bool newValue) {
		//Dev.Log("Set Logic State: " + val);
		if (newValue == true && this._isPossible == false) {
			this.transform.localScale = this._origScale;
			this._sr.color = this._origColor;
			this._isPossible = true;
		} else if (newValue == false && this._isPossible == true) {
			this.transform.localScale = this._origScale * 0.5f;
			this._sr.color = Color.gray;
			this._isPossible = false;
		}
	}

	/// <summary>
	/// Sets whether the prereq is met for the current pin. Has the side effect
	/// of changing SpriteRenderer's pin to the "prereq pin" if the prereqs are not met.
	/// </summary>
	private void _SetPrereqState(bool newValue) {
		if (newValue == true && this._isPrereqMet == false) {
			this._sr.sprite = this._origSprite;
			this._isPrereqMet = true;
		} else if (newValue == false && this._isPrereqMet == true) {
			if (_resources == null) {
				_logger.Error("Tried to invoke setPrereqState when resources was null");
			}
			this._sr.sprite = _resources.Sprite(SpriteId.MissingPrereq);
			this._isPrereqMet = false;
		}
	}
	#endregion
}