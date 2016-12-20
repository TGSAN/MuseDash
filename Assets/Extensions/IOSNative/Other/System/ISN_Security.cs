#define INAPP_API_ENABLED

using UnityEngine;
using System;
using System.Collections;
#if (UNITY_IPHONE && !UNITY_EDITOR && INAPP_API_ENABLED) || SA_DEBUG_MODE
using System.Runtime.InteropServices;
#endif

public class ISN_Security : ISN_Singleton<ISN_Security> { 

	#if (UNITY_IPHONE && !UNITY_EDITOR && INAPP_API_ENABLED) || SA_DEBUG_MODE

	[DllImport ("__Internal")]
	private static extern void _ISN_RetrieveLocalReceipt();
	

	[DllImport ("__Internal")]
	private static extern void _ISN_ReceiptRefreshRequest();

	

	#endif


	public static event Action<ISN_LocalReceiptResult> OnReceiptLoaded = delegate{};
	public static event Action<ISN_Result> OnReceiptRefreshComplete = delegate{};


	void Awake() {
		DontDestroyOnLoad(gameObject);
	}


	public void RetrieveLocalReceipt() {
		#if (UNITY_IPHONE && !UNITY_EDITOR && INAPP_API_ENABLED) || SA_DEBUG_MODE
		_ISN_RetrieveLocalReceipt();
		#endif
	}

	

	public void StartReceiptRefreshRequest() {
		#if (UNITY_IPHONE && !UNITY_EDITOR && INAPP_API_ENABLED) || SA_DEBUG_MODE
		_ISN_ReceiptRefreshRequest();
		#endif
	}


	

	private void Event_ReceiptLoaded(string data) {
		ISN_LocalReceiptResult result =  new ISN_LocalReceiptResult(data);
		OnReceiptLoaded(result);
	}

	private void Event_ReceiptRefreshRequestReceived(string data) {
	
		ISN_Result result;
		if(data.Equals("1")) {
			result =  new ISN_Result(true);
		} else {
			result =  new ISN_Result(false);
		}

		OnReceiptRefreshComplete(result);
	}



}
