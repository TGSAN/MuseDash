#define CAMERA_API
////////////////////////////////////////////////////////////////////////////////
//  
// @module IOS Native Plugin for Unity3D 
// @author Osipov Stanislav (Stan's Assets) 
// @support stans.assets@gmail.com 
//
////////////////////////////////////////////////////////////////////////////////




using UnityEngine;
using System;
using System.Collections;
#if (UNITY_IPHONE && !UNITY_EDITOR && CAMERA_API) || SA_DEBUG_MODE
using System.Runtime.InteropServices;
#endif


public class IOSCamera : ISN_Singleton<IOSCamera> {


	//Actions
	public static event Action<IOSImagePickResult> OnImagePicked = delegate{};
	public static event Action<ISN_Result> OnImageSaved = delegate{};
	public static event Action<string> OnVideoPathPicked = delegate{};


	private bool IsWaitngForResponce = false;



	#if (UNITY_IPHONE && !UNITY_EDITOR && CAMERA_API) || SA_DEBUG_MODE

	[DllImport ("__Internal")]
	private static extern void _ISN_SaveToCameraRoll(string encodedMedia);


	[DllImport ("__Internal")]
	private static extern void _ISN_GetVideoPathFromAlbum();

	[DllImport ("__Internal")]
	private static extern void _ISN_PickImage(int source);

	
	[DllImport ("__Internal")]
	private static extern void _ISN_InitCameraAPI(float compressionRate, int maxSize, int encodingType);


	#endif


	void Awake() {
		DontDestroyOnLoad(gameObject);

		#if (UNITY_IPHONE && !UNITY_EDITOR && CAMERA_API) || SA_DEBUG_MODE
		_ISN_InitCameraAPI(IOSNativeSettings.Instance.JPegCompressionRate, IOSNativeSettings.Instance.MaxImageLoadSize, (int) IOSNativeSettings.Instance.GalleryImageFormat);
		#endif
	}



	public void SaveTextureToCameraRoll(Texture2D texture) {
		#if (UNITY_IPHONE && !UNITY_EDITOR && CAMERA_API) || SA_DEBUG_MODE
		if(texture != null) {
			byte[] val = texture.EncodeToPNG();
			string bytesString = System.Convert.ToBase64String (val);
			_ISN_SaveToCameraRoll(bytesString);
		} 
		#endif
	}


	public void SaveScreenshotToCameraRoll() {
		StartCoroutine(SaveScreenshot());
	}

	public void GetVideoPathFromAlbum() {
		#if (UNITY_IPHONE && !UNITY_EDITOR && CAMERA_API) || SA_DEBUG_MODE
		_ISN_GetVideoPathFromAlbum();
		#endif
	}

	[Obsolete("GetImageFromAlbum is deprecated, please use PickImage(ISN_ImageSource.Album) ")]
	public void GetImageFromAlbum() {
		PickImage(ISN_ImageSource.Album);
	}

	[Obsolete("GetImageFromCamera is deprecated, please use PickImage(ISN_ImageSource.Camera) ")]
	public void GetImageFromCamera() {
		PickImage(ISN_ImageSource.Camera);
	}

	public void PickImage(ISN_ImageSource source) {
		if(IsWaitngForResponce) {
			return;
		}
		IsWaitngForResponce = true;

		#if (UNITY_IPHONE && !UNITY_EDITOR && CAMERA_API) || SA_DEBUG_MODE
		_ISN_PickImage((int) source);
		#endif
	}



	private void OnImagePickedEvent(string data) {

		IsWaitngForResponce = false;

		IOSImagePickResult result =  new IOSImagePickResult(data);
		OnImagePicked(result);


	}

	private void OnImageSaveFailed() {
		ISN_Result result =  new ISN_Result(false);

		OnImageSaved(result);
	}

	private void OnImageSaveSuccess() {
		ISN_Result result =  new ISN_Result(true);

		OnImageSaved(result);
	}

	private void OnVideoPickedEvent(string path) {
		OnVideoPathPicked(path);
	}

	
	private IEnumerator SaveScreenshot() {
		
		yield return new WaitForEndOfFrame();
		// Create a texture the size of the screen, RGB24 format
		int width = Screen.width;
		int height = Screen.height;
		Texture2D tex = new Texture2D( width, height, TextureFormat.RGB24, false );
		// Read screen contents into the texture
		tex.ReadPixels( new Rect(0, 0, width, height), 0, 0 );
		tex.Apply();
		
		SaveTextureToCameraRoll(tex);
		
		Destroy(tex);
		
	}
}
