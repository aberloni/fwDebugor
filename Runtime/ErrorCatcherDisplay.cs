using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace fwp.debug
{

	/// <summary>
	/// https://docs.unity3d.com/Manual/LogFiles.html
	/// https://answers.unity.com/questions/331247/how-to-get-the-string-from-debuglog.html
	/// https://docs.unity3d.com/ScriptReference/Application.LogCallback.html
	/// </summary>
	public class ErrorCatcherDisplay : MonoBehaviour, iDump
	{
		protected const string _new_line = "\n";

		GUIStyle generic_style;
		Texture2D rgb_texture;

		struct LogEvent
		{
			public string log;
			public string stack;
		}

		List<LogEvent> logs = new();

		bool show = false;

		public string GetFilename() => "error-catcher";
		public bool IsTimestamped() => true;

		virtual protected string dumpSubFolder() => "Dump";

		void Start()
		{
			subApplication();

			show = false;

			Debug.Log("<color=red>ERROR CATCHER EXISTS</color>");

			setup();
		}

		virtual protected void setup()
		{ }

		/// <summary>
		/// a key to simulate an error
		/// </summary>
		virtual protected bool keySimuReleased()
		{
			return Keyboard.current.homeKey.wasReleasedThisFrame;
		}

		/// <summary>
		/// a key to force a dump
		/// </summary>
		virtual protected bool keyDumpReleased()
		{
			return false;
		}

		private void Update()
		{
			update();
		}

		virtual protected void update()
		{
			if (keyDumpReleased()) // force dump
			{
				Debug.LogWarning("dump!");
				dump();
			}

			if (keySimuReleased()) // simulate error
			{
				simulateLogError();
				simulateNullRef();
			}
		}

		virtual public string StringifyHeader()
		{
			return GetType().ToString() + "x" + logs.Count;
		}

		/// <summary>
		/// dump interface
		/// </summary>
		virtual public string StringifyContent()
		{
			string ret = string.Empty;

			foreach (var l in logs)
			{
				ret += _new_line + l.log;
				ret += _new_line + l.stack;
			}

			return ret;
		}

		private void OnDestroy()
		{
			unsubApplication();
			dump();
		}

		/// <summary>
		/// called on destroy
		/// </summary>
		void dump()
		{
			if (logs.Count > 0)
			{
				Dumper.dumpRoot(this, dumpSubFolder());
			}
		}

		void subApplication()
		{
			// main thread only
			// https://docs.unity3d.com/6000.2/Documentation/ScriptReference/Application-logMessageReceived.html
			//Application.logMessageReceived += HandleLog;

			// outside maion thread
			// The multi-threaded variant will also be called for messages on the main thread.
			// https://docs.unity3d.com/6000.2/Documentation/ScriptReference/Application-logMessageReceivedThreaded.html
			Application.logMessageReceivedThreaded += HandleLogThreaded;
		}

		void unsubApplication()
		{
			//Application.logMessageReceived -= HandleLog;
			Application.logMessageReceivedThreaded -= HandleLogThreaded;
		}

		string getHexMatchingColor(LogType lt)
		{
			switch (lt)
			{
				case LogType.Exception:
				case LogType.Assert:
				case LogType.Error:
					return "#FF0000";

				case LogType.Warning:
					return "#111100";
			}

			return "#AAAAAA";
		}

		void HandleLogThreaded(string logString, string stackTrace, LogType type)
			=> HandleLog(logString, stackTrace, type);

		void HandleLog(string logString, string stackTrace, LogType type)
		{
			switch (type)
			{
				case LogType.Error:
				case LogType.Assert:
				case LogType.Exception:
					addError(logString, stackTrace, type);
					break;
				case LogType.Log:
				case LogType.Warning:
				default:
					break;
			}
		}

		void addError(string log, string stack, LogType type)
		{
			var _log = new LogEvent();
			_log.log = $"[{type}]	" + log;
			_log.stack = stack;
			logs.Add(_log);

			//Debug.Log(" >>> (" + type + ") " + log);

			if (logs.Count > 10)
			{
				logs.RemoveAt(0); // oldest
			}

			if (contentStyle == null)
			{
				contentStyle = new GUIStyle();
				contentStyle.richText = true;
				contentStyle.normal.textColor = Color.red;
			}

			content = string.Empty;
			content += "ERROR:" + _new_line;
			content += log + _new_line;
			content += "STACKTRACE:" + _new_line;
			content += stack + _new_line;

			Render_Colored_Rectangle();

			show = true;
		}

		string content = string.Empty;
		Rect rect = new Rect();
		GUIStyle contentStyle;

		private void OnGUI()
		{
			if (!show)
				return;

			if (rgb_texture == null)
				return;

			drawBox();
			drawStack();
		}

		void drawBox()
		{
			GUI.skin.box = generic_style;

			rect.y = 0f;
			rect.width = rgb_texture.width;
			rect.height = rgb_texture.height;

			GUI.Box(rect, rgb_texture);
		}

		void drawStack()
		{
			rect.y = 100f;
			rect.width = 1000f;
			rect.height = 1000f;

			updateFontSize();

			GUI.Label(rect, content, contentStyle);
		}

		void updateFontSize()
		{

			float pxSize = 20f;
			float ratio = pxSize / 1920f; // N px on a 1080p screen
			int size = Mathf.FloorToInt(Screen.width * ratio);
			contentStyle.fontSize = size;

		}

		/// <summary>
		/// https://answers.unity.com/questions/37752/how-to-render-a-colored-2d-rectangle.html
		/// </summary>
		void Render_Colored_Rectangle()
		{
			int w = 40;
			int h = 40;
			if (rgb_texture == null) rgb_texture = new Texture2D(40, 40);
			Color rgb_color = new Color(1f, 0f, 0f);
			int i, j;
			for (i = 0; i < w; i++)
			{
				for (j = 0; j < h; j++)
				{
					rgb_texture.SetPixel(i, j, rgb_color);
				}
			}
			rgb_texture.Apply();
			if (generic_style == null) generic_style = new GUIStyle();
		}

		static public void simulateLogError()
		{
			Debug.LogError("simulated log.error");
		}

		static public void simulateNullRef()
		{
			GameObject obj = null;
			obj.name += "foo";
		}

#if UNITY_EDITOR
		[UnityEditor.MenuItem("Tools/Debugor/trigger error")]
		static public void cmMenuTrigger()
		{
			if (!Application.isPlaying) return;
			var ecd = GameObject.FindAnyObjectByType<ErrorCatcherDisplay>();
			ecd.cmTriggerError();
			ecd.cmTriggerNull();
		}

		[ContextMenu("Tools/Debugor/sim:log error")]
		public void cmTriggerError() => simulateLogError();

		[ContextMenu("Tools/Debugor/sim:null ref")]
		public void cmTriggerNull() => simulateNullRef();

#endif

	}

}