// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/Editor/CompactSplatTex" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "black" {}
		_Tex1("Base (RGB)", 2D) = "black" {}
		}
		Category {
		Tags { "RenderType" = "Opaque" }
		Cull off Lighting Off ZWrite Off Fog{ Mode Off }
		ColorMask RGBA
		SubShader {
			Pass {
		
				CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag

				#include "UnityCG.cginc"

			
				struct appdata_t {
					half4 vertex : POSITION;		
					half2 texcoord : TEXCOORD0;
				};

				struct v2f {
					half4 vertex : SV_POSITION;				
					half2 uv : TEXCOORD0;
				};
			
				sampler2D _MainTex;
				sampler2D _Tex1;
				v2f vert (appdata_t v)
				{
					v2f o = (v2f)0;

					o.vertex = UnityObjectToClipPos(v.vertex);
					o.uv = v.texcoord;
					return o;
				}

				fixed4 frag (v2f i) : SV_Target
				{				
					float4 mainColor = tex2D(_MainTex, i.uv);
					float4 texColor = tex2D(_Tex1, i.uv);
					mainColor.r = mainColor.r*0.5f + texColor.r * 0.5f;
					mainColor.g = mainColor.g*0.5f + texColor.g * 0.5f;
					mainColor.b = mainColor.b*0.5f + texColor.b * 0.5f;
					mainColor.a = mainColor.a*0.5f + texColor.a * 0.5f;
					return mainColor;
				}
				ENDCG 
			}
		}	
	}
}
