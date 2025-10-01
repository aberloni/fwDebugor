using UnityEngine;
using System.IO;
using System;

namespace fwp.debug
{

	public interface iDump
	{
		public string GetFilename();
		public string StringifyHeader();
		public string StringifyContent();
		public bool IsTimestamped();
	}

	static public class Dumper
	{
		const string ext = ".dump";

		/// <summary>
		/// https://docs.unity3d.com/6000.0/Documentation/ScriptReference/Application-dataPath.html
		/// 
		/// build :	app/_data/
		/// editor:	app/Assets/
		/// 
		/// Win/Linux player: <path to executablename_Data folder> (note that most Linux installations will be case-sensitive!)
		/// Unity Editor: <path to project folder>/Assets
		/// Android: Normally it points directly to the APK.If you are running a split binary build, it points to the OBB instead.
		/// </summary>
		static public string PathData => Application.dataPath;

		// app/_data/dump/
		static public string Dump
		{
			get
			{
				return Path.Combine(PathData, "dump");
			}
		}

		static DateTime _dt_init;
		static TimeSpan DeltaSinceStartup => DateTime.Now - _dt_init;

		[RuntimeInitializeOnLoadMethod]
		static void generate()
		{
			_dt_init = DateTime.Now;
		}

		static string solvePath(iDump candidate, string subFolder = null)
		{
			var dt = System.DateTime.Now;

			// device_file
			string _file = hardware.Hardware.DeviceUid + "_" + candidate.GetFilename();

			if (candidate.IsTimestamped())
			{
				// device_file_2025...
				_file += "_" + dt.ToString("yyyy-MM-dd_HH-mm");
			}

			string path = PathData;

			if (!string.IsNullOrEmpty(subFolder))
			{
				path = Path.Combine(PathData, subFolder);

				if (!Directory.Exists(path)) Directory.CreateDirectory(path);
			}

			path = Path.Combine(path, _file + ext);

			return path;
		}

		/// <summary>
		/// won't dump if file exists
		/// </summary>
		static public void dumpSingle(iDump candidate, string subFolder = null)
		{
			string path = solvePath(candidate, subFolder);

			if (File.Exists(path))
				return;

			dump(candidate, path);
		}

		/// <summary>
		/// dump content in a file at root level of build/editor
		/// </summary>
		static public void dumpRoot(iDump candidate, string subFolder = null)
		{
			dump(candidate, solvePath(candidate, subFolder));
		}

		static void dump(iDump candidate, string path)
		{
			var dt = System.DateTime.Now;

			// universal header
			string dump = "[" + fwp.hardware.Hardware.DeviceUid + "]	" + dt.ToString("yyyy-MM-dd HH:mm:ss");
			if (candidate.IsTimestamped())
			{
				dump += Environment.NewLine + "> played time :	" + DeltaSinceStartup; // HH:MM:SS
			}

			// header
			dump += System.Environment.NewLine + candidate.StringifyHeader();

			// content
			dump += System.Environment.NewLine + candidate.StringifyContent();

			// dump
			File.WriteAllText(path, dump);

			Debug.LogWarning("dump (" + dump.Length + ") @ " + path);
		}
	}

}