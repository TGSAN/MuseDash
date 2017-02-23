// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'

// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'

Shader "Unlit/SkeleClip"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
		_ClipTex("Texture", 2D) = "white" {}
		_Length("Length", float) = 69.34
	}
		SubShader
	{
		Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"  }
		LOD 100
		Cull Off
		ZWrite Off
		Lighting Off
		Blend One OneMinusSrcAlpha

		Pass
		{
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
				float4 pos : POSITION1;
			};

			sampler2D _MainTex;
			sampler2D _ClipTex;
			float4 _MainTex_ST;
			float _Length;

			v2f vert(appdata v)
			{
				v2f o;
				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
				o.pos = v.vertex;
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				UNITY_TRANSFER_FOG(o, o.vertex);
				return o;
			}

			fixed4 frag(v2f i) : SV_Target
			{
				fixed4 col = tex2D(_MainTex, i.uv);
				float localX = i.pos.x;

				float x = localX / _Length;
				float4 mask = tex2D(_ClipTex, float2(x, 0.0));
				float maskValue = mask.r == 1.0 ? -1.0 : 1.0;
				clip(maskValue);
				return col;
			}
	ENDCG
	}
	}
}