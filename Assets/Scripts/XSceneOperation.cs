using System;
using UnityEngine;
using XUtliPoolLib;

internal class XSceneOperation : XUIObject, IXSceneOperation
{
    public void SetLightMap()
    {
        LightMapSetter lightMapSetter = GetComponent<LightMapSetter>();
        if (lightMapSetter != null)
            lightMapSetter.SetLightMap();
    }

    public bool Deprecated
    {
        get;
        set;
    }
}
