using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;

[CustomEditor(typeof(EnverinmentSetting))]
public class EnverinmentEditor : Editor
{
    private SerializedObject _serializedObj;

    private SerializedProperty _envs;

    private EnverinmentSetting _obj;

    private GUIStyle _labelstyle = null;
    private GUILayoutOption[] _line = new GUILayoutOption[] { GUILayout.ExpandWidth(true), GUILayout.Height(1) };
    private GUIStyle _buttonPresslstyle = null;
    private int decorateSize = 0;

    void OnEnable()
    {
        _obj = target as EnverinmentSetting;
        _obj.EnableSetting(0);        
    }

    public override void OnInspectorGUI()
    {
        if (_labelstyle == null)
        {
            _labelstyle = new GUIStyle(EditorStyles.boldLabel);
            _labelstyle.fontSize = 13;
        }
        if (_buttonPresslstyle == null)
        {
            _buttonPresslstyle = new GUIStyle(GUI.skin.button);
            _buttonPresslstyle.normal = _buttonPresslstyle.active;
        }
        EditorGUI.BeginChangeCheck();
        GUILayout.Label("TerrainEditor", EditorStyles.boldLabel);
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("PrepareTerrain", GUILayout.MaxWidth(100)))
        {
            _obj.PrepareTerrain();
        }
        //if (GUILayout.Button("ConvertTerrain", GUILayout.MaxWidth(100)))
        //{
        //    _obj.ConvertTerrain();
        //}
        if (GUILayout.Button("EndConvertTerrain", GUILayout.MaxWidth(150)))
        {
            _obj.EndConvertTerrain();
        }
        EditorGUILayout.EndHorizontal();
        
        if (_obj.sceneConfig !=null)
        {
            for (int y = 0; y < _obj.sceneConfig.terrainBlock; ++y)
            {
                EditorGUILayout.BeginHorizontal(GUILayout.Width(240));
                for (int x = 0; x < _obj.sceneConfig.terrainBlock; ++x)
                {
                    int terrainIndex = y * _obj.sceneConfig.terrainBlock + x;
                    EditorGUILayout.BeginVertical(GUILayout.Width(60));
                    if (terrainIndex == _obj.selectBlockIndex)
                    {
                        if (GUILayout.Button("Block_" + terrainIndex.ToString(), _buttonPresslstyle, GUILayout.Width(55), GUILayout.Height(55)))
                        {
                            _obj.selectBlockIndex = -1;
                        }
                    }
                    else
                    {
                        if (GUILayout.Button("Block" + terrainIndex.ToString(), GUILayout.Width(55), GUILayout.Height(55)))
                        {
                            _obj.selectBlockIndex = terrainIndex;
                            _obj.editLodArea = null;
                        }
                    }

                    EditorGUILayout.EndVertical();
                }
                EditorGUILayout.EndHorizontal();
            }

            if (_obj.selectBlockIndex >= 0 && _obj.selectBlockIndex < _obj.sceneConfig.terrainMeshInfo.Length)
            {
                SceneConfig.TerrainMeshInfo tmi = _obj.sceneConfig.terrainMeshInfo[_obj.selectBlockIndex];
                EditorGUILayout.BeginHorizontal();
                tmi.enable = GUILayout.Toggle(tmi.enable, "EnableBlock", GUILayout.Width(100));
                GUILayout.Label(string.Format("Lod {0}", tmi.lod));
                tmi.lod = (int)GUILayout.HorizontalSlider(tmi.lod, 0, 3, GUILayout.Width(200));
                if (GUILayout.Button("Refresh"))
                {
                    _obj.ConvertTerrainBlock();
                }
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("Add LodArea",GUILayout.Width(100)))
                {
                    tmi.AddLodArea();
                }
                EditorGUILayout.EndHorizontal();
                if (tmi.lodArea != null)
                {
                    EditorGUILayout.BeginHorizontal();
                    for (int i = 0; i < tmi.lodArea.Count; ++i)
                    {
                        SceneConfig.LodArea lodArea = tmi.lodArea[i];
                        GUILayout.Label(string.Format("Area {0} Lod {1}", lodArea.area, lodArea.lod));
                        lodArea.lod = (int)GUILayout.HorizontalSlider(lodArea.lod, 0, 3, GUILayout.Width(100));
                        if (_obj.editLodArea == lodArea)
                        {
                            if (GUILayout.Button("Edit", _buttonPresslstyle, GUILayout.Width(50)))
                            {
                                _obj.editLodArea = null;
                            }
                        }
                        else
                        {
                            if (GUILayout.Button("Edit", GUILayout.Width(50)))
                            {
                                _obj.editLodArea = lodArea;
                            }
                        }

                        if (GUILayout.Button("X"))
                        {
                            tmi.RemoveLodArea(i);
                            break;
                        }
                    }
                    EditorGUILayout.EndHorizontal();
                }
            }

        }


        EditorGUILayout.BeginHorizontal();
        _obj.terrainMat = EditorGUILayout.ObjectField("TerrainMat", _obj.terrainMat, typeof(Material), true) as Material;
        EditorGUILayout.EndHorizontal();
        GUILayout.Space(5);
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("LightmapEditor", EditorStyles.boldLabel);
        if (GUILayout.Button("Add", GUILayout.MaxWidth(50)))
        {
            if(_obj.envs==null)
            {
                _obj.envs = new List<EnverinmentSetting.EnvInfo>();
            }
            _obj.envs.Add(new EnverinmentSetting.EnvInfo());
        }
        if (GUILayout.Button("Reset", GUILayout.MaxWidth(50)))
        {
            _obj.ResetEnv();
        }
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.Space();
        if (_obj.envs != null)
        {
            for (int i = 0, imax = _obj.envs.Count; i < imax; ++i)
            {
                EnverinmentSetting.EnvInfo ei = _obj.envs[i];
                EditorGUILayout.BeginHorizontal();
                GUILayout.Label("Env :" + i.ToString(), _labelstyle);

                if (GUILayout.Button("UseCurrentLightMapSetting", GUILayout.MaxWidth(200))){
                    _obj.UseCurrentLightMapSetting(i);
                    AssetDatabase.SaveAssets();
                }
                if (GUILayout.Button("Active", GUILayout.MaxWidth(50)))
                {
                    _obj.EnableSetting(i);
                }
                if (GUILayout.Button("X", GUILayout.MaxWidth(50)))
                {
                    _obj.envs.RemoveAt(i);
                    break;
                }

                EditorGUILayout.EndHorizontal();
                GUILayout.Box("", _line);
                EditorGUILayout.BeginHorizontal();
                ei.fog = EditorGUILayout.Toggle("Fog Enable", ei.fog);
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.BeginHorizontal();
                ei.fogColor = EditorGUILayout.ColorField("FogColor", ei.fogColor);
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.BeginHorizontal();
                ei.fogMode = (FogMode)EditorGUILayout.EnumPopup("FogMode", ei.fogMode);
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.BeginHorizontal();
                ei.fogDensity = EditorGUILayout.FloatField("FogDensity", ei.fogDensity);
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.BeginHorizontal();
                ei.fogStartDistance = EditorGUILayout.FloatField("FogStart", ei.fogStartDistance);
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.BeginHorizontal();
                ei.fogEndDistance = EditorGUILayout.FloatField("FogEnd", ei.fogEndDistance);
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.BeginHorizontal();
                Material mat = ei.skybox;
                ei.skybox = EditorGUILayout.ObjectField("Skybox", ei.skybox, typeof(Material),true) as Material;
 
                if (mat != ei.skybox)
                {
                    if (ei.skybox == null)
                    {
                        ei.skyboxPath = "";
                    }
                    else
                    {
                        ei.skyboxPath = AssetDatabase.GetAssetPath(ei.skybox);
                        ei.skyboxPath = ei.skyboxPath.Replace("Assets/Resources/", "");
                        ei.skyboxPath = ei.skyboxPath.Replace(".mat", "");
                    }                    
                }
                EditorGUILayout.EndHorizontal();               
                EditorGUILayout.BeginHorizontal();
                GUILayout.Label(ei.skyboxPath);
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.BeginHorizontal();
                ei.ambientLight = EditorGUILayout.ColorField("AmbientLight", ei.ambientLight);
                EditorGUILayout.EndHorizontal();
                
                //if(ei.lightmapDatas == null)
                //{
                //    ei.lightmapDatas = new List<EnverinmentSetting.EnvLightMapData>();
                //    ei.lightmapDataSize = 0;
                //}
                //EditorGUILayout.BeginHorizontal();
                //ei.lightmapDataSize = EditorGUILayout.IntField("LightmapSize:",ei.lightmapDataSize);
                //EditorGUILayout.EndHorizontal();
                //while(ei.lightmapDataSize > ei.lightmapDatas.Count)
                //{
                //    ei.lightmapDatas.Add(new EnverinmentSetting.EnvLightMapData());
                //}
                
                for(int j = 0;j < ei.lightmapDatas.Count; j++)
                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.BeginVertical();
                    GUILayout.Label("lightmap "+j.ToString()+":");
                    if (GUILayout.Button("X", GUILayout.MaxWidth(40)))
                    {
                        ei.lightmapDatas.RemoveAt(j);
                        return;
                    }
                    if (GUILayout.Toggle(j == ei.terrainLightmapIndex, "Terrain", GUILayout.MaxWidth(80)))
                    {
                        ei.terrainLightmapIndex = (byte)j;
                    }
                    EditorGUILayout.EndVertical();
                    EnverinmentSetting.EnvLightMapData lightmap = ei.lightmapDatas[j];
                    EditorGUILayout.BeginVertical();
                    GUILayout.Label("Far");

                    Texture2D tex = EditorGUILayout.ObjectField(lightmap.lightmapFar, typeof(Texture2D), true) as Texture2D;
                    if (tex != lightmap.lightmapFar)
                    {
                        lightmap.lightmapFar = tex;
                        lightmap.lightmapFarPath = GetPath(lightmap.lightmapFar, ".exr");
                    }
                        
                    GUILayout.Label(lightmap.lightmapFarPath);
                    EditorGUILayout.EndVertical();
                    //EditorGUILayout.BeginVertical();
                    //GUILayout.Label("Near");
                    //lightmap.lightmapNear = EditorGUILayout.ObjectField(lightmap.lightmapNear, typeof(Texture2D), true) as Texture2D;
                    //lightmap.lightmapNearPath = GetPath(lightmap.lightmapNear, ".exr");
                    //GUILayout.Label(lightmap.lightmapNearPath);
                    //EditorGUILayout.EndVertical();
                    EditorGUILayout.EndVertical();
                }              
                EditorGUILayout.BeginHorizontal();
                ei.decorate = EditorGUILayout.ObjectField("Decorate:", ei.decorate, typeof(GameObject), true) as GameObject;
                EditorGUILayout.EndHorizontal();
            }
        }
        if (EditorGUI.EndChangeCheck())
        {
            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        }
    }

    public void OnSceneGUI()
    {
        if (_obj.editLodArea != null)
        {
        }
    }
    private string GetPath(Object obj , string fixname)
    {
        if (obj == null) return "";
        string path = AssetDatabase.GetAssetPath(obj);
        path = path.Replace("Assets/Resources/", "");
        path = path.Replace(fixname, "");
        return path;
    }
}
