using UnityEngine;
using UnityEditor;
using System.Collections;
using System.IO;
using System.Collections.Generic;


public class SA_RemoveTool  {
	
	
	public static void RemoveOneSignal() {
		RemoveNativeFileIOS("libOneSignal");
		RemoveNativeFileIOS("OneSignal");
		RemoveNativeFileIOS("OneSignalUnityRuntime");
		FileStaticAPI.DeleteFolder("StansAssetsCommon/OneSignal");
	}
	
	
	
	public static void RemovePlugins() {
		
		int option = EditorUtility.DisplayDialogComplex(
			"Remove Stans Asets Plugins",
			"Following plugins wiil be removed:\n" + SA_VersionsManager.InstalledPluginsList,
			"Remove",
			"Cancel",
			"Documentation");
		
		
		switch(option) {
		case 0:
			ProcessRemove();
			break;
			
		case 2:
			string url = "https://goo.gl/CCBFIZ";
			Application.OpenURL(url);
			break;
		}
		
	}
	
	
	
	private static void ProcessRemove() {
		FileStaticAPI.DeleteFolder ("Extensions/AllDocumentation");
		FileStaticAPI.DeleteFolder ("Extensions/FlashLikeEvents");
		FileStaticAPI.DeleteFolder ("Extensions/AndroidManifestManager");
		FileStaticAPI.DeleteFolder ("Extensions/GooglePlayCommon");
		FileStaticAPI.DeleteFolder ("Extensions/StansAssetsCommon");
		FileStaticAPI.DeleteFolder ("Extensions/StansAssetsPreviewUI");
		FileStaticAPI.DeleteFolder ("Extensions/IOSDeploy");
		FileStaticAPI.DeleteFolder ("Facebook");
		
		
		
		if (SA_VersionsManager.Is_AN_Installed) {
			FileStaticAPI.DeleteFolder ("Extensions/AndroidNative");
			RemoveAndroidPart();	
		}
		
		
		if (SA_VersionsManager.Is_MSP_Installed){
			FileStaticAPI.DeleteFolder ("Extensions/MobileSocialPlugin");
			RemoveIOSPart();
			RemoveAndroidPart();
		}
		
		
		if (SA_VersionsManager.Is_GMA_Installed){
			FileStaticAPI.DeleteFolder ("Extensions/GoogleMobileAd");
			RemoveIOSPart();
			RemoveAndroidPart();
			RemoveWP8Part();
		}
		
		
		
		if (SA_VersionsManager.Is_ISN_Installed){
			FileStaticAPI.DeleteFolder("Extensions/IOSNative");
			RemoveIOSPart();
		}
		
		
		if (SA_VersionsManager.Is_UM_Installed){
			FileStaticAPI.DeleteFolder("Extensions/UltimateMobile");
			FileStaticAPI.DeleteFolder("Extensions/WP8Native");
			FileStaticAPI.DeleteFolder("WebPlayerTemplates");
			FileStaticAPI.DeleteFolder("Extensions/GoogleAnalytics");
			FileStaticAPI.DeleteFolder("Extensions/MobileNativePopUps");
			
			RemoveWP8Part();
			RemoveIOSPart();
			RemoveAndroidPart();
		}
		
		
		FileStaticAPI.DeleteFolder ("Plugins/StansAssets");
		AssetDatabase.Refresh();
		
		
		EditorUtility.DisplayDialog("Plugins Removed", "Unity Editor relaunch required.", "Okay");
	}
	
	
	
	
	
	private static void RemoveAndroidPart() {
		FileStaticAPI.DeleteFile(PluginsInstalationUtil.ANDROID_DESTANATION_PATH + "androidnative.jar");
		FileStaticAPI.DeleteFile(PluginsInstalationUtil.ANDROID_DESTANATION_PATH + "mobilenativepopups.jar");
		
		
		FileStaticAPI.DeleteFolder (PluginsInstalationUtil.ANDROID_DESTANATION_PATH + "facebook");
		FileStaticAPI.DeleteFolder (PluginsInstalationUtil.ANDROID_DESTANATION_PATH + "libs");
		
		/*
		FileStaticAPI.DeleteFile ("Plugins/Android/res/values/analytics.xml");
		FileStaticAPI.DeleteFile ("Plugins/Android/res/values/ids.xml");
		FileStaticAPI.DeleteFile ("Plugins/Android/res/values/version.xml");
		FileStaticAPI.DeleteFile ("Plugins/Android/res/xml/file_paths.xml");
		*/
	}
	
	
	private static void RemoveWP8Part() {
		FileStaticAPI.DeleteFile ("Plugins/WP8/GoogleAds.dll");
		FileStaticAPI.DeleteFile ("Plugins/WP8/GoogleAds.xml");
		FileStaticAPI.DeleteFile ("Plugins/WP8/MockIAPLib.dll");
		FileStaticAPI.DeleteFile ("Plugins/WP8/WP8Native.dll");
		FileStaticAPI.DeleteFile ("Plugins/WP8/WP8PopUps.dll");
		FileStaticAPI.DeleteFile ("Plugins/WP8/GoogleAdsWP8.dll");
		FileStaticAPI.DeleteFile ("Plugins/GoogleAdsWP8.dll");
		FileStaticAPI.DeleteFile ("Plugins/Metro/WP8Native.dll");
		FileStaticAPI.DeleteFile ("Plugins/Metro/WP8PopUps.dll");
	}
	
	
	private static void RemoveIOSPart() {
		//TODO просмотреть не забыли ли чего лучге смотреть в УМ
		
		//ISN
		RemoveNativeFileIOS("AppEventListener");
		RemoveNativeFileIOS("CloudManager");
		RemoveNativeFileIOS("CustomBannerView");
		RemoveNativeFileIOS("iAdBannerController");
		RemoveNativeFileIOS("iAdBannerObject");
		RemoveNativeFileIOS("InAppPurchaseManager");
		RemoveNativeFileIOS("IOSNativeNotificationCenter");
		RemoveNativeFileIOS("ISN_GameCenterListner");
		RemoveNativeFileIOS("ISN_GameCenterManager");
		RemoveNativeFileIOS("ISN_GameCenter");
		RemoveNativeFileIOS("ISN_Media");
		RemoveNativeFileIOS("ISN_iAd");
		RemoveNativeFileIOS("ISN_InApp");
		RemoveNativeFileIOS("ISN_NativePopUpsManager");
		RemoveNativeFileIOS("ISN_NativeUtility");
		RemoveNativeFileIOS("ISN_NSData+Base64");
		RemoveNativeFileIOS("ISN_Reachability");
		RemoveNativeFileIOS("ISN_Security");
		RemoveNativeFileIOS("ISN_Camera");
		RemoveNativeFileIOS("ISN_ReplayKit");
		RemoveNativeFileIOS("ISN_SocialGate");
		RemoveNativeFileIOS("ISN_NativeCore");
		RemoveNativeFileIOS("ISNDataConvertor");
		RemoveNativeFileIOS("ISNSharedApplication");
		RemoveNativeFileIOS("ISNVideo");
		RemoveNativeFileIOS("SKProduct+LocalizedPrice");
		RemoveNativeFileIOS("SocialGate");
		RemoveNativeFileIOS("StoreProductView");
		RemoveNativeFileIOS("TransactionServer");
		
		
		//UM
		RemoveNativeFileIOS("UM_IOS_INSTALATION_MARK");
		
		//GMA
		RemoveNativeFileIOS("GoogleMobileAdBanner");
		RemoveNativeFileIOS("GoogleMobileAdController");
		
		//MPS
		RemoveNativeFileIOS("IOSInstaPlugin");
		RemoveNativeFileIOS("IOSTwitterPlugin");
		RemoveNativeFileIOS("MGInstagram");
		
		
		RemoveOneSignal();
	}
	
	
	
	
	
	
	private static void RemoveNativeFileIOS(string filename) {
		string filePath = PluginsInstalationUtil.IOS_DESTANATION_PATH  + filename;
		
		FileStaticAPI.DeleteFile (filePath + ".h");
		FileStaticAPI.DeleteFile (filePath + ".m");
		FileStaticAPI.DeleteFile (filePath + ".mm");
		FileStaticAPI.DeleteFile (filePath + ".a");
		FileStaticAPI.DeleteFile (filePath + ".txt");
		
	}
	
}
