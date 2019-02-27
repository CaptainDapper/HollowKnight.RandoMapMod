using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace RandoMapMod {
	class PinData_S {
		public static Dictionary<string, PinData> All {
			get {
				return Resources.PinData();
			}
		}
	}

	class ObjectName {
		// "{\"sceneName\":\"Abyss_10\",\"objectName\":\"Dish Plat\",\"newShinyName\":\"Randomizer Shiny\"}",
		// "{\"sceneName\":\"RestingGrounds_07\",\"x\":27.0,\"y\":10.0,\"newShinyName\":\"New Shiny 0\"}",
		public string sceneName;
		public string objectName;
		public string newShinyName;
		public float x;
		public float y;
	}

	static class ObjectNames {
		private static List<ObjectName> list = null;

		internal static void Reset() {
			list = new List<ObjectName>();
		}

		internal static void Add( ObjectName onc ) {
			list.Add( onc );
		}

		internal static string Get( PinData pinD ) {
			if ( list == null )
				Load();

			foreach ( ObjectName onc in list ) {
				if ( pinD.NewShiny == true && pinD.NewX == (int) onc.x && pinD.NewY == (int) onc.y ) {
					return onc.newShinyName;
				}

				if ( onc.sceneName == pinD.SceneName && onc.objectName == pinD.OriginalName ) {
					return onc.newShinyName;
				}
			}

			return pinD.OriginalName;
		}

		internal static void Load() {
			Reset();

			foreach ( string val in RandomizerMod.RandomizerMod.Instance.Settings.StringValues.Values ) {
				if ( val.Contains( "newShinyName" ) ) {
					ObjectName newONC = JsonUtility.FromJson<ObjectName>( val );
					Add( newONC );
				}
			}
		}
	}
}
