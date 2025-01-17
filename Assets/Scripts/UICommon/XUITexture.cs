using UILib;
using UnityEngine;
using XUtliPoolLib;

public class XUITexture : XUIObject, IXUITexture
{
    public int spriteWidth
    {
        get
        {
            return m_uiTexture.width;
        }
        set
        {
        	m_uiTexture.width = value;
        }
    }

    public int spriteHeight
    {
        get
        {
            return m_uiTexture.height;
        }
        set
        {
        	m_uiTexture.height = value;
        }
    }

    public int spriteDepth
    {
        get
        {
            return m_uiTexture.depth;
        }
        set
        {
            m_uiTexture.depth = value;
        }
    }

    public int aspectRatioSource 
    {
        get
        {
            return (int)m_uiTexture.keepAspectRatio;
        }
        set
        {
            m_uiTexture.keepAspectRatio = (UIWidget.AspectRatioSource)value;
        }
    }
    public void SetRuntimeTex(Texture texture, bool autoDestroy = true)
    {
        if (null != m_uiTexture && !m_uiTexture.Equals(null) && (m_uiTexture is UITexture))
        {
            (m_uiTexture as UITexture).SetRuntimeTexture(texture, autoDestroy);
        } 
    }
    

    public void SetTexturePath(string path)
    {
        if (null != m_uiTexture && !m_uiTexture.Equals(null))
        {
            m_uiTexture.SetTexture(path/*, m_uiTexture.mtexType*/);
        }
    }

    //public Texture GetTexture()
    //{
    //    return m_uiTexture.mainTexture;
    //}
    public void SetUVRect(Rect rect)
    {
        if(m_uiTexture is UITexture)
            (m_uiTexture as UITexture).uvRect = rect;
    }

    public void SetEnabled(bool bEnabled)
    {
        if (bEnabled)
        {
            m_uiTexture.color = Color.white;
        }
        else
        {
            m_uiTexture.color = new Color(0, 0, 0, 1);
        }
    }

    public void SetColor(Color color)
    {
        m_uiTexture.color = color;
    }

    public void SetAlpha(float alpha)
    {
        m_uiTexture.alpha = alpha;
    }

    public void MakePixelPerfect()
    {
        m_uiTexture.MakePixelPerfect();
    }

    public void RegisterLabelClickEventHandler(TextureClickEventHandler eventHandler)
    {
        UIEventListener.Get(this.gameObject).onClick = OnTextureClick;
        m_textureClickEventHandler = eventHandler;
    }

    void OnTextureClick(GameObject button)
    {

        if (m_CD.IsInCD())
            return;

        if (m_tutorial != null && m_tutorial.NoforceClick && Exculsive)
        {
            if (null != m_textureClickEventHandler)
            {
                m_textureClickEventHandler(this);
                if (m_operation != null) m_operation.FindRecordID(button.transform);
            }
            m_tutorial.OnTutorialClicked();
            Exculsive = false;
        }
        else if (m_tutorial == null || !m_tutorial.Exculsive)
        {
            if (null != m_textureClickEventHandler)
            {
                m_textureClickEventHandler(this);
                if (m_operation != null) m_operation.FindRecordID(button.transform);
            }
        }
        else if (m_tutorial.Exculsive && Exculsive)
        {
            if (null != m_textureClickEventHandler)
            {
                m_textureClickEventHandler(this);
                if (m_operation != null) m_operation.FindRecordID(button.transform);
            }
            m_tutorial.OnTutorialClicked();
            Exculsive = false;
        }
        else
        {
            XDebug.singleton.AddLog("Exculsive block");
        }
    }

    protected override void OnAwake()
    {
        base.OnAwake();
        m_uiTexture = GetComponent<UITexture>();
        m_tutorial = XInterfaceMgr.singleton.GetInterface<IXTutorial>(XCommon.singleton.XHash("XTutorial"));
        m_operation = XInterfaceMgr.singleton.GetInterface<IXOperationRecord>(XCommon.singleton.XHash("XOperationRecord"));
        //if (!string.IsNullOrEmpty(TexturePath))
        //{
        //    SetTexturePath("atlas/UI/" + TexturePath);
        //}
        //if (null == m_uiTexture)
        //{
        //    Debug.LogError("null == m_uiTexture");
        //}

        m_CD.SetClickCD(CustomClickCDGroup, CustomClickCD);
    }

    public TextureClickEventHandler GetTextureClickHandler()
    {
        return m_textureClickEventHandler;
    }

    public void CloseScrollView()
    {
        UIDragScrollView m_ScrollView = GetComponent<UIDragScrollView>();
        if (m_ScrollView != null)
        {
            m_ScrollView.enabled = false;
        }
    }

    public void SetClickCD(float cd)
    {
        CustomClickCD = cd;
        m_CD.SetClickCD(CustomClickCDGroup, CustomClickCD);
    }

    private IXTutorial m_tutorial = null;
    private UITexture m_uiTexture = null;
    private TextureClickEventHandler m_textureClickEventHandler;
    private IXOperationRecord m_operation = null;

    public float CustomClickCD = -1f;
    public int CustomClickCDGroup = 0;

    //public string TexturePath = "";

    private XUICD m_CD = new XUICD();
}
