#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.AutoTune;

public class AutoTuneSample : MonoBehaviour {

	public GameObject testParticleSystem;
	private bool _experimentStarted = false;

	// Use this for initialization
	void Start ()
	{
		var defaults = new Dictionary<string, object>() {
			{"particlesRate", 100.0f}
		};
		AutoTune.Init("1",  // build id
					  true, // use persistent path
					  defaults, // defaults in case of network error the first time
					  AutoTune.Endpoint.Sandbox); // what endpoint to use
		AutoTune.Fetch(GotSettings);
	}

	public void OnGUI()
	{
		if (_experimentStarted)
		{
			if (GUILayout.Button("End Experiment"))
			{
				AutoTune.GetPerfRecorder().EndExperiment();
				_experimentStarted = false;
			}
		}
		else
		{
			if (GUILayout.Button("Start Experiment"))
			{
				AutoTune.GetPerfRecorder().BeginExperiment("MyExperiment");
				_experimentStarted = true;
			}
		}
	}

	void GotSettings(Dictionary<string,object> settings, int group)
	{
		// apply settings to your game, eg:
		// SettingsManager.maxParticles = settings['max_particles']
		Debug.Log("got new settings");
		/**
		foreach(KeyValuePair<string, object> entry in settings)
		{
			Debug.Log("key: " + entry.Key);
			Debug.Log(entry.Value);
		}
		**/
		var ps = testParticleSystem.GetComponent<ParticleSystem>();
		var em = ps.emission;
		em.rateOverTime = (float)settings["particlesRate"];
	}
	
	// Update is called once per frame
	void Update ()
	{
	}
}
#endif