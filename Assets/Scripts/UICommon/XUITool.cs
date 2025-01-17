using UILib;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using XUtliPoolLib;

public class XUITool : MonoBehaviour, IXUITool
{
	public static XUITool Instance { get { return s_instance; } }

    private Dictionary<int, uint> _TweenFadeInGroupDelayNum = new Dictionary<int, uint>();
    private IXGameUI m_gameui = null;
    private GameObject m_preloadBillboard = null;
    private GameObject m_preloadTitle = null;

    public IXGameUI XGameUI
    {
        get
        {
            if (m_gameui == null)
            {
                m_gameui = XInterfaceMgr.singleton.GetInterface<IXGameUI>(XCommon.singleton.XHash("XGameUI"));
            }
            return m_gameui;
        }
    }
    public Camera GetUICamera()
    {
        return UICamera.currentCamera;
    }

    public void SetActive(GameObject obj, bool state)
	{
		NGUITools.SetActive(obj, state);
	}
	
	public void SetLayer(GameObject go, int layer)
	{
		go.layer = layer;
		Transform t = go.transform;
		for (int i=0, imax = t.childCount; i<imax; ++i)
		{
			Transform child = t.GetChild(i);
			SetLayer(child.gameObject, layer);
		}
	}

    public void SetUIDepthDelta(GameObject go, int delta)
    {
    	XUISprite[] sp = go.GetComponentsInChildren<XUISprite>();
    	XUILabel[] la = go.GetComponentsInChildren<XUILabel>();
    	XUITexture[] te = go.GetComponentsInChildren<XUITexture>();

    	for(int i = 0; i < sp.Length; ++i)
    	{
    		sp[i].spriteDepth += delta;
    	}

		for(int i = 0; i < la.Length; ++i)
    	{
			la[i].spriteDepth += delta;
    	}

		for(int i = 0; i < te.Length; ++i)
    	{
			te[i].spriteDepth += delta;
    	}
    }
	
	public void SetUIEventFallThrough(GameObject obj)
	{
		UICamera.fallThrough = obj;
	}

    public void SetUIGenericEventHandle(GameObject obj)
    {
        UICamera.genericEventHandler = obj;
    }

    public void ShowTooltip(string str)
    {
        UITooltip.ShowText(str);
    }
	
	public void RegisterLoadUIAsynEventHandler(LoadUIAsynEventHandler eventHandler)
	{
		m_loadUIAsynEventHandler = eventHandler;
	}
	
	public void LoadResAsyn(string strFile, LoadUIFinishedEventHandler eventHandler)
	{
		if (null != m_loadUIAsynEventHandler)
		{
			//m_loadUIAsynEventHandler(strFile, eventHandler);
		}
		else
		{
			Debug.LogError("null == m_loadUIAsynEventHandler");
		}
		
	}
	
	public void SetCursor(string strSpriteName)
	{
		//UICursor.Set(strSpriteName);	
	}
	
	public void SetCursor(string strSprite, string strAtlas)
	{
//		if (null != UICursor.Instance)
//		{
//		}
	}
	
	public void PlayAnim(Animation anim, string strClipName, AnimFinishedEventHandler eventHandler)
	{
		if (null == anim || null == strClipName || strClipName.Length == 0)
		{
			eventHandler();
			return;
		}
		//ActiveAnimation.Play(anim, strClipName, Direction.Forward, EnableCondition.DoNothing, DisableCondition.DoNotDisable, eventHandler);
	}
	
	private void Awake()
	{
		s_instance = this;
	}

    public void MarkParentAsChanged(GameObject go)
    {
        NGUITools.MarkParentAsChanged(go);
    }
    public void HideGameObject(GameObject go)
    {
        NGUITools.ParentPanelChanged(go, XUICommon.singleton.m_inVisiablePanel);
    }

    public void ShowGameObject(GameObject go, IXUIPanel panel)
    {
        XUIPanel p = panel as XUIPanel;
        if (p != null)
            NGUITools.ParentPanelChanged(go, p.UIComponent as UIPanel);
        else
            NGUITools.MarkParentAsChanged(go);
    }

    public void ChangePanel(GameObject go, IUIRect parent, IXUIPanel panel)
    {
        XUIPanel p = panel as XUIPanel;
        UIRect pp = parent as UIRect;
        if (p != null)
            NGUITools.ParentPanelChanged(go, pp, p.UIComponent as UIPanel);

    }
    public void ChangePanel(GameObject go, IUIRect parent, IUIPanel panel)
    {
        UIPanel p = panel as UIPanel;
        UIRect pp = parent as UIRect;
        if (p != null)
            NGUITools.ParentPanelChanged(go, pp, p);

    }
    public string GetLocalizedStr(string key)
    {
        return Localization.Get(key);
    }

    void IXUITool.Destroy(UnityEngine.Object obj)
    {
    	NGUITools.Destroy(obj);
    }

    private LoadUIAsynEventHandler m_loadUIAsynEventHandler = null;
	//private string m_spriteNameForCusorCached = "";
	static XUITool s_instance = null;

    public Vector2 CalculatePrintedSize(string text)
    {
        return NGUIText.CalculatePrintedSize(text);
    }

    public void ReleaseAllDrawCall()
    {
        UIDrawCall.ReleaseInactive();
    }

    bool delayDealWith = false;
    void LateUpdate()
    {
        if(delayDealWith)
        {
            delayDealWith = false;
            ClearFadeInGroupDict();
        }
    }

    public bool GetTweenFadeInDelayByGroup(int group, float interval, int max, out float addDelay)
    {
        addDelay = 0f;

        if(_TweenFadeInGroupDelayNum.Count == 0)
        {
            //StartCoroutine(ClearFadeInGroupDict());
            delayDealWith = true;
        }

        uint num;
        if (!_TweenFadeInGroupDelayNum.TryGetValue(group, out num))
        {
            _TweenFadeInGroupDelayNum[group] = 0;
            num = 0;
            addDelay = 0;
            return true;
        }
        else
        {
            _TweenFadeInGroupDelayNum[group] = num + 1;
            num++;
            if (num >= max)
                return false;
            addDelay = num * interval;
            return true;
        }
    }

    void ClearFadeInGroupDict()
    {
        //yield return new WaitForEndOfFrame();
        _TweenFadeInGroupDelayNum.Clear();
    }

    public void ResetGroupDelay(int group)
    {
        if (_TweenFadeInGroupDelayNum.ContainsKey(group))
            _TweenFadeInGroupDelayNum.Remove(group);
    }

    public void SetRootPanelUpdateFreq(int count)
    {
        XUICommon.singleton.SetRootPanelUpdateFreq(count);
    }

    public void PreLoad(bool load)
    {
        if(load)
        {
            if (m_preloadBillboard == null)
                m_preloadBillboard = XResourceLoaderMgr.singleton.GetSharedResource<GameObject>("atlas/UI/common/Billboard", ".prefab");
            if (m_preloadTitle == null)
                m_preloadTitle = XResourceLoaderMgr.singleton.GetSharedResource<GameObject>("atlas/UI/common/Title", ".prefab");
        }
        else
        {
            XResourceLoaderMgr.SafeDestroyShareResource("atlas/UI/common/Billboard", ref m_preloadBillboard);
            XResourceLoaderMgr.SafeDestroyShareResource("atlas/UI/common/Title", ref m_preloadTitle);
        }
    }

    public void EnableUILoadingUpdate(bool enable)
    {
        NGUITools.mEnableLoadingUpdate = enable;
    }

	public void SetUIOptOption(bool globalMerge,bool selectMerge,bool lowDeviceMerge)
	{
		//disable global ui merge if we are using low device
		if ( XUpdater.XUpdater.singleton.XPlatform.GetQualityLevel () == 0 && !lowDeviceMerge) 
		{
			UIPanel.GlobalUseMerge = false;
			UIPanel.SelectUseMerge = selectMerge;
		} else {
			UIPanel.GlobalUseMerge = globalMerge;
			UIPanel.SelectUseMerge = selectMerge;
		}

	}
}

