using UILib;
using UnityEngine;
using System;

public class XUIDragDropItem : XUIObject, IXUIDragDropItem
{
 
    private UIPanel  m_panel;
    private void Awake()
    {
        m_dragdrop = GetComponent<UIDragDropItem>();

        if (null == m_dragdrop)
        {
            Debug.LogError("null == m_dragdrop");
        }
    }

    public void SetCloneOnDrag(bool cloneOnDrag)
    {
        m_dragdrop.cloneOnDrag = cloneOnDrag;
    }

    public void SetRestriction(int restriction)
    {
        m_dragdrop.restriction = (UIDragDropItem.Restriction)Enum.ToObject(typeof(UIDragDropItem.Restriction), restriction);
    }


    public void SetActive(bool active)
    {
        m_dragdrop.enabled = active;
        enabled = active;
    }


    public void SetParent(Transform parent , bool addPanel = false, int depth = 0)
    {
        transform.parent = parent;
        m_panel = gameObject.GetComponent<UIPanel>();
        if (addPanel)
        {
            m_panel = gameObject.GetComponent<UIPanel>();
            if (m_panel == null)  m_panel = gameObject.AddComponent<UIPanel>();
            m_panel.depth = depth;
            m_panel.enabled = true;
        }
        else
        {
            if (m_panel != null) m_panel.enabled = false;
        }
    }

    public void RegisterOnFinishEventHandler(OnDropReleaseEventHandler eventHandler)
    {
        m_OnFinishHandler = eventHandler;

        //m_uiPlayTween.customFinishCallback = OnFinish;
        m_dragdrop.onFinished.Clear();
        EventDelegate.Add(m_dragdrop.onFinished, OnFinish);
    }

    public void RegisterOnStartEventHandler(OnDropStartEventHandler eventHandler)
    {
        m_OnStartHandler = eventHandler;
        m_dragdrop.onStart.Clear();
        EventDelegate.Add(m_dragdrop.onStart, OnStart);
    }

    public OnDropStartEventHandler GetStartEventHandler()
    {
        return m_OnStartHandler;
    }

    public OnDropReleaseEventHandler GetReleaseEventHandler()
    {
        return m_OnFinishHandler;
    }


    void OnFinish()
    {
        if (m_OnFinishHandler != null)
        {
            m_OnFinishHandler(gameObject, UICamera.hoveredObject);
        }
    }

    new void OnStart()
    {
        if (m_OnStartHandler != null)
        {
            m_OnStartHandler(gameObject);
        }
    }

    public UIDragDropItem m_dragdrop;

    public OnDropReleaseEventHandler m_OnFinishHandler = null;
    public OnDropStartEventHandler m_OnStartHandler = null;


}
