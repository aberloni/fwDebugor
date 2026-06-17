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
				if (instance == null) instance = new();
				return instance;
			}

		}

		public string DeviceUID => deviceName;

		public string deviceModel;
		public string deviceName;
		public DeviceType deviceType;
		public string graphicsDeviceName;
		public string graphicsDeviceVendor;
		public string graphicsDeviceVersion;

		public string operatingSystem;
		public int processorCount;
		public string processorType;
		public int systemMemorySize;

		struct Graphics
		{
			public Vector2 screenSize;

			/// <summary>
			/// vSyncCount = 0 → VSync off (may get tearing but faster FPS).
			/// vSyncCount = 1 → sync to monitor refresh (e.g., 60Hz → 60 FPS).
			/// vSyncCount = 2 → sync every second frame (half refresh rate).
			/// </summary>
			public int vSyncCount;
			public int antiAliasing;

			public void read()
			{
				screenSize.x = Screen.width;
				screenSize.y = Screen.height;
				vSyncCount = QualitySettings.vSyncCount;
				antiAliasing = QualitySettings.antiAliasing;
			}
		}

		Graphics graphics;

		public string GetFilename() => "hardware";
		public bool IsTimestamped() => false;

		public Hardware()
		{
			instance = this;
			readLocals();
		}
		
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

			graphics.read();
		}

		virtual public string StringifyHeader()
		{
			return "[HARDWARE]";
		}

		public string StringifyContent()
		{
			return doStringify().ToString();
		}

		virtual protected System.Text.StringBuilder doStringify()
		{
			System.Text.StringBuilder str = new();

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

			str.AppendLine("Vsync: " + graphics.vSyncCount);

			return str;
		}

		virtual public void log()
		{
			Debug.Log(StringifyHeader());
			Debug.Log(StringifyContent());
		}

#if UNITY_EDITOR
		[UnityEditor.MenuItem(DebugorStatics.base_path + "Hardware/log")]
		static void miLogHardware()
		{
			if (instance == null) Debug.LogWarning("can't log hardware, not instance of wrapper");
			else instance.log();
		}
		[UnityEditor.MenuItem(DebugorStatics.base_path + "Hardware/dump")]
		static void miDumpHardware()
		{
			if (instance == null) Debug.LogWarning("can't dump hardware, not instance of wrapper");
			else new Dumper(instance);
		}
#endif

	}

}