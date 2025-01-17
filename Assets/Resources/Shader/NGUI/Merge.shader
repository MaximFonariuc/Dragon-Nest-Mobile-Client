// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/UI/Merge" 
{
	Properties
	{
		_MainTex0 ("Base0 (RGB)", 2D) = "white" {}
    	_Mask0 ("Mask0", 2D) = "white" {}
    	_MainTex1 ("Base1 (RGB)", 2D) = "white" {}
    	_Mask1 ("Mask1", 2D) = "white" {}
    	_MainTex2 ("Base2 (RGB)", 2D) = "white" {}
    	_Mask2 ("Mask2", 2D) = "white" {}
    	_MainTex3 ("Base3 (RGB)", 2D) = "white" {}
    	_Mask3 ("Mask3", 2D) = "white" {}
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
			#pragma target 2.0
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
				float2 texcoord : TEXCOORD0;
				fixed4 color : COLOR;
				half2 worldPos : TEXCOORD1;
				half4 m:TEXCOORD3;
				fixed  colMask:TEXCOORD4;

			};
	
        	sampler2D _MainTex0;
        	sampler2D _Mask0;
        	sampler2D _MainTex1;
        	sampler2D _Mask1;
        	sampler2D _MainTex2;
        	sampler2D _Mask2;
        	sampler2D _MainTex3;
        	sampler2D _Mask3;

        	float4 _MainTex0_ST;        		
			float4 _MainTex_ST;
			
			v2f vert (appdata_t v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex0);
				o.color = v.color;	
				float4 theRange = float4(1.0,1.0,0.0,0.0);		
									
				o.m.x = InRange(o.texcoord,theRange);
				theRange.xy += 2.0;
				theRange.zw += 2.0;
				o.m.y = InRange(o.texcoord,theRange);
				theRange.xy += 2.0;
				theRange.zw += 2.0;
				o.m.z = InRange(o.texcoord,theRange);
				theRange.xy += 2.0;
				theRange.zw += 2.0;
				o.m.w = InRange(o.texcoord,theRange);
					
				o.colMask = sign(v.color.r + v.color.g + v.color.b);

				o.worldPos = Clip1(v.vertex.xy);

				return o;
			}
				
			fixed4 GetSperateColor(sampler2D mainTex,sampler2D mask,v2f i)
			{
					
				fixed3 col = tex2D(mainTex,i.texcoord).rgb;
				float grey = Luminance(col.rgb);
				fixed4 final = fixed4(fixed3(grey, grey, grey)*(1 - i.colMask) + col*i.colMask*i.color.rgb, i.color.a);
				final.a *= tex2D(mask,i.texcoord).a;
				return final;
			}
				
			fixed4 frag (v2f i) : COLOR
			{
				//int index = int(i.t.x);
				i.texcoord = frac(i.texcoord);
				    
				fixed4 finalColor = fixed4(0.0,0.0,0.0,0.0);
				finalColor += GetSperateColor(_MainTex0,_Mask0,i) * i.m.x;
				finalColor += GetSperateColor(_MainTex1,_Mask1,i) * i.m.y;
				finalColor += GetSperateColor(_MainTex2,_Mask2,i) * i.m.z;
				finalColor += GetSperateColor(_MainTex3,_Mask3,i) * i.m.w;
				    
				return UI1Clip(finalColor, i.worldPos);
			}
			ENDCG
		}
	}
}
