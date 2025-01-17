using UILib;
using UnityEngine;
using XUtliPoolLib;

public class XUILongPress : XUIObject, IXUILongPress
{
    public void RegisterSpriteLongPressEventHandler(SpriteClickEventHandler eventHandler)
    {
        if (eventHandler != null)
        {
            UIEventListener.Get(this.gameObject).onPress -= OnSpritePress;
            UIEventListener.Get(this.gameObject).onPress += OnSpritePress;
            UIEventListener.Get(this.gameObject).onDrag -= OnSpriteDrag;
            UIEventListener.Get(this.gameObject).onDrag += OnSpriteDrag;
        }

        m_spriteLongPressEventHandler = eventHandler;
    }
	
	protected override void OnAwake()
	{
		base.OnAwake();
        m_XUISprite = GetComponent<XUISprite>();
        if (null == m_XUISprite)
        {
            Debug.LogError("null == XUISprite, " + this.gameObject.name);
        }
    }

    void Update()
    {
        if (m_spriteLongPressEventHandler != null && _lastPress > 0.0f && Time.time - _lastPress > _longClickDuration)
        {
            m_spriteLongPressEventHandler(m_XUISprite);
            _lastPress = -1.0f;
            //bPressed = false;
            m_XUISprite.ClickCanceled = true;
        }
    }

    void OnSpritePress(GameObject button, bool isPressed)
    {
        if (m_spriteLongPressEventHandler != null)
        {
            if (isPressed)
            {
                _lastPress = Time.time;
                //bPressed = true;
            }
            if (!isPressed)
            {
                _lastPress = -1.0f;
            }
        }
    }
    void OnSpriteDrag(GameObject button, Vector2 delta)
    {
        if (m_spriteLongPressEventHandler != null)
        {
            if (_lastPress > 0)
            {
                //bPressed = false;
                _lastPress = -1.0f;
            }
        }
    }

    private SpriteClickEventHandler m_spriteLongPressEventHandler = null;
    private XUISprite m_XUISprite = null;

    private static readonly float _longClickDuration = 0.5f;
    float _lastPress = -1f;
    //bool bPressed = false;
}

