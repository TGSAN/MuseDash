﻿Shader "Unlit/UICornerClip"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
		_MainColor("Color(RGB)", Color) = (1, 1, 1, 1)
		_Percent("Percent", float) = 1
		_TanValue("Tan Value", float) = 1
		_Offset("offset", Vector) = (0, 0, 0, 0)
	}
		SubShader
		{
			Tags
			{
			"RenderType" = "Opaque"
			"RenderType" = "Transparent"
			}
			LOD 100

			Pass
			{
				ZWrite off
				Blend SrcAlpha OneMinusSrcAlpha
				CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag

				#include "UnityCG.cginc"

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

				sampler2D _MainTex;
				float4 _MainTex_ST;
				float _Percent, _TanValue;
				float4 _MainColor;
				float4 _Offset;

				v2f vert(appdata v)
				{
					v2f o;
					o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
					o.uv = TRANSFORM_TEX(v.uv, _MainTex);
					return o;
				}

				fixed4 frag(v2f i) : SV_Target
				{
					fixed4 col = tex2D(_MainTex, i.uv);
					if ((i.uv.x + _Offset.x) * _TanValue + (i.uv.y + _Offset.y) / _TanValue >= 1 / _TanValue - _Percent)
					{
					}
					else
					{
						col.a = 0.0;
					}
					return col;
				}
				ENDCG
			}
		}
}