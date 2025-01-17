

using UnityEngine;
using XUtliPoolLib;
using System.Collections;
using System;
using System.Text;
#if !DISABLE_JOYSDK
using Assets.SDK;
#endif


public class XBuglyMgr : MonoBehaviour, IXBuglyMgr
{
	#if !DISABLE_JOYSDK
    private JoyYouSDK _interface;
	#endif
    void Awake()
    {
		#if !DISABLE_JOYSDK
        _interface = new JoyYouSDK();
		#endif

#if BUGLY
        BuglyAgent.ConfigDebugMode(false);
        switch (Application.platform)
        {
            case RuntimePlatform.IPhonePlayer:
                BuglyAgent.InitWithAppId("i1105309683");
                break;
            case RuntimePlatform.Android:
                BuglyAgent.InitWithAppId("1105309683");
                break;
        }
        BuglyAgent.EnableExceptionHandler();
#endif
    }

    public void ReportCrashToBugly(string serverid, string rolename, uint rolelevel, int roleprof, string openid, string version, string realtime, string scenename, string sceneid, string content)
    {
#if BUGLY
        //((IHuanlePlatform)_interface).SendGameExtData("BuglyReport", token.ToString());
        BuglyAgent.ReportException("Exception", string.Format("ServerID: {0}  RoleName: {1}  RoleLevel: {2}  RoleProf: {3}  OpenID: {4}  Version: {5}  RealTime:  {6}  SceneName:  {7}({8})",
            serverid, rolename, rolelevel, roleprof, openid, version, realtime, scenename, sceneid), content);
#endif
    }

    public bool Deprecated
    {
        get;
        set;
    }
}


