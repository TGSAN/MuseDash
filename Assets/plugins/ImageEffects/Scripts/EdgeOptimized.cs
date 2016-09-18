/*************
** author:esfog
/************/
using UnityEngine;
using System.Collections;

public class EdgeOptimized : MonoBehaviour {
	//遮挡描边颜色
	public Color OutLineColor = Color.green;
	
	private GameObject _cameraGameObject;
	//摄像机(专门用来处理遮挡描边的)
	private Camera _camera;
	private Camera _mainCamera;
	//所需的RenderTexture
	private RenderTexture _renderTextureDepth;
	private RenderTexture _renderTextureOcclusion;
	private RenderTexture _renderTextureStretch;
	
	//临时材质
	private Material _materialOcclusion;
	private Material _materialStretch;
	private Material _materialMix;
	//用来处理玩家深度的Shader
	private Shader _depthShader;
	
	// 相关初始化
	void Start () {
		_mainCamera = Camera.main;
		_mainCamera.depthTextureMode = DepthTextureMode.Depth;
		_depthShader = Shader.Find ("Esfog/OutLine/Depth");
		
		_cameraGameObject = new GameObject ();
		_cameraGameObject.transform.parent = _mainCamera.transform;
		_cameraGameObject.transform.localPosition = Vector3.zero;
		_cameraGameObject.transform.localScale = Vector3.one;
		_cameraGameObject.transform.localRotation = Quaternion.identity;
		
		_camera = _cameraGameObject.AddComponent<Camera> ();
		_camera.aspect = _mainCamera.aspect;
		_camera.fieldOfView = _mainCamera.fieldOfView;
		_camera.orthographic = false;
		_camera.nearClipPlane = _mainCamera.nearClipPlane;
		_camera.farClipPlane = _mainCamera.farClipPlane;
		_camera.rect = _mainCamera.rect;
		_camera.depthTextureMode = DepthTextureMode.None;
		_camera.cullingMask = 1 << (int)LayerMask.NameToLayer ("Main Player");
		_camera.enabled = false;
		_materialOcclusion = new Material (Shader.Find ("Esfog/OutLine/Occlusion"));
		_materialStretch = new Material (Shader.Find ("Esfog/OutLine/Stretch"));
		_materialMix = new Material (Shader.Find ("Esfog/OutLine/Mix"));
		Shader.SetGlobalColor ("_OutLineColor", OutLineColor);
		if (!_depthShader.isSupported || !_materialMix.shader.isSupported || !_materialMix.shader.isSupported || !_materialOcclusion.shader.isSupported) {
			return;
		}
	}
	
	void OnRenderImage(RenderTexture source,RenderTexture destination) {
		_renderTextureDepth = RenderTexture.GetTemporary (Screen.width, Screen.height, 24, RenderTextureFormat.Depth);
		_renderTextureOcclusion = RenderTexture.GetTemporary (Screen.width, Screen.height, 0);
		_renderTextureStretch = RenderTexture.GetTemporary (Screen.width, Screen.height, 0);
		_camera.targetTexture = _renderTextureDepth;
		
		_camera.fieldOfView = _mainCamera.fieldOfView;
		_camera.aspect = _mainCamera.aspect;
		_camera.RenderWithShader (_depthShader, string.Empty);
		
		//对比我们为角色生成的RenderTexture和主摄像机自身的深度缓冲区,计算出角色的哪些区域被挡住了
		Graphics.Blit (_renderTextureDepth, _renderTextureOcclusion, _materialOcclusion);
		var screenSize = new Vector4 (1.0f / Screen.width, 1.0f / Screen.height, 0.0f, 0.0f);
		
		_materialStretch.SetVector ("_ScreenSize", screenSize);
		Graphics.Blit (_renderTextureOcclusion, _renderTextureStretch, _materialStretch, 0);
		Graphics.Blit (_renderTextureStretch, _renderTextureStretch, _materialStretch, 1);
		
		_materialMix.SetTexture ("_OcclusionTex", _renderTextureOcclusion);
		_materialMix.SetTexture ("_StretchTex", _renderTextureStretch);
		Graphics.Blit (source, destination, _materialMix);
		
		RenderTexture.ReleaseTemporary (_renderTextureDepth);
		RenderTexture.ReleaseTemporary (_renderTextureOcclusion);
		RenderTexture.ReleaseTemporary (_renderTextureStretch);
	}
}