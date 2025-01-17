using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;

public class AssembleUI : MonoBehaviour
{
    [MenuItem(@"XEditor/Tools/AssembleUI")]
    static void Execute()
    {
        EditorWindow.GetWindowWithRect<AssembleUIEditor>(new Rect(300, 300, 350, 350), true, @"AssembleUI");
    }
}
[ExecuteInEditMode]
internal class AssembleUIEditor : EditorWindow
{

    private Dictionary<string, string> PathDict = new Dictionary<string, string>();
    private GameObject MainGameObject;
    private Object MainPrefab;

    private List<Object> FileList = new List<Object>();
    private List<GameObject> SaveList = new List<GameObject>();
    private List<Object> PrefabList = new List<Object>();

    private List<List<string>> PrefabInfo = new List<List<string>>();
    private List<string> TitleList = new List<string>();

    private bool _init = false;

    void OnGUI()
    {
        if (!_init)
        {
            _init = true;
            LoadInfo();
        }

        GUILayout.Label("不要关闭此窗口，否则会无法保存");
        GUILayout.Label("每次只保存最后一个Load的UI");

        if (GUILayout.Button("保存", GUILayout.MaxWidth(350)))
        {
            SavePrefab();
        }

        EditorGUILayout.BeginHorizontal();
        GUILayout.Label(" ");
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        for (int i = 0; i < PrefabInfo.Count; i++ )
        {
            if (GUILayout.Button(TitleList[i], GUILayout.MaxWidth(100)))
            {
                GetFileList(i);
                SetMainPrefab(i);
                SetSubPrefab(i);
            }

            if(i % 3 == 2)  // 换行
            {
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.BeginHorizontal();
            }
        }


        EditorGUILayout.EndHorizontal();
    }

    private void GetFileList(int index)
    {
        string[] arrStrAudioPath = Directory.GetFiles(Application.dataPath + PrefabInfo[index][1], "*", SearchOption.AllDirectories);

        foreach (string strAudioPath in arrStrAudioPath)
        {
            //替换路径中的反斜杠为正斜杠       
            string strTempPath = strAudioPath.Replace(@"\", "/");
            //截取我们需要的路径
            strTempPath = strTempPath.Substring(strTempPath.IndexOf("Assets"));
            //根据路径加载资源
            Object obj = AssetDatabase.LoadAssetAtPath(@strTempPath, typeof(Object));
            if (!AssetDatabase.GetAssetPath(obj).Contains(".meta"))
                FileList.Add(obj);
        }
    }

    private void SetMainPrefab(int index)
    {
        for (int i = 0; i < FileList.Count; i++)
        {
            Object o = FileList[i];
            if (AssetDatabase.GetAssetPath(o).Contains(".prefab") && o.name == PrefabInfo[index][0])
            {
                MainGameObject = PrefabUtility.InstantiatePrefab(o) as GameObject;
                MainGameObject.transform.SetParent(GameObject.Find("UIRoot").transform, false);
                MainGameObject.transform.localScale = Vector3.one;

                MainPrefab = (o == null) ? PrefabUtility.CreateEmptyPrefab(AssetDatabase.GetAssetPath(o)) : (o as GameObject);
            }
        }
    }

    private void SetSubPrefab(int index)
    {
        SaveList.Clear();
        PrefabList.Clear();

        List<Object> tempList = new List<Object>();
        string path;

        PathDict.Clear();
        for (int i = 2; i < PrefabInfo[index].Count; i += 2)
        {
            PathDict[PrefabInfo[index][i]] = PrefabInfo[index][i + 1];
        }
        

        for(int i = 0; i < 3; i++)
        {
            for (int j = 0; j < FileList.Count; j++)
            {
                Object o = FileList[j];
                if (AssetDatabase.GetAssetPath(o).Contains(".prefab") && PathDict.TryGetValue(o.name, out path))
                {
                    if (MainGameObject.transform.Find(path) == null)
                    {
                        tempList.Add(o);
                    }
                    else
                    {
                        GameObject go = PrefabUtility.InstantiatePrefab(o) as GameObject;
                        go.transform.SetParent(MainGameObject.transform.Find(path), false);
                        go.transform.localScale = Vector3.one;
                        SaveList.Add(go);

                        Object prefab = (o == null) ? PrefabUtility.CreateEmptyPrefab(AssetDatabase.GetAssetPath(o)) : (o as GameObject);
                        PrefabList.Add(prefab);
                    }
                }
            }

            FileList.Clear();
            for (int j = 0; j < tempList.Count; j++)
                FileList.Add(tempList[j]);
            tempList.Clear();
        }
    }

    private void SavePrefab()
    {
        if(SaveList.Count == 0)
        {
            ShowNotification(new GUIContent("保存失败"));
            return;
        }

        for (int i = SaveList.Count - 1; i >= 0; i--)
        {
            PrefabUtility.ReplacePrefab(SaveList[i], PrefabList[i]);

            DestroyImmediate(SaveList[i]);
        }

        PrefabUtility.ReplacePrefab(MainGameObject, MainPrefab);

        DestroyImmediate(MainGameObject);


        ShowNotification(new GUIContent("保存成功"));
    }

    private void LoadInfo()
    {
        PrefabInfo.Clear();
        TitleList.Clear();

        var fileAddress = System.IO.Path.Combine(Application.persistentDataPath, "AssembleUI.txt");
        FileInfo fInfo0 = new FileInfo(fileAddress);

        List<string> StrList;
        bool saveEnd = false;
        bool isNew = true;

        if (fInfo0.Exists)
        {
            StreamReader r;
            try
            {
                r = new StreamReader(fileAddress, System.Text.Encoding.Default);
            }
            catch (System.IO.IOException e)
            {
                Debug.Log(e);
                ShowNotification(new GUIContent("读表失败，请关闭正在打开的表格重试"));
                return;
            }

            StrList = new List<string>();
            while (!r.EndOfStream)
            {
                string oneLine = r.ReadLine();
                if(oneLine.Length == 0 || oneLine[0] == '\t')
                {
                    if(StrList.Count != 0)
                    {
                        PrefabInfo.Add(StrList);
                        StrList = new List<string>();
                    }
                    isNew = true;
                    saveEnd = true;
                    continue;
                }
                string[] str = oneLine.Split('\t');
                if(isNew)
                {
                    isNew = false;
                    TitleList.Add(str[0]);
                }
                else
                {
                    StrList.Add(str[0]);
                    StrList.Add(str[1]);
                    saveEnd = false;
                }
            }
            if(!saveEnd)
            {
                PrefabInfo.Add(StrList);
            }
            r.Close();
        }
    }
}