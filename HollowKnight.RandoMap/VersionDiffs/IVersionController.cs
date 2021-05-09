using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace RandoMapMod.VersionDiffs {
	public interface IVersionController {
		#region Non-Private Non-Methods
		public bool RandomizeGrimmkinFlames();

		Assembly GetInfoAssembly();

		bool CanGet(string itemName);
		#endregion
	}
}
