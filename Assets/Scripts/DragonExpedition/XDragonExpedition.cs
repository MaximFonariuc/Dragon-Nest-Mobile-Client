using UnityEngine;
using System.Collections;
using XUtliPoolLib;
using System;

public class XDragonExpedition : MonoBehaviour, IXDragonExpedition
{
    #region 接口
    public void Drag(float delta)
    {
        MoveCamera(delta);
    }

    public void  Assign(float delta)
    {
        AssignCamera(delta);
    }

    public Transform GetGO(string name)
    {
        return transform.Find(name);
    }

    public void SetLimitPos(float MinPos)
    {
        MIN_POS = MinPos;
    }

    RaycastHit[] hits = null;

    public GameObject Click()
    {
        //Vector3 pos = mCamera.ScreenToViewportPoint(Input.mousePosition);

        Ray ray = mCamera.ScreenPointToRay(Input.mousePosition);

        float dist = mCamera.farClipPlane - mCamera.nearClipPlane;

        hits = Physics.RaycastAll(ray, dist);
        for (int i = 0; i < hits.Length; ++i)
        {
            if (hits[i].collider.gameObject.name.StartsWith("building"))
                return hits[i].collider.gameObject;
        }

        return null;
    }

    public Camera GetDragonCamera()
    {
        return mCamera;
    }

    #endregion

    void Start()
    {
        curPos = mCamera.transform.localPosition;
    }

    public Camera mCamera = null;

    public float MoveSpeed = 5;

    public float MIN_POS = 0;

    public float MAX_POS = 100;

    Vector3 curPos = Vector3.zero;

    void MoveCamera(float delta)
    {
        curPos.x += delta * MoveSpeed;

        if (curPos.x < MIN_POS) curPos.x = MIN_POS;
        if (curPos.x > MAX_POS) curPos.x = MAX_POS;

        mCamera.transform.localPosition = curPos;
    }

    void AssignCamera(float delta)
    {
        Vector3 pos = mCamera.transform.localPosition;
        pos.x = delta;
        if (pos.x < MIN_POS) pos.x = MIN_POS;
        if (pos.x > MAX_POS) pos.x = MAX_POS;
        curPos.x = pos.x;
        mCamera.transform.localPosition = pos;
    }
}
