using UILib;
using UnityEngine;
using System.Collections.Generic;
using XUtliPoolLib;

public class XSthCollector : XUIObject, IXUISthCollector
{
    public List<GameObject> SthList = new List<GameObject>();

    public Vector3 Src = new Vector3(0, 0, 0);
    public Vector3 Des = new Vector3(-450, -280, 0);

    public int Count = 15;
    public float EmitInterval = 0.01f;

    public int EmitDegreeRange = 300;
    public float MinEmitSpeed = 1900;
    public float MaxEmitSpeed = 2100;

    public float SrcAcceleration = 20000;
    public float DesAcceleration = 8000;
    public float SthAcceleration = 10000;

    public float MinStartFindDesTime = 0.3f;
    public float MaxStartFindDesTime = 0.6f;

    public float MinIdleSpeed = 30;
    List<XSth> m_SthList = null;
    bool m_bActive;

    //string m_CurName;
    GameObject m_CurSthGo;

    //Dictionary<string, List<XSth>> m_SthListMap = new Dictionary<string, List<XSth>>();
    Dictionary<string, GameObject> m_SthTplMap = new Dictionary<string, GameObject>();
    List<XSth> m_ExternalSthList = new List<XSth>();

    Vector3 m_Direction;
    SthArrivedEventHandler m_SthArrivedEventHandler;
    CollectFinishEventHandler m_CollectFinishEventHandler;

    int m_ArrivedCount;
    void Awake()
    {
        for (int i = 0; i < SthList.Count; ++i)
        {
            m_SthTplMap.Add(SthList[i].name, SthList[i]);
            SthList[i].SetActive(false);
        }
        m_Direction = (Des - Src).normalized;
    }
    void OnEnable()
    {
        m_bActive = false;

        if (m_SthList != null)
        {
            for (int i = 0; i < m_SthList.Count; ++i)
            {
                m_SthList[i].bEnable = false;
            }
        }
    }

    public void SetPosition(Vector3 srcGlobalPos, Vector3 desGlobalPos)
    {
        Src = transform.worldToLocalMatrix * srcGlobalPos;
        Des = transform.worldToLocalMatrix * desGlobalPos;
        m_Direction = (Des - Src).normalized;
    }

    public void SetSth(List<GameObject> goes)
    {
        for (int i = m_ExternalSthList.Count; i < goes.Count; ++i)
        {
            XSth sth = new XSth();
            m_ExternalSthList.Add(sth);
        }
        for (int i = m_ExternalSthList.Count - 1; i >= goes.Count; --i)
        {
            m_ExternalSthList.RemoveAt(i);
        }
        m_SthList = m_ExternalSthList;

        for (int i = 0; i < m_ExternalSthList.Count; ++i)
        {
            m_SthList[i].Go = goes[i];
            goes[i].SetActive(false);
            m_SthList[i].bEnable = false;
        }
    }

    public void SetSth(string name)
    {
        //if (m_SthList != null && m_CurName != name)
        //{
        //    for (int i = 0; i < m_SthList.Count; ++i)
        //    {
        //        m_SthList[i].bEnable = false;
        //    }
        //}
        //if (!m_SthTplMap.TryGetValue(name, out m_CurSthGo))
        //{
        //    Debug.LogError("Cant find tpl name: " + name);
        //    return;
        //}

        //if (!m_SthListMap.TryGetValue(name, out m_SthList))
        //{
        //    m_SthList = new List<XSth>();
        //    m_SthListMap.Add(name, m_SthList);
        //}

        //_GenerateSth();
    }

    private void _GenerateSth()
    {
        if (m_SthList.Count != Count)
        {
            m_CurSthGo.SetActive(true);
            for (int i = m_SthList.Count; i < Count; ++i)
            {
                XSth sth = new XSth();
                GameObject go = UnityEngine.Object.Instantiate(m_CurSthGo) as GameObject;
                go.transform.parent = transform;
                go.transform.localScale = Vector3.one;
                sth.Go = go;
                go.SetActive(false);
                m_SthList.Add(sth);
            }
            m_CurSthGo.SetActive(false);

            for (int i = m_SthList.Count - 1; i >= Count; --i)
            {
                m_SthList[i].Destroy();
                m_SthList.RemoveAt(i);
            }
        }

        for (int i = 0; i < m_SthList.Count; ++i)
        {
            m_SthList[i].bEnable = false;
        }
    }

    [ContextMenu("Emit")]
    public void Emit()
    {
        float fDelayTime = 0.0f;
        for (int i = 0; i < m_SthList.Count; ++i, fDelayTime += EmitInterval)
        {
            XSth sth = m_SthList[i];
            sth.DelayTime = fDelayTime;

            sth.Speed = XCommon.singleton.RandomFloat(MinEmitSpeed, MaxEmitSpeed) * _RandEmitDir();
            sth.StartFindDesTime = XCommon.singleton.RandomFloat(MinStartFindDesTime, MaxStartFindDesTime);
            sth.Time = 0.0f;
            sth.MinIdleSpeed = MinIdleSpeed;
            sth.Go.transform.localPosition = Src;// transform.worldToLocalMatrix* Src;
            sth.Des = Des;// transform.worldToLocalMatrix* Des;
            sth.bEnable = true;
        }
        m_bActive = true;
        m_ArrivedCount = 0;
    }

    Vector3 _RandEmitDir()
    {
        float radRange = EmitDegreeRange * Mathf.Deg2Rad;
        float rad = XCommon.singleton.RandomFloat(radRange) - radRange / 2;
        float sinA = Mathf.Sin(rad);
        float cosA = Mathf.Cos(rad);

        Vector3 vec = m_Direction;
        vec.x = m_Direction.x * cosA - m_Direction.y * sinA;
        vec.y = m_Direction.x * sinA + m_Direction.y * cosA;

        return vec;
    }

    Vector3 _GetAcceleration(XSth sth, float t)
    {
        Vector3 curPos = sth.Go.transform.localPosition;
        Vector3 acc = Vector3.zero;

        Vector3 gDes = (Des - curPos).normalized;
        if (sth.State == XSth.SthState.FLAME_OUT)
        {
            acc += gDes * DesAcceleration;
        }

        Vector3 gSrc = (Src - curPos).normalized;
        if (sth.State == XSth.SthState.IDLE)
        {
            acc += gSrc * SrcAcceleration;
        }

        if (sth.State == XSth.SthState.DIRECTION_ADJUSTING)
        {
            float desCross = gDes.x * sth.Speed.y - gDes.y * sth.Speed.x;

            Vector3 perpendicular = new Vector3(-sth.Speed.y, sth.Speed.x, sth.Speed.z);
            if ((perpendicular.x * sth.Speed.y - perpendicular.y * sth.Speed.x) * desCross < 0)
            {
                perpendicular.x = -perpendicular.x;
                perpendicular.y = -perpendicular.y;
            }
            perpendicular = perpendicular.normalized * SthAcceleration;
            acc += perpendicular;
        }

        return acc;
    }

    void Update()
    {
        if (!m_bActive)
            return;

        m_bActive = false;
        for (int i = 0; i < m_SthList.Count; ++i)
        {
            XSth sth = m_SthList[i];
            if (!sth.bEnable)
                continue;
            m_bActive = true;

            if (!sth.Update(Time.deltaTime))
            {
                if (m_SthArrivedEventHandler != null)
                    m_SthArrivedEventHandler(m_ArrivedCount++);
            }

            if (sth.DelayTime <= 0.0f)
                sth.Acceleration = _GetAcceleration(sth, Time.deltaTime);
        }

        if (!m_bActive && m_CollectFinishEventHandler != null)
            m_CollectFinishEventHandler();
    }

    public void RegisterSthArrivedEventHandler(SthArrivedEventHandler eventHandler)
    {
        m_SthArrivedEventHandler = eventHandler;
    }

    public void RegisterCollectFinishEventHandler(CollectFinishEventHandler eventHandler)
    {
        m_CollectFinishEventHandler = eventHandler;
    }
}

public class XSth
{
    public enum SthState
    {
        IDLE,
        DIRECTION_ADJUSTING,
        FLAME_OUT
    }
    public GameObject Go;
    public Vector3 Speed;
    public float MinIdleSpeed;
    public Vector3 Acceleration;
    public float Time;
    public float DelayTime;
    public float StartFindDesTime;
    public Vector3 Des;
    public SthState State;

    private bool m_bEnable = false;
    public bool bEnable 
    { 
        get { return m_bEnable; } 
        set
        { 
            m_bEnable = value;
            Acceleration = Vector3.zero;
            State = SthState.IDLE;
            if (!m_bEnable)
            {
                Go.SetActive(false);
            }
        } 
    }

    public void Destroy()
    {
        UnityEngine.Object.Destroy(Go);
    }

    public bool Update(float t)
    {
        if (!bEnable)
            return bEnable;

        if (DelayTime > 0.0f)
        {
            DelayTime -= t;
            if (DelayTime > 0.0f)
                return true;
            t = -DelayTime;
        }

        Time += t;

        Vector3 newSpeed = Speed + (Acceleration * t);

        Vector3 pos = Go.transform.localPosition;

        if (State == SthState.IDLE)
        {
            if (newSpeed.x * Speed.x + newSpeed.y * Speed.y <= 0)
            {
                // 速度减到回落了，避免这种情况，让它以极低速度运行
                newSpeed = Speed.normalized * MinIdleSpeed;
            }

            if (Time > StartFindDesTime)
            {
                State = SthState.DIRECTION_ADJUSTING;
            }
        }
        if (State == SthState.DIRECTION_ADJUSTING)
        {
            Vector3 desDir = Des - pos;
            float crossProduct0 = Speed.x * desDir.y - Speed.y * desDir.x;
            float crossProduct1 = desDir.x * newSpeed.y - desDir.y * newSpeed.x;

            if (crossProduct0 * crossProduct1 > 0)
            {
                newSpeed = newSpeed.magnitude * desDir.normalized;
                State = SthState.FLAME_OUT;
            }
        }
        if (!Go.activeSelf)
            Go.SetActive(true);

        Speed = newSpeed;

        Vector3 newPos = pos + Speed * t;
        if ((newPos - pos).sqrMagnitude >= (Des - pos).sqrMagnitude)
            bEnable = false;
        else
            Go.transform.localPosition = newPos; 

        return bEnable;
    }
}

