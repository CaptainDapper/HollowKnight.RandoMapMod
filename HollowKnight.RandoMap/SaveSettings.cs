using SereCore;

namespace RandoMapMod {
	public class SaveSettings : BaseSettings {
		#region Statics
		public static SaveSettings Instance;
		#endregion

		#region Constructors
		public SaveSettings() {
			AfterDeserialize += () => {
				//This space probably unintentially left blank
			};
			Instance = this;
		}
		#endregion

		#region Non-Private Non-Methods
		public bool MapsGiven {
			get => GetBool(false);
			set => SetBool(value);
		}
		#endregion
	}
}
