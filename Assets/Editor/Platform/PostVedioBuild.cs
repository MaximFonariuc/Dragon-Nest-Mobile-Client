using System.IO;
using UnityEngine;


public class PostVedioBuild
{
    
    public static void OnPostProcessBuild()
    {
        string bt = Path.GetDirectoryName(Application.dataPath);
        string kind = SelectPlatformEditor.Kind;
        string src = Path.Combine(bt, "Shell/FullScreenVideoPlayer.mm");
        string dst = Path.Combine(bt, "IOS/DragonNest" + kind + "/Classes/Unity/FullScreenVideoPlayer.mm");
        Debug.Log("vedio src:" + src);
        Debug.Log("vedio dest:" + dst);
        File.Copy(src, dst, true);

        src = Path.Combine(bt, "Shell/UnityViewControllerBaseiOS.mm");
        dst = Path.Combine(bt, "IOS/DragonNest" + kind + "/Classes/UI/UnityViewControllerBaseiOS.mm");
        Debug.Log("src:" + src);
        Debug.Log("dest:" + dst);
        File.Copy(src, dst, true);
    }
}
