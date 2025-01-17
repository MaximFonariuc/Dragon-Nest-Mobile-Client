#if UNITY_5
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using XUpdater;
using XUtliPoolLib;

namespace ABSystem
{
    public class AssetBundleBuilder5x : ABBuilder
    {
        public AssetBundleBuilder5x(AssetBundlePathResolver resolver)
            : base(resolver)
        {

        }

        public override void Export()
        {
            base.Export();

            Dictionary<string, string> hashDic = new Dictionary<string, string>();
            string assetPath = "";
            List<AssetTarget> hashList = AssetBundleUtils.GetAll();
            List<string> dupList = new List<string>();
            bool flag = false;
            foreach (AssetTarget target in hashList)
            {
                if (hashDic.TryGetValue(target.bundleName, out assetPath))
                {
                    dupList.Add("Hash Duplicate"+ assetPath + " & " + target.assetPath);

                    flag = true;
                }
                else
                {
                    hashDic.Add(target.bundleName, target.assetPath);
                }
            }
            foreach(string str in dupList)
            {
                Debug.LogError(str);
            }
            if (flag) return;

            string bundleSavePath = this.pathResolver.DefaultBundleSavePath;
            switch (EditorUserBuildSettings.activeBuildTarget)
            {
                case UnityEditor.BuildTarget.iOS:
                    bundleSavePath = this.pathResolver.iOSBundleSavePath;
                    break;
                case UnityEditor.BuildTarget.Android:
                    bundleSavePath = this.pathResolver.AndroidBundleSavePath;
                    break;
                default:
                    bundleSavePath = this.pathResolver.DefaultBundleSavePath;
                    break;
            }

            List<AssetBundleBuild> list = new List<AssetBundleBuild>();
            //标记所有 asset bundle name
            List<AssetTarget> all = AssetBundleUtils.GetAll();
            int size = 0;
            int length = AssetBundleUtils.UpdateBuild() ? all.Count : 400;
            do
            {
                for (int i = size; i < all.Count && i < size + length; i++)
                {
                    AssetTarget target = all[i];
                    if (target.needSelfExport)
                    {
                        AssetBundleBuild build = new AssetBundleBuild();
                        build.assetBundleName = target.bundleName;
                        build.assetNames = new string[] { target.assetPath };
                        list.Add(build);
                    }
                }

                //开始打包
                BuildPipeline.BuildAssetBundles(bundleSavePath, list.ToArray(), BuildAssetBundleOptions.DeterministicAssetBundle | BuildAssetBundleOptions.ChunkBasedCompression, EditorUserBuildSettings.activeBuildTarget);

                size += length;
            }
            while (size < all.Count);

#if UNITY_5_1 || UNITY_5_2
            AssetBundle ab = AssetBundle.CreateFromFile(pathResolver.BundleSavePath + "/AssetBundles");
#else
            AssetBundle ab = AssetBundle.LoadFromFile(bundleSavePath + "/AssetBundles");
#endif
            AssetBundleManifest manifest = ab.LoadAsset("AssetBundleManifest") as AssetBundleManifest;
            //hash
            for (int i = 0; i < all.Count; i++)
            {
                AssetTarget target = all[i];
                if (target.needSelfExport)
                {
                    Hash128 hash = manifest.GetAssetBundleHash(target.bundleName);
                    if (target.bundleCrc != hash.ToString())
                    {
                        if (AssetBundleBuildPanel.ABUpdateOpen)
                        {
                            string savePath = target.bundleSavePath;
                            string downloadPath = Path.Combine(Path.Combine(AssetBundleBuildPanel.TargetFolder, "AssetBundles"), target.bundleName);
                            File.Copy(savePath, downloadPath, true);
                            XMetaResPackage pack = new XMetaResPackage();
                            pack.buildinpath = target.bundleShortName;
                            pack.download = string.Format("AssetBundles/{0}", target.bundleName);
                            pack.Size = (uint)(new FileInfo(downloadPath)).Length;
                            if (pack.Size != (new FileInfo(downloadPath)).Length)
                            {
                                EditorUtility.DisplayDialog("Bundle ", target.bundleShortName + " is lager than UINTMAX!!!", "OK");
                            }
                            AssetBundleBuildPanel.assetBundleUpdate.Add(pack);
                        }
                    }
                    target.bundleCrc = hash.ToString();
                }
            }
            this.SaveDepAll(all);
            ab.Unload(true);
            this.RemoveUnused(all);

            AssetDatabase.RemoveUnusedAssetBundleNames();
            AssetDatabase.Refresh();
        }
    }
}
#endif