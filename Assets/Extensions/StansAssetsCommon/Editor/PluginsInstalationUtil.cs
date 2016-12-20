using UnityEngine;
using UnityEditor;
using System.Collections;

public class PluginsInstalationUtil : MonoBehaviour {
	
	
	public const string ANDROID_SOURCE_PATH       = "Plugins/StansAssets/Android/";
	public const string ANDROID_DESTANATION_PATH  = "Plugins/Android/";
	
	
	public const string IOS_SOURCE_PATH       = "Plugins/StansAssets/IOS/";
	public const string IOS_DESTANATION_PATH  = "Plugins/IOS/";
	
	
	
	
	
	public static void IOS_UpdatePlugin() {
		IOS_InstallPlugin(false);
	}
	
	public static void IOS_InstallPlugin(bool IsFirstInstall = true) {
		
		IOS_CleanUp();
		
		
		
		
		
		//IOS Native
		FileStaticAPI.CopyFile(PluginsInstalationUtil.IOS_SOURCE_PATH + "ISN_Camera.mm.txt", 		PluginsInstalationUtil.IOS_DESTANATION_PATH + "ISN_Camera.mm");
		FileStaticAPI.CopyFile(PluginsInstalationUtil.IOS_SOURCE_PATH + "ISN_GameCenter.mm.txt", 	PluginsInstalationUtil.IOS_DESTANATION_PATH + "ISN_GameCenter.mm");
		FileStaticAPI.CopyFile(PluginsInstalationUtil.IOS_SOURCE_PATH + "ISN_iAd.mm.txt", 			PluginsInstalationUtil.IOS_DESTANATION_PATH + "ISN_iAd.mm");
		FileStaticAPI.CopyFile(PluginsInstalationUtil.IOS_SOURCE_PATH + "ISN_InApp.mm.txt", 		PluginsInstalationUtil.IOS_DESTANATION_PATH + "ISN_InApp.mm");
		FileStaticAPI.CopyFile(PluginsInstalationUtil.IOS_SOURCE_PATH + "ISN_Media.mm.txt", 		PluginsInstalationUtil.IOS_DESTANATION_PATH + "ISN_Media.mm");
		FileStaticAPI.CopyFile(PluginsInstalationUtil.IOS_SOURCE_PATH + "ISN_ReplayKit.mm.txt", 	PluginsInstalationUtil.IOS_DESTANATION_PATH + "ISN_ReplayKit.mm");
		FileStaticAPI.CopyFile(PluginsInstalationUtil.IOS_SOURCE_PATH + "ISN_CloudKit.mm.txt", 		PluginsInstalationUtil.IOS_DESTANATION_PATH + "ISN_CloudKit.mm");
		FileStaticAPI.CopyFile(PluginsInstalationUtil.IOS_SOURCE_PATH + "ISN_NSData+Base64.h.txt", 	PluginsInstalationUtil.IOS_DESTANATION_PATH + "ISN_NSData+Base64.h");
		FileStaticAPI.CopyFile(PluginsInstalationUtil.IOS_SOURCE_PATH + "ISN_NSData+Base64.m.txt", 	PluginsInstalationUtil.IOS_DESTANATION_PATH + "ISN_NSData+Base64.m");
		
		
		IOS_Install_SocialPart();
		InstallGMAPart();
		
		
		
	}
	
	public static void InstallGMAPart() {
		//GMA
		FileStaticAPI.CopyFile(PluginsInstalationUtil.IOS_SOURCE_PATH + "GMA_SA_Lib_Proxy.mm.txt", 	PluginsInstalationUtil.IOS_DESTANATION_PATH + "GMA_SA_Lib_Proxy.mm");
		FileStaticAPI.CopyFile(PluginsInstalationUtil.IOS_SOURCE_PATH + "GMA_SA_Lib.h.txt", 	PluginsInstalationUtil.IOS_DESTANATION_PATH + "GMA_SA_Lib.h");
		FileStaticAPI.CopyFile(PluginsInstalationUtil.IOS_SOURCE_PATH + "GMA_SA_Lib.m.txt", 	PluginsInstalationUtil.IOS_DESTANATION_PATH + "GMA_SA_Lib.m");
		
	}
	
	
	public static void IOS_Install_SocialPart() {
		//IOS Native +  MSP
		FileStaticAPI.CopyFile(PluginsInstalationUtil.IOS_SOURCE_PATH + "ISN_SocialGate.mm.txt", 	PluginsInstalationUtil.IOS_DESTANATION_PATH + "ISN_SocialGate.mm");
		FileStaticAPI.CopyFile(PluginsInstalationUtil.IOS_SOURCE_PATH + "ISN_NativeCore.h.txt", 	PluginsInstalationUtil.IOS_DESTANATION_PATH + "ISN_NativeCore.h");
		FileStaticAPI.CopyFile(PluginsInstalationUtil.IOS_SOURCE_PATH + "ISN_NativeCore.mm.txt", 	PluginsInstalationUtil.IOS_DESTANATION_PATH + "ISN_NativeCore.mm");
	}




	public static void Remove_FB_SDK_WithDialog() {
		bool result = EditorUtility.DisplayDialog(
			"Removing Facebook SDK",
			"Are you sure you want to remove Facebook OAuth API?",
			"Remove",
			"Cansel");
		
		if(result) {
			Remove_FB_SDK();
		}
	}
	public static void Remove_FB_SDK() {


		FileStaticAPI.DeleteFolder(PluginsInstalationUtil.ANDROID_DESTANATION_PATH + "facebook");
		FileStaticAPI.DeleteFolder("Plugins/facebook", false);
		FileStaticAPI.DeleteFolder("Facebook", false);

		//MSP
		FileStaticAPI.DeleteFile("Extensions/MobileSocialPlugin/Example/Scripts/MSPFacebookUseExample.cs", false);
		FileStaticAPI.DeleteFile("Extensions/MobileSocialPlugin/Example/Scripts/MSP_FacebookAnalyticsExample.cs", false);
		FileStaticAPI.DeleteFile("Extensions/MobileSocialPlugin/Example/Scripts/MSP_FacebookAndroidTurnBasedAndGiftsExample.cs", false);

		//FB v7
		FileStaticAPI.DeleteFolder("Examples", false);
		FileStaticAPI.DeleteFolder(PluginsInstalationUtil.IOS_DESTANATION_PATH + "Facebook", false);


		FileStaticAPI.DeleteFolder(PluginsInstalationUtil.ANDROID_DESTANATION_PATH + "libs/bolts-android-1.2.0.jar");
		FileStaticAPI.DeleteFolder(PluginsInstalationUtil.ANDROID_DESTANATION_PATH + "libs/facebook-android-sdk-4.7.0.jar");
		FileStaticAPI.DeleteFolder(PluginsInstalationUtil.ANDROID_DESTANATION_PATH + "libs/facebook-android-wrapper-release.jar");

		AssetDatabase.Refresh();
	}


	private static string AN_SoomlaGrowContent = "Extensions/AndroidNative/Other/Soomla/AN_SoomlaGrow.cs";
	public static void DisableSoomlaFB() {
		ChnageDefineState(AN_SoomlaGrowContent, "FACEBOOK_ENABLED", false);
	}



	

	private static void ChnageDefineState(string file, string tag, bool IsEnabled) {

		if(!FileStaticAPI.IsFileExists(file)) {
			Debug.Log("ChnageDefineState for tag: " + tag + " File not found at path: " + file);
			return;
		}
		
		string content = FileStaticAPI.Read(file);
		
		int endlineIndex;
		endlineIndex = content.IndexOf(System.Environment.NewLine);
		if(endlineIndex == -1) {
			endlineIndex = content.IndexOf("\n");
		}
		
		string TagLine = content.Substring(0, endlineIndex);
		
		if(IsEnabled) {
			content 	= content.Replace(TagLine, "#define " + tag);
		} else {
			content 	= content.Replace(TagLine, "//#define " + tag);
		}
		
		FileStaticAPI.Write(file, content);
		
	}
	
	
	public static void IOS_CleanUp() {
		
		
		//Old APi
		RemoveIOSFile("AppEventListener");
		RemoveIOSFile("CloudManager");
		RemoveIOSFile("CustomBannerView");
		RemoveIOSFile("GameCenterManager");
		RemoveIOSFile("GCHelper");
		RemoveIOSFile("iAdBannerController");
		RemoveIOSFile("iAdBannerObject");
		RemoveIOSFile("InAppPurchaseManager");
		RemoveIOSFile("IOSGameCenterManager");
		RemoveIOSFile("IOSNativeNotificationCenter");
		RemoveIOSFile("IOSNativePopUpsManager");
		RemoveIOSFile("IOSNativeUtility");
		RemoveIOSFile("ISN_NSData+Base64");
		RemoveIOSFile("ISN_Reachability");
		RemoveIOSFile("ISNCamera");
		RemoveIOSFile("ISNDataConvertor");
		RemoveIOSFile("ISNSharedApplication");
		RemoveIOSFile("ISNVideo");
		RemoveIOSFile("PopUPDelegate");
		RemoveIOSFile("RatePopUPDelegate");
		RemoveIOSFile("SKProduct+LocalizedPrice");
		RemoveIOSFile("SocialGate");
		RemoveIOSFile("StoreProductView");
		RemoveIOSFile("TransactionServer");
		
		RemoveIOSFile("OneSignalUnityRuntime");
		RemoveIOSFile("OneSignal");
		RemoveIOSFile("libOneSignal");
		RemoveIOSFile("ISN_Security");
		RemoveIOSFile("ISN_NativeUtility");
		RemoveIOSFile("ISN_NativePopUpsManager");
		RemoveIOSFile("ISN_Media");
		RemoveIOSFile("ISN_GameCenterTBM");
		RemoveIOSFile("ISN_GameCenterRTM");
		RemoveIOSFile("ISN_GameCenterManager");
		RemoveIOSFile("ISN_GameCenterListner");
		RemoveIOSFile("IOSNativeNotificationCenter");
		
		
		
		//New API
		RemoveIOSFile("ISN_Camera");
		RemoveIOSFile("ISN_GameCenter");
		RemoveIOSFile("ISN_InApp");
		RemoveIOSFile("ISN_iAd");
		RemoveIOSFile("ISN_NativeCore");
		RemoveIOSFile("ISN_SocialGate");
		RemoveIOSFile("ISN_ReplayKit");
		RemoveIOSFile("ISN_CloudKit");
		RemoveIOSFile("ISN_Soomla");
		
		
		
		
		//Google Ad old v1
		RemoveIOSFile("GADAdMobExtras");
		RemoveIOSFile("GADAdNetworkExtras");
		RemoveIOSFile("GADAdSize");
		RemoveIOSFile("GADBannerViewDelegate");
		RemoveIOSFile("GADInAppPurchase");
		RemoveIOSFile("GADInAppPurchaseDelegate");
		RemoveIOSFile("GADInterstitialDelegate");
		RemoveIOSFile("GADModules");
		RemoveIOSFile("GADRequest");
		RemoveIOSFile("GADRequestError");
		RemoveIOSFile("libGoogleAdMobAds");
		
		//Google Ad old v2
		RemoveIOSFile("GoogleMobileAdBanner");
		RemoveIOSFile("GoogleMobileAdController");
		
		
		//Google Ad new
		RemoveIOSFile("GMA_SA_Lib");
		
		
		//MSP old
		RemoveIOSFile("IOSInstaPlugin");
		RemoveIOSFile("IOSTwitterPlugin");
		RemoveIOSFile("MGInstagram");
		
		
		
		
		
	}
	
	
	public static void RemoveIOSFile(string filename) {
		FileStaticAPI.DeleteFile(IOS_DESTANATION_PATH + filename + ".h");
		FileStaticAPI.DeleteFile(IOS_DESTANATION_PATH + filename + ".m");
		FileStaticAPI.DeleteFile(IOS_DESTANATION_PATH + filename + ".mm");
		FileStaticAPI.DeleteFile(IOS_DESTANATION_PATH + filename + ".a");
	}
	
	
	public static void Android_UpdatePlugin() {
		Android_InstallPlugin(false);
	}
	
	
	
	public static void EnableGooglePlayAPI() {
		FileStaticAPI.CopyFile(ANDROID_SOURCE_PATH + "google_play/an_googleplay.txt", 	ANDROID_DESTANATION_PATH + "libs/an_googleplay.jar");
		FileStaticAPI.CopyFile(ANDROID_SOURCE_PATH + "google_play/google-play-services.txt", 	ANDROID_DESTANATION_PATH + "libs/google-play-services.jar");
	}
	
	public static void DisableGooglePlayAPI() {
		FileStaticAPI.DeleteFile(ANDROID_DESTANATION_PATH + "libs/google-play-services.jar");
		FileStaticAPI.DeleteFile(ANDROID_DESTANATION_PATH + "libs/an_googleplay.jar");
	}
	
	
	public static void EnableAppLicensingAPI() {
		FileStaticAPI.CopyFile(ANDROID_SOURCE_PATH + "app_licensing/an_licensing_library.txt", 	ANDROID_DESTANATION_PATH + "libs/an_licensing_library.jar");
	}
	
	
	public static void DisableAppLicensingAPI() {
		FileStaticAPI.DeleteFile(ANDROID_DESTANATION_PATH + "libs/an_licensing_library.jar");
	}


	public static void EnableSoomlaAPI() {
		FileStaticAPI.CopyFile(ANDROID_SOURCE_PATH + "libs/an_sa_soomla.txt", 	ANDROID_DESTANATION_PATH + "libs/an_sa_soomla.jar");
	}
	
	
	public static void DisableSoomlaAPI() {
		FileStaticAPI.DeleteFile(ANDROID_DESTANATION_PATH + "libs/an_sa_soomla.jar");
	}

	
	
	public static void EnableBillingAPI() {
		FileStaticAPI.CopyFile(ANDROID_SOURCE_PATH + "billing/an_billing.txt", 	ANDROID_DESTANATION_PATH + "libs/an_billing.jar");
	}
	
	public static void DisableBillingAPI() {
		FileStaticAPI.DeleteFile(ANDROID_DESTANATION_PATH + "libs/an_billing.jar");
	}
	
	
	
	
	public static void EnableSocialAPI() {
		FileStaticAPI.CopyFile(ANDROID_SOURCE_PATH + "social/an_social.txt", 	ANDROID_DESTANATION_PATH + "libs/an_social.jar");
		FileStaticAPI.CopyFile(ANDROID_SOURCE_PATH + "social/twitter4j-core-3.0.5.txt", 	ANDROID_DESTANATION_PATH + "libs/twitter4j-core-3.0.5.jar");
	}
	
	public static void DisableSocialAPI() {
		FileStaticAPI.DeleteFile(ANDROID_DESTANATION_PATH + "libs/an_social.jar");
		FileStaticAPI.DeleteFile(ANDROID_DESTANATION_PATH + "libs/twitter4j-core-3.0.5.jar");
	}
	
	
	
	
	
	
	public static void EnableCameraAPI() {
		//Unity 5 upgdare:
		FileStaticAPI.CopyFile(ANDROID_SOURCE_PATH + "libs/image-chooser-library-1.3.0.txt", 	ANDROID_DESTANATION_PATH + "libs/image-chooser-library-1.3.0.jar");
	}
	
	public static void DisableCameraAPI() {
		FileStaticAPI.DeleteFile(ANDROID_DESTANATION_PATH + "libs/image-chooser-library-1.3.0.jar");
	}
	
	
	
	
	
	public static void Android_InstallPlugin(bool IsFirstInstall = true) {
		
		
		//Unity 5 upgdare:
		FileStaticAPI.DeleteFile(ANDROID_SOURCE_PATH + "libs/httpclient-4.3.1.jar");
		FileStaticAPI.DeleteFile(ANDROID_SOURCE_PATH + "libs/signpost-commonshttp4-1.2.1.2.jar");
		FileStaticAPI.DeleteFile(ANDROID_SOURCE_PATH + "libs/signpost-core-1.2.1.2.jar");
		FileStaticAPI.DeleteFile(ANDROID_SOURCE_PATH + "libs/libGoogleAnalyticsServices.jar");
		
		FileStaticAPI.DeleteFile(ANDROID_SOURCE_PATH + "libs/android-support-v4.jar");
		FileStaticAPI.DeleteFile(ANDROID_SOURCE_PATH + "libs/image-chooser-library-1.3.0.jar");
		FileStaticAPI.DeleteFile(ANDROID_SOURCE_PATH + "libs/twitter4j-core-3.0.5.jar");
		FileStaticAPI.DeleteFile(ANDROID_SOURCE_PATH + "libs/google-play-services.jar");
		
		
		FileStaticAPI.DeleteFile(ANDROID_SOURCE_PATH + "social/an_social.jar");
		FileStaticAPI.DeleteFile(ANDROID_SOURCE_PATH + "social/twitter4j-core-3.0.5.jar");
		
		
		FileStaticAPI.DeleteFile(ANDROID_SOURCE_PATH + "google_play/an_googleplay.jar");
		FileStaticAPI.DeleteFile(ANDROID_SOURCE_PATH + "google_play/google-play-services.jar");
		
		FileStaticAPI.DeleteFile(ANDROID_SOURCE_PATH + "billing/an_billing.jar");
		
		
		
		FileStaticAPI.CopyFile(ANDROID_SOURCE_PATH + "libs/android-support-v4.txt", ANDROID_DESTANATION_PATH + "libs/android-support-v4.jar");
		FileStaticAPI.CopyFile(ANDROID_SOURCE_PATH + "androidnative.txt", 	        ANDROID_DESTANATION_PATH + "androidnative.jar");
		FileStaticAPI.CopyFile(ANDROID_SOURCE_PATH + "mobilenativepopups.txt", 	        ANDROID_DESTANATION_PATH + "mobilenativepopups.jar");
		
		
		
		
		
		FileStaticAPI.CopyFolder(ANDROID_SOURCE_PATH + "facebook", 			ANDROID_DESTANATION_PATH + "facebook");
		
		#if UNITY_3_5 || UNITY_4_0 || UNITY_4_1	|| UNITY_4_2 || UNITY_4_3 || UNITY_4_5 || UNITY_4_6
		
		#else
		FileStaticAPI.DeleteFolder(ANDROID_SOURCE_PATH + "facebook");
		#endif
		
		if(IsFirstInstall) {
			EnableBillingAPI();
			EnableGooglePlayAPI();
			EnableSocialAPI();
			EnableCameraAPI();
			EnableAppLicensingAPI();
		}
		
		
		
		
		string file;
		file = "AN_Res/res/values/analytics.xml";
		if(!FileStaticAPI.IsFileExists(ANDROID_DESTANATION_PATH + file)) {
			FileStaticAPI.CopyFile(ANDROID_SOURCE_PATH + file, 	ANDROID_DESTANATION_PATH + file);
		}
		
		
		file = "AN_Res/res/values/ids.xml";
		if(!FileStaticAPI.IsFileExists(ANDROID_DESTANATION_PATH + file)) {
			FileStaticAPI.CopyFile(ANDROID_SOURCE_PATH + file, 	ANDROID_DESTANATION_PATH + file);
		}
		
		file = "AN_Res/res/xml/file_paths.xml";
		if(!FileStaticAPI.IsFileExists(ANDROID_DESTANATION_PATH + file)) {
			FileStaticAPI.CopyFile(ANDROID_SOURCE_PATH + file, 	ANDROID_DESTANATION_PATH + file);
		}
		
		
		file = "AN_Res/res/values/version.xml";
		FileStaticAPI.CopyFile(ANDROID_SOURCE_PATH + file, 	ANDROID_DESTANATION_PATH + file);

		file = "AN_Res/project.properties";
		FileStaticAPI.CopyFile(ANDROID_SOURCE_PATH + file, 	ANDROID_DESTANATION_PATH + file);
		
		file = "AN_Res/AndroidManifest.xml";
		FileStaticAPI.CopyFile(ANDROID_SOURCE_PATH + file, 	ANDROID_DESTANATION_PATH + file);
		
		//First install dependense		
		
		file = "AndroidManifest.xml";
		if(!FileStaticAPI.IsFileExists(ANDROID_DESTANATION_PATH + file)) {
			FileStaticAPI.CopyFile(ANDROID_SOURCE_PATH + file, 	ANDROID_DESTANATION_PATH + file);
		} 
		
		AssetDatabase.Refresh();
		
	}



	public static bool IsFacebookInstalled {
		get {
			return FileStaticAPI.IsFileExists("Facebook/Scripts/FB.cs");
		}
	}


}
