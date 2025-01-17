Shader "Custom/Common/CutoutDiffuseMaskRNoLight" 
{
	Properties 
	{
		_MainTex ("Base (RGB) Trans (A)", 2D) = "white" {}
		_Mask ("Mask (A)", 2D) = "white" {}
	}
	SubShader 
	{  
		Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="TransparentCutout"  }
		LOD 100            
		Pass 
		{   
			Cull Off
			CGPROGRAM 
			//define
			#define CUTOUT
			#define SMALLCUTOUT
			#define MASKTEX
			#define DEFAULTBASECOLOR
			#define NONORMAL
			//head
			#include "../Include/CommonHead_Include.cginc"
			//vertex&fragment
			#pragma vertex vert
			#pragma fragment frag 
			//include
			#include "../Include/CommonBasic_Include.cginc"
			ENDCG
		}  
	} 
}
