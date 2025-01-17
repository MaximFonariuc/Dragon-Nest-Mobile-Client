using UnityEngine;
using XUtliPoolLib;

public class XColliderCoin : XCollierObject
{
    public string exString;
    public XColliderObjectType ColliderObjectType;

    // Use this for initialization
    void Start()
    {

    }

    protected override void XTriggerEnter(Collider c)
    {
        if (exString != null && exString.Length > 0)
        {

        }
    }

    public new void OnTriggerEnter(Collider c)
    {
        if (c.gameObject.CompareTag("Player"))
        {
            XFx fx = XFxMgr.singleton.CreateFx("Effects/FX_Particle/Shared/Drop_xishou");
            //GameObject go = fx.Fx;
            //fx.Play(transform.position, go.transform.rotation, 1.0f);
            fx.Play(c.transform,Vector3.zero, Vector3.one, 1.0f, true);

            transform.position = new Vector3(0, 0, 10000);
        }
    }

    protected override void XTriggerExit(Collider c)
    {

    }
}
