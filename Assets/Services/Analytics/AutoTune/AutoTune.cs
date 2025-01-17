using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Analytics;
using Unity.Performance;
using MiniJSON;
using System.Net;
using System.IO;
using System;

namespace Unity.AutoTune {

public class AutoTune : MonoBehaviour {
	private static string SandboxEndpoint = "https://test-auto-tune.uca.cloud.unity3d.com";
	private static string ProductionEndpoint = "https://prd-auto-tune.uca.cloud.unity3d.com";
	private static string CLIENT_DEFAULT_SEGMENT = "-1";
	private static int CLIENT_DEFAULT_GROUP = -1;
	private static string AutoTuneDir = "/unity.autotune";
	private static string SegmentConfigCacheFilePath = AutoTuneDir + "/segmentconfig.json";

	public enum Endpoint
	{
		Sandbox,
		Production
	};

	/// <summary>
	/// This is a custom version of DeviceInfo that we will use to get a segment
	/// for this particular device.
	/// </summary>
	private class DeviceInfo {
		public string model;
		public int    ram;
		public string cpu;
		public string gfx_name;
		public string gfx_vendor;
		public string device_id;
		public int    cpu_count;
		public float  dpi;
		public string screen;
		public string project_id;
		public int platform_id;
		public string os_ver;
		public int gfx_shader;
		public string gfx_ver;
		public int max_texture_size;
		public string app_build_version;
		public bool in_editor;

		public DeviceInfo(string projectId, string app_build_version) {
			this.project_id = projectId;
			this.app_build_version = app_build_version;
			this.model = GetDeviceModel();
			this.device_id = SystemInfo.deviceUniqueIdentifier;
			this.ram = SystemInfo.systemMemorySize;
			this.cpu = SystemInfo.processorType;
			this.cpu_count = SystemInfo.processorCount;
			this.gfx_name = SystemInfo.graphicsDeviceName;
			this.gfx_vendor = SystemInfo.graphicsDeviceVendor;
			this.screen = Screen.currentResolution.ToString();
			this.dpi = Screen.dpi;
			this.in_editor = false;
			if (Application.isEditor) {
                    // when running on editor, always send android
                    //Debug.Log ("*** running in editor: Will send platform as Android");
#if UNITY_ANDROID
                    this.platform_id = (int)RuntimePlatform.Android;
#else
                    this.platform_id = (int) RuntimePlatform.IPhonePlayer;
#endif

                    this.in_editor = true;
			} else {
				this.platform_id = (int) Application.platform;
			}
			this.os_ver = SystemInfo.operatingSystem;
			this.gfx_shader = SystemInfo.graphicsShaderLevel;
			this.gfx_ver = SystemInfo.graphicsDeviceVersion;
			this.max_texture_size = SystemInfo.maxTextureSize;
		}

		private string GetDeviceModel()
		{
#if UNITY_ANDROID && !UNITY_EDITOR
			// get manufacturer/model/device
			AndroidJavaClass jc = new AndroidJavaClass("android.os.Build");
			string manufacturer = jc.GetStatic<string>("MANUFACTURER");
			string model = jc.GetStatic<string>("MODEL");
			string device = jc.GetStatic<string>("DEVICE");
			return String.Format("{0}/{1}/{2}", manufacturer, model, device);
#else
			return SystemInfo.deviceModel;
#endif
		}
	}

	private static AutoTune _instance;
	private static PerfRecorder _prInstance;

	[RuntimeInitializeOnLoadMethod]
	static AutoTune GetInstance()
	{
		if (_instance == null)
		{
			_instance = FindObjectOfType<AutoTune>();
			if (_instance == null)
			{
				var gO = new GameObject("AutoTune");
				_instance = gO.AddComponent<AutoTune>();
				_prInstance = gO.AddComponent<PerfRecorder>();
				gO.hideFlags = HideFlags.HideAndDontSave;
			}
			DontDestroyOnLoad(_instance.gameObject);
		}
		return _instance;
	}

	void Awake()
	{
		if (!GetInstance().Equals(this)) Destroy(gameObject);
	}
	
	public delegate void AutoTuneCallback(Dictionary<string, object> settings, int group);
	private string _projectId;
	private string _buildVersion;
	private SegmentConfig _clientDefaultConfig;
	private string _storePath;
	private Endpoint _endpoint;

	// once initialized, the _cachedSegmentConfig is never null
	private SegmentConfig _cachedSegmentConfig;
	private bool _isPlayerOverride = false;

	// request state
	private bool _updateNeeded = false;
	private bool _isError = false;
	private long _startTime = 0;
	private long _requestTime = 0;
	private float _fetchInGameTime = 0;
	private AutoTuneCallback _callback;
	private DeviceInfo _deviceInfo = null;


	/// <summary>
	/// Setups the AutoTune API. Make sure you call this before
	/// anything.
	/// <param name="buildVersion">The build version.</param>
	/// <param name="usePersistentPath">True to use persistentDataPath for storing segment config, otherwise temporaryCachePath would be use.</param>
	/// <param name="defaultValues">The default values that will be used in case of network error.</param>
	/// <param name="endpoint">What endpoint to use. Defaults to Sandbox environment.</param>
	/// </summary>
	public static void Init(string buildVersion,
							bool usePersistentPath,
							Dictionary<string, object> defaultValues,
							Endpoint endpoint = Endpoint.Sandbox
							)
	{
		if (String.IsNullOrEmpty(Application.cloudProjectId))
		{
			Debug.LogError("You must enable Analytics to be able to use AutoTune");
		}
		GetInstance ()._projectId = Application.cloudProjectId;
		GetInstance ()._clientDefaultConfig = new SegmentConfig(CLIENT_DEFAULT_SEGMENT, CLIENT_DEFAULT_GROUP, defaultValues, "client_default");

		// Application.persistentDataPath can only be called from the main thread so we cache it in init
		GetInstance ()._storePath = usePersistentPath ? Application.persistentDataPath : Application.temporaryCachePath;
		GetInstance ()._buildVersion = buildVersion;
		GetInstance ()._endpoint = endpoint;

		// load cache segment config last after all other variables has been set
		GetInstance ().LoadCacheSegmentConfig();
		_prInstance.SetBuildVersion(buildVersion);
		System.Net.ServicePointManager.ServerCertificateValidationCallback += (o, certificate, chain, errors) => true;
	}

	/// <summary>
	/// Start AutoTune: fetch new settings for this specific device.
	/// <param name="callback">The callback that will get executed when new settings are received and parsed</param>
	/// </summary>
	public static void Fetch(AutoTuneCallback callback)
	{
		GetInstance ().CleanUp();
		GetInstance ()._callback = callback;
		GetInstance ()._startTime = AutoTune.getCurrentTimestamp();
		GetInstance ()._fetchInGameTime = Time.time;
		GetInstance ().TryFetch();
	}

	/// <summary>
	/// Gets the PerfRecorder instance
	/// </summary>
	public static PerfRecorder GetPerfRecorder()
	{
		return _prInstance;
	}

	/// <summary>
	/// returns an int from the fetched settings. If no settings have
	/// been fetched, it will use the cached version or the developer
	/// defaults if there is no cache.
	/// </summary>
	public static int GetInt(string key, int defValue = 0)
	{
		return GetValue<int>(key, defValue);
	}

	/// <summary>
	/// returns a float from the fetched settings. If no settings have
	/// been fetched, it will use the cached version or the developer
	/// defaults if there is no cache.
	/// </summary>
	public static float GetFloat(string key, float defValue = 0.0f)
	{
		return GetValue<float>(key, defValue);
	}

	/// <summary>
	/// returns a string from the fetched settings. If no settings
	/// have been fetched, it will use the cached version or the
	/// developer defaults if there is no cache.
	/// </summary>
	public static string GetString(string key, string defValue = "")
	{
		return GetValue<string>(key, defValue);
	}

	/// <summary>
	/// returns a bool from the fetched settings. If no settings have
	/// been fetched, it will use the cached version or the developer
	/// defaults if there is no cache.
	/// </summary>
	public static bool GetBool(string key, bool defValue = false)
	{
		return GetValue<bool>(key, defValue);
	}

	public static T GetValue<T>(string key, T defValue)
	{
		SegmentConfig sc = GetInstance ()._cachedSegmentConfig;
		try {
			if (sc != null && sc.settings.ContainsKey(key))
			{
				return (T)sc.settings[key];
			}
		} catch (Exception e) {
			Debug.LogError(e);
		}
		return defValue;
	}

	/// <summary>
	/// API to auto-tune that the player is using manual override of setting.
	/// This information allows us to differentiate devices that do not use
	/// auto-tune settings.
	/// </summary>
	public static void SetPlayerOverride(bool isPlayerOverride) 
	{
		GetInstance ()._isPlayerOverride = isPlayerOverride;
	}

	public void Update()
	{
		if (_updateNeeded == true && _callback != null)
		{
			var segmentConfig = _cachedSegmentConfig;
			var deviceInfo = _deviceInfo;
			try
			{
				var callbackTime = AutoTune.getCurrentTimestamp() - _startTime;
#if DEBUG
                    Debug.LogFormat ("autotune callback time: {0}", callbackTime);
#endif
                    var callbackInGameTime = Time.time;
				_callback(segmentConfig.settings, segmentConfig.group_id);
                                
				// should not happen but do not want to write null checks in code below this
				if (deviceInfo == null) {
					deviceInfo = new DeviceInfo(_projectId, _buildVersion);
				}

				// device information data should reuse the same naming convention as DeviceInfo event
				var status = Analytics.CustomEvent("autotune.SegmentRequestInfo", new Dictionary<string,object>() {
						{"segment_id", segmentConfig.segment_id},
						{"group_id", segmentConfig.group_id},
						{"error", _isError},
						{"player_override", _isPlayerOverride},
						{"request_latency", _requestTime},
						{"callback_latency", callbackTime},
						{"fetch_time", _fetchInGameTime},
						{"callback_time", callbackInGameTime},
						{"model", deviceInfo.model},
						{"ram", deviceInfo.ram},
						{"cpu", deviceInfo.cpu},
						{"cpu_count", deviceInfo.cpu_count},
						{"gfx_name", deviceInfo.gfx_name},
						{"gfx_vendor", deviceInfo.gfx_vendor},
						{"screen", deviceInfo.screen},
						{"dpi", deviceInfo.dpi},
						{"gfx_ver", deviceInfo.gfx_ver},
						{"gfx_shader", deviceInfo.gfx_shader},
						{"max_texture_size", deviceInfo.max_texture_size},
						{"os_ver", deviceInfo.os_ver},
						{"platform_id", deviceInfo.platform_id},
						{"app_build_version", _buildVersion},
						{"plugin_version", AutoTuneMeta.version},
						{"project_id", deviceInfo.project_id},
						{"environment", _endpoint}
					});
#if DEBUG
                    Debug.Log("autotune.SegmentRequestInfo event status: " + status);
#endif
                }
			catch (System.Exception e)
			{
				Debug.LogError(e);
			}
			finally
			{
				_isError = false;
				_updateNeeded = false;
			}
		}
	}

	private void TryFetch()
	{
		using (var client = new WebClient()) {
			client.UploadDataCompleted += (new UploadDataCompletedEventHandler(wc_UploadDataCompleted));
			client.Headers.Add("Content-Type","application/json");
			DeviceInfo di = new DeviceInfo(_projectId, _buildVersion);
			string payload = JsonUtility.ToJson(di);
			_deviceInfo = di;
			byte[] bytes = System.Text.Encoding.UTF8.GetBytes(payload);
			string baseURL = (_endpoint == Endpoint.Sandbox ? SandboxEndpoint : ProductionEndpoint);
			var uri = new System.Uri(baseURL + "/v1/settings");
			try {
#if DEBUG
                    Debug.Log("autotune will send data:");
				Debug.Log(payload);
#endif
                    client.UploadDataAsync(uri, "POST", bytes);
			} catch (WebException err) {
				Debug.LogError("autotune error on web request");
				Debug.LogError(err);
			}
		}
	}

	/// <summary>
	/// Clean the dictionary and strip the params from the server response.
	/// Never returns null.
	/// </summary>
	private SegmentConfig ParseResponse(Dictionary<string,object> response)
	{
		var res = new Dictionary<string,object>();
		var pars = response["params"] as List<object>;
		var segmentId = (string)response["segment_id"];
		int groupId = (int)(long)response["group"];
		foreach (var param in pars)
		{
			var dict = param as Dictionary<string,object>;
			res[(string)dict["name"]] = dict["value"];
		}
		
		// TODO: add server provided hash
		return new SegmentConfig(segmentId, groupId, res, "0");
	}

	private void wc_UploadDataCompleted(object sender,
										UploadDataCompletedEventArgs e)
	{
		if (e.Cancelled)
		{
			Debug.Log("autotune request canceled");
			lock(this)
			{
				_isError = true;
				_updateNeeded = true;
			}
		}
		else if (e.Error != null)
		{
#if DEBUG
                // something happened, send the default config
                //Debug.Log("autotune request error: ");
			//Debug.LogError(e.Error);
#endif

                lock(this)
			{
				_isError = true;
				_updateNeeded = true;
			}
		}
		else
		{
			try {
				var jsonStr = System.Text.Encoding.UTF8.GetString(e.Result);
				var resp = Json.Deserialize(jsonStr) as Dictionary<string,object>;
				Debug.LogFormat ("autotune response payload: {0}", jsonStr);
				var newSegmentConfig = ParseResponse(resp);

				lock(this)
				{
					// configuration changed, save it to file
					if(_cachedSegmentConfig.config_hash != newSegmentConfig.config_hash) {
						CacheSegmentConfig(newSegmentConfig);
					}
					else {
						// TODO - remove once server provided hash is in place
						CacheSegmentConfig(newSegmentConfig);
					}

					_cachedSegmentConfig = newSegmentConfig;
					_updateNeeded = true;
				}
			}
			catch(Exception ex) {
				Debug.LogError("autotune error parsing response: " + ex);
				lock(this)
				{
					_isError = true;
					_updateNeeded = true;
				}
			}
		}

		_requestTime = AutoTune.getCurrentTimestamp() - _startTime;
		Debug.LogFormat ("autotune request time: {0}", _requestTime);
	}

	private void CacheSegmentConfig(SegmentConfig config) 
    {
    	var dirPath = _storePath + AutoTuneDir;
    	if(!Directory.Exists(dirPath)) {
    		Directory.CreateDirectory(dirPath);
    	}

    	var filePath = _storePath + SegmentConfigCacheFilePath;
    	Debug.Log("autotune storing segment config to file: " + filePath);
    	using(var writer = new StreamWriter(filePath)) {
    		writer.Write(config.ToJsonDictionary()); 
    	}
    }


    /// <summary>
	/// Loads from file.
	/// On failure this defaults to use client default setting.
	/// </summary>
    private void LoadCacheSegmentConfig()
    {
    	var filePath = _storePath + SegmentConfigCacheFilePath;
    	if(!File.Exists(filePath)){
    		_cachedSegmentConfig = _clientDefaultConfig;
    		Debug.Log("autotune did not find cached config in path: " + filePath);
    		return;
    	}

		try {
			using(var reader = new StreamReader(filePath)) {
				var json = reader.ReadToEnd();
				_cachedSegmentConfig = SegmentConfig.fromJsonDictionary(json);
				Debug.Log("autotune loaded cached config: " + json);
			}
		}
		catch(Exception ex) {
			// for any issues with the file, use client defaults
			_cachedSegmentConfig = _clientDefaultConfig;
			Debug.LogError("autotune error processing cached config file: " + filePath + " , error: " + ex);
		}
    }

	private void CleanUp()
	{
		_updateNeeded = false;
		_isError = false;
		_deviceInfo = null;
		_callback = null;
		_startTime = 0;
	}

	private static long getCurrentTimestamp() {
		return DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
	}
}

}
