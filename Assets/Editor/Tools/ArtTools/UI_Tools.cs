
using System;
using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;
using System.Runtime.InteropServices;

public class UI_Tools : EditorWindow {

    [DllImport("user32.dll", EntryPoint = "keybd_event")]
    public static extern void keybd_event(

        byte bVk,    //虚拟键值 对应按键的ascll码十进制值  

        byte bScan,// 0  

        int dwFlags,  //0 为按下，1按住，2为释放  

        int dwExtraInfo  // 0  

    );

    [MenuItem("ArtTools/UI_Tools")]
    static void Init()
    {
        // Get existing open window or if none, make a new one:
        UI_Tools window = (UI_Tools)EditorWindow.GetWindow(typeof(UI_Tools));
        window.Show();


    }
    Vector2 v;
    string itemPath;
    //private  string na;
    //private float strength=1.0f;
    //private bool MainlLightNoNull=true;
    //bool LightParaGroup;
    //private enum shadowType
    //{
    //    None,
    //    HardShadows,
    //    SoftShadows
    //}
    //private shadowType st =shadowType.HardShadows;
    void OnGUI()
    {
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        
        v= GUILayout.BeginScrollView(v);

        GUILayout.BeginHorizontal();
        GUILayout.Label("Altas目录");
        if (GUILayout.Button("Show in Explorer", GUILayout.MaxWidth(200)))
        {
            EditorUtility.RevealInFinder("Assets/Resources/atlas/UI");

        }
        GUILayout.EndHorizontal();
        EditorGUILayout.Space();
        GUILayout.BeginHorizontal();
        GUILayout.Label("图转统一格式");
        if (GUILayout.Button("Compress Separate Alpha",GUILayout.MaxWidth(200)))
        {
            XEditor.AssetModify.CompressSeparateAlpha();
        }
        GUILayout.EndHorizontal();

        EditorGUILayout.Space();

        GUILayout.BeginHorizontal();
        GUILayout.Label("Prefab转路径");
        if (GUILayout.Button("Reference 2 Path",  GUILayout.MaxWidth(200)))
        {
            UIReferenceToPathEditor.Reference2Path();
        }
        GUILayout.EndHorizontal();
        EditorGUILayout.Space();

        GUILayout.BeginHorizontal();
        GUILayout.Label("路径转回");
        if (GUILayout.Button("Path 2 Reference", GUILayout.MaxWidth(200)))
        {
            UIReferenceToPathEditor.Path2Reference();
        }
        GUILayout.EndHorizontal();
        EditorGUILayout.Space();
        GUILayout.BeginHorizontal();
        GUILayout.Label("批量替换图集");
        if (GUILayout.Button("Change Altas Sprite  (window)",  GUILayout.MaxWidth(200)))
        {
            ChangeAtlasSpriteEditor ChangeAtlasSpriteWin = (ChangeAtlasSpriteEditor)EditorWindow.GetWindow(typeof(ChangeAtlasSpriteEditor));
          
        }
		GUILayout.EndHorizontal();
		EditorGUILayout.Space();
		GUILayout.BeginHorizontal();
		GUILayout.Label("移动图集并更新UI");
		if (GUILayout.Button("AtlasMerge  (window)",  GUILayout.MaxWidth(200)))
		{
			AtlasMerge ChangeAtlasSpriteWin = (AtlasMerge)EditorWindow.GetWindow(typeof(AtlasMerge));

		}
        GUILayout.EndHorizontal();
        EditorGUILayout.Space();
        GUILayout.BeginHorizontal();
        GUILayout.Label("UI层级调整");
        if (GUILayout.Button("UI DrawCall  (window)", GUILayout.MaxWidth(200)))
        {
			XUIDrawCallWindow.Execute ();
        }

		GUILayout.EndHorizontal();
		EditorGUILayout.Space();
		GUILayout.BeginHorizontal();
		GUILayout.Label("查看Prefab图集与层级");
		if (GUILayout.Button("Depth UI  (window)", GUILayout.MaxWidth(200)))
		{
			XUIDepthWindow.Execute();
		}

        GUILayout.EndHorizontal();
        EditorGUILayout.Space();
        GUILayout.BeginHorizontal();
        GUILayout.Label("查看图压缩");
        if (GUILayout.Button("Texture Status  (window)", GUILayout.MaxWidth(200)))
        {
            TextureStatus TextureStatusWin = (TextureStatus)EditorWindow.GetWindow(typeof(TextureStatus), true, "TextureStatus");
        }
        GUILayout.EndHorizontal();
        EditorGUILayout.Space();
        GUILayout.BeginHorizontal();
        GUILayout.Label("查看Prefab依赖");
        if (GUILayout.Button("Select Dependencies", GUILayout.MaxWidth(200)))
        {
            object[] objs = Selection.objects;
            string[] paths = new string[objs.Length];
            for (int i=0;i<objs.Length;i++)
            {
                 paths[i] = AssetDatabase.GetAssetPath(objs[i] as UnityEngine.Object);
            }

            string[] path = AssetDatabase.GetDependencies( paths );
            for (int i=0;i<path.Length;i++)
            {
                Debug.Log(path[i]);
            }
            UnityEngine.Object[] o = new UnityEngine.Object[path.Length];
            for (int i=0;i<path.Length;i++)
            {
                o[i] = AssetDatabase.LoadAssetAtPath(path[i],typeof(UnityEngine.Object));
            }
            Selection.objects=o;
        }
        GUILayout.EndHorizontal();

        EditorGUILayout.Space();

        GUILayout.BeginHorizontal();
        GUILayout.Label("性能分析");
        if (GUILayout.Button("Profiler  (window)", GUILayout.MaxWidth(200)))
        {
            keybd_event(17, 0, 0, 0);
            keybd_event(55, 0, 0, 0);
            //keybd_event(17, 0, 1, 0);
            //keybd_event(55, 0, 1, 0);
            keybd_event(17, 0, 2, 0);
            keybd_event(55, 0, 2, 0);

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
