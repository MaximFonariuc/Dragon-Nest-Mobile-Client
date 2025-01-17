using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
using System.Text;
using XUtliPoolLib;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
public class FxInfo
{
    public string name;
    public Vector3 pos;
    public Vector3 rot;
    public Vector3 scale;
}

public class SceneFxHelper : EditorWindow
{
    [SerializeField]
    static SceneFxHelper _windowInstance;

    private static GUIContent
        ExtractFxContent = new GUIContent("提取场景特效信息", "generate map data");

    private static GUIContent
        LoadFxContent = new GUIContent("载入场景特效", "load mapheight file");

    List<FxInfo> _FxInfo = new List<FxInfo>();

    //private SceneFxTable _data_table = null;

    private string _scene_name = "";

    [MenuItem(@"XEditor/SceneFxHelper Editor")]
    static void Init()
    {
        //Debug.Log ("init start");
        _windowInstance = (SceneFxHelper)EditorWindow.GetWindow(typeof(SceneFxHelper));
        _windowInstance.Show();
    }

    public void OnEnable()
    {
        //if (_data_table == null)
        //{
        //    _data_table = new SceneFxTable();
        //    LoadFile("Assets/Table/SceneFxTable.txt", _data_table);
        //}
    }
    
    public void OnGUI()
    {
        EditorGUILayout.Space();
        if (GUILayout.Button(ExtractFxContent))
        {
            ExtractFxData();
        }
        EditorGUILayout.Space();

        if (GUILayout.Button(LoadFxContent))
        {
            LoadFxData();
        }
    }

    private void ExtractFxData()
    {
        GetCurrentSceneName();

        if (GetCurrentSceneFxData())
        {
            MergeTableData();
            SaveCurrentSceneToFile();
        }

        EditorUtility.DisplayDialog("完成", "完成", "好");
    }

    private void GetCurrentSceneName()
    {
        Scene scene = EditorSceneManager.GetActiveScene();
        string sceneName = scene.name;

        int l = sceneName.LastIndexOf('/');
        int l2 = sceneName.LastIndexOf('.');
        _scene_name = sceneName.Substring(l + 1, l2 - l - 1);
    }

    private bool GetCurrentSceneFxData()
    {
        GameObject FxRoot = GameObject.Find("Scene/Effect");
        if (FxRoot == null)
            FxRoot = GameObject.Find("Scene/effect");

        if (FxRoot == null) return false;

        _FxInfo.Clear();

        for (int i = 0; i < FxRoot.transform.childCount; i++)
        {
            Transform fx = FxRoot.transform.GetChild(i);

            if (!fx.gameObject.activeInHierarchy) continue;

            FxInfo o = new FxInfo();
            o.name = fx.name;
            o.pos = fx.localPosition;
            o.rot = fx.localRotation.eulerAngles;
            o.scale = fx.localScale;

            _FxInfo.Add(o);
        }

        for (int i = FxRoot.transform.childCount - 1; i >= 0; i--)
        {
            Transform fx = FxRoot.transform.GetChild(i);

            GameObject.DestroyImmediate(fx.gameObject);
        }
        Scene scene = EditorSceneManager.GetActiveScene();
        EditorSceneManager.SaveScene(scene);
        //EditorApplication.SaveScene();
        return true;
    }

    private void MergeTableData()
    {
        //for (int i = _data_table.Table.Count - 1; i >= 0; i--)
        //{
        //    if (_data_table.Table[i].unityscene == _scene_name)
        //    {
        //        _data_table.Table.RemoveAt(i);
        //    }
        //}

        //for (int i = 0; i < _FxInfo.Count; i++)
        //{
        //    SceneFxTable.RowData data = new SceneFxTable.RowData();
        //    data.unityscene = _scene_name;
        //    data.name = _FxInfo[i].name;
        //    data.position = Vector3ToArray(_FxInfo[i].pos);
        //    data.rotation = Vector3ToArray(_FxInfo[i].rot);
        //    data.scale = Vector3ToArray(_FxInfo[i].scale);

        //    _data_table.Table.Add(data);
        //}
    }

    private void SaveCurrentSceneToFile()
    {
        //string path = "./Assets/Table/SceneFxTable.txt";

        //using (FileStream writer = new FileStream(path, FileMode.Truncate))
        //{
        //    StreamWriter sw = new StreamWriter(writer, Encoding.Unicode);

        //    sw.WriteLine("unityscene" + '\t' + "name" + '\t' + "position" + '\t' + "rotation" + '\t' + "scale");
        //    sw.WriteLine("unityscene" + '\t' + "name" + '\t' + "position" + '\t' + "rotation" + '\t' + "scale");

        //    for (int i = 0; i < _data_table.Table.Count; i++)
        //    {
        //        string line = "";

        //        line += _data_table.Table[i].unityscene;
        //        line += '\t';

        //        line += _data_table.Table[i].name;
        //        line += '\t';

        //        line += ArrayToString(_data_table.Table[i].position);
        //        line += '\t';

        //        line += ArrayToString(_data_table.Table[i].rotation);
        //        line += '\t';

        //        line += ArrayToString(_data_table.Table[i].scale);

        //        sw.WriteLine(line);
        //    }
        //    sw.Flush();
        //    sw.Close();
        //}
    }

    private void LoadFxData()
    {

    }

    private string ArrayToString(float[] v)
    {
        return v[0].ToString() + '|' + v[1].ToString() + '|' + v[2].ToString();
    }

    private float[] Vector3ToArray(Vector3 v)
    {
        float[] ret = new float[3];
        ret[0] = v.x;
        ret[1] = v.y;
        ret[2] = v.z;
        return ret;
    }

    private void LoadFile(string location, CVSReader reader)
    {
        //TextAsset data = null;
        //data = AssetDatabase.LoadAssetAtPath(location, typeof(TextAsset)) as TextAsset;// Resources.Load<TextAsset>(location);

        //using (Stream s = new System.IO.MemoryStream(data.bytes))
        //{
        //    reader.LoadFile(s, false);
        //}
    }
}
