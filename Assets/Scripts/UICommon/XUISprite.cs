using UILib;
using UnityEngine;
using XUtliPoolLib;

public class XUISprite : XUIObject, IXUISprite
{
	public IXUIAtlas uiAtlas
	{
		get
		{
			if (null != m_uiSprite)
			{
				if (null == m_uiSprite.atlas)
				{
					return null;
				}
				return m_uiSprite.atlas.GetComponent<XUIAtlas>();
			}
			else
			{
				return null;
			}
		}
	}
	
	public string spriteName
	{
		get
		{
			if (null != m_uiSprite)
			{
				return m_uiSprite.spriteName;
			}
			else
			{
				return null;
			}
		}

	    set
	    {
            if (!SetSpriteByCode(value))
            {
                m_uiSprite.spriteName = value;
            }
	    }
	}

    public int spriteWidth
    {
        get
        {
            return m_uiSprite.width;
        }
        set
        {
            m_uiSprite.width = value;
        }
    }

    public int spriteHeight {
        get
        {
            return m_uiSprite.height;
        }
        set
        {
            m_uiSprite.height = value;
        }
    }

    public int spriteDepth
    {
        get
        {
            return m_uiSprite.depth;
        }
        set
        {
            m_uiSprite.depth = value;
        }
    }

    public string atlasPath
    {
        get
        {
            return m_uiSprite.atlasPath;
        }
    }

    public Vector4 drawRegion
    {
        get
        {
            return m_uiSprite.drawRegion;
        } 
        set 
        {
            m_uiSprite.drawRegion = value;
        } 
    }


    public bool SetSpriteByCode(string code)
    {
        if (!string.IsNullOrEmpty(code))
        {
            if (code[0] == '|')
            {
                string[] str = code.Split('|');
                if (str.Length == 3)
                {
                    SetSprite(str[1], str[2]);
                }
                return true;
            }
        }
        return false;
    }

	public bool SetSprite(string strSprite, string strAtlas, bool fullAtlasName = false)
	{
		if (null == m_uiSprite)
		{
			return false;
		}
        m_uiSprite.spriteName = strSprite;
        if(string.IsNullOrEmpty(strAtlas))
        {
            m_uiSprite.SetAtlas("");
        }
        else
        {
            if(fullAtlasName)
                m_uiSprite.SetAtlas(strAtlas);
            else
                m_uiSprite.SetAtlas("atlas/UI/" + strAtlas);
        }
        

        return true;
	}

    public void MakePixelPerfect()
    {
        if (m_uiSprite == null)
        {
            XDebug.singleton.AddErrorLog("Sprite is Null");
            return;
        }
        m_uiSprite.MakePixelPerfect();
    }

    public void SetRootAsUIPanel(bool bFlg)
    {
        if (bFlg)
        {
            m_uiSprite.SetPanel(XUICommon.singleton.m_uiRootPanel);
        }
        else
        {
            m_uiSprite.SetPanel(null);
        }
    }

    public void SetColor(Color c)
    {
        m_uiSprite.color = c;
    }
	
	private void OnLoadAtlasFinished(UnityEngine.Object obj)
	{
		UIAtlas uiAtlas = obj as UIAtlas;
		if (null == uiAtlas)
		{
			Debug.LogError("null == uiAtlas");
			return;
		}
        m_uiSprite.spriteName = m_spriteNameCached;
        m_uiSprite.atlas = uiAtlas;
		
	}

    public void ResetPanel()
    {
        m_uiSprite.panel = null;
    }
	
	public bool SetSprite(string strSpriteName)
	{
		if (null == m_uiSprite)
		{
			return false;
		}
        if (!SetSpriteByCode(strSpriteName))
        {
            m_uiSprite.spriteName = strSpriteName;
        }
		return true;
	}

    public void SetEnabled(bool bEnabled)
    {
        m_bEnabled = bEnabled;

        SetGrey(bEnabled);
    }

    public static Color GREY_COLOR = new Color(0, 0, 0, 0.7f);
    public void SetGrey(bool bGrey)
    {
        if (bGrey)
        {
            m_uiSprite.color = m_sourceColor;
        }
        else
        {
            m_uiSprite.color = GREY_COLOR;
        }

        UISprite[] sp = gameObject.GetComponentsInChildren<UISprite>(true);

        for (int i = 0; i < sp.Length; i++)
        {
            sp[i].color = bGrey ? Color.white : GREY_COLOR;
        }
    }

    public void SetAlpha(float alpha)
    {
        m_uiSprite.color = new Color(m_uiSprite.color.r, m_uiSprite.color.g, m_uiSprite.color.b, alpha);
    }

    public float GetAlpha()
    {
        return m_uiSprite.alpha;
    }

    public void SetAudioClip(string name)
    {
        audioClip = name;
    }

    public void CloseScrollView()
    {
        UIDragScrollView m_ScrollView= GetComponent<UIDragScrollView>();
        if (m_ScrollView != null)
        {
            m_ScrollView.enabled = false;
        }
    }

    public void RegisterSpriteClickEventHandler(SpriteClickEventHandler eventHandler)
    {
        UIEventListener.Get(this.gameObject).onClick -= OnSpriteClick;
        UIEventListener.Get(this.gameObject).onClick += OnSpriteClick;

		m_spriteClickEventHandler = eventHandler;
    }

    public void RegisterSpritePressEventHandler(SpritePressEventHandler eventHandler)
	{
		m_spritePressEventHandler = eventHandler;
	}
	
	public void RegisterSpriteDragEventHandler(SpriteDragEventHandler eventHandler)
	{
		m_spriteDragEventHandler = eventHandler;
	}
	
	protected override void OnAwake()
	{
		base.OnAwake();

        m_gameui = XInterfaceMgr.singleton.GetInterface<IXGameUI>(XCommon.singleton.XHash("XGameUI"));
        m_tutorial = XInterfaceMgr.singleton.GetInterface<IXTutorial>(XCommon.singleton.XHash("XTutorial"));
        m_operation = XInterfaceMgr.singleton.GetInterface<IXOperationRecord>(XCommon.singleton.XHash("XOperationRecord"));

		m_uiSprite = GetComponent<UISprite>();
        if (!string.IsNullOrEmpty(SpriteAtlasPath))
        {
            SetSprite(SPriteName, SpriteAtlasPath);
            SPriteName = "";
            SpriteAtlasPath = "";
        }
		if (null == m_uiSprite)
		{
			Debug.Log("null == m_uiSprite," + this.gameObject.name);
		}
        m_sourceColor = m_uiSprite.color;

        CloneFromTpl();

        if (m_NeedAudio && (string.IsNullOrEmpty(audioClip) || !audioClip.StartsWith("Audio")))
            SetAudioClip("Audio/UI/UI_Button_ok");

        m_CD.SetClickCD(CustomClickCDGroup, CustomClickCD);

        ClickCanceled = false;
	}

    protected void CloneFromTpl()
    {
        if (m_SpriteAnimationType <= 0) return;

        GameObject tpl = m_gameui.spriteTpl[m_SpriteAnimationType - 1];

        XUICommon.CloneTplTweens(tpl, gameObject);
    }

    void OnSpriteClick(GameObject button)
    {
        if (!m_bEnabled) return;
        if (ClickCanceled)
        {
            ClickCanceled = false;
            return;
        }

        if (m_CD.IsInCD())
            return;

        if (m_tutorial != null && m_tutorial.NoforceClick && Exculsive)
        {
            if (null != m_spriteClickEventHandler)
            {
                m_spriteClickEventHandler(this);
                if (m_operation != null) m_operation.FindRecordID(button.transform);
            }
            m_tutorial.OnTutorialClicked();
            Exculsive = false;
        }
        else if (m_tutorial == null || !m_tutorial.Exculsive)
        {
            if (null != m_spriteClickEventHandler)
            {
                m_spriteClickEventHandler(this);
                if (m_operation != null) m_operation.FindRecordID(button.transform);
            }
        }
        else if (m_tutorial.Exculsive && Exculsive)
        {
            if (null != m_spriteClickEventHandler)
            {
                m_spriteClickEventHandler(this);
                if (m_operation != null) m_operation.FindRecordID(button.transform);
            }
            m_tutorial.OnTutorialClicked();
            Exculsive = false;
        }
        else
        {
            XDebug.singleton.AddLog("Exculsive block");
        }
        if (m_NeedAudio && !string.IsNullOrEmpty(audioClip))
            NGUITools.PlayFmod("event:/" + audioClip);
	}

    public SpriteClickEventHandler GetSpriteClickHandler()
    {
        return m_spriteClickEventHandler;
    }

    public SpritePressEventHandler GetSpritePressHandler()
    {
        return m_spritePressEventHandler;
    }
	
	protected override void OnPress(bool isPressed)
	{
        if (!m_bEnabled) return;

		if (null != m_spritePressEventHandler)
		{
            m_spritePressEventHandler(this, isPressed);
		}
	}

    protected override void OnDrag(Vector2 delta)
	{
        if (!m_bEnabled) return;

        if (null != m_spriteDragEventHandler && m_invalidDrag)
		{
			m_spriteDragEventHandler(delta);
		}
	}

    protected void OnDragOut(GameObject go)
    {
        m_invalidDrag = false;
    }

    protected void OnDragOver(GameObject go)
    {
        m_invalidDrag = true;
    }
    
    public void SetFillAmount(float val)
    {
        m_uiSprite.fillAmount = val;
    }

    public void SetFlipHorizontal(bool bValue)
    {
        if(bValue)
            m_uiSprite.flip = UISprite.Flip.Horizontally;
        else 
            m_uiSprite.flip = UISprite.Flip.Nothing;
    }

    public void SetFlipVertical(bool bValue)
    {
        if (bValue)
            m_uiSprite.flip = UISprite.Flip.Vertically;
        else
            m_uiSprite.flip = UISprite.Flip.Nothing;
    }

    public void ResetAnimationAndPlay()
    {
        m_Animation = GetComponent<UISpriteAnimation>();

        if (m_Animation == null) return;

        gameObject.SetActive(true);

        m_Animation.Reset();

        m_Animation.LastLoopFinishTime = RealTime.time;
    }

    public void UpdateAnchors()
    {
        m_uiSprite.UpdateAnchors();
    }

    public bool IsEnabled()
    {
        return m_bEnabled;
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

    public IXUIPanel GetPanel()
    {
        return XUIPanel.GetPanel(m_uiSprite.panel);
    }

    public int m_SpriteAnimationType = 0;
	private UISprite m_uiSprite = null;
    public string audioClip;
    public bool m_NeedAudio = true;

    public float CustomClickCD = -1f;
    public int CustomClickCDGroup = 0;

    private XUICD m_CD = new XUICD();

    [Range(0f, 1f)]
    public float volume = 1f;
    [Range(0f, 2f)]
    public float pitch = 1f;

    public string SpriteAtlasPath = "";
    public string SPriteName = "";
    //private UIPanel m_uiRootPanel = null;
    private UISpriteAnimation m_Animation = null;
    private SpriteClickEventHandler m_spriteClickEventHandler = null;
    //private SpriteClickEventHandler m_spriteLongPressEventHandler = null;
    private SpritePressEventHandler m_spritePressEventHandler = null;
	private SpriteDragEventHandler m_spriteDragEventHandler = null;
	private string m_spriteNameCached = "";
    private bool m_bEnabled = true;
    private Color m_sourceColor;

    private IXTutorial m_tutorial = null;
    private IXGameUI m_gameui = null;
    private IXOperationRecord m_operation = null;

    private bool m_invalidDrag = false;

    public bool ClickCanceled { get; set; }
}

