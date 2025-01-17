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

[ExecuteInEditMode]
[AddComponentMenu("NGUI/UI/NGUI BloodGrid")]
public sealed class UIBloodGrid : UIWidget, UILib.IUIBloodGrid
{
    public int SignalWidth;
    public int SignalHeight;
    public float UnitSize;
    public int MaxHP;
    public int MAXHP { get { return MaxHP; } }
    /// <summary>
    /// 用于纯色矩形渲染的材质, 独立，不共享
    /// </summary>
    private static Material m_UIColorQuadMaterial = null;  // 静态，唯一，共享

    public override Material material
    {
        get { return UIBloodGrid.m_UIColorQuadMaterial; }
    }
    private static Shader mShader;
    public override Shader shader
    {
        get
        {
            return mShader;
        }
    }

    protected override void OnStart()
    {
        base.OnStart();
        mChanged = true;  // Start时让其重新渲染一次，否则在客户端会加载后没东西
    }

    public void SetMAXHP(int maxHp)
    {
        //Debug.Log("set maxhp = " + maxHp);
        MaxHP = maxHp;
        base.mChanged = true;
    }

    /// <summary>
    /// 负责显示内容，它的工作是填写如何显示，显示什么。就是把需要显示的内容存储在UIWidget
    /// </summary>
    /// <param name="verts"></param>
    /// <param name="uvs">显示的多边形形状</param>
    /// <param name="cols">颜色调配</param>
    public override void OnFill(BetterList<Vector3> verts, BetterList<Vector2> uvs, BetterList<Color32> cols)
    {
        int num = (int)((MaxHP - 1) / UnitSize);
        float eachSize = MaxHP * 1f / mWidth;
        Vector3[] arrVerts = localCorners;  // 取1为左上角定点
        for (int i = 1; i <= num; i++)
        {
            int pixel = Mathf.CeilToInt(i * UnitSize / eachSize);
            float x = arrVerts[1].x + pixel;
            float y = arrVerts[1].y;
            float px = x + SignalWidth > arrVerts[2].x ? arrVerts[2].x : x + SignalWidth;
            verts.Add(new Vector3(x, y - SignalHeight));
            verts.Add(new Vector3(x, y));
            verts.Add(new Vector3(px, y));
            verts.Add(new Vector3(px, y - SignalHeight));
        }

        num *= 4;

        // 贴图点
        for (int i = 0; i < num; i++)
            uvs.Add(new Vector2(0, 0));

        // 顶点颜色
        Color pmaColor = NGUITools.ApplyPMA(this.color);  // NGUI PMA
        for (int i = 0; i < num; i++)
        {
            cols.Add(pmaColor);
        }
    }

    // 创建材质
    void CheckQuadMaterial()
    {
        if (mShader == null)
        {
            //mShader = Shader.Find("Custom/UI/Blood");
            mShader = XUtliPoolLib.ShaderManager.singleton.FindShader("Blood", "Custom/UI/Blood");
        }
    }

    protected override void OnUpdate()
    {
        base.OnUpdate();

        if (mChanged)
        {
            CheckQuadMaterial();
        }

    }

    public UILib.IXUIPanel GetPanel()
    {
        return XUIPanel.GetPanel(panel);
    }
}
