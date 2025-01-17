//----------------------------------------------
//            NGUI: Next-Gen UI kit
// Copyright © 2011-2014 Tasharen Entertainment
//----------------------------------------------

using UnityEngine;
using System.Collections.Generic;
using XUtliPoolLib;

/// <summary>
/// If you don't have or don't wish to create an atlas, you can simply use this script to draw a texture.
/// Keep in mind though that this will create an extra draw call with each UITexture present, so it's
/// best to use it only for backgrounds or temporary visible widgets.
/// </summary>

[ExecuteInEditMode]
[AddComponentMenu("NGUI/UI/NGUI TextureRGBA32")]
public class UITextureRGBA32 : UITexture
{
    protected override void ResetShader()
    {
        if (mTexture == null)
        {
            mShader = null;
            return;
        }

        if (colorTex == null || colorTex.name == "Custom/UI/ColorTexture")
        {
            colorTex = XUtliPoolLib.ShaderManager.singleton.FindShader("RGBA", "Custom/UI/RGBA");
            //colorTex = Shader.Find("Custom/UI/RGBA");
        }

        mShader = colorTex;
    }
}
