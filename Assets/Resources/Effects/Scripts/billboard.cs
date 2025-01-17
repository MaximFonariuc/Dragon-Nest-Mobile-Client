using UnityEngine;
using XUtliPoolLib;

public class billboard : MonoBehaviour
{
    private IAssociatedCamera _camera = null;
    private Transform cacheTrans = null;
    private Transform cacheCameraTrans = null;
    //private static float updateTime = 0.1f;
    //private float time = 0.0f;
    // Use this for initialization
    //void Start () 
    //   {

    //}

    // Update is called once per frame
    void Update()
    {
        if (cacheTrans == null)
            cacheTrans = transform;
        if (_camera == null || _camera.Deprecated) _camera = XInterfaceMgr.singleton.GetInterface<IAssociatedCamera>(XCommon.singleton.XHash("IAssociatedCamera"));

        if (cacheCameraTrans == null && _camera != null)
        {
            Camera c = _camera.Get();
            if (c != null)
                cacheCameraTrans = c.transform;
        }
        if (YBillboard.IsUpdate && cacheCameraTrans != null && cacheTrans != null)
        {
            cacheTrans.LookAt(cacheCameraTrans.position);
        }
    }
}
