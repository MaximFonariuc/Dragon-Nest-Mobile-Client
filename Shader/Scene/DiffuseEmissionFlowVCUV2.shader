Shader "Custom/Scene/DiffuseEmissionFlowVCUV2" 
{
	Properties 
	{
		_Color ("Main Color(RGB)   EmiAreaLumen(A)", Color) = (1,1,1,1)
		_MainTex ("Base (RGB) Gloss (A)", 2D) = "white" {}
		_Illum ("Illumin (R)", 2D) = "white" {}
		_EmissionColor ("(RGB)FlowLightColor  (A)ColorIntensity", Color) = (1,1,1,0.5)   
		_FlowDir("x:U Speed y:V Speed  z:FlowLightInte w:FlowLightPow",Vector)= (1.0,1.0,1.0,10)
	}	
	SubShader 
	{
		Tags { "RenderType"="Opaque" }
		LOD 100		
		Pass 
		{  
			//Moblie
			Tags { "LightMode" = "VertexLM" }
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#define VC
			#define EMISSION
			#include "../Include/SceneHead_Include.cginc"

			fixed3 EmissionColor(v2f i,fixed4 col,fixed4 mask)
			{
				return col.rgb*i.color.rgb*2;
			}
			#include "../Include/Scene_Include.cginc"	
			ENDCG
		}
	}	
}
