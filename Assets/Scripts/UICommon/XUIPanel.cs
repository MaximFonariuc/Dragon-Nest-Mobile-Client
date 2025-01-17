using UILib;
using UnityEngine;
using System;

public class XUIPanel : XUIObject, IXUIPanel
{
    protected override void OnAwake()
    {
		base.OnAwake();        
        if (null == m_uiPanel)        
        {
            m_uiPanel = GetComponent<UIPanel>();
            if (null == m_uiPanel)
            {
                Debug.LogError("null == m_uiPanel");
            }
        }
        m_uiPanel.onClipMove += OnMove;
    }

    public void OnMove(UIPanel panel)
    {
        if (onMoveDel != null) onMoveDel();
    }

    public void SetSize(float width, float height)
    {
        Vector4 origin = m_uiPanel.baseClipRegion;
        m_uiPanel.baseClipRegion = new Vector4(origin.x, origin.y, width, height);
    }

    public void SetCenter(float width, float height)
    {
        Vector4 origin = m_uiPanel.baseClipRegion;
        m_uiPanel.baseClipRegion = new Vector4(width, height, origin.x, origin.y);
    }
    public Vector4 GetBaseRect()
    {
        return m_uiPanel.baseClipRegion;
    }

    public void SetAlpha(float a)
    {
        m_uiPanel.alpha = a;
    }

    public float GetAlpha()
    {
        return m_uiPanel.alpha;
    }

    public void SetDepth(int d)
    {
    	m_uiPanel.depth = d;
    }

    public int GetDepth()
    {
    	return m_uiPanel.depth;
    }

    public Vector2 offset
    {
        get
        {
            return m_uiPanel.clipOffset;
        }
        set
        {
            m_uiPanel.clipOffset = value;
        }
    }

    public bool IsVisible(GameObject go)
    {
        return m_uiPanel.IsVisible(go.GetComponent<UIWidget>());
    }

    public Vector4 ClipRange
    {
        get
        {
            return m_uiPanel.baseClipRegion;
        }
        set
        {
            m_uiPanel.baseClipRegion = value;
        }
    }

    public Vector2 softness
    {
        get
        {
            return m_uiPanel.clipSoftness;
        }
        set
        {
            m_uiPanel.clipSoftness = value;
        }
    }

    public UIPanel m_uiPanel = null;

    public Action onMoveDel { get; set; }

    public Component UIComponent { get { return m_uiPanel; } }


    public static IXUIPanel GetPanel(UIPanel panel)
    {
        if (panel == null)
            return null;

        XUIPanel xPanel = panel.GetComponent<XUIPanel>();
        if (xPanel == null)
        {
            xPanel = panel.gameObject.AddComponent<XUIPanel>();
        }

        return xPanel;
    }
}

