using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;
using XEditor;
using XUtliPoolLib;
using System.IO;
using System;
//public enum ETextureSize
//{
//    Original,
//    Half,
//    X32,
//    X64,
//    X128,
//    X256,
//    X512,  
//}
//public enum ETextureCompress
//{
//    TrueColor,
//    Alpha8,
//    Compress,
//    Delete,
//    RGB16,
   
//}

public class XResImportEditor
{
    public static bool bAccordingSettings = true;
    public static XImageImporterSet Sets = null;

    //[MenuItem(@"XEditor/Res Import Setting.../UITexture Settings")]
    //static void Execute()
    //{
    //    EditorWindow.GetWindowWithRect<XUITextureImportEditorWnd>(new Rect(0, 0, 800, 600), true, @"XRes Import Editor");
    //}
}

public class XImageImporterSet
{
    public List<XUITextureImporterData> ImageSet = new List<XUITextureImporterData>();
    public void Init()
    {
        HashSet<string> names = new HashSet<string>();
        for (int i = ImageSet.Count - 1; i >= 0; --i)
        {
            XUITextureImporterData data = ImageSet[i];
            Texture2D tex = AssetDatabase.LoadAssetAtPath(data.path, typeof(UnityEngine.Texture2D)) as Texture2D;
            if (tex != null)
            {
                data.SetTex(tex);
                data.SetName(tex.name);
                if(names.Contains(tex.name))
                {
                    ImageSet.RemoveAt(i);
                }
                else
                {
                    names.Add(tex.name);
                }
            }
            else
            {
                string name = data.path;
                int index = data.path.LastIndexOf("/");
                if (index>=0)
                {
                    name = data.path.Substring(index + 1);                   
                }
                index = name.LastIndexOf(".");
                if (index >= 0)
                {
                    name = name.Substring(0, index);
                }
                data.SetName(name);
            }
        }
    }
    public XUITextureImporterData Find(string name)
    {
        for (int i = 0; i < ImageSet.Count; ++i)
        {
            XUITextureImporterData data = ImageSet[i];
            if (data != null && data.GetName() == name)
            {
                return data;
            }
        }
        return null;
    }
}

public class XUITextureImporterData
{
    
    public string path = "";
    public ETextureSize textureSize = ETextureSize.Original;
    public ETextureCompress textureCompress = ETextureCompress.TrueColor;
    private string name = "";
    private Texture2D tex = null;
    public string GetName()
    {
        return name;
    }
    public void SetName(string n)
    {
        name = n;
    }
    public Texture2D GetTex()
    {
        return tex;
    }
    public void SetTex(Texture2D t)
    {
        tex = t;
    }

    public int GetSize(int srcSize)
    {
        switch (textureSize)
        {
            case ETextureSize.Original:
                return srcSize;
            case ETextureSize.Half:
                return srcSize/2;
            case ETextureSize.X32:
                return 32;
            case ETextureSize.X64:
                return 64;
            case ETextureSize.X128:
                return 128;
            case ETextureSize.X256:
                return 256;
            case ETextureSize.X512:
                return 512;
        }
        return srcSize;
    }
}

//[ExecuteInEditMode]
//internal class XUITextureImportEditorWnd : EditorWindow
//{
//    private Vector2 scrollPosition = Vector2.zero;

//    private GUIStyle _labelstyle = null;
//    private GUIStyle _labelstyle_1 = null;

//    private XImageImporterSet dataSet = new XImageImporterSet();

//    void OnEnable()
//    {
//        dataSet = XDataIO<XImageImporterSet>.singleton.DeserializeData("Assets/Editor/ResImporter/ImporterData/Image/ResourceImportXML.xml");
//        if (dataSet == null)
//            dataSet = new XImageImporterSet();
//        else
//        {
//            dataSet.Init();
//        }
//    }
//    void AddNullTexture()
//    {
//        XUITextureImporterData data = new XUITextureImporterData();
//        dataSet.ImageSet.Add(data);
//    }
//    void SaveTexture()
//    {
//        XDataIO<XImageImporterSet>.singleton.SerializeData("Assets/Editor/ResImporter/ImporterData/Image/ResourceImportXML.xml", dataSet);
//    }
//    void DelTexture(XUITextureImporterData data)
//    {
//        dataSet.ImageSet.Remove(data);
//    }

//    bool DrawTexture(XUITextureImporterData data)
//    {
//        bool delete = false;
//        Texture2D tex = data.GetTex();
//        if (tex != null)
//        {
//            data.SetName(tex.name);
//        }
//        Texture2D newTex = EditorGUILayout.ObjectField(data.GetName(), tex, typeof(Texture2D), true, GUILayout.MaxWidth(250)) as Texture2D;
//        if (tex != newTex)
//        {
//            if (newTex == null || dataSet.Find(newTex.name) == null)
//            {
//                data.SetTex(newTex);
//                data.path = AssetDatabase.GetAssetPath(newTex);
//                TextureImporter textureImporter = AssetImporter.GetAtPath(data.path) as TextureImporter;
//                if (textureImporter != null)
//                {
//                    TextureImporterFormat texFormat = TextureImporterFormat.ETC_RGB4;
//                    int texSize = 0;
//                    if (textureImporter.GetPlatformTextureSettings("Android", out texSize, out texFormat))
//                    {
//                        //if (texFormat == TextureImporterFormat.Alpha8)
//                        //    data.textureCompress = ETextureCompress.Alpha8;
//                    }
//                }
//            }
//            else
//            {
//                data.SetTex(newTex);
//                data.path = AssetDatabase.GetAssetPath(newTex);
//                //EditorUtility.DisplayDialog("Finish", string.Format("texture {0} already exit!", newTex.name), "OK");
//            }
//        }
//        data.textureSize = (ETextureSize)EditorGUILayout.EnumPopup("缩放", data.textureSize);
//        data.textureCompress = (ETextureCompress)EditorGUILayout.EnumPopup("压缩", data.textureCompress);
//        if (GUILayout.Button("X"))
//        {
//            DelTexture(data);
//            delete = true;
//        }
//        return delete;
//    }

//    void OnGUI()
//    {
//        if (_labelstyle == null)
//        {
//            _labelstyle = new GUIStyle(EditorStyles.boldLabel);
//            _labelstyle.fontSize = 11;
//        }

//        if (_labelstyle_1 == null)
//        {
//            _labelstyle_1 = new GUIStyle(EditorStyles.boldLabel);
//            _labelstyle_1.fontStyle = FontStyle.BoldAndItalic;
//            _labelstyle_1.fontSize = 11;
//        }

//        EditorGUILayout.Space();
//        GUILayout.Label("Resource Importer:", _labelstyle);
//        EditorGUILayout.Space();

//        GUILayout.BeginHorizontal();

//        if (GUILayout.Button("Add", GUILayout.MaxWidth(150)))
//        {
//            AddNullTexture();
//        }
//        if (GUILayout.Button("Save", GUILayout.MaxWidth(150)))
//        {
//            SaveTexture();
//        }
//        GUILayout.EndHorizontal();
//        scrollPosition = GUILayout.BeginScrollView(scrollPosition, false, false);
//        for (int i = 0; i < dataSet.ImageSet.Count; ++i)
//        {
//            GUILayout.BeginHorizontal();
//            bool delete = DrawTexture(dataSet.ImageSet[i]);
//            GUILayout.EndHorizontal();
//            if (delete)
//                break;
//        }
//        EditorGUILayout.EndScrollView();
//    }
//}
