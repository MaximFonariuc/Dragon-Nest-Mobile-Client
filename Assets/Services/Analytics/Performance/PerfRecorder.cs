using System;

using UnityEngine;
using UnityEngine.SceneManagement;

// Add PERF_DEV_PREVIEW to Scripting Define Symbols in Player Settings to enable preview features.

namespace Unity.Performance
{

	/// <summary>
	/// Driver class to be called to begin and end experiements as well as
	/// record scene load times.
	/// </summary>
	public class PerfRecorder : MonoBehaviour
	{
		/// <summary>
		/// Gets the plugin version.
		/// </summary>
		/// <value>The plugin version.</value>
		public string pluginVersion
		{
			get { return Benchmarking.pluginVersion; }
		}
		
		/// <summary>
		/// Gets or sets the build version. Only needs to be set once after application start.
		/// </summary>
		/// <value>The build version.</value>
		public string buildVersion
		{
			get { return Benchmarking.buildVersion; }
			set { Benchmarking.buildVersion = value; }
		}

		/// <summary>
		/// Sets the build version. Only needs to be set once after application start.
		/// </summary>
		/// <param name="buildVersion">Build version.</param>
		public void SetBuildVersion (string buildVersion)
		{
			Benchmarking.buildVersion = buildVersion;
		}

		/// <summary>
		/// Begins the experiment that collects estimated dropped frames as well
		/// as a histogram of miliseconds per frame.
		/// </summary>
		/// <param name="experimentName">Experiment name.</param>
		public void BeginExperiment (string experimentName)
		{
			Benchmarking.BeginExperiment (experimentName);
		}

#if PERF_DEV_PREVIEW
		[Obsolete("Use EndExperiment(string) instead.")]
#endif
		public void EndExperiment ()
		{
			Benchmarking.EndExperiment();
			Benchmarking.Finished();
		}

#if PERF_DEV_PREVIEW
		/// <summary>
		/// Ends the experiment that collects estimated dropped frames as well
		/// as a histogram of miliseconds per frame.
		/// </summary>
		public void EndExperiment (string experimentName)
		{
			Benchmarking.EndExperiment (experimentName);
			Benchmarking.Finished (experimentName);
		}
#endif

		/// <summary>
		/// Loads the scene, and reports the time in milliseconds that it takes to load.
		/// </summary>
		/// <param name="sceneBuildIndex">Scene index in Build Settings.</param>
		public void LoadScene (int sceneBuildIndex)
		{
			Benchmarking.LoadScene(sceneBuildIndex);
		}

		/// <summary>
		/// Loads the scene, and reports the time in milliseconds that it takes to load.
		/// </summary>
		/// <param name="sceneBuildIndex">Scene index in Build Settings.</param>
		/// <param name="mode">Load scene mode.</param>
		public void LoadScene (int sceneBuildIndex, LoadSceneMode mode)
		{
			Benchmarking.LoadScene(sceneBuildIndex);
		}

		/// <summary>
		/// Loads the scene, and reports the time in milliseconds that it takes to load.
		/// </summary>
		/// <param name="sceneName">Scene name.</param>
		public void LoadScene (string sceneName)
		{
			Benchmarking.LoadScene(sceneName);
		}

		/// <summary>
		/// Loads the scene, and reports the time in milliseconds that it takes to load.
		/// </summary>
		/// <param name="sceneName">Scene name.</param>
		/// <param name="mode">Load scene mode.</param>
		public void LoadScene (string sceneName, LoadSceneMode mode)
		{
			Benchmarking.LoadScene(sceneName, mode);
		}

		/// <summary>
		/// Loads the scene asynchronously, and reports the time in milliseconds that it takes to load.
		/// </summary>
		/// <returns>The AsyncOperation.</returns>
		/// <param name="sceneBuildIndex">Scene index in Build Settings.</param>
		public AsyncOperation LoadSceneAsync (int sceneBuildIndex)
		{
			return Benchmarking.LoadSceneAsync(sceneBuildIndex);
		}

		/// <summary>
		/// Loads the scene asynchronously, and reports the time in milliseconds that it takes to load.
		/// </summary>
		/// <returns>The AsyncOperation.</returns>
		/// <param name="sceneBuildIndex">Scene index in Build Settings.</param>
		/// <param name="mode">Load scene mode.</param>
		public AsyncOperation LoadSceneAsync (int sceneBuildIndex, LoadSceneMode mode)
		{
			return Benchmarking.LoadSceneAsync(sceneBuildIndex, mode);
		}

		/// <summary>
		/// Loads the scene asynchronously, and reports the time in milliseconds that it takes to load.
		/// </summary>
		/// <returns>The AsyncOperation.</returns>
		/// <param name="sceneName">Scene name.</param>
		public AsyncOperation LoadSceneAsync (string sceneName)
		{
			return Benchmarking.LoadSceneAsync(sceneName);
		}

		/// <summary>
		/// Loads the scene asynchronously, and reports the time in milliseconds that it takes to load.
		/// </summary>
		/// <returns>The AsyncOperation.</returns>
		/// <param name="sceneName">Scene name.</param>
		/// <param name="mode">Load scene mode.</param>
		public AsyncOperation LoadSceneAsync (string sceneName, LoadSceneMode mode)
		{
			return Benchmarking.LoadSceneAsync(sceneName, mode);
		}

		/// <summary>
		/// Starts the generic timer.
		/// </summary>
		/// <param name="context">Context.</param>
		public void StartTimer (string context)
		{
			Benchmarking.StartTimer(context);
		}

#if PERF_DEV_PREVIEW
		[Obsolete("Use StopTimer(string) or StopAllTimers() instead.")]
#endif
		public float StopTimer ()
		{
			return Benchmarking.StopTimer();
		}

#if PERF_DEV_PREVIEW
		/// <summary>
		/// Stops the generic timer.
		/// </summary>
		/// <returns>The time elapsed in milliseconds.</returns>
		/// <param name="context">Context.</param>
		public float StopTimer (string context)
		{
			return Benchmarking.StopTimer(context);
		}

		/// <summary>
		/// Stops all generic timers.
		/// </summary>
		public void StopAllTimers ()
		{
			Benchmarking.StopAllTimers();
		}

		public void ClearAllResults ()
		{
			Benchmarking.Clear ();
		}
#endif
	}

}
