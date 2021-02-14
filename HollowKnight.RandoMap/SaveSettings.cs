using SeanprCore;

namespace RandoMapMod {
	public class SaveSettings : BaseSettings {
		public static SaveSettings Instance;
		public SaveSettings() {
			AfterDeserialize += () => {
				//This space probably unintentially left blank
			};
			Instance = this;
		}

		public bool MapsGiven {
			get => GetBool(false);
			set => SetBool(value);
		}
	}
}
