//----------------------------------------------
//            NGUI: Next-Gen UI kit
// Copyright © 2011-2014 Tasharen Entertainment
//----------------------------------------------

using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

/// <summary>
/// Inspector class used to edit UITextures.
/// </summary>

[CanEditMultipleObjects]
#if UNITY_3_5
[CustomEditor(typeof(UITexture))]
#else
[CustomEditor(typeof(UITexture), true)]
#endif
public class UITextureInspector : UIWidgetInspector
{
	UITexture mTex;
    //Texture mainTex;
	protected override void OnEnable ()
	{
		base.OnEnable();
		mTex = target as UITexture;
        //if (mTex != null)
        //{
        //    mainTex = mTex.mainTexture;
        //}

	}
    bool forceChange = false;

    protected override bool ShouldDrawProperties ()
	{
        if(mTex is UITextureStatic)
        {
            UITextureStatic staticTex = mTex as UITextureStatic;
            staticTex.StaticTexture = EditorGUILayout.ObjectField("Tex0", staticTex.StaticTexture, typeof(UnityEngine.Texture), true) as Texture;
        }
        else
        {
            NGUIEditorTools.DrawProperty("IsRuntimeLoad", serializedObject, "mIsRuntimeLoad", GUILayout.Width(150f));
            GUILayout.BeginHorizontal();
            GUILayout.Label("Path");
            GUILayout.Label(mTex.texPath, GUILayout.Width(250f));
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            Texture tex0 = EditorGUILayout.ObjectField("Tex0", mTex.mTexture, typeof(UnityEngine.Texture), true) as Texture;
            GUILayout.EndHorizontal();
            if (tex0 != mTex.mTexture)
            {
                if (tex0 == null)
                {
                    mTex.SetTexture("");
                }
                else
                {
                    string path = AssetDatabase.GetAssetPath(tex0);
                    string uiPath = "Assets/Resources/";
                    if (path.StartsWith(uiPath))
                    {
                        path = path.Replace(uiPath, "");
                        int index = path.LastIndexOf(".");
                        if (index >= 0)
                        {
                            path = path.Substring(0, index);
                        }
                        mTex.SetTexture(path);
                    }
                }

            }
            NGUISettings.texture = tex0;
            if (mTex.mtexType == UITexture.normalTex)
            {
                Shader shader = EditorGUILayout.ObjectField("Tex0", mTex.shader, typeof(UnityEngine.Shader), true) as Shader;
                if (shader != mTex.shader)
                {
                    if (shader != null &&
                        shader.name == "Custom/UI/Mask" ||
                        shader.name == "Custom/UI/MaskColor" ||
                        shader.name == "Custom/UI/RenderTexture")
                    {
                        mTex.shaderName = shader.name;
                        mTex.shader = shader;
                    }
                    else
                    {
                        mTex.shaderName = null;
                    }
                }

                NGUIEditorTools.DrawPaddedProperty("Flip", serializedObject, "mFlip");
                EditorGUI.BeginDisabledGroup(serializedObject.isEditingMultipleObjects);

                if (tex0 != null)
                {
                    Rect rect = EditorGUILayout.RectField("UV Rectangle", mTex.uvRect);

                    if (rect != mTex.uvRect)
                    {
                        NGUIEditorTools.RegisterUndo("UV Rectangle Change", mTex);
                        mTex.uvRect = rect;
                    }
                }
                EditorGUI.EndDisabledGroup();

                GUILayout.BeginHorizontal();
                if (GUILayout.Button("ToList"))
                {
                    mTex.mtexType = UITexture.horizontally2;
                }
                GUILayout.EndHorizontal();
            }
            else
            {
                GUILayout.BeginHorizontal();
                EditorGUILayout.ObjectField("Tex1", mTex.mTexture1, typeof(UnityEngine.Texture), true);
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                if (GUILayout.Button("ToNormal"))
                {
                    mTex.mtexType = UITexture.normalTex;
                }
                GUILayout.EndHorizontal();
            }
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Clear"))
            {
                mTex.shaderName = "";
                mTex.mtexType = UITexture.normalTex;
                mTex.mTexture = null;
                mTex.mTexture1 = null;
                mTex.texPath = "";
                mTex.mShader = null;
            }
            if (GUILayout.Button("Apply"))
            {
                mTex.Refresh();
            }
            GUILayout.EndHorizontal();
        }
        
        //if (!AssetDatabase.GetAssetPath(tex0).ToLower().EndsWith("split.png"))
        //{
        //    mTex.mType = UITexture.TextureType.Normal;
        //    mTex.mainTexture = tex0;
        //    mTex.mTexture0 = null;
        //    return true;
        //}

        //if (mTex.mType == UITexture.TextureType.Normal)
        //{
        //    SerializedProperty sp = NGUIEditorTools.DrawProperty("Texture", serializedObject, "texPath");
        //    NGUIEditorTools.DrawProperty("AlphaTexture", serializedObject, "mAlphaTexture");
        //    NGUIEditorTools.DrawProperty("Material", serializedObject, "mMat");

        //    NGUISettings.texture = sp.objectReferenceValue as Texture;
        //    if(AssetDatabase.GetAssetPath(sp.objectReferenceValue as Texture).ToLower().EndsWith("split.png"))
        //    {
        //        mTex.mainTexture = null;
        //        mTex.mType = UITexture.TextureType.List;
        //        mTex.mTexture = sp.objectReferenceValue as Texture;
        //        forceChange = true;
        //        return true;
        //    }

        //    if (mTex.material == null || serializedObject.isEditingMultipleObjects)
        //    {
        //        NGUIEditorTools.DrawProperty("Shader", serializedObject, "mShader");
        //    }


        //}
        //else
        //{
        //    EditorGUILayout.ObjectField("Tex1", mTex.mTexture1, typeof(UnityEngine.Texture), true);


        //    //UITexture.TexType newTexType = (UITexture.TexType)EditorGUILayout.EnumPopup(texType);
        //    //if (texType != newTexType)
        //    //{
        //    //    mTex.SetTextureType((byte)newTexType);
        //    //}
        //    Texture tex0 = EditorGUILayout.ObjectField("Tex0", mTex.mTexture0, typeof(UnityEngine.Texture), true) as Texture;
        //    if(!AssetDatabase.GetAssetPath(tex0).ToLower().EndsWith("split.png"))
        //    {
        //        mTex.mType = UITexture.TextureType.Normal;
        //        mTex.mainTexture = tex0;
        //        mTex.mTexture0 = null;
        //        return true;
        //    }
        //    EditorGUILayout.ObjectField("Tex1", mTex.mTexture1, typeof(UnityEngine.Texture), true);
        //    if (tex0 != mTex.mTexture0 || forceChange)
        //    {
        //        forceChange = false;
        //        if (tex0 != null)
        //        {
        //            string texPath = AssetDatabase.GetAssetPath(tex0);
        //            if (texPath.StartsWith("Assets/atlas/UI"))
        //            {
        //                texPath = texPath.Replace("Assets/", "");
        //                int index = texPath.LastIndexOf(".");
        //                if (index >= 0)
        //                {
        //                    texPath = texPath.Substring(0, index);
        //                }


        //            }
        //            else if (texPath.StartsWith("Assets/Resources/atlas/UI"))
        //            {
        //                texPath = texPath.Replace("Assets/Resources/", "");
        //                int index = texPath.LastIndexOf(".");
        //                if (index >= 0)
        //                {
        //                    texPath = texPath.Substring(0, index);
        //                }
        //            }
        //            else
        //            {
        //                Debug.LogError("需要将贴图放在Assets/atlas/UI或Assets/Resources/atlas/UI目录下");
        //                return true;
        //            }
        //            //byte t = UITexture.horizontally2;
        //            //if (tex0.width / tex0.height == 2)
        //            //{
        //            //    t = UITexture.horizontally2;
        //            //}
        //            //else if (tex0.height / tex0.width == 2)
        //            //{
        //            //    t = UITexture.vertically2;
        //            //}
        //            //else if (tex0.width / tex0.height == 4)
        //            //{
        //            //    t = UITexture.horizontally4;
        //            //}
        //            //else if (tex0.height / tex0.width == 4)
        //            //{
        //            //    t = UITexture.vertically4;
        //            //}
        //            mTex.SetTexture(texPath/*, t*/);
        //            if (mTex.mainTexture == null)
        //            {
        //                mTex.SetTexture(""/*, (byte)newTexType*/);
        //            }
        //        }
        //        else
        //        {
        //            mTex.SetTexture(""/*, (byte)newTexType*/);
        //        }


        //    }
        //}

        return true;
	}

	/// <summary>
	/// Allow the texture to be previewed.
	/// </summary>

	public override bool HasPreviewGUI ()
	{
		return (mTex != null) && (mTex.mainTexture as Texture2D != null);
	}

    /// <summary>
    /// Draw the sprite preview.
    /// </summary>

    private static Rect uvRect = new Rect(0, 0, 1, 1);
    public override void OnPreviewGUI (Rect rect, GUIStyle background)
	{
		Texture2D tex = mTex.mainTexture as Texture2D;
        if (tex != null)
        {
            if(mTex.mtexType == UITexture.normalTex)
                NGUIEditorTools.DrawTexture(tex, rect, mTex.uvRect, mTex.color);
            else
                NGUIEditorTools.DrawTexture(tex, rect, uvRect, mTex.color);
        }
    }
}
