Shader "Custom/MaskLayer" {
//	Properties {
//		_MainTex ("Base (RGB)", 2D) = "white" {}//目标图片，即需要被遮罩的图片
//		_MaskLayer("Culling Mask",2D) = "white"{}//混合的图片，设置为白色的图片，任何颜色与白色混合，其颜色不变
//		_Cutoff("Alpha cutoff",Range(0,1)) = 0
//	}
//	SubShader {
//		Tags { "Queue"="Transparent" }//渲染队列设置为  以从后往前的顺序渲染透明物体
////		Lighting off //关闭光照
////		ZWrite off //关闭深度缓存
//		Blend off //关闭混合
//		ZWrite off
//		Blend zero OneMinusSrcColor
//
//		AlphaTest GEqual[_Cutoff] //启用alpha测试
//		Cull Off
//
//		Pass{
//			SetTexture[_MaskLayer]{combine texture}//混合贴图
//			//混合贴图，previous为放置在前一序列这样在进行AlphaTest的时候会以这个图片为主来进行混合
//			SetTexture[_MainTex]{combine texture,previous}
//		} 
//	}
//}
	Properties {
		MainTex ("Base (RGB)", 2D) = "white" {}//目标图片，即需要被遮罩的图片
	}
	
	SubShader{
		//Tags { "Queue"="Geometry" }
		ColorMask 0
		Pass {
			//SetTexture[_MainTex]{combine previous, texture}
		}
	}
}
