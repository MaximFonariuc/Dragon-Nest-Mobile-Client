using UnityEngine;
using System.Collections;
using XUtliPoolLib;

public class XUIGenericEventHandle : MonoBehaviour 
{
    private IXGameUI m_game_ui = null;

    void Awake()
    {
        m_game_ui = XInterfaceMgr.singleton.GetInterface<IXGameUI>(XCommon.singleton.XHash("XGameUI"));
    }

	// Use this for initialization
	void OnClick () 
    {
        m_game_ui.OnGenericClick();
	}
}
