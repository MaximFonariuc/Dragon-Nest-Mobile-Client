using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.SceneManagement;
using Stopwatch = System.Diagnostics.Stopwatch;

// Add PERF_DEV_PREVIEW to Scripting Define Symbols in Player Settings to enable preview features.
// Add PERF_DEBUG_VERBOSE to Scripting Define Symbols in Player Settings to enable verbose debug logging.

namespace Unity.Performance
{
	/// <summary>
	/// The main Benchmarking API for controlling experiment data recording. Use this from your own code directly, or use
	/// the BenchmarkRecorder component to drive it for you.
	/// </summary>
	public class Benchmarking : MonoBehaviour
	{
		static readonly string k_PluginVersion = "v1.1.1";

		static Benchmarking _instance;
		static bool _applicationIsQuitting;

		[RuntimeInitializeOnLoadMethod]
		static Benchmarking GetInstance ()
		{
			if (_applicationIsQuitting) return null;

			if (_instance == null)
			{
				_instance = FindObjectOfType<Benchmarking>();

				if (_instance == null)
				{
					var gO = new GameObject("Benchmarking");
					_instance = gO.AddComponent<Benchmarking>();
					gO.hideFlags = HideFlags.HideAndDontSave;
				}

				DontDestroyOnLoad(_instance.gameObject);
			}

			return _instance;
		}

		void Awake ()
		{
			if (!GetInstance().Equals(this)) Destroy(gameObject);
		}

		void OnApplicationQuit ()
		{
			_applicationIsQuitting = true;
		}

		private static Result _activeExperiment;
		private static float _deltaTimeMSSum;
		private static float _deltaTimeMSSqrSum;
		private static int _lastFrameRecorded;

		// We currently only allow for one experiment at a time. Keeping List structure in case we need to implement
		// multiple experiments in the future.
		private static Dictionary<string, Result> _results = new Dictionary<string, Result> ();

		static readonly Dictionary<string, Stopwatch> m_GenericTimers = new Dictionary<string, Stopwatch>();

		/// <summary>
		/// The name of the experiment.
		/// </summary>
		private static string _experimentName;

#if !PERF_DEV_PREVIEW
		static string m_ActiveTimerContext;
#endif

		/// <summary>
		/// The data source to use. By default, use an implementation that just talks to Unity's APIs.
		/// This is pluggable so that the unit tests can swap it out for a dummy implementation.
		/// </summary>
		public static IDataSource DataSource = new DefaultDataSource ();

#if PERF_DEV_PREVIEW
		public 
#endif
		static IProfilerControl ProfilerControl = new DefaultProfilerControl ();

#if PERF_DEV_PREVIEW
		/// <summary>
		/// If set, perform a full profiler capture for every experiment and save it to the given path.
		/// </summary>
		public 
#endif
		static string profilerCapturePath { get; set; }

		/// <summary>
		/// Gets or sets the build version.
		/// </summary>
		/// <value>The build version.</value>
		public static string buildVersion { get; set; }

		/// <summary>
		/// Gets the plugin version.
		/// </summary>
		/// <value>The plugin version.</value>
		public static string pluginVersion
		{
			get { return k_PluginVersion; }
		}

		/// <summary>
		/// Gets the build version.
		/// </summary>
		/// <returns>The build version.</returns>
		[Obsolete("Use pluginVersion propery instead.")]
		public static string getBuildVersion ()
		{
			return buildVersion;
		}

		/// <summary>
		/// Setter for build version.
		/// </summary>
		/// <param name="newBuildVersion">Build version.</param>
		[Obsolete("Use SetBuildVersion(string) or buildVersion property instead.")]
		public static void setBuildVersion (string newBuildVersion)
		{
			buildVersion = newBuildVersion;
		}

		public static void SetBuildVersion (string buildVersion)
		{
			Benchmarking.buildVersion = buildVersion;
		}

#if PERF_DEV_PREVIEW
		/// <summary>
		/// Gets the names of all the active experiments.
		/// </summary>
		/// <returns>The experiment name.</returns>
		public
#endif
		static IEnumerable<string> getExperimentNames ()
		{
			return _results.Keys;
		}

#if PERF_DEV_PREVIEW
		/// <summary>
		/// Retrieve the results of all experiments performed so far.
		/// </summary>
		public 
#endif
		static Dictionary<string, Result> Results {
			get { return _results; }
		}

		/// <summary>
		/// Begin recording data for an experiment.
		/// </summary>
		/// <param name="name">The name of the experiment shown in the results.</param>
		/// <returns>The newly initialized experiment result, for further configuration (e.g. bins for values)</returns>
		public static Result BeginExperiment (string experimentName)
		{
			if (GetInstance() == null) return null;

			if (string.IsNullOrEmpty(experimentName))
			{
				Debug.LogError("Unable to start experiment. Name cannot be null or empty.");
				return null;
			}
#if DEBUG
            Debug.Log ("Beginning Experiment: " + experimentName);
#endif

#if !PERF_DEV_PREVIEW
            if (_results.Count > 0)
			{
				EndExperiment();
				Finished();
			}

			_experimentName = experimentName;
#endif

			if (_results.ContainsKey(experimentName)) {
				EndExperiment(experimentName);
				Finished(experimentName);
			}

			_results[experimentName] = new Result(experimentName);

			_deltaTimeMSSum = 0;
			_deltaTimeMSSqrSum = 0;

			_lastFrameRecorded = DataSource.frameCount;

			if (!string.IsNullOrEmpty (profilerCapturePath) && ProfilerControl != null && ProfilerControl.supported)
				ProfilerControl.StartRecording (profilerCapturePath + experimentName);

			return _results[experimentName];
		}

		/// <summary>
		/// End recording data for the current experiment.
		/// </summary>
#if PERF_DEV_PREVIEW
		[Obsolete("Use EndExperiment(string) instead.")]
#endif
		public static Result EndExperiment ()
		{
#if PERF_DEV_PREVIEW
			foreach (string key in _results.Keys)
			{
				EndExperiment(key);
			}

			return null;
#else
			if (_results.Count == 0) return null;

			return EndExperiment(_experimentName);
#endif
		}

#if PERF_DEV_PREVIEW
		/// <summary>
		/// End recording data for the current experiment.
		/// </summary>
		public 
#endif
		static Result EndExperiment (string experimentName)
		{
			if (string.IsNullOrEmpty(experimentName))
			{
				Debug.LogError("Unable to end experiment. Name cannot be null or empty.");
				return null;
			}

			if (!_results.ContainsKey(experimentName))
			{
				Debug.LogErrorFormat("Unable to end experiment. Experiment with name '{0}' not found.", experimentName);
			}
#if DEBUG
            Debug.Log("Ending Experiment: " + experimentName);
#endif
            var xpm = _results[experimentName];
			
			xpm.lastFrameNumber = DataSource.frameCount;
			xpm.realtimeAtStop = DataSource.realtimeSinceStartup;
			xpm.memoryUsageAtEnd = DataSource.memoryAllocated;

			if (ProfilerControl != null && ProfilerControl.recording)
				ProfilerControl.StopRecording ();

#if !PERF_DEV_PREVIEW
			_experimentName = null;
#endif

			return xpm;
		}

		/// <summary>
		/// Indicate that all experiments have now finished being run.
		/// </summary>
		public static void Finished ()
		{
			foreach (Result value in _results.Values)
			{
				value.sendFrameData();
			}

			_results.Clear();
		}

#if PERF_DEV_PREVIEW
		public 
#endif
		static void Finished (string experimentName)
		{
			_results[experimentName].sendFrameData ();
			_results.Remove(experimentName);
		}

		/// <summary>
		/// Loads the scene, and reports the time in milliseconds that it takes to load.
		/// </summary>
		/// <param name="sceneBuildIndex">Scene index in Build Settings.</param>
		public static void LoadScene (int sceneBuildIndex)
		{
			LoadScene(SceneManager.GetSceneAt(sceneBuildIndex).name);
		}

		/// <summary>
		/// Loads the scene, and reports the time in milliseconds that it takes to load.
		/// </summary>
		/// <param name="sceneBuildIndex">Scene index in Build Settings.</param>
		/// <param name="mode">Load scene mode.</param>
		public static void LoadScene (int sceneBuildIndex, LoadSceneMode mode)
		{
			LoadScene(SceneManager.GetSceneAt(sceneBuildIndex).name, mode);
		}

		/// <summary>
		/// Loads the scene asynchronously, and reports the time in milliseconds that it takes to load.
		/// </summary>
		/// <returns>The AsyncOperation.</returns>
		/// <param name="sceneBuildIndex">Scene index in Build Settings.</param>
		public static AsyncOperation LoadSceneAsync (int sceneBuildIndex)
		{
			return LoadSceneAsync(SceneManager.GetSceneAt(sceneBuildIndex).name);
		}

		/// <summary>
		/// Loads the scene asynchronously, and reports the time in milliseconds that it takes to load.
		/// </summary>
		/// <returns>The AsyncOperation.</returns>
		/// <param name="sceneBuildIndex">Scene index in Build Settings.</param>
		/// <param name="mode">Load scene mode.</param>
		public static AsyncOperation LoadSceneAsync (int sceneBuildIndex, LoadSceneMode mode)
		{
			return LoadSceneAsync(SceneManager.GetSceneAt(sceneBuildIndex).name, mode);
		}

		/// <summary>
		/// Loads the scene, and reports the time in milliseconds that it takes to load.
		/// </summary>
		/// <param name="sceneName">Scene name.</param>
		public static void LoadScene (string sceneName)
		{
			GetInstance().DoLoadScene(sceneName, LoadSceneMode.Single);
		}

		/// <summary>
		/// Loads the scene, and reports the time in milliseconds that it takes to load.
		/// </summary>
		/// <param name="sceneName">Scene name.</param>
		/// <param name="mode">Load scene mode.</param>
		public static void LoadScene (string sceneName, LoadSceneMode mode)
		{
			GetInstance().DoLoadScene(sceneName, mode);
		}

		/// <summary>
		/// Loads the scene asynchronously, and reports the time in milliseconds that it takes to load.
		/// </summary>
		/// <returns>The AsyncOperation.</returns>
		/// <param name="sceneName">Scene name.</param>
		public static AsyncOperation LoadSceneAsync (string sceneName)
		{
			return GetInstance().DoLoadSceneAsync(sceneName, LoadSceneMode.Single);
		}

		/// <summary>
		/// Loads the scene asynchronously, and reports the time in milliseconds that it takes to load.
		/// </summary>
		/// <returns>The AsyncOperation.</returns>
		/// <param name="sceneName">Scene name.</param>
		/// <param name="mode">Load scene mode.</param>
		public static AsyncOperation LoadSceneAsync (string sceneName, LoadSceneMode mode)
		{
			return GetInstance().DoLoadSceneAsync(sceneName, mode);
		}

		void DoLoadScene (string sceneName, LoadSceneMode mode)
		{
			var stopWatch = new Stopwatch();
			uint memAllocAtStart = DataSource.memoryAllocated;

			stopWatch.Start();

			SceneManager.LoadScene(sceneName, mode);

			OnSceneLoaded(sceneName, stopWatch, memAllocAtStart);
		}

		AsyncOperation DoLoadSceneAsync (string sceneName, LoadSceneMode mode)
		{
			Scene scene = SceneManager.GetSceneByName(sceneName);
			AsyncOperation asyncOp = null;

			var stopWatch = new Stopwatch();
			uint memAllocAtStart = DataSource.memoryAllocated;

			stopWatch.Start();

			asyncOp = SceneManager.LoadSceneAsync(sceneName, mode);

			StartCoroutine(WaitWhileLoadingScene(asyncOp, sceneName, stopWatch, memAllocAtStart));

			return asyncOp;
		}

		IEnumerator WaitWhileLoadingScene (AsyncOperation asyncOp, string sceneName, Stopwatch stopWatch, uint memAllocAtStart)
		{
			yield return asyncOp;

			OnSceneLoaded(sceneName, stopWatch, memAllocAtStart);
		}

		void OnSceneLoaded (string sceneName, Stopwatch stopWatch, uint memAllocAtStart)
		{
			stopWatch.Stop();

			double loadTimeInMilliseconds = stopWatch.Elapsed.TotalMilliseconds;
			long memoryDeltaInBytes = (long)DataSource.memoryAllocated - memAllocAtStart;
#if DEBUG
            if (Debug.isDebugBuild)
			{
				if (loadTimeInMilliseconds > float.MaxValue) Debug.LogWarning("Load time exceeded the measurable range.");
				if (memoryDeltaInBytes > int.MaxValue) Debug.LogWarning("Memory delta exceeded the measurable range.");
			}
#endif

            AnalyticsResult eventResult = SceneLoadEvent(sceneName, (float)loadTimeInMilliseconds, (int)memoryDeltaInBytes);
#if DEBUG
            if (Debug.isDebugBuild) Debug.LogFormat(
				"Scene '{0}' loaded in {1} milliseconds ({2}). Memory delta between scenes: {3} bytes",
				sceneName,
				loadTimeInMilliseconds,
				eventResult,
				memoryDeltaInBytes
			);
#endif
        }

		/// <summary>
		/// Starts a generic timer with the specified context.
		/// </summary>
		/// <param name="context">Context.</param>
		public static void StartTimer (string context)
		{
			if (_applicationIsQuitting) return;

			if (string.IsNullOrEmpty(context))
			{
				Debug.LogError("Unable to start timer. Context cannot be null or empty.");
				return;
			}

#if !PERF_DEV_PREVIEW
			if (m_GenericTimers.Count > 0) StopTimer();

			m_ActiveTimerContext = context;
#endif

			if (m_GenericTimers.ContainsKey(context))
			{
				string log = null;

				StopTimer(context, m_GenericTimers[context], out log);

				m_GenericTimers[context].Reset();

				if (Debug.isDebugBuild) Debug.Log(log);
			}
			else
			{
				var timer = new Stopwatch();

				m_GenericTimers.Add(context, timer);
			}

			m_GenericTimers[context].Start();
#if DEBUG
            if (Debug.isDebugBuild) Debug.LogFormat("Timer with context '{0}' started.", context);
#endif
        }

		/// <summary>
		/// Stops the generic timer.
		/// </summary>
		/// <returns>The timer.</returns>
#if PERF_DEV_PREVIEW
		[Obsolete("Use StopTimer(string) or StopAllTimers() instead")]
#endif
		public static float StopTimer ()
		{
#if PERF_DEV_PREVIEW
			StopAllTimers();

			return 0f;
#else
			if (m_GenericTimers.Count == 0) return -1f;

			return StopTimer(m_ActiveTimerContext);
#endif
		}

#if PERF_DEV_PREVIEW
		/// <summary>
		/// Stops the generic timer matching the specified context.
		/// </summary>
		/// <returns>The time elapsed in milliseconds (returns -1 if an error occurred).</returns>
		/// <param name="context">Context.</param>
		public 
#endif
		static float StopTimer (string context)
		{
			float timeInMilliseconds = -1f;

			if (_applicationIsQuitting) return timeInMilliseconds;

			if (string.IsNullOrEmpty(context))
			{
				Debug.LogError("Unable to start timer. Context cannot be null or empty.");
				return timeInMilliseconds;
			}

			if (!m_GenericTimers.ContainsKey(context))
			{
				Debug.LogErrorFormat("Unable to stop timer. Timer with context '{0}' not found.", context);
				return timeInMilliseconds;
			}

			string log = null;

			timeInMilliseconds = StopTimer(context, m_GenericTimers[context], out log);

			m_GenericTimers.Remove(context);

			if (Debug.isDebugBuild) Debug.Log(log);

			return timeInMilliseconds;
		}

#if PERF_DEV_PREVIEW
		/// <summary>
		/// Stops all generic timers.
		/// </summary>
		public 
#endif
		static void StopAllTimers ()
		{
			if (_applicationIsQuitting || m_GenericTimers.Count == 0) return;

			string message = string.Empty;
#if DEBUG
            if (Debug.isDebugBuild)
			{
				message = string.Format("Stopping {0} active timers...", m_GenericTimers.Count);
			}
#endif
            foreach (KeyValuePair<string, Stopwatch> timer in m_GenericTimers)
			{
				string log = null;

				StopTimer(timer.Key, timer.Value, out log);

				if (Debug.isDebugBuild) message += string.Concat("\n  ", log);
			}

			m_GenericTimers.Clear();

			if (Debug.isDebugBuild) Debug.Log(message);
		}

		static float StopTimer (string context, Stopwatch timer, out string log)
		{
			log = null;

			timer.Stop();

			double timeInMilliseconds = timer.Elapsed.TotalMilliseconds;
#if DEBUG
            if (Debug.isDebugBuild && timeInMilliseconds > float.MaxValue)
			{
				Debug.LogWarning("The timer exceeded the measurable range.");
			}
#endif
            AnalyticsResult eventResult = GenericTimerEvent(context, (float)timeInMilliseconds);
#if DEBUG
            if (Debug.isDebugBuild) log = string.Format(
				"Timer with context '{0}' stopped ({1}). Time elapsed in milliseconds: {2}",
				context,
				eventResult,
				timeInMilliseconds
			);
#endif
            return (float)timeInMilliseconds;
		}

#if PERF_DEV_PREVIEW
		/// <summary>
		/// Clear all results from past experiments.
		/// </summary>
		public 
#endif
		static void Clear ()
		{
			_results.Clear();
		}

		private static void LogWarning (string format, params object[] args)
		{
			Debug.LogWarningFormat (format, args);
		}

		void OnApplicationPause (bool paused)
		{
			foreach (Result result in _results.Values)
			{
				if (paused) result.pauseTimer.Start();
				else result.pauseTimer.Stop();
			}

			foreach (Stopwatch timer in m_GenericTimers.Values)
			{
				if (paused) timer.Stop();
				else timer.Start();
			}
		}

		void Update ()
		{
			if (DataSource.frameCount == _lastFrameRecorded) // Avoid multiple-counting things
				return;
#if DEBUG
            if (DataSource.frameCount != _lastFrameRecorded + 1)
				LogWarning ("{0} missing frames detected. The experiment results will be inaccurate. Please check that you are calling Benchmarking.Update every frame!", DataSource.frameCount - _lastFrameRecorded - 1);
#endif
            _lastFrameRecorded = DataSource.frameCount;
			var deltaTimeMS = DataSource.unscaledDeltaTimeSeconds * 1000f;
            var it = _results.GetEnumerator();
            while(it.MoveNext())
            {
                Result r = it.Current.Value;				
				if (deltaTimeMS < r.deltaTimeMSMin)
					r.deltaTimeMSMin = deltaTimeMS;
				if (deltaTimeMS > r.deltaTimeMSMax)
					r.deltaTimeMSMax = deltaTimeMS;
				
				++r.totalSamples;
				_deltaTimeMSSum += deltaTimeMS;
				_deltaTimeMSSqrSum += (deltaTimeMS * deltaTimeMS);
				
				// Calculate the running variance
				// http://stackoverflow.com/questions/5543651/computing-standard-deviation-in-a-stream
				
				#if NEW_VAR
				if (r.totalSamples > 1) {
					r.deltaTimeMSVariance = (r.totalSamples * _deltaTimeMSSqrSum -
											 _deltaTimeMSSum * _deltaTimeMSSum) /
						(r.totalSamples * (r.totalSamples - 1));
				}
				#endif
				
				// Bin the value for median/percentile calculations

				r.deltaTimeMSBins.AddValue (deltaTimeMS);
				uint memory = DataSource.memoryAllocated;
				r.memoryTotal += memory;
				if (memory > r.memoryUsageAtPeak)
					r.memoryUsageAtPeak = memory;
				
			}
		}

		static AnalyticsResult CustomEvent (string name, IDictionary<string, object> data)
		{
			data.Add("build_version", buildVersion);
			data.Add("plugin_version", pluginVersion);

			AnalyticsResult eventResult = Analytics.CustomEvent(name, data);

#if PERF_DEBUG_VERBOSE
			string message = string.Format(
				"Submit '{0}' event: {1}\n" +
				"Number of entries: {2}",
				name,
				eventResult,
				data.Count
			);

			foreach (KeyValuePair<string, object> entry in data)
			{
				message += string.Format("\n  {0} = {1}", entry.Key, entry.Value);
			}

			Debug.Log(message);
#endif

			if (eventResult != AnalyticsResult.Ok)
			{
				Debug.LogErrorFormat(
					"An error occured when attempting to submit custom event '{0}' ({1}).",
					name,
					eventResult
				);
			}

			return eventResult;
		}

		static AnalyticsResult GenericTimerEvent (string context, float timeInMilliseconds)
		{
			return CustomEvent(
				"perfGenericTimer",
				new Dictionary<string, object> {
					{ "context", context },
					{ "time_elapsed", timeInMilliseconds },
				}
			);
		}

		static AnalyticsResult SceneLoadEvent (string sceneName, float loadTimeInMilliseconds, int memoryDeltaInBytes)
		{
			return CustomEvent(
				"perfSceneLoad",
				new Dictionary<string, object> {
					{ "scene_name", sceneName },
					{ "load_time", loadTimeInMilliseconds },
					{ "memory_delta_between_loads(bytes)", memoryDeltaInBytes },
				}
			);
		}
	}
}
