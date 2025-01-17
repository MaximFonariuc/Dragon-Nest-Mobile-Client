using System;

public class XUIObject : XUIObjectBase
{
	protected override void OnAwake()
	{
		base.OnAwake();
		if (null == transform.parent)
        {
            parent = null;
            return;
        }
        //XUIObjectBase uiObject = NGUITools.FindInParents<XUIObjectBase>(transform.parent.gameObject);
        //if (null != uiObject)
        //{
        //    parent = uiObject;
        //}
	}
    public override UILib.IXUIObject parent
    {
        get
        {
            if(!mParentCached)
            {
                XUIObjectBase uiObject = NGUITools.FindInParents<XUIObjectBase>(transform.parent.gameObject);
                if (null != uiObject)
                {
                    base.parent = uiObject;
                }
                mParentCached = true;
            }
            return base.parent;
        }
        set
        {
            base.parent = value;
            mParentCached = true;
        }
    }

    private bool mParentCached = false;
}

public class XUICD
{
    public static readonly float DEFAULT_CLICK_CD = 0.0f;
    public static readonly float[] CLICK_CD_GROUPS = new float[] { 0.0f, 0.5f, 1.0f };

    protected float m_ClickCD;
    protected float m_ClickTime = 0f;

    public void SetClickCD(int customCDGroup, float customCD)
    {
        if (customCD >= 0)
        {
            m_ClickCD = customCD;
        }
        else if (customCDGroup < 0 || customCDGroup >= CLICK_CD_GROUPS.Length)
        {
            m_ClickCD = DEFAULT_CLICK_CD;
        }
        else
        {
            m_ClickCD = CLICK_CD_GROUPS[customCDGroup];
        }
    }

    public bool IsInCD()
    {
        float time = UnityEngine.Time.realtimeSinceStartup;
        if (time - m_ClickTime < m_ClickCD)
            return true;
        m_ClickTime = time;
        return false;
    }

    public void Reset()
    {
        m_ClickTime = 0;
    }

    public void SetUnavailableCD(int cd)
    {
        float time = UnityEngine.Time.realtimeSinceStartup;
        m_ClickTime = time;

        SetClickCD(0, cd);
    }
}