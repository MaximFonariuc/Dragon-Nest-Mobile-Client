using UnityEngine;
using UnityEditor;
using System.Collections;
using System.IO;

public class BathProcessingPrefab : MonoBehaviour
{
    [MenuItem(@"XEditor/Tools/BathProcessingPrefab")]
    static void Execute()
    {
        EditorWindow.GetWindowWithRect<BathProcessingPrefabEditor>(new Rect(300, 300, 350, 350), true, @"BathProcessingPrefab");
    }
}

[ExecuteInEditMode]
internal class BathProcessingPrefabEditor : EditorWindow
{
    private int TargetGroup;
    private Object TargetPrefab;

    void OnGUI()
    {
        TargetPrefab = EditorGUILayout.ObjectField( "选择目标Prefab" ,TargetPrefab, typeof(GameObject),true);

        GUILayout.BeginHorizontal();
        TargetGroup = EditorGUILayout.IntField("Group", TargetGroup);
        GUILayout.EndHorizontal();

        if (GUILayout.Button("替换子物件", GUILayout.MaxWidth(80)))
        {
            if (TargetGroup > 0 && TargetPrefab != null)
            {
                ChangePrefab();
            }
            else
            {
                if (TargetPrefab == null)
                    ShowNotification(new GUIContent("Prefab不能为空！"));
                else
                    ShowNotification(new GUIContent("目标组必须为正整数！"));
            }
        }
    }

    private void ChangePrefab()
    {
        GameObject TargetGo = TargetPrefab as GameObject;

        Vector3 pos = TargetGo.transform.localPosition;
        Vector3 scale = TargetGo.transform.localScale;

        foreach (Object o in Selection.GetFiltered(typeof(Object), SelectionMode.DeepAssets))
        {
            if (AssetDatabase.GetAssetPath(o).Contains(".prefab"))
            {
                bool change = false;
                GameObject go = PrefabUtility.InstantiatePrefab(o) as GameObject;

                GroupManager[] gmList = go.GetComponentsInChildren<GroupManager>(true);
                foreach (GroupManager gm in gmList)
                {
                    if(gm == null)
                    {
                        Debug.LogError("GroupManager is null!!! maybe his father has GroupManager.");
                        continue;
                    }
                    if(gm.Group == TargetGroup)
                    {
                        int index = gm.transform.childCount - 1;
                        if (index == -1)
                        {
                            Debug.LogError("Error! son num is 0!!!");
                            continue;
                        }

                        for (int i = index; i >=0; i--)
                        {
                            if (gm.transform.GetChild(i) == null)
                                Debug.LogError("Error! son is null!");
                            DestroyImmediate(gm.transform.GetChild(i).gameObject);  //删除
                        }

                        change = true;
                        string name = TargetGo.name;
                        GameObject son = Instantiate(TargetGo) as GameObject;
                        son.name = name;
                        son.transform.parent = gm.transform;
                        son.transform.localPosition = pos;
                        son.transform.localScale = scale;
                    }
                }

                if(change)
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
}