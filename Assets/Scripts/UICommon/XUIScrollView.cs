using System;
using UILib;
using UnityEngine;
using XUtliPoolLib;

public class XUIScrollView : XUIObject, IXUIScrollView
{

    protected override void OnAwake()
    {
		base.OnAwake();
        m_uiScrollView = GetComponent<UIScrollView>();
        if (null == m_uiScrollView)
        {
            XDebug.singleton.AddErrorLog("null == m_uiScrollView");
        }
    }

    public void ResetPosition()
    {
        m_uiScrollView.ResetPosition();
    }

    public void UpdatePosition()
    {
        m_uiScrollView.UpdatePosition();
    }

    public void SetCustomMovement(Vector2 movment)
    {
        m_uiScrollView.customMovement = movment;
    }

    public void SetPosition(float pos)
    {
        Vector2 pv = NGUIMath.GetPivotOffset(m_uiScrollView.contentPivot);
        m_uiScrollView.SetDragAmount(pv.x, pos, false);

        // Next move the clipping area back and update the scroll bars
        m_uiScrollView.SetDragAmount(pv.x, pos, true);
    }

    public void SetDragPositionX(float pos)
    {
        Vector2 pv = NGUIMath.GetPivotOffset(m_uiScrollView.contentPivot);
        m_uiScrollView.SetDragAmount(pos+pv.x, pv.y, false);

        // Next move the clipping area back and update the scroll bars
        m_uiScrollView.SetDragAmount(pos + pv.x, pv.y, true);
    }

    public void SetDragFinishDelegate(Delegate func)
    {
        m_uiScrollView.onDragFinished = (UIScrollView.OnDragFinished)func;
    }

    public void SetAutoMove(float from, float to, float moveSpeed)
    {
        m_uiScrollView.SetAutoMove(from, to, moveSpeed);
    }

    public bool RestrictWithinBounds(bool instant)
    {
        return m_uiScrollView.RestrictWithinBounds(instant);
    }

    public void MoveAbsolute(Vector3 absolute)
    {
        m_uiScrollView.MoveAbsolute(absolute);
    }

    public void MoveRelative(Vector3 relative)
    {
        m_uiScrollView.MoveRelative(relative);
    }

    public void NeedRecalcBounds()
    {
        m_uiScrollView.NeedRecalcBounds();
    }

    private UIScrollView m_uiScrollView = null;
}

