Shader "Unlit/Gradient"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
		_UVStartRamp("Start Point uv", Vector) = (0, 0, 0, 0)
		_Radius("Radius", float) = 1
		_Color0("Color 0", Color) = (0, 0, 0, 0)
		_Color1("Color 1", Color) = (0, 0, 0, 0)
		_Color2("Color 2", Color) = (0, 0, 0, 0)
		_Color3("Color 3", Color) = (0, 0, 0, 0)
		_X0("X 0", float) = -1
		_X1("X 1", float) = -1
		_X2("X 2", float) = -1
		_X3("X 3", float) = -1
	}
		SubShader
		{
			Tags { "RenderType" = "Opaque" }
			LOD 100

			Pass
			{
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
					UNITY_FOG_COORDS(1)
					float4 vertex : SV_POSITION;
				};

				sampler2D _MainTex;
				float4 _MainTex_ST;
				float4 _UVStartRamp;
				float4 _Color0, _Color1, _Color2, _Color3;
				float _X0, _X1, _X2, _X3;
				float _Radius;

				v2f vert(appdata v)
				{
					v2f o;
					o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
					o.uv = TRANSFORM_TEX(v.uv, _MainTex);
					UNITY_TRANSFER_FOG(o,o.vertex);
					return o;
				}

				fixed4 frag(v2f i) : SV_Target
				{
					fixed4 col = tex2D(_MainTex, i.uv);

				half offset = sqrt(pow(i.uv.x - _UVStartRamp.x, 2) + pow(i.uv.y - _UVStartRamp.y, 2));
				fixed4 color = fixed4(0, 0, 0, 1);
				if (offset <= _Radius)
				{
					if (offset >= _X0 && offset <= _X1)
					{
						color = lerp(_Color0, _Color1, (offset - _X0) / (_X1 - _X0));
					}
					else if (offset <= _X2 && _X2 != -1)
					{
						color = lerp(_Color1, _Color2, (offset - _X1) / (_X2 - _X1));
					}
					else if (offset <= _X3 && _X3 != -1)
					{
						color = lerp(_Color2, _Color3, (offset - _X2) / (_X3 - _X2));
					}
				}
				col *= color;
				return col;
			}
		ENDCG
		}
		}
}