//----------------------------------------------
//            NGUI: Next-Gen UI kit
// Copyright © 2011-2014 Tasharen Entertainment
//----------------------------------------------

using UnityEngine;

/// <summary>
/// This script makes it possible for a scroll view to wrap its content, creating endless scroll views.
/// Usage: simply attach this script underneath your scroll view where you would normally place a UIGrid:
/// 
/// + Scroll View
/// |- UIWrappedContent
/// |-- Item 1
/// |-- Item 2
/// |-- Item 3
/// </summary>
public delegate void WrapContentItemUpdateEventHandler(Transform itemTransform, int index);

[AddComponentMenu("NGUI/Interaction/Wrap Content")]
public class UIWrapContent : MonoBehaviour
{
    /// <summary>
    /// Width or height of the child items for positioning purposes.
    /// </summary>

    //public int itemSize = 100;
    public Vector2 itemSize = new Vector2(100, 100);

    public bool cullContent = true;

    // 区别于滚动方向的宽度
    public int WidthDimension = 2;
    /// <summary>
    /// Whether the content will be automatically culled. Enabling this will improve performance in scroll views that contain a lot of items.
    /// </summary>
    public bool bBounds = true;

    private float lowerBound = 0;
    private float upperBound = 1600;

    private float lowerBoundWithEpsilon = 0;
    private float upperBoundWithEpsilon = 1600;

    private int _ContentCount;     // item的总数量

    public int HeightDimemsionMax = 10;

    public WrapContentItemUpdateEventHandler updateHandler;
    Transform mTrans;
    UIPanel mPanel;
    UIScrollView mScroll;
    UIWidget mPlaceHolder;
    bool mHorizontal = false;
    BetterList<Transform> mChildren = new BetterList<Transform>();
    public BetterList<Transform> ItemList { get { return mChildren; } }

    /// <summary>
    /// Initialize everything and register a callback with the UIPanel to be notified when the clipping region moves.
    /// </summary>

    protected virtual void Start()
    {
        //SortBasedOnScrollMovement();
        //WrapContent();

        //if (mScroll != null)
        //{
        //    mScroll.GetComponent<UIPanel>().onClipMove = OnMove;
        //    //mScroll.restrictWithinPanel = false;
        //    if (mScroll.dragEffect == UIScrollView.DragEffect.MomentumAndSpring)
        //        mScroll.dragEffect = UIScrollView.DragEffect.Momentum;
        //}
    }

    /// <summary>
    /// Callback triggered by the UIPanel when its clipping region moves (for example when it's being scrolled).
    /// </summary>

    protected virtual void OnMove(UIPanel panel) { WrapContent(); }

    /// <summary>
    /// Immediately reposition all children.
    /// </summary>

    [ContextMenu("Sort Based on Scroll Movement")]
    public void SortBasedOnScrollMovement()
    {
        if (!CacheScrollView()) return;

        // Cache all children and place them in order
        mChildren.Clear();
        for (int i = 0; i < mTrans.childCount; ++i)
            mChildren.Add(mTrans.GetChild(i));

        // Sort the list of children so that they are in order
        if (mHorizontal) mChildren.Sort(UIGrid.SortHorizontal);
        else mChildren.Sort(UIGrid.SortVertical);
        ResetChildPositions();
    }

    /// <summary>
    /// Immediately reposition all children, sorting them alphabetically.
    /// </summary>

    [ContextMenu("Sort Alphabetically")]
    public void SortAlphabetically()
    {
        if (!CacheScrollView()) return;

        Vector3 pos = gameObject.transform.localPosition;
        if (mHorizontal)
            pos.x = 0;
        else
            pos.y = 0;
        gameObject.transform.localPosition = pos;

        // Cache all children and place them in order
        mChildren.Clear();
        for (int i = 0; i < mTrans.childCount; ++i)
            mChildren.Add(mTrans.GetChild(i));

        // Sort the list of children so that they are in order
        mChildren.Sort(UIGrid.SortByName);
        ResetChildPositions();
    }

    /// <summary>
    /// Cache the scroll view and return 'false' if the scroll view is not found.
    /// </summary>

    protected bool CacheScrollView()
    {
        mTrans = transform;
        mPanel = NGUITools.FindInParents<UIPanel>(gameObject);
        mScroll = mPanel.GetComponent<UIScrollView>();
        if (mScroll == null) return false;
        if (mScroll.movement == UIScrollView.Movement.Horizontal) mHorizontal = true;
        else if (mScroll.movement == UIScrollView.Movement.Vertical) mHorizontal = false;
        else return false;

        CreatePlaceHolder();

        ToggleClipMove(true);
        //if (mScroll != null)
        //{
        //    UIPanel panel = mScroll.GetComponent<UIPanel>();
        //    panel.onClipMove -= OnMove;
        //    panel.onClipMove += OnMove;
        //    //mScroll.restrictWithinPanel = false;
        //    //if (mScroll.dragEffect == UIScrollView.DragEffect.MomentumAndSpring)
        //    //    mScroll.dragEffect = UIScrollView.DragEffect.Momentum;
        //}

        return true;
    }

    public void OnEnable()
    {
        ToggleClipMove(true);
    }

    public void OnDisable()
    {
        ToggleClipMove(false);
    }

    private void ToggleClipMove(bool bEnable)
    {
        if (mPanel == null)
            return;

        mPanel.onClipMove -= OnMove;
        if (bEnable)
        {
            mPanel.onClipMove += OnMove;
        }
    }

    public void CreatePlaceHolder()
    {
        if (bBounds)
        {
            Transform t = mPanel.transform.Find("PlaceHolder");
            if (t == null)
            {
                mPlaceHolder = NGUITools.AddWidget<UIWidget>(mPanel.gameObject);
                mPlaceHolder.name = "PlaceHolder";
            }
            else
            {
                mPlaceHolder = t.GetComponent<UIWidget>();
            }
            if (mPlaceHolder == null)
                return;
            mPlaceHolder.width = (int)itemSize.x;
            mPlaceHolder.height = (int)itemSize.y;
            if (mHorizontal)
            {
                mPlaceHolder.pivot = UIWidget.Pivot.Left;
                mPlaceHolder.transform.localPosition = new Vector3(-itemSize.x / 2, 0, 0);
            }
            else
            {
                mPlaceHolder.pivot = UIWidget.Pivot.Top;
                mPlaceHolder.transform.localPosition = new Vector3(0, itemSize.y / 2, 0);
            }
            UpdatePlaceHolder();
        }

    }

    public void UpdatePlaceHolder()
    {
        if (bBounds)
        {
            if (mHorizontal)
            {
                lowerBound = 0;
                lowerBoundWithEpsilon = lowerBound - (itemSize.x / 2);

                if (_ContentCount <= 0)
                    upperBound = 0;
                else
                    upperBound = ((_ContentCount - 1) / WidthDimension) * (int)(itemSize.x);
                upperBoundWithEpsilon = upperBound + itemSize.x / 2;

                mPlaceHolder.width = (int)upperBound + (int)(itemSize.x);
            }
            else
            {
                if (_ContentCount <= 0)
                    lowerBound = 0;
                else
                    lowerBound = -((_ContentCount - 1) / WidthDimension) * (int)(itemSize.y);
                lowerBoundWithEpsilon = lowerBound - (itemSize.y / 2);

                upperBound = 0;
                upperBoundWithEpsilon = upperBound + itemSize.y / 2;

                mPlaceHolder.height = -(int)lowerBound + (int)(itemSize.y);
            }
        }
    }
    public bool Init()
    {
        if (!CacheScrollView())
            return false;
        Vector3 pos = gameObject.transform.localPosition;
        if (mHorizontal)
            pos.x = 0;
        else
            pos.y = 0;
        gameObject.transform.localPosition = pos;

        //ResetChildPositions();
        return true;
    }
    /// <summary>
    /// Helper function that resets the position of all the children.
    /// </summary>

    void ResetChildPositions()
    {
        //for (int i = 0; i < mChildren.size; ++i)
        //{
        //    int h = i / WidthDimension;
        //    int w = i % WidthDimension;
        //    Transform t = mChildren[i];
        //    //t.localPosition = mHorizontal ? new Vector3(h * itemSize.x, -w * itemSize.y, 0f) : new Vector3(h * itemSize.x, -w * itemSize.y, 0f);
        //    t.localPosition = new Vector3(h * itemSize.x, -w * itemSize.y, 0f);
        //}
        SetChildPositionOffset();
    }

    public void SetContentCount(int count)
    {
        _ContentCount = count;
        UpdatePlaceHolder();
    }
    public void AdjustContent()
    {
        float scrollLength = mHorizontal ? mPanel.finalClipRegion.z : mPanel.finalClipRegion.w;
        float scrollCenter = mHorizontal ? mPanel.baseClipRegion.x : mPanel.baseClipRegion.y; 
        float panelLength;
        float singleItemLength;
        float totalContentLength;
        float panelStartOffset;
        if (mHorizontal)
        {
            singleItemLength = itemSize.x;
            panelStartOffset = scrollLength / 2 + scrollCenter - singleItemLength / 2 - mPanel.clipSoftness.x;
            panelLength = mPanel.clipOffset.x + scrollLength - panelStartOffset;
            totalContentLength = Mathf.Abs(upperBound) + singleItemLength;
        }
        else
        {
            singleItemLength = itemSize.y;
            panelStartOffset = scrollLength / 2 + scrollCenter - singleItemLength / 2 + mPanel.clipSoftness.y;
            panelLength = mPanel.clipOffset.y - scrollLength + panelStartOffset;
            totalContentLength = Mathf.Abs(lowerBound) + singleItemLength;
        }

        if (scrollLength < totalContentLength)
        {
            // content少于滚动条滚到的距离，滚动条需要回滚
            if (Mathf.Abs(panelLength) > totalContentLength)
            {
                //int onePageContentCount = (int)(scrollLength / singleItemLength);
                SetChildPositionOffset(_ContentCount - mChildren.size);
                mScroll.NeedRecalcBounds();
                mScroll.RestrictWithinBounds(true, mHorizontal, !mHorizontal);
                //Vector2 pv = NGUIMath.GetPivotOffset(mScroll.contentPivot);
                //mScroll.SetDragAmount(pv.x, 1, false);
                //mScroll.SetDragAmount(pv.x, 1, true);
            }
            else
            {
                RefreshAllChildrenContent();
            }
        }
        else
        {
            SetChildPositionOffset(0);
            //Vector2 pv = NGUIMath.GetPivotOffset(mScroll.contentPivot);
            //mScroll.SetDragAmount(pv.x, 0, false);
            //mScroll.SetDragAmount(pv.x, 0, true);
            mScroll.ResetPosition();
        }
        WrapContent();
    }

    public void SetChildPositionOffset(int offset = 0, bool bUpdate = true)
    {
        for (int i = 0; i < mChildren.size; ++i)
        {
            int offI = offset + i;
            int h = offI / WidthDimension;
            int w = offI % WidthDimension;
            Transform t = mChildren[i];
            //t.localPosition = mHorizontal ? new Vector3(h * itemSize.x, -w * itemSize.y, 0f) : new Vector3(h * itemSize.x, -w * itemSize.y, 0f);
            t.localPosition = mHorizontal
                ? new Vector3(h * itemSize.x, -w * itemSize.y, 0f)
                : new Vector3(w * itemSize.x, -h * itemSize.y, 0f);
            if (bUpdate)
                UpdateItem(t, i);
        }
    }

    public void RefreshAllChildrenContent()
    {
        for (int i = 0; i < mChildren.size; ++i)
        {
            UpdateItem(mChildren[i], i);
        }
    }
    /// <summary>
    /// Wrap all content, repositioning all children as needed.
    /// </summary>

    public void WrapContent()
    {
        float extents = (mHorizontal ? itemSize.x : itemSize.y) * (mChildren.size / WidthDimension) * 0.5f;
        extents = Mathf.Max(extents, 1);
        float extentsX2 = extents * 2;
        float extentsWithEpsilon = (mHorizontal ? itemSize.x : itemSize.y) / 2 + extents;

        Vector3 delta = mHorizontal ? new Vector3(extents * 2f, 0f, 0f) : new Vector3(0f, extents * 2f, 0f);
        Vector3[] corners = mPanel.worldCorners;

        for (int i = 0; i < 4; ++i)
        {
            Vector3 v = corners[i];
            v = mTrans.InverseTransformPoint(v);
            corners[i] = v;
        }
        Vector3 center = Vector3.Lerp(corners[0], corners[2], 0.5f);

        int times = 1;
        bool bShouldUpdate = false;
        bool bShouldShow = true;
        if (mHorizontal)
        {
            float min = corners[0].x - center.x - itemSize.x;
            float max = corners[2].x - center.x + itemSize.x;

            for (int i = 0; i < mChildren.size; ++i, bShouldUpdate = false, bShouldShow = true)
            {
                Transform t = mChildren[i];
                float distance = t.localPosition.x - center.x;
                if (distance < -extentsWithEpsilon)
                {
                    times = (int)(-extents - distance) / (int)(extentsX2) + 1;
                    if (!bBounds || (t.localPosition.x + extentsX2 * times <= upperBoundWithEpsilon))
                    {
                        t.localPosition += delta * times;
                        bShouldUpdate = true;
                        //UpdateItem(t, i);
                        distance = t.localPosition.x - center.x;
                    }
                }
                else if (distance > extentsWithEpsilon)
                {
                    times = (int)(distance - extents) / (int)(extentsX2) + 1;
                    if (!bBounds || (t.localPosition.x - extentsX2 * times >= lowerBoundWithEpsilon))
                    {
                        t.localPosition -= delta * times;
                        bShouldUpdate = true;
                        //UpdateItem(t, i);
                        distance = t.localPosition.x - center.x;
                    }
                }

                if (bBounds)
                {
                    if (t.localPosition.x > upperBoundWithEpsilon)
                    {
                        times = (int)(t.localPosition.x - upperBound) / (int)(extentsX2) + 1;
                        t.localPosition -= delta * times;
                        bShouldUpdate = t.localPosition.x >= lowerBoundWithEpsilon;
                        bShouldShow = bShouldUpdate;
                        //UpdateItem(t, i);
                        distance = t.localPosition.x - center.x;
                    }
                    else if (t.localPosition.x < lowerBoundWithEpsilon)
                    {
                        times = (int)(lowerBound - t.localPosition.x) / (int)(extentsX2) + 1;
                        t.localPosition += delta * times;
                        bShouldUpdate = t.localPosition.x <= upperBoundWithEpsilon;
                        bShouldShow = bShouldUpdate;
                        //UpdateItem(t, i);
                        distance = t.localPosition.x - center.x;
                    }
                }

                if (cullContent)
                {

                    distance += /*mPanel.clipOffset.x*/ - mTrans.localPosition.x;
                    if (!UICamera.IsPressed(t.gameObject))
                    {
                        if (bShouldUpdate)
                        {
                            if (!t.gameObject.activeSelf)
                                NGUITools.SetActive(t.gameObject, true, false,true);
                            UpdateItem(t, i);
                            bShouldUpdate = false;
                        }
                        NGUITools.SetActive(t.gameObject, bShouldShow && (distance > min && distance < max), false,true);
                    }
                }
                
                if (bShouldUpdate)
                    UpdateItem(t, i);
            }
        }
        else
        {
            float min = corners[0].y - center.y - itemSize.y;
            float max = corners[2].y  - center.y + itemSize.y;

            for (int i = 0; i < mChildren.size; ++i, bShouldUpdate = false, bShouldShow = true)
            {
                Transform t = mChildren[i];
                float distance = t.localPosition.y - center.y;

                if (distance < -extentsWithEpsilon)
                {
                    times = (int)(-extents - distance) / (int)(extentsX2) + 1;
                    if (!bBounds || (t.localPosition.y + extentsX2 * times <= upperBoundWithEpsilon))
                    {
                        t.localPosition += delta * times;
                        bShouldUpdate = true;
                        //UpdateItem(t, i);
                        distance = t.localPosition.y - center.y;
                    }
                }
                else if (distance > extentsWithEpsilon)
                {
                    times = (int)(distance - extents) / (int)(extentsX2) + 1;
                    if (!bBounds || (t.localPosition.y - extentsX2 * times >= lowerBoundWithEpsilon))
                    {
                        t.localPosition -= delta * times;
                        bShouldUpdate = true;
                        //UpdateItem(t, i);
                        distance = t.localPosition.y - center.y;
                    }
                }

                if (bBounds)
                {
                    if (t.localPosition.y > upperBoundWithEpsilon)
                    {
                        times = (int)(t.localPosition.y - upperBound) / (int)(extentsX2) + 1;
                        t.localPosition -= delta * times;
                        bShouldUpdate = t.localPosition.y >= lowerBoundWithEpsilon;
                        bShouldShow = bShouldUpdate;
                        //UpdateItem(t, i);
                        distance = t.localPosition.y - center.y;
                    }
                    else if (t.localPosition.y < lowerBoundWithEpsilon)
                    {
                        times = (int)(lowerBound - t.localPosition.y) / (int)(extentsX2) + 1;
                        t.localPosition += delta * times;
                        bShouldUpdate = t.localPosition.y <= upperBoundWithEpsilon;
                        bShouldShow = bShouldUpdate;
                        //UpdateItem(t, i);
                        distance = t.localPosition.y - center.y;
                    }
                }

                if (cullContent)
                {
                    distance += /*mPanel.clipOffset.y*/ - mTrans.localPosition.y;
                    if (!UICamera.IsPressed(t.gameObject))
                    {
                        if (bShouldUpdate)
                        {
                            if (!t.gameObject.activeSelf)
                                NGUITools.SetActive(t.gameObject, true, false, true);
                            UpdateItem(t, i);
                            bShouldUpdate = false;
                        }
                        else
                            NGUITools.SetActive(t.gameObject, bShouldShow && (distance > min && distance < max), false, true);
                    }
                }

                if (bShouldUpdate)
                    UpdateItem(t, i);
            }
        }
    }

    /// <summary>
    /// Want to update the content of items as they are scrolled? Override this function.
    /// </summary>

    protected virtual void UpdateItem(Transform item, int index)
    {
        //Debug.Log("item updated " + index);
        int itemIndex;
        if(mHorizontal)
            itemIndex = (int)(item.localPosition.x / itemSize.x + 0.5f) * WidthDimension + (int)(-item.localPosition.y / itemSize.y + 0.5f);
        else
            itemIndex = (int)(-item.localPosition.y / itemSize.y + 0.5f) * WidthDimension + (int)(item.localPosition.x / itemSize.x + 0.5f);

        if (itemIndex < 0 || itemIndex >= _ContentCount)
        {
            if(item.gameObject.activeSelf)
                NGUITools.SetActive(item.gameObject, false, false, true);
            return;
        }

        if (updateHandler != null)
            updateHandler(item, itemIndex);
    }

}
