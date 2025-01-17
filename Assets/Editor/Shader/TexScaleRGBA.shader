// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/Editor/TexScaleRGBA" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "black" {}
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
				half4 _MainTex_ST;
				v2f vert (appdata_t v)
				{
					v2f o = (v2f)0;

					o.vertex = UnityObjectToClipPos(v.vertex);
					o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
					return o;
				}

				fixed4 frag (v2f i) : SV_Target
				{				
					return tex2D(_MainTex, i.uv);
				}
				ENDCG 
			}
		}	
	}
}
