//----------------------------------------------
//            NGUI: Next-Gen UI kit
// Copyright © 2011-2014 Tasharen Entertainment
//----------------------------------------------

using UnityEngine;
using System.Collections.Generic;
using XUtliPoolLib;
/// <summary>
/// Sprite is a textured element in the UI hierarchy.
/// </summary>

public sealed class UI3DFollow : MonoBehaviour, UILib.IUI3DFollow
{
    private Vector3 _target_pos;

    public void SetPos(Vector3 pos)
    {
        _target_pos = pos;
    }

    void Update()
    {
        Vector3 pt = Camera.main.WorldToScreenPoint(_target_pos);
        Vector3 ff = UICamera.mainCamera.ScreenToWorldPoint(pt);
        ff.z = 0.0f;
        transform.position = ff;
    }
}
