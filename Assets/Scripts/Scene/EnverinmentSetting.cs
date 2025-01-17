using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using XUtliPoolLib;
#if UNITY_EDITOR
using UnityEditor;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;
using XEditor;
using System.IO;
#endif

[ExecuteInEditMode]
public class EnverinmentSetting : MonoBehaviour, IEnvSetting
{
    [Serializable]
    public class EnvLightMapData
    {
        [SerializeField]
        public string lightmapFarPath;
        //[SerializeField]
        //public string lightmapNearPath;
        [System.NonSerialized]
        public Texture2D lightmapFar;
        //[System.NonSerialized]
        //public Texture2D lightmapNear;
    }

    [Serializable]
    public class EnvInfo
    {
        [SerializeField]
        public bool fog = true;
        [SerializeField]
        public Color fogColor;
        [SerializeField]
        public FogMode fogMode = FogMode.Linear;
        [SerializeField]
        [Range(0f, 1f)]
        public float fogDensity;
        [SerializeField]
        public float fogStartDistance;
        [SerializeField]
        public float fogEndDistance;
        [SerializeField]
        public string skyboxPath;
        [SerializeField]
        public Color ambientLight;
        [SerializeField]
        public GameObject decorate;

        [SerializeField]
        public List<EnvLightMapData> lightmapDatas;

        [SerializeField]
        public byte terrainLightmapIndex;

        [System.NonSerialized]
        public Material skybox;
        //public int lightmapDataSize = 0;

        //public Material skybox
        //{
        //    get;
        //    set;
        //}        
    }
    [SerializeField]
    public List<EnvInfo> envs;
    [SerializeField]
    public Material terrainMat;
    [System.NonSerialized]
    private int currentIndex = -1;
    [System.NonSerialized]
    public EnvInfo currentEnvInfo;

#if UNITY_EDITOR
    [System.NonSerialized]
    private EnvInfo backup = new EnvInfo();
    [System.NonSerialized]
    private LightmapData[] normaLightMaps;
    private bool dynamicLoadLightMap = false;
    private GameObject terrainGo = null;
    private RenderTexture terrainRT = null;
    [System.NonSerialized]
    public SceneConfig sceneConfig = null;
    [System.NonSerialized]
    public int selectBlockIndex = -1;
    [System.NonSerialized]
    public SceneConfig.LodArea editLodArea = null;

    string sceneFolder = "";
    List<Vector3> vertexList = new List<Vector3>();
    List<int> indexList = new List<int>();
    List<Vector2> uvList = new List<Vector2>();
    float heithMapWidthScale;
    float heightMapLengthScale;
    float widthPerBlockLod0;
    float lengthPerBlockLod0;
    float uvPerBlockLod0;
#endif
    void Awake()
    {
        if (Application.isPlaying)
            XUpdater.XShell.singleton.MonoObjectRegister("EnvSet", this);
#if UNITY_EDITOR

        if (terrainMat != null || envs != null && envs.Count > 1)
        {
            dynamicLoadLightMap = true;
        }
        else
        {
            backup.fog = RenderSettings.fog;
            backup.fogColor = RenderSettings.fogColor;
            backup.fogMode = RenderSettings.fogMode;
            backup.fogDensity = RenderSettings.fogDensity;
            backup.fogStartDistance = RenderSettings.fogStartDistance;
            backup.fogEndDistance = RenderSettings.fogEndDistance;
            backup.skybox = RenderSettings.skybox;
            backup.ambientLight = RenderSettings.ambientLight;
            normaLightMaps = LightmapSettings.lightmaps;
        }

        if (envs != null)
        {
            for (int i = 0; i < envs.Count; ++i)
            {
                EnvInfo ei = envs[i];
                if (ei.lightmapDatas != null)
                {
                    for (int j = 0; j < ei.lightmapDatas.Count; ++j)
                    {
                        EnvLightMapData eld = ei.lightmapDatas[j];
                        if (!string.IsNullOrEmpty(eld.lightmapFarPath))
                        {
                            eld.lightmapFar = XResourceLoaderMgr.singleton.GetSharedResource<Texture2D>(eld.lightmapFarPath, ".exr");
                        }
                    }
                }
            }
        }
        EnableSetting(0);
#endif
    }
    public void EnableSetting(int indexList)
    {
        if (envs != null &&
            indexList >= 0 &&
            indexList < envs.Count &&
            currentIndex != indexList)
        {
            currentIndex = indexList;
            EnvInfo ei = envs[indexList];
            RenderSettings.fog = ei.fog;
            RenderSettings.fogColor = ei.fogColor;
            RenderSettings.fogMode = ei.fogMode;
            RenderSettings.fogDensity = ei.fogDensity;
            RenderSettings.fogStartDistance = ei.fogStartDistance;
            RenderSettings.fogEndDistance = ei.fogEndDistance;

#if !UNITY_EDITOR
            if (currentEnvInfo != null)
            {
                XResourceLoaderMgr.SafeDestroyShareResource(ei.skyboxPath, ref currentEnvInfo.skybox);
                if (currentEnvInfo.decorate != null)
                {
                    currentEnvInfo.decorate.SetActive(false);
                }
                if (currentEnvInfo.lightmapDatas != null)
                {
                    for (int i = 0, length = currentEnvInfo.lightmapDatas.Count; i < length; i++)
                    {
                        EnvLightMapData elmd = currentEnvInfo.lightmapDatas[i];
                        XResourceLoaderMgr.singleton.UnSafeDestroyShareResource(elmd.lightmapFarPath, ".exr", elmd.lightmapFar);
                        elmd.lightmapFar = null;
                    }
                }
            }
#endif
            ei.skybox = XResourceLoaderMgr.singleton.GetSharedResource<Material>(ei.skyboxPath, ".mat");
            if (ei.skybox != null)
                RenderSettings.skybox = ei.skybox;
            RenderSettings.ambientLight = ei.ambientLight;

            if (ei.lightmapDatas != null && ei.lightmapDatas.Count > 0)
            {
                LightmapData[] lightmaps = new LightmapData[ei.lightmapDatas.Count];
                for (int i = 0, length = lightmaps.Length; i < length; i++)
                {
                    LightmapData ld = new LightmapData();
                    lightmaps[i] = ld;
                    EnvLightMapData elmd = ei.lightmapDatas[i];
                    if (!string.IsNullOrEmpty(elmd.lightmapFarPath))
                    {
                        elmd.lightmapFar = XResourceLoaderMgr.singleton.GetSharedResource<Texture2D>(elmd.lightmapFarPath, ".exr");
                        ld.lightmapColor = elmd.lightmapFar;
                    }
                }
                LightmapSettings.lightmaps = lightmaps;
                if (ei.decorate != null)
                {
                    ei.decorate.SetActive(true);
                }
                if (terrainMat != null && ei.terrainLightmapIndex >= 0 && ei.terrainLightmapIndex < lightmaps.Length)
                {
                    Shader.SetGlobalTexture("_TerrainLightMap", lightmaps[ei.terrainLightmapIndex].lightmapColor);
                }
            }
            currentEnvInfo = ei;

            for (int i = 0; i < envs.Count; ++i)
            {
                if (i != indexList)
                {
                    EnvInfo oterhEI = envs[i];
                    if (oterhEI.decorate != null)
                    {
                        oterhEI.decorate.SetActive(false);
                    }
                }
            }
        }
    }

#if UNITY_EDITOR

    [MenuItem(@"GameObject/Scene/TerrainEditor", false, 0)]
    private static void EditTerrain()
    {
        GameObject envGo = GameObject.Find("Environment");
        if (envGo == null)
        {
            envGo = new GameObject("Environment");
        }
        EnverinmentSetting es = envGo.GetComponent<EnverinmentSetting>();
        if (es == null)
        {
            GameObject cameraGo = GameObject.Find("Main Camera");
            if (cameraGo != null)
            {
                es = cameraGo.GetComponent<EnverinmentSetting>();
                if (es != null)
                {
                    UnityEditorInternal.ComponentUtility.CopyComponent(es);
                    UnityEditorInternal.ComponentUtility.PasteComponentAsNew(envGo);
                    UnityEngine.Object.DestroyImmediate(es);
                }
            }            
        }
        if(es==null)
        {
            es = envGo.AddComponent<EnverinmentSetting>();
        }
        
    }

    public void PrepareTerrain()
    {
        //terrain
        Terrain terrain = Terrain.activeTerrain;
        UnityEngine.SceneManagement.Scene s = EditorSceneManager.GetActiveScene();
        sceneFolder = AssetModify.GetFolder(s.path);
        if (terrain == null)
        {
            string terrainPrefabPath = AssetModify.MakeAssetPathWithFolder(sceneFolder, s.name + "_Terrain", "prefab");
            GameObject go = AssetDatabase.LoadAssetAtPath<GameObject>(terrainPrefabPath);
            if (go != null)
            {
                terrainGo = GameObject.Instantiate<GameObject>(go);
                terrainGo.name = "Terrain";
                terrain = Terrain.activeTerrain;
            }
        }
        else
        {
            terrainGo = terrain.gameObject;
        }
        if (terrain != null)
        {
            TerrainData td = terrain.terrainData;
            Vector3 size = td.size;
            float width = td.size.x;
            float length = td.size.z;

            heithMapWidthScale = td.heightmapResolution / width;
            heightMapLengthScale = td.heightmapResolution / length;

            int widthVertexCount = td.heightmapResolution - 1;
            int lengthVertexCount = td.heightmapResolution - 1;
            widthPerBlockLod0 = width / (float)widthVertexCount;
            lengthPerBlockLod0 = length / (float)lengthVertexCount;
            uvPerBlockLod0 = 1.0f / (float)widthVertexCount;

            bool needExportAlphamap = false;
            Texture2D[] alphaMap = td.alphamapTextures;
            if (alphaMap != null)
            {
                for (int i = 0; i < alphaMap.Length; ++i)
                {
                    string filePath = string.Format("{0}/{1}_{2}.png", sceneFolder, s.name, i);
                    if(!File.Exists(filePath))
                    {
                        needExportAlphamap = true;
                        break;
                    }
                }
            }
            if(needExportAlphamap)
            {
                ExportAlphaMap();
            }

            string matPath = AssetModify.MakeAssetPathWithFolder(sceneFolder, s.name, "mat");
            terrainMat = AssetDatabase.LoadAssetAtPath<Material>(matPath);
            if (terrainMat == null)
            {
                terrainMat = new Material(Shader.Find("Custom/Scene/CustomTerrainDiffuse"));
                terrainMat.name = s.name;
                terrainMat = AssetModify.CreateOrReplaceAsset<Material>(terrainMat, AssetModify.MakeAssetPathWithFolder(sceneFolder, terrainMat.name, "mat"));
            }
            string controlTexPath = string.Format("{0}/{1}_0.png", sceneFolder, s.name);
            Texture controlTex = AssetDatabase.LoadAssetAtPath<Texture>(controlTexPath);
            terrainMat.SetTexture("_Control", controlTex);
            SplatPrototype[] splatTexs = td.splatPrototypes;
            if (splatTexs != null)
            {
                for (int i = 0; i < splatTexs.Length; ++i)
                {
                    SplatPrototype sp = splatTexs[i];
                    string texName = string.Format("_Splat{0}", i);
                    terrainMat.SetTexture(texName, sp.texture);
                    terrainMat.SetTextureScale(texName, new Vector2(width / sp.tileSize.x, length / sp.tileSize.y));
                }
            }
        }
        if (terrainGo != null)
        {
            terrainGo.transform.parent = this.transform;
        }
        

        //config
        Transform terrainConfigTran = this.transform.Find("SceneConfig");
        if (terrainConfigTran == null)
        {
            string terrainConfigPath = string.Format("{0}/{1}_SceneConfig.prefab", sceneFolder, s.name);
            GameObject terrainConfigGO = AssetDatabase.LoadAssetAtPath<GameObject>(terrainConfigPath);
            if (terrainConfigGO != null)
            {
                terrainConfigGO = GameObject.Instantiate<GameObject>(terrainConfigGO);
                terrainConfigGO.name = "SceneConfig";
                sceneConfig = terrainConfigGO.GetComponent<SceneConfig>();
            }
            if (sceneConfig == null)
            {
                terrainConfigGO = new GameObject("SceneConfig");
                sceneConfig = terrainConfigGO.AddComponent<SceneConfig>();
                sceneConfig.terrainBlock = 4;
                AssetModify.CreateOrReplacePrefab(terrainConfigGO, terrainConfigPath);
            }
        }
        else
        {
            GameObject terrainConfigGO = terrainConfigTran.gameObject;
            sceneConfig = terrainConfigGO.GetComponent<SceneConfig>();
        }
        if (sceneConfig != null)
        {
            if (terrain.lightmapIndex >= 0)
            {
                sceneConfig.terrainLightmapScaleOffset = terrain.lightmapScaleOffset;
            }
            int blockCount = sceneConfig.terrainBlock * sceneConfig.terrainBlock;
            if (sceneConfig.terrainMeshInfo == null || sceneConfig.terrainMeshInfo.Length != blockCount)
            {
                sceneConfig.terrainMeshInfo = new SceneConfig.TerrainMeshInfo[blockCount];
                for (int i = 0; i < sceneConfig.terrainMeshInfo.Length; ++i)
                {
                    sceneConfig.terrainMeshInfo[i] = new SceneConfig.TerrainMeshInfo(widthPerBlockLod0, lengthPerBlockLod0, i, sceneConfig.terrainBlock);
                }
            }
            sceneConfig.transform.parent = this.transform;
        }
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
    private void ExportTerrainBlendTex(TerrainData td, List<Texture2D> texs)
    {
        float[,,] map = td.GetAlphamaps(0, 0, td.alphamapWidth, td.alphamapHeight);
        int layerCount = map.GetLength(2);
        if (layerCount == 0)
            return;

        for (int i = 0; i < layerCount; i += 4)
        {
            texs.Add(new Texture2D(td.alphamapWidth, td.alphamapHeight, TextureFormat.ARGB32, false));
        }
        int maxLayCount = texs.Count * 4;
        float[] colorValue = new float[maxLayCount];
        for (int y = 0; y < td.alphamapHeight; y++)
        {
            for (int x = 0; x < td.alphamapWidth; x++)
            {
                for (int i = 0; i < colorValue.Length; ++i)
                {

                    if (i < layerCount)
                    {
                        colorValue[i] = map[y, x, i];
                    }
                    else
                    {
                        colorValue[i] = 0.0f;
                    }
                }
                int indexList = 0;
                for (int i = 0; i < layerCount; i += 4)
                {
                    Color c = new Color(colorValue[i], colorValue[i + 1], colorValue[i + 2], colorValue[i + 3]);
                    texs[indexList++].SetPixel(x, y, c);
                }

            }
        }
        for (int i = 0; i < texs.Count; ++i)
        {
            if (texs[i] != null)
                texs[i].Apply();
        }
    }
    public void ExportAlphaMap()
    {
        Terrain terrain = Terrain.activeTerrain;
        if(terrain!=null)
        {
            UnityEngine.SceneManagement.Scene s = EditorSceneManager.GetActiveScene();
            TerrainData td = terrain.terrainData;
            List<Texture2D> texs = new List<Texture2D>();
            ExportTerrainBlendTex(td, texs);
            for (int i = 0; i < texs.Count; ++i)
            {
                if (texs[i] != null)
                {
                    byte[] bytes = texs[i].EncodeToPNG();
                    string filePath = string.Format("{0}/{1}_{2}.png", sceneFolder, s.name, i);
                    File.WriteAllBytes(filePath, bytes);
                }
            }
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

    }

    private void GenerateBlockMesh(TerrainData td, int terrainIndex, string sceneName, int i, int j)
    {
        SceneConfig.TerrainMeshInfo tmi = sceneConfig.terrainMeshInfo[terrainIndex];
        string terrainMeshName = string.Format("{0}_terrain_{1}", sceneName, terrainIndex);
        string terrainMehsPath = string.Format("{0}/{1}.asset", sceneFolder, terrainMeshName);
        Transform tran = this.transform.Find(terrainMeshName);
        if (tran != null)
        {
            GameObject.DestroyImmediate(tran.gameObject);
        }
        if (File.Exists(terrainMehsPath))
        {
            AssetDatabase.DeleteAsset(terrainMehsPath);
        }
        if (!tmi.enable)
        {
            return;
        }
        int lod = (int)Math.Pow(2, tmi.lod);
        float widthPerBlock = lod * widthPerBlockLod0;
        float lengthPerBlock = lod * lengthPerBlockLod0;
        float uvPerBlock = lod * uvPerBlockLod0;
        vertexList.Clear();
        indexList.Clear();
        uvList.Clear();
        int xVertexCount = 32 / lod;
        int zVertexCount = 32 / lod;
        int startX = i * xVertexCount;
        int startZ = j * zVertexCount;
        int endX = startX + xVertexCount;
        int endZ = startZ + zVertexCount;
        for (int z = startZ; z < endZ + 1; ++z)
        {
            for (int x = startX; x < endX + 1; ++x)
            {
                float currentX = x * widthPerBlock;
                float currentY = z * lengthPerBlock;
                Vector3 p = new Vector3(currentX, 0, currentY);
                p.y = td.GetHeight((int)(p.x * heithMapWidthScale), (int)(p.z * heightMapLengthScale));
                vertexList.Add(p);
                Vector2 uv = new Vector2(x * uvPerBlock, z * uvPerBlock);
                uvList.Add(uv);
                if (x > startX && z > startZ)
                {
                    int lastVertexIndex = (xVertexCount + 1) * (z - startZ - 1) + x - startX;
                    int curcentVertexIndex = vertexList.Count - 1;
                    indexList.Add(lastVertexIndex - 1); indexList.Add(curcentVertexIndex); indexList.Add(lastVertexIndex);
                    indexList.Add(lastVertexIndex - 1); indexList.Add(curcentVertexIndex - 1); indexList.Add(curcentVertexIndex);
                }               
            }
        }
        Mesh terrainMesh = new Mesh();
        terrainMesh.name = terrainMeshName;
        terrainMesh.vertices = vertexList.ToArray();
        terrainMesh.triangles = indexList.ToArray();
        terrainMesh.uv = uvList.ToArray();
        terrainMesh.uv2 = null;
        MeshUtility.SetMeshCompression(terrainMesh, ModelImporterMeshCompression.Medium);
        MeshUtility.Optimize(terrainMesh);
        terrainMesh.UploadMeshData(true);

        
        Mesh mesh = AssetModify.CreateOrReplaceAsset<Mesh>(terrainMesh, terrainMehsPath);
        GameObject terrainMeshGo = null;
        Transform meshTran = this.transform.Find(terrainMesh.name);

        if (meshTran != null)
        {
            terrainMeshGo = meshTran.gameObject;
        }
        else
        {
            terrainMeshGo = new GameObject(terrainMesh.name);
            terrainMeshGo.layer = 9;
        }
        terrainMeshGo.transform.position = terrainGo.transform.position;
        terrainMeshGo.transform.parent = this.transform;
        MeshFilter mf = terrainMeshGo.GetComponent<MeshFilter>();
        if (mf == null)
        {
            mf = terrainMeshGo.AddComponent<MeshFilter>();
        }
        mf.sharedMesh = mesh;
        MeshRenderer mr = terrainMeshGo.GetComponent<MeshRenderer>();
        if (mr == null)
        {
            mr = terrainMeshGo.AddComponent<MeshRenderer>();
        }
        AssetModify.ProcessRender(mr, false);
        terrainMat.SetVector("_LightMap_ST", sceneConfig.terrainLightmapScaleOffset);
        mr.sharedMaterial = terrainMat;

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

    }
    public void ConvertTerrainBlock()
    {
        Terrain terrain = Terrain.activeTerrain;
        if (terrain != null)
        {
            TerrainData td = terrain.terrainData;

            UnityEngine.SceneManagement.Scene s = EditorSceneManager.GetActiveScene();
            int i = selectBlockIndex % sceneConfig.terrainBlock;
            int j = selectBlockIndex / sceneConfig.terrainBlock;
            GenerateBlockMesh(td, selectBlockIndex, s.name, i, j);
        }
    }
    public void ConvertTerrain()
    {
        Terrain terrain = Terrain.activeTerrain;
        if (terrain != null)
        {
            TerrainData td = terrain.terrainData;


            UnityEngine.SceneManagement.Scene s = EditorSceneManager.GetActiveScene();

            for (int j = 0; j < sceneConfig.terrainBlock; ++j)
            {
                for (int i = 0; i < sceneConfig.terrainBlock; ++i)
                {
                    int terrainIndex = j * sceneConfig.terrainBlock + i;
                    GenerateBlockMesh(td, terrainIndex, s.name, i, j);
                }
            }
        }
    }
    public void EndConvertTerrain()
    {
        selectBlockIndex = -1;
        UnityEngine.SceneManagement.Scene s = EditorSceneManager.GetActiveScene();
        if (terrainGo != null)
        {
            string terrainPath = string.Format("{0}/{1}_Terrain.prefab", sceneFolder, s.name);
            AssetModify.CreateOrReplacePrefab(terrainGo, terrainPath);
            GameObject.DestroyImmediate(terrainGo);
            terrainGo = null;
        }
        if (sceneConfig != null)
        {
            string sceneConfigPath = string.Format("{0}/{1}_SceneConfig.prefab", sceneFolder, s.name);
            AssetModify.CreateOrReplacePrefab(sceneConfig.gameObject, sceneConfigPath);
            GameObject.DestroyImmediate(sceneConfig.gameObject);
            sceneConfig = null;
        }
        if (terrainRT != null)
        {
            terrainRT.Release();
            terrainRT = null;
        }
    }
    public void ResetEnv()
    {
        if (dynamicLoadLightMap)
        {
            EnableSetting(0);
        }
        else
        {
            RenderSettings.fog = backup.fog;
            RenderSettings.fogColor = backup.fogColor;
            RenderSettings.fogMode = backup.fogMode;
            RenderSettings.fogDensity = backup.fogDensity;
            RenderSettings.fogStartDistance = backup.fogStartDistance;
            RenderSettings.fogEndDistance = backup.fogEndDistance;
            RenderSettings.skybox = backup.skybox;
            RenderSettings.ambientLight = backup.ambientLight;
            LightmapSettings.lightmaps = normaLightMaps;
            currentIndex = -1;
        }
        
    }
    public void UseCurrentLightMapSetting(int indexList)
    {
        if (envs != null &&
            indexList >= 0 &&
            indexList < envs.Count)
        {
            UnityEngine.SceneManagement.Scene s = UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene();
            EnvInfo ei = envs[indexList];
            ei.fog = RenderSettings.fog;
            ei.fogColor = RenderSettings.fogColor;
            ei.fogMode = RenderSettings.fogMode;
            ei.fogDensity = RenderSettings.fogDensity;
            ei.fogStartDistance = RenderSettings.fogStartDistance;
            ei.fogEndDistance = RenderSettings.fogEndDistance;
            ei.skybox = RenderSettings.skybox;
            LightmapData[] lightmaps = LightmapSettings.lightmaps;
            if (lightmaps != null && lightmaps.Length > 0)
            {
                ei.lightmapDatas = new List<EnvLightMapData>();
                for (int i = 0; i < lightmaps.Length; ++i)
                {
                    LightmapData ld = lightmaps[i];
                    string dir = string.Format("Assets/Resources/Scene/{0}", s.name);
                    if (!System.IO.Directory.Exists(dir))
                    {
                        System.IO.Directory.CreateDirectory(dir);
                    }
                    string targetPath = string.Format("{0}/{1}.exr", dir, ld.lightmapColor.name);
                    string srcPath = UnityEditor.AssetDatabase.GetAssetPath(ld.lightmapColor);
                    UnityEditor.AssetDatabase.CopyAsset(srcPath, targetPath);
                    EnvLightMapData eld = new EnvLightMapData();
                    eld.lightmapFarPath = string.Format("Scene/{0}/{1}", s.name, ld.lightmapColor.name);
                    eld.lightmapFar = ld.lightmapColor;
                    ei.lightmapDatas.Add(eld);
                }
            }
        }
    }

    private void Update()
    {
    }
#endif

}
