// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/UI/Mask" 
{
	Properties  
    {  
		_Color ("Main Color", Color) = (1,1,1,1)  
		_MainTex ("Base (RGB) Trans (A)", 2D) = "white" {}  
		_Mask ("Mask (A)", 2D) = "white" {}  
		[HideInInspector]_ClipRange0("ClipRange", Vector) = (0.0, 0.0, 0.0, 0.0)
		[HideInInspector]_ClipArgs0("ClipArgs", Vector) = (1.0, 1.0, 0.0, 1.0)
    }  
	Category  
	{
		Tags{ "Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent" }
		ZWrite Off Cull back Fog{ Mode Off }
		Blend SrcAlpha OneMinusSrcAlpha  
		SubShader
		{  
			Pass  
			{  
				CGPROGRAM  
				#pragma vertex vert  
				#pragma fragment frag  
				#include "UnityCG.cginc"
				#include "UI_Include.cginc"
				sampler2D _MainTex;  
				sampler2D _Mask;  
				fixed4 _Color;  
				struct appdata  
				{  
					float4 vertex : POSITION;  
					float4 texcoord : TEXCOORD0;  
				};  
				struct v2f  
				{  
					float4 pos : SV_POSITION;  
					float2 uv : TEXCOORD0;  
					float2 worldPos : TEXCOORD1;
				};  
				v2f vert (appdata v)  
				{  
					v2f o;  
					o.pos = UnityObjectToClipPos(v.vertex);  
					o.uv = v.texcoord.xy;  
					o.worldPos = Clip1(v.vertex.xy);
					return o;  
			}  
				half4 frag(v2f i) : COLOR  
				{  
					fixed4 final = tex2D(_MainTex, i.uv) * _Color;
					final.a *= 1 - tex2D(_Mask, i.uv).a;

					return UI1Clip(final, i.worldPos);
				}  
				ENDCG  
			}  
		}  
              
	}  
}
