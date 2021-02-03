using Modding;
using RandoMapMod;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

class MapTextOverlay : Component {
	private static readonly DebugLog _logger = new DebugLog(nameof(MapTextOverlay));

	private GameObject _canvas = null;
	private GameObject _textObj = null;
	private Text _textComponent;

	private GameObject _GetOrInitializeCanvas() {
		if (_canvas == null) {
			_logger.Log("Initializing Canvas.");
			_canvas = new GameObject();
			_canvas.AddComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
			CanvasScaler scaler = _canvas.AddComponent<CanvasScaler>();
			scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
			scaler.referenceResolution = new Vector2(1920f, 1080f);
			DontDestroyOnLoad(_canvas);
		}
		return _canvas;
	}

	private GameObject _GetOrInitializeTextObj() {
		if (_textObj == null) {
			_logger.Log("Initializing textObj.");
			_textObj = new GameObject();
			_textObj.AddComponent<CanvasRenderer>();
			CanvasGroup group = _textObj.AddComponent<CanvasGroup>();
			group.interactable = false;
			group.blocksRaycasts = false;
			RectTransform textTransform = _textObj.AddComponent<RectTransform>();
			_textObj.transform.SetParent(_GetOrInitializeCanvas().transform, false);
			Vector2 pos = new Vector2(100f, 100f);
			Vector2 size = new Vector2(500f, 400f);
			Vector2 anchorPosition = new Vector2((pos.x + size.x / 2f) / 1920f, (1080f - (pos.y + size.y / 2f)) / 1080f);
			textTransform.anchorMin = anchorPosition;
			textTransform.anchorMax = anchorPosition;
			_textObj.SetActive(false);
			UnityEngine.Object.DontDestroyOnLoad(_textObj);
		}
		return _textObj;
	}

	private Text _GetOrInitializeTextComponent() {
		if (_textComponent == null) {
			_logger.Log("Initializing textComponent");
			_textComponent = _GetOrInitializeTextObj().AddComponent<Text>();
			_textComponent.horizontalOverflow = HorizontalWrapMode.Overflow;
			_textComponent.resizeTextForBestFit = true;
			_textComponent.text = "";
			CanvasUtil.CreateFonts();
			_textComponent.font = CanvasUtil.TrajanNormal;
			_textComponent.fontSize = 30;
			_textComponent.fontStyle = FontStyle.Normal;
			_textComponent.alignment = TextAnchor.UpperLeft;
			_textComponent.color = Color.white;
		}
		return _textComponent;
	}

	public void Show() {
		HelperLog.Refresh();

		try {
			if (HelperLog.Data == null) {
				return; //do nothing
			}
			_GetOrInitializeTextComponent().text = string.Join("\n", HelperLog.Data.GetReachableCountStrings());
			_GetOrInitializeTextObj().SetActive(true);
		} catch (Exception e) {
			_logger.Warn($"Show failed: {e}");
		}
	}

	public void Hide() {
		if (_textComponent == null) {
			_logger.Warn("Hide: textComponent was null");
		} else {
			_GetOrInitializeTextComponent().text = "";
		}
		if (_textObj == null) {
			_logger.Warn("Hide: textObj was null");
		} else {
			_GetOrInitializeTextObj().SetActive(false);
		}
	}
}
