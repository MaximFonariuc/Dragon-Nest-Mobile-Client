// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/SFX/AlphaBlendEmisPannerUV2" 
{
	Properties 
	{
		_Color ("Tint Color", Color) = (0.5,0.5,0.5,0.5)
		_EmisColor ("Emission Color", Color) = (1,1,1,1)
		_MainTex ("Particle Texture", 2D) = "white" {}
		_EmisTex ("Emission", 2D) = "black" {}
		_Mask ("r:Alpha g:FlowTex b:EmisMask", 2D) = "white" {}
		_parameter("R(EmisScale) G(NoiseAlpha) B(VC_Alpha) A(Alpha)",vector)=(1,1,1,1)
		_FlowDir("x:USpeed  x:VSpeed ",vector)=(0,0,1,1)
	}

	Category 
	{
		Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
		Blend SrcAlpha OneMinusSrcAlpha
		Lighting Off 
		 

		SubShader 
		{
			Pass 
			{
				CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
				#pragma multi_compile_fog 
				#include "UnityCG.cginc"

				sampler2D _MainTex;
				fixed4 _Color;
				fixed4 _EmisColor;
				sampler2D _Mask;
				sampler2D _EmisTex;
				half4 _parameter;
				half4 _FlowDir;

				struct appdata_t 
				{
					half4 vertex : POSITION;
					fixed4 color : COLOR;
					half2 texcoord : TEXCOORD0;
					half2 texcoord1 : TEXCOORD1;
				};

				struct v2f 
				{
					half4 vertex : POSITION;
					fixed4 color : COLOR;
					half2 texcoord : TEXCOORD0;
					half2 texcoord1 : TEXCOORD1;
					UNITY_FOG_COORDS(2)
				};
			
				float4 _MainTex_ST;
				float4 _Mask_ST;

				v2f vert (appdata_t v)
				{
					v2f o;
					o.vertex = UnityObjectToClipPos(v.vertex);
					o.color = v.color;
					o.texcoord = TRANSFORM_TEX(v.texcoord,_MainTex);
					o.texcoord1 = TRANSFORM_TEX(v.texcoord1,_Mask);
					UNITY_TRANSFER_FOG(o, o.vertex);
					return o;
				}

				fixed4 frag (v2f i) : SV_Target
				{				
				
					fixed4 Mask = tex2D(_Mask, i.texcoord);
					fixed4 Emis=tex2D(_EmisTex, i.texcoord);
					fixed flowLight=tex2D(_Mask, i.texcoord1+_FlowDir.xy*_Time.y).g;
					fixed4 col = tex2D(_MainTex, i.texcoord)*_Color + _parameter.r*_EmisColor  * _EmisColor.a * Mask.b*Emis*flowLight;
					col.a=i.color .a*_Color.a*Mask.r;
					UNITY_APPLY_FOG(i.fogCoord, col);
					return  col;
				}
				ENDCG 
			}
		}	
	}
}
