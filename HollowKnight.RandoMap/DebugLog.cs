using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace RandoMapMod {
	static class DebugLog {
		private static string pLogPath = "";
		public static string LogPath {
			get {
				if ( pLogPath == "" ) {
					string codeBase = Assembly.GetExecutingAssembly().CodeBase;
					UriBuilder uri = new UriBuilder( codeBase );
					pLogPath = Path.GetDirectoryName(Uri.UnescapeDataString( uri.Path ));
				}
				return pLogPath;
			}
		}

		public static string LogFile {
			get {
				return LogPath + @"/RandoMapMod.log";
			}
		}

		internal static void Write( string prmStr ) {
			Write( new string[] { prmStr } );
		}

		internal static void Write( List<string> prmList ) {
			Write( prmList.ToArray() );
		}

		internal static void Write( string[] lines ) {
			if ( File.Exists( LogFile ) ) {
				using ( var writer = new StreamWriter( LogFile, true ) ) {
					foreach ( var line in lines )
						writer.WriteLine( DateTime.Now.ToString( "HH:mm:ss tt" ) + " " + line );
				}
			}
		}

		internal static void Error( string v ) {
			RandoMapMod.Instance.LogError( v );
		}

		internal static void Log( string v ) {
			RandoMapMod.Instance.Log( v );
		}

		internal static void Warn( string v ) {
			RandoMapMod.Instance.LogWarn( v );
		}
	}
}
