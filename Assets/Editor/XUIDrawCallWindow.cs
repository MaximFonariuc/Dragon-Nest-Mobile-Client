using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using XUtliPoolLib;

public class XUIDrawCallWindow : EditorWindow
{
    public class UIDrawCallPanelInfo
    {
        public UIDrawCallPanelInfo(GameObject go)
        {
            panel = go;
            if (showInfoIndex >= showInfoList.Count)
                showInfoList.Add(false);
            showIndex = showInfoIndex;
            showInfoIndex++;

            uiPanel = go.transform.GetComponent<UIPanel>();
            if (uiPanel == null) uiPanel = new UIPanel();
        }
        public GameObject panel;
        public UIPanel uiPanel;
        public int dc;
        public int showIndex;
        public List<UIDrawCallWidgetInfo> list = new List<UIDrawCallWidgetInfo>();

        public int Refresh()
        {
            list.Sort(Compare);
            string data = "???";
            dc = 0;
            for(int i = 0; i < list.Count; i++)
            {
                if(list[i].data != data)
                {
                    if(i != 0 && list[i - 1].widget.depth == list[i].widget.depth)
                    {
                        for(int j = i - 2; j >= 0; j --)
                        {
                            if(list[j].widget.depth != list[i].widget.depth)
                            {
                                if(list[j].data == list[i - 1].data)
                                {
                                    list[i - 1].warning = true;
                                }
                                break;
                            }
                            if (list[j].data == list[i].data)
                                list[j].warning = true;
                        }
                        for(int j = i + 1; j < list.Count; j++)
                        {
                            if(list[j].widget.depth != list[i].widget.depth)
                            {
                                if(list[j].data == list[i].data)
                                {
                                    list[i].warning = true;
                                }
                                break;
                            }
                            if (list[j].data == list[i - 1].data)
                                list[j].warning = true;
                        }
                    }
                    data = list[i].data;
                    dc++;
                }

                BoxCollider boxCollider = list[i].widget.gameObject.GetComponent<BoxCollider>();
                list[i].hasCollider = boxCollider!=null;
            }
            return dc;
        }
    }
    
    public class UIDrawCallWidgetInfo
    {
        public UIDrawCallWidgetInfo(UIWidget w, string t, string d)
        {
            widget = w;
            type = t;
            data = d;
            warning = false;
            hasCollider = false;
        }
        public UIWidget widget;
        public string type;
        public string data;//使用的材质名
        public bool warning;
        public bool hasCollider;
    }

    public List<UIDrawCallPanelInfo> PanelList = new List<UIDrawCallPanelInfo>();
    private Vector2 _depthScrollPos = new Vector2(0, 0);
    public int allDc;
    public static List<bool> showInfoList = new List<bool>();
    public static int showInfoIndex;
    public static GameObject selectGo;

    [MenuItem("Tools/UI/UI DrawCall")]
    public static void Execute()
    {
        XUIDrawCallWindow window = (XUIDrawCallWindow)EditorWindow.GetWindow(typeof(XUIDrawCallWindow));
        selectGo = Selection.activeGameObject;
        showInfoList.Clear();
        window.Setup();
        window.minSize = new Vector2(450, 500);
    }

    void OnGUI()
    {
        if (GUILayout.Button("Select"))
        {
            selectGo = Selection.activeGameObject;
            showInfoList.Clear();
            Setup();
        }
        if (GUILayout.Button("Refresh"))
        {
            Setup();
        }
        GUILayout.BeginHorizontal();
        GUILayout.Label("All Draw Call:" + allDc, GUILayout.Width(150));
        if (GUILayout.Button("Wiki", GUILayout.Width(50)))
        {
            ShowNotification(new GUIContent("用法：选中Prefab实例，再打开此界面或点击Select\n" + 
                "每次改变层级之后，DrawCall和排序并不会马上刷新，需要点一下Refresh\n" + 
                "默认选中Prefab最外层都有Panel，如果是没Panel的Handler\n" + 
                "请拖入主Prefab中再对主Prefab使用此工具"));
        }
        GUILayout.EndHorizontal();
        _depthScrollPos = EditorGUILayout.BeginScrollView(_depthScrollPos);
        for(int i = 0; i < PanelList.Count; i++)
        {
            GUILayout.BeginHorizontal();
            Color r = GUI.color;
            GUILayout.Label("Panel Name:", GUILayout.Width(80));

            GUI.color = new Color(0, 1, 0);
            if (GUILayout.Button(PanelList[i].panel.name, GUILayout.Width(150)))
            {
                Selection.activeObject = PanelList[i].panel.gameObject;
            }
            if (showInfoList[PanelList[i].showIndex])
            {
                if (GUILayout.Button("Hide", GUILayout.Width(45)))
                {
                    showInfoList[PanelList[i].showIndex] = false;
                }
            }
            else
            {
                if (GUILayout.Button("Show", GUILayout.Width(45)))
                {
                    showInfoList[PanelList[i].showIndex] = true;
                }
            }
            GUI.color = r;
            int panelDepth = EditorGUILayout.IntField(PanelList[i].uiPanel.depth, GUILayout.Width(40));
            if (GUI.changed)
            {
                PanelList[i].uiPanel.depth = panelDepth;
            }

            GUILayout.Label("Count:" + PanelList[i].list.Count + " DC:" + PanelList[i].dc);
            GUILayout.EndHorizontal();
            if (!showInfoList[PanelList[i].showIndex]) continue;
            for(int j = 0; j < PanelList[i].list.Count; j++)
            {
                GUILayout.BeginHorizontal();
                if (GUILayout.Button(PanelList[i].list[j].widget.name, GUILayout.Width(120)))
                {
                    Selection.activeObject = PanelList[i].list[j].widget;
                }
                Object selectToGo;
                if (PanelList[i].list[j].widget is UISprite)
                {
                    selectToGo = (PanelList[i].list[j].widget as UISprite).atlas;
                }
                else if (PanelList[i].list[j].widget is UILabel)
                {
                    selectToGo = (PanelList[i].list[j].widget as UILabel).ambigiousFont;
                }
                else
                {
                    selectToGo = (PanelList[i].list[j].widget as UITexture).mainTexture;
                }
                if (GUILayout.Button(PanelList[i].list[j].data, GUILayout.Width(160)))
                {
                    Selection.activeObject = selectToGo;
                }
                int depth = EditorGUILayout.IntField(PanelList[i].list[j].widget.depth, GUILayout.Width(40));
                if(GUI.changed)
                {
                    PanelList[i].list[j].widget.depth = depth;
                }
                if (GUILayout.Button("↑", GUILayout.Width(30)))
                {
                    PanelList[i].list[j].widget.depth ++;
                }
                if (GUILayout.Button("↓", GUILayout.Width(30)))
                {
                    PanelList[i].list[j].widget.depth --;
                }
                GUILayout.Label(PanelList[i].list[j].type, GUILayout.Width(50));
                if (PanelList[i].list[j].warning)
                {
                    r = GUI.color;
                    GUI.color = new Color(255, 0, 0);
                    GUILayout.Label("Warning", GUILayout.Width(50));
                    GUI.color = r;
                }
                if (PanelList[i].list[j].hasCollider)
                {
                    r = GUI.color;
                    GUI.color = new Color(0, 1, 0);
                    GUILayout.Label("Collider", GUILayout.Width(50));
                    GUI.color = r;
                }
                GUILayout.EndHorizontal();
            }

        }
        EditorGUILayout.EndScrollView();

        GUILayout.BeginHorizontal();
        GUILayout.Label("", GUILayout.Width(this.position.width - 100));
        if (GUILayout.Button("收起", GUILayout.Width(80)))
        {
            for (int i = 0; i < showInfoList.Count; i++)
                showInfoList[i] = false;
        }
        GUILayout.EndHorizontal();
    }

    void Setup()
    {
        PanelList.Clear();
        showInfoIndex = 0;
        if (Selection.transforms.Length != 1)
        {
            Debug.Log("请仅选择一个UI");
        }
        else
        {
            GameObject targetGo = selectGo;

            UIDrawCallPanelInfo info = new UIDrawCallPanelInfo(targetGo);
            PanelList.Add(info);
            LinkGo(targetGo, info.list);  //先在自己身上找一遍
            DealWith(info, targetGo);

            PanelList.Sort(PanelCompare);
        }
        allDc = 0;
        for (int i = 0; i < PanelList.Count; i++)
        {
            allDc += PanelList[i].Refresh();
        }
    }

    public void DealWith(UIDrawCallPanelInfo info, GameObject targetGo)
    {
        Transform p1 = targetGo.transform;
        for (int i = 0; i < p1.childCount; i++)  //处理儿子里面有panel的情况
        {
            Transform ts = p1.GetChild(i);
            //if (!ts.gameObject.activeInHierarchy && !ContainUnEnable)
            //    continue;
            UIPanel pl = ts.GetComponent<UIPanel>();
            if (pl != null)
            {
                UIDrawCallPanelInfo newInfo = new UIDrawCallPanelInfo(pl.gameObject);
                PanelList.Add(newInfo);
                LinkGo(ts.gameObject, newInfo.list);  //先在自己身上找一遍
                DealWith(newInfo, ts.gameObject);
            }
        }
        for (int i = 0; i < p1.childCount; i++)  //处理所有儿子里面的组件
        {
            Transform ts = p1.GetChild(i);
            //if (!ts.gameObject.activeInHierarchy && !ContainUnEnable)
            //    continue;
            UIPanel pl = ts.GetComponent<UIPanel>();
            if (pl != null)
                continue;
            LinkGo(ts.gameObject, info.list);
            if (ts.childCount != 0)
                DealWith(info, ts.gameObject);
        }
    }

    public static int Compare(UIDrawCallWidgetInfo x, UIDrawCallWidgetInfo y)
    {
        if (x.widget.depth < y.widget.depth) return -1;
        if (x.widget.depth > y.widget.depth) return 1;

        Material leftMat = x.widget.material;
        Material rightMat = y.widget.material;

        if (leftMat == rightMat) return 0;
        if (leftMat == null) return 1;
        if (rightMat == null) return -1;

        return (leftMat.GetInstanceID() < rightMat.GetInstanceID()) ? -1 : 1;
    }

    public static int PanelCompare(UIDrawCallPanelInfo x, UIDrawCallPanelInfo y)
    {
        return (x.uiPanel.depth < y.uiPanel.depth) ? -1 : 1;
    }

    public void LinkGo(GameObject go, List<UIDrawCallWidgetInfo> list)  //请求把go里面所有需要绘制的组件复制进list
    {
        UISprite[] sps = go.GetComponents<UISprite>();
        if (sps.Length != 0)
        {
            for (int i = 0; i < sps.Length; i++)
            {
                if (sps[i].atlas == null)
                    continue;
                //string name = string.Format("{0:D3}[sp][{1}][{2}]", sps[i].depth, sps[i].atlas.name, sps[i].name);
                //list.Add(CreateHelper(go, sps[i].depth, name, sps[i]));
                list.Add(new UIDrawCallWidgetInfo(sps[i], "Sprite", sps[i].atlas.name));
            }
        }

        UILabel[] las = go.GetComponents<UILabel>();
        if (las.Length != 0)
        {
            for (int i = 0; i < las.Length; i++)
            {
                if (las[i].ambigiousFont == null)
                    continue;
                //string name = string.Format("{0:D3}[la][{1}][{2}]", las[i].depth, las[i].ambigiousFont.name, las[i].name);
                //list.Add(CreateHelper(go, las[i].depth, name, las[i]));
                list.Add(new UIDrawCallWidgetInfo(las[i], "Label", las[i].ambigiousFont.name));
            }
        }

        UITexture[] tes = go.GetComponents<UITexture>();
        if (tes.Length != 0)
        {
            for (int i = 0; i < tes.Length; i++)
            {
                if (tes[i].mainTexture == null)
                {
                    //string name = string.Format("{0:D3}[te][{1}][{2}]", tes[i].depth, "null", tes[i].name);
                    //list.Add(CreateHelper(go, tes[i].depth, name, tes[i]));
                    list.Add(new UIDrawCallWidgetInfo(tes[i], "Texture", "null"));
                }
                else
                {
                    //string name = string.Format("{0:D3}[te][{1}][{2}]", tes[i].depth, tes[i].mainTexture.name, tes[i].name);
                    //list.Add(CreateHelper(go, tes[i].depth, name, tes[i]));
                    list.Add(new UIDrawCallWidgetInfo(tes[i], "Texture", tes[i].mainTexture.name));
                }
            }
        }
    }
}
