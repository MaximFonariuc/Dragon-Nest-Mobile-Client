using UnityEngine;
using System.Collections;
using XUtliPoolLib;

public class XOpenNestUI : MonoBehaviour {

    private IXPlayerAction _uiOperation;

    void OnTriggerEnter(Collider c)
    {
        if (c.gameObject.CompareTag("Player"))
        {
            if (_uiOperation == null || _uiOperation.Deprecated)
                _uiOperation = XInterfaceMgr.singleton.GetInterface<IXPlayerAction>(1);

            if (_uiOperation != null) _uiOperation.GotoNest();
        }
    }
}
