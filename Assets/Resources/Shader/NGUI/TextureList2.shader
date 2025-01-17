// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/UI/TextureList2" 
{
	Properties
	{
		_MainTex ("Base (RGB)", 2D) = "black" {}
		_MainTex1("Base (RGB)", 2D) = "black" {}
		_UVScale("UV Scale", Vector) = (-0.5, 0.0, 2, 1.0)
		_UVRange("UV Range", Vector) = (1.0, 1.0, 0.5, 0.0)
		[HideInInspector]_ClipRange0("ClipRange", Vector) = (0.0, 0.0, 0.0, 0.0)
		[HideInInspector]_ClipArgs0("ClipArgs", Vector) = (1.0, 1.0, 0.0, 1.0)
	}

	SubShader
	{
		Tags{ "Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent" }
		LOD 100
		
		Cull Off ZWrite Off Fog{ Mode Off }
		Blend SrcAlpha OneMinusSrcAlpha

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
				
			#include "UnityCG.cginc"
			#include "UI_Include.cginc"
			struct appdata_t
			{
				float4 vertex : POSITION;
				float2 texcoord : TEXCOORD0;
				fixed4 color : COLOR;
			};
	
			struct v2f
			{
				float4 vertex : SV_POSITION;
				half2 texcoord : TEXCOORD0;
				fixed4 color : TEXCOORD2;
				float2 worldPos : TEXCOORD1;
			};
	
        	sampler2D _MainTex;
			sampler2D _MainTex1;
			half4 _UVScale;
			half4 _UVRange;
			v2f vert (appdata_t v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.texcoord = v.texcoord;
				o.color = v.color;
				o.worldPos = Clip1(v.vertex.xy);
				return o;
			}
				
			fixed4 frag (v2f i) : SV_Target
			{
				half2 uv0 = i.texcoord*_UVScale.zw;
				half2 uv1 = (i.texcoord + _UVScale.xy)*_UVScale.zw;
				float maskY = InRange(i.texcoord, _UVRange);
				fixed4 tex1 = UISample(_MainTex1, i.color, uv1)*maskY;
				float maskX = 1 - maskY;
				fixed4 tex0 = UISample(_MainTex, i.color, uv0)*maskX;					
				return UI1Clip(tex0 + tex1, i.worldPos);
			}
			ENDCG
		}
	}
}
