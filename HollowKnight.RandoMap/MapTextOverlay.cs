using Modding;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace RandoMapMod
{
	class MapTextOverlay
	{
		private static readonly DebugLog logger = new DebugLog(nameof(MapTextOverlay));

		private GameObject canvas = null;
		private GameObject textObj = null;
		private Text textComponent;

		private GameObject GetOrInitializeCanvas()
		{
			if (canvas == null)
			{
				logger.Log("Initializing Canvas.");
				canvas = new GameObject();
				canvas.AddComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
				CanvasScaler scaler = canvas.AddComponent<CanvasScaler>();
				scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
				scaler.referenceResolution = new Vector2(1920f, 1080f);
				UnityEngine.Object.DontDestroyOnLoad(canvas);
			}
			return canvas;
		}

		private GameObject GetOrInitializeTextObj()
		{
			if (textObj == null)
			{
				logger.Log("Initializing textObj.");
				textObj = new GameObject();
				textObj.AddComponent<CanvasRenderer>();
				CanvasGroup group = textObj.AddComponent<CanvasGroup>();
				group.interactable = false;
				group.blocksRaycasts = false;
				RectTransform textTransform = textObj.AddComponent<RectTransform>();
				textObj.transform.SetParent(GetOrInitializeCanvas().transform, false);
				Vector2 pos = new Vector2(100f, 100f);
				Vector2 size = new Vector2(500f, 400f);
				Vector2 anchorPosition = new Vector2((pos.x + size.x / 2f) / 1920f, (1080f - (pos.y + size.y / 2f)) / 1080f);
				textTransform.anchorMin = anchorPosition;
				textTransform.anchorMax = anchorPosition;
				textObj.SetActive(false);
				UnityEngine.Object.DontDestroyOnLoad(textObj);
			}
			return textObj;
		}

		private Text GetOrInitializeTextComponent()
		{
			if (textComponent == null)
			{
				logger.Log("Initializing textComponent");
				textComponent = GetOrInitializeTextObj().AddComponent<Text>();
				textComponent.horizontalOverflow = HorizontalWrapMode.Overflow;
				textComponent.resizeTextForBestFit = true;
				textComponent.text = "";
				CanvasUtil.CreateFonts();
				textComponent.font = CanvasUtil.TrajanNormal;
				textComponent.fontSize = 30;
				textComponent.fontStyle = FontStyle.Normal;
				textComponent.alignment = TextAnchor.UpperLeft;
				textComponent.color = Color.white;
			}
			return textComponent;
		}

		public MapTextOverlay()
		{
		}

		public void Show(HelperData helperData)
		{
			try
			{
				if (helperData == null)
				{
					return; //do nothing
				}
				string[] foo = helperData.reachable.Values
					.Where((location) => location.items.Count > 0)
					.OrderByDescending((location) => location.items.Count)
					.Select((location) => $"{location.name} - {location.items.Count} reachable")
					.ToArray();
				GetOrInitializeTextComponent().text = string.Join("\n", foo);
				GetOrInitializeTextObj().SetActive(true);
			}
			catch (Exception e)
			{
				logger.Warn($"Show failed: {e}");
			}
		}
		public void Hide()
		{
			if (textComponent == null)
			{
				logger.Warn("Hide: textComponent was null");
			}
			else
			{
				GetOrInitializeTextComponent().text = "";
			}
			if (textObj == null)
			{
				logger.Warn("Hide: textObj was null");
			}
			else
			{
				GetOrInitializeTextObj().SetActive(false);
			}
		}
	}
}
