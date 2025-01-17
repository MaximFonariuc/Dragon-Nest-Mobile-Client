using UnityEngine;
using System.Collections;
using XUtliPoolLib;

/// <summary>
/// item对像的封装类LoopItemObject，不要求具体的item类来继承它。
/// 但我们要示具体的item对像一定要包含UIWidget组件。
/// </summary>
[System.Serializable]
public class LoopItemObject:ILoopItemObject
{
    /// <summary>
    /// The widget.
    /// </summary>
    public UIWidget widget;

    /// <summary>
    /// 本item，在实际整个scrollview中的索引位置，
    /// 即对就数据，在数据列表中的索引
    /// </summary>
    public int _dataIndex = -1;

    public int dataIndex
    {
        get { return _dataIndex; }
        set { _dataIndex = value; }
    }

    public bool isVisible()
    {
        LoopScrollView sc = NGUITools.FindInParents<LoopScrollView>(widget.gameObject);
        if (sc != null) return sc.IsVisible(this);
        return false;
    }

    public GameObject GetObj()
    {
        return widget != null ? widget.gameObject : null;
    }


    public void SetHeight(int height)
    {
        widget.height = height;
    }

}
