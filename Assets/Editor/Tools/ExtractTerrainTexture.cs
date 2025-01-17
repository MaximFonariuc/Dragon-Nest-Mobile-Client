using UnityEditor;
using UnityEngine;
using System.IO;
using System.Collections.Generic;

public class ExtractTerrain : EditorWindow
{
    public enum EScaleSize
    {
        x2 = 2,
        x4 = 4,
    }

    private EScaleSize size = EScaleSize.x2;
    private float height = 1.0f;
    private Vector2 newSize = Vector2.zero;
    private bool inversX = false;
    private bool inversY = false;
    [MenuItem(@"XEditor/Tools/Terrain Tool")]
    static void Init()
    {
        EditorWindow.GetWindow(typeof(ExtractTerrain));
    }
    public void ExportTerrainBlendTex(Terrain t, List<Texture2D> texs)
    {
        float[, ,] map = t.terrainData.GetAlphamaps(0, 0, t.terrainData.alphamapWidth, t.terrainData.alphamapHeight);
        int layerCount = map.GetLength(2);
        if (layerCount == 0)
            return;

        for (int i = 0; i < layerCount; i += 4)
        {
            texs.Add(new Texture2D(t.terrainData.alphamapWidth, t.terrainData.alphamapHeight, TextureFormat.ARGB32, false));
        }
        int maxLayCount = texs.Count * 4;
        float[] colorValue = new float[maxLayCount];
        for (int y = 0; y < t.terrainData.alphamapHeight; y++)
        {
            for (int x = 0; x < t.terrainData.alphamapWidth; x++)
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
                int index = 0;
                for (int i = 0; i < layerCount; i += 4)
                {
                    Color c = new Color(colorValue[i], colorValue[i + 1], colorValue[i + 2], colorValue[i + 3]);
                    texs[index++].SetPixel(x, y, c);
                }

            }
        }
        for (int i = 0; i < texs.Count; ++i)
        {
            if (texs[i] != null)
                texs[i].Apply();
        }
    }

    public void ImportTerrainBlendTex(Terrain t, List<Texture2D> texs)
    {
        int maxLayerCount = texs.Count * 4;
        if (t.terrainData.alphamapLayers <= maxLayerCount)
        {
            Texture2D tex0 = texs[0];
            float[, ,] map = new float[tex0.width, tex0.height, t.terrainData.alphamapLayers];
            for (int x = 0; x < tex0.width; ++x)
            {
                for (int y = 0; y < tex0.height; ++y)
                {
                    int index = 0;
                    for (int i = 0; i < texs.Count; ++i)
                    {
                        Texture2D tex = texs[i];
                        if (tex != null)
                        {
                            Color c = tex.GetPixel(x, y);
                            if (index < t.terrainData.alphamapLayers)
                                map[y, x, index] = c.r;
                            index++;
                            if (index < t.terrainData.alphamapLayers)
                                map[y, x, index] = c.g;
                            index++;
                            if (index < t.terrainData.alphamapLayers)
                                map[y, x, index] = c.b;
                            index++;
                            if (index < t.terrainData.alphamapLayers)
                                map[y, x, index] = c.a;
                            index++;
                        }
                    }
                }
            }
            //t.terrainData.alphamapResolution = tex0.width;
            t.terrainData.SetAlphamaps(0, 0, map);
        }
    }
    public void ImportTerrainBlendTex(Terrain t, Texture2D tex)
    {
        if (tex!=null)
        {
            float[,,] map = new float[tex.width, tex.height, t.terrainData.alphamapLayers];
            for (int x = 0; x < tex.width; ++x)
            {
                for (int y = 0; y < tex.height; ++y)
                {
                    int index = 0;
                    Color c = tex.GetPixel(x, y);
                    if (index < t.terrainData.alphamapLayers)
                        map[y, x, index] = c.r;
                    index++;
                    if (index < t.terrainData.alphamapLayers)
                        map[y, x, index] = c.g;
                    index++;
                    if (index < t.terrainData.alphamapLayers)
                        map[y, x, index] = c.b;
                    index++;
                    if (index < t.terrainData.alphamapLayers)
                        map[y, x, index] = c.a;
                    index++;
                }
            }
            t.terrainData.SetAlphamaps(0, 0, map);
        }
    }

    public void OnGUI()
    {
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Extract", GUILayout.MaxWidth(150)))
        {
            ExtractTerrainData();
        }
        EditorGUILayout.EndHorizontal();
        
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Replace Terrain Blend Tex", GUILayout.MaxWidth(200)))
        {
            ReplaceTerrainData();
        }
        size = (EScaleSize)EditorGUILayout.EnumPopup("缩放", size, GUILayout.MaxWidth(200));
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Scale Terrain Blend Tex", GUILayout.MaxWidth(200)))
        {
            ScaleTerrainTex();
        }
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.BeginHorizontal();
        newSize = EditorGUILayout.Vector2Field("new size: x,y", newSize, GUILayout.MaxWidth(200));
        inversX = EditorGUILayout.Toggle("inversX", inversX, GUILayout.MaxWidth(100));
        inversY = EditorGUILayout.Toggle("inversY", inversY, GUILayout.MaxWidth(100));
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("ExportHeightMap", GUILayout.MaxWidth(200)))
        {
            ExportHeightMap();
        }
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("CompactBlendTex", GUILayout.MaxWidth(200)))
        {
            CompactBlendTex();
        }
        EditorGUILayout.EndHorizontal();

        
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Raise", GUILayout.MaxWidth(150)))
        {
            RaiseTerrain(height / 50.0f);
        }
        height = EditorGUILayout.FloatField("升降", height, GUILayout.MaxWidth(200));
        EditorGUILayout.EndHorizontal();
    }

    protected void ExtractTerrainData()
    {
        List<Texture2D> texs = new List<Texture2D>();
        ExportTerrainBlendTex(Terrain.activeTerrain, texs);

        string path = EditorUtility.SaveFolderPanel("Select a folder to save", "./Temp", "");
        for (int i = 0; i < texs.Count; ++i)
        {
            if (texs[i] != null)
            {
                byte[] bytes = texs[i].EncodeToPNG();
                string filePath = path + "/SplatAlpha" + i.ToString() + ".png";
                File.WriteAllBytes(filePath, bytes);
            }
        }

    }
    protected void ReplaceTerrainData()
    {
        List<Texture2D> texs = new List<Texture2D>();
        ExportTerrainBlendTex(Terrain.activeTerrain, texs);
        for (int i = 0; i < texs.Count; ++i)
        {
            Texture2D tex = texs[i];
            TextureScale.Bilinear(tex, tex.width * (int)size, tex.height * (int)size);
        }
        ImportTerrainBlendTex(Terrain.activeTerrain, texs);
    }
    protected void ScaleTerrainTex()
    {
        if (Terrain.activeTerrain != null)
        {
            TerrainData td = Terrain.activeTerrain.terrainData;

            Vector3 size = td.size;
            int width = (int)size.x;
            int height = (int)size.z;
            Vector2 newTile = new Vector2(newSize.x / width, newSize.y / height);
            Vector2 offset = new Vector2(0, (height - newSize.y) / height);
            List<Texture2D> texs = new List<Texture2D>();
            List<Texture2D> newTexs = new List<Texture2D>();
            string path = AssetDatabase.GetAssetPath(td);
            int index = path.LastIndexOf("/");
            path = path.Substring(0, index);
            ExportTerrainBlendTex(Terrain.activeTerrain, texs);
            for (int i = 0; i < texs.Count; ++i)
            {
                Texture2D tex = texs[i];
                //string texPath = string.Format("{0}/SplatAlpha_{1}.png", path, i);
                Texture2D newTex = Render2Texture.ScaleTexture(tex, "", 0, 0, newTile, offset, "Custom/Editor/TexScaleRGBA");
                if (newTex != null)
                {
                    newTexs.Add(newTex);
                }
            }
            AssetDatabase.Refresh();


            //float[,] heightMap = td.GetHeights(0, 0, td.heightmapWidth, td.heightmapHeight);
            //float[,] newHeightMap = new float[td.heightmapWidth, td.heightmapHeight];
            //int xOffset = 0;
            //if (inversX)
            //{
            //    xOffset = td.heightmapWidth - (int)newTile.x * td.heightmapWidth;
            //}
            //int yOffset = 0;
            //if (inversY)
            //{
            //    yOffset = td.heightmapHeight - (int)newTile.y * td.heightmapHeight;
            //}
            //Texture2D testTex = new Texture2D(td.heightmapWidth, td.heightmapHeight);

            //for (int y = 0; y < td.heightmapHeight; ++y)
            //{
            //    for (int x = 0; x < td.heightmapWidth; ++x)
            //    {
            //        float newX = (float)x/ td.heightmapWidth* newSize.x + xOffset;
            //        float newY = (float)y /td.heightmapHeight * newSize.y;
            //        int intX = (int)newX;
            //        if (intX >= td.heightmapWidth)
            //            intX = td.heightmapWidth - 1;
            //        int intY = (int)newY;
            //        if (intY >= td.heightmapHeight)
            //            intY = td.heightmapHeight - 1;
            //        float h = heightMap[intX, intY];
            //        newHeightMap[x, y] = h;
            //    }
            //}
            //td.SetHeights(0, 0, newHeightMap);

            //Terrain.activeTerrain.terrainData.size = new Vector3(newSize.x, size.y, newSize.y);
            //Transform terrainT = Terrain.activeTerrain.transform;
            //Vector3 pos = terrainT.localPosition;
            //pos.z = height - newSize.y;
            //terrainT.localPosition = pos;
            ImportTerrainBlendTex(Terrain.activeTerrain, texs);
        }
    }
    protected void CompactBlendTex()
    {
        TerrainData td = Terrain.activeTerrain.terrainData;

        Vector3 size = td.size;
        int width = (int)size.x;
        int height = (int)size.z;
        Vector2 newTile = Vector2.one;// new Vector2(newSize.x / width, newSize.y / height);
        Vector2 offset = Vector2.zero;// new Vector2(0, (height - newSize.y) / height);
        List<Texture2D> texs = new List<Texture2D>();
        if (td.alphamapLayers > 4)
        {
            ExportTerrainBlendTex(Terrain.activeTerrain, texs);
            Texture2D newTex = Render2Texture.CompactTexture(texs[0], texs[1], 0, 0, newTile, offset, "Custom/Editor/CompactSplatTex");
            ImportTerrainBlendTex(Terrain.activeTerrain, newTex);
        }
    }
        
    protected void ExportHeightMap()
    {
        if (Terrain.activeTerrain != null)
        {
            TerrainData td = Terrain.activeTerrain.terrainData;

            float[,] heightMap = td.GetHeights(0, 0, td.heightmapResolution, td.heightmapResolution);
            Texture2D testTex = new Texture2D(td.heightmapResolution, td.heightmapResolution, TextureFormat.ARGB32, false);
            for (int y = 0; y < td.heightmapResolution; ++y)
            {
                for (int x = 0; x < td.heightmapResolution; ++x)
                {
                    float h0 = heightMap[x, y];
                    testTex.SetPixel(x, y, new Color(h0, h0, h0, 1));
                }
            }

            byte[] bytes = testTex.EncodeToPNG();
            string path = AssetDatabase.GetAssetPath(td);
            int index = path.LastIndexOf("/");
            path = path.Substring(0, index);
            string texPath = string.Format("{0}/heightmap.png", path);
            File.WriteAllBytes(texPath, bytes);
            AssetDatabase.Refresh();
        }
    }
    
    protected void RaiseTerrain(float scale)
    {
        if (Terrain.activeTerrain != null)
        {
            TerrainData td = Terrain.activeTerrain.terrainData;
            
            float[,] heightMap = td.GetHeights(0, 0, td.heightmapResolution, td.heightmapResolution);
            for (int y = 0; y < td.heightmapResolution; ++y)
            {
                for (int x = 0; x < td.heightmapResolution; ++x)
                {
                    float height = heightMap[y, x];
                    height += scale;
                    heightMap[y, x] = height;
                }
            }
            td.SetHeights(0, 0, heightMap);

        }
    }
}
