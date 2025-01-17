using UnityEngine;
using UILib;
using XUtliPoolLib;

class XUIPressTips: XUIObject
{
    public string content;
    public Vector3 offset = new Vector3(84,-21);

    protected override void OnPress(bool isPressed)
    {
        if (string.IsNullOrEmpty(content)) return;
        IUiUtility entance = XInterfaceMgr.singleton.GetInterface<IUiUtility>(XCommon.singleton.XHash("IUiUtility"));
        if (entance != null) {
            entance.ShowPressToolTips(isPressed, content , transform.position, offset);
        }
    }
}
