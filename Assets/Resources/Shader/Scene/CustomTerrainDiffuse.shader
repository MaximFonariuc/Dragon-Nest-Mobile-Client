Shader "Custom/Scene/CustomTerrainDiffuse" 
{
	Properties
	{
		_Control("Control (RGBA)", 2D) = "red" {}
		_Splat3("Layer 3 (A)", 2D) = "black" {}
		_Splat2("Layer 2 (B)", 2D) = "black" {}
		_Splat1("Layer 1 (G)", 2D) = "black" {}
		_Splat0("Layer 0 (R)", 2D) = "black" {}
		_LightMap_ST("LightMap Scale Offset", Vector) = (1,1,1,1)
	}

	SubShader
	{
		Tags{ "Queue" = "Geometry-99" "RenderType" = "Opaque"}
		Pass
		{
			Tags { "LightMode" = "Always" }
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_fog
			#pragma multi_compile __ CUSTOM_SHADOW_ON 
			#pragma target 3.0
			#define INPUTLIGHTMAP
			#define TERRAIN
			#define TERRAIN_FIRSTPASS
			#define LM
			#include "../Include/SceneHead_Include.cginc"
			#include "../Include/Scene_Include.cginc"
			ENDCG
		}		
	}
}
