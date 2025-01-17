using UnityEngine;
using XUtliPoolLib;

public class XUIScript : MonoBehaviour 
{
    private IXGameUI _game_ui = null;

	void Awake ()
	{
        _game_ui = XInterfaceMgr.singleton.GetInterface<IXGameUI>(XCommon.singleton.XHash("XGameUI"));

        _game_ui.UICamera = gameObject.GetComponent<Camera>();

	    Transform uiRoot = transform.parent;
        _game_ui.UIRoot = uiRoot;

	    UIRoot nguiRoot = uiRoot.GetComponent<UIRoot>();
        _game_ui.Base_UI_Width = nguiRoot.base_ui_width;
        _game_ui.Base_UI_Height = nguiRoot.base_ui_height;

        XUICommon.singleton.Init(uiRoot);
	}

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
	}
}
