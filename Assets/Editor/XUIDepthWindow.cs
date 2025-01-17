using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using XUtliPoolLib;

public class XUIDepthWindow : EditorWindow {

    public class DepthInfo
    {
        public string atlasName;
        public int sortDepth;
        public List<UIWidget> depthWidgets = new List<UIWidget>();
        public bool close = true;
        private void SetupDepth()
        {
            if (depthWidgets != null && depthWidgets.Count > 0)
                sortDepth = depthWidgets[0].depth;
            else
                sortDepth = 0;
        }

        public void ReSetupDepth()
        {
            for (int i = 0, length = depthWidgets.Count; i < length; i++)
            {
                depthWidgets[i].depth = sortDepth + i;
            }
        }

        public void Sort()
        {
            depthWidgets.Sort(SortCompare);
            SetupDepth();
        }
        private int SortCompare(UIWidget widget1, UIWidget widget2)
        {
            return widget1.depth - widget2.depth;
        }
    }


    
    [MenuItem("Tools/UI/UI Depth")]
    public static void Execute()
    {
        XUIDepthWindow window = (XUIDepthWindow)EditorWindow.GetWindow(typeof(XUIDepthWindow));
        window.Setup();
        window.minSize = new Vector2(450, 500);
    }
    private GUILayoutOption[] _line = new GUILayoutOption[] { GUILayout.ExpandWidth(true), GUILayout.Height(1)};
    private Dictionary<string, DepthInfo> _depthDic;
    private int _widgetCount = 0;
    private Vector2 _depthScrollPos = new Vector2(0,0);
    private bool ctrlPressed = false;
    private static ObjectPool<DepthInfo> DepthInfoPool = new ObjectPool<DepthInfo>(Create,null,null);
    private string targetName = "";
    private bool showActive = true;

    private static DepthInfo Create()
    {
        return new DepthInfo();
    }

    private void OnDisable()
    {
        Release();
    }
    private void OnDestroy()
    {
        Release();
    }

    void OnGUI()
    {

        if (GUILayout.Button("Refresh"))
        {
            Setup();
        }
        GUILayout.BeginHorizontal();
        GUILayout.Label("Current:"+targetName);
        showActive = GUILayout.Toggle(showActive, "All Show");
        GUILayout.Label("Atlas Count:"+_depthDic.Count.ToString());
        GUILayout.Label("Widget Count:"+ _widgetCount.ToString());
        GUILayout.EndHorizontal();
        int i = 0, length = 0;
        ctrlPressed = Event.current.control || Event.current.command;
        _depthScrollPos = EditorGUILayout.BeginScrollView(_depthScrollPos);
        foreach (KeyValuePair<string , DepthInfo> valuePair  in _depthDic)
        {
            EditorGUILayout.Space();
            GUILayout.Box("", _line);
            EditorGUILayout.Space();
            DepthInfo info = valuePair.Value;
            GUILayout.BeginHorizontal();
            GUILayout.Label(info.atlasName, GUILayout.Width(100));
            if (info.close)
            {
                if (GUILayout.Button("Show" ,GUILayout.Width(100)))
                {
                    info.close = false;
                }
            }
            else
            {
                if (GUILayout.Button("Hide",GUILayout.Width(100)))
                {
                    info.close = true;
                }
            }
            info.sortDepth =  EditorGUILayout.IntField(info.sortDepth, GUILayout.Width(100));
            if (GUILayout.Button("Active" , GUILayout.Width(100)))
            {
                info.ReSetupDepth();
            }
            GUILayout.EndHorizontal();
            if (info.close) continue;
            for (i = 0,length = info.depthWidgets.Count;i < length; i++)
            {
                GUILayout.BeginHorizontal();
       

                if (GUILayout.Button(info.depthWidgets[i].name, GUILayout.Width(100)))
                {
                    SelectObject(info.depthWidgets[i], ctrlPressed);
                }
                if(info.depthWidgets[i] is UISprite)
                {
                    
                    UISprite sprite = info.depthWidgets[i] as UISprite;
                    if ((sprite.atlas != null) && !string.IsNullOrEmpty(sprite.spriteName))
                    {
                        if (GUILayout.Button(sprite.spriteName, GUILayout.Width(100)))
                        {
                            SelectObject(sprite.atlas.gameObject, ctrlPressed);
                        }
                    }
                    else
                    {
                        GUILayout.Label("", GUILayout.Width(100));
                    }
                }else if(info.depthWidgets[i] is UILabel)
                {
                    UILabel label = info.depthWidgets[i] as UILabel;
                    if (label.bitmapFont != null)
                    {
                        if (GUILayout.Button(label.bitmapFont.name, GUILayout.Width(100)))
                        {
                            SelectObject(label.bitmapFont.gameObject, ctrlPressed);
                        }
                    }
                    else if (label.trueTypeFont != null)
                    {
                        if (GUILayout.Button(label.trueTypeFont.name, GUILayout.Width(100)))
                            SelectObject(label.trueTypeFont, ctrlPressed);
                    }
                    else
                    {
                        GUILayout.Label("", GUILayout.Width(100));
                    }
                }
                else
                {
                    GUILayout.Label("", GUILayout.Width(100));
                }
                info.depthWidgets[i].depth = EditorGUILayout.IntField(info.depthWidgets[i].depth, GUILayout.Width(200));
                bool isHide = info.depthWidgets[i].isVisible;
                if (!isHide) GUILayout.Label("*",GUILayout.Width(20));
                GUILayout.EndHorizontal();

            }  
        }
        EditorGUILayout.EndScrollView();

    }

    void SelectObject(Object selectedObject, bool append)
    {
        if (append)
        {
            List<Object> currentSelection = new List<Object>(Selection.objects);
            if (currentSelection.Contains(selectedObject)) currentSelection.Remove(selectedObject);
            else currentSelection.Add(selectedObject);

            Selection.objects = currentSelection.ToArray();
        }
        else Selection.activeObject = selectedObject;
    }

    public void Setup()
    {
        if (_depthDic == null) _depthDic = new Dictionary<string, DepthInfo>();
        Release();
        GameObject go = Selection.activeGameObject;
        if (go != null)
        {
            targetName = go.name;
            UISprite[] uiSprites = go.transform.GetComponentsInChildren<UISprite>(showActive);
            _widgetCount = uiSprites.Length;
            UISprite widget;
            string nullAtlas = "NullAtlas";
            int i, length;
            for (i = 0, length = uiSprites.Length; i < length; i++)
            {
                widget = uiSprites[i];
                if (widget.atlas != null)
                {
                    Insert(_depthDic, widget.atlas.name, widget);
                }
                else
                {
                    Insert(_depthDic, nullAtlas, widget);
                }
            }
            string fontName = "Font";
            UILabel[] fonts = go.transform.GetComponentsInChildren<UILabel>(showActive);
            _widgetCount += fonts.Length;
            for (i = 0, length = fonts.Length; i < length; i++)
            {
                if (fonts[i].bitmapFont != null)
                {
                    Insert(_depthDic, "bitmapFont:" + fontName, fonts[i]);
                }
                else
                {
                    Insert(_depthDic, fontName, fonts[i]);
                }
            }
          
            foreach (KeyValuePair<string, DepthInfo> info in _depthDic)
            {
  
                info.Value.Sort();
            }
        }
        else
        {
            targetName = "";
        }
    }

    private void Release()
    {
        if(_depthDic != null && _depthDic.Count > 0)
        {
            foreach(KeyValuePair<string , DepthInfo> depthInfo in _depthDic)
            {
                depthInfo.Value.depthWidgets.Clear();
                depthInfo.Value.close = true;
                DepthInfoPool.Release(depthInfo.Value);
            }
            _depthDic.Clear();
        }
    }
  

    private void Insert(Dictionary<string, DepthInfo> infos, string atlasName, UIWidget widget)
    {
        DepthInfo info;
        if (!infos.TryGetValue(atlasName, out info))
        {
            info = DepthInfoPool.Get();
            info.atlasName = atlasName;
            infos.Add(atlasName, info);
        }
        info.depthWidgets.Add(widget);
    }
}
