#if UNITY_EDITOR
using UnityEngine;
using System.Collections.Generic;
using UnityEditor;
public class SceneConfig : MonoBehaviour
{
    [System.Serializable]
    public class TerrainMeshInfo
    {
        public bool enable = true;
        public int lod = 1;
        public List<LodArea> lodArea;
        public List<Vector4> excludeArea;
        public Vector4 area;
        public TerrainMeshInfo(float widthPerBlockLod0, float lengthPerBlockLod0, int index,int terrainBlock)
        {
            int i = index % terrainBlock;
            int j = index / terrainBlock;
            
            float widthPerBlock = widthPerBlockLod0;
            float lengthPerBlock = lengthPerBlockLod0;            
            int startX = i * 32;
            int startZ = j * 32;
            int endX = startX + 32;
            int endZ = startZ + 32;

            area.x = startX * widthPerBlockLod0;
            area.y = startZ * lengthPerBlockLod0;

            area.z = endX * widthPerBlockLod0;
            area.w = endZ * lengthPerBlockLod0;

        }
        public void AddLodArea()
        {
            if (lodArea == null)
                lodArea = new List<LodArea>();
            lodArea.Add(new LodArea(area));
        }
        public void RemoveLodArea(int i)
        {
            if (lodArea != null)
            {
                lodArea.RemoveAt(i);
            }
        }            
    }
    [System.Serializable]
    public class LodArea
    {
        public Vector4 area;
        public int lod = 1;
        public LodArea(Vector4 a)
        {
            area = a;
        }
    }
    public int terrainBlock;
    public TerrainMeshInfo[] terrainMeshInfo;
    public Vector4 terrainLightmapScaleOffset;
}
#endif