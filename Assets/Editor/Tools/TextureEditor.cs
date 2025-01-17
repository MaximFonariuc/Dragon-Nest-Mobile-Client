using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using System.Reflection;
using System;
using System.IO;
using XEditor;
using System.Linq;
using UnityEditor.AnimatedValues;
using UnityEngine.Events;

[CanEditMultipleObjects, CustomEditor(typeof(TextureImporter))]
public class TextureEditor : DecoratorEditor
{

    public class TexFormat
    {
        public ETextureCompress srcFormat = ETextureCompress.Compress;
        public ETextureSize alphaSize = ETextureSize.Original;
    }
    private Editor nativeEditor;
    private string path;
    private bool isUITex = false;
    private bool cannotHotfix = false;
    private bool needAlpha = false;
    private TextureImporter texImporter = null;
    private ETextureCompress srcFormat = ETextureCompress.Compress;
    private ETextureSize alphaSize = ETextureSize.Original;
    private List<TexFormat> texFormat = new List<TexFormat>();
    private GUIStyle warningStyle = null;

    public static bool IsDefaultFormat(bool isAtlas, ETextureCompress format, ETextureSize size)
    {
        if (isAtlas)
        {
            return format == ETextureCompress.Compress && size == ETextureSize.Original;
        }
        else
        {
            return format == ETextureCompress.Compress && size == ETextureSize.Half;
        }
    }

    public static bool IsAtlas(string path)
    {
        int index = path.LastIndexOf(".");
        if (index >= 0)
        {
            path = path.Substring(0, index);
        }

        Material mat = AssetDatabase.LoadAssetAtPath(path + ".mat", typeof(Material)) as Material;
        GameObject atlas = AssetDatabase.LoadAssetAtPath(path + ".prefab", typeof(GameObject)) as GameObject;
        return mat != null && atlas != null;
    }
    public TextureEditor()
        : base("TextureImporterInspector")
        { }

    public void OnEnable()
    {
        texImporter = target as TextureImporter;
        texFormat.Clear();
        if (texImporter != null)
        {
            path = texImporter.assetPath;
            cannotHotfix = path.StartsWith("Assets/Resources/StaticUI");
            isUITex = (path.StartsWith("Assets/Resources/atlas/UI") || path.StartsWith("Assets/Resources/StaticUI")) && !path.EndsWith("_A.png");
            if (isUITex)
            {
                string userData = texImporter.userData;

                AssetModify.GetTexFormat(IsAtlas(path), texImporter.userData, out srcFormat, out alphaSize);
                for (int i = 0; i < targets.Length; ++i)
                {
                    TextureImporter ti = targets[i] as TextureImporter;
                    if (ti != null)
                    {
                        needAlpha = ti.DoesSourceTextureHaveAlpha();
                        TexFormat tf = new TexFormat();
                        AssetModify.GetTexFormat(IsAtlas(ti.assetPath), ti.userData, out tf.srcFormat, out tf.alphaSize);
                        texFormat.Add(tf);
                        if (tf.srcFormat != srcFormat)
                        {
                            srcFormat = (ETextureCompress)(-1);
                        }
                        if (tf.alphaSize != alphaSize)
                        {
                            alphaSize = (ETextureSize)(-1);
                        }
                    }
                }
            }

        }

    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if (cannotHotfix)
        {
            if (warningStyle == null)
            {
                warningStyle = new GUIStyle(GUI.skin.label);
                warningStyle.fontStyle = FontStyle.Bold;
                warningStyle.normal.textColor = new Color(1, 0, 0);
                warningStyle.fontSize = 20;
            }


            GUILayout.Label("此图不能热更新!!!", warningStyle);
        }
        if (texImporter != null)
        {
            
            if (isUITex)
            {
                GUILayout.Space(20);
                ETextureCompress newSrcFormat = (ETextureCompress)EditorGUILayout.EnumPopup("压缩", srcFormat);
                ETextureSize newAlphaSize = (ETextureSize)EditorGUILayout.EnumPopup("Alpha缩放", alphaSize);
                if (newSrcFormat != srcFormat || newAlphaSize != alphaSize)
                {
                    //texImporter.userData = string.Format("{0} {1}", (int)newSrcFormat, (int)newAlphaSize);
                    if (newSrcFormat != srcFormat)
                    {
                        srcFormat = newSrcFormat;
                    }
                    if (newAlphaSize != alphaSize)
                    {
                        alphaSize = newAlphaSize;
                    }
                }
                GUILayout.Label(needAlpha ? "有alpha" : "无alpha");
                GUILayout.Space(10);
                GUILayout.BeginHorizontal();
                if (GUILayout.Button("Apply", GUILayout.MaxWidth(50)))
                {
                    int format = (int)newSrcFormat;
                    int size = (int)newAlphaSize;
                    for (int i = 0; i < targets.Length; ++i)
                    {
                        TextureImporter ti = targets[i] as TextureImporter;
                        TexFormat tf = texFormat[i];
                        if (ti != null)
                        {
                            int f = format == -1 ? (int)tf.srcFormat : format;
                            int s = size == -1 ? (int)tf.alphaSize : size;
                            ti.userData = string.Format("{0} {1}", f, s);
                            EditorUtility.DisplayProgressBar(string.Format("Processing-{0}/{1}", i, targets.Length), ti.assetPath, (float)i / targets.Length);
                            XEditor.AssetModify.CompressSeparateAlpha(ti);
                        }
                    }
                    EditorUtility.ClearProgressBar();
                    EditorUtility.DisplayDialog("Finish", "All textures processed finish", "OK");
                }
                if (GUILayout.Button("Texture Status", GUILayout.MaxWidth(100)))
                {
                    TextureStatus window = (TextureStatus)EditorWindow.GetWindow(typeof(TextureStatus), true, "TextureStatus");
                    window.Show();
                }
                GUILayout.EndHorizontal();
            }
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Split", GUILayout.MaxWidth(100)))
            {
                List<string> list = new List<string>();
                string road = Application.dataPath.Substring(0, Application.dataPath.LastIndexOf("/") + 1);
                for (int i = 0; i < Selection.objects.Length; i++)
                {
                    string path = AssetDatabase.GetAssetPath(Selection.objects[i]);
                    bool isUI = path.StartsWith("Assets/Resources/atlas/UI");
                    if (isUI)
                    {
                        string newPath = path.Replace("Assets/Resources/atlas/UI", "Assets/atlas/UI");
                        File.Copy(road + path, road + newPath);
                        list.Add(newPath);
                    }
                    else
                    {
                        list.Add(path);
                    }
                }
                AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
                Selection.activeObject = null;
                List<UnityEngine.Object> currentSelection = new List<UnityEngine.Object>();
                for (int i = 0; i < list.Count; i++)
                {
                    currentSelection.Add(AssetDatabase.LoadAssetAtPath(list[i], typeof(UnityEngine.Object)));
                }
                Selection.objects = currentSelection.ToArray();
                AssetModify.SplitTex();
            }
            GUILayout.EndHorizontal();
        }
    }

}

internal class TextureStatus : EditorWindow
{
    private Vector2 scrollPosition = Vector2.zero;

    private GUIStyle _labelstyle = null;
    private GUIStyle _labelstyle_1 = null;
    public class TexInfo
    {
        public string path = "";
        public ETextureCompress srcFormat = ETextureCompress.Compress;
        public ETextureSize alphaSize = ETextureSize.Original;
        public bool isAtlas = false;
    }
    //private GameObject _model = null;
    private static List<TexInfo> allTexStatus = new List<TexInfo>();
    private int CompareNewMsg(TexInfo a, TexInfo b)
    {
        return a.path.CompareTo(b.path);
    }



    private void RefreshTexStatus()
    {
        allTexStatus.Clear();
        string[] arrStrPath = Directory.GetFiles(Application.dataPath + "/Resources/atlas/UI/", "*.png", SearchOption.AllDirectories);
        for (int i = 0; i < arrStrPath.Length; ++i)
        {
            string strTempPath = arrStrPath[i].Replace(@"\", "/");
            strTempPath = strTempPath.Substring(strTempPath.IndexOf("Assets"));
            TextureImporter textureImporter = AssetImporter.GetAtPath(strTempPath) as TextureImporter;
            if (textureImporter != null)
            {
                ETextureCompress f = ETextureCompress.Compress;
                ETextureSize s = ETextureSize.Original;
                bool isAtlas = TextureEditor.IsAtlas(strTempPath);
                AssetModify.GetTexFormat(isAtlas, textureImporter.userData, out f, out s);

                if (!TextureEditor.IsDefaultFormat(isAtlas, f, s))
                {
                    TexInfo tf = new TexInfo();
                    tf.path = strTempPath;
                    tf.srcFormat = f;
                    tf.alphaSize = s;
                    tf.isAtlas = isAtlas;
                    allTexStatus.Add(tf);
                }
            }
        }
        allTexStatus.Sort(CompareNewMsg);
    }
    private void SetDefaultAlpha()
    {
        string[] arrStrPath = Directory.GetFiles(Application.dataPath + "/Resources/atlas/UI/", "*.png", SearchOption.AllDirectories);

        for (int i = 0; i < arrStrPath.Length; ++i)
        {
            string strTempPath = arrStrPath[i].Replace(@"\", "/");
            strTempPath = strTempPath.Substring(strTempPath.IndexOf("Assets"));
            TextureImporter textureImporter = AssetImporter.GetAtPath(strTempPath) as TextureImporter;
            if (textureImporter != null)
            {
                ETextureCompress f = ETextureCompress.Compress;
                ETextureSize s = ETextureSize.Original;
                bool isAtlas = TextureEditor.IsAtlas(strTempPath);
                AssetModify.GetTexFormat(isAtlas, textureImporter.userData, out f, out s);
                if (!isAtlas && (f != ETextureCompress.Compress || s != ETextureSize.Half))
                {
                    f = ETextureCompress.Compress;
                    s = ETextureSize.Half;
                    textureImporter.userData = string.Format("{0} {1}", (int)f, (int)s);
                    AssetDatabase.ImportAsset(textureImporter.assetPath, ImportAssetOptions.ForceUpdate);
                }
            }
            EditorUtility.DisplayProgressBar(string.Format("SetDefaultAlpha:{0}/{1}", i, arrStrPath.Length), strTempPath, (float)i / arrStrPath.Length);
        }
        EditorUtility.ClearProgressBar();
        EditorUtility.DisplayDialog("Finish", "All textures processed finish", "OK");
    }
    private void OnEnable()
    {
        RefreshTexStatus();
    }

    void OnGUI()
    {
        if (_labelstyle == null)
        {
            _labelstyle = new GUIStyle(EditorStyles.boldLabel);
            _labelstyle.fontSize = 11;
        }

        if (_labelstyle_1 == null)
        {
            _labelstyle_1 = new GUIStyle(EditorStyles.boldLabel);
            _labelstyle_1.fontStyle = FontStyle.BoldAndItalic;
            _labelstyle_1.fontSize = 11;
        }

        EditorGUILayout.Space();
        GUILayout.Label("UI Texture Stats:", _labelstyle);
        if (GUILayout.Button("SetDefault", GUILayout.MaxWidth(80)))
        {
            SetDefaultAlpha();
        }
        EditorGUILayout.Space();

        scrollPosition = GUILayout.BeginScrollView(scrollPosition, false, false);
        for (int i = 0; i < allTexStatus.Count; ++i)
        {
            TexInfo tf = allTexStatus[i];
            GUILayout.BeginHorizontal();
            GUILayout.Label(tf.path);
            EditorGUILayout.EnumPopup("压缩", tf.srcFormat);
            EditorGUILayout.EnumPopup("Alpha缩放", tf.alphaSize);
            //EditorGUILayout.ObjectField(data.GetName(), tex, typeof(Texture2D), true, GUILayout.MaxWidth(250)) as Texture2D;
            GUILayout.EndHorizontal();
        }
        EditorGUILayout.EndScrollView();
    }
}
