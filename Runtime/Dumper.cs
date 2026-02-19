using UnityEngine;
using System.IO;
using System;
using System.Text;

namespace fwp.debug
{

	public interface iDump
	{
		public string GetFilename();
		public string StringifyHeader();
		public string StringifyContent();
		public bool IsTimestamped();
	}

	public class Dumper
	{
		// data/dump/file.dump
		public const string __defaultFolder = "Dumps";

		// file.dump
		public const string __ext = ".dump";

		static DateTime _dt_init;

		/// <summary>
		/// time elapsed since app launched
		/// </summary>
		static TimeSpan DeltaSinceStartup => DateTime.Now - _dt_init;

		[RuntimeInitializeOnLoadMethod]
		static void initialize()
		{
			_dt_init = DateTime.Now;
		}

		public string PathDump
		{
			get
			{
				return Path.Combine(GetLocalHDrivePath(), "dump");
			}
		}

		/// <summary>
		/// subfolder
		/// data/subFolder/dump.dump
		/// </summary>
		/// <param name="candidate"></param>
		/// <param name="subFolder"></param>
		public Dumper(iDump candidate, string subFolder = __defaultFolder, bool noDuplicate = false)
		{
			string _path = solvePath(subFolder);
			if (!Directory.Exists(_path)) Directory.CreateDirectory(_path);
			_path = Path.Combine(_path, solveFileName(candidate));

			if (noDuplicate && File.Exists(_path))
			{
				if (Application.isEditor)
				{
					Debug.Log("[dump] no duplicate @ " + _path);

				}
				return;
			}

			dump(candidate, _path);
		}

		/// <summary>
		/// returns filename.ext
		/// </summary>
		string solveFileName(iDump candidate)
		{
			string _file = hardware.Hardware.DeviceUid + "_" + candidate.GetFilename();

			var dt = System.DateTime.Now;

			if (candidate.IsTimestamped())
			{
				// device_file_2025...
				_file += "_" + dt.ToString("yyyy-MM-dd_HH-mm");
			}

			return _file + __ext;
		}

		/// <summary>
		/// folder where file should be created/dumped
		/// </summary>
		string solvePath(string subFolder = null)
		{
			string _path = GetLocalHDrivePath();

			if (!string.IsNullOrEmpty(subFolder))
			{
				_path = Path.Combine(_path, subFolder);
			}

			return _path;
		}

		/// <summary>
		/// absPath is path/filename.ext
		/// </summary>
		void dump(iDump candidate, string absPath)
		{
			// SOLVE PATH

			// SOLVE CONTENT

			
			StringBuilder dump = new();

			dump.AppendLine("[" + fwp.hardware.Hardware.DeviceUid + "]");

			var dt = DateTime.Now;
			dump.AppendLine(dt.ToString("yyyy-MM-dd HH:mm:ss"));

			if (candidate.IsTimestamped())
			{
				dump.AppendLine("time played: " + DeltaSinceStartup); // HH:MM:SS
			}

			dump.AppendLine(string.Empty);
			dump.AppendLine(candidate.StringifyHeader());
			dump.AppendLine(candidate.StringifyContent());

			// DUMP

			File.WriteAllText(absPath, dump.ToString());

			if (Application.isEditor)
			{
				Debug.Log("/! dump (len:" + dump.Length + "): " + absPath);
			}
		}

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
		virtual protected string GetLocalHDrivePath()
		{
			if (Application.isEditor) return Application.dataPath;
			else return Application.persistentDataPath;
		}
	}

}