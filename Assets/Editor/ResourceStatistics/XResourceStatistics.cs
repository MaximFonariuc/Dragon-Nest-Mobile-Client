using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using System.Diagnostics;
using System.Text;
using System.Reflection;

public class XResourceStatistics: MonoBehaviour
{
    //[MenuItem(@"Assets/Do Resource Statistics")]
    //static void AddPathtoStatistics()
    //{
    //    string path = GetSelectPath();
    //    if (path == null)
    //        return;

    //    UnityEngine.Debug.Log("Selected Path: " + path);

    //    XResourceStatisticsEditor window = XResourceStatisticsEditor.GetWindow();
    //    window.AddKeyWord(path);
    //    window.AnalyseOldLog();
    //}

    static string GetSelectPath()
    {
        Type projectBrowserType = Type.GetType("UnityEditor.ProjectBrowser,UnityEditor");
        if (projectBrowserType == null)
        {
            UnityEngine.Debug.LogError("Can't find UnityEditor.ProjectBrowser type!");
            return null;
        }

        FieldInfo lastProjectBrowser = projectBrowserType.GetField("s_LastInteractedProjectBrowser", BindingFlags.Static | BindingFlags.Public);
        if (lastProjectBrowser == null)
        {
            UnityEngine.Debug.LogError("Can't find s_LastInteractedProjectBrowser field!");
            return null;
        }

        object lastProjectBrowserInstance = lastProjectBrowser.GetValue(null);
        FieldInfo projectBrowserViewMode = projectBrowserType.GetField("m_ViewMode", BindingFlags.Instance | BindingFlags.NonPublic);
        if (projectBrowserViewMode == null)
        {
            UnityEngine.Debug.LogError("Can't find m_ViewMode field!");
            return null;
        }

        // 0 - one column, 1 - two column
        int viewMode = (int)projectBrowserViewMode.GetValue(lastProjectBrowserInstance);
        if (viewMode == 1)
        {
            MethodInfo getTreeViewFolderSelection = projectBrowserType.GetMethod("GetTreeViewFolderSelection", BindingFlags.NonPublic | BindingFlags.Static);
            if (getTreeViewFolderSelection == null)
            {
                UnityEngine.Debug.LogError("Can't find getTreeViewFolderSelection method!");
                return null;
            }
            object o = getTreeViewFolderSelection.Invoke(null, null);
            int[] selectedIndex = o as int[];
            if (selectedIndex.Length == 0)
                return AssetDatabase.GetAssetPath(Selection.activeObject);

            //UnityEngine.Debug.Log(selectedIndex[0]);
            return AssetDatabase.GetAssetPath(selectedIndex[0]);
        }
        else
        {
            return AssetDatabase.GetAssetPath(Selection.activeObject);
        }
            //FieldInfo treeView = projectBrowserType.GetField("m_FolderTree", BindingFlags.NonPublic | BindingFlags.Instance);
            //if (treeView == null)
            //{
            //    UnityEngine.Debug.LogError("Can't find treeView field!");
            //    return null;
            //}
            //Type treeViewType = Type.GetType("UnityEditor.TreeView,UnityEditor");
            //if (treeViewType == null)
            //{
            //    UnityEngine.Debug.LogError("Can't find UnityEditor.treeViewType type!");
            //    return null;
            //}
            //MethodInfo findNode = treeViewType.GetMethod("FindNode", BindingFlags.Public | BindingFlags.Instance);
            //if (findNode == null)
            //{
            //    UnityEngine.Debug.LogError("Can't find FindNode method!");
            //    return null;
            //}
            //UnityEngine.Debug.Log(AssetDatabase.GetAssetPath(selectedIndex[0]));
            //object selectedTreeViewItem = findNode.Invoke(treeView, new object[] { 1 });

            //Type treeViewItemType = Type.GetType("UnityEditor.TreeViewItem,UnityEditor");
            //if (treeViewItemType == null)
            //{
            //    UnityEngine.Debug.LogError("Can't find UnityEditor.treeViewItemType type!");
            //    return null;
            //}
            //PropertyInfo idProperty = treeViewItemType.GetProperty("id");
            //if (idProperty == null)
            //{
            //    UnityEngine.Debug.LogError("Can't find idProperty method!");
            //    return null;
            //}
            //int instanceID = (int)idProperty.GetGetMethod().Invoke(selectedTreeViewItem, null);
            //UnityEngine.Debug.Log("InstanceID: " + instanceID);
        //return null;
     }
}

public class XResourceStatisticsEditor : EditorWindow 
{
    public class StatisticResult
    {
        public string[] m_Datas = new string[MAX_KEYWORDS_COUNT];
        public string m_Version;
        public bool m_bValid = false;   // false if current result is not of the current input
    }

    public readonly static int MAX_KEYWORDS_COUNT = 5;
    string[] m_KeyWords = new string[MAX_KEYWORDS_COUNT];
    string[] m_PreKeyWords = new string[MAX_KEYWORDS_COUNT];

    StatisticResult m_Result = new StatisticResult();

    bool m_bUseRegular = false;

    [MenuItem(@"XBuild/ResourceStatistics")]
    public static XResourceStatisticsEditor GetWindow()
    {
        XResourceStatisticsEditor window = EditorWindow.GetWindow(typeof(XResourceStatisticsEditor)) as XResourceStatisticsEditor;
        return window;
        //window._FirstInit();
    }

    XResourceStatisticsEditor()
    {
        this.minSize = new UnityEngine.Vector2(450, 400);
        _FirstInit();
    }

    void _FirstInit()
    {
        m_KeyWords[0] = "Resources/Effects/";
        m_KeyWords[1] = "Resources/Animation/";
        m_KeyWords[2] = "atlas/UI";
        m_KeyWords[3] = "XScene";
        m_KeyWords[4] = "";
    }
    public static string ParseResult(float kb)
    {
        if (kb > 1024f)
        {
            kb = kb / 1024f;
            return String.Format("{0:F} MB", kb);
        }
        return String.Format("{0:F} KB", kb);
    }

    void OnGUI()
    {
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Key Words: ");
        EditorGUILayout.EndHorizontal();
        for (int i = 0; i < MAX_KEYWORDS_COUNT; ++i)
        {
            EditorGUILayout.BeginHorizontal();
            m_KeyWords[i] = GUILayout.TextField(m_KeyWords[i], GUILayout.MaxWidth(180));
            EditorGUILayout.EndHorizontal();
        }

        GUILayout.Space(18f);

        m_bUseRegular = GUILayout.Toggle(m_bUseRegular, "使用正则表达式");
        if (GUILayout.Button("分析最新的日志", GUILayout.MaxWidth(160)))
        {
            _AnalyseNewLog();
        }

        if (GUILayout.Button("分析缓存的日志", GUILayout.MaxWidth(160), GUILayout.MinHeight(35)))
        {
            _AnalyseOldLog();
        }

        GUILayout.Space(18f);

        EditorGUILayout.BeginVertical();

        if (GUILayout.Button("打开缓存日志文件夹", GUILayout.MaxWidth(160)))
        {
            if (System.Environment.OSVersion.Platform != PlatformID.Unix)
            {
                string s = "/select," + PreservedLogPath.Replace("/", "\\");
                System.Diagnostics.Process.Start("Explorer.exe", s);
            }
        }
        EditorGUILayout.EndVertical();

        GUILayout.Space(18f);

        if (m_Result.m_bValid)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(m_Result.m_Version);
            EditorGUILayout.EndHorizontal();

            GUILayout.Space(18f);

            //foreach (KeyValuePair<string, string> keyvalue in m_Result)
            for (int i = 0; i < MAX_KEYWORDS_COUNT; ++i)
            {
                if (m_Result.m_Datas[i] == null)
                    continue;
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(m_PreKeyWords[i]);
                GUILayout.Space(18f);
                EditorGUILayout.LabelField(m_Result.m_Datas[i]);
                EditorGUILayout.EndHorizontal();
            }
        }
    }

    public void AddKeyWord(string s, int index = 999999)
    {
        index = Mathf.Clamp(index, 0, MAX_KEYWORDS_COUNT - 1);
        m_KeyWords[index] = s;
    }

    public void AnalyseOldLog()
    {
        _AnalyseOldLog();
    }

    void _AnalyseNewLog()
    {
        Analyse(m_KeyWords, "default", m_bUseRegular, ref m_Result);
        _PreserveKeyWords();
        if (m_Result != null)
        {
            //System.IO.File.Copy(DefaultLogPath, PreservedLogPath, true);
            UnityEngine.Debug.Log("Latest log is preserved.");
        }
        else
        {
            UnityEngine.Debug.Log("Latest log has no valid information.");
        }
    }
    void _AnalyseOldLog()
    {
        Analyse(m_KeyWords, PreservedLogPath, m_bUseRegular, ref m_Result);
        _PreserveKeyWords();
    }

    void _PreserveKeyWords()
    {
        for (int i = 0; i < MAX_KEYWORDS_COUNT; ++i)
            m_PreKeyWords[i] = m_KeyWords[i];
    }
    static string PreservedLogPath
    {
        get { return Application.dataPath + "/Editor/ResourceStatistics/Editor.log"; }
    }

    static string DefaultLogPath
    {
        get
        {
            if (System.Environment.OSVersion.Platform == PlatformID.Unix)
            {
                return "/Users/liming/Library/Logs/Unity/Editor.log";
            }
            else
            {
                return System.Environment.GetEnvironmentVariable("HOMEDRIVE") + System.Environment.GetEnvironmentVariable("HOMEPATH") + "\\AppData\\Local\\Unity\\Editor\\Editor.log";
            }
        }
    }
    public static void Analyse(string[] keywords, string logPath, bool bUseRegular, ref StatisticResult result)
    {
        Process p = new Process();

        //if (System.Environment.OSVersion.Platform == PlatformID.Unix)
        //{
        //    //System.Diagnostics.Process.Start("/Applications/Utilities/Terminal.app/Contents/MacOS/Terminal",
        //    //    "~/gitpull.sh");
        //}
        //else
        {
            p.StartInfo.FileName = @"python";
            StringBuilder sb = new StringBuilder();
            sb.Append("\"" + Application.dataPath + "/Editor/ResourceStatistics/ResourceStatistics.py\" \"" + logPath + "\" " + (bUseRegular ? '1' : '0'));
            for (int i = 0;keywords != null && i < keywords.Length; ++i)
            {
                if (keywords[i] != null && keywords[i].Length > 0)
                {
                    sb.Append(' ').Append('"').Append(keywords[i]).Append('"');
                }
            }

            p.StartInfo.Arguments = sb.ToString();
            //p.StartInfo.WorkingDirectory = Environment.GetEnvironmentVariable("XResourcePath");
            p.StartInfo.CreateNoWindow = true;
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.RedirectStandardInput = true;
            p.StartInfo.RedirectStandardError = true;

            p.Start();
            string output = p.StandardOutput.ReadToEnd();
            //if (String.IsNullOrEmpty(output) == false)
            //    UnityEngine.Debug.Log(output);
            p.WaitForExit();

            Dictionary<string, string> dic = null;
            result.m_bValid = _ProcessResult(output, ref result.m_Version, ref dic);
            if(result.m_bValid)
                _SortResult(keywords, dic, ref result.m_Datas);
        }
        //return null;
    }

    private static void _SortResult(string[] keywords, Dictionary<string, string> output, ref string[] result)
    {
        for (int i = 0; i < MAX_KEYWORDS_COUNT; ++i)
        {
            if (output.ContainsKey(keywords[i]))
                result[i] = output[keywords[i]];
            else
                result[i] = null;
        }
    }
    private static bool _ProcessResult(string output, ref string version, ref Dictionary<string, string> res)
    {
        output = output.Replace("\r", "");
        if (output.Length <= 2)
        {
            UnityEngine.Debug.Log("No output.");
            return false;
        }

        string[] keyValues = output.Split(new char[] {'\n'}, StringSplitOptions.RemoveEmptyEntries);

        if (keyValues.Length < 2 || keyValues[0] != "Success")
        {
            UnityEngine.Debug.Log(output);
            return false;
        }

        version = keyValues[1];

        char[] split = new char[] {'\t'};

        if (res != null)
            res.Clear();
        else
            res = new Dictionary<string, string>();

        for (int i = 2; i < keyValues.Length; ++i)
        {
            string[] splited = keyValues[i].Split(split, StringSplitOptions.None);
            if (splited.Length != 2)
                continue;

            string key = splited[0];
            string value = splited[1];
            if (res.ContainsKey(key))
                res[key] = value;
            else
                res.Add(key, value);
        }

        return true;
    }
}
