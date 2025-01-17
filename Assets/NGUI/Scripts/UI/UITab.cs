using UnityEngine;
using System.Collections;


[AddComponentMenu("NGUI/UI/Tab")]
public class UITab : MonoBehaviour
{
    public UIWidget[] tabSprite;

    public bool startsChecked = true;

    public Transform tabRoot;

    public bool mChecked = false;
    //bool mStarted = false;

    public delegate void OnTabClick();
    public OnTabClick onTabClick;
    public GameObject eventReceiver;
    public string functionName = "OnTabClick";

    public bool isChecked
    {
        get { return mChecked; }
        set { mChecked = value; Set(value); }
    }

    void Awake()
    {
        foreach (var item in tabSprite)
        {
            if (item != null)
            {
                item.alpha = startsChecked ? 1f : 0f;
            }
        }
        if (tabRoot == null) tabRoot = transform.parent;
    }


    void OnClick()
    {
        if (mChecked) return;
        if (enabled)
        {
            isChecked = !isChecked;
            DispatcherEvents();
        }
    }


    public void Set(bool state)
    {
        if (tabRoot != null && state)
        {
            UITab[] cbs = tabRoot.GetComponentsInChildren<UITab>(true);
            for (int i = 0, imax = cbs.Length; i < imax; ++i)
            {
                UITab cb = cbs[i];
                if (cb != this && cb.tabRoot == tabRoot)
                {
                    cb.Set(false);
                    cb.mChecked = false;
                }
            }
        }
        mChecked = state;
        for(int i=0;i<tabSprite.Length;i++)
        //foreach (var item in tabSprite)
        {
            if (tabSprite[i] != null)
            {
                TweenAlpha.Begin(tabSprite[i].gameObject, 0.15f, mChecked ? 1f : 0f);
            }
        }
        DispatcherEvents();
    }



    private void DispatcherEvents()
    {
        if (eventReceiver != null && !string.IsNullOrEmpty(functionName) && Application.isPlaying)
        {
            eventReceiver.SendMessage(functionName, SendMessageOptions.DontRequireReceiver);
        }
        if (onTabClick != null) onTabClick();
    }
}


