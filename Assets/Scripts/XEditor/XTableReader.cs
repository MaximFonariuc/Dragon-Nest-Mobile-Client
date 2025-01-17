#if UNITY_EDITOR
using UnityEngine;
using System.Collections;
using XUtliPoolLib;

public class XTableReader
{
    public static bool ReadFile(string location, CVSReader reader)
    {
        CVSReader.Init();
        XBinaryReader.Init();
        return XResourceLoaderMgr.singleton.ReadFile(location, reader);
    }
}
#endif