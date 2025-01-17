using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System.Collections;
using XUpdater;
using System.Text;
using System.IO;
using System.Collections.Generic;
using XUtliPoolLib;

public class XGUIPresent : MonoBehaviour 
{
    [MenuItem(@"XAssetBundle/Build AssetBundle For X...")]
    static void Execute()
    {
        EditorWindow.GetWindowWithRect<XBundlePresent>(new Rect(0, 0, 1360, 620), true, @"XBundle Present");
    }
}

[ExecuteInEditMode]
internal class XBundlePresent : EditorWindow
{
    private bool _enabled = false;

    private Vector2 _scrollPos;

    private GUIStyle _labelstyle = null;
    private GUIStyle _labelstyle_1 = null;
    private GUIStyle _labelstyle_2 = null;
    private GUIStyle _style = null;

    private bool _res_foldOut;
    private bool _ui_foldOut;
    private bool _scene_foldOut;
    private bool _fmod_foldOut;
    private bool _otr_foldOut;

    private static AssetLevel _level = AssetLevel.Memory;
    private GUILayoutOption[] _line = new GUILayoutOption[] { GUILayout.ExpandWidth(true), GUILayout.Height(1) };

    private Dictionary<string, bool> _res_folds = new Dictionary<string, bool>();
    private Dictionary<string, bool> _ui_folds = new Dictionary<string, bool>();
    private Dictionary<string, bool> _scene_folds = new Dictionary<string, bool>();
    private Dictionary<string, bool> _fmod_folds = new Dictionary<string, bool>();
    private List<string> _temp_folds = new List<string>();
    private HashSet<uint> _uniqe = new HashSet<uint>();

    public static AssetLevel Level { get { return _level; } }

    void OnEnable()
    {
        if (!XBundleTools.singleton.OnInit())
        {
            _enabled = false;
        }
        else
            _enabled = true;

        _res_folds.Clear();
        _ui_folds.Clear();
        _scene_folds.Clear();
        _fmod_folds.Clear();
    }

    void Update()
    {
        if (!_enabled) Close();
    }

    void OnGUI()
    {
        if (_labelstyle == null)
        {
            _labelstyle = new GUIStyle(EditorStyles.boldLabel);
            _labelstyle.fontStyle = FontStyle.BoldAndItalic;
            _labelstyle.fontSize = 13;
        }

        if (_labelstyle_1 == null)
        {
            _labelstyle_1 = new GUIStyle(EditorStyles.boldLabel);
            _labelstyle_1.fontStyle = FontStyle.BoldAndItalic;
            _labelstyle_1.fontSize = 11;
        }

        if (_labelstyle_2 == null)
        {
            _labelstyle_2 = new GUIStyle(EditorStyles.textField);
            _labelstyle_2.fontStyle = FontStyle.BoldAndItalic;
            _labelstyle_2.fontSize = 11;
        }

        if (_style == null) _style = new GUIStyle(GUI.skin.GetStyle("Label"));
        _style.alignment = TextAnchor.UpperRight;

        _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos, false, false);

        EditorGUILayout.Space();
        string platform = "Target Platform : ";
        switch (EditorUserBuildSettings.activeBuildTarget)
        {
            case UnityEditor.BuildTarget.Android: platform += "Android"; break;
            case UnityEditor.BuildTarget.iOS: platform += "IOS"; break;
        }
        EditorGUILayout.LabelField(platform, _labelstyle, new GUILayoutOption[] { GUILayout.Height(25) });
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Version: ", _labelstyle_1, new GUILayoutOption[] { GUILayout.Width(90) });
        Color r = GUI.color;
        GUI.color = new Color(255, 0, 0);
        GUILayout.Label(XBundleTools.singleton.Version, _labelstyle_1);
        GUI.color = r;
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Next Version: ", _labelstyle_1, new GUILayoutOption[] { GUILayout.Width(90) });
        r = GUI.color;
        GUI.color = new Color(255, 0, 0);
        GUILayout.Label(XBundleTools.singleton.Version_Next, _labelstyle_1, new GUILayoutOption[] { GUILayout.Width(90) });
        GUI.color = r;

        GUILayout.Label("Manifest : ", _labelstyle_1, new GUILayoutOption[] { GUILayout.Width(90) });
        r = GUI.color;
        GUI.color = new Color(255, 0, 0);
        GUILayout.Label(XBundleTools.singleton.Manifest.ToString(), _labelstyle_1);
        GUI.color = r;
        if (!XBundleTools.singleton.CurrentVersion.HasRCVersion)
        {
            if (GUILayout.Button("Extract Stable Branch", new GUILayoutOption[] { GUILayout.Width(150) }))
            {
                if (EditorUtility.DisplayDialog("Notice", "Extract stable branch for v" + XBundleTools.singleton.Version, "OK", "Cancel"))
                {
                    string ios = ASCIIEncoding.ASCII.GetString(File.ReadAllBytes("Assets/Resources/ios-version.bytes"));
                    string android = ASCIIEncoding.ASCII.GetString(File.ReadAllBytes("Assets/Resources/android-version.bytes"));

                    XGitExtractor.CreateStableBranch(ios + ".x" + android + ".x");

                    XBundleTools.singleton.CurrentVersion.RC();

                    XBundleBuilder.WriteVersion(
                        XBundleTools.singleton.Manifest,
                        EditorUserBuildSettings.activeBuildTarget,
                        XBundleTools.singleton.CurrentVersion);

                    XBundleTools.singleton.LoadManifest();
                    XBundleTools.singleton.UpdateVersion(XBundleTools.singleton.Version);

                    XGitExtractor.Push(XBundleTools.singleton.Version);
                    XGitExtractor.TagSrc(XBundleTools.singleton.Version);

                    XBundleTools.singleton.FetchNewlyUpdate();
                }
            }
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Build New : ", _labelstyle);
        EditorGUILayout.Space();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField(new GUIContent("Level", "Faster but take more memory"), _labelstyle_1, new GUILayoutOption[] { GUILayout.Width(120) });
        _level = (AssetLevel)EditorGUILayout.EnumPopup(_level, new GUILayoutOption[] { GUILayout.Width(250) });
        EditorGUILayout.EndHorizontal();
        GUILayout.Label("Selections", _labelstyle_1);

        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("", new GUILayoutOption[] { GUILayout.Width(400) });
        GUILayout.Label("Location", _labelstyle_1, new GUILayoutOption[] { GUILayout.Width(400) });
        GUILayout.Label("Status", _labelstyle_1, new GUILayoutOption[] { GUILayout.Width(100) });
        GUILayout.Label("Fetched", _labelstyle_1, new GUILayoutOption[] { GUILayout.Width(100) });
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        _res_foldOut = EditorGUILayout.Foldout(_res_foldOut, "Detailed Resources Updating");
        GUILayout.FlexibleSpace();
        EditorGUILayout.LabelField("Total " + XBundleTools.singleton.ResUpdateFiles.Count.ToString(), _style);
        EditorGUILayout.EndHorizontal();

        int i = 0;

        if (_res_foldOut)
        {
            GUILayout.Box("", _line);
            foreach (XBundleTools.UpdatedFile m in XBundleTools.singleton.ResUpdateFiles)
            {
                i++;

                EditorGUILayout.BeginHorizontal();

                GUILayout.Label(i + ": " + m.name, new GUILayoutOption[] { GUILayout.Width(400) });
                GUILayout.Label(m.location, new GUILayoutOption[] { GUILayout.Width(400) });
                GUILayout.Label(m.status, new GUILayoutOption[] { GUILayout.Width(100) });
                GUILayout.Label(m.fetched ? System.Text.UnicodeEncoding.Unicode.GetString(new byte[] { 0x14, 0x27 }) : "", new GUILayoutOption[] { GUILayout.Width(100) });
                EditorGUILayout.EndHorizontal();
            }
        }

        EditorGUILayout.Space();
        EditorGUILayout.BeginHorizontal();
        _ui_foldOut = EditorGUILayout.Foldout(_ui_foldOut, "Detailed AB Updating");
        GUILayout.FlexibleSpace();
        EditorGUILayout.LabelField("Total " + XBundleTools.singleton.ABUpdateFiles.Count.ToString(), _style);
        EditorGUILayout.EndHorizontal();
        if (_ui_foldOut)
        {
            GUILayout.Box("", _line);
            foreach (XBundleTools.UpdatedFile m in XBundleTools.singleton.ABUpdateFiles)
            {
                i++;
                r = GUI.color;

                /*if (XBundleTools.singleton.GetUis().Contains(m.location + m.name))
                {
                    m.fetched = true;
                }
                else*/
                {
                    //GUI.color = new Color(255, 0, 0);
                    m.fetched = false;
                }

                EditorGUILayout.BeginHorizontal();

                GUILayout.Label(i + ": " + m.name, new GUILayoutOption[] { GUILayout.Width(400) });
                GUILayout.Label(m.location, new GUILayoutOption[] { GUILayout.Width(400) });
                GUILayout.Label(m.status, new GUILayoutOption[] { GUILayout.Width(100) });
                GUILayout.Label(m.fetched ? System.Text.UnicodeEncoding.Unicode.GetString(new byte[] { 0x13, 0x27 }) : "", new GUILayoutOption[] { GUILayout.Width(100) });
                EditorGUILayout.EndHorizontal();

                GUI.color = r;
            }
        }

        EditorGUILayout.Space();
        EditorGUILayout.BeginHorizontal();
        _scene_foldOut = EditorGUILayout.Foldout(_scene_foldOut, "Detailed Scene Meta Updating");
        GUILayout.FlexibleSpace();
        EditorGUILayout.LabelField("Total " + XBundleTools.singleton.SceUpdateFiles.Count.ToString(), _style);
        EditorGUILayout.EndHorizontal();
        if (_scene_foldOut)
        {
            GUILayout.Box("", _line);
            foreach (XBundleTools.UpdatedFile m in XBundleTools.singleton.SceUpdateFiles)
            {
                i++;
                r = GUI.color;

                /*if (XBundleTools.singleton.GetUis().Contains(m.location + m.name))
                {
                    m.fetched = true;
                }
                else*/
                {
                    //GUI.color = new Color(255, 0, 0);
                    m.fetched = false;
                }

                EditorGUILayout.BeginHorizontal();

                GUILayout.Label(i + ": " + m.name, new GUILayoutOption[] { GUILayout.Width(400) });
                GUILayout.Label(m.location, new GUILayoutOption[] { GUILayout.Width(400) });
                GUILayout.Label(m.status, new GUILayoutOption[] { GUILayout.Width(100) });
                GUILayout.Label(m.fetched ? System.Text.UnicodeEncoding.Unicode.GetString(new byte[] { 0x13, 0x27 }) : "", new GUILayoutOption[] { GUILayout.Width(100) });
                EditorGUILayout.EndHorizontal();

                GUI.color = r;
            }
        }

        EditorGUILayout.Space();
        EditorGUILayout.BeginHorizontal();
        _fmod_foldOut = EditorGUILayout.Foldout(_fmod_foldOut, "Detailed FMOD Meta Updating");
        GUILayout.FlexibleSpace();
        EditorGUILayout.LabelField("Total " + XBundleTools.singleton.FMODUpdateFiles.Count.ToString(), _style);
        EditorGUILayout.EndHorizontal();
        if (_fmod_foldOut)
        {
            GUILayout.Box("", _line);
            foreach (XBundleTools.UpdatedFile m in XBundleTools.singleton.FMODUpdateFiles)
            {
                i++;
                r = GUI.color;

                /*if (XBundleTools.singleton.GetUis().Contains(m.location + m.name))
                {
                    m.fetched = true;
                }
                else*/
                {
                    //GUI.color = new Color(255, 0, 0);
                    m.fetched = false;
                }

                EditorGUILayout.BeginHorizontal();

                GUILayout.Label(i + ": " + m.name, new GUILayoutOption[] { GUILayout.Width(400) });
                GUILayout.Label(m.location, new GUILayoutOption[] { GUILayout.Width(400) });
                GUILayout.Label(m.status, new GUILayoutOption[] { GUILayout.Width(100) });
                GUILayout.Label(m.fetched ? System.Text.UnicodeEncoding.Unicode.GetString(new byte[] { 0x13, 0x27 }) : "", new GUILayoutOption[] { GUILayout.Width(100) });
                EditorGUILayout.EndHorizontal();

                GUI.color = r;
            }
        }

        EditorGUILayout.Space();
        EditorGUILayout.BeginHorizontal();
        _otr_foldOut = EditorGUILayout.Foldout(_otr_foldOut, "Detailed Other res Updating");
        GUILayout.FlexibleSpace();
        EditorGUILayout.LabelField("Total " + XBundleTools.singleton.OtrUpdateFiles.Count.ToString(), _style);
        EditorGUILayout.EndHorizontal();
        if (_otr_foldOut)
        {
            GUILayout.Box("", _line);
            foreach (XBundleTools.UpdatedFile m in XBundleTools.singleton.OtrUpdateFiles)
            {
                i++;

                EditorGUILayout.BeginHorizontal();

                GUILayout.Label(i + ": " + m.name, new GUILayoutOption[] { GUILayout.Width(400) });
                GUILayout.Label(m.location, new GUILayoutOption[] { GUILayout.Width(400) });
                GUILayout.Label(m.status, new GUILayoutOption[] { GUILayout.Width(100) });
                GUILayout.Label(m.fetched ? System.Text.UnicodeEncoding.Unicode.GetString(new byte[] { 0x13, 0x27 }) : "", new GUILayoutOption[] { GUILayout.Width(100) });
                EditorGUILayout.EndHorizontal();
            }
        }

        EditorGUILayout.Space();
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("", new GUILayoutOption[] { GUILayout.Width(585) });
        if (GUILayout.Button("Build Bundle", new GUILayoutOption[] { GUILayout.Width(150) }))
        {
            if (EditorUtility.DisplayDialog("Notice", "Build version to v" + XBundleTools.singleton.Version_Next + " ?", "OK", "Cancel"))
            {
                EditorApplication.delayCall = DelayCallforBuildBundle;
            }
        }

        EditorGUILayout.EndHorizontal();
        EditorGUILayout.Space();

        EditorGUILayout.LabelField("Bundle Detail :", _labelstyle);
        EditorGUILayout.Space();
        GUILayout.BeginHorizontal();
        GUILayout.Label("", new GUILayoutOption[] { GUILayout.Width(150) });
        GUILayout.Label("MD5", _labelstyle_1, new GUILayoutOption[] { GUILayout.Width(550) });
        GUILayout.Label("Size", _labelstyle_1, new GUILayoutOption[] { GUILayout.Width(170) });
        GUILayout.Label(new GUIContent("Level", "faster but take more memory"), _labelstyle_1, new GUILayoutOption[] { GUILayout.Width(80) });
        GUILayout.EndHorizontal();
        foreach (XBundleData m in XBundleTools.singleton.Manifest.Bundles)
        {
            XVersionBundleDrawer(m);
        }
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Res Detail :", _labelstyle);
        EditorGUILayout.Space();
        GUILayout.BeginHorizontal();
        GUILayout.Label("", new GUILayoutOption[] { GUILayout.Width(500) });
        GUILayout.Label("Location", _labelstyle_1, new GUILayoutOption[] { GUILayout.Width(420) });
        GUILayout.Label("Version", _labelstyle_1, new GUILayoutOption[] { GUILayout.Width(70) });
        GUILayout.Label(new GUIContent("Type", "faster but take more memory"), _labelstyle_1, new GUILayoutOption[] { GUILayout.Width(80) });
        GUILayout.Label("Object Type", _labelstyle_1, new GUILayoutOption[] { GUILayout.Width(250) });
        GUILayout.EndHorizontal();
        i = 0;
        _temp_folds.Clear();
        _uniqe.Clear();
        foreach (XResPackage m in XBundleTools.singleton.Manifest.Res)
        {
            if (!_temp_folds.Contains(m.bundle))
            {
                _temp_folds.Add(m.bundle);

                if (!_res_folds.ContainsKey(m.bundle)) _res_folds.Add(m.bundle, false);
                _res_folds[m.bundle] = EditorGUILayout.Foldout(_res_folds[m.bundle], m.bundle);
            }

            i++;

            string name = m.location.Substring(m.location.LastIndexOf('/') + 1);
            uint hash = XCommon.singleton.XHash(name + m.type);

            if (_uniqe.Contains(hash))
            {
                EditorUtility.DisplayDialog("Error", "Duplicate Bundle Name and Type: " + name, "OK");
                Close();
                return;
            }
            else if (_res_folds[m.bundle]) XVersionResDrawer(i, m);

            _uniqe.Add(hash);
        }
        GUILayout.Label("AB Res", _labelstyle_1);
        EditorGUILayout.Space();
        _temp_folds.Clear();
        foreach (XMetaResPackage m in XBundleTools.singleton.Manifest.AB)
        {
            if (!_temp_folds.Contains(m.bundle))
            {
                _temp_folds.Add(m.bundle);

                if (!_ui_folds.ContainsKey(m.bundle)) _ui_folds.Add(m.bundle, false);
                _ui_folds[m.bundle] = EditorGUILayout.Foldout(_ui_folds[m.bundle], m.bundle);
            }

            i++;
            if (_ui_folds[m.bundle]) XVersionMetaDrawer(i, m);
        }
        GUILayout.Label("Scene Res", _labelstyle_1);
        EditorGUILayout.Space();
        GUILayout.BeginHorizontal();
        GUILayout.Label("", new GUILayoutOption[] { GUILayout.Width(500) });
        GUILayout.Label("Location", _labelstyle_1, new GUILayoutOption[] { GUILayout.Width(420) });
        GUILayout.Label("Size", _labelstyle_1, new GUILayoutOption[] { GUILayout.Width(70) });
        GUILayout.Label("Version", _labelstyle_1, new GUILayoutOption[] { GUILayout.Width(70) });
        GUILayout.Label(new GUIContent("Type", "faster but take more memory"), _labelstyle_1, new GUILayoutOption[] { GUILayout.Width(80) });
        GUILayout.Label("Object Type", _labelstyle_1, new GUILayoutOption[] { GUILayout.Width(100) });
        GUILayout.EndHorizontal();
        _temp_folds.Clear();
        foreach (XMetaResPackage m in XBundleTools.singleton.Manifest.Scene)
        {
            if (!_temp_folds.Contains(m.bundle))
            {
                _temp_folds.Add(m.bundle);

                if (!_scene_folds.ContainsKey(m.bundle)) _scene_folds.Add(m.bundle, false);
                _scene_folds[m.bundle] = EditorGUILayout.Foldout(_scene_folds[m.bundle], m.bundle);
            }

            i++;
            if (_scene_folds[m.bundle]) XVersionMetaDrawer(i, m);
        }
        GUILayout.Label("FMOD Res", _labelstyle_1);
        EditorGUILayout.Space();
        _temp_folds.Clear();
        foreach (XMetaResPackage m in XBundleTools.singleton.Manifest.FMOD)
        {
            if (!_temp_folds.Contains(m.bundle))
            {
                _temp_folds.Add(m.bundle);

                if (!_fmod_folds.ContainsKey(m.bundle)) _fmod_folds.Add(m.bundle, false);
                _fmod_folds[m.bundle] = EditorGUILayout.Foldout(_fmod_folds[m.bundle], m.bundle);
            }

            i++;
            if (_fmod_folds[m.bundle]) XVersionMetaDrawer(i, m);
        }

        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("", new GUILayoutOption[] { GUILayout.Width(585) });
        if (GUILayout.Button("Push", new GUILayoutOption[] { GUILayout.Width(150) }))
        {
            if (XBundleTools.singleton.Manifest.ToString() != XBundleTools.singleton.Version_Next)
            {
                EditorUtility.DisplayDialog("Error", "Bundle Version not match with manifest!", "OK");
            }
            else if (EditorUtility.DisplayDialog("Notice", "Confirm your push: next version is v" + XBundleTools.singleton.Version_Next, "OK", "Cancel"))
            {
                string next = XBundleTools.singleton.Version_Next;
                XBundleTools.singleton.UpdateVersion(XBundleTools.singleton.Version_Next);
                XGitExtractor.Push(next);
                XGitExtractor.TagSrc(next);

                XBundleTools.singleton.FetchNewlyUpdate();
            }
        }
        GUILayout.EndHorizontal();
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.EndScrollView();
    }

    void DelayCallforBuildBundle()
    {
        bool res = false;

        if (XBundleTools.singleton.Rebuild)
            res = XVersionMgr.BuildRePublished();
        else
            res = XVersionMgr.BuildHotPatch();

        if (res)
        {
            //load new manifest
            XBundleTools.singleton.LoadManifest();
        }
        else
            EditorUtility.DisplayDialog("Error", "Build bundle failed!", "OK");
    }

    void XVersionBundleDrawer(XBundleData bundle)
    {
        float size = bundle.Size;
        string s = "bytes";

        if (size > 1024)
        {
            size /= 1024.0f;
            s = "K";
        }
        if (size > 1024)
        {
            size /= 1024.0f;
            s = "M";
        }

        GUILayout.BeginHorizontal();
        GUILayout.Label(bundle.Name, new GUILayoutOption[] { GUILayout.Width(150) });
        GUILayout.Label(bundle.MD5.ToString(), new GUILayoutOption[] { GUILayout.Width(550) });
        GUILayout.Label(size.ToString("F1") + s, new GUILayoutOption[] { GUILayout.Width(170) });
        EditorGUILayout.EnumPopup(bundle.Level, new GUILayoutOption[] { GUILayout.Width(75) });
        GUILayout.EndHorizontal();
    }

    void XVersionResDrawer(int n, XResPackage res)
    {
        string name = res.location.Substring(res.location.LastIndexOf('/') + 1);
        string location = res.location.Remove(res.location.LastIndexOf('/') + 1);

        GUILayout.BeginHorizontal();
        GUILayout.Label(n + ": " + name, new GUILayoutOption[] { GUILayout.Width(500) });
        GUILayout.Label(new GUIContent(location, location), _labelstyle_1, new GUILayoutOption[] { GUILayout.Width(420) });
        GUILayout.Label(res.bundle, _labelstyle_1, new GUILayoutOption[] { GUILayout.Width(70) });
        GUILayout.Label(res.rtype.ToString(), _labelstyle_1, new GUILayoutOption[] { GUILayout.Width(80) });
        GUILayout.Label(res.type, _labelstyle_1, new GUILayoutOption[] { GUILayout.Width(250) });
        GUILayout.EndHorizontal();
    }

    void XVersionMetaDrawer(int n, XMetaResPackage meta)
    {
        string name = meta.download.Substring(meta.download.LastIndexOf('/') + 1);
        string location = meta.buildinpath;

        float size = meta.Size;
        string s = "bytes";

        if (size > 1024)
        {
            size /= 1024.0f;
            s = "K";
        }
        if (size > 1024)
        {
            size /= 1024.0f;
            s = "M";
        }

        GUILayout.BeginHorizontal();
        GUILayout.Label(n + ": " + name, new GUILayoutOption[] { GUILayout.Width(500) });
        GUILayout.Label(new GUIContent(location, location), _labelstyle_1, new GUILayoutOption[] { GUILayout.Width(420) });
        GUILayout.Label(size.ToString("F1") + s, new GUILayoutOption[] { GUILayout.Width(70) });
        GUILayout.Label(meta.bundle, _labelstyle_1, new GUILayoutOption[] { GUILayout.Width(70) });
        GUILayout.Label("Meta Assets".ToString(), _labelstyle_1, new GUILayoutOption[] { GUILayout.Width(80) });
        GUILayout.Label("NONE", _labelstyle_1, new GUILayoutOption[] { GUILayout.Width(100) });
        GUILayout.EndHorizontal();
    }
}
