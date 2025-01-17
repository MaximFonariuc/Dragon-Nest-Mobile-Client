using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public class CubeDataView : EditorWindow
{
    [MenuItem(@"Assets/Tool/Test/ParseCubeData")]
    private static void ParseCubeData()
    {
        CubeDataView w = EditorWindow.GetWindow<CubeDataView>();
        w.minSize = new Vector2(800, 600);
        w.Show();
    }

    public class CubeData
    {
        public string Name = "";
        public string Type = "";
        public long Size = 0;
        public string SizeStr = "";
    }
    private TextAsset m_Txt = null;
    private List<object> rows = new List<object>();
    private TableView _table;
    private int Count = 0;
    private long TotalSize = 0;

    private int Start = 0;
    private int End = 0;

    private int RangeCount = 0;
    private long RangeSize = 0;
    void Awake()
    {
        this.titleContent = new GUIContent("CubeData");
        // create the table with a specified object type
        _table = new TableView(this, typeof(CubeData));

        // setup the description for content
        _table.AddColumn("Name", "Name", 0.3f, TextAnchor.MiddleLeft);
        _table.AddColumn("Type", "Type", 0.3f);
        _table.AddColumn("Size", "Size", 0.2f, TextAnchor.MiddleCenter, "0.000");
        _table.AddColumn("SizeStr", "KB", 0.1f, TextAnchor.MiddleCenter, "0.00");

        UnityEngine.Object[] txts = Selection.GetFiltered(typeof(TextAsset), SelectionMode.DeepAssets);
        if (txts != null && txts.Length > 0)
        {
            m_Txt = txts[0] as TextAsset;
            Parse();
        }
        // register the event-handling function
        //_table.OnSelected += TableView_Selected;
    }
    void Parse()
    {
        TotalSize = 0;
        Count = 0;
        rows.Clear();
        if (m_Txt != null && _table != null)
        {

            using (MemoryStream ms = new MemoryStream(m_Txt.bytes))
            {
                StreamReader sr = new StreamReader(ms);
                while (true)
                {
                    string line = sr.ReadLine();
                    Dictionary<string, object> dict = MiniJSON.Json.Deserialize(line) as Dictionary<string, object>;
                    if (dict != null)
                    {
                        CubeData cd = new CubeData();
                        object data = null;
                        dict.TryGetValue("name", out data);
                        cd.Name = data as string;
                        data = null;
                        dict.TryGetValue("type", out data);
                        cd.Type = data as string;
                        data = null;
                        dict.TryGetValue("size", out data);
                        cd.Size = (long)data;
                        TotalSize += cd.Size;
                        cd.SizeStr = (cd.Size / 1024).ToString();
                        rows.Add(cd);
                    }
                    if (sr.EndOfStream)
                        break;
                }

            }
            Count = rows.Count;
            _table.RefreshData(rows, 2);
        }
    }
    void Calc()
    {
        RangeCount = 0;
        RangeSize = 0;
        int start = Start - 1;
        int end = End;
        if (start < 0)
        {
            start = 0;
        }
        if (end <= start)
        {
            end = Count;
        }
        for (int i = start; i < end; ++i)
        {
            CubeData cd = rows[i] as CubeData;
            RangeCount++;
            RangeSize += cd.Size;

        }
    }
    void OnGUI()
    {
        GUILayout.BeginHorizontal();
        m_Txt = EditorGUILayout.ObjectField("", m_Txt, typeof(UnityEngine.TextAsset), true, GUILayout.MaxWidth(250)) as TextAsset;
        if (GUILayout.Button("Generate", GUILayout.MaxWidth(150)))
        {
            Parse();
        }
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        EditorGUILayout.LabelField(string.Format("Count:{0} TotalSize:{1}KB-{2}MB ", Count, TotalSize / 1024, TotalSize / 1024 / 1024), GUILayout.MaxWidth(400));
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();

        Start = EditorGUILayout.IntField("Start", Start, GUILayout.MaxWidth(200));
        End = EditorGUILayout.IntField("End", End, GUILayout.MaxWidth(200));
        if (GUILayout.Button("CalcSize", GUILayout.MaxWidth(120)))
        {
            Calc();
        }
        GUILayout.EndHorizontal();
        EditorGUILayout.LabelField(string.Format("Frome {0} To {1} Count: {2} TotalSize:{3}KB-{4}MB ", Start, End, RangeCount, RangeSize / 1024, RangeSize / 1024 / 1024), GUILayout.MaxWidth(400));
        GUILayout.BeginHorizontal();
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();

        GUILayout.BeginVertical();

        GUILayout.BeginArea(new Rect(20, 80, position.width * 0.9f, position.height - 80));
        if (_table != null)
            _table.Draw(new Rect(0, 0, position.width * 0.9f, position.height - 80));
        GUILayout.EndArea();

        GUILayout.EndVertical();
        GUILayout.EndHorizontal();
    }

    //void TableView_Selected(object selected, int col)
    //{
    //    FooItem foo = selected as FooItem;
    //    if (foo == null)
    //    {
    //        Debug.LogErrorFormat("the selected object is not a valid one. ({0} expected, {1} got)",
    //            typeof(FooItem).ToString(), selected.GetType().ToString());
    //        return;
    //    }

    //    string text = string.Format("object '{0}' selected. (col={1})", foo.Name, col);
    //    Debug.Log(text);
    //    ShowNotification(new GUIContent(text));
    //}

    void OnDestroy()
    {
        if (_table != null)
            _table.Dispose();

        _table = null;
    }


}
