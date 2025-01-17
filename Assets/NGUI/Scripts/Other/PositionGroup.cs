using UnityEngine;
using XUtliPoolLib;
using System.Collections;
using System.Collections.Generic;

[ExecuteInEditMode]
public class PositionGroup : MonoBehaviour, IXPositionGroup
{
    public List<Vector3> pos = new List<Vector3>();
    public List<string> desc = new List<string>();

    void Awake()
    {
        if(!Application.isPlaying && pos.Count == 0)
        {
            pos.Add(transform.localPosition);
            desc.Add("");
        }
    }

    public void ChangeList(int index)
    {
        if(index == -1)
        {
            pos.Add(Vector3.zero);
            desc.Add("");
        }
        else
        {
            if (pos.Count <= 1)
                return;
            pos.RemoveAt(index);
            desc.RemoveAt(index);
        }
    }

    public void SetGroup(int index)
    {
        if (index >= 0 && index < pos.Count)
            transform.localPosition = pos[index];
    }

    public Vector3 GetGroup(int index)
    {
        if (index >= 0 && index < pos.Count)
            return pos[index];
        return Vector3.zero;
    }
}
