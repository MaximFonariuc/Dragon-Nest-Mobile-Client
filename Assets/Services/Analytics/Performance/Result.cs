using System;
using System.Text;
using UnityEngine;
using UnityEngine.Analytics;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;



namespace Unity.Performance
{
	// VARIANCE will be used when calculating variance and standard deviation
	//	#define VARIANCE

	/// <summary>
	///     A single result from a benchmarking experiment.
	/// </summary>
	[Serializable]
	public class Result
	{
		/// <summary>
		///     The name of the experiment.
		/// </summary>
		public string name;

		#region Frame delta-time values

		/// <summary>
		///     The MS-per-frame values observed during the experiment, packed into bins.
		/// </summary>
		public ValueBin[] deltaTimeMSBins;

		/// <summary>
		///     The maximum observed frametime in MS
		/// </summary>
		public float deltaTimeMSMax;

		/// <summary>
		///     The minimum observed frametime in MS
		/// </summary>
		public float deltaTimeMSMin;

		#if VARIANCE
		/// <summary>
		///     The variance of the observed frametimes
		/// </summary>
		public float deltaTimeMSVariance;

		/// <summary>
		///     The standard deviation of the observed frametimes
		/// </summary>
		public float deltaTimeMSStdDev {
		get { return Mathf.Sqrt (deltaTimeMSVariance); }
		}
		#endif

		#endregion

		#region Memory values

		/// <summary>
		///     Total memory in use at experiment start, in bytes
		/// </summary>
		public uint memoryUsageAtStart;

		/// <summary>
		///     Total memory in use at experiment end, in bytes
		/// </summary>
		public uint memoryUsageAtEnd;

		/// <summary>
		///     The highest memory usage observed during the experiment, in bytes
		/// </summary>
		public uint memoryUsageAtPeak;

		/// <summary>
		/// 	Running total of memory, in bytes
		/// </summary>
		public System.Int64 memoryTotal;

		/// <summary>
		/// 	Average memory consumed during an experiment, in bytes
		/// </summary>
		public double memoryAverage;

		/// <summary>
		///     Delta in total memory allocated between experiment start and experiment end, in bytes
		/// </summary>
		public int memoryUsageDelta {
			get { return (int)(memoryUsageAtEnd - (long)memoryUsageAtStart); }
		}
		
		/// <summary>
		/// 	Amount of system memory present in megabytes
		/// </summary>
		public System.Int64 memorySystem;

		#endregion

		/// <summary>
		///     The number of the frame on which the experiment began.
		/// </summary>
		public int firstFrameNumber;

		/// <summary>
		///     The number of the frame on which the experiment ended.
		/// </summary>
		public int lastFrameNumber;

		/// <summary>
		///     The wall-clock-time-since-app-startup when the experiment began.
		/// </summary>
		public float realtimeAtStart;

		/// <summary>
		///     The wall-clock-time-since-app-startup when the experiment ended.
		/// </summary>
		public float realtimeAtStop;

		/// <summary>
		///     The total number of samples (frames) captured.
		/// </summary>
		public int totalSamples;

		/// <summary>
		/// 	The vertical refresh rate, will be used later to calculated expected
		/// 	number of frames
		/// </summary>
		public int refreshRate;

		/// <summary>
		/// 	Number of frames expected based on refresh rate.
		/// </summary>
		private float expectedFrames;

		/// <summary>
		/// 	An estimate of the number of frames dropped.
		/// </summary>
		private float droppedFrames;

		/// <summary>
		/// 	The target frame time.
		/// </summary>
		private float targetFrameTime;

		/// <summary>
		/// 	The target frame rate of the device.
		/// </summary>
		private int deviceRefreshRate;

		/// <summary>
		/// 	The target frame rate of the application.
		/// </summary>
		private int appRefreshRate;

		/// <summary>
		/// 	List of percentages
		/// </summary>
		private float[] percList;

		public Stopwatch pauseTimer = new Stopwatch ();

		/// <summary>
		/// 	List of upperbounds in string format.
		/// </summary>
		private string[] upperboundList;

		public Result (string experimentName)
		{
			name = experimentName;
			totalSamples = 0;
			firstFrameNumber = Benchmarking.DataSource.frameCount;
			memoryUsageAtStart = Benchmarking.DataSource.memoryAllocated;
			realtimeAtStart = Benchmarking.DataSource.realtimeSinceStartup;
			deltaTimeMSMin = float.MaxValue;
			deltaTimeMSMax = 0;
			populatePercList ();
			populateUpperboundList ();

			#if VARIANCE
			deltaTimeMSVariance = 0;
			#endif

			memoryUsageAtPeak = 0;
			memoryTotal = 0;

			// Convert memorySystem from megabytes to bytes since all other memory
			// related data is in bytes
			memorySystem = SystemInfo.systemMemorySize * (System.Int64)1048576;

			deviceRefreshRate = Screen.currentResolution.refreshRate;
			appRefreshRate = Application.targetFrameRate;

			// If vSyncCount is 2, device refresh rate will be cut in half
			if (QualitySettings.vSyncCount == 2) {
				deviceRefreshRate /= 2;
			}

			// Assuming that deviceRefreshRate and appRefreshRate are both > 0, we will use
			// the smaller of the two refresh rates
			if (deviceRefreshRate <= 0 && appRefreshRate <= 0) {

				refreshRate = 60;
				// Refresh rate will be overwritten if running on iOS to 30.
				setDefaultRefreshRate ();

			} else if (deviceRefreshRate <= 0) {

				refreshRate = appRefreshRate;

			} else if (appRefreshRate <= 0) {

				refreshRate = deviceRefreshRate;
				// Refresh rate will be overwritten if running on iOS to 30.
				setDefaultRefreshRate ();

			} else if (deviceRefreshRate < appRefreshRate) {

				refreshRate = deviceRefreshRate;

			} else { // if appRefreshRate < deviceRefreshRate

				refreshRate = appRefreshRate;

			}

			targetFrameTime = (1f / refreshRate) * 1000f;

			// Creating bins
			var builder = new ValueBinBuilder ();

			builder.AddBin (percList [0] * targetFrameTime);
			for (int i = 0; i < percList.Length - 1; i++) {
				float size = (percList [i + 1] - percList [i]) * targetFrameTime;
				builder.AddBin (size);
			}

			deltaTimeMSBins = builder.Result;

		}

		/// <summary>
		/// 	Sets refresh rate to 30 if running on iOS.
		/// </summary>
		public void setDefaultRefreshRate ()
		{

			#if UNITY_IOS
			refreshRate = 30;
			#endif

		}

		/// <summary>
		///     The total wall-clock time elapsed during the experiment in seconds.
		/// </summary>
		public float totalTime {
			get { return realtimeAtStop - realtimeAtStart - ((float)pauseTimer.ElapsedMilliseconds / 1000); }
		}

		/// <summary>
		/// 	A print method to help with debugging that displays
		/// 	upperbound of each bin, amount in each bin, and log of amount.
		/// </summary>
		public void printHist ()
		{
			foreach (ValueBin bin in deltaTimeMSBins) {
				UnityEngine.Debug.Log ("upperbound: " + bin.v + ", \n\tamount: " + bin.f);
			}

		}

		/// <summary>
		/// 	Sends the fps histogram as a custom event with 10 parameters and
		/// 	sends the dropped frames per minute as a custom event.
		/// </summary>
		public void sendFrameData ()
		{
			Dictionary<string, object> frameData = new Dictionary<string, object> ();

			frameData.Add ("experiment_name", name);
			frameData.Add ("build_version", Benchmarking.buildVersion);
			frameData.Add ("plugin_version", Benchmarking.pluginVersion);
			frameData.Add ("refresh_rate", refreshRate);
			frameData.Add ("experiment_time", totalTime);

			for (int i = 0; i < percList.Length; i++) {
				frameData.Add (upperboundList [i], deltaTimeMSBins [i].f);
			}

			var actualTotalTime = (this.totalTime - (float)pauseTimer.ElapsedMilliseconds);

			expectedFrames = (float)this.refreshRate * actualTotalTime;

			droppedFrames = (this.expectedFrames - this.totalSamples);

			// Diving total time by 60 since there are 60 seconds in a minute.
			float timeRatio = 60 / actualTotalTime;

			if (droppedFrames < 0) {
				UnityEngine.Debug.Log ("Dropped frames is neg");
				droppedFrames = 0;
			} else {
				droppedFrames = droppedFrames * timeRatio;
			}

			frameData.Add ("dropped_frames_per_min", droppedFrames);

			#if MEM
			memoryAverage = memoryTotal / totalSamples;
			frameData.Add ("averageMemory(bytes)", memoryAverage);
			frameData.Add ("peakMemory(bytes)", memoryUsageAtPeak);
			frameData.Add ("systemMemory(bytes)", memorySystem);
			#endif

			//The following parameters are used for testing purposes.
			//frameData.Add ("total_pause_time ", (float)pauseTimer.ElapsedMilliseconds);
			//frameData.Add ("elapsed_edited ", actualTotalTime);

			Analytics.CustomEvent ("perfFrameData", frameData);

		}

		/// <summary>
		/// 	Populates percList with a list of percentages. The percentages are used to decide the
		/// 	granularity of the bins.
		/// </summary>
		public void populatePercList ()
		{

			percList = new float[] { 0.5f, 0.75f, 0.85f, 0.90f, 0.92f, 0.94f, 0.96f, 0.98f, 1f, 1.01f, 1.02f,
				1.03f, 1.04f, 1.05f, 1.06f, 1.07f, 1.08f, 1.09f, 1.1f, 1.15f, 1.2f,
				2f, 4f, 8f, 16f, 32f, (float)Double.PositiveInfinity
			};

		}

		/// <summary>
		/// 	Hard coded array of string representations of each bin's upperbound percentage.
		/// </summary>
		public void populateUpperboundList ()
		{
			upperboundList = new string[] {"upper_bound: 0.5", "upper_bound: 0.75", "upper_bound: 0.85", "upper_bound: 0.90",
				"upper_bound: 0.92", "upper_bound: 0.94", "upper_bound: 0.96", "upper_bound: 0.98", "upper_bound: 1",
				"upper_bound: 1.01", "upper_bound: 1.02", "upper_bound: 1.03", "upper_bound: 1.04", "upper_bound: 1.05",
				"upper_bound: 1.06", "upper_bound: 1.07", "upper_bound: 1.08", "upper_bound: 1.09", "upper_bound: 1.1",
				"upper_bound: 1.15", "upper_bound: 1.2", "upper_bound: 2", "upper_bound: 4", "upper_bound: 8", "upper_bound: 16",
				"upper_bound: 32", "upper_bound: Infinity"
			};
		}

	}
}
