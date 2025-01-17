using UILib;
using UnityEngine;

public class XUIInput : XUIObject, IXUIInput
{
    protected override void OnAwake()
    {
		base.OnAwake();
        m_uiInput = GetComponent<UIInput>();
        if (null == m_uiInput)
        {
            Debug.LogError("null == m_uiInput");
        }
    }

    public void selected(bool value)
    {
        m_uiInput.isSelected = value;
    }
    public string GetText()
    {
        if (null != m_uiInput)
        {
            return m_uiInput.value;
        }
        return "";
    }

    public void SetText(string strText)
    {
        if (null != m_uiInput)
        {
            m_uiInput.value = strText;
        }
    }


    public void SetDefault(string strText)
    {
        if(null != m_uiInput)
        {
            m_uiInput.defaultText = strText;
        }
    }

    public string GetDefault()
    {
        if (null != m_uiInput)
        {
            return m_uiInput.defaultText;
        }

        return "";
    }

    public void RegisterKeyTriggeredEventHandler(InputKeyTriggeredEventHandler eventHandler)
    {
        //UIEventListener.Get(this.gameObject).onKey = OnKeyHehe;
        EventDelegate.Add(m_uiInput.onKeyTriggered, OnKeyTriggered);
        
        m_keyTriggerEventHandler = eventHandler;
    }

    public void OnKeyTriggered()
    {
        if (m_keyTriggerEventHandler != null)
            m_keyTriggerEventHandler(this, UIInput.current.recentKey);
    }

    public void RegisterSubmitEventHandler(InputSubmitEventHandler eventHandler)
    {
        //UIEventListener.Get(this.gameObject).onKey = OnKeyHehe;
        EventDelegate.Add(m_uiInput.onSubmit, OnSubmit);

        m_submitEventHandler = eventHandler;
    }

    public void OnSubmit()
    {
        if (m_submitEventHandler != null)
            m_submitEventHandler(this);
    }

    public void RegisterChangeEventHandler(InputChangeEventHandler eventHandler)
    {
    	EventDelegate.Add(m_uiInput.onChange, OnChange);

        m_changeEventHandler = eventHandler;
    }

    public void OnChange()
    {
    	if(m_changeEventHandler != null)
    		m_changeEventHandler(this);
    }

    public void SetCharacterLimit(int num)
    {
        m_uiInput.characterLimit = num;
    }
    UIInput m_uiInput = null;
    InputKeyTriggeredEventHandler m_keyTriggerEventHandler;
    InputSubmitEventHandler m_submitEventHandler;
    InputChangeEventHandler m_changeEventHandler;
}

