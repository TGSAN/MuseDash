Shader "Hidden/OverlayShader"
{
	Properties
	{
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_BlendTex ("Blend Texture", 2D) = "white" {}
		_Opacity ("Blend Opacity", Range(0.0, 1.0)) = 1.0
	}
	SubShader
	{
		// No culling or depth
		Cull Off ZWrite Off ZTest Always

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"
			
			uniform sampler2D _MainTex;
			uniform sampler2D _BlendTex;
			fixed _Opacity;

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
				o.uv = v.uv;
				return o;
			}

			fixed4 frag (v2f_img i) : COLOR
			{
				//Get the colors from the RenderTexture and the uv's
				//from the v2f_img struct
				fixed4 renderTex = tex2D(_MainTex, i.uv);
				fixed4 blendTex = tex2D(_BlendTex, i.uv);
				
				// Perform a multiply Blend mode
				fixed4 blendedMultiply = renderTex * blendTex;
				
				// Adjust amount of Blend Mode with a lerp
				renderTex = lerp(renderTex, blendedMultiply, _Opacity);
				
				return renderTex;
			}
			
			ENDCG
		}
	}
}
