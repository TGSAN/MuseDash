using UnityEngine;
using System.Collections;
using System;

namespace UnityStandardAssets.ImageEffects
{
	[ExecuteInEditMode]
	[RequireComponent (typeof(Camera))]
	[AddComponentMenu ("Image Effects/Blur/Overlay (Optimized)")]
	public class OverlayOptimized : MonoBehaviour {
		#region Variables
		public Shader curShader;
		public Texture2D blendTexture;
		public float blendOpacity = 1.0f;

		private Material material;
		#endregion

		void Start () {
			this.FindShaders ();
			this.CheckSupport ();
			this.CreateMaterials ();
		}

		void FindShaders () {
			if (!this.curShader) {
				this.curShader = Shader.Find ("Hidden/OverlayShader");
			}
		}
		
		void CreateMaterials() {
			if (!this.material) {
				this.material = new Material (this.curShader);
				this.material.hideFlags = HideFlags.HideAndDontSave;	
			}
		}
		
		bool Supported(){
			//return (SystemInfo.supportsImageEffects && SystemInfo.supportsRenderTextures && RadialBlurShader.isSupported);
			return true;
		}
		
		bool CheckSupport() {
			if (!this.Supported ()) {
				enabled = false;
				return false;
			}
			// rtFormat = SystemInfo.SupportsRenderTextureFormat (RenderTextureFormat.RGB565) ? RenderTextureFormat.RGB565 : RenderTextureFormat.Default;
			return true;
		}

		void Update () {
			this.blendOpacity = Mathf.Clamp(blendOpacity, 0.0f, 1.0f);
		}

		void OnRenderImage (RenderTexture sourceTexture, RenderTexture destTexture) {
			#if UNITY_EDITOR
			this.FindShaders ();
			this.CheckSupport ();
			this.CreateMaterials ();	
			#endif

			if (this.curShader != null) {
				this.material.SetTexture("_BlendTex", this.blendTexture);
				this.material.SetFloat("_Opacity", this.blendOpacity);
				
				Graphics.Blit(sourceTexture, destTexture, this.material);
			} else {
				Graphics.Blit(sourceTexture, destTexture);
			}
		}
		
		public void OnDisable () {
			// 设备上会跑到这里，毁掉材质后就没戏了
			//if (RadialBlurMaterial)
			//	DestroyImmediate (RadialBlurMaterial);
			// RadialBlurMaterial = null;
		}
	}
}