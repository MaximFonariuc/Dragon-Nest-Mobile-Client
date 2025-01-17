using System.Collections.Generic;
using System.IO;
using Tangzx.ABSystem;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using XUpdater;
using AssetBundlePathResolver = XUtliPoolLib.AssetBundlePathResolver;

namespace ABSystem
{
    public class AssetBundleBuildPanel : EditorWindow
    {
        [MenuItem("ABSystem/Builder Panel")]
        static void Open()
        {
            GetWindow<AssetBundleBuildPanel>("ABSystem", true);
        }

        public static void BuildAssetBundles()
        {
            AssetBundleBuildConfig config = LoadAssetAtPath<AssetBundleBuildConfig>(savePath);

            if (config == null)
                return;

			ABBuilder builder = new ABBuilder(new AssetBundlePathResolver());

            builder.SetDataWriter(config.depInfoFileFormat == AssetBundleBuildConfig.Format.Text ? new AssetBundleDataWriter() : new AssetBundleDataBinaryWriter());

            builder.Begin();

            for (int i = 0; i < config.filters.Count; i++)
            {
                AssetBundleFilter f = config.filters[i];
                if (f.valid)
                {
                    if (Directory.Exists(f.path))
                        builder.AddRootTargets(new DirectoryInfo(f.path), new string[] { f.filter });
                }
            }

            for (int i = 0; i < config.files.Count; ++i)
            {
                AssetBundleFilter f = config.files[i];
                if (f.valid)
                {
                    if (File.Exists(f.path))
                        builder.AddRootTargets(f.path);
                }
            }

            builder.Export();
            builder.End();

            EditorUtility.DisplayDialog("AssetBundle Build Finish", "AssetBundle Build Finish!", "OK");
        }

        public static bool ABUpdateOpen = false;
        public static List<XMetaResPackage> assetBundleUpdate = new List<XMetaResPackage>();
        public static string TargetFolder = "";

        public static List<XMetaResPackage> BuildAssetBundlesUpdate(List<string> rawList,string destFolder)
        {
            if (!Directory.Exists(Path.Combine(destFolder, "AssetBundles")))
                Directory.CreateDirectory(Path.Combine(destFolder, "AssetBundles"));

            ABUpdateOpen = true;
            assetBundleUpdate.Clear();
            TargetFolder = destFolder;

            AssetBundleBuildConfig config = LoadAssetAtPath<AssetBundleBuildConfig>(savePath);

            if (config == null)
            {
                ABUpdateOpen = false;
                return assetBundleUpdate;
            }
            
            ABBuilder builder = new ABBuilder(new AssetBundlePathResolver());

            builder.SetDataWriter(config.depInfoFileFormat == AssetBundleBuildConfig.Format.Text ? new AssetBundleDataWriter() : new AssetBundleDataBinaryWriter());

            builder.Begin();

            for (int i = 0; i < rawList.Count; ++i)
            {
                AssetBundleFilter filter = new AssetBundleFilter();
                filter.valid = true;
                filter.path = "Assets/" + rawList[i];

                int j = 0;
                for (j = 0; j < config.files.Count; ++j)
                {
                    if (config.files[j].path == filter.path) break;
                }
                if (j == config.files.Count)
                {
                    config.files.Add(filter);
                }
            }

            AssetBundlePathResolver.instance = new AssetBundlePathResolver();

            if (LoadAssetAtPath<AssetBundleBuildConfig>(savePath) == null)
            {
                AssetDatabase.CreateAsset(config, savePath);
            }
            else
            {
                EditorUtility.SetDirty(config);
            }

            for (int i = 0; i < config.filters.Count; i++)
            {
                AssetBundleFilter f = config.filters[i];
                if (f.valid)
                {
                    if (Directory.Exists(f.path))
                        builder.AddRootTargets(new DirectoryInfo(f.path), new string[] { f.filter });
                }
            }

            for (int i = 0; i < config.files.Count; ++i)
            {
                AssetBundleFilter f = config.files[i];
                if (f.valid)
                {
                    if (File.Exists(f.path))
                        builder.AddRootTargets(f.path);
                }
            }

            builder.Export();
            builder.End();

            if (assetBundleUpdate.Count > 0)
            {
                string downloadPath = Path.Combine(Path.Combine(AssetBundleBuildPanel.TargetFolder, "AssetBundles"), "dep.all");
                switch(EditorUserBuildSettings.activeBuildTarget)
                {
                    case UnityEditor.BuildTarget.Android:
                        File.Copy(Path.Combine(AssetBundlePathResolver.instance.AndroidBundleSavePath, AssetBundlePathResolver.instance.DependFileName), downloadPath, true);
                        break;
                    case UnityEditor.BuildTarget.iOS:
                        File.Copy(Path.Combine(AssetBundlePathResolver.instance.iOSBundleSavePath, AssetBundlePathResolver.instance.DependFileName), downloadPath, true);
                        break;
                    default:
                        File.Copy(Path.Combine(AssetBundlePathResolver.instance.DefaultBundleSavePath, AssetBundlePathResolver.instance.DependFileName), downloadPath, true);
                        break;
                }

                XMetaResPackage pack = new XMetaResPackage();
                pack.buildinpath = "ABConfig";
                pack.download = string.Format("AssetBundles/dep.all");
                pack.Size = (uint)(new FileInfo(downloadPath)).Length;
                AssetBundleBuildPanel.assetBundleUpdate.Add(pack);
            }

            ABUpdateOpen = false;
            return assetBundleUpdate;
        }

        public static T LoadAssetAtPath<T>(string path) where T:Object
		{
#if UNITY_5
			return AssetDatabase.LoadAssetAtPath<T>(savePath);
#else
			return (T)AssetDatabase.LoadAssetAtPath(savePath, typeof(T));
#endif
		}

        public static string savePath
        {
            get
            {
                switch(EditorUserBuildSettings.activeBuildTarget)
                {
                    case UnityEditor.BuildTarget.Android:
                        return "Assets/ABSystem/Android/config.asset";
                    case UnityEditor.BuildTarget.iOS:
                        return "Assets/ABSystem/iOS/config.asset";
                    default:
                        return "Assets/ABSystem/config.asset";
                }
            }
        }

        private AssetBundleBuildConfig _config;
        private ReorderableList _list;
        private ReorderableList _fileList;

        public Vector2 scrollPosition = Vector2.zero;

        AssetBundleBuildPanel()
        {

        }

        void OnListElementGUI(Rect rect, int index, bool isactive, bool isfocused)
        {
            const float GAP = 5;

            AssetBundleFilter filter = _config.filters[index];
            rect.y++;

            Rect r = rect;
            r.width = 16;
            r.height = 18;
            filter.valid = GUI.Toggle(r, filter.valid, GUIContent.none);

            r.xMin = r.xMax + GAP;
            r.xMax = rect.xMax - 300;
            GUI.enabled = false;
            filter.path = GUI.TextField(r, filter.path);
            GUI.enabled = true;

            r.xMin = r.xMax + GAP;
            r.width = 50;
            if (GUI.Button(r, "Select"))
            {
                string dataPath = Application.dataPath;
                string selectedPath = EditorUtility.OpenFolderPanel("Path", dataPath, "");
                if (!string.IsNullOrEmpty(selectedPath))
                {
                    if (selectedPath.StartsWith(dataPath))
                    {
                        filter.path = "Assets/" + selectedPath.Substring(dataPath.Length + 1);
                    }
                    else
                    {
                        ShowNotification(new GUIContent("不能在Assets目录之外!"));
                    }
                }
            }

            r.xMin = r.xMax + GAP;
            r.xMax = rect.xMax;
            filter.filter = GUI.TextField(r, filter.filter);
        }

        void OnFileListElementGUI(Rect rect, int index, bool isactive, bool isfocused)
        {
            const float GAP = 5;

            AssetBundleFilter file = _config.files[index];
            rect.y++;

            Rect r = rect;
            r.width = 16;
            r.height = 18;
            file.valid = GUI.Toggle(r, file.valid, GUIContent.none);

            r.xMin = r.xMax + GAP;
            r.xMax = rect.xMax - 300;
            GUI.enabled = false;
            file.path = GUI.TextField(r, file.path);
            GUI.enabled = true;

            r.xMin = r.xMax + GAP;
            r.width = 50;
            if (GUI.Button(r, "Select"))
            {
                string dataPath = Application.dataPath;
                string selectedPath = EditorUtility.OpenFilePanel("Path", dataPath, "");
                if (!string.IsNullOrEmpty(selectedPath))
                {
                    if (selectedPath.StartsWith(dataPath))
                    {
                        file.path = "Assets/" + selectedPath.Substring(dataPath.Length + 1);
                    }
                    else
                    {
                        ShowNotification(new GUIContent("不能在Assets目录之外!"));
                    }
                }
            }

            r.xMin = r.xMax + GAP;
            r.xMax = rect.xMax;
            file.filter = GUI.TextField(r, file.filter);
        }

        void OnListHeaderGUI(Rect rect)
        {
            EditorGUI.LabelField(rect, "Asset Filter");
        }

        void OnFileListHeaderGUI(Rect rect)
        {
            EditorGUI.LabelField(rect, "Asset File");
        }

        void OnGUI()
        {
            bool execBuild = false;
            if (_config == null)
            {
                _config = LoadAssetAtPath<AssetBundleBuildConfig>(savePath);
                if (_config == null)
                {
                    _config = new AssetBundleBuildConfig();
                }
            }

            if (_list == null)
            {
                _list = new ReorderableList(_config.filters, typeof(AssetBundleFilter));
                _list.drawElementCallback = OnListElementGUI;
                _list.drawHeaderCallback = OnListHeaderGUI;
                _list.draggable = true;
                _list.elementHeight = 22;
            }

            //tool bar
            GUILayout.BeginHorizontal(EditorStyles.toolbar);
            {
                if (GUILayout.Button("Add", EditorStyles.toolbarButton))
                {
                    _config.filters.Add(new AssetBundleFilter());
                }
                if (GUILayout.Button("Save", EditorStyles.toolbarButton))
                {
                    Save();
                }
                GUILayout.FlexibleSpace();
                if (GUILayout.Button("Build", EditorStyles.toolbarButton))
                {
                    execBuild = true;
                }
            }
            GUILayout.EndHorizontal();

            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, false, false);

            if (_fileList == null)
            {
                _fileList = new ReorderableList(_config.files, typeof(AssetBundleFilter));
                _fileList.drawElementCallback = OnFileListElementGUI;
                _fileList.drawHeaderCallback = OnFileListHeaderGUI;
                _fileList.draggable = true;
                _fileList.elementHeight = 22;
            }

            //context
            GUILayout.BeginVertical();

            //format
            GUILayout.BeginHorizontal();
            {
                EditorGUILayout.PrefixLabel("DepInfoFileFormat");
                _config.depInfoFileFormat = (AssetBundleBuildConfig.Format)EditorGUILayout.EnumPopup(_config.depInfoFileFormat);
            }
            GUILayout.EndHorizontal();

            GUILayout.Space(10);
            
            _list.DoLayoutList();
            GUILayout.EndVertical();

            GUILayout.Space(10);

            _fileList.DoLayoutList();

            EditorGUILayout.EndScrollView();

            //set dirty
            if (GUI.changed)
                EditorUtility.SetDirty(_config);

            if (execBuild)
                Build();
        }

        private void Build()
        {
            Save();
            BuildAssetBundles();
        }

        void Save()
        {
            AssetBundlePathResolver.instance = new AssetBundlePathResolver();

            if (LoadAssetAtPath<AssetBundleBuildConfig>(savePath) == null)
            {
                AssetDatabase.CreateAsset(_config, savePath);
            }
            else
            {
                EditorUtility.SetDirty(_config);
            }
        }
    }
}