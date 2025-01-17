#if UNITY_EDITOR
using UnityEngine;
using XUtliPoolLib;
using XEditor;

public class XCutSceneUI : XSingleton<XCutSceneUI>
{
    public UISprite m_BG;
    public UILabel m_Text;
    public UILabel m_Skip;
    public UIPlayTween m_IntroTween;
    public UILabel m_Name;

    public GameObject _objUI;

    public override bool Init()
    {
        UIPanel p = NGUITools.CreateUI(false);
        
        _objUI = XResourceLoaderMgr.singleton.CreateFromPrefab("UI/Common/CutSceneUI") as GameObject;

        if (null != _objUI)
        {
            _objUI.transform.parent = p.transform;
            _objUI.transform.localPosition = new Vector3(0.0f, 0.0f, 0);
            _objUI.transform.localScale = new Vector3(1, 1, 1);
        }

        UIRoot rt = p.gameObject.GetComponent<UIRoot>();
        rt.scalingStyle = UIRoot.Scaling.FixedSize;
        rt.manualHeight = 1148;

        m_Text = _objUI.transform.Find("_canvas/DownBG/Text").GetComponent<UILabel>();

        m_Name = _objUI.transform.Find("_canvas/Intro/Name").GetComponent<UILabel>();
        m_IntroTween = _objUI.transform.Find("_canvas/Intro").GetComponent<UIPlayTween>();

        m_Text.text = "";

        _objUI.SetActive(false);
        m_IntroTween.gameObject.SetActive(false);
        return true;
    }

    public void SetText(string text)
    {
        m_Text.text = text;
    }

    public void SetVisible(bool visible)
    {
        _objUI.SetActive(visible);
    }

    public void SetIntroText(bool enabled, string name, string text, float x, float y)
    {
        if (!_objUI.activeInHierarchy) return;

        if (enabled)
        {
            m_Name.text = name;

            m_IntroTween.gameObject.transform.localPosition = new Vector2(x, y);
            m_IntroTween.tweenGroup = 0;
            m_IntroTween.ResetByGroup(true, 0);
            m_IntroTween.Play(true);

        }
        else 
        {
            m_IntroTween.tweenGroup = 1;
            m_IntroTween.ResetByGroup(true, 1);
            m_IntroTween.Play(true);
        }
    }
}
#endif