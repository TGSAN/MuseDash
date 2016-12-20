////////////////////////////////////////////////////////////////////////////////
//  
// @module IOS Native Plugin for Unity3D 
// @author Osipov Stanislav (Stan's Assets) 
// @support stans.assets@gmail.com 
//
////////////////////////////////////////////////////////////////////////////////


using System;
#if UNITY_3_5 || UNITY_4_0 || UNITY_4_0_1 || UNITY_4_1 || UNITY_4_2 || UNITY_4_3 || UNITY_4_5 || UNITY_4_6
using UnityEngine;
#else
#if UNITY_IOS
using UnityEngine.iOS;
#endif
#endif


using System.Collections;

public class NotificationExample : BaseIOSFeaturePreview {
	
	
	private int lastNotificationId = 0;
	
	//--------------------------------------
	// INITIALIZE
	//--------------------------------------
	
	
	void Awake() {
		IOSNotificationController.instance.RequestNotificationPermissions();

		IOSNotificationController.OnLocalNotificationReceived += HandleOnLocalNotificationReceived;

		if(IOSNotificationController.Instance.LaunchNotification != null) {
			ISN_LocalNotification notification = IOSNotificationController.Instance.LaunchNotification;

			IOSMessage.Create("Launch Notification", "Messgae: " + notification.Message + "\nNotification Data: " + notification.Data);
		}
	}


	
	//--------------------------------------
	//  PUBLIC METHODS
	//--------------------------------------
	
	void OnGUI() {
		
		UpdateToStartPos();
		
		UnityEngine.GUI.Label(new UnityEngine.Rect(StartX, StartY, UnityEngine.Screen.width, 40), "Local and Push Notifications", style);
		
		
		StartY+= YLableStep;
		if(UnityEngine.GUI.Button(new UnityEngine.Rect(StartX, StartY, buttonWidth, buttonHeight), "Schedule Notification Silent")) {
			IOSNotificationController.OnNotificationScheduleResult += OnNotificationScheduleResult;

			ISN_LocalNotification notification =  new ISN_LocalNotification(DateTime.Now.AddSeconds(5),"Your Notification Text No Sound", false);
			notification.SetData("some_test_data");
			notification.Schedule();

			lastNotificationId = notification.Id;
		}
		
		StartX += XButtonStep;
		if(UnityEngine.GUI.Button(new UnityEngine.Rect(StartX, StartY, buttonWidth, buttonHeight), "Schedule Notification")) {
			IOSNotificationController.OnNotificationScheduleResult += OnNotificationScheduleResult;

			ISN_LocalNotification notification =  new ISN_LocalNotification(DateTime.Now.AddSeconds(5),"Your Notification Text", true);
			notification.SetData("some_test_data");
			notification.SetSoundName("purchase_ok.wav");
			notification.SetBadgesNumber(1);
			notification.Schedule();

			lastNotificationId = notification.Id; 
		}
		
		
		StartX += XButtonStep;
		if(UnityEngine.GUI.Button(new UnityEngine.Rect(StartX, StartY, buttonWidth, buttonHeight), "Cancel All Notifications")) {
			IOSNotificationController.instance.CancelAllLocalNotifications();
			IOSNativeUtility.SetApplicationBagesNumber(0);
		}
		
		StartX += XButtonStep;
		if(UnityEngine.GUI.Button(new UnityEngine.Rect(StartX, StartY, buttonWidth, buttonHeight), "Cansel Last Notification")) {


			IOSNotificationController.instance.CancelLocalNotificationById(lastNotificationId);
		}
		
		
		StartX = XStartPos;
		StartY+= YButtonStep;
		if(UnityEngine.GUI.Button(new UnityEngine.Rect(StartX, StartY, buttonWidth, buttonHeight), "Reg Device For Push Notif. ")) {
			
			
			
			#if UNITY_IPHONE
			
			#if UNITY_3_5 || UNITY_4_0 || UNITY_4_0_1 || UNITY_4_1 || UNITY_4_2 || UNITY_4_3 || UNITY_4_5 || UNITY_4_6
			IOSNotificationController.Instance.RegisterForRemoteNotifications (RemoteNotificationType.Alert |  RemoteNotificationType.Badge |  RemoteNotificationType.Sound);

			IOSNotificationController.OnDeviceTokenReceived += OnDeviceTokenReceived;

			#else
			IOSNotificationController.Instance.RegisterForRemoteNotifications (NotificationType.Alert |  NotificationType.Badge |  NotificationType.Sound);
			IOSNotificationController.OnDeviceTokenReceived += OnDeviceTokenReceived;
			#endif
			
			
			
			#endif
			
			
		}
		
		StartX += XButtonStep;
		if(UnityEngine.GUI.Button(new UnityEngine.Rect(StartX, StartY, buttonWidth, buttonHeight), "Show Game Kit Notification")) {
			IOSNotificationController.instance.ShowGmaeKitNotification("Title", "Message");
		}
		
		
	}

	public void CheckNotificationSettings() {
		IOSNotificationController.OnNotificationSettingsInfoResult += HandleOnNotificationSettingsInfoResult;
		IOSNotificationController.Instance.RequestNotificationSettings();

	}

	void HandleOnNotificationSettingsInfoResult (int avaliableTypes) {
		if((avaliableTypes & ISN_NotificationType.Sound) != 0) {
			//Sound avaliable;
		}
	}
	
	//--------------------------------------
	//  GET/SET
	//--------------------------------------
	
	//--------------------------------------
	//  EVENTS
	//--------------------------------------
	
	private void OnDeviceTokenReceived(IOSNotificationDeviceToken token) {
		UnityEngine.Debug.Log ("OnTokenReceived");
		UnityEngine.Debug.Log (token.tokenString);
		
		IOSDialog.Create("OnTokenReceived", token.tokenString);
		
		IOSNotificationController.OnDeviceTokenReceived -= OnDeviceTokenReceived;
	}
	

	void HandleOnLocalNotificationReceived (ISN_LocalNotification notification) {
		IOSMessage.Create("Notification Received", "Messgae: " + notification.Message + "\nNotification Data: " + notification.Data);
	}

	private void OnNotificationScheduleResult (ISN_Result res) {
		IOSNotificationController.OnNotificationScheduleResult -= OnNotificationScheduleResult;
		
		
		
		string msg = string.Empty;
		
		if(res.IsSucceeded) {
			msg += "Notification was successfully scheduled\n allowed notifications types: \n";
			
			
			if((IOSNotificationController.AllowedNotificationsType & IOSUIUserNotificationType.Alert) != 0) {
				msg += "Alert ";
			}
			
			if((IOSNotificationController.AllowedNotificationsType & IOSUIUserNotificationType.Sound) != 0) {
				msg += "Sound ";
			}
			
			if((IOSNotificationController.AllowedNotificationsType & IOSUIUserNotificationType.Badge) != 0) {
				msg += "Badge ";
			}
			
		} else {
			msg += "Notification scheduling failed";
		}
		
		
		IOSMessage.Create("On Notification Schedule Result", msg);
	}
	
	//--------------------------------------
	//  PRIVATE METHODS
	//--------------------------------------
	
	//--------------------------------------
	//  DESTROY
	//--------------------------------------
	
	
}
