using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace RandoMapMod {
	class PinData_S {
		private static List<PinData> pAll = null;

		public static List<PinData> All {
			get {
				return Resources.PinData();
			}
		}
	}

	class ObjectNameChange {
		// "{\"sceneName\":\"Abyss_10\",\"objectName\":\"Dish Plat\",\"newShinyName\":\"Randomizer Shiny\"}",
		public string sceneName;
		public string objectName;
		public string newShinyName;

		private static List<ObjectNameChange> list = null;

		internal static void Reset() {
			list = new List<ObjectNameChange>();
		}

		internal static void Add(ObjectNameChange onc) {
			list.Add( onc );
		}

		internal static string GetName( string pSceneName, string pObjectName ) {
			foreach ( ObjectNameChange onc in list ) {
				if ( onc.sceneName == pSceneName && onc.objectName == pObjectName ) {
					return onc.newShinyName;
				}
			}

			return pObjectName;
		}

		internal static void Load() {
			Reset();

			foreach ( string val in RandomizerMod.RandomizerMod.Instance.Settings.StringValues.Values ) {
				if ( val.Contains( "newShinyName" ) ) {
					ObjectNameChange newONC = JsonUtility.FromJson<ObjectNameChange>( val );
					Add( newONC );
				}
			}
		}
	}
}
