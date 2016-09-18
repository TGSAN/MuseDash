using UnityEngine;
using System.Collections;
using System;

namespace UnityStandardAssets.ImageEffects
{
	[ExecuteInEditMode]
	[RequireComponent (typeof(Camera))]
	[AddComponentMenu ("Image Effects/Blur/RadialBlur (Optimized)")]
	public class RadialBlurOptimized : MonoBehaviour {
		#region Variables
		private const float DIST_LIM = 0.15f;
		private const float STRENGTH_LIM = 2f;
		private const float EFFECT_GROW_STEP = 0.1f;

		public Shader RadialBlurShader = null;
		private Material RadialBlurMaterial = null;
		// private RenderTextureFormat rtFormat = RenderTextureFormat.Default;
		
		[Range(0.0f, 1.0f)]
		public float SampleDist = 0.17f;
		
		[Range(1.0f, 5.0f)]
		public float SampleStrength = 2.09f;

		#endregion

		void Start () {
			FindShaders ();
			CheckSupport ();
			CreateMaterials ();
		}

		void OnEnable() {
			this.SampleDist = EFFECT_GROW_STEP;
			this.SampleStrength = EFFECT_GROW_STEP;
		}
		
		void FindShaders () {
			if (!RadialBlurShader) {
				RadialBlurShader = Shader.Find ("Blue/ImageEffect/Unlit/RadialBlur");
			}
		}
		
		void CreateMaterials() {
			if (!RadialBlurMaterial) {
				RadialBlurMaterial = new Material (RadialBlurShader);
				RadialBlurMaterial.hideFlags = HideFlags.HideAndDontSave;	
			}
		}
		
		bool Supported(){
			return (SystemInfo.supportsImageEffects && SystemInfo.supportsRenderTextures && RadialBlurShader.isSupported);
			// return true;
		}
		
		bool CheckSupport() {
			if (!Supported ()) {
				enabled = false;
				return false;
			}
			// rtFormat = SystemInfo.SupportsRenderTextureFormat (RenderTextureFormat.RGB565) ? RenderTextureFormat.RGB565 : RenderTextureFormat.Default;
			return true;
		}

		private void EffectGrow() {
			this.SampleDist += EFFECT_GROW_STEP;
			this.SampleStrength += EFFECT_GROW_STEP;

			this.SampleDist = Math.Min (this.SampleDist, DIST_LIM);
			this.SampleStrength = Math.Min (this.SampleStrength, STRENGTH_LIM);
		}

		void OnRenderImage (RenderTexture sourceTexture, RenderTexture destTexture) {	
			#if UNITY_EDITOR
			FindShaders ();
			CheckSupport ();
			CreateMaterials ();	
			#endif

			if (this.SampleDist == 0f || this.SampleStrength == 0f) {
				Graphics.Blit (sourceTexture, destTexture);
				return;
			}

			this.EffectGrow ();

			int rtW = sourceTexture.width / 8;
			int rtH = sourceTexture.height / 8;
			
			
			RadialBlurMaterial.SetFloat ("_SampleDist", SampleDist);
			RadialBlurMaterial.SetFloat ("_SampleStrength", SampleStrength);	
			
			
			RenderTexture rtTempA = RenderTexture.GetTemporary (rtW, rtH, 0, RenderTextureFormat.Default);
			rtTempA.filterMode = FilterMode.Bilinear;
			
			Graphics.Blit (sourceTexture, rtTempA);
			
			RenderTexture rtTempB = RenderTexture.GetTemporary (rtW, rtH, 0, RenderTextureFormat.Default);
			rtTempB.filterMode = FilterMode.Bilinear;
			// RadialBlurMaterial.SetTexture ("_MainTex", rtTempA);
			Graphics.Blit (rtTempA, rtTempB, RadialBlurMaterial, 0);
			
			RadialBlurMaterial.SetTexture ("_BlurTex", rtTempB);
			Graphics.Blit (sourceTexture, destTexture, RadialBlurMaterial, 1);
			
			RenderTexture.ReleaseTemporary (rtTempA);
			RenderTexture.ReleaseTemporary (rtTempB);
		}
		
		public void OnDisable () {
			// 设备上会跑到这里，毁掉材质后就没戏了
			//if (RadialBlurMaterial)
			//	DestroyImmediate (RadialBlurMaterial);
			// RadialBlurMaterial = null;
		}
	}
}