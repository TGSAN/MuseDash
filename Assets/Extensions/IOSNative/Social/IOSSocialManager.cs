#define SOCIAL_API
////////////////////////////////////////////////////////////////////////////////
//  
// @module IOS Native Plugin for Unity3D 
// @author Osipov Stanislav (Stan's Assets) 
// @support stans.assets@gmail.com 
//
////////////////////////////////////////////////////////////////////////////////



using System;
using UnityEngine;
using System.Collections;
#if (UNITY_IPHONE && !UNITY_EDITOR && SOCIAL_API) || SA_DEBUG_MODE
using System.Runtime.InteropServices;
#endif

public class IOSSocialManager : ISN_Singleton<IOSSocialManager> {

	#if (UNITY_IPHONE && !UNITY_EDITOR && SOCIAL_API) || SA_DEBUG_MODE
	[DllImport ("__Internal")]
	private static extern void _ISN_TwPost(string text, string url, string encodedMedia);

	[DllImport ("__Internal")]
	private static extern void _ISN_FbPost(string text, string url, string encodedMedia);

	[DllImport ("__Internal")]
	private static extern void _ISN_MediaShare(string text, string encodedMedia);

	[DllImport ("__Internal")]
	private static extern void _ISN_SendMail(string subject, string body,  string recipients, string encodedMedia);


	[DllImport ("__Internal")]
	private static extern void _ISN_InstaShare(string encodedMedia, string message);


	[DllImport ("__Internal")]
	private static extern void _ISN_WP_ShareText(string message);

	[DllImport ("__Internal")]
	private static extern void _ISN_WP_ShareMedia(string encodedMedia);


	#endif
	


	//Actions

	public static event Action OnFacebookPostStart = delegate {};
	public static event Action OnTwitterPostStart = delegate {};
	public static event Action OnInstagramPostStart = delegate {};


	public static event Action<ISN_Result> OnFacebookPostResult = delegate {};
	public static event Action<ISN_Result> OnTwitterPostResult = delegate {};
	public static event Action<ISN_Result> OnInstagramPostResult = delegate {};
	public static event Action<ISN_Result> OnMailResult = delegate {};



	
	//--------------------------------------
	// INITIALIZE
	//--------------------------------------

	void Awake() {
		DontDestroyOnLoad(gameObject);
	}



	//--------------------------------------
	//  PUBLIC METHODS
	//--------------------------------------

	public void ShareMedia(string text) {
		ShareMedia(text, null);
	}

	public void ShareMedia(string text, Texture2D texture) {
		#if (UNITY_IPHONE && !UNITY_EDITOR && SOCIAL_API) || SA_DEBUG_MODE
			if(texture != null) {
				byte[] val = texture.EncodeToPNG();
				string bytesString = System.Convert.ToBase64String (val);
				_ISN_MediaShare(text, bytesString);
			} else {
				_ISN_MediaShare(text, "");
			}
		#endif
	}

	public void TwitterPost(string text, string url = null, Texture2D texture = null) {
		OnTwitterPostStart();
		#if (UNITY_IPHONE && !UNITY_EDITOR && SOCIAL_API) || SA_DEBUG_MODE
		if(text == null) {
			text = "";
		}

		if(url == null) {
			url = "";
		}

		string encodedMedia = "";

		if(texture != null) {
			byte[] val = texture.EncodeToPNG();
			encodedMedia = System.Convert.ToBase64String (val);
		}


		_ISN_TwPost(text, url, encodedMedia);
		#endif
	}



	public void FacebookPost(string text, string url = null, Texture2D texture = null) {
		OnFacebookPostStart();
		#if (UNITY_IPHONE && !UNITY_EDITOR && SOCIAL_API) || SA_DEBUG_MODE
		if(text == null) {
			text = "";
		}
		
		if(url == null) {
			url = "";
		}
		
		string encodedMedia = "";
		
		if(texture != null) {
			byte[] val = texture.EncodeToPNG();
			encodedMedia = System.Convert.ToBase64String (val);
		}
		
		
		_ISN_FbPost(text, url, encodedMedia);
		#endif
	}


	public void InstagramPost(Texture2D texture) {
		InstagramPost(texture, "");
	}
	
	
	public void InstagramPost(Texture2D texture, string message) {
		OnInstagramPostStart();
		#if (UNITY_IPHONE && !UNITY_EDITOR && SOCIAL_API) || SA_DEBUG_MODE
		
		byte[] val = texture.EncodeToPNG();
		string bytesString = System.Convert.ToBase64String (val);
		
		
		_ISN_InstaShare(bytesString, message);
		
		#endif
		
	}


	public void WhatsAppShareText(string message) {
		#if (UNITY_IPHONE && !UNITY_EDITOR && SOCIAL_API) || SA_DEBUG_MODE
		_ISN_WP_ShareText(message);
		#endif
	}


	public void WhatsAppShareImage(Texture2D texture) {

		#if (UNITY_IPHONE && !UNITY_EDITOR && SOCIAL_API) || SA_DEBUG_MODE

		byte[] val = texture.EncodeToPNG();
		string bytesString = System.Convert.ToBase64String (val);

		_ISN_WP_ShareMedia(bytesString);

		#endif
	}



	public void SendMail(string subject, string body, string recipients) {
		SendMail(subject, body, recipients, null);
	}
	
	public void SendMail(string subject, string body, string recipients, Texture2D texture) {
		if(texture == null) {
			#if (UNITY_IPHONE && !UNITY_EDITOR && SOCIAL_API) || SA_DEBUG_MODE
			_ISN_SendMail(subject, body, recipients, "");
			#endif
		} else {
			
			
			#if (UNITY_IPHONE && !UNITY_EDITOR && SOCIAL_API) || SA_DEBUG_MODE
			byte[] val = texture.EncodeToPNG();
			string bytesString = System.Convert.ToBase64String (val);
			_ISN_SendMail(subject, body, recipients, bytesString);
			#endif
		}
	}

	
	//--------------------------------------
	//  EVENTS
	//--------------------------------------

	private void OnTwitterPostFailed() {
		ISN_Result result = new ISN_Result(false);
		OnTwitterPostResult(result);
	}

	private void OnTwitterPostSuccess() {
		ISN_Result result = new ISN_Result(true);
		OnTwitterPostResult(result);
	}

	private void OnFacebookPostFailed() {
		ISN_Result result = new ISN_Result(false);
		OnFacebookPostResult(result);
	}
	
	private void OnFacebookPostSuccess() {
		ISN_Result result = new ISN_Result(true);
		OnFacebookPostResult(result);
	}

	private void OnMailFailed() {
		ISN_Result result = new ISN_Result(false);
		OnMailResult(result);
	}

	private void OnMailSuccess() {
		ISN_Result result = new ISN_Result(true);
		OnMailResult(result);
	}


	private void OnInstaPostSuccess() {
		ISN_Result result = new ISN_Result(true);
		OnInstagramPostResult(result);
	}
	
	
	private void OnInstaPostFailed(string data) {

		int code = System.Convert.ToInt32(data);

		ISN_Error error =  new ISN_Error(code, "Posting Failed");
		ISN_Result result = new ISN_Result(error);
		OnInstagramPostResult(result);

	}


	
	//--------------------------------------
	//  PRIVATE METHODS
	//--------------------------------------


	
	//--------------------------------------
	//  DESTROY
	//--------------------------------------

}
