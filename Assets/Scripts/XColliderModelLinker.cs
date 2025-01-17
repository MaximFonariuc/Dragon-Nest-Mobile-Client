using UnityEngine;
using XUtliPoolLib;
using System.Collections.Generic;

public class XColliderModelLinker : MonoBehaviour
{
    public List<GameObject> Models = new List<GameObject>();
    void Start()
    {
    }

    public List<GameObject> GetLinkedModel()
    {
        return Models;
    }

    public bool Deprecated
    {
        get;
        set;
    }
}