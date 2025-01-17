using UILib;
using UnityEngine;
using System;
using System.Collections.Generic;

public class XUISimpleList : XUIObject , IXUISimpleList
{
    public enum Sorting
    {
        Horizontal,
        Vertical,
    }

    struct SimpleNode : IComparable<SimpleNode>
    {
        public Transform t;
        public Vector3 pos;

        public int CompareTo(SimpleNode other)
        {
            switch (sSorting)
            {
                case Sorting.Horizontal:
                    return sRevert * pos.x.CompareTo(other.pos.x);
                case Sorting.Vertical:
                    return sRevert * pos.y.CompareTo(other.pos.y);
            }
            return 0;
        }

        public static Sorting sSorting = Sorting.Horizontal;
        public static int sRevert = -1;

    }

    SimpleNode[] m_OriginDatas;

    public Sorting sorting = Sorting.Horizontal;
    public bool IsRevert = false;

    protected override void OnAwake()
    {
        base.OnAwake();

        int count = 0;
        for (int i = 0; i < transform.childCount; ++i)
        {
            Transform child = transform.GetChild(i);
            if (child && child.gameObject)
            {
                ++count;
            }
        }

        m_OriginDatas = new SimpleNode[count];

        for (int i = 0, j = 0; i < transform.childCount; ++i)
        {
            Transform child = transform.GetChild(i);
            if (child && child.gameObject)
            {
                m_OriginDatas[j].pos = child.localPosition;
                m_OriginDatas[j].t = child;
                ++j;
            }
        }

        SimpleNode.sSorting = sorting;
        SimpleNode.sRevert = IsRevert ? -1 : 1;

        Array.Sort(m_OriginDatas);
    }

    public void Refresh()
    {
        int j = 0;
        for (int i = 0; i < m_OriginDatas.Length; ++i)
        {
            if (m_OriginDatas[i].t == null || !m_OriginDatas[i].t.gameObject.activeSelf)
                continue;

            m_OriginDatas[i].t.localPosition = m_OriginDatas[j++].pos;
        }
    }
}
