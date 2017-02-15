﻿Shader "Custom/MaterialMask" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_Mask ("Culling Mask", 2D) = "white" {}
		_Cutoff ("Alpha cutoff", Range (0, 1)) = 0.1
	}
	
	SubShader {
		Tags {"Queue" = "Transparent"}
		Lighting Off
		ZWrite Off
		Blend SrcAlpha OneMinusSrcAlpha
		AlphaTest GEqual [_Cutoff]
		Pass {
			SetTexture [_Mask] {combine texture}
			SetTexture [_MainTex] {combine texture, previous}
		}
	}
}