//----------------------------------------------
//            NGUI: Next-Gen UI kit
// Copyright © 2011-2014 Tasharen Entertainment
//----------------------------------------------

using UnityEngine;

/// <summary>
/// Simple example script of how a button can be colored when the mouse hovers over it or it gets pressed.
/// </summary>

[ExecuteInEditMode]
[AddComponentMenu("NGUI/Interaction/Button Color")]
public class UIButtonColor : UIWidgetContainer
{
	public enum State
	{
		Normal,
		Hover,
		Pressed,
		Disabled,
	}

	/// <summary>
	/// Target with a widget, renderer, or light that will have its color tweened.
	/// </summary>

	public GameObject tweenTarget;

	/// <summary>
	/// Color to apply on hover event (mouse only).
	/// </summary>

	public Color hover = new Color(225f / 255f, 200f / 255f, 150f / 255f, 1f);

	/// <summary>
	/// Color to apply on the pressed event.
	/// </summary>

	public Color pressed = new Color(183f / 255f, 163f / 255f, 123f / 255f, 1f);

	/// <summary>
	/// Color that will be applied when the button is disabled.
	/// </summary>

	public Color disabledColor = Color.grey;

	/// <summary>
	/// Duration of the tween process.
	/// </summary>

    public bool changeStateSprite = false;

	public float duration = 0.2f;

	protected Color mColor;
	protected bool mInitDone = false;
	protected UIWidget mWidget;
	protected State mState = State.Normal;
    [System.NonSerialized]
    TweenColor tc;
    
	/// <summary>
	/// Button's current state.
	/// </summary>

	public State state { get { return mState; } set { SetState(value, false); } }

	/// <summary>
	/// UIButtonColor's default (starting) color. It's useful to be able to change it, just in case.
	/// </summary>

	public Color defaultColor
	{
		get
		{
#if UNITY_EDITOR
			if (!Application.isPlaying) return Color.white;
#endif
			if (!mInitDone) OnInit();
			return mColor;
		}
		set
		{
#if UNITY_EDITOR
			if (!Application.isPlaying) return;
#endif
			if (!mInitDone) OnInit();
			mColor = value;
		}
	}

	/// <summary>
	/// Whether the script should be active or not.
	/// </summary>

	public virtual bool isEnabled { get { return enabled; } set { enabled = value; } }

	void Awake () { if (!mInitDone) OnInit(); }

    void Start()
    {
        //if (!isEnabled) SetState(State.Disabled, true);
    }

	protected virtual void OnInit ()
	{
		mInitDone = true;
		if (tweenTarget == null) tweenTarget = gameObject;
		mWidget = tweenTarget.GetComponent<UIWidget>();
        tc = tweenTarget.GetComponent<TweenColor>();
		if (mWidget != null)
		{
			mColor = mWidget.color;
		}
		else
		{
			Renderer ren = tweenTarget.GetComponent<Renderer>();

			if (ren != null)
			{
				mColor = Application.isPlaying ? ren.material.color : ren.sharedMaterial.color;
			}
			else
			{
				Light lt = tweenTarget.GetComponent<Light>();

				if (lt != null)
				{
					mColor = lt.color;
				}
				else
				{
					tweenTarget = null;
					mInitDone = false;
				}
			}
		}
	}

	/// <summary>
	/// Set the initial state.
	/// </summary>

	protected virtual void OnEnable ()
	{
#if UNITY_EDITOR
		if (!Application.isPlaying) return;
#endif
		//if (mInitDone) OnHover(UICamera.IsHighlighted(gameObject));

		if (UICamera.currentTouch != null)
		{
			if (UICamera.currentTouch.pressed == gameObject) OnPress(true);
			//else if (UICamera.currentTouch.current == gameObject) OnHover(true);
		}
	}

	/// <summary>
	/// Reset the initial state.
	/// </summary>

	protected virtual void OnDisable ()
	{
#if UNITY_EDITOR
		if (!Application.isPlaying) return;
#endif
		if (mInitDone && tweenTarget != null)
		{
			SetState(State.Normal, true);

            //TweenColor tc = tweenTarget.GetComponent<TweenColor>();

			if (tc != null)
			{
				tc.value = mColor;
				tc.enabled = false;
			}
		}
	}

	/// <summary>
	/// Set the hover state.
	/// </summary>

	protected virtual void OnHover (bool isOver)
	{
		if (isEnabled)
		{
			if (!mInitDone) OnInit();
			if (tweenTarget != null) SetState(isOver ? State.Hover : State.Normal, false);
		}
	}

	/// <summary>
	/// Set the pressed state.
	/// </summary>

	protected virtual void OnPress (bool isPressed)
	{
		if (isEnabled && UICamera.currentTouch != null)
		{
			if (!mInitDone) OnInit();

			if (tweenTarget != null)
			{
				if (isPressed)
				{
					SetState(State.Pressed, false);
				}
				else if (UICamera.currentTouch.current == gameObject)
				{
					if (UICamera.currentScheme == UICamera.ControlScheme.Controller)
					{
						SetState(State.Hover, false);
					}
					else if (UICamera.currentScheme == UICamera.ControlScheme.Mouse && UICamera.hoveredObject == gameObject)
					{
						SetState(State.Hover, false);
					}
					else SetState(State.Normal, false);
				}
				else SetState(State.Normal, false);
			}
		}
	}

	/// <summary>
	/// Set the pressed state on drag over.
	/// </summary>

	protected virtual void OnDragOver ()
	{
		if (isEnabled)
		{
			if (!mInitDone) OnInit();
			if (tweenTarget != null) SetState(State.Pressed, false);
		}
	}

	/// <summary>
	/// Set the normal state on drag out.
	/// </summary>

	protected virtual void OnDragOut ()
	{
		if (isEnabled)
		{
			if (!mInitDone) OnInit();
			if (tweenTarget != null) SetState(State.Normal, false);
		}
	}

	/// <summary>
	/// Set the selected state.
	/// </summary>

	protected virtual void OnSelect (bool isSelected)
	{
		if (isEnabled && (!isSelected || UICamera.currentScheme == UICamera.ControlScheme.Controller) && tweenTarget != null)
			OnHover(isSelected);
	}

	/// <summary>
	/// Change the visual state.
	/// </summary>

	public virtual void SetState (State state, bool instant)
	{
		if (!mInitDone)
		{
			mInitDone = true;
			OnInit();
		}

		if (mState != state)
		{
			mState = state;

            if (changeStateSprite)
            {
                TweenColor tc;

                switch (mState)
                {
                    case State.Hover: tc = TweenColor.Begin(tweenTarget, duration, hover); break;
                    case State.Pressed: tc = TweenColor.Begin(tweenTarget, duration, pressed); break;
                    case State.Disabled: tc = TweenColor.Begin(tweenTarget, duration, disabledColor); break;
                    default: tc = TweenColor.Begin(tweenTarget, duration, mColor); break;
                }

                if (instant && tc != null)
                {
                    tc.value = tc.to;
                    tc.enabled = false;
                }
            }
		}
	}
}
