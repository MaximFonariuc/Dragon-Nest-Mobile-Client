using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;


[InitializeOnLoad]
class MsdkMenu : ScriptableObject
{
    [MenuItem("MSDK/Deploy Settings")]
    public static void EditDeploy()
    {
        Selection.activeObject = DeploySettings.Instance;
    }

    [MenuItem("MSDK/Config Settings")]
    public static void EditConfig()
    {
        Selection.activeObject = ConfigSettings.Instance;
    }

    [MenuItem("MSDK/PC Environment Settings")]
    public static void EditCallback()
    {
        Selection.activeObject = CallbackSettings.Instance;
    }

    [MenuItem("MSDK/MSDK Unity Wiki")]
    public static void OpenWiki()
    {
        string url = "http://msdk.oa.com/Unity/config.html#Unity_Outline";
        Application.OpenURL(url);
    }

}
