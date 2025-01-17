
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;
using System.Runtime.InteropServices;

public class Scene_Tools : EditorWindow {


    [MenuItem("ArtTools/Scene_Tools")]
    static void Init()
    {
        // Get existing open window or if none, make a new one:
        Scene_Tools window = (Scene_Tools)EditorWindow.GetWindow(typeof(Scene_Tools));
        window.Show();

    }
    Vector2 v;
    int Size = 1;
    string CombineName="CombinedMesh";
    void OnGUI()
    {
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.Space();

        v = GUILayout.BeginScrollView(v);

        GUILayout.BeginHorizontal();
        GUILayout.Label("场景Lightmap烘焙");
        if (GUILayout.Button("准备烘焙", GUILayout.MaxWidth(100)))
        {
            XEditor.AssetModify.PrepareBake();

        }
        if (GUILayout.Button("结束烘焙", GUILayout.MaxWidth(100)))
        {
            XEditor.AssetModify.EndBake();

        }
        GUILayout.EndHorizontal();

        EditorGUILayout.Space();

        GUILayout.BeginHorizontal();
        GUILayout.Label("CutOut转换");
        if (GUILayout.Button("PrcessSceneMat", GUILayout.MaxWidth(200)))
        {
            XEditor.AssetModify.PrcessSceneMat();

        }

        GUILayout.EndHorizontal();

        EditorGUILayout.Space();

        GUILayout.BeginHorizontal();
        GUILayout.Label("怪物行走绿格子");
        if (GUILayout.Button("Map Editor (Window)", GUILayout.MaxWidth(200)))
        {
            EditorWindow.GetWindow(typeof(XEditor.MapEditor));

        }

        GUILayout.EndHorizontal();

        EditorGUILayout.Space();

        GUILayout.BeginHorizontal();
        GUILayout.Label("2.5D半透明碰撞生成");
        if (GUILayout.Button("2.5D Model Collider Linker (Win)", GUILayout.MaxWidth(200)))
        {
            EditorWindow.GetWindow<XLinkerEditor>(@"Linker Editor");

        }

        GUILayout.EndHorizontal();

        EditorGUILayout.Space();

        GUILayout.BeginHorizontal();
        GUILayout.Label("FBX生成Prefab");
        if (GUILayout.Button("CreatePrefab", GUILayout.MaxWidth(200)))
        {
            AutoCreatePrefab.CreatePrefabFromFBX();
        }
        GUILayout.EndHorizontal();

        EditorGUILayout.Space();

        GUILayout.BeginHorizontal();
        GUILayout.Label("FBX压缩设置");
        if (GUILayout.Button("Model Settings (Window)", GUILayout.MaxWidth(200)))
        {
            EditorWindow.GetWindowWithRect<XResModelImportEditorWnd>(new Rect(0, 0, 900, 500), true, @"XRes Import Editor");
        }
        GUILayout.EndHorizontal();

        EditorGUILayout.Space();

        GUILayout.BeginHorizontal();
        GUILayout.Label("寻路生成辅助工具");
        if (GUILayout.Button("Navigation Helper (Window)", GUILayout.MaxWidth(200)))
        {
            EditorWindow.GetWindow(typeof(NavigationHelper));
        }
        GUILayout.EndHorizontal();

        EditorGUILayout.Space();

        GUILayout.BeginHorizontal();
        GUILayout.Label("Lightmap尺寸修改");
        if (GUILayout.Button("LightMapHelper (Window)", GUILayout.MaxWidth(200)))
        {
            EditorWindow.GetWindow(typeof(LightMapHelper));
        }
        GUILayout.EndHorizontal();

        EditorGUILayout.Space();

        GUILayout.BeginHorizontal();
        GUILayout.Label("Skybox添加到摄像机");
        if (GUILayout.Button("SkyboxHelper (Window)", GUILayout.MaxWidth(200)))
        {
            EditorWindow.GetWindow<SkyboxHelper>().Show();
        }
        GUILayout.EndHorizontal();

        EditorGUILayout.Space();

        GUILayout.BeginHorizontal();
        GUILayout.Label("添加脚本");
        if (GUILayout.Button("LoadGameAtHere", GUILayout.MaxWidth(200)))
        {
            GameObject[] gos = Selection.gameObjects;
            if (gos.Length > 0)
            {
                for (int i = 0; i < gos.Length; i++)
                {
                    if (!gos[i].GetComponent<LoadGameAtHere>())
                        gos[i].AddComponent<LoadGameAtHere>();
                }
            }
        }

        GUILayout.EndHorizontal();

        EditorGUILayout.Space();

        GUILayout.BeginHorizontal();
        GUILayout.Label("添加脚本");

        if (GUILayout.Button("ColliderShow", GUILayout.MaxWidth(200)))
        {
            GameObject[] gos = Selection.gameObjects;
            if (gos.Length > 0)
            {
                for (int i = 0; i < gos.Length; i++)
                {
                    if (!gos[i].GetComponent<ColliderShow>())
                        gos[i].AddComponent<ColliderShow>();
                }
            }
        }
        GUILayout.EndHorizontal();

        EditorGUILayout.Space();


        GUILayout.BeginHorizontal();
        GUILayout.Label("截屏");

        Size = EditorGUILayout.IntField(Size, GUILayout.MaxWidth(50));
        if (GUILayout.Button("CaptureSuperImage", GUILayout.MaxWidth(200)))
        {
            if (EditorApplication.isPlaying)
                ScreenCapture.CaptureScreenshot("Screenshot.png", Size);
            else
                EditorUtility.DisplayDialog("错误", "必须在运行状态下截屏", "OK");
        }
        GUILayout.EndHorizontal();

        EditorGUILayout.Space();


        GUILayout.BeginHorizontal();
        GUILayout.Label("合并模型");

        CombineName = EditorGUILayout.TextArea(CombineName, GUILayout.MaxWidth(100));
        if (GUILayout.Button("CombineMesh", GUILayout.MaxWidth(200)))
        { 
            GameObject[] GOS = Selection.gameObjects;
            List<MeshFilter> mfs = new List<MeshFilter>();
            for (int i = 0; i < GOS.Length; i++)
            {
                MeshFilter mf = GOS[i].GetComponent<MeshFilter>();
                if (mf)
                {
                    mfs.Add(mf);
                }
            }
            CombineInstance[] CI = new CombineInstance[mfs.Count];
            for (int i = 0; i < CI.Length; i++)
            {
                CI[i].mesh = mfs[i].sharedMesh;
                CI[i].transform = mfs[i].transform.localToWorldMatrix;
            }
            Mesh M = new Mesh();
            M.CombineMeshes(CI, true);
            // FolderBrowserDialog fb = new FolderBrowserDialog();   //创建控件并实例化
            string ScenePath = SceneManager.GetActiveScene().path;
            Debug.Log(ScenePath);
            if (ScenePath.Length > 0)
            {
                ScenePath = ScenePath.Remove(ScenePath.LastIndexOf('/') + 1);
            }
            else
            {
                ScenePath = "Assets/XScene/";
            }
            AssetDatabase.CreateAsset(M, ScenePath + CombineName + ".asset");

    }
        GUILayout.EndHorizontal();

        EditorGUILayout.Space();


        GUILayout.EndScrollView();


        //        LightParaGroup = EditorGUILayout.BeginToggleGroup("LightOptionsModify", LightParaGroup);
        //       na=  EditorGUILayout.TextField("Light Name",na);
        //        st = (shadowType)EditorGUILayout.EnumPopup("ShadowType", st);
        //        if(st !=shadowType.None)
        //            {
        //strength = EditorGUILayout.FloatField("ShadowStrength", strength);
        //        }


        //       if ( GUILayout.Button("Modify All Scene Light "))
        //        {


        //            // UnityEngine.Object[] curves = Selection.GetFiltered(typeof(TextAsset), SelectionMode.DeepAssets);

        //            string[] guids = AssetDatabase.FindAssets("t:Scene");
        //            SceneAsset[] SA = new SceneAsset[guids.Length];
        //            // List<string> paths = new List<string>(guids.Length);
        //            //guids.ToList().ForEach(m => paths.Add(AssetDatabase.GUIDToAssetPath(m)));
        //            for (int i=0;i<guids.Length;i++)
        //            {

        //                       // paths[i] = AssetDatabase.GUIDToAssetPath(guids[i]);

        //                       // SA[i] = AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(guids[i]),typeof(SceneAsset)) as SceneAsset;

        //                    Scene scene = EditorSceneManager.OpenScene(AssetDatabase.GUIDToAssetPath(guids[i]));
        //                //    SceneAsset _sa = AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(guids[i]), typeof(SceneAsset)) as SceneAsset;
        //                    Light[] lights = MonoBehaviour.FindObjectsOfType<Light>(); 
        //                    for(int j=0;j<lights.Length;j++)
        //                    {
        //                        if(lights[j].name== na)
        //                        {
        //                            Selection.activeGameObject = lights[j].gameObject;
        //                        lights[j].type = LightType.Directional;
        //                        lights[j].lightmappingMode = LightmappingMode.Realtime;
        //                            switch (st)
        //                            {
        //                                case shadowType.None:
        //                                    lights[j].shadows = LightShadows.None;
        //                                    break;
        //                                case shadowType.HardShadows:
        //                                    lights[j].shadows = LightShadows.Hard;
        //                                    lights[j].shadowStrength = strength;
        //                                    break;
        //                                case shadowType.SoftShadows:
        //                                    lights[j].shadows = LightShadows.Soft;
        //                                    lights[j].shadowStrength = strength;
        //                                    break;
        //                            }


        //                        }
        //                    }
        //                    EditorSceneManager.SaveScene(scene);
        //                Debug.Log(i + "------"+scene.name + "------修改完成");

        //                }
        //        }

        //        EditorGUILayout.EndToggleGroup();


        //        EditorGUILayout.Space();
        //        EditorGUILayout.Space();
        //        EditorGUILayout.Space();
        //        EditorGUILayout.Space();
        //        if (GUILayout.Button("test "))
        //        {
        //     //       XEditor.AssetModify.Compress();
        //        }





    }
}
