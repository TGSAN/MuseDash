//#define PUSH_ENABLED
////////////////////////////////////////////////////////////////////////////////
//  
// @module IOS Native Plugin for Unity3D 
// @author Osipov Stanislav (Stan's Assets) 
// @support stans.assets@gmail.com 
//
////////////////////////////////////////////////////////////////////////////////



using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if (UNITY_IPHONE && !UNITY_EDITOR) || SA_DEBUG_MODE
using System.Runtime.InteropServices;
#endif




#if UNITY_3_5 || UNITY_4_0 || UNITY_4_0_1 || UNITY_4_1 || UNITY_4_2 || UNITY_4_3 || UNITY_4_5 || UNITY_4_6 
#else
#if UNITY_IOS || UNITY_IPHONE
using UnityEngine.iOS;
#endif

#endif






public class IOSNotificationController : ISN_Singleton<IOSNotificationController> {


	private static IOSNotificationController _instance;
	private static int _AllowedNotificationsType = -1;

	private const string PP_KEY = "IOSNotificationControllerKey";
	private const string PP_ID_KEY = "IOSNotificationControllerrKey_ID";


	private ISN_LocalNotification _LaunchNotification = null;
	

	//Actions
	public static event Action<IOSNotificationDeviceToken> OnDeviceTokenReceived = delegate {};
	public static event Action<ISN_Result>  OnNotificationScheduleResult = delegate {};
	public static event Action<int>  OnNotificationSettingsInfoResult = delegate {};

	public static event Action<ISN_LocalNotification>  OnLocalNotificationReceived = delegate {};

	#if PUSH_ENABLED || SA_DEBUG_MODE

	#if UNITY_IOS || UNITY_IPHONE

	#if UNITY_3_5 || UNITY_4_0 || UNITY_4_0_1 || UNITY_4_1 || UNITY_4_2 || UNITY_4_3 || UNITY_4_5 || UNITY_4_6
	public static event Action<RemoteNotification> OnRemoteNotificationReceived = delegate {};
	#else
	public static event Action<UnityEngine.iOS.RemoteNotification> OnRemoteNotificationReceived = delegate {};
	#endif

	#endif


	#endif
	

	#if (UNITY_IPHONE && !UNITY_EDITOR) || SA_DEBUG_MODE
	[DllImport ("__Internal")]
	private static extern void _ISN_ScheduleNotification (int time, string message, bool sound, string nId, int badges, string data, string soundName);

	[DllImport ("__Internal")]
	private static extern void _ISN_CancelNotifications();


	[DllImport ("__Internal")]
	private static extern void _ISN_RequestNotificationPermissions();

	[DllImport ("__Internal")]
	private static extern void _ISN_CancelNotificationById(string nId);

	[DllImport ("__Internal")]
	private static extern  void _ISN_ApplicationIconBadgeNumber (int badges);


	[DllImport ("__Internal")]
	private static extern void _ISN_RegisterForRemoteNotifications(int types);


	[DllImport ("__Internal")]
	private static extern void _ISN_RequestNotificationSettings();

	#endif

	

	//--------------------------------------
	// INITIALIZE
	//--------------------------------------
	

	void Awake() {

		DontDestroyOnLoad(gameObject);

		#if UNITY_IPHONE || UNITY_IOS

		#if UNITY_3_5 || UNITY_4_0 || UNITY_4_0_1 || UNITY_4_1 || UNITY_4_2 || UNITY_4_3 || UNITY_4_5 || UNITY_4_6
		if( NotificationServices.localNotificationCount > 0) {
			LocalNotification n = NotificationServices.localNotifications[0];
			
			ISN_LocalNotification notif = new ISN_LocalNotification(DateTime.Now, n.alertBody, true);
			
			int id = 0;
			if(n.userInfo.Contains("AlarmKey")) {
				id = System.Convert.ToInt32(n.userInfo["AlarmKey"]);
			}
			
			if(n.userInfo.Contains("data")) {
				notif.SetData(System.Convert.ToString(n.userInfo["data"]));
			}
			notif.SetId(id);

			_LaunchNotification = notif;
		}
		#else

			#if UNITY_IOS
			if( UnityEngine.iOS.NotificationServices.localNotificationCount > 0) {
				UnityEngine.iOS.LocalNotification n = UnityEngine.iOS.NotificationServices.localNotifications[0];
				
				ISN_LocalNotification notif = new ISN_LocalNotification(DateTime.Now, n.alertBody, true);
				
				int id = 0;
				if(n.userInfo.Contains("AlarmKey")) {
					id = System.Convert.ToInt32(n.userInfo["AlarmKey"]);
				}
				
				if(n.userInfo.Contains("data")) {
					notif.SetData(System.Convert.ToString(n.userInfo["data"]));
				}
				notif.SetId(id);
					
				_LaunchNotification = notif;
			}
			#endif

		#endif


		#endif
	}



	#if (UNITY_IPHONE && !UNITY_EDITOR && PUSH_ENABLED && PUSH_ENABLED) || SA_DEBUG_MODE
	void FixedUpdate() {
		if(NotificationServices.remoteNotificationCount > 0) {
			foreach(var rn in NotificationServices.remoteNotifications) {
				if(!IOSNativeSettings.Instance.DisablePluginLogs) 
					UnityEngine.Debug.Log("Remote Noti: " + rn.alertBody);
				//IOSNotificationController.instance.ShowNotificationBanner("", rn.alertBody);
				OnRemoteNotificationReceived(rn);
			}
			NotificationServices.ClearRemoteNotifications();
		}
	}
	#endif




	#if UNITY_IPHONE

	#if UNITY_3_5 || UNITY_4_0 || UNITY_4_0_1 || UNITY_4_1 || UNITY_4_2 || UNITY_4_3 || UNITY_4_5 || UNITY_4_6
	public void RegisterForRemoteNotifications(RemoteNotificationType notificationTypes) {
	#else
	public void RegisterForRemoteNotifications(NotificationType notificationTypes) {;
	#endif



		#if (UNITY_IPHONE && !UNITY_EDITOR && PUSH_ENABLED) || SA_DEBUG_MODE

		string sysInfo = SystemInfo.operatingSystem;
		sysInfo = sysInfo.Replace("iPhone OS ", "");
		string[] chunks = sysInfo.Split('.');
		int majorVersion = int.Parse(chunks[0]);
		if (majorVersion >= 8) {
			_ISN_RegisterForRemoteNotifications((int) notificationTypes);
		} 

		#if UNITY_3_5 || UNITY_4_0 || UNITY_4_0_1 || UNITY_4_1 || UNITY_4_2 || UNITY_4_3 || UNITY_4_5 || UNITY_4_6
		NotificationServices.RegisterForRemoteNotificationTypes(notificationTypes);
		#else
		NotificationServices.RegisterForNotifications(notificationTypes);
		#endif

		DeviceTokenListener.Create ();

		#endif
	}
	#endif

	//--------------------------------------
	//  PUBLIC METHODS
	//--------------------------------------

	public void RequestNotificationPermissions() {
			if(ISN_Device.CurrentDevice.MajorSystemVersion < 8) {
				return;
			}


			#if (UNITY_IPHONE && !UNITY_EDITOR) || SA_DEBUG_MODE
			_ISN_RequestNotificationPermissions ();
		#endif

	}
	

	public void ShowGmaeKitNotification (string title, string message) {
		GameCenterManager.ShowGmaeKitNotification(title, message);
	}

	[System.Obsolete("ShowNotificationBanner is deprecated, please use ShowGmaeKitNotification instead.")]
	public void ShowNotificationBanner (string title, string message) {
		ShowGmaeKitNotification(title, message);
	}

	[System.Obsolete("CancelNotifications is deprecated, please use CancelAllLocalNotifications instead.")]
	public void CancelNotifications () {
		CancelAllLocalNotifications();
	}


	public void CancelAllLocalNotifications () {
			SaveNotifications(new List<ISN_LocalNotification>());

			#if (UNITY_IPHONE && !UNITY_EDITOR) || SA_DEBUG_MODE
			_ISN_CancelNotifications();
		#endif
	}


	public void CancelLocalNotification (ISN_LocalNotification notification) {
		CancelLocalNotificationById(notification.Id);
	}


	public void CancelLocalNotificationById (int notificationId) {

		#if (UNITY_IPHONE && !UNITY_EDITOR) || SA_DEBUG_MODE

			List<ISN_LocalNotification> scheduled = LoadPendingNotifications();
			List<ISN_LocalNotification> newList =  new List<ISN_LocalNotification>();
			
			foreach(ISN_LocalNotification n in scheduled) {
				if(n.Id != notificationId) {
					newList.Add(n);
				}
			}

			SaveNotifications(newList);
				


		
			_ISN_CancelNotificationById(notificationId.ToString());
		#endif
	}


	public void ScheduleNotification (ISN_LocalNotification notification) {

		#if (UNITY_IPHONE && !UNITY_EDITOR) || SA_DEBUG_MODE

			int time =  System.Convert.ToInt32((notification.Date -DateTime.Now).TotalSeconds); 

			List<ISN_LocalNotification> scheduled = LoadPendingNotifications();
			scheduled.Add(notification);
			SaveNotifications(scheduled);



			_ISN_ScheduleNotification (time, notification.Message, notification.UseSound, notification.Id.ToString(), notification.Badges, notification.Data, notification.SoundName);
		#endif
	}


	public  List<ISN_LocalNotification> LoadPendingNotifications(bool includeAll = false) {
		#if UNITY_IPHONE

		string data = string.Empty;
		if(PlayerPrefs.HasKey(PP_KEY)) {
			data = PlayerPrefs.GetString(PP_KEY);
		}

		List<ISN_LocalNotification>  tpls = new List<ISN_LocalNotification>();
		
		if(data != string.Empty) {
			string[] notifications = data.Split(SA_DataConverter.DATA_SPLITTER);

			foreach(string n in notifications) {
				
				String templateData = System.Text.Encoding.UTF8.GetString(System.Convert.FromBase64String(n) );
				
				try {
					ISN_LocalNotification notification = new ISN_LocalNotification(templateData);
					
					if(!notification.IsFired || includeAll) {
						tpls.Add(notification);
					}
				} catch(Exception e) {
					Debug.Log("IOS Native. IOSNotificationController loading notification data failed: " + e.Message);
				}
				
			}
		}
		return tpls;
		#else
		return null;
		#endif
		
		
	}



	public void ApplicationIconBadgeNumber (int badges) {
			#if (UNITY_IPHONE && !UNITY_EDITOR) || SA_DEBUG_MODE
			_ISN_ApplicationIconBadgeNumber (badges);
		#endif

	}

	public void RequestNotificationSettings () {
			#if (UNITY_IPHONE && !UNITY_EDITOR) || SA_DEBUG_MODE
		_ISN_RequestNotificationSettings ();
		#endif
		
	}


	
	//--------------------------------------
	//  GET/SET
	//--------------------------------------


	public static int AllowedNotificationsType {
		get {
			return _AllowedNotificationsType;
		}
	}

	public ISN_LocalNotification LaunchNotification {
		get {
			return _LaunchNotification;
		}
	}
	
	//--------------------------------------
	//  EVENTS
	//--------------------------------------

	public void OnDeviceTockeReceivedAction (IOSNotificationDeviceToken token) {
		OnDeviceTokenReceived(token);
	}

	private void OnNotificationScheduleResultAction (string array) {

		string[] data;
		data = array.Split("|" [0]);


		ISN_Result result = null;

		if(data[0].Equals("0")) {
			result =  new ISN_Result(false);
		} else {
			result =  new ISN_Result(true);
		}


		_AllowedNotificationsType = System.Convert.ToInt32(data[1]);

		OnNotificationScheduleResult(result);

	
	}

	private void OnNotificationSettingsInfoRetrived(string data) {
			int types = System.Convert.ToInt32(data);
			OnNotificationSettingsInfoResult(types);
	}

	private void OnLocalNotificationReceived_Event(string array) {
		string[] data;
		data = array.Split("|" [0]);

		string msg = data[0];
		int Id = System.Convert.ToInt32(data[1]);
		string notifDta = data[2];
		int badges = System.Convert.ToInt32(data[3]);

		ISN_LocalNotification n =  new ISN_LocalNotification(DateTime.Now, msg);
		n.SetData(notifDta);
		n.SetBadgesNumber(badges);
		n.SetId(Id);

		OnLocalNotificationReceived(n);
	}
	
	//--------------------------------------
	//  PRIVATE METHODS
	//--------------------------------------


	private void SaveNotifications(List<ISN_LocalNotification> notifications) {
		
		if(notifications.Count == 0) {
			PlayerPrefs.DeleteKey(PP_KEY);
			return;
		}
		
		string srialzedNotifications = "";
		int len = notifications.Count;
		for(int i = 0; i < len; i++) {
			if(i != 0) {
				srialzedNotifications += SA_DataConverter.DATA_SPLITTER;
			}
			
			srialzedNotifications += notifications[i].SerializedString;
		}
		
		PlayerPrefs.SetString(PP_KEY, srialzedNotifications);
	}

	
	//--------------------------------------
	//  DESTROY
	//--------------------------------------

}
