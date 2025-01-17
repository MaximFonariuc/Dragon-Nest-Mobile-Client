using UnityEngine;
using System.Collections;
using XUpdater;
using System;
using UnityEditor;
using XEditor;
using System.Security.Cryptography;
using System.IO;
using System.Collections.Generic;
using System.Text;

public class XBundleBuilder
{
    public static int Md5Length = 16;

    private static UnityEditor.BuildTarget TargetConvertor(XUpdater.BuildTarget target)
    {
        switch (target)
        {
            case XUpdater.BuildTarget.IOS: return UnityEditor.BuildTarget.iOS;
            case XUpdater.BuildTarget.Android: return UnityEditor.BuildTarget.Android;
            default: throw new Exception();
        }
    }

    private static XUpdater.BuildTarget TargetConvertor(UnityEditor.BuildTarget target)
    {
        switch (target)
        {
            case UnityEditor.BuildTarget.iOS: return XUpdater.BuildTarget.IOS;
            case UnityEditor.BuildTarget.Android: return XUpdater.BuildTarget.Android;
            default: throw new Exception();
        }
    }

    private static void WriteVersionToFile(XVersionData data, string path)
    {
        XDataIO<XVersionData>.singleton.SerializeData(path, data, new Type[] { typeof(UnityEngine.Object) });

        //if (MD5)
        {
            MD5CryptoServiceProvider _md5Generator = new MD5CryptoServiceProvider();
            byte[] content = File.ReadAllBytes(path);

            byte[] bs = _md5Generator.ComputeHash(content);

            using (FileStream fs = new FileStream(path, FileMode.Truncate))
            {
                using (BinaryWriter bw = new BinaryWriter(fs))
                {
                    bw.Write(bs);
                    bw.Write(content);
                }
            }

            BitConverter.ToString(bs);
        }
    }

    public static MemoryStream LoadBundleText(string path, string name)
    {
        byte[] bs = File.ReadAllBytes(path);
        AssetBundle ab = AssetBundle.LoadFromMemory(bs);

		TextAsset t = ab.LoadAsset (name) as TextAsset;
        MemoryStream s = new MemoryStream(t.bytes, Md5Length, t.bytes.Length - Md5Length);

        ab.Unload(true);
        return s;
    }

    public static bool WriteEmptyVersion(UnityEditor.BuildTarget target, XVersionData version)
    {
        string path = XBundleTools.BundleRoot + "manifest.bytes";

        XVersionData current_data = new XVersionData();
        current_data.Target_Platform = TargetConvertor(target);
        current_data.VersionCopy(version);

        WriteVersionToFile(current_data, path);

        return BundleVersion(path, target, version.ToString());
    }

    public static bool WriteVersion(XVersionData version, UnityEditor.BuildTarget target, XVersionData next)
    {
        string path = XBundleTools.BundleRoot + "manifest.bytes";

        version.Target_Platform = TargetConvertor(target);
        version.VersionCopy(next);

        WriteVersionToFile(version, path);

        return BundleVersion(path, target, version.ToString());
    }

    public static bool BundleVersion(string path, UnityEditor.BuildTarget target, string version)
    {
        AssetDatabase.Refresh();

        TextAsset data = AssetDatabase.LoadAssetAtPath(path, typeof(TextAsset)) as TextAsset;

        if (data == null || data.bytes.Length == 0)
        {
            EditorUtility.DisplayDialog("Error", "Generate Version Bundle error.", "OK");
            return false;
        }

        List<AssetBundleBuild> list = new List<AssetBundleBuild>();
        AssetBundleBuild build = new AssetBundleBuild();
        build.assetBundleName = "manifest.assetbundle";
        build.assetNames = new string[] { path };
        list.Add(build);

        BuildPipeline.BuildAssetBundles(XBundleTools.BundleRoot, list.ToArray(), BuildAssetBundleOptions.ChunkBasedCompression | BuildAssetBundleOptions.DeterministicAssetBundle, EditorUserBuildSettings.activeBuildTarget);

        Resources.UnloadAsset(data);

        File.Delete(path);
        AssetDatabase.Refresh();

        //create manifest with version marked.
        byte[] content = File.ReadAllBytes(XBundleTools.BundleRoot + "manifest.assetbundle");
        File.WriteAllBytes(XBundleTools.BundleRoot + "manifest." + version + ".assetbundle", content);

        AssetDatabase.Refresh();

        return true;
    }

    public static void BuildBundle(List<UnityEngine.Object> objects, UnityEditor.BuildTarget target)
    {
        if (objects.Count == 0) return;

        string bundlepath = XBundleTools.BundleRoot + XBundleTools.singleton.Version_Next + ".assetbundle";

        string MD5 = InnerBuild(objects, bundlepath, target);
        //process bundle
        BundleDetail(XBundleTools.singleton.Version_Next, MD5, bundlepath);

        //process res package
        ResDetail(objects, XBundleTools.singleton.Version_Next);

        AssetDatabase.Refresh();
    }

    private static string InnerBuild(List<UnityEngine.Object> objects, string bundlepath, UnityEditor.BuildTarget target)
    {

        List<AssetBundleBuild> list = new List<AssetBundleBuild>();
        AssetBundleBuild build = new AssetBundleBuild();
        List<string> pathList = new List<string>();
        build.assetBundleName = Path.GetFileName(bundlepath);
        for (int i = 0; i < objects.Count; ++i)
        {
            pathList.Add(AssetDatabase.GetAssetPath(objects[i]));
        }
        build.assetNames = pathList.ToArray();
        list.Add(build);

        BuildPipeline.BuildAssetBundles(XBundleTools.BundleRoot, list.ToArray(), BuildAssetBundleOptions.ChunkBasedCompression | BuildAssetBundleOptions.DeterministicAssetBundle, EditorUserBuildSettings.activeBuildTarget);

        MD5CryptoServiceProvider _md5Generator = new MD5CryptoServiceProvider();

        byte[] content = File.ReadAllBytes(bundlepath);
        byte[] bs = _md5Generator.ComputeHash(content);

        return System.BitConverter.ToString(bs);
    }

    private static void BundleDetail(string name, string md5, string path)
    {
        XBundleData bundle = new XBundleData();

        bundle.Name = name;
        bundle.Level = XBundlePresent.Level;

        FileInfo info = new FileInfo(path);
        //size
        bundle.Size = (uint)info.Length;
        //update crc
        bundle.MD5 = md5;

        //update manifest
        for (int i = 0; i < XBundleTools.singleton.Manifest.Bundles.Count; i++)
        {
            if (XBundleTools.singleton.Manifest.Bundles[i].Name == name)
            {
                XBundleTools.singleton.Manifest.Bundles.RemoveAt(i);
                break;
            }
        }
        XBundleTools.singleton.Manifest.Bundles.Add(bundle);
    }

    private static void ResDetail(List<UnityEngine.Object> objects, string bundle)
    {
        foreach (UnityEngine.Object o in objects)
        {
            XResPackage package = new XResPackage();

            package.location = GetResLoadPath(o);
            package.type = o.GetType().ToString();
            package.rtype = package.location == "XMainClient" ? ResourceType.Script : ResourceType.Scene;
            package.bundle = bundle;

            //update manifest
            for (int i = 0; i < XBundleTools.singleton.Manifest.Res.Count; i++)
            {
                if (XBundleTools.singleton.Manifest.Res[i].location == package.location &&
                    XBundleTools.singleton.Manifest.Res[i].type == package.type)
                {
                    XBundleTools.singleton.Manifest.Res.RemoveAt(i);
                    break;
                }
            }
            XBundleTools.singleton.Manifest.Res.Add(package);
        }
    }

    private static string GetResLoadPath(UnityEngine.Object o)
    {
        if (o == null) return null;

        string path = AssetDatabase.GetAssetPath(o);
        int idx = path.LastIndexOf('.');

        StringBuilder bs = new StringBuilder(path);
        bs = bs.Remove(idx, bs.Length - idx);
        bs = bs.Remove(0, 17);
        return bs.ToString();
    }
}
