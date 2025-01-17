using UnityEngine;
#if UNITY_5_5_OR_NEWER
using UnityEngine.Profiling;
#endif

namespace Unity.Performance
{
	public class DefaultDataSource : IDataSource
	{
		public float unscaledDeltaTimeSeconds { get { return Time.unscaledDeltaTime; } }

		public float realtimeSinceStartup { get { return Time.realtimeSinceStartup; } }

		public int frameCount { get { return Time.frameCount; } }

		public uint memoryAllocated { get { return Profiler.GetTotalAllocatedMemory (); } }
	}
}
