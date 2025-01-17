// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/UI/Additive"
{
    Properties
    {
        _MainTex ("Base (RGB)", 2D) = "white" {}
		_Mask ("Mask (A)", 2D) = "white" {}
		[HideInInspector]_ClipRange0("ClipRange", Vector) = (0.0, 0.0, 0.0, 0.0)
		[HideInInspector]_ClipArgs0("ClipArgs", Vector) = (1.0, 1.0, 0.0, 1.0)
    }

    SubShader
    {
		Tags{ "Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent" }
        LOD 100
		Cull Off ZWrite Off Fog{ Mode Off }
		Blend SrcAlpha One
        Pass
        {        
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
			#include "UI_Include.cginc"
            sampler2D _MainTex;
			sampler2D _Mask;
            float4 _MainTex_ST;

            struct appdata_t
            {
                float4 vertex : POSITION;
                half4 color : COLOR;
                float2 texcoord : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex : POSITION;
                half4 color : COLOR;
                float2 texcoord : TEXCOORD0;
				float2 worldPos : TEXCOORD1;
            };

            v2f vert (appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.color = v.color;
                o.texcoord = v.texcoord;
				o.worldPos = Clip1(v.vertex.xy);
                return o;
            }

            half4 frag (v2f IN) : COLOR
            {   
				return UI1ClipFade(UISampleFx(_MainTex, _Mask, IN.color, IN.texcoord), IN.worldPos);
            }
            ENDCG
        }
    }    
}