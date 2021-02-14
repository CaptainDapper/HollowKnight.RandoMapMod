using System;
using System.IO;
using System.Reflection;

namespace RandoMapMod {
	public class DebugLog {
		#region Static
		private static string _logPath = "";
		private static string _LogPath {
			get {
				if (_logPath == "") {
					string codeBase = Assembly.GetExecutingAssembly().CodeBase;
					UriBuilder uri = new UriBuilder(codeBase);
					_logPath = Path.GetDirectoryName(Uri.UnescapeDataString(uri.Path));
				}
				return _logPath;
			}
		}

		private static string _LogFile => _LogPath + @"/RandoMapMod.log";
		#endregion


		private readonly string _name;

		public DebugLog(string name) {
			this._name = name;
		}

		public void Error(string msg) {
			this._Write("ERROR", msg);
			RandoMapMod.Instance.LogError(msg);
		}

		public void Log(string v) {
			this._Write("LOG", v);
			RandoMapMod.Instance.Log(v);
		}

		public void Warn(string v) {
			this._Write("WARN", v);
			RandoMapMod.Instance.LogWarn(v);
		}

		private void _Write(string logLevel, string line) {
			if (File.Exists(_LogFile)) {
				DateTime now = DateTime.Now;
				using StreamWriter writer = new StreamWriter(_LogFile, true);
				writer.WriteLine($"{now:HH:mm:ss tt} {logLevel,5} {this._name,12} - {line}");
			}
		}
	}
}