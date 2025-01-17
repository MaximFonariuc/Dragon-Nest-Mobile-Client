using UILib;
using UnityEngine;
using System.Collections.Generic;

public class XUIPopupList : XUIObject, IXUIPopupList
{
   
    protected override void OnAwake()
    {
		base.OnAwake();
        m_uiPopupList = GetComponent<UIPopupList>();
        if (null == m_uiPopupList)
        {
            Debug.LogError("null == m_uiPopupList");
        }
    }

    public void SetOptionList(List<string> options)
    {
        m_uiPopupList.items = options;
    }

    public string value
    {
        get { return m_uiPopupList.value; }
        set { m_uiPopupList.value = value; }
    }

    public int currentIndex
    {
        get 
        {
            return m_uiPopupList.items.IndexOf(m_uiPopupList.value);
        }
        set
        {
            if(value >= m_uiPopupList.items.Count)
            {
                Debug.LogError("Index out of range. " + value);
                return;
            }
            m_uiPopupList.value = m_uiPopupList.items[value];
        }
    }
    private UIPopupList m_uiPopupList;
}

