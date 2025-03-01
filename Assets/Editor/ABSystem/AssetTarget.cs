﻿using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;
using XUtliPoolLib;
using XUpdater;

namespace ABSystem
{
    public enum AssetType
    {
        Asset,
        Builtin
    }

    public class AssetTarget : System.IComparable<AssetTarget>
    {
        /// <summary>
        /// 目标Object
        /// </summary>
        public Object asset;
        /// <summary>
        /// 文件路径
        /// </summary>
        public FileInfo file;
        /// <summary>
        /// 相对于Assets文件夹的目录
        /// </summary>
        public string assetPath;
        /// <summary>
        /// 此文件是否已导出
        /// </summary>
        public bool isExported;
        /// <summary>
        /// 素材类型
        /// </summary>
        public AssetType type = AssetType.Asset;
        /// <summary>
        /// 导出类型
        /// </summary>
        public AssetBundleExportType exportType = AssetBundleExportType.Asset;
        /// <summary>
        /// 保存地址
        /// </summary>
        public string bundleSavePath;
        /// <summary>
        /// BundleName
        /// </summary>
        public string bundleName;
        /// <summary>
        /// 短名
        /// </summary>
        public string bundleShortName;

        public int level = -1;
        public List<AssetTarget> levelList;

        //目标文件是否已改变
        private bool _isFileChanged = false;
        //是否已分析过依赖
        private bool _isAnalyzed = false;
        //依赖树是否改变（用于增量打包）
        private bool _isDepTreeChanged = false;
        //上次打包的信息（用于增量打包）
        private AssetCacheInfo _cacheInfo;
        //.meta 文件的Hash
        private string _metaHash;
        //上次打好的AB的CRC值（用于增量打包）
        private string _bundleCrc;
        //是否是新打包的
        private bool _isNewBuild;
        /// <summary>
        /// 我要依赖的项
        /// </summary>
        private HashSet<AssetTarget> _dependencies = new HashSet<AssetTarget>();
        /// <summary>
        /// 依赖我的项
        /// </summary>
        private HashSet<AssetTarget> _dependsChildren = new HashSet<AssetTarget>();

        public AssetTarget(Object o, FileInfo file, string assetPath)
        {
            this.asset = o;
            this.file = file;
            this.assetPath = assetPath;
            //this.bundleName = AssetBundleUtils.ConvertToABName(assetPath);
            //this.bundleSavePath = Path.Combine(AssetBundleUtils.pathResolver.BundleSavePath, bundleName);
            this.bundleShortName = file.Name.ToLower();
            this.bundleName = XCommon.singleton.XHash(AssetBundleUtils.ConvertToABName(assetPath)) + ".ab";
            switch (EditorUserBuildSettings.activeBuildTarget)
            {
                case UnityEditor.BuildTarget.Android:
                    this.bundleSavePath = Path.Combine(AssetBundleUtils.pathResolver.AndroidBundleSavePath, bundleName);
                    break;
                case UnityEditor.BuildTarget.iOS:
                    this.bundleSavePath = Path.Combine(AssetBundleUtils.pathResolver.iOSBundleSavePath, bundleName);
                    break;
                default:
                    this.bundleSavePath = Path.Combine(AssetBundleUtils.pathResolver.DefaultBundleSavePath, bundleName);
                    break;
            }

            _isFileChanged = true;
            _metaHash = "0";
        }

        /// <summary>
        /// Texture
        /// AudioClip
        /// Mesh
        /// Model
        /// Shader
        /// 这些类型的Asset的一配置是放在.meta中的，所以要监视它们的变化
        /// 而在5x中系统会自己处理的，不用管啦
        /// </summary>
        void LoadMetaHashIfNecessary()
        {
            bool needLoad = false;
            if (typeof(Texture).IsInstanceOfType(asset) ||
                typeof(AudioClip).IsInstanceOfType(asset) ||
                typeof(Mesh).IsInstanceOfType(asset) ||
                typeof(Shader).IsInstanceOfType(asset))
            {
                needLoad = true;
            }

            if (!needLoad)
            {
                AssetImporter importer = AssetImporter.GetAtPath(assetPath);
                needLoad = typeof(ModelImporter).IsInstanceOfType(importer);
            }

            if (needLoad)
            {
                _metaHash = AssetBundleUtils.GetFileHash(assetPath + ".meta");
            }
        }

        /// <summary>
        /// 分析引用关系
        /// </summary>
        public void Analyze()
        {
            if (_isAnalyzed) return;
            _isAnalyzed = true;

#if !UNITY_5
            LoadMetaHashIfNecessary();
#endif
            _cacheInfo = AssetBundleUtils.GetCacheInfo(assetPath);
            _isFileChanged = _cacheInfo == null || !_cacheInfo.fileHash.Equals(GetHash()) || !_cacheInfo.metaHash.Equals(_metaHash);
            if (_cacheInfo != null)
            {
                _bundleCrc = _cacheInfo.bundleCrc;
                if (_isFileChanged)
                    Debug.Log("File was changed : " + assetPath);
            }

            Object[] deps = EditorUtility.CollectDependencies(new Object[] { asset });
#if UNITY_5
            List<Object> depList = new List<Object>();
            for (int i = 0; i < deps.Length; i++)
            {
                Object o = deps[i];
                //不包含脚本对象
                if (o is MonoScript)
                    continue;

                //不包含builtin对象
                string path = AssetDatabase.GetAssetPath(o);
                if (path.StartsWith("Resources"))
                    continue;

                depList.Add(o);
            }
            deps = depList.ToArray();
#else
            //提取 resource.builtin
            for (int i = 0; i < deps.Length; i++)
            {
                Object dep = deps[i];
                string path = AssetDatabase.GetAssetPath(dep);
                if (path.StartsWith("Resources"))
                {
                    AssetTarget builtinAsset = AssetBundleUtils.Load(dep);
                    this.AddDepend(builtinAsset);
                    builtinAsset.Analyze();
                }
            }
#endif

            var res = from s in deps
                      let obj = AssetDatabase.GetAssetPath(s)
                      select obj;
            var paths = res.Distinct().ToArray();

            for (int i = 0; i < paths.Length; i++)
            {
                if (File.Exists(paths[i]) == false)
                {
                    //Debug.Log("invalid:" + paths[i]);
                    continue;
                }
                FileInfo fi = new FileInfo(paths[i]);
                AssetTarget target = AssetBundleUtils.Load(fi);
                if (target == null)
                    continue;

                this.AddDepend(target);

                target.Analyze();
            }
        }

        public void Merge()
        {
            if (this.NeedExportStandalone())
            {
                var children = new List<AssetTarget>(_dependsChildren);
                this.RemoveDependsChildren();
                foreach (AssetTarget child in children)
                {
                    child.AddDepend(this);
                }
            }
        }

        private void GetRoot(HashSet<AssetTarget> rootSet)
        {
            switch (this.exportType)
            {
                case AssetBundleExportType.Standalone:
                case AssetBundleExportType.Root:
                    rootSet.Add(this);
                    break;
                default:
                    foreach (AssetTarget item in _dependsChildren)
                    {
                        item.GetRoot(rootSet);
                    }
                    break;
            }
        }

        private bool beforeExportProcess;

        /// <summary>
        /// 在导出之前执行
        /// </summary>
        public void BeforeExport()
        {
            if (beforeExportProcess) return;
            beforeExportProcess = true;

            foreach (AssetTarget item in _dependsChildren)
            {
                item.BeforeExport();
            }

            if (this.exportType == AssetBundleExportType.Asset)
            {
                HashSet<AssetTarget> rootSet = new HashSet<AssetTarget>();
                this.GetRoot(rootSet);
                if (rootSet.Count > 1)
                    this.exportType = AssetBundleExportType.Standalone;
            }
        }

        /// <summary>
        /// 判断是否依赖树变化了
        /// 如果现在的依赖和之前的依赖不一样了则改变了，需要重新打包
        /// </summary>
        public void AnalyzeIfDepTreeChanged()
        {
            _isDepTreeChanged = false;
            if (_cacheInfo != null)
            {
                HashSet<AssetTarget> deps = new HashSet<AssetTarget>();
                GetDependencies(deps);

                if (deps.Count != _cacheInfo.depNames.Length)
                {
                    _isDepTreeChanged = true;
                }
                else
                {
                    foreach (AssetTarget dep in deps)
                    {
                        if (!ArrayUtility.Contains<string>(_cacheInfo.depNames, dep.assetPath))
                        {
                            _isDepTreeChanged = true;
                            break;
                        }
                    }
                }
            }
        }

        public void UpdateLevel(int level, List<AssetTarget> lvList)
        {
            this.level = level;
            if (level == -1 && levelList != null)
                levelList.Remove(this);
            this.levelList = lvList;
        }

        /// <summary>
        /// 获取所有依赖项
        /// </summary>
        /// <param name="list"></param>
        public void GetDependencies(HashSet<AssetTarget> list)
        {
            var ie = _dependencies.GetEnumerator();
            while (ie.MoveNext())
            {
                AssetTarget target = ie.Current;
                if (target.needSelfExport)
                {
                    list.Add(target);
                }
                else
                {
                    target.GetDependencies(list);
                }
            }
        }

        public List<AssetTarget> dependencies
        {
            get { return new List<AssetTarget>(_dependencies); }
        }

        public AssetBundleExportType compositeType
        {
            get
            {
                AssetBundleExportType type = exportType;
                if (type == AssetBundleExportType.Root && _dependsChildren.Count > 0)
                    type |= AssetBundleExportType.Asset;
                return type;
            }
        }

        public bool isNewBuild
        {
            get { return _isNewBuild; }
        }

        public string bundleCrc
        {
            get { return _bundleCrc; }
            set
            {
                _isNewBuild = value != _bundleCrc;
                if (_isNewBuild)
                {
                    Debug.Log("Export AB : " + bundleName);
                }
                _bundleCrc = value;
            }
        }

        /// <summary>
        /// 是不是需要重编
        /// </summary>
        public bool needRebuild
        {
            get
            {
                if (_isFileChanged || _isDepTreeChanged)
                    return true;

                foreach (AssetTarget child in _dependsChildren)
                {
                    if (child.needRebuild)
                        return true;
                }

                return false;
            }
        }

        /// <summary>
        /// 是不是自己的原因需要重编的，有的可能是因为被依赖项的原因需要重编
        /// </summary>
        public bool needSelfRebuild
        {
            get
            {
                if (_isFileChanged || _isDepTreeChanged)
                    return true;
                return false;
            }
        }

        /// <summary>
        /// 是不是自身的原因需要导出如指定的类型prefab等，有些情况下是因为依赖树原因需要单独导出
        /// </summary>
        public bool needSelfExport
        {
            get
            {
                if (type == AssetType.Builtin)
                    return false;

                bool v = exportType == AssetBundleExportType.Root || exportType == AssetBundleExportType.Standalone;

                return v;
            }
        }

        /// <summary>
        /// 是否需要导出
        /// </summary>
        public bool needExport
        {
            get
            {
                if (isExported)
                    return false;

                bool v = needSelfExport && needRebuild;

                return v;
            }
        }

        /// <summary>
        /// (作为AssetType.Asset时)是否需要单独导出
        /// </summary>
        /// <returns></returns>
        private bool NeedExportStandalone()
        {
            return _dependsChildren.Count > 1;
        }

        /// <summary>
        /// 增加依赖项
        /// </summary>
        /// <param name="target"></param>
        private void AddDepend(AssetTarget target)
        {
            if (target == this || this.ContainsDepend(target))
                return;

            _dependencies.Add(target);
            target.AddDependChild(this);
            this.ClearParentDepend(target);
        }

        /// <summary>
        /// 是否已经包含了这个依赖（检查子子孙孙）
        /// </summary>
        /// <param name="target"></param>
        /// <param name="recursive"></param>
        /// <returns></returns>
        private bool ContainsDepend(AssetTarget target, bool recursive = true)
        {
            if (_dependencies.Contains(target))
                return true;
            if (recursive)
            {
                var e = _dependencies.GetEnumerator();
                while (e.MoveNext())
                {
                    if (e.Current.ContainsDepend(target, true))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private void AddDependChild(AssetTarget parent)
        {
            _dependsChildren.Add(parent);
        }

        /// <summary>
        /// 我依赖了这个项，那么依赖我的项不需要直接依赖这个项了
        /// </summary>
        private void ClearParentDepend(AssetTarget target = null)
        {
            IEnumerable<AssetTarget> cols = _dependencies;
            if (target != null) cols = new AssetTarget[] { target };
            foreach (AssetTarget at in cols)
            {
                var e = _dependsChildren.GetEnumerator();
                while (e.MoveNext())
                {
                    AssetTarget dc = e.Current;
                    dc.RemoveDepend(at);
                }
            }
        }

        /// <summary>
        /// 移除依赖项
        /// </summary>
        /// <param name="target"></param>
        /// <param name="recursive"></param>
        private void RemoveDepend(AssetTarget target, bool recursive = true)
        {
            _dependencies.Remove(target);
            target._dependsChildren.Remove(this);

            //recursive
            var dcc = new HashSet<AssetTarget>(_dependsChildren);
            var e = dcc.GetEnumerator();
            while (e.MoveNext())
            {
                AssetTarget dc = e.Current;
                dc.RemoveDepend(target);
            }
        }

        private void RemoveDependsChildren()
        {
            var all = new List<AssetTarget>(_dependsChildren);
            _dependsChildren.Clear();
            foreach (AssetTarget child in all)
            {
                child._dependencies.Remove(this);
            }
        }

        /// <summary>
        /// 依赖我的项
        /// </summary>
        public List<AssetTarget> dependsChildren
        {
            get { return new List<AssetTarget>(_dependsChildren); }
        }

        int System.IComparable<AssetTarget>.CompareTo(AssetTarget other)
        {
            return other._dependsChildren.Count.CompareTo(_dependsChildren.Count);
        }

        public string GetHash()
        {
            if (type == AssetType.Builtin)
                return "0000000000";
            else
                return AssetBundleUtils.GetFileHash(file.FullName);
        }

#if UNITY_4_7
        public void BuildBundle(BuildAssetBundleOptions options)
        {
            string savePath = Path.Combine(Path.GetTempPath(), bundleName);

            this.isExported = true;

            var children = dependencies;

            int isAnim = 0;
            if (asset is AnimationClip)
            {

                StreamWriter sw;

                sw = new StreamWriter(assetPath.Replace(".anim", ".txt"));
                sw.WriteLine((asset as AnimationClip).length);

                sw.Flush();
                sw.Close();
                isAnim = 1;
                AssetDatabase.ImportAsset(assetPath.Replace(".anim", ".txt"));
            }

            Object[] assets = new Object[children.Count + 1 + isAnim];
            assets[0] = asset;

            for (int i = 0; i < children.Count; i++)
            {
                assets[i + 1] = children[i].asset;
            }

            if (asset is AnimationClip)
            {
                assets[children.Count + 1] = AssetDatabase.LoadAssetAtPath(assetPath.Replace(".anim", ".txt"), typeof(TextAsset));
            }

            uint crc = 0;
            if (file.Extension.EndsWith("unity"))
            {
                //string scenePath = file.FullName.Replace("\\", "/");
                //int index = scenePath.IndexOf("Assets/XScene");
                //if (index >= 0)
                //{
                //    scenePath = scenePath.Substring(index);
                //    XEditor.AssetModify.RemoveLightmapBakeThing(scenePath);
                //}
                BuildPipeline.BuildStreamedSceneAssetBundle(
                    new string[] { file.FullName },
                    savePath,
                    EditorUserBuildSettings.activeBuildTarget,
                    out crc,
                    BuildOptions.UncompressedAssetBundle);
            }
            else
            {
                BuildPipeline.BuildAssetBundle(
                    asset,
                    assets,
                    savePath,
                    out crc,
                    options,
                    EditorUserBuildSettings.activeBuildTarget);
            }

            bundleCrc = crc.ToString();

            if (_isNewBuild)
            {
                if (AssetBundleBuildPanel.ABUpdateOpen)
                {
                    string downloadPath = Path.Combine(Path.Combine(AssetBundleBuildPanel.TargetFolder, "AssetBundles"), bundleName);
                    File.Copy(savePath, downloadPath, true);
                    XMetaResPackage pack = new XMetaResPackage();
                    pack.buildinpath = asset.name;
                    pack.download = string.Format("AssetBundles/{0}", bundleName);
                    pack.Size = (uint)(new FileInfo(downloadPath)).Length;
                    if (pack.Size != (new FileInfo(downloadPath)).Length)
                    {
                        EditorUtility.DisplayDialog("Bundle ", asset.name + " is lager than UINTMAX!!!", "OK");
                    }
                    AssetBundleBuildPanel.assetBundleUpdate.Add(pack);
                }
                File.Copy(savePath, bundleSavePath, true);
            }
        }
#endif

        public void WriteCache(StreamWriter sw)
        {
            sw.WriteLine(this.assetPath);
            sw.WriteLine(GetHash());
            sw.WriteLine(_metaHash);
            sw.WriteLine(this._bundleCrc);
            HashSet<AssetTarget> deps = new HashSet<AssetTarget>();
            this.GetDependencies(deps);
            sw.WriteLine(deps.Count.ToString());
            foreach (AssetTarget at in deps)
            {
                sw.WriteLine(at.assetPath);
            }
        }
    }
}
