// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/UI/Text"
{
	Properties
	{
		_MainTex ("Alpha (A)", 2D) = "white" {}
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
				half4 color : COLOR;
				float2 texcoord : TEXCOORD0;
			};

			struct v2f
			{
				float4 vertex : SV_POSITION;
				half4 color : TEXCOORD2;
				float2 texcoord : TEXCOORD0;
				float2 worldPos : TEXCOORD1;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;

			v2f vert (appdata_t v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.texcoord = v.texcoord;
				o.color = v.color;
				o.worldPos = Clip1(v.vertex.xy);
				return o;
			}

			fixed4 frag (v2f IN) : SV_Target
			{
				return UI1Clip(UISampleText(_MainTex, IN.color, IN.texcoord), IN.worldPos);
			}
			ENDCG
		}
	}
}
