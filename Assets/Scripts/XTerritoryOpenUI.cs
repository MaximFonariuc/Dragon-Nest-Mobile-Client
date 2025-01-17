using UnityEngine;
using System.Collections;
using XUtliPoolLib;

public class XTerritoryOpenUI : MonoBehaviour
{
    private IXPlayerAction _uiOperation;

    public int index = 0;

    void OnTriggerEnter(Collider c)
    {
        ///if (c.gameObject.CompareTag("Player"))
        if (c.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            if (_uiOperation == null || _uiOperation.Deprecated)
                _uiOperation = XInterfaceMgr.singleton.GetInterface<IXPlayerAction>(1);

            if (_uiOperation != null) _uiOperation.GotoTerritoryBattle(index);
        }
    }
}
