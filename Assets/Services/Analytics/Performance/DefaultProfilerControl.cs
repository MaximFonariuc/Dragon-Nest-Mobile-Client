using System;
using UnityEngine;
#if UNITY_5_5_OR_NEWER
using UnityEngine.Profiling;
#endif

namespace Unity.Performance
{
	class DefaultProfilerControl : IProfilerControl
	{
		public bool supported { get { return Profiler.supported; } }

		public bool recording { get { return Profiler.supported && Profiler.enabled; } }

		public void StartRecording (string filePath)
		{
			Profiler.logFile = filePath;
			Profiler.enableBinaryLog = true;
			Profiler.enabled = true;
		}

		public void StopRecording ()
		{
			Profiler.enabled = false;
		}
	}
}