﻿//#define SA_DEBUG_MODE
using UnityEngine;
using System;
using System.Collections;
#if (UNITY_IPHONE && !UNITY_EDITOR) || SA_DEBUG_MODE
using System.Runtime.InteropServices;
#endif



public class IOSNativeUtility : ISN_Singleton<IOSNativeUtility> {


	#if (UNITY_IPHONE && !UNITY_EDITOR) || SA_DEBUG_MODE
	[DllImport ("__Internal")]
	private static extern void _ISN_RedirectToAppStoreRatingPage(string appId);

	[DllImport ("__Internal")]
	private static extern void _ISN_ShowPreloader();
	
	[DllImport ("__Internal")]
	private static extern void _ISN_HidePreloader();


	[DllImport ("__Internal")]
	private static extern void _ISN_SetApplicationBagesNumber(int count);


	[DllImport ("__Internal")]
	private static extern void _ISN_GetLocale();

	[DllImport ("__Internal")]
	private static extern bool _ISN_IsGuidedAccessEnabled();


	[DllImport ("__Internal")]
	private static extern void _ISN_RequestGuidedAccessSession(bool enable);

	#endif
	public static event Action<ISN_Locale> OnLocaleLoaded = delegate {};
	public static event Action<bool> GuidedAccessSessionRequestResult = delegate {};


	void Awake() {
		DontDestroyOnLoad (gameObject);
	}


	public void GetLocale() {
		#if (UNITY_IPHONE && !UNITY_EDITOR) || SA_DEBUG_MODE
		_ISN_GetLocale();
		#endif
	}

	public static void RedirectToAppStoreRatingPage() {
		RedirectToAppStoreRatingPage(IOSNativeSettings.Instance.AppleId);
	}

	public static void RedirectToAppStoreRatingPage(string appleId) {
		#if (UNITY_IPHONE && !UNITY_EDITOR) || SA_DEBUG_MODE
			_ISN_RedirectToAppStoreRatingPage(appleId);
		#endif
	}

	public static void SetApplicationBagesNumber(int count) {
		#if (UNITY_IPHONE && !UNITY_EDITOR) || SA_DEBUG_MODE
			_ISN_SetApplicationBagesNumber(count);
		#endif
	}



	public static void ShowPreloader() {
		#if (UNITY_IPHONE && !UNITY_EDITOR) || SA_DEBUG_MODE
			_ISN_ShowPreloader();
		#endif
	}
	
	public static void HidePreloader() {
		#if (UNITY_IPHONE && !UNITY_EDITOR) || SA_DEBUG_MODE
			_ISN_HidePreloader();
		#endif
	}

	public void RequestGuidedAccessSession(bool enabled) {
		#if (UNITY_IPHONE && !UNITY_EDITOR) || SA_DEBUG_MODE
		_ISN_RequestGuidedAccessSession(enabled);
		#endif
	}

	//--------------------------------------
	//  Get / Set
	//--------------------------------------

	public bool IsGuidedAccessEnabled {
		get {
			#if (UNITY_IPHONE && !UNITY_EDITOR) || SA_DEBUG_MODE
			return _ISN_IsGuidedAccessEnabled();
			#else
			return false;
			#endif
		}
	}


	//--------------------------------------
	//  Handlers
	//--------------------------------------

	private void OnGuidedAccessSessionRequestResult(string data) {
		bool result = System.Convert.ToBoolean(data);
		GuidedAccessSessionRequestResult(result);
	}


	private void OnLocaleLoadedHandler(string data)  {
		string[] dataArray 		= data.Split(IOSNative.DATA_SPLITTER);
		string countryCode 		= dataArray[0];
		string contryName 		= dataArray[1];
		string languageCode 	= dataArray[2]; 
		string languageName  	= dataArray[3];

		ISN_Locale locale = new ISN_Locale (countryCode, contryName, languageCode, languageName);
		OnLocaleLoaded (locale);

		

	}


}
