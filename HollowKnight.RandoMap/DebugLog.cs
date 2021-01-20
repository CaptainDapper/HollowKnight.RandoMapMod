using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace RandoMapMod
{
	class DebugLog
	{
		private static string pLogPath = "";
		private static string LogPath
		{
			get
			{
				if (pLogPath == "")
				{
					string codeBase = Assembly.GetExecutingAssembly().CodeBase;
					UriBuilder uri = new UriBuilder(codeBase);
					pLogPath = Path.GetDirectoryName(Uri.UnescapeDataString(uri.Path));
				}
				return pLogPath;
			}
		}

		private static string LogFile
		{
			get
			{
				return LogPath + @"/RandoMapMod.log";
			}
		}

		private readonly string name;

		public DebugLog(string name)
		{
			this.name = name;
		}

		private void Write(string logLevel, string line)
		{
			if (File.Exists(LogFile))
			{
				DateTime now = DateTime.Now;
				using (var writer = new StreamWriter(LogFile, true))
				{
					writer.WriteLine($"{now:HH:mm:ss tt} {logLevel,5} {this.name,12} - {line}");
				}
			}
		}

		internal void Error(string v)
		{
			this.Write("ERROR", v);
			RandoMapMod.Instance.LogError(v);
		}

		internal void Log(string v)
		{
			this.Write("LOG", v);
			RandoMapMod.Instance.Log(v);
		}

		internal void Warn(string v)
		{
			this.Write("WARN", v);
			RandoMapMod.Instance.LogWarn(v);
		}
	}
}