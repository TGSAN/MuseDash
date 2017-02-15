﻿
Shader "Blue/ImageEffect/Unlit/RadialBlur" {
Properties {
	_MainTex ("Base (RGB)", 2D) = "white" {}
}


	CGINCLUDE

		#include "UnityCG.cginc"

		struct appdata_t {
			float4 vertex : POSITION;
			half2 texcoord : TEXCOORD0;
		};

		struct v2f {
			float4 vertex : SV_POSITION;
			half2 texcoord : TEXCOORD0;
		};

		sampler2D _MainTex;
		sampler2D _BlurTex;
		uniform float _SampleDist;
		uniform float _SampleStrength;
		
		v2f vert (appdata_t v)
		{
			v2f o;
			o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
			o.texcoord = v.texcoord;
			return o;
		}
		
		fixed4 fragRadialBlur (v2f i) : COLOR
		{
			fixed2 dir = 0.5-i.texcoord;
			fixed dist = length(dir);
			dir /= dist;
			dir *= _SampleDist;

			fixed4 sum = tex2D(_MainTex, i.texcoord - dir*0.01);
			sum += tex2D(_MainTex, i.texcoord - dir*0.02);
			sum += tex2D(_MainTex, i.texcoord - dir*0.03);
			sum += tex2D(_MainTex, i.texcoord - dir*0.05);
			sum += tex2D(_MainTex, i.texcoord - dir*0.08);
			sum += tex2D(_MainTex, i.texcoord + dir*0.01);
			sum += tex2D(_MainTex, i.texcoord + dir*0.02);
			sum += tex2D(_MainTex, i.texcoord + dir*0.03);
			sum += tex2D(_MainTex, i.texcoord + dir*0.05);
			sum += tex2D(_MainTex, i.texcoord + dir*0.08);
			sum *= 0.1;
			
			return sum;
		}

		fixed4 fragCombine (v2f i) : COLOR
		{
			// fixed2 dir = 0.5-i.texcoord;
			fixed dist = length(0.5-i.texcoord);
			fixed4  col = tex2D(_MainTex, i.texcoord);
			fixed4  blur = tex2D(_BlurTex, i.texcoord);
			col=lerp(col, blur,saturate(_SampleStrength*dist));
			return col;
		}
	ENDCG

	SubShader {
	  ZTest Always  ZWrite Off Cull Off Blend Off

	  Fog { Mode off } 
	//0  
	Pass { 
		CGPROGRAM
		
		#pragma vertex vert
		#pragma fragment fragRadialBlur
		#pragma fragmentoption ARB_precision_hint_fastest 
		
		ENDCG	 
		}	
	//1	
	Pass { 
		CGPROGRAM
		
		#pragma vertex vert
		#pragma fragment fragCombine
		#pragma fragmentoption ARB_precision_hint_fastest 
		
		ENDCG	 
		}
	}
}