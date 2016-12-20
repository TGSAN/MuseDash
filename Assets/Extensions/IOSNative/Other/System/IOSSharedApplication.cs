//#define SA_DEBUG_MODE
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
#if (UNITY_IPHONE && !UNITY_EDITOR) || SA_DEBUG_MODE
using System.Runtime.InteropServices;
#endif



public class IOSSharedApplication : ISN_Singleton<IOSSharedApplication> {


	public const string URL_SCHEME_EXISTS = "url_scheme_exists";
	public const string URL_SCHEME_NOT_FOUND  = "url_scheme_not_found";

	public static event Action<ISN_CheckUrlResult> OnUrlCheckResultAction = delegate {};
	public static event Action<string> OnAdvertisingIdentifierLoadedAction = delegate {};


	#if (UNITY_IPHONE && !UNITY_EDITOR) || SA_DEBUG_MODE
	
	[DllImport ("__Internal")]
	private static extern void _ISN_checkUrl(string url);

	[DllImport ("__Internal")]
	private static extern void _ISN_openUrl(string url);

	[DllImport ("__Internal")]
	private static extern void _ISN_GetIFA();
	

	
	#endif



	//--------------------------------------
	// Public Methods
	//--------------------------------------


	public void CheckUrl(string url) {
		#if (UNITY_IPHONE && !UNITY_EDITOR) || SA_DEBUG_MODE
		_ISN_checkUrl(url);
		#endif
	}


	public void OpenUrl(string url) {
		#if (UNITY_IPHONE && !UNITY_EDITOR) || SA_DEBUG_MODE
		_ISN_openUrl(url);
		#endif
	}


	public void GetAdvertisingIdentifier() {
		#if (UNITY_IPHONE && !UNITY_EDITOR) || SA_DEBUG_MODE
		_ISN_GetIFA();
		#endif
	}


	//--------------------------------------
	// Events
	//--------------------------------------


	private void UrlCheckSuccess(string url) {
		OnUrlCheckResultAction(new ISN_CheckUrlResult(url, true));
	}


	private void UrlCheckFailed(string url) {
		OnUrlCheckResultAction(new ISN_CheckUrlResult(url, false));
	}

	private void OnAdvertisingIdentifierLoaded(string Identifier) {
		OnAdvertisingIdentifierLoadedAction(Identifier);
	}



}
