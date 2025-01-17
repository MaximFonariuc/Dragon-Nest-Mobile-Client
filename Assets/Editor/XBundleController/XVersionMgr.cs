using UnityEngine;
using System.Collections;
using System.IO;
using UnityEditor;
using XEditor;
using XUpdater;
using System.Collections.Generic;
using ABSystem;

public class XVersionMgr
{
    public static bool BuildRePublished()
    {
        //create new empty version bundle
        return BuildEmptyManifest(XBundleTools.singleton.NextVersion);
    }

    public static bool BuildHotPatch()
    {
        //build ui res
        if (!BuildMetaRes(XBundleTools.singleton.GetAbs(), XBundleTools.singleton.Manifest.AB, AssetBundleBuildPanel.BuildAssetBundlesUpdate))
        {
            EditorUtility.DisplayDialog("Error", "AB Package Failed! Press OK to Quit.", "OK");
            return false;
        }
        else
            EditorUtility.DisplayDialog("Information", "AB Package Completed! Press OK to Continue.", "OK");

        ////build scene res
        //if (!BuildMetaRes(XBundleTools.singleton.GetScenes(), XBundleTools.singleton.Manifest.Scene, XSceneBundleTools.singleton.BundleRunPackage))
        //{
        //    EditorUtility.DisplayDialog("Error", "Scene Package Failed! Press OK to Quit.", "OK");
        //    return false;
        //}
        //else
        //    EditorUtility.DisplayDialog("Information", "Scene Package Completed! Press OK to Continue.", "OK");

        //build ui res
        if (!BuildMetaRes(XBundleTools.singleton.GetFmods(), XBundleTools.singleton.Manifest.FMOD, FmodConfigEditor.BuildFMODUpdate))
        {
            EditorUtility.DisplayDialog("Error", "FMOD Package Failed! Press OK to Quit.", "OK");
            return false;
        }
        else
            EditorUtility.DisplayDialog("Information", "FMOD Package Completed! Press OK to Continue.", "OK");

        //get patch objects
        List<UnityEngine.Object> objects = XBundleTools.singleton.GetObjects();
        //build res bundle
        XBundleBuilder.BuildBundle(objects, EditorUserBuildSettings.activeBuildTarget);

        //build manifest bundle
        return XBundleBuilder.WriteVersion(
            XBundleTools.singleton.Manifest,
            EditorUserBuildSettings.activeBuildTarget,
            XBundleTools.singleton.NextVersion);
    }

    private static bool BuildMetaRes(List<string> res, List<XMetaResPackage> manifest, BundleMetaRes call)
    {
        if (res == null) return false;

        if (res.Count > 0)
        {
            List<XMetaResPackage> packages = InnerBuildMetaRes(res, call);
            if (packages != null && packages.Count > 0)
            {
                Details(packages, manifest, XBundleTools.singleton.Version_Next);
            }
            else
                return false;
        }
        
        return true;
    }

    private static List<XMetaResPackage> InnerBuildMetaRes(List<string> res, BundleMetaRes call)
    {
        List<XMetaResPackage> packages = null;

        if (res.Count > 0)
        {
            //call bundles pack function
            packages = call(res, XBundleTools.BundleRoot + XBundleTools.singleton.Version_Next + "/");

            string error = (packages == null || packages.Count == 0) ? "empty package" : null;

            for (int i = 0; i < packages.Count; i++)
            {
                if (!(packages[i].download.EndsWith(".ab") || packages[i].download.EndsWith(".all") || packages[i].download.EndsWith(".bank") || packages[i].download.EndsWith(".cfg")))
                {
                    error += packages[i].buildinpath + "\n";
                }
            }

            if (!string.IsNullOrEmpty(error))
            {
                EditorUtility.DisplayDialog("Error", error, "OK");
                return null;
            }
        }

        return packages;
    }

    private static void Details(List<XMetaResPackage> outresbundles, List<XMetaResPackage> manifest, string bundle)
    {
        foreach (XMetaResPackage s in outresbundles)
        {
            XMetaResPackage package = new XMetaResPackage();

            package.download = bundle + "/" + s.download;
            package.buildinpath = s.buildinpath;
            package.bundle = bundle;
            package.Size = s.Size;

            //update manifest
            for (int i = 0; i < manifest.Count; i++)
            {
                string download = manifest[i].download.Substring(manifest[i].download.IndexOf('/') + 1);
                if (download == s.download)
                {
                    manifest.RemoveAt(i);
                    break;
                }
            }
            manifest.Add(package);
        }
    }

    public static bool BuildEmptyManifest(XVersionData version)
    {
        return XBundleBuilder.WriteEmptyVersion(
            EditorUserBuildSettings.activeBuildTarget,
            version);
    }

    public static XVersionData GetManifest()
    {
        return XDataIO<XVersionData>.singleton.DeserializeData(XBundleBuilder.LoadBundleText(XBundleTools.BundleManifest, "manifest"));
    }
}
