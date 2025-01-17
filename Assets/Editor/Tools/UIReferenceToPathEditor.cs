using UnityEngine;
using System.Collections;
using UnityEditor;
using System.IO;
using System.Collections.Generic;
using XUtliPoolLib;

public class UIReferenceToPathEditor : EditorWindow
{
    public static bool UISpriteRefrence2Path(GameObject go)
    {
        bool change = false;

        UISprite[] spList = go.GetComponentsInChildren<UISprite>(true);
        foreach (UISprite sp in spList)
        {
            XUISprite xsp = sp.GetComponent<XUISprite>();
            if (xsp == null)
            {
                change = true;
                xsp = sp.gameObject.AddComponent<XUISprite>();
                xsp.m_NeedAudio = false;
            }

            if (sp.atlas != null)
            {
                change = true;
                string path = AssetDatabase.GetAssetPath(sp.atlas.gameObject).Replace("Assets/Resources/atlas/UI/", "");
                xsp.SpriteAtlasPath = path.Remove(path.LastIndexOf('.'));
               
                xsp.SPriteName = sp.spriteName;
                sp.atlas = null;
                sp.spriteName = "";
            }
        }

        return change;
    }

    //private static bool UITextureRefrence2Path(GameObject go)
    //{
    //    bool change = false;

    //    UITexture[] texList = go.GetComponentsInChildren<UITexture>(true);
    //    foreach (UITexture tex in texList)
    //    {
    //        //XUITexture xtex = tex.GetComponent<XUITexture>();
    //        //if (xtex == null)
    //        //{
    //        //    change = true;
    //        //    xtex = tex.gameObject.AddComponent<XUITexture>();
    //        //}
    //        if (tex.material != null || tex.shader != null)
    //        {
    //            Debug.LogWarning(go.name);
    //        }
    //        string path = "";
    //        if (tex.mTexture != null)
    //        {
    //            path = AssetDatabase.GetAssetPath(tex.mTexture).Replace("Assets/Resources/", "");
    //            path = path.Remove(path.LastIndexOf('.'));
    //            tex.texPath = path;
    //        }
    //        else if (string.IsNullOrEmpty(tex.texPath))
    //        {
    //            XUITexture xtex = tex.GetComponent<XUITexture>();
    //            if (xtex != null && !string.IsNullOrEmpty(xtex.TexturePath))
    //            {
    //                tex.texPath = "atlas/UI/" + xtex.TexturePath;
    //            }
    //        }
    //        if (!string.IsNullOrEmpty(tex.texPath))
    //        {
    //            tex.mtexType = UITexture.GetTextureListType(tex.texPath);
    //        }
    //        if (tex.shader != null &&
    //            tex.shader.name != "Custom/UI/Mask" &&
    //            tex.shader.name != "Custom/UI/MaskColor" &&
    //            tex.shader.name != "Custom/UI/RenderTexture")
    //        {
    //            tex.shaderName = null;
    //        }
    //        else if(tex.shader != null)
    //        {
    //            tex.shaderName = tex.shader.name;
    //        }
    //        tex.shader = null;
    //        change = true;
    //        tex.mTexture = null;
    //    }

    //    return change;
    //}

    private static bool UILabelRefrence2Path(GameObject go)
    {
        bool change = false;

        UILabel[] labelList = go.GetComponentsInChildren<UILabel>(true);
        foreach (UILabel label in labelList)
        {
            XUILabel xlabel = label.GetComponent<XUILabel>();
            if (xlabel == null)
            {
                change = true;
                xlabel = label.gameObject.AddComponent<XUILabel>();
            }

            if (label.trueTypeFont != null)
            {
                string[] path = AssetDatabase.GetAssetPath(label.trueTypeFont).Replace("Assets/Resources/", "").Split('.');
                if (path.Length < 2) continue;
                xlabel.fontPath = path[0];
                xlabel.fontSuffix = "." + path[1];
                label.trueTypeFont = null;
                change = true;
            }
            else if (label.bitmapFont != null)
            {
                string[] path = AssetDatabase.GetAssetPath(label.bitmapFont).Replace("Assets/Resources/", "").Split('.');
                if (path.Length < 2) continue;
                xlabel.fontPath = path[0];
                xlabel.fontSuffix = "." + path[1];
                label.bitmapFont = null;
                change = true;
            }
        }

        return change;
    }

    private static bool UISpritePath2Refrence(GameObject go)
    {
        bool change = false;

        XUISprite[] xspList = go.GetComponentsInChildren<XUISprite>(true);
        foreach (XUISprite xsp in xspList)
        {
            UISprite sp = xsp.GetComponent<UISprite>();
            if (sp == null)
            {
                DestroyImmediate(xsp);
                change = true;
                continue;
            }

            if (xsp.SpriteAtlasPath != "")
            {
                change = true;
                UIAtlas atlas = (AssetDatabase.LoadAssetAtPath("Assets/Resources/atlas/UI/" + xsp.SpriteAtlasPath + ".prefab", typeof(GameObject)) as GameObject).GetComponent<UIAtlas>();
                sp.atlas = atlas;
                sp.spriteName = xsp.SPriteName;
                xsp.SpriteAtlasPath = "";
                xsp.SPriteName = "";
            }
        }

        return change;
    }

    private static bool UITexturePath2Refrence(GameObject go)
    {
        bool change = false;

        XUITexture[] xtexList = go.GetComponentsInChildren<XUITexture>(true);
        foreach (XUITexture xtex in xtexList)
        {
            UITexture tex = xtex.GetComponent<UITexture>();
            if (tex == null)
            {
                DestroyImmediate(xtex);
                change = true;
                continue;
            }

            //if (xtex.TexturePath != "")
            //{
            //    change = true;
            //    if(tex.mType == UITexture.TextureType.Normal)
            //    {
            //        Texture texture = AssetDatabase.LoadAssetAtPath("Assets/Resources/atlas/UI/" + xtex.TexturePath + ".png", typeof(Texture)) as Texture;
            //        tex.mainTexture = texture;
            //        xtex.TexturePath = "";
            //    }
            //}
        }

        return change;
    }

    private static bool UILabelPath2Refrence(GameObject go)
    {
        bool change = false;

        XUILabel[] xlabelList = go.GetComponentsInChildren<XUILabel>(true);
        foreach(XUILabel xlabel in xlabelList)
        {
            UILabel label = xlabel.GetComponent<UILabel>();
            if(label==null)
            {
                DestroyImmediate(xlabel);
                change = true;
                continue;
            }

            if (xlabel.fontPath != "" && xlabel.fontSuffix != "")
            {
                change = true;
                switch (xlabel.fontSuffix)
                {
                    case ".ttf":
                        {
                            Font font = AssetDatabase.LoadAssetAtPath("Assets/Resources/" + xlabel.fontPath + xlabel.fontSuffix, typeof(Font)) as Font;
                            label.trueTypeFont = font;
                        }
                        break;
                    case ".prefab":
                        {
                            GameObject obj = AssetDatabase.LoadAssetAtPath("Assets/Resources/" + xlabel.fontPath + xlabel.fontSuffix, typeof(GameObject)) as GameObject;
                            label.bitmapFont = obj == null ? null : obj.GetComponent<UIFont>();
                        }
                        break;
                }
                xlabel.fontPath = "";
                xlabel.fontSuffix = "";
            }
        }

        return change;
    }

    [MenuItem(@"Assets/CheckName")]
    public static void CheckName()
    {
        Dictionary<string, List<string>> pathDic = new Dictionary<string, List<string>>();
        foreach (Object o in Selection.GetFiltered(typeof(Object), SelectionMode.DeepAssets))
        {
            string fileName = Path.GetFileNameWithoutExtension(AssetDatabase.GetAssetPath(o));
            if(!pathDic.ContainsKey(fileName))
            {
                pathDic.Add(fileName, new List<string>());
            }
            pathDic[fileName].Add(AssetDatabase.GetAssetPath(o));
        }
        string Log = "";
        int count = 0;
        foreach (KeyValuePair<string, List<string>> o in pathDic)
        {
            if (o.Value.Count > 1)
            {
                Log += string.Format("{0}\n", ++count);
                for (int i = 0; i < o.Value.Count; ++i)
                {
                    Log += o.Value[i] + "\n";
                }
            }
        }
        Debug.Log(Log);

        EditorUtility.DisplayDialog("CheckName", "Finish", "OK");
    }

    [MenuItem(@"Assets/CheckHash")]
    public static void CheckHash()
    {
        Dictionary<uint, List<string>> pathDic = new Dictionary<uint, List<string>>();
        Dictionary<uint, List<string>> resDic = new Dictionary<uint, List<string>>();
        foreach (Object o in Selection.GetFiltered(typeof(Object), SelectionMode.DeepAssets))
        {
            string fileName = ABSystem.AssetBundleUtils.ConvertToABName(AssetDatabase.GetAssetPath(o));
            if (!pathDic.ContainsKey(XCommon.singleton.XHash(fileName)))
            {
                pathDic.Add(XCommon.singleton.XHash(fileName), new List<string>());
            }
            pathDic[XCommon.singleton.XHash(fileName)].Add(fileName);

            fileName = AssetDatabase.GetAssetPath(o).Replace("Assets/Resources/", "");
            if (!resDic.ContainsKey(XCommon.singleton.XHash(fileName)))
            {
                resDic.Add(XCommon.singleton.XHash(fileName), new List<string>());
            }
            resDic[XCommon.singleton.XHash(fileName)].Add(fileName);
        }
        string Log = "";
        int count = 0;
        foreach (KeyValuePair<uint, List<string>> o in pathDic)
        {
            if (o.Value.Count > 1)
            {
                Log += string.Format("{0}\n", ++count);
                for (int i = 0; i < o.Value.Count; ++i)
                {
                    Log += o.Value[i] + "\n";
                }
            }
        }
        Debug.Log(Log);

        Log = "";
        count = 0;
        foreach (KeyValuePair<uint, List<string>> o in resDic)
        {
            if (o.Value.Count > 1)
            {
                Log += string.Format("{0}\n", ++count);
                for (int i = 0; i < o.Value.Count; ++i)
                {
                    Log += o.Value[i] + "\n";
                }
            }
        }
        Debug.Log(Log);

        EditorUtility.DisplayDialog("CheckHash", "Finish", "OK");
    }

    [MenuItem(@"Assets/UIHotFix/Reference2Path")]
    public static void Reference2Path()
    {
        foreach (Object o in Selection.GetFiltered(typeof(Object), SelectionMode.DeepAssets))
        {
            if (AssetDatabase.GetAssetPath(o).Contains(".prefab"))
            {
                GameObject go = PrefabUtility.InstantiatePrefab(o) as GameObject;

                bool change = false;
                change |= UISpriteRefrence2Path(go);

                //change |= UITextureRefrence2Path(go);
                change |= UILabelRefrence2Path(go);

                if (change)
                {
                    Debug.Log(go.name);
                    Object prefab = (o == null) ? PrefabUtility.CreateEmptyPrefab(AssetDatabase.GetAssetPath(o)) : (o as GameObject);

                    PrefabUtility.ReplacePrefab(go, prefab);
                }
                DestroyImmediate(go);
            }
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        EditorUtility.DisplayDialog("Reference2Path", "引用转路径，转换成功！！！", "OK");
    }

    [MenuItem(@"Assets/UIHotFix/Path2Reference")]
    public static void Path2Reference()
    {
        foreach (Object o in Selection.GetFiltered(typeof(Object), SelectionMode.DeepAssets))
        {
            if (AssetDatabase.GetAssetPath(o).Contains(".prefab"))
            {
                GameObject go = PrefabUtility.InstantiatePrefab(o) as GameObject;

                bool change = false;
                change |= UISpritePath2Refrence(go);
                //change |= UITexturePath2Refrence(go);
                change |= UILabelPath2Refrence(go);

                if (change)
                {
                    Debug.Log(go.name);
                    Object prefab = (o == null) ? PrefabUtility.CreateEmptyPrefab(AssetDatabase.GetAssetPath(o)) : (o as GameObject);

                    PrefabUtility.ReplacePrefab(go, prefab);
                }
                DestroyImmediate(go);
            }
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        EditorUtility.DisplayDialog("Path2Reference", "路径转引用，转换成功！！！", "OK");
    }
}
