using UnityEngine;
using System.Diagnostics.Eventing.Reader;
using System.Collections;
using System;
using System.Collections.Generic;
using RandoMapMod.BoringInternals;

namespace RandoMapMod.UnityComponents {
	class InputListener : MonoBehaviour {
		#region Statics
		private static GameObject _instance_GO = null;

		public static InputListener Instance {
			get {
				InstantiateSingleton();
				return _instance_GO.GetComponent<InputListener>();
			}
		}

		public static void InstantiateSingleton() {
			if (_instance_GO == null) {
				_instance_GO = GameObject.Find("RandoMapInputListener");
				if (_instance_GO == null) {
					DebugLog.Log("Adding Input Listener.");
					_instance_GO = new GameObject("RandoMapInputListener");
					_instance_GO.AddComponent<InputListener>();
					DontDestroyOnLoad(_instance_GO);
				}
			}
		}
		#endregion

		#region Private Non-Methods
		private string _typedString = "";
		#endregion

		#region MonoBehaviour "Overrides"
		protected void Update() {
			if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)) {
				if (Input.GetKeyDown(KeyCode.P)) {
					DebugLog.Log("Ctrl+P : Toggle Pins");
					MapMod.TogglePins();
				}
				if (Input.GetKeyDown(KeyCode.G)) {
					DebugLog.Log("Ctrl+G : Toggle Resource Helpers");
					MapMod.ToggleResourceHelpers();
				}
				if (Input.GetKeyDown(KeyCode.M)) {
					DebugLog.Log("Ctrl+M : Give All Maps");
					MapMod.GiveAllMaps("Hotkey");
				}
			}

			List<(string, Action)> keyPhrases = new List<(string, Action)> {
				("alsoafraidofchange", () => MapMod.SetPinStyleOrReturnToNormal(MapMod.PinStyles.AlsoAfraid)),
				("afraidofchange", () => MapMod.SetPinStyleOrReturnToNormal(MapMod.PinStyles.Afraid)),
				(SeriouslyBoring.BORING_PHRASE_1, SeriouslyBoring.ToggleBoringMode1),
				(SeriouslyBoring.BORING_PHRASE_2, SeriouslyBoring.ToggleBoringMode2),
			};

			string inputString = Input.inputString;
			if (inputString != string.Empty) {
				_typedString += inputString.Replace("'", "").ToLower();

				foreach ((string phrase, Action call) item in keyPhrases) {
					if (_typedString.ToLower().Contains(item.phrase.ToLower())) {
						DebugLog.Log($"'{item.phrase}' KeyPhrase found!");
						item.call.Invoke();

						_typedString = "";
					}
				}

				if (_typedString.Length > 20) {
					_typedString.Substring(1);
				}
			}
		}
		#endregion

		#region Non-Private Methods
		#endregion
	}
}
