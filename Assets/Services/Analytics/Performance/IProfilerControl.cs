namespace Unity.Performance
{
	public interface IProfilerControl
	{
		bool supported { get; }

		bool recording { get; }

		void StartRecording (string filePath);

		void StopRecording ();
	}
}