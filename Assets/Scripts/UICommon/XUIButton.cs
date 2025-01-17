using UILib;
using XUtliPoolLib;
using UnityEngine;
using System.Collections.Generic;

public class XUIButton : XUIObject, IXUIButton
{
    private static List<UILabel> tmpLabel = new List<UILabel>();
    private static List<XUILabel> tmpXLabel = new List<XUILabel>();
    private static List<UISprite> tmpSprite = new List<UISprite>();
    private static Color black = new Color(0.0f, 0.0f, 0.0f, 1.0f);

    private UIEventListener eventListenerCache = null;
    public int spriteWidth
    {
        get
        {
            return m_uiSpriteBG.width;
        }
    }

    public int spriteHeight
    {
        get
        {
            return m_uiSpriteBG.height;
        }
    }

    public int spriteDepth
    {
        get
        {
            return m_uiSpriteBG.depth;
        }
        set
        {
            m_uiSpriteBG.depth = value;
        }
    }

    public void SetSpriteWithPrefix(string prefix)
    {
        m_uiButton.normalSprite = prefix + "_0";
        m_uiButton.hoverSprite = prefix + "_0";
        m_uiButton.pressedSprite = prefix + "_1";
    }

    public void SetSprites(string normal, string hover, string press)
    {
        m_uiButton.normalSprite = normal;
        m_uiButton.hoverSprite = hover;
        m_uiButton.pressedSprite = press;
    }

    public void SetCaption(string strText)
    {
        GetComponentsInChildren<UILabel>(true, tmpLabel);
        if (tmpLabel.Count > 0)
        {
            tmpLabel[0].text = strText;
        }
        tmpLabel.Clear();
    }

    public void SetEnable(bool bEnable, bool withcollider = false)
    {
        if (m_bEnabled!=bEnable)
        {
            if (!bEnable)
            {
                if (m_state)
                    _ButtonPress(gameObject, false);

                m_bEnabled = bEnable;
            }
            else
            {
                m_bEnabled = bEnable;

                if (m_state)
                    _ButtonPress(gameObject, true);
            }
        }

        
		if (null != m_colliderCached)
		{
            m_colliderCached.enabled = bEnable || withcollider;
		}
		
		if (null !=  m_uiButtonColor)
		{
            m_uiButtonColor.enabled = bEnable || withcollider;
		}
		
		if (null != m_uiButtonScale)
		{
			m_uiButtonScale.enabled = bEnable;
		}
		
		if (null != m_uiButtonOffset)
		{
			m_uiButtonOffset.enabled = bEnable;
		}
		if (null != m_uiSpriteBG)
		{
            m_uiSpriteBG.color = bEnable ? Color.white : XUISprite.GREY_COLOR;

            gameObject.GetComponentsInChildren<UISprite>(true, tmpSprite);

            for (int i = 0; i < tmpSprite.Count; i++)
		    {
                tmpSprite[i].color = bEnable ? Color.white : XUISprite.GREY_COLOR;
		    }
            tmpSprite.Clear();
		}

        gameObject.GetComponentsInChildren<XUILabel>(tmpXLabel);
        for (int i = 0; i < tmpXLabel.Count; i++)
        {
            tmpXLabel[i].SetEnabled(bEnable);
        }
        tmpXLabel.Clear();
    }

    public void SetGrey(bool bGrey)
    {
        if (null != m_uiSpriteBG)
        {
            m_uiSpriteBG.color = bGrey ? Color.white : new Color(0.0f, 0.0f, 0.0f, 1);

            m_uiButtonColor.defaultColor = bGrey ? Color.white : black;
            m_uiButtonColor.hover = bGrey ? Color.white : black;
            m_uiButtonColor.pressed = bGrey ? Color.white : black;

            gameObject.GetComponentsInChildren<UISprite>(tmpSprite);

            for (int i = 0; i < tmpSprite.Count; i++)
            {
                tmpSprite[i].color = bGrey ? Color.white : black;
            }
            tmpSprite.Clear();
        }
    }

    public void CloseScrollView()
    {
        UIDragScrollView m_ScrollView = GetComponent<UIDragScrollView>();
        if (m_ScrollView != null)
        {
            m_ScrollView.enabled = false;
        }
    }

    public void ResetPanel()
    {
        m_uiSpriteBG.panel = null;
    }

    private UIEventListener GetUIEventListener()
    {
        if (eventListenerCache == null)
        {
            eventListenerCache = UIEventListener.Get(this.gameObject);
        }
        return eventListenerCache;
    }
    public void RegisterClickEventHandler(ButtonClickEventHandler eventHandler)
    {
        //UIButtonMessage uiButtonMessage = GetComponent<UIButtonMessage>();
        //if (null == uiButtonMessage)
        //{
        //    uiButtonMessage = gameObject.AddComponent<UIButtonMessage>();
        //}
		//uiButtonMessage.functionName = "OnButtonClick";

        GetUIEventListener().onClick -= OnButtonClick;
        GetUIEventListener().onClick += OnButtonClick;
        
        m_buttonClickEventHandler = eventHandler;
    }

    public ButtonClickEventHandler GetClickEventHandler()
    {
        return m_buttonClickEventHandler;
    }

    public ButtonPressEventHandler GetPressEventHandler()
    {
        return m_buttonPressEventHandler;
    }

    public void ResetState()
    {
        m_state = false;
    }

    public void RegisterPressEventHandler(ButtonPressEventHandler eventHandler)
    {
        UIEventListener.Get(this.gameObject).onPress = OnButtonPressed; 
        m_buttonPressEventHandler = eventHandler;
    }

    public void RegisterDragEventHandler(ButtonDragEventHandler eventHandler)
    {
        UIEventListener.Get(this.gameObject).onDrag = OnButtonDrag;
        m_buttonDragEventHandler = eventHandler;
    }

    void OnButtonPressed(GameObject button, bool state)
    {
        m_state = state;

        if (!m_bEnabled) return;

        _ButtonPress(button, state);
    }

    void OnButtonDrag(GameObject button, Vector2 delta)
    {
        if (!m_bEnabled) return;

        if (m_buttonDragEventHandler != null)
            m_buttonDragEventHandler(this, delta);
    }

    private void _ButtonPress(GameObject button, bool state)
    {
        if (m_tutorial != null && m_tutorial.NoforceClick && Exculsive)
        {
            if (null != m_buttonPressEventHandler)
            {
                //if (m_operation != null) m_operation.FindRecordID(button.transform);
                m_buttonPressEventHandler(this, state);
                
                if (state)
                    m_tutorial.OnTutorialClicked();
            }
        }
        if (m_tutorial == null || !m_tutorial.Exculsive)
        {
            if (null != m_buttonPressEventHandler)
            {
                //if (m_operation != null && state) m_operation.FindRecordID(button.transform);
                m_buttonPressEventHandler(this, state);
                
            }
        }
        else if (m_tutorial.Exculsive && Exculsive)
        {
            if (null != m_buttonPressEventHandler)
            {
                m_buttonPressEventHandler(this, state);
                //if (m_operation != null && state) m_operation.FindRecordID(button.transform);

                if (state) m_tutorial.OnTutorialClicked();
            }
        }
        else
        {
            XDebug.singleton.AddLog("Exculsive block");
        }
    }

    void OnButtonClick(GameObject button)
    {
        if (!m_bEnabled) return;

        if (m_CD.IsInCD())
            return;

        if (m_tutorial != null && m_tutorial.NoforceClick && Exculsive)
        {
            if (null != m_buttonClickEventHandler)
            {
                m_buttonClickEventHandler(this);
                m_tutorial.OnTutorialClicked();
            }
        }
        else if (m_tutorial == null || !m_tutorial.Exculsive)
        {
            if (null != m_buttonClickEventHandler)
            {
                m_buttonClickEventHandler(this);
                //if (m_operation != null) m_operation.FindRecordID(button.transform);
            }
        }
        else if (m_tutorial.Exculsive && Exculsive)
        {
            if (null != m_buttonClickEventHandler)
            {
                m_buttonClickEventHandler(this);
                //if (m_operation != null) m_operation.FindRecordID(button.transform);

                m_tutorial.OnTutorialClicked();
            }
        }
        else
        {
            XDebug.singleton.AddLog("Exculsive block");
        }

        if (m_NeedAudio && !string.IsNullOrEmpty(audioClip))
            NGUITools.PlayFmod("event:/" + audioClip);
    }

    public void SetAlpha(float f)
    {
        m_uiSpriteBG.alpha = f;
        m_uiButtonColor.defaultColor = new Color(m_uiButtonColor.defaultColor.r, m_uiButtonColor.defaultColor.g, m_uiButtonColor.defaultColor.b, f);
        m_uiButtonColor.hover.a = f;
        m_uiButtonColor.pressed.a = f;
    }
	
	protected override void OnAwake()
	{
		base.OnAwake();

        m_gameui = XInterfaceMgr.singleton.GetInterface<IXGameUI>(XCommon.singleton.XHash("XGameUI"));
        m_tutorial = XInterfaceMgr.singleton.GetInterface<IXTutorial>(XCommon.singleton.XHash("XTutorial"));
        //m_operation = XInterfaceMgr.singleton.GetInterface<IXOperationRecord>(XCommon.singleton.XHash("XOperationRecord"));
        m_uiButton = GetComponent<UIButton>();

        m_uiButtonColor = m_uiButton as UIButtonColor;// GetComponent<UIButtonColor>();
		
		m_uiButtonScale = GetComponent<UIButtonScale>();

        m_uiButtonOffset = GetComponent<UIButtonOffset>();
        
		
		m_uiSpriteBG = GetComponentInChildren<UISprite>();
#if UNITY_EDITOR
		if (null == m_uiSpriteBG)
		{
			XDebug.singleton.AddLog("null == m_uiSpriteBG");
        }
#endif
        if (m_uiButton!=null)
            m_colliderCached = m_uiButton.cacheCol;
        else
            m_colliderCached = GetComponent<Collider>();
#if UNITY_EDITOR
        if (null == m_colliderCached)
        {
            XDebug.singleton.AddLog("null == m_colliderCached:", gameObject.name);
        }
#endif
        CloneFromTpl();

        if (m_NeedAudio && (string.IsNullOrEmpty(audioClip) || !audioClip.StartsWith("Audio")))
            SetAudioClip("Audio/UI/UI_Button_ok");

        m_CD.SetClickCD(CustomClickCDGroup, CustomClickCD);

        if (m_uiButton != null)
            m_uiButton.changeStateSprite = ChangeStateSprite;
	}

    protected void CloneFromTpl()
    {
        if (m_ButtonAnimationType <= 0) return;

        GameObject tpl = m_gameui.buttonTpl[m_ButtonAnimationType - 1];

        XUICommon.CloneTplTweens(tpl, gameObject);

        if (m_useSprite)
        {
            UISprite srcSp = tpl.GetComponent<UISprite>();
            UIButton srcBtn = tpl.GetComponent<UIButton>();

            UISprite dstSp = gameObject.GetComponent<UISprite>();
            UIButton dstBtn = gameObject.GetComponent<UIButton>();

            dstSp.spriteName = srcSp.spriteName;
            dstBtn.normalSprite = srcBtn.normalSprite;
            dstBtn.hoverSprite = srcBtn.hoverSprite;
            dstBtn.pressedSprite = srcBtn.pressedSprite;
        }
    }

    public void SetAudioClip(string name)
    {
        if (m_NeedAudio == false)
            return;

        audioClip = name;
    }

    public void SetClickCD(float cd)
    {
        CustomClickCD = cd;
        m_CD.SetClickCD(CustomClickCDGroup, CustomClickCD);
    }

    public void ResetCD()
    {
        m_CD.Reset();
    }

    public void SetUnavailableCD(int cd)
    {
        m_CD.SetUnavailableCD(cd);
    }

    public int m_ButtonAnimationType = 0;
    public bool ChangeStateSprite = false;
    public bool m_useSprite = false;
    public string audioClip;
    public bool m_NeedAudio = true;

    public float CustomClickCD = -1f;
    public int CustomClickCDGroup = 0;

    private XUICD m_CD = new XUICD();

    [Range(0f, 1f)]
    public float volume = 1f;
    [Range(0f, 2f)]
    public float pitch = 1f;

	private UIButtonOffset m_uiButtonOffset = null;
	private UIButtonScale m_uiButtonScale = null;
	private UIButtonColor m_uiButtonColor = null;
    private UIButton m_uiButton = null;
	private Collider m_colliderCached = null;
	private UISprite m_uiSpriteBG = null;
    private bool m_bEnabled = true;

    private IXTutorial m_tutorial = null;
    private IXGameUI m_gameui = null;
    //private IXOperationRecord m_operation = null;

    private ButtonClickEventHandler m_buttonClickEventHandler = null;
    private ButtonPressEventHandler m_buttonPressEventHandler = null;
    private ButtonDragEventHandler m_buttonDragEventHandler = null;
    private bool m_state = false;
}

