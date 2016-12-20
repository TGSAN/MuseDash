using UnityEngine;
using System;
using System.Collections;
#if (UNITY_IPHONE && !UNITY_EDITOR) || SA_DEBUG_MODE
using System.Runtime.InteropServices;
#endif

public class IOSNativeAppEvents : ISN_Singleton<IOSNativeAppEvents> {

	#if (UNITY_IPHONE && !UNITY_EDITOR) || SA_DEBUG_MODE

	[DllImport ("__Internal")]
	private static extern void _ISNsubscribe();
	#endif
	

	public static event Action OnApplicationDidEnterBackground	 = delegate {};
	public static event Action OnApplicationDidBecomeActive = delegate {};
	public static event Action OnApplicationDidReceiveMemoryWarning = delegate {};
	public static event Action OnApplicationWillResignActive = delegate {};
	public static event Action OnApplicationWillTerminate = delegate {};


	
	void Awake() {
		DontDestroyOnLoad(gameObject);
		#if (UNITY_IPHONE && !UNITY_EDITOR) || SA_DEBUG_MODE
			_ISNsubscribe();
		#endif
	}

	public void Subscribe() {

	}


	private void applicationDidEnterBackground() {
		OnApplicationDidEnterBackground();
	}
	
	private void applicationDidBecomeActive() {
		OnApplicationDidBecomeActive();
	}
	
	private void applicationDidReceiveMemoryWarning() {
		OnApplicationDidReceiveMemoryWarning();
	}
	
	
	private void applicationWillResignActive() {
		OnApplicationWillResignActive();
	}
	
	
	private void applicationWillTerminate() {
		OnApplicationWillTerminate();
	}


}

