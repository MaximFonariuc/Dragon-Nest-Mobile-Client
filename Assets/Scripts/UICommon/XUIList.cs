using UILib;
using UnityEngine;

public class XUIList : XUIObject, IXUIList
{
    public UIRect _parent;
    public void Refresh()
    {
        m_uiGrid.repositionNow = true;
        //m_uiGrid.Reposition();
    }

    public void CloseList()
    {
        m_uiGrid.CloseList();
    }

    public void SetAnimateSmooth(bool b)
    {
        m_uiGrid.animateSmoothly = b;
    }
    public void RegisterRepositionHandle(OnAfterRepostion reposition)
    {
        onRepositionFinish = reposition;
    }

    private void OnAfterReposition()
    {
        if (onRepositionFinish != null)
        {
            onRepositionFinish();
        }
    }
    public IUIRect GetParentUIRect()
    {
        return _parent;
    }
    public IUIPanel GetParentPanel()
    {
        return m_uiGrid.panel;
    }

    protected override void OnAwake()
    {
        base.OnAwake();
        m_uiGrid = GetComponent<UIGrid>();
        if (m_uiGrid != null)
            m_uiGrid.onReposition = OnAfterReposition;
        else
            Debug.Log("no ngui grid component");
    }

    private UIGrid m_uiGrid;

    private OnAfterRepostion onRepositionFinish = null;

}
