using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using System.Diagnostics;
using System.Text;
using System.Reflection;

static class ComponentTool
{
    public static T GetCopyOf<T>(this Component comp, T other) where T : Component
    {
        Type type = comp.GetType();
        if (type != other.GetType()) return null; // type mis-match

        FillValues(comp, other, type);

        return comp as T;
    }

    public static T AddComponent<T>(this GameObject go, T toAdd) where T : Component
    {
        return go.AddComponent<T>().GetCopyOf(toAdd) as T;
    }

    private static void FillValues(Component to, Component from, Type type)
    {
        BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Default | BindingFlags.DeclaredOnly | BindingFlags.FlattenHierarchy;
        PropertyInfo[] pinfos = type.GetProperties(flags);
        foreach (var pinfo in pinfos)
        {
            if (pinfo.CanWrite)
            {
                try
                {
                    pinfo.SetValue(to, pinfo.GetValue(from, null), null);
                }
                catch { } // In case of NotImplementedException being thrown. For some reason specifying that exception didn't seem to catch it, so I didn't catch anything specific.
            }
        }
        FieldInfo[] finfos = type.GetFields(flags);
        foreach (var finfo in finfos)
        {
            finfo.SetValue(to, finfo.GetValue(from));
        }

        if (type.BaseType != null)
            FillValues(to, from, type.BaseType);
    }
}

public class XPrefabBatchModifier: MonoBehaviour
{
    delegate void ModDelegate(UnityEngine.Object o);

    static ModDelegate doFunc;

    [MenuItem(@"Assets/Tool/Prefab/Batch Modify Item Depth")]
    static void ItemDepth()
    {
        doFunc = new ModDelegate(_DoItemDepth);
        BatchModify();
    }


    static void BatchModify()
    {
        UnityEngine.Object[] jj = Selection.GetFiltered(typeof(UnityEngine.Object), SelectionMode.DeepAssets | SelectionMode.Editable);
        foreach (UnityEngine.Object o in jj)
        {
            doFunc(o);
        }
    }

    static List<UISprite> s_Result = new List<UISprite>();
    static void _DoItemDepth(UnityEngine.Object o)
    {
        bool bDirty = false;
        GameObject go = PrefabUtility.InstantiatePrefab(o) as GameObject;
        s_Result.Clear();
        go.GetComponentsInChildren<UISprite>(true, s_Result);

        foreach (UISprite sp in s_Result)
        {
            if (sp == null)
                continue;
            if (sp.gameObject.name != "Quality")
                continue;

            _ChangeBox(sp.gameObject.transform.parent.gameObject);
            bDirty = true;
        }

        if (bDirty)
        {
            PrefabUtility.ReplacePrefab(go, o, ReplacePrefabOptions.ConnectToPrefab);
            UnityEngine.Debug.Log("ItemDepth Modified: " + AssetDatabase.GetAssetPath(o));
        }

        GameObject.DestroyImmediate(go);
    }

    static void _ChangeBox(GameObject go)
    {
        Transform iconT = go.transform.Find("Icon");
        if (iconT != null)
        {
            BoxCollider box = iconT.GetComponent<BoxCollider>();
            UIWidget widget = iconT.GetComponent<UIWidget>();

            if (widget != null)
                widget.autoResizeBoxCollider = false;
            if (box != null)
            {
                box.center = Vector3.zero;
                box.size = new UnityEngine.Vector3(90, 90, 0);
            }
        }
        Transform iconUnderIconT = go.transform.Find("Icon/Icon");
        if (iconUnderIconT != null)
        {
            UIWidget widget = iconUnderIconT.GetComponent<UIWidget>();

            if (widget != null)
                widget.autoResizeBoxCollider = false;
        }
    }

    static void _SetItem(GameObject go)
    {
        Transform iconT = go.transform.Find("Icon");
        Transform iconUnderIconT = go.transform.Find("Icon/Icon");
        Transform qualityT = go.transform.Find("Quality");
        Transform bgT = go.transform.Find("Bg");
        Transform numT = go.transform.Find("Num");
        Transform numTopT = go.transform.Find("NumTop");
        Transform nameT = go.transform.Find("Name");
        Transform maskT = go.transform.Find("Mask");

        if (iconT != null)
        {
            UISprite sp = iconT.GetComponent<UISprite>();

            //UISprite spUnderIcon = null;
            if (iconUnderIconT == null)
            {
                //spUnderIcon = NGUITools.AddChild<UISprite>(iconT.gameObject);
                //iconUnderIconT = spUnderIcon.gameObject.transform;
                iconUnderIconT = NGUITools.AddChild(iconT.gameObject, false).transform;
                //spUnderIcon = iconUnderIconT.gameObject.AddComponent<UISprite>().GetCopyOf(sp) as UISprite;
                iconUnderIconT.gameObject.AddComponent<XUISprite>().GetCopyOf(iconT.gameObject.GetComponent<XUISprite>());
            }
            else
            {
                //spUnderIcon = iconUnderIconT.GetComponent<UISprite>();
            }
            iconUnderIconT.localPosition = Vector3.zero;
            iconUnderIconT.gameObject.name = "Icon";
            BoxCollider box = iconUnderIconT.gameObject.GetComponent<BoxCollider>();
            if (box != null)
                GameObject.DestroyImmediate(box);

            sp.spriteName = null;
            sp.atlas = null;
        }

        _ChangeDepth<UISprite>(bgT, 200);
        _ChangeDepth<UISprite>(qualityT, 210);
        _ChangeDepth<UISprite>(iconT, 220);
        _ChangeDepth<UISprite>(iconUnderIconT, 230);
        _ChangeDepth<UILabel>(numT, 250);
        _ChangeDepth<UILabel>(numTopT, 250);
        _ChangeDepth<UILabel>(nameT, 250);

        if (maskT != null)
        {
            GameObject.DestroyImmediate(maskT.gameObject);
            UnityEngine.Debug.Log("Mask removed");
        }
    }

    static void _ChangeDepth<T>(Transform t, int depth) where T: UIWidget
    {
        if (t != null)
        {
            T widget = t.GetComponent<T>();
            if (widget != null)
                widget.depth = depth;
        }
    }
}
