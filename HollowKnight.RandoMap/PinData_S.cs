using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RandoMapMod {
	class PinData_S {
		private static List<PinData> pAll = null;

		public static List<PinData> All {
			get {
				return Resources.PinData();
			}
		}
	}
}
