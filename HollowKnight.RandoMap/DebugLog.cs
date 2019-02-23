using System;
using System.Collections.Generic;
using System.IO;

namespace RandoMapMod {
	class DebugLog {
		private const string LOG_FILE = @"E:\Games\Steam\steamapps\common\Hollow Knight\hollow_knight_Data\Managed\Mods\RandoMapLog.txt";

		internal static void Write( string prmStr ) {
			Write( new string[] { prmStr } );
		}

		internal static void Write( List<string> prmList ) {
			Write( prmList.ToArray() );
		}

		internal static void Write( string[] lines ) {
#if DEBUG
			if ( File.Exists( LOG_FILE ) ) {
				using ( var writer = new StreamWriter( LOG_FILE, true ) ) {
					foreach ( var line in lines )
						writer.WriteLine( DateTime.Now.ToString( "HH:mm:ss tt" ) + " " + line );
				}
			}
#endif
		}
	}
}
