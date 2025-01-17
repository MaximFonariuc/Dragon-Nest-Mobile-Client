using UnityEngine;
using UnityEditor;
using System.Collections;
using System.IO;

public class ChangeAtlasSprite : MonoBehaviour
{
    [MenuItem(@"XEditor/Tools/ChangeAtlasSprite")]
    static void Execute()
    {
        EditorWindow.GetWindowWithRect<ChangeAtlasSpriteEditor>(new Rect(300, 300, 350, 350), true, @"ChangeAtlasSprite");
    }
}
[ExecuteInEditMode]
internal class ChangeAtlasSpriteEditor : EditorWindow
{
    private UIAtlas _CurrentAtlas = null;
    private UIAtlas _TargetAtlas = null;

    private string _CurrentSprite = "";
    private string _TargetSprite = "";

    private UISprite.Type _SpriteType = UISprite.Type.Sliced;

    //private string _path = null;
    //private string[] _files = null;

    void OnGUI()
    {
        GUILayout.Label("当前图集");
        ComponentSelector.Draw<UIAtlas>("Atlas", _CurrentAtlas, OnCurrentSelectAtlas, true, GUILayout.MinWidth(80f));

        if (_CurrentAtlas != null)
        {
            _CurrentAtlas.GetSprite(_CurrentSprite);
            NGUIEditorTools.DrawAdvancedSpriteField(_CurrentAtlas, _CurrentSprite, SelectCurrentSprite, true);
        }

        GUILayout.Label("目标图集");
        ComponentSelector.Draw<UIAtlas>("Atlas", _TargetAtlas, OnTargetSelectAtlas, true, GUILayout.MinWidth(80f));

        if (_TargetAtlas != null)
        {
            _TargetAtlas.GetSprite(_TargetSprite);
            NGUIEditorTools.DrawAdvancedSpriteField(_TargetAtlas, _TargetSprite, SelectTargetSprite, true);
        }

        _SpriteType = (UISprite.Type)EditorGUILayout.EnumPopup("Sprite Type", _SpriteType);

        if (GUILayout.Button("替换", GUILayout.MaxWidth(80)))
        {
            if (_CurrentAtlas != null && _TargetAtlas != null && _CurrentSprite != "" && _TargetSprite != null)
            {
                ChangeAtlasSprite();
            }
            else
            {
                ShowNotification(new GUIContent("替换参数不完整！！！"));
            }
        }
    }

    private void ChangeAtlasSprite()
    {
        foreach (Object o in Selection.GetFiltered(typeof(Object), SelectionMode.DeepAssets))
        {
            if (AssetDatabase.GetAssetPath(o).Contains(".prefab"))
            {
                bool change = false;
                GameObject go = PrefabUtility.InstantiatePrefab(o) as GameObject;

                UISprite[] spList = go.GetComponentsInChildren<UISprite>(true);
                foreach (UISprite sp in spList)
                {
                    if (sp.atlas == _CurrentAtlas && sp.spriteName == _CurrentSprite)
                    {
                        change = true;
                        sp.atlas = _TargetAtlas;
                        sp.spriteName = _TargetSprite;
                        sp.type = _SpriteType;
                    }
                }

                XUISprite[] xspList = go.GetComponentsInChildren<XUISprite>(true);
                foreach (XUISprite sp in xspList)
                {
                    if (sp.SpriteAtlasPath == AssetDatabase.GetAssetPath(_CurrentAtlas).Replace(".prefab", "").Replace("Assets/Resources/atlas/UI/", "") && sp.SPriteName == _CurrentSprite)
                    {
                        change = true;
                        sp.SpriteAtlasPath = AssetDatabase.GetAssetPath(_TargetAtlas).Replace(".prefab", "").Replace("Assets/Resources/atlas/UI/", "");
                        sp.SPriteName = _TargetSprite;
                        UISprite uisp=sp.transform.GetComponent<UISprite>();
                        uisp.type = _SpriteType;
                    }
                }

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
        ShowNotification(new GUIContent("替换成功！！！"));
    }

    private void OnCurrentSelectAtlas(Object obj)
    {
        if (_CurrentAtlas != obj)
        {
            _CurrentAtlas = obj as UIAtlas;
            _CurrentSprite = "";
            Repaint();
        }
    }

    private void OnTargetSelectAtlas(Object obj)
    {
        if (_TargetAtlas != obj)
        {
            _TargetAtlas = obj as UIAtlas;
            _TargetSprite = "";
            Repaint();
        }
    }

    private void SelectCurrentSprite(string spriteName)
    {
        if (_CurrentSprite != spriteName)
        {
            _CurrentSprite = spriteName;
            Repaint();
        }
    }

    private void SelectTargetSprite(string spriteName)
    {
        if (_TargetSprite != spriteName)
        {
            _TargetSprite = spriteName;
            Repaint();
        }
    }

    [MenuItem("Assets/XChangeUISound")]
    private static void Execute2()
    {
        ChangeCloseSound();
    }

    private static void ChangeCloseSound()
    {
        foreach (Object o in Selection.GetFiltered(typeof(Object), SelectionMode.DeepAssets))
        {
            if (AssetDatabase.GetAssetPath(o).Contains(".prefab"))
            {
                bool change = false;
                GameObject go = PrefabUtility.InstantiatePrefab(o) as GameObject;

                XUISprite[] spList = go.GetComponentsInChildren<XUISprite>(true);
                foreach (XUISprite sp in spList)
                {
                    if (sp.m_NeedAudio && (sp.gameObject.name == "Close" || sp.gameObject.name == "close"))
                    {
                        //if (string.IsNullOrEmpty(sp.audioClip) || !sp.audioClip.StartsWith("Audio"))
                        {
                            sp.SetAudioClip("Audio/UI/UI_button_cancel");
                            change = true;
                        }
                    }
                }

                XUIButton[] btnList = go.GetComponentsInChildren<XUIButton>(true);
                foreach (XUIButton btn in btnList)
                {
                    if (btn.m_NeedAudio && (btn.gameObject.name == "Close" || btn.gameObject.name == "close"))
                    {
                        //if (string.IsNullOrEmpty(btn.audioClip) || !btn.audioClip.StartsWith("Audio"))
                        {
                            btn.SetAudioClip("Audio/UI/UI_button_cancel");
                            change = true;
                        }
                    }
                }

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

        EditorUtility.DisplayDialog("替换成功！！！", "替换成功！！！", "OK");
        //ShowNotification(new GUIContent("替换成功！！！"));
    }
}