//----------------------------------------------
//            NGUI: Next-Gen UI kit
// Copyright © 2011-2014 Tasharen Entertainment
//----------------------------------------------

using UnityEngine;
using AnimationOrTween;
using System.Collections.Generic;

/// <summary>
/// Simple toggle functionality.
/// </summary>

[ExecuteInEditMode]
[AddComponentMenu("NGUI/Interaction/Toggle")]
public class UIToggle : UIWidgetContainer
{
	/// <summary>
	/// List of all the active toggles currently in the scene.
	/// </summary>

	static public BetterList<UIToggle> list = new BetterList<UIToggle>();

	/// <summary>
	/// Current toggle that sent a state change notification.
	/// </summary>

	static public UIToggle current;

	/// <summary>
	/// If set to anything other than '0', all active toggles in this group will behave as radio buttons.
	/// </summary>

	public int group = 0;

	/// <summary>
	/// Sprite that's visible when the 'isActive' status is 'true'.
	/// </summary>

	public UIWidget activeSprite;
    public UIWidget activeSprite2;

	/// <summary>
	/// Animation to play on the active sprite, if any.
	/// </summary>

	public Animation activeAnimation;

	/// <summary>
	/// Whether the toggle starts checked.
	/// </summary>

	public bool startsActive = false;

	/// <summary>
	/// If checked, tween-based transition will be instant instead.
	/// </summary>

	public bool instantTween = false;

	/// <summary>
	/// Can the radio button option be 'none'?
	/// </summary>

	public bool optionCanBeNone = false;

	/// <summary>
	/// Callbacks triggered when the toggle's state changes.
	/// </summary>

	public List<EventDelegate> onChange = new List<EventDelegate>();

	/// <summary>
	/// Deprecated functionality. Use the 'group' option instead.
	/// </summary>

	[HideInInspector][SerializeField] UISprite checkSprite = null;
    [HideInInspector][SerializeField] UISprite checkSprite2 = null;
	[HideInInspector][SerializeField] Animation checkAnimation;
	[HideInInspector][SerializeField] GameObject eventReceiver;
	[HideInInspector][SerializeField] string functionName = "OnActivate";
	[HideInInspector][SerializeField] bool startsChecked = false; // Use 'startsActive' instead

	bool mIsActive = false;
	bool mStarted = false;

	/// <summary>
	/// Whether the toggle is checked.
	/// </summary>

	public bool value
	{
		get { return mIsActive; }
		set { if (group == 0 || value || optionCanBeNone || !mStarted) Set(value); }
	}

	[System.Obsolete("Use 'value' instead")]
	public bool isChecked { get { return value; } set { this.value = value; } }

	/// <summary>
	/// Return the first active toggle within the specified group.
	/// </summary>

	static public UIToggle GetActiveToggle (int group)
	{
		for (int i = 0; i < list.size; ++i)
		{
			UIToggle toggle = list[i];
			if (toggle != null && toggle.group == group && toggle.mIsActive)
				return toggle;
		}
		return null;
	}

	void OnEnable () { list.Add(this); }
	void OnDisable () { list.Remove(this); }

	/// <summary>
	/// Activate the initial state.
	/// </summary>

	void Start ()
	{
		if (startsChecked)
		{
			startsChecked = false;
			startsActive = true;
#if UNITY_EDITOR
			NGUITools.SetDirty(this);
#endif
		}

		// Auto-upgrade
		if (!Application.isPlaying)
		{
			if (checkSprite != null && activeSprite == null)
			{
				activeSprite = checkSprite;
				checkSprite = null;
			}

            if (checkSprite2 != null && activeSprite2 == null)
            {
                activeSprite2 = checkSprite2;
                checkSprite2 = null;
            }

			if (checkAnimation != null && activeAnimation == null)
			{
				activeAnimation = checkAnimation;
				checkAnimation = null;
			}

			if (Application.isPlaying && activeSprite != null)
				activeSprite.alpha = startsActive ? 1f : 0f;

            if (Application.isPlaying && activeSprite2 != null)
                activeSprite2.alpha = startsActive ? 1f : 0f;

			if (EventDelegate.IsValid(onChange))
			{
				eventReceiver = null;
				functionName = null;
			}
		}
		else
		{
			mIsActive = !startsActive;
			mStarted = true;
			bool instant = instantTween;
			instantTween = true;
			Set(startsActive);
			instantTween = instant;
		}
	}

	/// <summary>
	/// Check or uncheck on click.
	/// </summary>

	void OnClick () { if (enabled) value = !value; }

    public void ForceSetActive(bool state)
    {
        Set(state);
    }
	/// <summary>
	/// Fade out or fade in the active sprite and notify the OnChange event listener.
	/// </summary>
	void Set (bool state)
	{
		if (!mStarted)
		{
			mIsActive = state;
			startsActive = state;
			if (activeSprite != null) activeSprite.alpha = state ? 1f : 0f;
            if (activeSprite2 != null) activeSprite2.alpha = state ? 1f : 0f;
		}
		else if (mIsActive != state)
		{
			// Uncheck all other toggles
			if (group != 0 && state)
			{
				for (int i = 0, imax = list.size; i < imax; )
				{
					UIToggle cb = list[i];
					if (cb != this && cb.group == group) cb.Set(false);
					
					if (list.size != imax)
					{
						imax = list.size;
						i = 0;
					}
					else ++i;
				}
			}

			// Remember the state
			mIsActive = state;

			// Tween the color of the active sprite
			if (activeSprite != null)
			{
				if (instantTween)
				{
					activeSprite.alpha = mIsActive ? 1f : 0f;
				}
				else
				{
					TweenAlpha.Begin(activeSprite.gameObject, 0.15f, mIsActive ? 1f : 0f);
				}
			}

            if (activeSprite2 != null)
            {
                if (instantTween)
                {
                    activeSprite2.alpha = mIsActive ? 1f : 0f;
                    //activeSprite2.MakePixelPerfect();
                }
                else
                {
                    TweenAlpha.Begin(activeSprite2.gameObject, 0.15f, mIsActive ? 1f : 0f);
                }
            }

			if (current == null)
			{
				current = this;

				if (EventDelegate.IsValid(onChange))
				{
					EventDelegate.Execute(onChange);
				}
				else if (eventReceiver != null && !string.IsNullOrEmpty(functionName))
				{
					// Legacy functionality support (for backwards compatibility)
					eventReceiver.SendMessage(functionName, mIsActive, SendMessageOptions.DontRequireReceiver);
				}
				current = null;
			}

			// Play the checkmark animation
			if (activeAnimation != null)
			{
				ActiveAnimation aa = ActiveAnimation.Play(activeAnimation, state ? Direction.Forward : Direction.Reverse);
				if (instantTween) aa.Finish();
			}
		}
	}
}
