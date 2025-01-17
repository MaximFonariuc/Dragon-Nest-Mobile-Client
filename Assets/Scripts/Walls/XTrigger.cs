using UnityEngine;
using XUtliPoolLib;

public abstract class XTrigger : MonoBehaviour
{
    protected IXPlayerAction _interface;
    private CapsuleCollider _cap = null;

    // Use this for initialization
    void Awake()
    {
        _cap = GetComponent<CapsuleCollider>();
        _cap.enabled = false;
    }

    void Update()
    {
        if (_interface == null || _interface.Deprecated) _interface = XInterfaceMgr.singleton.GetInterface<IXPlayerAction>(1);

        if (_interface != null && _interface.IsValid)
        {
            Vector3 pos = _interface.PlayerPosition(true);
            Vector3 last_pos = _interface.PlayerLastPosition(true);

            if ((last_pos - pos).sqrMagnitude > 0)
            {
                CollisionDetected(pos);
            }
        }
    }

    private void CollisionDetected(Vector3 pos)
    {
        Vector3 delta = (pos - (_cap.transform.position + _cap.center)); delta.y = 0;
        if (delta.sqrMagnitude < _cap.radius * _cap.radius)
        {
            OnTriggered();
        }
    }

    protected abstract void OnTriggered();
}
