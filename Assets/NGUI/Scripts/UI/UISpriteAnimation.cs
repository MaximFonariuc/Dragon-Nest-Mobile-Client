//----------------------------------------------
//            NGUI: Next-Gen UI kit
// Copyright © 2011-2014 Tasharen Entertainment
//----------------------------------------------

using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Very simple sprite animation. Attach to a sprite and specify a common prefix such as "idle" and it will cycle through them.
/// </summary>

[ExecuteInEditMode]
[RequireComponent(typeof(UISprite))]
[AddComponentMenu("NGUI/UI/Sprite Animation")]
public class UISpriteAnimation : MonoBehaviour
{
	[HideInInspector][SerializeField] int mFPS = 30;
    
	[HideInInspector][SerializeField] string mPrefix = "";
	[HideInInspector][SerializeField] bool mLoop = true;
    [HideInInspector][SerializeField] int mInterval = 0;
    [HideInInspector][SerializeField] bool mDisableWhenFinish = true;

	UISprite mSprite;
	float mDelta = 0f;
	int mIndex = 0;
    int mPresentIndex = 0;
	bool mActive = true;
    List<string> mSpriteNames = new List<string>();

    public UISprite sprite { get {
            if (mSprite == null) mSprite = GetComponent<UISprite>();
            return mSprite; } }
    private float mLastLoopFinishTime;

    
    public float LastLoopFinishTime { get { return mLastLoopFinishTime; } set { mLastLoopFinishTime = value; } }

	/// <summary>
	/// Number of frames in the animation.
	/// </summary>

	public int frames { get { return mSpriteNames.Count; } }

	/// <summary>
	/// Animation framerate.
	/// </summary>

	public int framesPerSecond { get { return mFPS; } set { mFPS = value; } }

	/// <summary>
	/// Set the name prefix used to filter sprites from the atlas.
	/// </summary>

	public string namePrefix { get { return mPrefix; } set { if (mPrefix != value) { mPrefix = value; RebuildSpriteList(); } } }

	/// <summary>
	/// Set the animation to be looping or not
	/// </summary>

	public bool loop { get { return mLoop; } set { mLoop = value; } }
    public int loopinterval { get { return mInterval; } set { mInterval = value; } }

    public bool disableWhenFinish { get { return mDisableWhenFinish; } set { mDisableWhenFinish = value; } }

	/// <summary>
	/// Returns is the animation is still playing or not
	/// </summary>

	public bool isPlaying { get { return mActive; } }


    private SpriteAnimationFinishEventHandler finishHandler = null;
    public SpriteAnimationFinishEventHandler FinishHandler { set { finishHandler = value; } }
    /// <summary>
	/// Rebuild the sprite list first thing.
	/// </summary>
    
	void Start () { RebuildSpriteList(); }

	/// <summary>
	/// Advance the sprite animation process.
	/// </summary>

	void Update ()
	{
		if (mActive && mSpriteNames.Count > 1 && Application.isPlaying && mFPS > 0f)
		{
			mDelta += RealTime.deltaTime;
			float rate = 1f / mFPS;

			if (rate < mDelta)
			{
                mDelta = (rate > 0f) ? mDelta - rate : 0f;

                if (loopinterval > 0 && RealTime.time - mLastLoopFinishTime < loopinterval - rate) return;

                if (mPresentIndex != mIndex)
                    mPresentIndex = mIndex;
                else if (++mIndex >= mSpriteNames.Count)
                {
                    mIndex = 0;
                    if (loopinterval == 0)
                        mPresentIndex = 0;
                    mActive = loop;
                    mLastLoopFinishTime = RealTime.time;

                    if (loop == false)
                    {
                        if (disableWhenFinish)
                        {
                            gameObject.SetActive(false);
                        }
                        if (finishHandler != null)
                        {
                            finishHandler();
                        }
                    }

                }
                else
                    ++mPresentIndex;

			    

				if (mActive)
				{
                    mSprite.spriteName = mSpriteNames[mPresentIndex];
				    //mSprite.type = UISprite.Type.Simple;
					//mSprite.MakePixelPerfect();
				}
			}
		}
	}

	/// <summary>
	/// Rebuild the sprite list after changing the sprite name.
	/// </summary>

	void RebuildSpriteList ()
	{
		if (mSprite == null) mSprite = GetComponent<UISprite>();
		mSpriteNames.Clear();

		if (mSprite != null && mSprite.atlas != null)
		{
			List<UISpriteData> sprites = mSprite.atlas.spriteList;

			for (int i = 0, imax = sprites.Count; i < imax; ++i)
			{
				UISpriteData sprite = sprites[i];

				if (string.IsNullOrEmpty(mPrefix) || sprite.name.StartsWith(mPrefix))
				{
					mSpriteNames.Add(sprite.name);
				}
			}
			mSpriteNames.Sort();
		}
	}
	
	/// <summary>
	/// Reset the animation to frame 0 and activate it.
	/// </summary>
	
	public void Reset()
	{
		mActive = true;
		mIndex = 0;
        mPresentIndex = 0;

		if (mSprite != null && mSpriteNames.Count > 0)
		{
			mSprite.spriteName = mSpriteNames[mIndex];
			//mSprite.MakePixelPerfect();
		}
	}

    public void StopAndReset()
    {
		mActive = false;
		mIndex = 0;
        mPresentIndex = 0;

		if (mSprite != null && mSpriteNames.Count > 0)
		{
			mSprite.spriteName = mSpriteNames[mIndex];
			//mSprite.MakePixelPerfect();
		}
    }
}
