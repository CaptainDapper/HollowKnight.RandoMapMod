using System.Collections.Generic;

namespace RandoMapMod {
	class PinData_S {
		public static Dictionary<string, PinData> All {
			get {
				return Resources.PinData();
			}
		}
	}
}
