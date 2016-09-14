using UnityEngine;
using System.Collections;
[ExecuteInEditMode]
public class UseMaterialDepth : MonoBehaviour 
{
	public Material curMaterial;
	public Shader materialShader;
	// Use this for initialization
	void Start () 
	{
		if (curMaterial == null || curMaterial.shader.isSupported == false) 
		{  
			enabled = false;  
		}
	}

	void OnRenderImage (RenderTexture source, RenderTexture destination)
	{
		Graphics.BlitMultiTap(source, destination, curMaterial);
		// UnityStandardAssets.ImageEffects.BlitWithMaterial(curMaterial, source, destination);
	}

	void OnEnable() 
	{
		GetComponent<Camera>().depthTextureMode |= DepthTextureMode.Depth;        
	}

	// Update is called once per frame
	void Update () 
	{
		if(curMaterial==null)
			enabled=false;
	}
}
