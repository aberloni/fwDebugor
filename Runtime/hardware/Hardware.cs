using System;
using System.Text;
using UnityEngine;

using fwp.debug;

namespace fwp.hardware
{
	[System.Serializable]
	public class Hardware : fwp.debug.iDump
	{
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

		virtual public string StringifyHeader()
		{
			return "[HARDWARE]";
		}

		virtual public string StringifyContent()
		{
			StringBuilder str = new();

			str.AppendLine("OS: " + operatingSystem);
			str.AppendLine("Memory size: " + systemMemorySize);

			str.AppendLine("Device model: " + deviceModel);
			str.AppendLine("Device name: " + deviceName);
			str.AppendLine("Device type: " + deviceType);

			str.AppendLine("Graphics device name: " + graphicsDeviceName);
			str.AppendLine("Graphics device vendor: " + graphicsDeviceVendor);
			str.AppendLine("Graphics device version: " + graphicsDeviceVersion);

			str.AppendLine("Processor count: " + processorCount);
			str.AppendLine("Processor type: " + processorType);

			return str.ToString();
		}

		public void log()
		{
			Debug.Log(StringifyHeader());
			Debug.Log(StringifyContent());
		}

#if UNITY_EDITOR
		[UnityEditor.MenuItem(DebugorStatics.base_path + "Hardware/log")]
		static void miLogHardware()
		{
			Instance.log();
		}
		[UnityEditor.MenuItem(DebugorStatics.base_path + "Hardware/dump")]
		static void miDumpHardware()
		{
			new Dumper(Instance);
		}
#endif

	}

}