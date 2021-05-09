using System;
using System.CodeDom;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace RandoMapMod {
	public static class DebugLog {
		#region Statics
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
		//This folder ends up being in the Mods folder! "hollow_knight_Data\Managed\Mods"
		private static string _LogFile => _LogPath + @"/RandoMapMod.log";

		public static void Trace() {
			string msg = new StackTrace(1, true).ToString();
			_Write("ERROR", msg);
			MapMod.Instance.LogError(msg);
		}

		public static void Error(string msg, Exception inner) {
			Error($"{msg}\n-----Inner Exception:-----\n{inner}\n-----");
		}

		public static void Error(string msg) {
			msg += "\n" + new StackTrace(1, true).ToString();
			_Write("ERROR", msg);
			MapMod.Instance.LogError(msg);
		}

		public static void Log(string v) {
			_Write("LOG", v);
			MapMod.Instance.Log(v);
		}

		public static void Warn(string v) {
			_Write("WARN", v);
			MapMod.Instance.LogWarn(v);
		}

		private static void _Write(string logLevel, string line) {
#if DEBUG
			string nickName = _DetermineClassNickName();

			string msg = $"{DateTime.Now:HH:mm:ss tt} {logLevel,5} {nameof(MapMod),12} - {line}";
			if (!File.Exists(_LogFile)) {
				try {
					File.Create(_LogFile);
				} catch {
					MapMod.Instance.LogWarn("RandoMapLog.log could not be created...");
				}
			}
			using StreamWriter writer = new StreamWriter(_LogFile, true);
			writer.WriteLine(msg);
#endif
		}

		private static string _DetermineClassNickName() {
			StackTrace st = new StackTrace(1);
			int i = 0;

			Type callingClass;
			do {
				callingClass = st.GetFrame(i).GetMethod().ReflectedType;
				i++;
			} while (callingClass == typeof(DebugLog));

			DebugNameAttribute[] attr = (DebugNameAttribute[]) callingClass.GetCustomAttributes(typeof(DebugNameAttribute), false);
			if (attr.Length > 0) {
				return attr[0].ToString();
			}

			return nameof(MapMod);
		}
		#endregion
	}

	[AttributeUsage(AttributeTargets.Class)]
	public class DebugNameAttribute : Attribute {
		private readonly string _nickname;

		public DebugNameAttribute(string nick) {
			_nickname = nick;
		}

		public override string ToString() {
			return _nickname;
		}
	}
}