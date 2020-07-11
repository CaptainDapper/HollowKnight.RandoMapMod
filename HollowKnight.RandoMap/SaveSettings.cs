using Modding;
using SeanprCore;
using System.Collections.Generic;
using System.Linq;

namespace RandoMapMod
{
	public class SaveSettings : BaseSettings
	{
		public static SaveSettings Instance;
		public SaveSettings()
		{
			AfterDeserialize += () =>
			{

			};
			Instance = this;
		}

		public bool MapsGiven
		{
			get => GetBool(false);
			set => SetBool(value);
		}
	}
}
