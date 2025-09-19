using UnityEngine;
using System;
using System.IO;
using System.Collections;
using UnityEditor;

[InitializeOnLoad]
public class DeploySettings : ScriptableObject {

	const string msdkSettingsAssetName = "DeploySettings";
	const string msdkSettingsPath = "Msdk/Editor/Resources";
	const string msdkSettingsAssetExtension = ".asset";

	private static DeploySettings _instance;

	public static DeploySettings Instance {
		get {
			if (_instance == null) {
				_instance = Resources.Load(msdkSettingsAssetName) as DeploySettings;
				if (_instance == null) {
					_instance = CreateInstance<DeploySettings>();

					string properPath = Path.Combine(Application.dataPath, msdkSettingsPath);
					if (!Directory.Exists(properPath)) {
						Directory.CreateDirectory(properPath);
					}

					string fullPath = Path.Combine(Path.Combine("Assets", msdkSettingsPath),
					                               msdkSettingsAssetName + msdkSettingsAssetExtension
					                               );
					AssetDatabase.CreateAsset(_instance, fullPath);
				}
			}
			return _instance;
		}
	}

	#region Deploy Settings
	[SerializeField]
	private string bundleId = "com.example.wegame";
	[SerializeField]
	private string qqAppId = "100703379";
	[SerializeField]
	private string wxAppId = "wxcde873f99466f74a";
	[SerializeField]
	private string msdkKey = "5d1467a4d2866771c3b289965db335f4";
	[SerializeField]
	private string androidOfferId = "100703379";
	[SerializeField]
	private string iOSOfferId = "1450001484";

    private string qqScheme = "QQ06009C93";

    [SerializeField]
    private bool enableMSDKPolicy = false;

	[SerializeField]
	private bool useMSDKBugly = true;
    
	public string BundleId {
		get { return Instance.bundleId; }
	}

	public string QqAppId {
		get { return Instance.qqAppId; }
	}


	public string WxAppId {
		get { return Instance.wxAppId; }
	}

	public string MsdkKey {
		get { return Instance.msdkKey; }
	}

	public string AndroidOfferId {
		get { return Instance.androidOfferId; }
	}

	public string IOSOfferId {
		get { return Instance.iOSOfferId; }
	}

    public string QqScheme {
        get { return Instance.qqScheme; }
    }

        
    public bool EnableMSDKPolicy
    {
	    get { return Instance.enableMSDKPolicy; }
    }

	public bool UseMSDKBugly
	{
		get {
			return Instance.useMSDKBugly;
		}
	}

	public void SetBundleId(string value) {
		bundleId = value.Trim ();
		DirtyEditor();
	}

	public void SetQQAppId(string value) {
		qqAppId = value.Trim ();
        try {
            int idNum;
            if (System.Int32.TryParse(QqAppId, out idNum)) {
                qqScheme = "QQ" + string.Format("{0:X8}", idNum);
            }
        } catch (Exception e) {
            qqScheme = "";
            Debug.LogException(e);
        }
		DirtyEditor();
	}

	public void SetWXAppId(string value) {
		wxAppId = value.Trim ();
		DirtyEditor ();
	}

	public void SetMsdkKey(string value) {
		msdkKey = value.Trim ();
		DirtyEditor ();
	}

	public void SetAndroidOfferId(string value) {
		androidOfferId = value.Trim ();
		DirtyEditor ();
	}

	public void SetIOSOfferId(string value) {
		iOSOfferId = value.Trim ();
		DirtyEditor ();
    }

	public void SetEnableMSDKPolicy(bool value)
	{
		if (enableMSDKPolicy != value)
		{
			enableMSDKPolicy = value;
			DirtyEditor();
		}
	}

	public void SetUseMSDKBugly(bool value)
	{
		if (useMSDKBugly != value)
		{
			useMSDKBugly = value;
			DirtyEditor();
		}
	}

	private static void DirtyEditor() {
		EditorUtility.SetDirty(Instance);
	}
	#endregion
}
