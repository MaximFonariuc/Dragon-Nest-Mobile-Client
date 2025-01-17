using UnityEngine;
using System.Collections;
using UnityEditor;
using System.IO;
using XUpdater;
using System.Collections.Generic;
using FMODUnity;

public class FmodConfigEditor : EditorWindow
{
    [MenuItem(@"XBuild/RefreshFMODConfig")]
    public static void FastBuild()
    {
        StreamWriter sw;
        sw = new StreamWriter("Assets/Resources/fmodconfig.txt");
        string str = "Master_Bank|Master_Bank.strings|";
        for (int i = 0; i < Settings.Instance.Banks.Count; ++i)
        {
            str += i == 0 ? Settings.Instance.Banks[i] : ("|" + Settings.Instance.Banks[i]);
        }
        sw.Write(str);
        sw.Flush();
        sw.Close();
        AssetDatabase.ImportAsset("Assets/Resources/fmodconfig.txt");

        List<AssetBundleBuild> list = new List<AssetBundleBuild>();
        AssetBundleBuild build = new AssetBundleBuild();
        build.assetBundleName = "fmodconfig.ab";
        build.assetNames = new string[] { "Assets/Resources/fmodconfig.txt" };
        list.Add(build);

        BuildPipeline.BuildAssetBundles("Assets/Resources", list.ToArray(), BuildAssetBundleOptions.DeterministicAssetBundle | BuildAssetBundleOptions.ChunkBasedCompression, EditorUserBuildSettings.activeBuildTarget);

        AssetDatabase.ImportAsset("Assets/Resources/fmodconfig.ab");
    }

    public static List<XMetaResPackage> fmodUpdate = new List<XMetaResPackage>();
    public static List<XMetaResPackage> BuildFMODUpdate(List<string> rawList, string destFolder)
    {
        if (!Directory.Exists(Path.Combine(destFolder, "FMOD")))
            Directory.CreateDirectory(Path.Combine(destFolder, "FMOD"));

        if (rawList.Count > 0) FastBuild();

        fmodUpdate.Clear();

        string targetPath = Path.Combine(destFolder, "FMOD");

        for (int i = 0; i < rawList.Count; ++i)
        {
            if (rawList[i].Contains("FMODStudioSettings.asset")) rawList[i] = rawList[i].Replace("FMODStudioSettings.asset", "fmodconfig.ab");
            string downloadPath = Path.Combine(targetPath, Path.GetFileName(rawList[i]));
            File.Copy(Path.Combine("Assets", rawList[i]), downloadPath, true);
            XMetaResPackage pack = new XMetaResPackage();
            pack.buildinpath = rawList[i];
            pack.download = string.Format("FMOD/{0}", Path.GetFileName(rawList[i]));
            pack.Size = (uint)(new FileInfo(downloadPath)).Length;
            fmodUpdate.Add(pack);
        }

        return fmodUpdate;
    }
}