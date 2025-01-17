using UILib;
using UnityEngine;

public class XUIScrollBar : XUIObject, IXUIScrollBar
{

    public float value
    {
        get { return m_uiScrollBar.value; }
        set { m_uiScrollBar.value = value; }
    }

    public float size
    {
        get { return m_uiScrollBar.barSize; }
        set { m_uiScrollBar.barSize = value; }
    }

    public void RegisterScrollBarChangeEventHandler(ScrollBarChangeEventHandler eventHandler)
    {
        //m_scrollBarChangeEventHandler = eventHandler;
    }

    public void RegisterScrollBarDragFinishedEventHandler(ScrollBarDragFinishedEventHandler eventHandler)
    {
        //m_scrollBarDragFinishedEventHandler = eventHandler;
    }

    protected override void OnAwake()
    {
        base.OnAwake();
        m_uiScrollBar = GetComponent<UIScrollBar>();
        if (null == m_uiScrollBar)
        {
            Debug.LogError("null == m_uiScrollBar");
        }
    }

    private UIScrollBar m_uiScrollBar = null;
    //private ScrollBarChangeEventHandler m_scrollBarChangeEventHandler = null;
    //private ScrollBarDragFinishedEventHandler m_scrollBarDragFinishedEventHandler = null;
}