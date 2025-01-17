// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/UI/TextureList4" 
{
	Properties
	{
		_MainTex ("Base (RGB)", 2D) = "black" {}
		_UVScale("UV Scale", Vector) = (2.0, 0.5, -1, 0.5)
		_UVRange("UV Range", Vector) = (0.5,1,0,0)
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
				fixed4 color : COLOR;
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

			fixed4 frag(v2f i) : COLOR
			{
				float mask0 = InRange(i.texcoord, _UVRange);
				float mask1 = 1 - mask0;
				half2 uv0 = i.texcoord*_UVScale.xy*mask0;
				half2 uv1 = (i.texcoord*_UVScale.xy + _UVScale.zw)*mask1;
					
				fixed4 tex = UISample(_MainTex, i.color, uv0 + uv1);
				return UI1Clip(tex, i.worldPos);
			}
			ENDCG
		}
	}
}
