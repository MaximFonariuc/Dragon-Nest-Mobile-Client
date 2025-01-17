namespace Unity.Performance
{
	public interface IDataSource
	{
		float unscaledDeltaTimeSeconds { get; }

		float realtimeSinceStartup { get; }

		int frameCount { get; }

		uint memoryAllocated { get; }
	}
}