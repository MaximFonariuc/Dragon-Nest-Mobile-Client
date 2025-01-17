using System.Collections.Generic;
using UILib;
using UnityEngine;
using XUtliPoolLib;

public class XUIWrapContent : XUIObject, IXUIWrapContent
{

    protected override void OnAwake()
    {
		base.OnAwake();
        m_uiWrapContent = GetComponent<UIWrapContent>();
        if (null == m_uiWrapContent)
        {
            Debug.LogError("null == m_uiWrapContent");
        }

        m_uiWrapContent.updateHandler = OnItemUpdate;

        if (Tpl == null)
        {
            Debug.LogError("Tpl == null");
        }

        if (!m_uiWrapContent.Init())
        {
            Debug.LogError("m_uiWrapContent.Init Failed");
        }
    }

    protected override void OnStart()
    {
        base.OnStart();

        if (!m_ItemPool.IsValid)
            m_ItemPool.SetupPool(this.gameObject, Tpl, (uint)maxItemCount, false);
    }

    public bool enableBounds
    {
        get
        {
            return m_uiWrapContent.bBounds;
        }
        set
        {
            m_uiWrapContent.bBounds = value;
        }
    }

    public Vector2 itemSize
    {
        get
        {
            return m_uiWrapContent.itemSize;
        }
        set
        {
            m_uiWrapContent.itemSize = value;
        }
    }

    public int widthDimension
    {
        get
        {
            return m_uiWrapContent.WidthDimension;
        }
        set
        {
            m_uiWrapContent.WidthDimension = value;
        }
    }

    public int heightDimensionMax 
    {
        get
        {
            return m_uiWrapContent.HeightDimemsionMax;
        }
    }
    public int maxItemCount
    {
        get
        {
            return (m_uiWrapContent.HeightDimemsionMax + 2) * m_uiWrapContent.WidthDimension;
        }
    }

    //public void SetBounds(int lowerBound, int upperBound)
    //{
    //    m_uiWrapContent.lowerBound = lowerBound;
    //    m_uiWrapContent.upperBound = upperBound;
    //    m_uiWrapContent.bBounds = true;
    //    m_uiWrapContent.AdjustContent();
    //}

    protected void _InitContent(int count, bool fadeIn = false)
    {
        int goCount = Mathf.Min(count, maxItemCount);

        if(!m_ItemPool.IsValid)
            m_ItemPool.SetupPool(this.gameObject, Tpl, (uint)maxItemCount, false);

        if (fadeIn || goCount != m_ItemPool.ActiveCount)
        {
            BetterList<Transform> itemList = m_uiWrapContent.ItemList;
            itemList.Clear();
            if (fadeIn)
                m_ItemPool.ReturnAll();
            else
                m_ItemPool.FakeReturnAll();
            for (int i = 0; i < goCount; ++i)
            {
                GameObject go = m_ItemPool.FetchGameObject(fadeIn);
                itemList.Add(go.transform);
                OnItemInit(go.transform, i);
            }
            if(!fadeIn)
                m_ItemPool.ActualReturnAll();

            m_uiWrapContent.SetChildPositionOffset(0);
        }
    }

    private int _GetRowCount(int count, int columnCount)
    {
        if (count <= 0)
            return 0;
        return ((count - 1) / columnCount + 1);
    }

    public void SetContentCount(int num, bool fadeIn = false)
    {
        if (m_uiWrapContent.WidthDimension != 1)
        {
            num = _GetRowCount(num, m_uiWrapContent.WidthDimension) * m_uiWrapContent.WidthDimension;
        }

        m_uiWrapContent.SetContentCount(num);
        _InitContent(num, fadeIn);
        m_uiWrapContent.AdjustContent();
    }

    private void OnItemUpdate(Transform item, int index)
    {
        if (updateHandler != null)
            updateHandler(item, index);
    }

    private void OnItemInit(Transform item, int index)
    {
        if (initHandler != null)
            initHandler(item, index);
    }

    public void RegisterItemUpdateEventHandler(WrapItemUpdateEventHandler eventHandler)
    {
        updateHandler = eventHandler;
    }

    public void RegisterItemInitEventHandler(WrapItemInitEventHandler eventHandler)
    {
        initHandler = eventHandler;
    }

    public void ResetPosition()
    {
        SetOffset(0);
    }

    public void SetOffset(int offset)
    {
        m_uiWrapContent.SetChildPositionOffset(offset);
        m_uiWrapContent.WrapContent();
    }

    public void InitContent()
    {
        m_uiWrapContent.Init();
    }

    public void RefreshAllVisibleContents()
    {
        m_uiWrapContent.RefreshAllChildrenContent();
    }

    public void GetActiveList(List<GameObject> ret)
    {
        m_ItemPool.GetActiveList(ret);
    }


    private UIWrapContent m_uiWrapContent = null;
    private WrapItemUpdateEventHandler updateHandler;
    private WrapItemInitEventHandler initHandler;
    private XUtliPoolLib.XUIPool m_ItemPool = new XUtliPoolLib.XUIPool(null);

    public GameObject Tpl;
}

