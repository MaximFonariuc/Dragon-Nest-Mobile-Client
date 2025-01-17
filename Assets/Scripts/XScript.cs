using UnityEngine;
using XUtliPoolLib;

public class XScript : MonoBehaviour
{
    void Awake()
    {
        RuntimePlatform editorPlatform = Application.platform;
#if UNITY_IOS
        editorPlatform =  RuntimePlatform.IPhonePlayer;
#elif UNITY_ANDROID
        editorPlatform = RuntimePlatform.Android;
#else
        editorPlatform = RuntimePlatform.WindowsPlayer;
#endif
        XUtliPoolLib.ShaderManager.singleton.Awake(editorPlatform);
    }

    // Use this for initialization
    void Start()
    {
        XUpdater.XShell.singleton.Awake();
        XUpdater.XShell.singleton.Start();
        XResourceLoaderMgr.singleton.SetUnloadCallback(BeforeUnityUnloadResource);
    }

    // Update is called once per frame
    void Update()
    {
        XUpdater.XShell.singleton.Update();
    }

    void LateUpdate()
    {
        XUpdater.XShell.singleton.PostUpdate();
    }
    void OnApplicationQuit()
    {
        FMODUnity.RuntimeStudioEventEmitter.isQuitting = true;
        XUpdater.XShell.singleton.Quit();
    }

    void BeforeUnityUnloadResource()
    {
        FastListV3.ms_Pool.Clear();
        FastListV2.ms_Pool.Clear();
        FastListColor32.ms_Pool.Clear();
    }

    private IGameSysMgr m_GameSysMgr = null;
    public IGameSysMgr GameSysMgr
    {
        get
        {
            if (null == m_GameSysMgr || m_GameSysMgr.Deprecated)
                m_GameSysMgr = XInterfaceMgr.singleton.GetInterface<IGameSysMgr>(XCommon.singleton.XHash("IGameSysMgr"));
            return m_GameSysMgr;
        }
    }

    void OnApplicationPause(bool pause)
    {
        if (GameSysMgr != null) GameSysMgr.GamePause(pause);
    }
}
