using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using XUtliPoolLib;

namespace ABSystem
{
    public class ABBuilder
    {
        protected AssetBundleDataWriter dataWriter = new AssetBundleDataBinaryWriter();
        protected AssetBundlePathResolver pathResolver;
        public static bool isBuildAB = false;

        public ABBuilder() : this(new AssetBundlePathResolver())
        {
        }

        public ABBuilder(AssetBundlePathResolver resolver)
        {
            this.pathResolver = resolver;
            this.InitDirs();
            AssetBundleUtils.pathResolver = pathResolver;
        }

        void InitDirs()
        {
            switch(EditorUserBuildSettings.activeBuildTarget)
            {
                case BuildTarget.Android:
                    new DirectoryInfo(pathResolver.AndroidBundleSavePath).Create();
                    new FileInfo(pathResolver.AndroidHashCacheSaveFile).Directory.Create();
                    break;
                case BuildTarget.iOS:
                    new DirectoryInfo(pathResolver.iOSBundleSavePath).Create();
                    new FileInfo(pathResolver.iOSHashCacheSaveFile).Directory.Create();
                    break;
                default:
                    new DirectoryInfo(pathResolver.DefaultBundleSavePath).Create();
                    new FileInfo(pathResolver.DefaultHashCacheSaveFile).Directory.Create();
                    break;
            }
        }

        public void Begin()
        {
            isBuildAB = true;
            EditorUtility.DisplayProgressBar("Loading", "Loading...", 0.1f);
            AssetBundleUtils.Init();
        }

        public void End()
        {
            AssetBundleUtils.SaveCache();
            AssetBundleUtils.ClearCache();
            EditorUtility.ClearProgressBar();
            isBuildAB = false;
        }

        public virtual void Analyze()
        {
            var all = AssetBundleUtils.GetAll();
            float total = all.Count;
            float count = 0;
            foreach (AssetTarget target in all)
            {
                target.Analyze();
                EditorUtility.DisplayProgressBar(string.Format("Analyze...({0}/{1})" ,count, total), target.assetPath, ++count / total);
            }
            all = AssetBundleUtils.GetAll();
            total = all.Count;
            count = 0;
            foreach (AssetTarget target in all)
            {
                target.Merge();
                EditorUtility.DisplayProgressBar(string.Format("Merge...({0}/{1})", count, total), target.assetPath, ++count / total);
            }
            all = AssetBundleUtils.GetAll();
            total = all.Count;
            count = 0;
            foreach (AssetTarget target in all)
            {
                target.BeforeExport();
                EditorUtility.DisplayProgressBar(string.Format("BeforeExport...({0}/{1})", count, total), target.assetPath, ++count / total);
            }
        }

        public virtual void Export()
        {
            this.Analyze();
        }

        private void AddAssetUIDependencies(Object asset)
        {
            Object[] deps = EditorUtility.CollectDependencies(new Object[] { asset });

            for (int i = 0; i < deps.Length; i++)
            {
                Object dep = deps[i];
                string path = AssetDatabase.GetAssetPath(dep);
                if (path.StartsWith("Assets/Resources/atlas")|| path.StartsWith("Assets/Resources/StaticUI"))
                {
                    AddRootTargets(path);
                }
            }
        }

        public void AddRootTargets(string path)
        {
            if (path.EndsWith(".meta")) return;

            if (path.Contains("atlas/UI"))
            {
                if (path.Contains("_A.png") && File.Exists(path.Replace("_A.png", ".prefab")))
                {
                    return;
                }
                else if (path.Contains(".png") && File.Exists(path.Replace(".png", ".prefab")))
                {
                    return;
                }
                else if (path.Contains(".mat") && File.Exists(path.Replace(".mat", ".prefab")))
                {
                    return;
                }
            }

            if(path.Contains("Resources/UI"))
            {
                AddAssetUIDependencies(AssetDatabase.LoadMainAssetAtPath(path));
            }

            FileInfo file = new FileInfo(path);
            AssetTarget target = AssetBundleUtils.Load(file);
            if (target == null)
                Debug.LogError(file);
            target.exportType = AssetBundleExportType.Root;
        }

        public void AddRootTargets(DirectoryInfo bundleDir, string[] partterns = null, SearchOption searchOption = SearchOption.AllDirectories)
        {
            if (partterns == null)
                partterns = new string[] { "*.*" };
            for (int i = 0; i < partterns.Length; i++)
            {
                FileInfo[] prefabs = bundleDir.GetFiles(partterns[i], searchOption);
                foreach (FileInfo file in prefabs)
                {
                    if (file.FullName.EndsWith(".meta")) continue;
                    if (file.FullName.Contains("atlas/UI"))
                    {
                        if (file.FullName.Contains("_A.png") && File.Exists(file.FullName.Replace("_A.png", ".prefab")))
                        {
                            continue;
                        }
                        else if (file.FullName.Contains(".png") && File.Exists(file.FullName.Replace(".png", ".prefab")))
                        {
                            continue;
                        }
                        else if (file.FullName.Contains(".mat") && File.Exists(file.FullName.Replace(".mat", ".prefab")))
                        {
                            continue;
                        }
                    }
                    if (file.FullName.Contains("Resources/UI"))
                    {
                        AddAssetUIDependencies(AssetDatabase.LoadMainAssetAtPath(file.FullName));
                    }

                    AssetTarget target = AssetBundleUtils.Load(file);
                    if (target == null)
                        Debug.LogError(file);
                    target.exportType = AssetBundleExportType.Root;
                }
            }
        }

        protected void SaveDepAll(List<AssetTarget> all)
        {
            string path = "";
            switch (EditorUserBuildSettings.activeBuildTarget)
            {
                case BuildTarget.Android:
                    path = Path.Combine(pathResolver.AndroidBundleSavePath, pathResolver.DependFileName);
                    break;
                case BuildTarget.iOS:
                    path = Path.Combine(pathResolver.iOSBundleSavePath, pathResolver.DependFileName);
                    break;
                default:
                    path = Path.Combine(pathResolver.DefaultBundleSavePath, pathResolver.DependFileName);
                    break;
            }

            if (File.Exists(path))
                File.Delete(path);

            List<AssetTarget> exportList = new List<AssetTarget>();
            for (int i = 0; i < all.Count; i++)
            {
                AssetTarget target = all[i];
                if (target.needSelfExport)
                    exportList.Add(target);
            }
            AssetBundleDataWriter writer = dataWriter;
            writer.Save(path, exportList.ToArray());
        }

        public void SetDataWriter(AssetBundleDataWriter w)
        {
            this.dataWriter = w;
        }

        /// <summary>
        /// 删除未使用的AB，可能是上次打包出来的，而这一次没生成的
        /// </summary>
        /// <param name="all"></param>
        protected void RemoveUnused(List<AssetTarget> all)
        {
            HashSet<string> usedSet = new HashSet<string>();
            for (int i = 0; i < all.Count; i++)
            {
                AssetTarget target = all[i];
                if (target.needSelfExport)
                    usedSet.Add(target.bundleName);
            }

            DirectoryInfo di;
            switch(EditorUserBuildSettings.activeBuildTarget)
            {
                case BuildTarget.Android:
                    di = new DirectoryInfo(pathResolver.AndroidBundleSavePath);
                    break;
                case BuildTarget.iOS:
                    di = new DirectoryInfo(pathResolver.iOSBundleSavePath);
                    break;
                default:
                    di = new DirectoryInfo(pathResolver.DefaultBundleSavePath);
                    break;
            }

            FileInfo[] abFiles = di.GetFiles("*.ab");
            for (int i = 0; i < abFiles.Length; i++)
            {
                FileInfo fi = abFiles[i];
                if (usedSet.Add(fi.Name))
                {
                    Debug.Log("Remove unused AB : " + fi.Name);

                    fi.Delete();
                    //for U5X
                    File.Delete(fi.FullName + ".manifest");
                }
            }
#if Jenkins
            FileInfo[] manis = di.GetFiles("*.ab.manifest");
            for(int i=0,max= manis.Length;i<max;i++)
            {
                File.Delete(manis[i].FullName);
            }
#endif
        }
    }
}