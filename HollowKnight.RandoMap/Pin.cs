using ModCommon;
using RandoMapMod;
using System;
using System.Collections.Generic;
using UnityEngine;
using static RandoMapMod.Resources;

class Pin : MonoBehaviour
{
	private static readonly DebugLog logger = new DebugLog(nameof(Pin));
	private PinData pinData;
	private RandoMapMod.Resources resources;
	private bool isPossible = true;
	private bool isPrereqMet = true;

	private SpriteRenderer sr = null;
	/// <summary>
	/// This is the sprite to show if all the pre-requisites are met (this is usually either randoPin.png or shopPin.png).
	/// </summary>
	private Sprite origSprite = null;
	private Vector3 origScale;
	private Color origColor;

	public PinData PinData
	{
		set
		{
			this.pinData = value;
		}
	}

	public RandoMapMod.Resources Resources
	{
		set
		{
			this.resources = value;
		}
	}

	/// <summary>
	/// This method is invoked by Unity "when the script instance is being loaded."
	/// </summary>
	void Awake()
	{
		this.sr = this.gameObject.GetComponent<SpriteRenderer>();
		this.origSprite = this.sr.sprite;
		this.origScale = this.transform.localScale;
		this.origColor = this.sr.color;
	}


	/// <summary>
	/// This method is invoked by Unity "when the object becomes enabled and active."
	/// </summary>
	void OnEnable()
	{
		try
		{
			//First off, if this item is not randomized, don't show it.
			string pool = resources.PinData()[this.pinData.ID].Pool;
			var isRandomized = pool switch
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
			if (!isRandomized)
			{
				this.disableSelf();
				return;
			}
			//Otherwise, check if it's reachable and/or if it has any prerequisites.
			this.SetIsPossible(this.pinData.Possible);
			if (this.isPossible)
			{
				//Set Pin state according to prereqs.
				this.setPrereqState(this.pinData.PreReqMet);
			}
			//Disable Pin if we've already obtained / checked this location.
			if (LogicManager.ItemIsChecked(this.pinData.ID.Replace('_', ' ')))
			{
				this.disableSelf();
			}
			else
			{

			}
		}
		catch (Exception e)
		{
			logger.Error($"Failed to enable pin: {e.Message} {e.StackTrace}");
		}
	}

	private void disableSelf()
	{
		this.gameObject.SetActive(false);
	}

	/// <summary>
	/// Shrinks and/or greys out a pin if it is not possible to get the pin at the current time.
	/// </summary>
	private void SetIsPossible(bool newValue)
	{
		//Dev.Log("Set Logic State: " + val);
		if (newValue == true && this.isPossible == false)
		{
			this.transform.localScale = this.origScale;
			this.sr.color = this.origColor;
			this.isPossible = true;
		}
		else if (newValue == false && this.isPossible == true)
		{
			this.transform.localScale = this.origScale * 0.5f;
			this.sr.color = Color.gray;
			this.isPossible = false;
		}
	}

	/// <summary>
	/// Sets whether the prereq is met for the current pin. Has the side effect
	/// of changing SpriteRenderer's pin to the "prereq pin" if the prereqs are not met.
	/// </summary>
	private void setPrereqState(bool newValue)
	{
		if (newValue == true && this.isPrereqMet == false)
		{
			this.sr.sprite = this.origSprite;
			this.isPrereqMet = true;
		}
		else if (newValue == false && this.isPrereqMet == true)
		{
			if (resources == null)
			{
				logger.Error("Tried to invoke setPrereqState when resources was null");
			}
			this.sr.sprite = resources.Sprite(SpriteId.MissingPrereq);
			this.isPrereqMet = false;
		}
	}
}