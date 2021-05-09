using UnityEngine;
using System.Diagnostics.Eventing.Reader;
using System.Collections;

namespace RandoMapMod.UnityComponents {
	class InputListener : MonoBehaviour {
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

#region Statics
		private const string OLD_PINS = "afraidofchange";
		private const string OLD_PINS_INV = "alsoafraidofchange";
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

			string inputString = Input.inputString;
			if (inputString != string.Empty) {
				_typedString += inputString.Replace("'", "").ToLower();

				if (_typedString.ToLower().Contains(OLD_PINS_INV)) {
					DebugLog.Log($"'{OLD_PINS}': Toggle Old Pins (inverted)");
					MapMod.SetPinStyleOrReturnToNormal(MapMod.PinStyles.AlsoAfraid);

					_typedString = "";
				} else if (_typedString.ToLower().Contains(OLD_PINS)) {
					DebugLog.Log($"'{OLD_PINS}': Toggle Old Pins");
					MapMod.SetPinStyleOrReturnToNormal(MapMod.PinStyles.Afraid);

					_typedString = "";
				}

				if (_typedString.Length > 20) {
					_typedString.Substring(1);
				}
			}
		}
		#endregion

		#region Non-Private Methods
		public void CheckIn() {
			// Literally just makes the instance happen... This is probably stupid.
		}
		#endregion
	}
}
