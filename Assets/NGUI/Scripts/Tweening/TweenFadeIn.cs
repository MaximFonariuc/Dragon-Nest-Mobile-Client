using UnityEngine;
using XUtliPoolLib;
using System.Collections;

[ExecuteInEditMode]
public class TweenFadeIn : MonoBehaviour,IXTweenFadeIn
{
    public float Duration = 0.2f;
    public float StartDelay = 0f;
    public float DelayInterval = 0.05f;
    public int Group = 666;
    public Vector3 MoveDeltaPos = new Vector3(0, 40, 0);
    public int TweenPlayItemNum = 999;

    float InitAlpha = 0f;

    float delayTime;

    public bool _editor_need_init = true;
    bool _need_to_init = true;

    [HideInInspector][System.NonSerialized]public UIWidget Widget;
    [HideInInspector][System.NonSerialized]public TweenPosition PosT;
    [HideInInspector][System.NonSerialized]public TweenAlpha AlpT;
    [HideInInspector][System.NonSerialized]public UIPlayTween PlayT;


    void Awake()
    {
        if (!Application.isPlaying)
        {
            if(Get_Component() == false)
                Add_Component();
        }
    }

    public void PlayFadeIn()
    {
        if(!gameObject.activeInHierarchy)
            return ;
        if (_need_to_init)
        {
            _need_to_init = false;
            _Init();  //初始化
        }

        float addDelay = 0f;

        if (!XUITool.Instance.GetTweenFadeInDelayByGroup(Group, DelayInterval, TweenPlayItemNum, out addDelay)) //同样的Tpl克隆设置不同的播放延迟
        {
            Widget.alpha = AlpT.to; //超过最大播放数量，不播放，直接显示
            return;
        }

        delayTime = StartDelay + addDelay; 

        PosT.delay = delayTime;
        AlpT.delay = delayTime;

        //StartCoroutine(IEPlay());  //重置动画
        delayDealWith = true;
        SetInitAlpha();  //为防止出现一帧Alpha不对的情况
    }

    void _Init()
    {
        Get_Component();
    }


    bool delayDealWith = false;
    void LateUpdate()
    {
        if(delayDealWith)
        {
            delayDealWith = false;
            _FakeCoroutine();
        }
    }

    void _FakeCoroutine()
    {
        //yield return null;  //此处要等一帧，兼容儿子也是挂载模式的pool，同时等待设置位置的协程处理完毕之后再reset

        /***************  Position设置  ***************/

        PosT.to = transform.localPosition;
        PosT.from = transform.localPosition - MoveDeltaPos;  //使用协程等待UI位置设置完成, 避免fetch之后立马调用导致的pos动画位置不对
        AlpT.to = 1.0f;
        AlpT.from = 0.0f;

        /***************  Play设置  ***************/

        //PosT.ResetToBeginning(true);  //播放之前重置动画
        //AlpT.ResetToBeginning(true);
        //PlayT.Reset(true);
        PlayT.Play(true);

        //SetInitAlpha();  //初始透明度需要在重置动画之后设置，否则会被覆盖

        /***************  ScrollView设置  ***************/

        UIScrollView _scrollView = NGUITools.FindInParents<UIScrollView>(transform);

        if (_scrollView != null)  //兼容没有scrollView的uisprite和uitexture
            _scrollView.moveControllerTime = Time.time + delayTime + Duration;  //刷新scrollView能拖拽的开始时间
    }

    void SetInitAlpha()
    {
        Color color = Widget.color;
        Widget.color = new Color(color.r, color.g, color.b, InitAlpha);
    }

    public void ResetGroupDelay()
    {
        XUITool.Instance.ResetGroupDelay(Group);
    }

    void OnDisable()
    {
        if (null == PosT || null == AlpT)  //没有init，表明此次不播，直接return
            return;

        if (!PosT.enabled || !AlpT.enabled)  //正常播放结束，不需要执行之后的操作
            return;

        transform.localPosition = PosT.to;  //播到一半的时候隐藏对象，直接设置到目标状态并停止动画
        Widget.alpha = AlpT.to;
        PlayT.Stop();
    }

    bool Get_Component()
    {
        Widget = GetComponent<UIWidget>();

        TweenPosition[] tp = GetComponents<TweenPosition>();
        for (int i = 0; i < tp.Length; i++)
        {
            if (tp[i].tweenGroup == Group)
            {
                PosT = tp[i];
                break;
            }
        }
        if (PosT == null)
            return false;

        TweenAlpha[] ta = GetComponents<TweenAlpha>();
        for (int i = 0; i < ta.Length; i++)
        {
            if (ta[i].tweenGroup == Group)
            {
                AlpT = ta[i];
                break;
            }
        }
        if (AlpT == null)
            return false;

        UIPlayTween[] pt = GetComponents<UIPlayTween>();
        for (int i = 0; i < pt.Length; i++)
        {
            if (pt[i].tweenGroup == Group)
            {
                PlayT = pt[i];
                break;
            }
        }
        if (PlayT == null)
            return false;


        return true;

    }

    public void Add_Component()  //挂载必要组件
    {
        if (null == GetComponent<UIWidget>())
            gameObject.AddComponent<UISprite>();
        Widget = GetComponent<UIWidget>();

        PosT = gameObject.AddComponent<TweenPosition>();
        PosT.enabled = false;
        AlpT = gameObject.AddComponent<TweenAlpha>();
        AlpT.enabled = false;
        AlpT.from = 0f;
        AlpT.to = 1f;
        PlayT = gameObject.AddComponent<UIPlayTween>();
    }
}
