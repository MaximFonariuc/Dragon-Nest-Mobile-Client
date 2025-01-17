Shader "Custom/Common/FadeMaskRNoLight" 
{
	Properties
	{
		_MainTex("Base (RGB) Trans (A)", 2D) = "white" {}
		_Mask ("Mask (A)", 2D) = "white" {}
		_Color("Color", Color) = (1, 1, 1, 0.3)
	}
	SubShader
	{
		Tags{ "Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent" }
		LOD 100
		Pass
		{
			ZWrite On
			Cull Back
			Blend SrcAlpha OneMinusSrcAlpha
			
			CGPROGRAM
			//define
			#define CUTOUT
			#define MASKTEX
			#define NONORMAL
			#define SMALLCUTOUT
			//head
			#include "../Include/CommonHead_Include.cginc"
			//vertex&fragment
			#pragma vertex vert
			#pragma fragment frag 

			sampler2D _MainTex;
			fixed4 _Color;
			//custom frag fun
			fixed4 BasicColor(in v2f i, inout fixed4 mask)
			{
				fixed4 c = tex2D(_MainTex, i.uv);
				fixed4 m = tex2D(_Mask, i.uv);
				c.a = m.r*_Color.a;
				return c;
			}
			//include
			#include "../Include/CommonBasic_Include.cginc"
			ENDCG
		}
	}
}
