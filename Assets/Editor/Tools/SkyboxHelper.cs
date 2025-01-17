using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using XUtliPoolLib;

public class SkyboxHelper : EditorWindow
{
    [MenuItem("XEditor/Tools/SkyboxHelper")]
    static void Init()
    {
        EditorWindow.GetWindow<SkyboxHelper>().Show();
    }

    void OnGUI()
    {
        if (GUILayout.Button("提取天空盒"))
        {
            SaveSkyboxToPath();
        }

        if (GUILayout.Button("预览天空盒"))
        {
            ApplySkybox();
        }

        GUILayout.Space(50);


        if (GUILayout.Button("RenderSetting加入到最后"))
        {
            RenderSettingAddLast();
        }

        GUILayout.Space(50);


        if (GUILayout.Button("所有场景天空盒刷刷刷"))
        {
            DoAll();
        }
    }

    void SaveSkyboxToPath()
    {
        GameObject go = GameObject.Find(@"Main Camera");

        if (go != null)
        {
            EnverinmentSetting env = go.GetComponent<EnverinmentSetting>();

            if (env == null)
            {
                env = go.AddComponent<EnverinmentSetting>();
            }

            if (env.envs == null)
            {
                env.envs = new List<EnverinmentSetting.EnvInfo>();
            }

            EnverinmentSetting.EnvInfo ei = new EnverinmentSetting.EnvInfo();

            ei.fog = RenderSettings.fog;
            ei.fogColor = RenderSettings.fogColor;
            ei.fogMode = RenderSettings.fogMode;
            ei.fogDensity = RenderSettings.fogDensity;
            ei.fogStartDistance = RenderSettings.fogStartDistance;
            ei.fogEndDistance = RenderSettings.fogEndDistance;

            if (RenderSettings.skybox != null)
            {
                ei.skyboxPath = AssetDatabase.GetAssetPath(RenderSettings.skybox);
                ei.skyboxPath = ei.skyboxPath.Replace("Assets/Resources/", "");
                ei.skyboxPath = ei.skyboxPath.Replace(".mat", "");
                ei.skybox = XResourceLoaderMgr.singleton.GetSharedResource<Material>(ei.skyboxPath, ".mat");
            }
            else
            {
                if (env.envs.Count > 0)
                {
                    ei.skyboxPath = env.envs[0].skyboxPath;
                    ei.skybox = env.envs[0].skybox;
                }
                
            }

            ei.ambientLight = RenderSettings.ambientLight;

            if (ei.skyboxPath != null)
            {
                if (env.envs.Count == 0)
                    env.envs.Add(ei);
                else if (env.envs.Count == 1)
                    env.envs[0] = ei;

                env.enabled = true;
            }
            else
            {
                env.enabled = false;
            }
            
        }

        RenderSettings.skybox = null;
    }

    void ApplySkybox()
    {
        GameObject go = GameObject.Find(@"Main Camera");
        if (go != null)
        {
            EnverinmentSetting env = go.GetComponent<EnverinmentSetting>();

            if (env != null)
            {
                env.EnableSetting(0);
            }
        }
    }

    void DoAll()
    {
        EditorBuildSettingsScene[] scenes = EditorBuildSettings.scenes;

        for (int i = 0; i < scenes.Length; i++)
        {
            EditorBuildSettingsScene scene = scenes[i];
            Debug.Log(scene.path);

            UnityEngine.SceneManagement.Scene s = UnityEditor.SceneManagement.EditorSceneManager.OpenScene(scene.path);
            //if (EditorApplication.OpenScene(scene.path))
            {
                ApplySkybox();
                SaveSkyboxToPath();
                UnityEditor.SceneManagement.EditorSceneManager.SaveScene(s);
                //EditorApplication.SaveScene(scene.path);
            }

        }
    }

    void RenderSettingAddLast()
    {
        GameObject go = GameObject.Find(@"Main Camera");
        if (go != null)
        {
            EnverinmentSetting env = go.GetComponent<EnverinmentSetting>();

            if (env != null)
            {
                EnverinmentSetting.EnvInfo ei = new EnverinmentSetting.EnvInfo();

                ei.fog = RenderSettings.fog;
                ei.fogColor = RenderSettings.fogColor;
                ei.fogMode = RenderSettings.fogMode;
                ei.fogDensity = RenderSettings.fogDensity;
                ei.fogStartDistance = RenderSettings.fogStartDistance;
                ei.fogEndDistance = RenderSettings.fogEndDistance;

                if (RenderSettings.skybox != null)
                {
                    ei.skyboxPath = AssetDatabase.GetAssetPath(RenderSettings.skybox);
                    ei.skyboxPath = ei.skyboxPath.Replace("Assets/Resources/", "");
                    ei.skyboxPath = ei.skyboxPath.Replace(".mat", "");
                    ei.skybox = XResourceLoaderMgr.singleton.GetSharedResource<Material>(ei.skyboxPath, ".mat");
                }

                ei.ambientLight = RenderSettings.ambientLight;

                if (env.envs == null)
                {
                    env.envs = new List<EnverinmentSetting.EnvInfo>();
                }

                env.envs.Add(ei);
            }
        }

        RenderSettings.skybox = null;
    }
}
