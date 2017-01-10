Shader "Unlit/SkeleClip"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
		_Length("Length", float) = 1.0
		_LengthClipX("LengthClipX", float) = 0
		_LengthClipY("LengthClipY", float) = 0
		_ClipRange("ClipRangeA", Vector) = (0, 0, 0, 0)
	}
		SubShader
	{
		Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"  }
		LOD 100
		Cull Off
		ZWrite Off
		Blend One OneMinusSrcAlpha
		Lighting Off
		Pass
		{
			Blend SrcAlpha OneMinusSrcAlpha
			Offset 1, 1
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
			float _LengthClipX;
			float _LengthClipY;
			float _Length;
			vector _ClipRange;

			v2f vert(appdata v)
			{
				v2f o;
				v.vertex.x *= _Length;
				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				UNITY_TRANSFER_FOG(o, o.vertex);
				return o;
			}

			fixed4 frag(v2f i) : SV_Target
			{
				fixed4 col = tex2D(_MainTex, i.uv);
				float v = 0.5625 / 2;
				if(i.uv.x >= _ClipRange.x * v && i.uv.x <= _ClipRange.y * v)
				{
					col.a = 0.0;
				}
				if(i.uv.x >= _ClipRange.z * v && i.uv.x <= _ClipRange.w * v)
				{
					col.a = 0.0;
				}

				if(i.uv.x >= _LengthClipX * v && i.uv.x <= _LengthClipY * v)
				{
					col.a = 0.0;
				}
				return col;
			}
	ENDCG
	}
	}


}