using UnityEngine;

using System.Linq;
using System;

public class LightMapSetter : MonoBehaviour
{
    public Texture2D[] LightMapNear;
    public Texture2D[] LightMapFar;

    private LightmapData[] lightMaps;

    void Awake()
    {
        //if (LightMapNear.Length != LightMapFar.Length)
        //{
        //    Debug.Log("In order for LightMapSwitcher to work, the Near and Far LightMap lists must be of equal length");
        //    return;
        //}

        // Sort the Day and Night arrays in numerical order, so you can just blindly drag and drop them into the inspector
        LightMapNear = LightMapNear.OrderBy(t2d => t2d.name, new NaturalSortComparer<string>()).ToArray();
        LightMapFar = LightMapFar.OrderBy(t2d => t2d.name, new NaturalSortComparer<string>()).ToArray();

        // Put them in a LightMapData structure
        int MaxLightMapLength = Math.Max(LightMapFar.Length, LightMapNear.Length);
        if (MaxLightMapLength == 0)
        {
            lightMaps = null;
            return;
        }

        lightMaps = new LightmapData[MaxLightMapLength];
        for (int i = 0; i < MaxLightMapLength; i++)
        {
            lightMaps[i] = new LightmapData();
            lightMaps[i].lightmapDir = (i < LightMapNear.Length ? LightMapNear[i] : null);
            lightMaps[i].lightmapColor = (i < LightMapFar.Length ? LightMapFar[i] : null);
        }
    }

    public void SetLightMap()
    {
        if (lightMaps != null)
            LightmapSettings.lightmaps = lightMaps;
    }
}