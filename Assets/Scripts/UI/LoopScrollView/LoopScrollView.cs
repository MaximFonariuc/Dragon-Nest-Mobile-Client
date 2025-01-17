using UnityEngine;
using System.Collections.Generic;
using XUtliPoolLib;
using System.Collections;

/// <summary>
/// 这个类主要做了一件事,就是优化了,NGUI UIScrollView 在数据量很多都时候,
/// 创建过多都GameObject对象,造成资源浪费.
/// </summary>
public class LoopScrollView : MonoBehaviour, ILoopScrollView
{

    public enum ArrangeDirection
    {
        Left_to_Right,
        Right_to_Left,
        Up_to_Down,
        Down_to_Up,
    }
    /// <summary>
    /// items的排列方式
    /// </summary>
    public ArrangeDirection arrangeDirection = ArrangeDirection.Up_to_Down;

    /// <summary>
    /// 列表单项模板
    /// </summary>
    public GameObject itemPrefab;

    /// <summary>
    /// The items list.
    /// </summary>
    public List<LoopItemObject> itemsList;

    /// <summary>
    /// The datas list.
    /// </summary>
    public List<LoopItemData> datasList;
    public UIScrollView scrollView;
    public GameObject itemParent;

    /// <summary>
    /// itemsList的第一个元素
    /// </summary>
    LoopItemObject firstItem;

    /// <summary>
    /// itemsList的最后一个元素
    /// </summary>
    LoopItemObject lastItem;

    /// <summary>
    /// 第一item的起始位置
    /// </summary>
    public Vector3 itemStartPos = Vector3.zero;

    public DelegateHandler OnItemInit;

    /// <summary>
    /// 菜单项间隙
    /// </summary>
    public float gapDis = 0f;

    private Vector3 m_TopLeft, m_BtmRight;

    private System.Action onDragFinish;

    /// <summary>
    /// 预制可见个数 会影响边缘的回弹spring
    /// </summary>
    public int m_maxViewCnt = 2;

    bool mTop = true;

    // 对象池
    Queue<LoopItemObject> itemLoop = new Queue<LoopItemObject>();

    void Awake()
    {
        if (itemPrefab == null || scrollView == null || itemParent == null)
        {
            Debug.LogError("LoopScrollView.Awake not set value in inspector!");
        }
        if (m_maxViewCnt < 2)
        {
            Debug.LogError("Make sure your view cnt more than 2!");
        }

        if (arrangeDirection == ArrangeDirection.Up_to_Down ||
           arrangeDirection == ArrangeDirection.Down_to_Up)
        {
            scrollView.movement = UIScrollView.Movement.Vertical;
        }
        else
        {
            scrollView.movement = UIScrollView.Movement.Horizontal;
        }
        scrollView.onDragFinished = OnDragFinish;
    }

    void Start()
    {
        UIPanel panel = scrollView.panel;
        Vector4 v4 = scrollView.panel.baseClipRegion;
        Vector3 v3 = scrollView.transform.localPosition;
        v3 = new Vector3(v3.x + panel.clipOffset.x + v4.x, v3.y + panel.clipOffset.y + v4.y, v3.z);

        Vector3 topleft = new Vector3(v3.x - v4.z / 2, v3.y + v4.w / 2, v3.z);
        Vector3 btmright = new Vector3(v3.x + v4.z / 2, v3.y - v4.w / 2, v3.z);

        m_TopLeft = scrollView.transform.parent.localToWorldMatrix.MultiplyPoint(topleft);
        m_BtmRight = scrollView.transform.parent.localToWorldMatrix.MultiplyPoint(btmright);
        //XDebug.singleton.AddGreenLog("topleft =" + topleft + "btmright" + btmright, "m_TopLeft=" + m_TopLeft + "...." + "m_BtmRight" + m_BtmRight);
    }

    private void OnDragFinish()
    {
        if (lastItem != null && IsScrollLast())
        {
            //XDebug.singleton.AddGreenLog("xxxxxxxxxxxxxxxxxxxxxxxxxx");
            if (onDragFinish != null) onDragFinish();
        }
    }

    void FixedUpdate()
    {
        Validate();
        if (frameIndex <= 4)
        {
            frameIndex = frameIndex << 1;
            if (frameIndex == 1 << 2)
            {
                YiledScroll(); //第二帧调用
            }
        }
    }


    private int frameIndex = 1 << 10;
    /// <summary>
    /// Init the specified datas.
    /// </summary>
    /// <param name="datas">Datas.</param>
    public void Init(List<LoopItemData> datas, DelegateHandler onItemInitCallback, System.Action dragFinish, int pivot = 0, bool forceRefreshPerTime = false)
    {
        mTop = pivot == 0;
        onDragFinish = dragFinish;

        if (forceRefreshPerTime)
        {
            RefeshPerTime(datas, onItemInitCallback, pivot);
        }
        else
        {
            if (IsEqual(datas))
            {
                // Debug.Log("equal, do noting!");
            }
            else if (IsAddNew(datas))
            {
                AddItem(datas[datas.Count - 1]);
            }
            else if (UpdateNewOne(datas))
            {
                UpdateItem(datas[datas.Count - 1]);
            }
            else
            {
                RefeshPerTime(datas, onItemInitCallback, pivot);
            }
        }
        frameIndex = 1;
    }

    private void RefeshPerTime(List<LoopItemData> datas,DelegateHandler onItemInitCallback,int pivot)
    {
        mTop = pivot == 0;
        this.OnItemInit = onItemInitCallback;
        Resetloop();
        datasList = datas;
        Validate();
        ResetScroll();
        int cnt = Mathf.Min(m_maxViewCnt, datasList.Count);
        for (int i = 0; i < cnt; i++) Validate();
    }
    
    void YiledScroll()
    {
        ResetScroll();
        CheckBTM();
    }


    private void Resetloop()
    {
        if (itemsList != null)
        {
            for (int i = 0; i < itemsList.Count; i++)
            {
                PutItemToLoop(itemsList[i]);
            }
            itemsList.Clear();
        }
        if (datasList != null) datasList.Clear();
    }

    private bool activable { get { return gameobject.activeSelf; } }

    public void SetPivot(bool istop){ }
    

    public GameObject GetTpl()
    {
        return itemPrefab;
    }

    private bool IsAllInvisible()
    {
        bool all_invisible = true;
        for (int i = 0; i < itemsList.Count; i++)
        {
            if (IsVisible(itemsList[i]))
            {
                all_invisible = false;
                break;
            }
        }
        return all_invisible;
    }

    /// <summary>
    /// 检验items的两端是否要补上或删除
    /// </summary>
    private void Validate()
    {
        if (datasList == null || datasList.Count == 0) return;
        // 如果itemsList还不存在
        if (itemsList == null || itemsList.Count == 0)
        {
            itemsList = new List<LoopItemObject>();
            LoopItemObject item = GetItemFromLoop();
            if (mTop) InitItem(item, 0, datasList[0]);  //默认起始0
            else InitItem(item, datasList.Count - 1, datasList[datasList.Count - 1]);
            firstItem = lastItem = item;
            itemsList.Add(item);
            ResetScroll();
        }
        if (IsAllInvisible()) return;

        // 先判断前端是否要增减
        if (IsVisible(firstItem))
        {
            // 判断要不要在它的前面补充一个item 
            if (firstItem.dataIndex > 0)
            {
                LoopItemObject item = GetItemFromLoop();
                int index = firstItem.dataIndex - 1;
                AddToFront(firstItem, item, index, datasList[index]);
                firstItem = item;
                itemsList.Insert(0, item);
            }
        }
        else
        {
            // 判断要不要将它移除
            // 条件：自身是不可见的; 且它后一个item也是不可见的（或被被裁剪过半的）.隐含条件itemsList.Count>=2.
            if (itemsList.Count > m_maxViewCnt && !IsVisible(itemsList[0]) && !IsVisible(itemsList[1]))
            {
                itemsList.Remove(firstItem);
                PutItemToLoop(firstItem);
                firstItem = itemsList[0];
            }
        }

        // 再判断后端是否要增减
        if (IsVisible(lastItem))
        {
            // 判断要不要在它的后面补充一个item
            if (lastItem.dataIndex < datasList.Count - 1)
            {
                LoopItemObject item = GetItemFromLoop();
                int index = lastItem.dataIndex + 1;
                AddToBack(lastItem, item, index, datasList[index]);
                lastItem = item;
                itemsList.Add(item);
            }
        }
        else
        {
            // 判断要不要将它移除
            // 条件：自身是不可见的;且它前一个item也是不可见的（或被被裁剪过半的）.隐含条件itemsList.Count>=2.
            if (itemsList.Count > m_maxViewCnt
                && !IsVisible(itemsList[itemsList.Count - 1])
                && !IsVisible(itemsList[itemsList.Count - 2]))
            {
                itemsList.Remove(lastItem);
                PutItemToLoop(lastItem);
                lastItem = itemsList[itemsList.Count - 1];
            }
        }
    }

    private bool isPivotLast { get { return lastItem != null && datasList != null && datasList.Count > 1 && lastItem.dataIndex == datasList.Count - 1; } }

    private bool IsEqual(List<LoopItemData> datas)
    {
        bool equal = true;
        if (datas != null && datasList != null && datas.Count == datasList.Count)
        {
            for (int i = 0; i < datasList.Count; i++)
            {
                if (datasList[i].LoopID != datas[i].LoopID)
                {
                    equal = false;
                    break;
                }
            }
        }
        else
        {
            equal = false;
        }
        return equal;
    }

    // 在列表最后更新一个
    private bool UpdateNewOne(List<LoopItemData> datas)
    {
        bool isnew = true;
        if (datas != null && isPivotLast && datas.Count == datasList.Count )
        {
            int iret = Mathf.Min(4, datasList.Count - 1); //只比较前四个是为了快速，比较新增和刷新整个list效果一样
            for (int i = 0; i < iret; i++)
            {
                if (datasList[i + 1].LoopID != datas[i].LoopID)
                {
                    isnew = false;
                    break;
                }
            }
        }
        else
        {
            isnew = false;
        }
        return isnew;
    }

    // 在列表最后新增一个
    private bool IsAddNew(List<LoopItemData> datas)
    {
        bool isnew = true;
        if (datas != null && isPivotLast && datas.Count - datasList.Count == 1)
        {
            int iret = Mathf.Min(4, datasList.Count); //只比较前四个是为了快速，比较新增和刷新整个list效果一样
            for (int i = 0; i < iret; i++)
            {
                if (datasList[i].LoopID != datas[i].LoopID)
                {
                    isnew = false;
                    break;
                }
            }
        }
        else
        {
            isnew = false;
        }
        return isnew;
    }

    // 此方法主要是为了避免新加item而整体进行刷新
    public void AddItem(LoopItemData data)
    {
        datasList.Add(data);
        LoopItemObject item = GetItemFromLoop();
        int index = datasList.Count - 1;
        AddToBack(lastItem, item, index, datasList[index]);
        lastItem = item;
        itemsList.Add(item);
    }

    public void UpdateItem(LoopItemData data)
    {
        if (datasList.Count > 0) datasList.RemoveAt(0);
        if (itemsList.Contains(firstItem)) itemsList.Remove(firstItem);
        PutItemToLoop(firstItem);
        if (itemsList.Count > 0) firstItem = itemsList[0];
        AddItem(data);
    }

    public bool IsVisible(LoopItemObject obj)
    {
        if (obj == null || obj.widget == null || obj.widget.transform == null) return false;
        return IsVisible(obj.widget.transform);
    }

    private bool IsVisible(Transform tran)
    {
        bool x = tran.position.x >= m_TopLeft.x && tran.position.x <= m_BtmRight.x;
        bool y = tran.position.y >= m_BtmRight.y && tran.position.y <= m_TopLeft.y;
        return x && y;
    }

    public GameObject gameobject
    {
        get { return this.gameObject; }
    }


    public bool IsScrollLast()
    {
        if (lastItem != null && lastItem.widget != null)
        {
            return IsVisible(lastItem);
        }
        else
        {
            return true;
        }
    }
    

    [ContextMenu("reset pos")]
    public void ResetScroll()
    {
        scrollView.ResetPosition(mTop ? 0f : 1f);
    }

    public void SetClipSize(Vector2 size)
    {
        Vector4 reg = scrollView.panel.baseClipRegion;
        reg.z = size.x;
        reg.w = size.y;
        scrollView.panel.baseClipRegion = reg;
    }

    public void SetDepth(int depth)
    {
        if (scrollView.panel != null) scrollView.panel.depth = depth;
    }

    public GameObject GetFirstItem()
    {
        return firstItem.GetObj();
    }

    public GameObject GetLastItem()
    {
        return lastItem.GetObj();
    }

    /// <summary>
    /// 构造一个 item 对象
    /// </summary>
    LoopItemObject CreateItem()
    {
        GameObject go = NGUITools.AddChild(itemParent, itemPrefab);
        UIWidget widget = go.GetComponent<UIWidget>();
        LoopItemObject item = new LoopItemObject();
        item.widget = widget;
        if(!go.activeSelf) go.SetActive(true);
        return item;
    }

    /// <summary>
    /// 用数据列表来初始化scrollview
    /// </summary>
    void InitItem(LoopItemObject item, int dataIndex, LoopItemData data)
    {
        item.dataIndex = dataIndex;
        if (OnItemInit != null)
        {
            OnItemInit(item as ILoopItemObject, data);
        }
        item.widget.transform.localPosition = itemStartPos;
    }

    /// <summary>
    /// 在itemsList前面补上一个item
    /// </summary>
    private void AddToFront(LoopItemObject priorItem, LoopItemObject newItem, int newIndex, LoopItemData newData)
    {
        if (!CheckItem(priorItem) || !CheckItem(newItem)) return;
        InitItem(newItem, newIndex, newData);
        // 计算新item的位置
        if (scrollView.movement == UIScrollView.Movement.Vertical)
        {
            float offsetY = priorItem.widget.height * 0.5f + gapDis + newItem.widget.height * 0.5f;
            if (arrangeDirection == ArrangeDirection.Down_to_Up) offsetY *= -1f;
            newItem.widget.transform.localPosition = priorItem.widget.cachedTransform.localPosition + new Vector3(0f, offsetY, 0f);
        }
        else
        {
            float offsetX = priorItem.widget.width * 0.5f + gapDis + newItem.widget.width * 0.5f;
            if (arrangeDirection == ArrangeDirection.Right_to_Left) offsetX *= -1f;
            newItem.widget.transform.localPosition = priorItem.widget.cachedTransform.localPosition - new Vector3(offsetX, 0f, 0f);
        }
    }

    /// <summary>
    /// 在itemsList后面补上一个item
    /// </summary>
    private void AddToBack(LoopItemObject backItem, LoopItemObject newItem, int newIndex, LoopItemData newData)
    {
        if (!CheckItem(backItem) || !CheckItem(newItem)) return;
        InitItem(newItem, newIndex, newData);
        // 计算新item的位置
        if (scrollView.movement == UIScrollView.Movement.Vertical)
        {
            float offsetY = backItem.widget.height * 0.5f + gapDis + newItem.widget.height * 0.5f;
            if (arrangeDirection == ArrangeDirection.Down_to_Up) offsetY *= -1f;
            newItem.widget.transform.localPosition = backItem.widget.cachedTransform.localPosition - new Vector3(0f, offsetY, 0f);
        }
        else
        {
            float offsetX = backItem.widget.width * 0.5f + gapDis + newItem.widget.width * 0.5f;
            if (arrangeDirection == ArrangeDirection.Right_to_Left) offsetX *= -1f;
            newItem.widget.transform.localPosition = backItem.widget.cachedTransform.localPosition + new Vector3(offsetX, 0f, 0f);
        }
    }


    private void CheckBTM()
    {
        if (mTop && lastItem != null
            && lastItem.widget != null
            && arrangeDirection == ArrangeDirection.Up_to_Down)
        {
            if (m_BtmRight.y > lastItem.widget.transform.position.y)
            {
                mTop = false;
                ResetScroll();
            }
        }
    }


    #region 对象池性能相关
    /// <summary>
    /// 从对象池中取行一个item
    /// </summary>
    /// <returns>The item from loop.</returns>
    LoopItemObject GetItemFromLoop()
    {
        LoopItemObject item = null;
        while (itemLoop.Count > 0 && !CheckItem(item))
        {
            item = itemLoop.Dequeue();
        }
        if (item == null)
        {
            item = CreateItem();
        }
        if (CheckItem(item) && !item.widget.gameObject.activeSelf) item.widget.gameObject.SetActive(true);
        return item;
    }

    private bool CheckItem(LoopItemObject item)
    {
        return item != null && item.widget != null && item.widget.gameObject != null;
    }

    /// <summary>
    /// 将要移除的item放入对象池中
    /// --这个里我保证这个对象池中存在的对象不超过3个
    /// </summary>
    /// <param name="item">Item.</param>
    void PutItemToLoop(LoopItemObject item)
    {
        if (!CheckItem(item)) return;
        if (itemLoop.Count >= 3)
        {
            DestroyImmediate(item.widget.gameObject);
            return;
        }
        item.dataIndex = -1;
        item.widget.gameObject.SetActive(false);
        itemLoop.Enqueue(item);
    }
    #endregion

}


