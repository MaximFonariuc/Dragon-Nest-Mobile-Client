using UnityEngine;
using XUtliPoolLib;

public class YBillboard : MonoBehaviour
{
    private IAssociatedCamera _camera = null;
    private Transform cacheTrans = null;
    private Transform cacheCameraTrans = null;
    public static bool IsUpdate = true;
    // Use this for initialization
    //void Awake () {
    //       cacheTrans = this.transform;
    //       if (Camera.main != null)
    //           cacheCameraTrans = Camera.main.transform;
    //   }

    // Update is called once per frame
    void Update()
    {
        if (cacheTrans == null)
            cacheTrans = this.transform;
        if (_camera == null || _camera.Deprecated) _camera = XInterfaceMgr.singleton.GetInterface<IAssociatedCamera>(XCommon.singleton.XHash("IAssociatedCamera"));
        if (cacheCameraTrans == null && _camera != null)
        {
            Camera c = _camera.Get();
            if (c != null)
                cacheCameraTrans = c.transform;
        }
        if (IsUpdate && cacheCameraTrans != null && cacheTrans != null)
        {
            Vector3 eularAngle = cacheTrans.rotation.eulerAngles;
            Vector3 cameraEularAngle = cacheCameraTrans.rotation.eulerAngles;
            cacheTrans.rotation = Quaternion.Euler(eularAngle.x, cameraEularAngle.y, eularAngle.z);
        }
    }
}
