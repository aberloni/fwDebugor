using UnityEngine;

namespace fwp.hardware
{
	[System.Serializable]
	public class Hardware : fwp.debug.iDump
	{
		/// <summary>
		/// must be called by context at appropriate time
		/// </summary>
		static public void dump()
		{
			fwp.debug.Dumper.dumpSingle(Instance);
		}

		static Hardware instance;
		static public Hardware Instance
		{
			get
			{
				if (instance == null) new Hardware();
				return instance;
			}
		}

		string deviceModel;
		string deviceName;
		DeviceType deviceType;
		string graphicsDeviceName;
		string graphicsDeviceVendor;
		string graphicsDeviceVersion;

		string operatingSystem;
		int processorCount;
		string processorType;
		int systemMemorySize;

		public string GetFilename() => "hardware";
		public bool IsTimestamped() => false;

		public Hardware()
		{
			instance = this;

			readLocals();
		}

		static public string DeviceUid => Instance.deviceName;

		public void readLocals()
		{
			deviceModel = SystemInfo.deviceModel;
			deviceName = SystemInfo.deviceName;
			deviceType = SystemInfo.deviceType;
			graphicsDeviceName = SystemInfo.graphicsDeviceName;
			graphicsDeviceVendor = SystemInfo.graphicsDeviceVendor;
			graphicsDeviceVersion = SystemInfo.graphicsDeviceVersion;

			operatingSystem = SystemInfo.operatingSystem;
			processorCount = SystemInfo.processorCount;
			processorType = SystemInfo.processorType;

			systemMemorySize = SystemInfo.systemMemorySize;
		}

		public string Stringify()
		{
			string ret = "<b>[HARDWARE]</b>";
			ret += "\nDevice model: " + deviceModel;
			ret += "\nDevice name: " + deviceName;
			ret += "\nDevice type: " + deviceType;
			ret += "\nGraphics device name: " + graphicsDeviceName;
			ret += "\nGraphics device vendor: " + graphicsDeviceVendor;
			ret += "\nGraphics device version: " + graphicsDeviceVersion;
			ret += "\nOS: " + operatingSystem;
			ret += "\nProcessor count: " + processorCount;
			ret += "\nProcessor type: " + processorType;
			ret += "\nMemory size: " + systemMemorySize;
			return ret;
		}

		public void log()
		{
			Debug.Log(Stringify());
		}

#if UNITY_EDITOR
		[UnityEditor.MenuItem("Tools/Debugor/Hardware/log")]
		static void miLogHardware()
		{
			new Hardware().log();
		}
		[UnityEditor.MenuItem("Tools/Debugor/Hardware/dump")]
		static void miDumpHardware() => dump();

#endif

	}

}