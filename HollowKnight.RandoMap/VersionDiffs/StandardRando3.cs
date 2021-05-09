using RandomizerLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace RandoMapMod.VersionDiffs {
	class StandardRando3 : IVersionController {
		#region Non-Private Non-Methods
		public bool RandomizeGrimmkinFlames() {
			return RandomizerMod.RandomizerMod.Instance.Settings.RandomizeGrimmkinFlames;
		}

		public Assembly GetInfoAssembly() {
			return typeof(RandomizerMod.RandomizerMod).Assembly;
		}

		public bool CanGet(string itemName) {
			throw new NotImplementedException();
		}
		#endregion
	}
}
