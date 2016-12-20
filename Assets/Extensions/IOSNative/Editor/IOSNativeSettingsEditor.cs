
using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

[CustomEditor(typeof(IOSNativeSettings))]
public class IOSNativeSettingsEditor : Editor {


	private static string[] ToolbarHeaders =new string[] {"General", "Billing", "Game Center", "Other"};

	private static GUIContent AppleIdLabel = new GUIContent("Apple Id [?]:", "Your Application Apple ID.");
	private static GUIContent SdkVersion   = new GUIContent("Plugin Version [?]", "This is the Plugin version.  If you have problems or compliments please include this so that we know exactly which version to look out for.");
	private static GUIContent SupportEmail = new GUIContent("Support [?]", "If you have any technical questions, feel free to drop us an e-mail");



	private static GUIContent SKPVDLabel = new GUIContent("Store Products View [?]:", "The SKStoreProductViewController class makes it possible to integrate purchasing from Apple’s iTunes, App and iBooks stores directly into iOS 6 applications with minimal coding work.");
	private static GUIContent CheckInternetLabel = new GUIContent("Check Internet Connection[?]:", "If set to true, the Internet connection will be checked before sending load request. Requests will be sent automatically if network becomes available.");
	private static GUIContent SendBillingFakeActions = new GUIContent("Enable Editor Testing[?]:", "Fake connect and purchase events will be fired in the editor, can be useful for testing your implementation in Editor.");

	private static GUIContent UseGCCaching  = new GUIContent("Use Requests Caching[?]:", "Requests to Game Center will be cached if no Internet connection is available. Requests will be resent on the next Game Center connect event.");

	private static GUIContent AutoLoadSmallImagesLoadTitle  = new GUIContent("Autoload Small Player Photo[?]:", "As soon as player info received, small player photo will be requested automatically");
	private static GUIContent AutoLoadBigmagesLoadTitle  = new GUIContent("Autoload Big Player Photo[?]:", "As soon as player info received, big player photo will be requested automatically");



	private static GUIContent DisablePluginLogsNote  = new GUIContent("Disable Plugin Logs[?]:", "All plugins 'Debug.Log' lines will be disabled if this option is enabled.");



	private static string IOSNotificationController_Path = "Extensions/IOSNative/Notifications/IOSNotificationController.cs";
	private static string DeviceTokenListener_Path = "Extensions/IOSNative/Notifications/DeviceTokenListener.cs";

	private static string GameCenterManager_Path = "Extensions/IOSNative/GameCenter/Manage/GameCenterManager.cs";
	private static string GameCenter_TBM_Path = "Extensions/IOSNative/GameCenter/Manage/GameCenter_TBM.cs";
	private static string GameCenter_RTM_Path = "Extensions/IOSNative/GameCenter/Manage/GameCenter_RTM.cs";

	private static string IOSNativeMarketBridge_Path = "Extensions/IOSNative/Market/IOSNativeMarketBridge.cs";
	private static string IOSStoreProductView_Path = "Extensions/IOSNative/Market/IOSStoreProductView.cs";
	private static string ISN_Security_Path = "Extensions/IOSNative/Other/System/ISN_Security.cs";


	private static string iAdBannerControllerr_Path = "Extensions/IOSNative/iAd/iAdBannerController.cs";
	private static string iAdBanner_Path = "Extensions/IOSNative/iAd/iAdBanner.cs";

	private static string IOSSocialManager_Path = "Extensions/IOSNative/Social/IOSSocialManager.cs";

	private static string IOSCamera_Path = "Extensions/IOSNative/Other/Camera/IOSCamera.cs";


	private static string IOSVideoManager_Path = "Extensions/IOSNative/Other/VIdeo/IOSVideoManager.cs";
	private static string ISN_MediaController_Path = "Extensions/IOSNative/Other/Media/Controllers/ISN_MediaController.cs";


	private static string ISN_ReplayKit_Path = "Extensions/IOSNative/Other/VIdeo/ISN_ReplayKit.cs";
	private static string ISN_CloudKit_Path = "Extensions/IOSNative/iCloud/ISN_CloudKit.cs";
	private static string ISN_Soomla_Path = "Extensions/IOSNative/Addons/Soomla/Controllers/ISN_SoomlaGrow.cs";


	private  GUIStyle _ImageBoxStyle = null;

	
	void Awake() {
		#if !UNITY_WEBPLAYER
		UpdatePluginSettings();
		#endif
	}





	public override void OnInspectorGUI()  {


		#if UNITY_WEBPLAYER
		EditorGUILayout.HelpBox("Editing IOS Native Settings not available with web player platfrom. Please switch to any other platform under Build Settings menu", MessageType.Warning);
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.Space();
		if(GUILayout.Button("Switch To IOS Platfrom",  GUILayout.Width(150))) {

		#if UNITY_5
			EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTarget.iOS);
		#else
			EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTarget.iPhone);
		#endif
			


		}
		EditorGUILayout.EndHorizontal();

		if(Application.isEditor) {
			return;
		}

		#endif



		GUI.changed = false;


		InstallOptions();


	

		
		GUI.SetNextControlName ("toolbar");
		IOSNativeSettings.Instance.ToolbarIndex = GUILayout.Toolbar (IOSNativeSettings.Instance.ToolbarIndex, ToolbarHeaders, new GUILayoutOption[]{ GUILayout.Height(25f)});


		switch(IOSNativeSettings.Instance.ToolbarIndex) {
		case 0: 
			APISettings();
			EditorGUILayout.Space();
			MoreActions();
			EditorGUILayout.Space();
			AboutGUI();
			break;
		case 1:
			BillingSettings(); 
			break;
		case 2:
			GameCenterSettings();
			break;
		case 3:
			OtherSettings();
			break;
		}


		if(GUI.changed) {
			DirtyEditor();
		}



	}




	private void InstallOptions() {

		if(!IsInstalled) {
			EditorGUILayout.BeginVertical (GUI.skin.box);

			EditorGUILayout.HelpBox("Install Required ", MessageType.Error);
			
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.Space();
			Color c = GUI.color;
			GUI.color = Color.cyan;
			if(GUILayout.Button("Install Plugin",  GUILayout.Width(350))) {
				ISN_Plugin_Install();
			}
			EditorGUILayout.Space();
			GUI.color = c;
			EditorGUILayout.EndHorizontal();
			EditorGUILayout.Space();

			EditorGUILayout.EndVertical();


			EditorGUILayout.Space();
		}

		if(IsInstalled) {
			if(!IsUpToDate) {

				EditorGUILayout.BeginVertical (GUI.skin.box);
				EditorGUILayout.HelpBox("Update Required \nResources version: " + SA_VersionsManager.ISN_StringVersionId + " Plugin version: " + IOSNativeSettings.VERSION_NUMBER, MessageType.Warning);
				
				EditorGUILayout.BeginHorizontal();
				EditorGUILayout.Space();
				Color c = GUI.color;
				GUI.color = Color.cyan;

				if(CurrentMagorVersion != SA_VersionsManager.ISN_MagorVersion) {
					if(GUILayout.Button("How to update",  GUILayout.Width(350))) {
						Application.OpenURL("https://goo.gl/GsUcA0");
					}
				} else {
					if(GUILayout.Button("Upgrade Resources",  GUILayout.Width(350))) {
						ISN_Plugin_Install();
					}
				}

				GUI.color = c;
				EditorGUILayout.Space();
				EditorGUILayout.EndHorizontal();
				EditorGUILayout.Space();

				EditorGUILayout.EndVertical();
				EditorGUILayout.Space();
				
			} else {
				EditorGUILayout.HelpBox("IOS Native Plugin v" + IOSNativeSettings.VERSION_NUMBER + " is installed", MessageType.Info);

			}
		}


	}


	public static void APISettings() {

		EditorGUILayout.Space();
		EditorGUILayout.HelpBox("(Required) Application Data", MessageType.None);
	
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField(AppleIdLabel);
		IOSNativeSettings.Instance.AppleId	 	= EditorGUILayout.TextField(IOSNativeSettings.Instance.AppleId);
		if(IOSNativeSettings.Instance.AppleId.Length > 0) {
			IOSNativeSettings.Instance.AppleId		= IOSNativeSettings.Instance.AppleId.Trim();
		}
		
		EditorGUILayout.EndHorizontal();
			
		EditorGUILayout.Space();

		EditorGUILayout.HelpBox("(Optional) Services Settings", MessageType.None);


		//IOSNativeSettings.
		IOSNativeSettings.Instance.ExpandAPISettings = EditorGUILayout.Foldout(IOSNativeSettings.Instance.ExpandAPISettings, "IOS Native Libs");
		if(IOSNativeSettings.Instance.ExpandAPISettings) {
			EditorGUI.indentLevel++;


			EditorGUI.BeginChangeCheck();

			EditorGUILayout.BeginHorizontal();
			GUI.enabled = false;
			EditorGUILayout.Toggle("ISN Basic Features",  true);
			GUI.enabled = true;

			IOSNativeSettings.Instance.EnableGameCenterAPI = EditorGUILayout.Toggle("Game Center",  IOSNativeSettings.Instance.EnableGameCenterAPI);
			EditorGUILayout.EndHorizontal();

			
			EditorGUILayout.BeginHorizontal();
			IOSNativeSettings.Instance.EnableInAppsAPI = EditorGUILayout.Toggle("In-App Purchases",  IOSNativeSettings.Instance.EnableInAppsAPI);
			IOSNativeSettings.Instance.EnableSocialSharingAPI = EditorGUILayout.Toggle("Social Sharing",  IOSNativeSettings.Instance.EnableSocialSharingAPI);
			EditorGUILayout.EndHorizontal();


			EditorGUILayout.BeginHorizontal();
			IOSNativeSettings.Instance.EnableCameraAPI = EditorGUILayout.Toggle("Camera And Gallery",  IOSNativeSettings.Instance.EnableCameraAPI);
			IOSNativeSettings.Instance.EnableiAdAPI = EditorGUILayout.Toggle("iAd",  IOSNativeSettings.Instance.EnableiAdAPI);
			EditorGUILayout.EndHorizontal();



			EditorGUILayout.BeginHorizontal();
			IOSNativeSettings.Instance.EnableMediaPlayerAPI = EditorGUILayout.Toggle("Media Player",  IOSNativeSettings.Instance.EnableMediaPlayerAPI);
			IOSNativeSettings.Instance.EnablePushNotificationsAPI = EditorGUILayout.Toggle("Push Notifications",  IOSNativeSettings.Instance.EnablePushNotificationsAPI);
			EditorGUILayout.EndHorizontal();

			EditorGUILayout.BeginHorizontal();
			IOSNativeSettings.Instance.EnableReplayKit = EditorGUILayout.Toggle("Replay Kit ",  IOSNativeSettings.Instance.EnableReplayKit);
			IOSNativeSettings.Instance.EnableCloudKit = EditorGUILayout.Toggle("Cloud Kit ",  IOSNativeSettings.Instance.EnableCloudKit);
			EditorGUILayout.EndHorizontal();


			if(EditorGUI.EndChangeCheck()) {
				UpdatePluginSettings();
			}
		
		
			EditorGUI.indentLevel--;
		}
	}




	public static void CameraAndGallery() {
		EditorGUILayout.Space();
		EditorGUILayout.LabelField("Gallery", EditorStyles.boldLabel);
		EditorGUI.indentLevel++; {
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField("Max Loaded Image Size");
			IOSNativeSettings.Instance.MaxImageLoadSize	 	= EditorGUILayout.IntField(IOSNativeSettings.Instance.MaxImageLoadSize);
			EditorGUILayout.EndHorizontal();
			
			
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField("Loaded Image Format");
			IOSNativeSettings.Instance.GalleryImageFormat	 	= (IOSGalleryLoadImageFormat) EditorGUILayout.EnumPopup(IOSNativeSettings.Instance.GalleryImageFormat);
			EditorGUILayout.EndHorizontal();
			
			
			if(IOSNativeSettings.Instance.GalleryImageFormat == IOSGalleryLoadImageFormat.JPEG) {
				GUI.enabled = true;
			} else {
				GUI.enabled = false;
			}

			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField("JPEG Compression Rate");
			IOSNativeSettings.Instance.JPegCompressionRate	 	= EditorGUILayout.Slider(IOSNativeSettings.Instance.JPegCompressionRate, 0f, 1f);
			EditorGUILayout.EndHorizontal();
			GUI.enabled = true;

		}EditorGUI.indentLevel--;



	}



	GUIContent L_IdDLabel 		= new GUIContent("Leaderboard ID[?]:", "A unique alphanumeric identifier that you create for this leaderboard. Can also contain periods and underscores.");
	GUIContent L_DisplayNameLabel  	= new GUIContent("Display Name[?]:", "The display name of the leaderboard.");
	GUIContent L_DescriptionLabel  	= new GUIContent("Description[?]:", "The description of your leaderboard.");

	GUIContent A_IdDLabel 		= new GUIContent("Achievement ID[?]:", "A unique alphanumeric identifier that you create for this achievement. Can also contain periods and underscores.");
	GUIContent A_DisplayNameLabel  	= new GUIContent("Display Name[?]:", "The display name of the achievement.");
	GUIContent A_DescriptionLabel 	= new GUIContent("Description[?]:", "The description of your achievement.");

	private void GameCenterSettings() {
		EditorGUILayout.Space();
		EditorGUILayout.HelpBox("Leaderboards Info", MessageType.None);


		EditorGUI.indentLevel++; {
			
			EditorGUILayout.BeginVertical (GUI.skin.box);

			EditorGUILayout.BeginHorizontal();
			IOSNativeSettings.Instance.ShowLeaderboards = EditorGUILayout.Foldout(IOSNativeSettings.Instance.ShowLeaderboards, "Leaderboards");
			
			
			
			EditorGUILayout.EndHorizontal();

			
			if(IOSNativeSettings.Instance.ShowLeaderboards) {
				EditorGUILayout.Space();
				
				foreach(GK_Leaderboard leaderboard in IOSNativeSettings.Instance.Leaderboards) {
					
					
					EditorGUILayout.BeginVertical (GUI.skin.box);
					
					EditorGUILayout.BeginHorizontal();
					
					
					
					if(leaderboard.Info.Texture != null) {
						GUILayout.Box(leaderboard.Info.Texture, ImageBoxStyle, new GUILayoutOption[]{GUILayout.Width(18), GUILayout.Height(18)});
					}
					
					leaderboard.IsOpen 	= EditorGUILayout.Foldout(leaderboard.IsOpen, leaderboard.Info.Title);
					
					

					bool ItemWasRemoved = DrawSrotingButtons((object) leaderboard, IOSNativeSettings.Instance.Leaderboards);
					if(ItemWasRemoved) {
						return;
					}
					
					
					EditorGUILayout.EndHorizontal();
					
					if(leaderboard.IsOpen) {
						EditorGUILayout.BeginHorizontal();
						EditorGUILayout.LabelField(L_IdDLabel);
						leaderboard.Info.Identifier	 	= EditorGUILayout.TextField(leaderboard.Info.Identifier);
						if(leaderboard.Info.Identifier.Length > 0) {
							leaderboard.Info.Identifier 		= leaderboard.Info.Identifier.Trim();
						}
						EditorGUILayout.EndHorizontal();
						
						
						EditorGUILayout.BeginHorizontal();
						EditorGUILayout.LabelField(L_DisplayNameLabel);
						leaderboard.Info.Title	 	= EditorGUILayout.TextField(leaderboard.Info.Title);
						EditorGUILayout.EndHorizontal();
		

						EditorGUILayout.Space();
						EditorGUILayout.Space();

						
						EditorGUILayout.BeginHorizontal();
						EditorGUILayout.LabelField(L_DescriptionLabel);
						EditorGUILayout.EndHorizontal();
						
						EditorGUILayout.BeginHorizontal();
						leaderboard.Info.Description	 = EditorGUILayout.TextArea(leaderboard.Info.Description,  new GUILayoutOption[]{GUILayout.Height(60), GUILayout.Width(200)} );
						leaderboard.Info.Texture = (Texture2D) EditorGUILayout.ObjectField("", leaderboard.Info.Texture, typeof (Texture2D), false);
						EditorGUILayout.EndHorizontal();
						
					}
					
					EditorGUILayout.EndVertical();
					
				}
				
				EditorGUILayout.BeginHorizontal();
				EditorGUILayout.Space();
				if(GUILayout.Button("Add new", EditorStyles.miniButton, GUILayout.Width(250))) {
					GK_Leaderboard leaderbaord =  new GK_Leaderboard("");
					IOSNativeSettings.Instance.Leaderboards.Add(leaderbaord);
				}
				
				EditorGUILayout.Space();
				EditorGUILayout.EndHorizontal();
				EditorGUILayout.Space();
			}

			EditorGUILayout.EndVertical();

		}EditorGUI.indentLevel--;



		EditorGUILayout.Space();
		EditorGUILayout.HelpBox("Achievements Info", MessageType.None);

		EditorGUI.indentLevel++; {

			EditorGUILayout.BeginVertical (GUI.skin.box);
			
			EditorGUILayout.BeginHorizontal();
			IOSNativeSettings.Instance.ShowAchievementsParams = EditorGUILayout.Foldout(IOSNativeSettings.Instance.ShowAchievementsParams, "Achievements");
			
			
			
			EditorGUILayout.EndHorizontal();
			
			
			if(IOSNativeSettings.Instance.ShowAchievementsParams) {
				EditorGUILayout.Space();
				
				foreach(GK_AchievementTemplate achievement in IOSNativeSettings.Instance.Achievements) {
					
					
					EditorGUILayout.BeginVertical (GUI.skin.box);
					
					EditorGUILayout.BeginHorizontal();
					
					
					
					if(achievement.Texture != null) {
						GUILayout.Box(achievement.Texture, ImageBoxStyle, new GUILayoutOption[]{GUILayout.Width(18), GUILayout.Height(18)});
					}
					
					achievement.IsOpen 	= EditorGUILayout.Foldout(achievement.IsOpen, achievement.Title);
					
					
					
					bool ItemWasRemoved = DrawSrotingButtons((object) achievement, IOSNativeSettings.Instance.Achievements);
					if(ItemWasRemoved) {
						return;
					}
					
					
					EditorGUILayout.EndHorizontal();
					
					if(achievement.IsOpen) {
						EditorGUILayout.BeginHorizontal();
						EditorGUILayout.LabelField(A_IdDLabel);
						achievement.Id	 	= EditorGUILayout.TextField(achievement.Id);
						if(achievement.Id.Length > 0) {
							achievement.Id 		= achievement.Id.Trim();
						}
						EditorGUILayout.EndHorizontal();
						
						
						EditorGUILayout.BeginHorizontal();
						EditorGUILayout.LabelField(A_DisplayNameLabel);
						achievement.Title	 	= EditorGUILayout.TextField(achievement.Title);
						EditorGUILayout.EndHorizontal();
						
						
						EditorGUILayout.Space();
						EditorGUILayout.Space();
						
						
						EditorGUILayout.BeginHorizontal();
						EditorGUILayout.LabelField(A_DescriptionLabel);
						EditorGUILayout.EndHorizontal();
						
						EditorGUILayout.BeginHorizontal();
						achievement.Description	 = EditorGUILayout.TextArea(achievement.Description,  new GUILayoutOption[]{GUILayout.Height(60), GUILayout.Width(200)} );
						achievement.Texture = (Texture2D) EditorGUILayout.ObjectField("", achievement.Texture, typeof (Texture2D), false);
						EditorGUILayout.EndHorizontal();
						
					}
					
					EditorGUILayout.EndVertical();
					
				}
				
				EditorGUILayout.BeginHorizontal();
				EditorGUILayout.Space();
				if(GUILayout.Button("Add new", EditorStyles.miniButton, GUILayout.Width(250))) {
					GK_AchievementTemplate achievement =  new GK_AchievementTemplate();
					IOSNativeSettings.Instance.Achievements.Add(achievement);
				}
				
				EditorGUILayout.Space();
				EditorGUILayout.EndHorizontal();
				EditorGUILayout.Space();
			}
			
			EditorGUILayout.EndVertical();




		}EditorGUI.indentLevel--;

		EditorGUILayout.Space();
		EditorGUILayout.HelpBox("Game Center API Settings", MessageType.None);



		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField(AutoLoadBigmagesLoadTitle);
		IOSNativeSettings.Instance.AutoLoadUsersBigImages = EditorGUILayout.Toggle(IOSNativeSettings.Instance.AutoLoadUsersBigImages);
		EditorGUILayout.EndHorizontal();
		
		
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField(AutoLoadSmallImagesLoadTitle);
		IOSNativeSettings.Instance.AutoLoadUsersSmallImages = EditorGUILayout.Toggle(IOSNativeSettings.Instance.AutoLoadUsersSmallImages);
		EditorGUILayout.EndHorizontal();


		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField(UseGCCaching);
		IOSNativeSettings.Instance.UseGCRequestCaching = EditorGUILayout.Toggle(IOSNativeSettings.Instance.UseGCRequestCaching);
		EditorGUILayout.EndHorizontal();
		
		
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField("Store Achievements Progress in PlayerPrefs[?]");
		IOSNativeSettings.Instance.UsePPForAchievements = EditorGUILayout.Toggle(IOSNativeSettings.Instance.UsePPForAchievements);
		EditorGUILayout.EndHorizontal();
			
			



	}

	private void OtherSettings() {

			
		CameraAndGallery();


		EditorGUILayout.Space();
		EditorGUILayout.LabelField("Replay Kit", EditorStyles.boldLabel);
		EditorGUI.indentLevel++; {
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField("Full screen video edit preview on iPad");
			
			bool windowed = IOSNativeSettings.Instance.RPK_iPadViewType == 0;
			windowed = EditorGUILayout.Toggle(windowed);
			if(windowed) {
				IOSNativeSettings.Instance.RPK_iPadViewType = 0;
			} else {
				IOSNativeSettings.Instance.RPK_iPadViewType = 1;
			}
			EditorGUILayout.EndHorizontal();
			
		}EditorGUI.indentLevel--;



		EditorGUILayout.Space();
		EditorGUILayout.LabelField("One Signal", EditorStyles.boldLabel);

		EditorGUI.indentLevel++; {
			
			EditorGUI.BeginChangeCheck(); 

			IOSNativeSettings.Instance.OneSignalEnabled = ToggleFiled("Enable One Signal", IOSNativeSettings.Instance.OneSignalEnabled);
			if(EditorGUI.EndChangeCheck())  {
				
				if(IOSNativeSettings.Instance.OneSignalEnabled) {
					if(!FileStaticAPI.IsFolderExists("Plugins/OneSignal")) {
						bool res = EditorUtility.DisplayDialog("One Signal not found", "IOS Native wasn't able to find One Signal libraryes in your project. Would you like to donwload and install it?", "Download", "No Thanks");
						if(res) {
							Application.OpenURL(IOSNativeSettings.Instance.OneSignalDocsLink);
						}
						IOSNativeSettings.Instance.OneSignalEnabled = false;
					}
				}
			}	

		} EditorGUI.indentLevel--;
		
		EditorGUILayout.BeginHorizontal ();
		EditorGUILayout.Space ();
		if (GUILayout.Button("[?] Read More", GUILayout.Width(100.0f))) {
			Application.OpenURL(IOSNativeSettings.Instance.OneSignalDocsLink);
		}
		EditorGUILayout.EndHorizontal();


		EditorGUILayout.Space();
		EditorGUILayout.LabelField("Soomla Configuration", EditorStyles.boldLabel);

		EditorGUI.indentLevel++; {


			EditorGUI.BeginChangeCheck();
			bool prevSoomlaState = IOSNativeSettings.Instance.EnableSoomla;
			IOSNativeSettings.Instance.EnableSoomla = ToggleFiled("Enable GROW", IOSNativeSettings.Instance.EnableSoomla);
			if(EditorGUI.EndChangeCheck())  {
				UpdatePluginSettings();
			}

			if(!prevSoomlaState && IOSNativeSettings.Instance.EnableSoomla) {
				bool res = EditorUtility.DisplayDialog("Soomla Grow", "Make sure you initialize SoomlaGrow when your games starts: \nISN_SoomlaGrow.Init();", "Documentation", "Got it");
				if(res) {
					Application.OpenURL(IOSNativeSettings.Instance.SoomlaDocsLink);
				}
			}




			GUI.enabled = IOSNativeSettings.Instance.EnableSoomla;
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField("Game Key");
			IOSNativeSettings.Instance.SoomlaGameKey =  EditorGUILayout.TextField(IOSNativeSettings.Instance.SoomlaGameKey);
			EditorGUILayout.EndHorizontal();

			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField("Env Key");
			IOSNativeSettings.Instance.SoomlaEnvKey =  EditorGUILayout.TextField(IOSNativeSettings.Instance.SoomlaEnvKey);
			EditorGUILayout.EndHorizontal();
			GUI.enabled = true;

		}EditorGUI.indentLevel--;
		

			


	
	}

	private void MoreActions() {


		
		IOSNativeSettings.Instance.ShowOtherParams = EditorGUILayout.Foldout(IOSNativeSettings.Instance.ShowOtherParams, "More Actions");
		if (IOSNativeSettings.Instance.ShowOtherParams) {




			EditorGUI.BeginChangeCheck();


			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField(DisablePluginLogsNote);
			IOSNativeSettings.Instance.DisablePluginLogs = EditorGUILayout.Toggle(IOSNativeSettings.Instance.DisablePluginLogs);
			EditorGUILayout.EndHorizontal();


			EditorGUILayout.Space();

			EditorGUILayout.BeginHorizontal();
			if(GUILayout.Button("Load Example Settings",  GUILayout.Width(140))) {
				PlayerSettings.bundleIdentifier = "com.stansassets.iosnative.dev";
				IOSNativeSettings.Instance.InAppProducts.Clear();

				IOSProductTemplate SmallPack =  new IOSProductTemplate();
				SmallPack.IsOpen = false;
				SmallPack.Id = "your.product.id1.here";
				SmallPack.PriceTier = ISN_InAppPriceTier.Tier1;
				SmallPack.DisplayName = "Small Pack";
				SmallPack.ProductType = ISN_InAppType.Consumable;


				IOSProductTemplate NonConsumablePack =  new IOSProductTemplate();
				NonConsumablePack.IsOpen = false;
				NonConsumablePack.Id = "your.product.id2.here";
				NonConsumablePack.PriceTier = ISN_InAppPriceTier.Tier2;
				NonConsumablePack.DisplayName = "Non Consumable Pack";
				NonConsumablePack.ProductType = ISN_InAppType.Consumable;

				IOSNativeSettings.Instance.InAppProducts.Add(SmallPack);
				IOSNativeSettings.Instance.InAppProducts.Add(NonConsumablePack);



				IOSNativeSettings.Instance.Leaderboards.Clear();
				GK_Leaderboard Leaderboard1 =  new GK_Leaderboard("your.ios.leaderbord1.id");
				Leaderboard1.IsOpen = false;
				Leaderboard1.Info.Title = "Leaderboard 1";

				IOSNativeSettings.Instance.Leaderboards.Clear();
				GK_Leaderboard Leaderboard2 =  new GK_Leaderboard("your.ios.leaderbord2.id");
				Leaderboard2.IsOpen = false;
				Leaderboard2.Info.Title = "Leaderboard 2";

				IOSNativeSettings.Instance.Leaderboards.Add(Leaderboard1);
				IOSNativeSettings.Instance.Leaderboards.Add(Leaderboard2);


				IOSNativeSettings.Instance.Achievements.Clear();
				GK_AchievementTemplate Achievement1 =  new GK_AchievementTemplate();
				Achievement1.Id = "your.achievement.id1.here";
				Achievement1.IsOpen = false;
				Achievement1.Title = "Achievement 1";
				
	
				GK_AchievementTemplate Achievement2 =  new GK_AchievementTemplate();
				Achievement2.Id = "your.achievement.id2.here";
				Achievement2.IsOpen = false;
				Achievement2.Title = "Achievement 2";

				GK_AchievementTemplate Achievement3 =  new GK_AchievementTemplate();
				Achievement3.Id = "your.achievement.id3.here";
				Achievement3.IsOpen = false;
				Achievement3.Title = "Achievement 3";
				
				IOSNativeSettings.Instance.Achievements.Add(Achievement1);
				IOSNativeSettings.Instance.Achievements.Add(Achievement2);
				IOSNativeSettings.Instance.Achievements.Add(Achievement3);

				IOSNativeSettings.Instance.SoomlaEnvKey = "3c3df370-ad80-4577-8fe5-ca2c49b2c1b4";
				IOSNativeSettings.Instance.SoomlaGameKey = "db24ba61-3aa7-4653-a3f7-9c613cb2c0f3";

			}


			if(GUILayout.Button("Remove IOS Native",  GUILayout.Width(140))) {
				SA_RemoveTool.RemovePlugins();
			}



			EditorGUILayout.EndHorizontal();



		

		}
	}




	public GUIStyle ImageBoxStyle {
		get {
			if(_ImageBoxStyle == null) {
				_ImageBoxStyle =  new GUIStyle();
				_ImageBoxStyle.padding =  new RectOffset();
				_ImageBoxStyle.margin =  new RectOffset();
				_ImageBoxStyle.border =  new RectOffset();
			}
			
			return _ImageBoxStyle;
		}
	}

	GUIContent ProductIdDLabel 		= new GUIContent("ProductId[?]:", "A unique identifier that will be used for reporting. It can be composed of letters and numbers.");
	GUIContent IsConsLabel 			= new GUIContent("Is Consumable[?]:", "Is prodcut allowed to be purchased more than once?");
	GUIContent DisplayNameLabel  	= new GUIContent("Display Name[?]:", "This is the name of the In-App Purchase that will be seen by customers (if this is their primary language). For automatically renewable subscriptions, don’t include a duration in the display name. The display name can’t be longer than 75 characters.");
	GUIContent DescriptionLabel 	= new GUIContent("Description[?]:", "This is the description of the In-App Purchase that will be used by App Review during the review process. If indicated in your code, this description may also be seen by customers. For automatically renewable subscriptions, do not include a duration in the description. The description cannot be longer than 255 bytes.");
	GUIContent PriceTierLabel 		= new GUIContent("Price Tier[?]:", "The retail price for this In-App Purchase subscription.");


	private void BillingSettings() {

		EditorGUILayout.Space();
		EditorGUILayout.HelpBox("In-App Purchases", MessageType.None);


		EditorGUI.indentLevel++; {

			EditorGUILayout.BeginVertical (GUI.skin.box);


			EditorGUILayout.BeginHorizontal();
			IOSNativeSettings.Instance.ShowStoreKitProducts = EditorGUILayout.Foldout(IOSNativeSettings.Instance.ShowStoreKitProducts, "Products");



			EditorGUILayout.EndHorizontal();
			EditorGUILayout.Space();

			if(IOSNativeSettings.Instance.ShowStoreKitProducts) {

				foreach(IOSProductTemplate product in IOSNativeSettings.Instance.InAppProducts) {


					EditorGUILayout.BeginVertical (GUI.skin.box);

					EditorGUILayout.BeginHorizontal();



					if(product.Texture != null) {
						GUILayout.Box(product.Texture, ImageBoxStyle, new GUILayoutOption[]{GUILayout.Width(18), GUILayout.Height(18)});
					}

					product.IsOpen 	= EditorGUILayout.Foldout(product.IsOpen, product.DisplayName);

				


					EditorGUILayout.LabelField("           "+ product.Price + "$");
					bool ItemWasRemoved = DrawSrotingButtons((object) product, IOSNativeSettings.Instance.InAppProducts);
					if(ItemWasRemoved) {
						return;
					}


					EditorGUILayout.EndHorizontal();

					if(product.IsOpen) {
						EditorGUILayout.BeginHorizontal();
						EditorGUILayout.LabelField(ProductIdDLabel);
						product.Id	 	= EditorGUILayout.TextField(product.Id);
						if(product.Id.Length > 0) {
							product.Id 		= product.Id.Trim();
						}
						EditorGUILayout.EndHorizontal();
						

						EditorGUILayout.BeginHorizontal();
						EditorGUILayout.LabelField(DisplayNameLabel);
						product.DisplayName	 	= EditorGUILayout.TextField(product.DisplayName);
						EditorGUILayout.EndHorizontal();



						EditorGUILayout.BeginHorizontal();
						EditorGUILayout.LabelField(IsConsLabel);
						product.ProductType	 	= (ISN_InAppType) EditorGUILayout.EnumPopup(product.ProductType);
						EditorGUILayout.EndHorizontal();

						EditorGUILayout.BeginHorizontal();
						EditorGUILayout.LabelField(PriceTierLabel);
						EditorGUI.BeginChangeCheck();
						product.PriceTier	 	= (ISN_InAppPriceTier) EditorGUILayout.EnumPopup(product.PriceTier);
						if(EditorGUI.EndChangeCheck()) {
							product.UpdatePriceByTier();
						}
						EditorGUILayout.EndHorizontal();

						EditorGUILayout.Space();
						EditorGUILayout.Space();



						EditorGUILayout.BeginHorizontal();
						EditorGUILayout.LabelField(DescriptionLabel);
						EditorGUILayout.EndHorizontal();

						EditorGUILayout.BeginHorizontal();
						product.Description	 = EditorGUILayout.TextArea(product.Description,  new GUILayoutOption[]{GUILayout.Height(60), GUILayout.Width(200)} );
						product.Texture = (Texture2D) EditorGUILayout.ObjectField("", product.Texture, typeof (Texture2D), false);
						EditorGUILayout.EndHorizontal();

					}

					EditorGUILayout.EndVertical();

				}

				EditorGUILayout.BeginHorizontal();
				EditorGUILayout.Space();
				if(GUILayout.Button("Add new", EditorStyles.miniButton, GUILayout.Width(250))) {
					IOSProductTemplate product =  new IOSProductTemplate();
					IOSNativeSettings.Instance.InAppProducts.Add(product);
				}

				EditorGUILayout.Space();
				EditorGUILayout.EndHorizontal();
				EditorGUILayout.Space();
			}









			EditorGUILayout.EndVertical();
		}EditorGUI.indentLevel--;



			EditorGUILayout.Space();
			EditorGUILayout.Space();
			EditorGUILayout.LabelField(SKPVDLabel);

			/*****************************************/

			if(IOSNativeSettings.Instance.DefaultStoreProductsView.Count == 0) {
				EditorGUILayout.HelpBox("No Default Store Products View Added", MessageType.Info);
			}
			
			

			int i = 0;
			foreach(string str in IOSNativeSettings.Instance.DefaultStoreProductsView) {
				EditorGUILayout.BeginHorizontal();
				IOSNativeSettings.Instance.DefaultStoreProductsView[i]	 	= EditorGUILayout.TextField(IOSNativeSettings.Instance.DefaultStoreProductsView[i]);
				if(IOSNativeSettings.Instance.DefaultStoreProductsView[i].Length > 0) {
					IOSNativeSettings.Instance.DefaultStoreProductsView[i]		= IOSNativeSettings.Instance.DefaultStoreProductsView[i].Trim();
				}

				if(GUILayout.Button("Remove",  GUILayout.Width(80))) {
					IOSNativeSettings.Instance.DefaultStoreProductsView.Remove(str);
					break;
				}
				EditorGUILayout.EndHorizontal();
				i++;
			}
			
			
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.Space();
			if(GUILayout.Button("Add",  GUILayout.Width(80))) {
				IOSNativeSettings.Instance.DefaultStoreProductsView.Add("");
			}
			EditorGUILayout.EndHorizontal();


			EditorGUILayout.Space();
			
			
			EditorGUILayout.HelpBox("API Settings", MessageType.None);
			
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField(SendBillingFakeActions);
			IOSNativeSettings.Instance.SendFakeEventsInEditor = EditorGUILayout.Toggle(IOSNativeSettings.Instance.SendFakeEventsInEditor);
			EditorGUILayout.EndHorizontal();
			
			
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField(CheckInternetLabel);
			IOSNativeSettings.Instance.checkInternetBeforeLoadRequest = EditorGUILayout.Toggle(IOSNativeSettings.Instance.checkInternetBeforeLoadRequest);
			EditorGUILayout.EndHorizontal();
		

			EditorGUILayout.Space();

		//}
	}



	private bool DrawSrotingButtons(object currentObject, IList ObjectsList) {

		int ObjectIndex = ObjectsList.IndexOf(currentObject);
		if(ObjectIndex == 0) {
			GUI.enabled = false;
		} 

		bool up 		= GUILayout.Button("↑", EditorStyles.miniButtonLeft, GUILayout.Width(20));
		if(up) {
			object c = currentObject;
			ObjectsList[ObjectIndex]  		= ObjectsList[ObjectIndex - 1];
			ObjectsList[ObjectIndex - 1] 	=  c;
		}


		if(ObjectIndex >= ObjectsList.Count -1) {
			GUI.enabled = false;
		} else {
			GUI.enabled = true;
		}

		bool down 		= GUILayout.Button("↓", EditorStyles.miniButtonMid, GUILayout.Width(20));
		if(down) {
			object c = currentObject;
			ObjectsList[ObjectIndex] =  ObjectsList[ObjectIndex + 1];
			ObjectsList[ObjectIndex + 1] = c;
		}
		

		GUI.enabled = true;
		bool r 			= GUILayout.Button("-", EditorStyles.miniButtonRight, GUILayout.Width(20));
		if(r) {
			ObjectsList.Remove(currentObject);
		}

		return r;
	}
	
	
	private void AboutGUI() {


		EditorGUILayout.HelpBox("About the Plugin", MessageType.None);
		EditorGUILayout.Space();
		
		SelectableLabelField(SdkVersion, IOSNativeSettings.VERSION_NUMBER);
		SelectableLabelField(SupportEmail, SA_VersionsManager.SUPPORT_EMAIL);
		
		
	}
	
	private void SelectableLabelField(GUIContent label, string value) {
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField(label, GUILayout.Width(180), GUILayout.Height(16));
		EditorGUILayout.SelectableLabel(value, GUILayout.Height(16));
		EditorGUILayout.EndHorizontal();
	}



	private static void DirtyEditor() {
		#if UNITY_EDITOR
		EditorUtility.SetDirty(IOSNativeSettings.Instance);
		#endif
	}


	public static bool IsInstalled {
		get {
			return SA_VersionsManager.Is_ISN_Installed;
		}
	}
	
	
	public static bool IsUpToDate {
		get {
			
			if(CurrentVersion == SA_VersionsManager.ISN_Version) {
				return true;
			} else {
				return false;
			}
		}
	}
	
	public static int CurrentVersion {
		get {
			return SA_VersionsManager.ParceVersion(IOSNativeSettings.VERSION_NUMBER);
		}
	}
	
	public static int CurrentMagorVersion {
		get {
			return SA_VersionsManager.ParceMagorVersion(IOSNativeSettings.VERSION_NUMBER);
		}
	}
	
	
	public static int Version {
		get {
			return SA_VersionsManager.ISN_Version;
		}
	}


	public static void ISN_Plugin_Install() {

		PluginsInstalationUtil.IOS_UpdatePlugin();
		UpdateVersionInfo();
		UpdatePluginSettings();
	}

	public static void UpdateVersionInfo() {
		FileStaticAPI.Write(SA_VersionsManager.ISN_VERSION_INFO_PATH, IOSNativeSettings.VERSION_NUMBER);
	}


	public static void DisableAdditionalFeatures() {
		ChnageDefineState(ISN_ReplayKit_Path, 					"REPLAY_KIT", false);
		ChnageDefineState(ISN_CloudKit_Path, 					"CLOUD_KIT", false);

		ChnageDefineState(IOSNotificationController_Path, 		"PUSH_ENABLED", false);
		ChnageDefineState(DeviceTokenListener_Path,		 		"PUSH_ENABLED", false);
	}

	public static void UpdatePluginSettings() {
		
		
		if(!IsInstalled || !IsUpToDate) {
			return;
		}
		
		
		ChnageDefineState(IOSNotificationController_Path, 		"PUSH_ENABLED", IOSNativeSettings.Instance.EnablePushNotificationsAPI);
		ChnageDefineState(DeviceTokenListener_Path,		 		"PUSH_ENABLED", IOSNativeSettings.Instance.EnablePushNotificationsAPI);
		
		
		ChnageDefineState(GameCenterManager_Path, 				"GAME_CENTER_ENABLED", IOSNativeSettings.Instance.EnableGameCenterAPI);
		ChnageDefineState(GameCenter_TBM_Path, 					"GAME_CENTER_ENABLED", IOSNativeSettings.Instance.EnableGameCenterAPI);
		ChnageDefineState(GameCenter_RTM_Path, 					"GAME_CENTER_ENABLED", IOSNativeSettings.Instance.EnableGameCenterAPI);
		
		ChnageDefineState(IOSNativeMarketBridge_Path, 			"INAPP_API_ENABLED", IOSNativeSettings.Instance.EnableInAppsAPI);
		ChnageDefineState(IOSStoreProductView_Path, 			"INAPP_API_ENABLED", IOSNativeSettings.Instance.EnableInAppsAPI);
		ChnageDefineState(ISN_Security_Path, 					"INAPP_API_ENABLED", IOSNativeSettings.Instance.EnableInAppsAPI);
		

		ChnageDefineState(iAdBannerControllerr_Path, 			"IAD_API", IOSNativeSettings.Instance.EnableiAdAPI);

		ChnageDefineState(iAdBanner_Path, 						"IAD_API", IOSNativeSettings.Instance.EnableiAdAPI);
		
		ChnageDefineState(IOSSocialManager_Path, 				"SOCIAL_API", IOSNativeSettings.Instance.EnableSocialSharingAPI);
		
		ChnageDefineState(IOSCamera_Path, 						"CAMERA_API", IOSNativeSettings.Instance.EnableCameraAPI);
		
		ChnageDefineState(IOSVideoManager_Path, 				"VIDEO_API", IOSNativeSettings.Instance.EnableMediaPlayerAPI);
		ChnageDefineState(ISN_MediaController_Path, 			"VIDEO_API", IOSNativeSettings.Instance.EnableMediaPlayerAPI);
		
		ChnageDefineState(ISN_ReplayKit_Path, 					"REPLAY_KIT", IOSNativeSettings.Instance.EnableReplayKit);
		ChnageDefineState(ISN_CloudKit_Path, 					"CLOUD_KIT", IOSNativeSettings.Instance.EnableCloudKit);


		
		
		
		
		
		if(!IOSNativeSettings.Instance.EnableGameCenterAPI) {
			PluginsInstalationUtil.RemoveIOSFile("ISN_GameCenter");
		} else {
			FileStaticAPI.CopyFile(PluginsInstalationUtil.IOS_SOURCE_PATH + "ISN_GameCenter.mm.txt", 	PluginsInstalationUtil.IOS_DESTANATION_PATH + "ISN_GameCenter.mm");
		}
		
		
		if(!IOSNativeSettings.Instance.EnableInAppsAPI) {
			PluginsInstalationUtil.RemoveIOSFile("ISN_InApp");
		} else {
			FileStaticAPI.CopyFile(PluginsInstalationUtil.IOS_SOURCE_PATH + "ISN_InApp.mm.txt", 	PluginsInstalationUtil.IOS_DESTANATION_PATH + "ISN_InApp.mm");
		}
		
		
		if(!IOSNativeSettings.Instance.EnableiAdAPI) {
			PluginsInstalationUtil.RemoveIOSFile("ISN_iAd");
		} else {
			FileStaticAPI.CopyFile(PluginsInstalationUtil.IOS_SOURCE_PATH + "ISN_iAd.mm.txt", 	PluginsInstalationUtil.IOS_DESTANATION_PATH + "ISN_iAd.mm");
		}
		
		
		if(!IOSNativeSettings.Instance.EnableCameraAPI) {
			PluginsInstalationUtil.RemoveIOSFile("ISN_Camera");
		} else {
			FileStaticAPI.CopyFile(PluginsInstalationUtil.IOS_SOURCE_PATH + "ISN_Camera.mm.txt", 	PluginsInstalationUtil.IOS_DESTANATION_PATH + "ISN_Camera.mm");
		}
		
		if(!IOSNativeSettings.Instance.EnableSocialSharingAPI) {
			PluginsInstalationUtil.RemoveIOSFile("ISN_SocialGate");
		} else {
			FileStaticAPI.CopyFile(PluginsInstalationUtil.IOS_SOURCE_PATH + "ISN_SocialGate.mm.txt", 	PluginsInstalationUtil.IOS_DESTANATION_PATH + "ISN_SocialGate.mm");
		}
		
		if(!IOSNativeSettings.Instance.EnableMediaPlayerAPI) {
			PluginsInstalationUtil.RemoveIOSFile("ISN_Media");
		} else {
			FileStaticAPI.CopyFile(PluginsInstalationUtil.IOS_SOURCE_PATH + "ISN_Media.mm.txt", 	PluginsInstalationUtil.IOS_DESTANATION_PATH + "ISN_Media.mm");
		}
		
		if(!IOSNativeSettings.Instance.EnableReplayKit) {
			PluginsInstalationUtil.RemoveIOSFile("ISN_ReplayKit");
		} else {
			FileStaticAPI.CopyFile(PluginsInstalationUtil.IOS_SOURCE_PATH + "ISN_ReplayKit.mm.txt", 	PluginsInstalationUtil.IOS_DESTANATION_PATH + "ISN_ReplayKit.mm");
		}


		if(!IOSNativeSettings.Instance.EnableCloudKit) {
			PluginsInstalationUtil.RemoveIOSFile("ISN_CloudKit");
		} else {
			FileStaticAPI.CopyFile(PluginsInstalationUtil.IOS_SOURCE_PATH + "ISN_CloudKit.mm.txt", 	PluginsInstalationUtil.IOS_DESTANATION_PATH + "ISN_CloudKit.mm");
		}



		if(!IOSNativeSettings.Instance.EnableSoomla) {
			ChnageDefineState(ISN_Soomla_Path, 						"SOOMLA", IOSNativeSettings.Instance.EnableSoomla);
			PluginsInstalationUtil.RemoveIOSFile("ISN_Soomla");
		} else {

			if(FileStaticAPI.IsFileExists("Plugins/IOS/libSoomlaGrowLite.a")) {
				ChnageDefineState(ISN_Soomla_Path, 						"SOOMLA", IOSNativeSettings.Instance.EnableSoomla);
				FileStaticAPI.CopyFile(PluginsInstalationUtil.IOS_SOURCE_PATH + "ISN_Soomla.mm.txt", 	PluginsInstalationUtil.IOS_DESTANATION_PATH + "ISN_Soomla.mm");
			} else {


				bool res = EditorUtility.DisplayDialog("Soomla Grow not found", "IOS Native wasn't able to find Soomla Grow libraryes in your project. Would you like to donwload and install it?", "Download", "No Thanks");
				if(res) {
					Application.OpenURL(IOSNativeSettings.Instance.SoomlaDownloadLink);
				}

				IOSNativeSettings.Instance.EnableSoomla = false;
				UpdatePluginSettings();
			}
		}
		
	}
	
	
	private static void ChnageDefineState(string file, string tag, bool IsEnabled) {
		
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

	private bool ToggleFiled(string titile, bool value) {

		ISN_Bool initialValue = ISN_Bool.Yes;
		if(!value) {
			initialValue = ISN_Bool.No;
		}
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField(titile);

		initialValue = (ISN_Bool) EditorGUILayout.EnumPopup(initialValue);
		if(initialValue == ISN_Bool.Yes) {
			value = true;
		} else {
			value = false;
		}
		EditorGUILayout.EndHorizontal();

		return value;
	}


	/*
		List<GUIContent> ToolbarContent  = new List<GUIContent>();
		GUIContent General =  new GUIContent();
		General.text = "General";


		GUIContent Billing =  new GUIContent();
		Billing.text = "Billing";

		GUIContent GameCenter =  new GUIContent();
		GameCenter.text = "Game Center";

		GUIContent Other =  new GUIContent();
		Other.text = "Other";

		ToolbarContent.Add(General);
		ToolbarContent.Add(Billing);
		ToolbarContent.Add(GameCenter);
		ToolbarContent.Add(Other);

*/
	
}
