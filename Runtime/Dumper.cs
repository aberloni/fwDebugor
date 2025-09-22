using UnityEngine;
using System.IO;
using System;

namespace fwp.debug
{

	public interface iDump
	{
		public string GetFilename();
		public string Stringify();
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

		static string solvePath(iDump candidate)
		{
			var dt = System.DateTime.Now;

			// device_file
			string _file = hardware.Hardware.DeviceUid + "_" + candidate.GetFilename();

			if (candidate.IsTimestamped())
			{
				// device_file_2025...
				_file += "_" + dt.ToString("yyyy-MM-dd_HH-mm");
			}

			string path = Path.Combine(PathData, _file + ext);

			return path;
		}

		/// <summary>
		/// won't dump if file exists
		/// </summary>
		static public void dumpSingle(iDump candidate)
		{
			string path = solvePath(candidate);
			
			if (File.Exists(path))
				return;

			dump(candidate, path);
		}

		/// <summary>
		/// dump content in a file at root level of build/editor
		/// </summary>
		static public void dumpRoot(iDump candidate)
		{
			string path = solvePath(candidate);
			dump(candidate, path);
		}

		static void dump(iDump candidate, string path)
		{
			var dt = System.DateTime.Now;

			// header
			string dump = "[" + fwp.hardware.Hardware.DeviceUid + "]	" + dt.ToString("yyyy-MM-dd HH:mm:ss");
			if(candidate.IsTimestamped())
			{
				dump += Environment.NewLine + "> played time :	" + DeltaSinceStartup; // HH:MM:SS
			}
			
			// content
			dump += System.Environment.NewLine + candidate.Stringify();

			// dump
			File.WriteAllText(path, dump);

			Debug.LogWarning("dump (" + dump.Length + ") @ " + path);
		}
	}

}