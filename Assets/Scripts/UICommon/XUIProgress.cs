using UILib;
using UnityEngine;
using System.Collections.Generic;

public class XUIProgress : XUIObject, IXUIProgress
{
    protected float mTargetValue = -1f;
    //protected bool bInAnimation = false;

    public uint totalSection = 1;
    public List<Color> SectionColors;

    public UILabel sectionText;

	public float value 
	{ 
		get
		{
			return  m_uiSlider.value;
		}
		set
		{
		    //if (!bInAnimation)
		    {
		        if (totalSection <= 1)
		        {
		            m_uiSlider.value = value;

                    _SetSection((int)totalSection);
                    //if(sectionText != null)
                    //    sectionText.alpha = 0.0f;

		            if (value < logicalValue)
		            {
                        targetSection = 0;
                        targetValue = value;

                        mDynamicVal = logicalValue;
                        mDynamicSection = 0;
                        mDynamicDelta = (mDynamicVal - value) / mDynamicStep;
		            }
		        }
		        else
		        {
		            int mySection = 0;
		            float v = 0;
		            GetSectionAndValue(value, ref mySection, ref v);
		            m_uiSlider.value = v;

		            int preSection = 0;
		            float preV = 0;
                    GetSectionAndValue(logicalValue, ref preSection, ref preV);

                    if (preSection == mySection && preV - v > mDynamicThreshold)
		            {
                        targetSection = mySection;
                        targetValue = v;

		                mDynamicVal = preV;
		                mDynamicSection = preSection;
		                mDynamicDelta = (mDynamicVal - v) / mDynamicStep;
		            }
                    else if(mySection != preSection)
                    {
                        targetSection = mySection;
                        targetValue = v;

                        mDynamicVal = preV;
                        mDynamicSection = preSection;
                        mDynamicDelta = (mDynamicVal - v + 1) / mDynamicStep;

                        _SetSection(mySection);
                        //if (sectionText != null)
                        //{
                        //    sectionText.alpha = 1.0f;
                        //    sectionText.text = "×" + mySection.ToString();
                        //}
                    }

                    if (SectionColors.Count > 0)
		            {
		                Color foreColor = SectionColors[mySection%SectionColors.Count];
		                m_Fore.color = foreColor;

		                if (mySection > 0)
		                {
                            Color backColor = SectionColors[(mySection - 1) % SectionColors.Count];
                            m_Back.alpha = 1.0f;
                            m_Back.color = backColor;
		                }
		                else
		                {
                            m_Back.alpha = 0.0f;
		                }
		            }                    

		        }

		        logicalValue = value;
		    }
            //else
            //{
            //    if (mTargetValue > m_uiSlider.value)
            //    {//正往上涨动画
            //        if (value > m_uiSlider.value)
            //        {
            //            mTargetValue = value;
            //        }
            //        else
            //        {
            //            m_uiSlider.value = value;
            //            bInAnimation = false;
            //        }
            //    }
            //    else
            //    {//正往下跌动画
            //        if (value < m_uiSlider.value)
            //        {
            //            mTargetValue = value;
            //        }
            //        else
            //        {
            //            m_uiSlider.value = value;
            //            bInAnimation = false;
            //        }
            //    }
            //}
		}
	}

    void _SetSection(int sec)
    {
        if (sectionText != null)
        {
            if (totalSection <= 1)
            {
                sectionText.alpha = 0.0f;
            }
            else
            {
                sectionText.alpha = 1.0f;
                //sectionText.text = "×" + sec.ToString();
                sectionText.text = "x" + sec.ToString();
            }
        }
    }

    void Update()
    {
        if (targetSection > -1 && targetValue > -1)
        {
            int depth = m_Fore.depth - 1;
            if (mDynamicSection == targetSection)
            {
                if (mDynamicVal <= targetValue)
                {
                    m_uiSlider.SetDynamicGround(0, depth);
                    targetSection = -1;
                    targetValue = -1;

                    //if (logicalValue == 0)
                    //{
                    //    SetVisible(false);
                    //}
                }
            }
            else if (mDynamicSection > targetSection)
            {
                depth = m_Fore.depth + 1;
                if (mDynamicVal <= 0)
                {
                    mDynamicSection = targetSection;
                    mDynamicVal = 1;
                }
            }

            m_uiSlider.SetDynamicGround(mDynamicVal, depth);
            //mDynamicStartVal -= RealTime.deltaTime;
            UpdateDynamicValue();
            
        }
        else
        {
            m_uiSlider.SetDynamicGround(0, m_Fore.depth - 1);
        }
        //if (bInAnimation)
        //{
        //    m_uiSlider.value += (mTargetValue - m_uiSlider.value)*0.02f;

        //    if (Mathf.Abs(mTargetValue - m_uiSlider.value) < 0.02f)
        //    {
        //        bInAnimation = false;
        //    }

        //}

    }

    protected void UpdateDynamicValue()
    {
        mDynamicVal -= mDynamicDelta;
    }

    public int width
    {
        get
        {
            return m_uiSlider.backgroundWidget.width;
        }
        set
        {
            m_uiSlider.backgroundWidget.width = value;
        }
    }

    public GameObject foreground
    {
        get
        {
            return m_uiSlider.mFG.gameObject; ;
        }
    }

    public void ForceUpdate()
    {
        //m_uiSlider.ForceUpdate();
    }

    public void SetDepthOffset(int d)
    {
        UIWidget[] w = gameObject.GetComponentsInChildren<UIWidget>();

        for (int i = 0; i < w.Length; i++)
        {
            w[i].depth += d;
        }
    }

    public void SetValueWithAnimation(float value)
    {
        //if (value != m_uiSlider.value)
        //{
        //    bInAnimation = true;
        //    mTargetValue = value;
        //}
    }

    public void SetTotalSection(uint section)
    {
        totalSection = section;

        perSection = 1.0f / totalSection;
    }

    protected void GetSectionAndValue(float logicalValue, ref int section, ref float v)
    {
        section = (int)(logicalValue / perSection);
        v = (logicalValue - section * perSection) / perSection;
    }

    protected override void OnAwake()
    { 
		base.OnAwake();
        m_uiSlider = GetComponent<UISlider>();
        if (null == m_uiSlider)
        {
            Debug.LogError("null == m_uiSlider");
        }

        m_Fore = m_uiSlider.mFG;
        m_Back = m_uiSlider.mBG;
        //m_Dynm= m_uiSlider.mDG;

        if (sectionText != null)
            sectionText.text = (string.Empty);
    }
    
    public void SetForegroundColor(Color c)
    {
        m_Fore.color = c;
    }
    private UISlider m_uiSlider = null;
    private UIWidget m_Fore = null;
    private UIWidget m_Back = null;
   // private UIWidget m_Dynm = null;

    protected float perSection = 0;
    protected float logicalValue = 0;

    protected float mDynamicThreshold = 0.01f;
    protected float mDynamicVal = 0;
    protected float mDynamicSection = 0;
    protected float mDynamicDelta = 0;
    protected int mDynamicStep = 12;

    protected int targetSection = -1;
    protected float targetValue = -1;

    
}

