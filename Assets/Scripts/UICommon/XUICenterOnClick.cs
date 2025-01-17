using System;
using UILib;
using UnityEngine;

public class XUICenterOnClick : XUIObject, IXUICenterOnClick
{

    protected override void OnAwake()
    {
        base.OnAwake();
        m_uiCenterOnClick = GetComponent<UICenterOnClick>();
        if (null == m_uiCenterOnClick)
        {
            Debug.LogError("null == m_uiCenterOnClick");
        }
    }

    public void OnClick()
    {
        m_uiCenterOnClick.OnClick();
    }

    private UICenterOnClick m_uiCenterOnClick = null;
}

