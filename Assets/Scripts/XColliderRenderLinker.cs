using UnityEngine;
using XUtliPoolLib;


public class XColliderRenderLinker : MonoBehaviour, IColliderRenderLinker
{
    public Renderer[] renders;

    public Renderer[] GetLinkedRender()
    {
        return renders;
    }

    public bool Deprecated
    {
        get;
        set;
    }
}