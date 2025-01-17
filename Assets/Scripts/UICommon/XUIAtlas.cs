using UILib;
using UnityEngine;


public class XUIAtlas : MonoBehaviour, IXUIAtlas
{
    public UIAtlas atlas
    {
        get { return m_uiAtlas;  }
    }

    void Awake()
    {
        m_uiAtlas = GetComponent<UIAtlas>();
        if (null == m_uiAtlas)
        {
            Debug.LogError("null == m_uiAtlas");
        }
    }

    void OnDisable()
    {
        m_uiAtlas = null;
    }
    private UIAtlas m_uiAtlas = null;
}

