using RandomizerLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace RandoMapMod.VersionDiffs {
	class MultiWorldRando3 : IVersionController {
		#region Non-Private Non-Methods
		public bool RandomizeGrimmkinFlames() {
			return false;
		}

		public Assembly GetInfoAssembly() {
			return typeof(LogicManager).Assembly;
		}

		public bool CanGet(string itemName) {
			throw new NotImplementedException();
		}
		#endregion
	}
}
