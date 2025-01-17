using System;
using UILib;
using UnityEngine;
using XUtliPoolLib;

public class XUICheckBox : XUIObject, IXUICheckBox
{
    public bool bChecked
    {
        get { return m_uiToggle.value; }
        set
        {
            m_uiToggle.value = value;
        }
    }

    public int spriteHeight
    {
    	get
    	{
            return m_uiSpriteBG.height;
    	}
    	set
    	{
    		m_uiSpriteBG.height = value;
    	}
    }

    public int spriteWidth
    {
    	get
    	{
    		return m_uiSpriteBG.width;
    	}
    	set
    	{
    		m_uiSpriteBG.width = value;
    	}
    }

    public bool bInstantTween 
    { 
        get { return m_uiToggle.instantTween; }
        set { m_uiToggle.instantTween = value; }
    }

    public void ForceSetFlag(bool bCheckd)
    {
        m_uiToggle.ForceSetActive(bCheckd);
    }

    public void RegisterOnCheckEventHandler(CheckBoxOnCheckEventHandler eventHandler)
    {
        m_stateChangeHandler = eventHandler;

        EventDelegate.Add(m_uiToggle.onChange, OnStateChange);

        UIEventListener.Get(this.gameObject).onClick -= OnTabClick;
        UIEventListener.Get(this.gameObject).onClick += OnTabClick;
    }

    public CheckBoxOnCheckEventHandler GetCheckEventHandler()
    {
        return m_stateChangeHandler;
    }

    public void SetEnable(bool bEnable)
    {
        if (null != m_colliderCached)
        {
            m_colliderCached.enabled = bEnable;
        }
        if (null != m_uiSpriteBG)
        {
            m_uiSpriteBG.color = bEnable ? Color.white : new Color(0.3f, 0.3f, 0.3f, 1);
        }
       
    }

    public void OnStateChange()
    {
            if (m_stateChangeHandler != null)
            {
                m_stateChangeHandler(this);
            }
    }

    public void SetAlpha(float f)
    {
        m_uiSpriteBG.alpha = f;
    }

    private void OnTabClick(GameObject button)
    {
        if (m_tutorial!=null&&m_tutorial.Exculsive && Exculsive)
        {
            m_tutorial.OnTutorialClicked();
        }

        if (m_NeedAudio && !string.IsNullOrEmpty(audioClip))
            NGUITools.PlayFmod("event:/" + audioClip);
    }

    protected override void OnAwake()
    {
        base.OnAwake();

        m_tutorial = XInterfaceMgr.singleton.GetInterface<IXTutorial>(XCommon.singleton.XHash("XTutorial"));
        m_gameui = XInterfaceMgr.singleton.GetInterface<IXGameUI>(XCommon.singleton.XHash("XGameUI"));

        m_colliderCached = GetComponent<Collider>();
        if (null == m_colliderCached)
        {
            Debug.Log("null == m_colliderCached");
        }
        m_uiToggle = GetComponent<UIToggle>();
        if (null == m_uiToggle)
        {
            Debug.Log("null == m_uiToggle");
        }
        m_uiSpriteBG = GetComponentInChildren<UISprite>();
        if (null == m_uiSpriteBG)
        {
            Debug.Log("null == m_uiSpriteBG");
        }

        CloneFromTpl();

        if (m_NeedAudio && (string.IsNullOrEmpty(audioClip) || !audioClip.StartsWith("Audio")))
            SetAudioClip("Audio/UI/UI_Button_ok");
    }

    protected void CloneFromTpl()
    {
        if (m_SpriteAnimationType <= 0) return;

        GameObject tpl = m_gameui.spriteTpl[m_SpriteAnimationType - 1];

        XUICommon.CloneTplTweens(tpl, gameObject);
    }

    public void SetAudioClip(string name)
    {
        if (m_NeedAudio == false)
            return;

        audioClip = name;
    }

    public void SetGroup(int group)
    {
        m_uiToggle.group = group;
    }

    public string audioClip;
    public bool m_NeedAudio = true;

    [Range(0f, 1f)]
    public float volume = 1f;
    [Range(0f, 2f)]
    public float pitch = 1f;

    public int m_SpriteAnimationType = 0;

    private IXTutorial m_tutorial = null;
    private IXGameUI m_gameui = null;

    private Collider m_colliderCached = null;
    private UISprite m_uiSpriteBG = null;
    private UIToggle m_uiToggle;
    private CheckBoxOnCheckEventHandler m_stateChangeHandler;
}

