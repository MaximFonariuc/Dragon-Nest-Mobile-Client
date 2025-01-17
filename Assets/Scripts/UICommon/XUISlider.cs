using UILib;
using UnityEngine;

public class XUISlider : XUIObject, IXUISlider
{
	public float Value
	{
		get
		{
            return m_uiSlider.value;
		}
		set
		{
            m_uiSlider.value = value;
		}
	}

    protected override void OnAwake()
	{ 
		base.OnAwake();
		m_uiSlider = GetComponent<UISlider>();
		if (null == m_uiSlider)
		{
			Debug.LogError("null == m_uiSlider");
		}
	}
	
	public void RegisterValueChangeEventHandler(SliderValueChangeEventHandler eventHandler)
	{
        m_uiSlider.eventHandler = eventHandler;
	}

	public void RegisterClickEventHandler(SliderClickEventHandler eventHandler)
	{
		UIEventListener.Get(this.gameObject).onClick = OnSliderClick;
		m_clickedEventHandler = eventHandler;
	}

	void OnSliderClick(GameObject slider)
	{
		if (null != m_clickedEventHandler)
		{
			m_clickedEventHandler(slider);
		}
	}

	//private SliderValueChangeEventHandler m_valueChangeEventHandler = null;
	private SliderClickEventHandler m_clickedEventHandler = null;

	private UISlider m_uiSlider = null;
}

