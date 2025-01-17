using UnityEngine;
using System.Collections;
using XUtliPoolLib;

public class XOpenFishingUI : MonoBehaviour {

    private IXPlayerAction _uiOperation;

    int m_SeatIndex = 0;

    void Awake()
    {
        m_SeatIndex = int.Parse(gameObject.name.Substring(gameObject.name.Length - 1, 1));
    }

    void OnTriggerEnter(Collider c)
    {
        if (c.gameObject.CompareTag("Player"))
        {
            if (_uiOperation == null || _uiOperation.Deprecated)
                _uiOperation = XInterfaceMgr.singleton.GetInterface<IXPlayerAction>(1);

            if (_uiOperation != null) _uiOperation.GotoFishing(m_SeatIndex, true);
        }
    }

    void OnTriggerStay(Collider c)
    {
        if (c.gameObject.CompareTag("Player"))
        {
            if (_uiOperation == null || _uiOperation.Deprecated)
                _uiOperation = XInterfaceMgr.singleton.GetInterface<IXPlayerAction>(1);

            if (_uiOperation != null) _uiOperation.GotoFishing(m_SeatIndex, true);
        }
    }

    void OnTriggerExit(Collider c)
    {
        if (c.gameObject.CompareTag("Player"))
        {
            if (_uiOperation == null || _uiOperation.Deprecated)
                _uiOperation = XInterfaceMgr.singleton.GetInterface<IXPlayerAction>(1);

            if (_uiOperation != null) _uiOperation.GotoFishing(m_SeatIndex, false);
        }
    }
}
