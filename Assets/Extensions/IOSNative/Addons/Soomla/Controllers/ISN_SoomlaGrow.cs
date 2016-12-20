//#define SOOMLA

using UnityEngine;
using System;
using System.Collections;


#if (UNITY_IPHONE && !UNITY_EDITOR && SOOMLA) || SA_DEBUG_MODE
using System.Runtime.InteropServices;
#endif




public class ISN_SoomlaGrow : SA_Singleton<ISN_SoomlaGrow> {


	public static event Action ActionInitialized 	= delegate {};
	public static event Action ActionConnected 		= delegate {};
	public static event Action ActionDisconnected 	= delegate {};


	#if (UNITY_IPHONE && !UNITY_EDITOR && SOOMLA) || SA_DEBUG_MODE
	

	[DllImport ("__Internal")]
	public static extern void _ISN_SM_Init(string gameKey, string envKey);

	[DllImport ("__Internal")]
	public static extern void _ISN_SM_PurchaseStarted(string prodcutId);

	[DllImport ("__Internal")]
	public static extern void _ISN_SM_PurchaseFinished(string prodcut, string priceInMicros, string currency);
	
	[DllImport ("__Internal")]
	public static extern void _ISN_SM_SetPurhsesSupportedState(bool isSupported);
	
	[DllImport ("__Internal")]
	public static extern void _ISN_SM_PurchaseCanceled(string prodcut);
	
	[DllImport ("__Internal")]
	public static extern void _ISN_SM_PurchaseError();
	
	[DllImport ("__Internal")]
	public static extern void _ISN_SM_VerificationFailed();
	
	[DllImport ("__Internal")]
	public static extern void _ISN_SM_RestoreStarted();
	
	[DllImport ("__Internal")]
	public static extern void _ISN_SM_RestoreFinished(bool state);

	[DllImport ("__Internal")]
	public static extern void _ISN_SM_SocialAction(int provider, int actionState, int actionType);
	
	#endif

	private static bool _IsInitialized = false;


	//--------------------------------------
	// INITIALIZE
	//--------------------------------------

	public void CreateObject() {
		DontDestroyOnLoad(gameObject);
	}


	public static void Init() {
		#if (UNITY_IPHONE && !UNITY_EDITOR && SOOMLA) || SA_DEBUG_MODE

		if(!_IsInitialized) {
			_IsInitialized = true;

			Instance.CreateObject();

			_ISN_SM_Init(IOSNativeSettings.Instance.SoomlaGameKey, IOSNativeSettings.Instance.SoomlaEnvKey);

			/*
			IOSSocialManager.OnFacebookPostStart += HandleOnFacebookPostStart;
			IOSSocialManager.OnTwitterPostStart += HandleOnTwitterPostStart;
			IOSSocialManager.OnInstagramPostStart += HandleOnInstagramPostStart;

			IOSSocialManager.OnFacebookPostResult += HandleOnFacebookPostResult;
			IOSSocialManager.OnTwitterPostResult += HandleOnTwitterPostResult;
			IOSSocialManager.OnInstagramPostResult += HandleOnInstagramPostResult;
*/



			IOSInAppPurchaseManager.OnTransactionComplete += HandleOnTransactionComplete;
			IOSInAppPurchaseManager.OnTransactionStarted += HandleOnTransactionStarted;

			IOSInAppPurchaseManager.OnRestoreStarted += HandleOnRestoreStarted;
			IOSInAppPurchaseManager.OnRestoreComplete += HandleOnRestoreComplete;
			IOSInAppPurchaseManager.OnVerificationComplete += HandleOnVerificationComplete;
		}

		#endif
	}


	private static void HandleOnVerificationComplete (IOSStoreKitVerificationResponse res) {
		if(res.status != 0) {
			VerificationFailed();
		}
	}

	private static void HandleOnRestoreComplete (IOSStoreKitRestoreResult res) {
		ISN_SoomlaGrow.RestoreFinished(res.IsSucceeded);
	}

	private static void HandleOnRestoreStarted () {
		ISN_SoomlaGrow.RestoreStarted();
	}

	private static void HandleOnTransactionStarted (string prodcutId) {
		ISN_SoomlaGrow.PurchaseStarted(prodcutId);
	}

	private static void HandleOnTransactionComplete (IOSStoreKitResult res) {
		switch(res.State) {
		case InAppPurchaseState.Purchased:
			IOSProductTemplate tpl = IOSInAppPurchaseManager.Instance.GetProductById(res.ProductIdentifier);
			if(tpl != null) {
				ISN_SoomlaGrow.PurchaseFinished(tpl.Id, tpl.PriceInMicros.ToString(), tpl.CurrencyCode);
			}
			break;
		case InAppPurchaseState.Failed:
			if(res.Error.Code == (int) IOSTransactionErrorCode.SKErrorPaymentCancelled) {
				ISN_SoomlaGrow.PurchaseCanceled(res.ProductIdentifier);
			} else {
				ISN_SoomlaGrow.PurchaseError();
			}
			break;
		}
	}

	//--------------------------------------
	// Public Methods
	//--------------------------------------

	public static void SocialAction(ISN_SoomlaEvent soomlaEvent, ISN_SoomlaAction action, ISN_SoomlaProvider provider) {
		#if (UNITY_IPHONE && !UNITY_EDITOR && SOOMLA) || SA_DEBUG_MODE
		_ISN_SM_SocialAction((int) provider, (int) soomlaEvent,  (int) action);
		#endif
	}

	private static void PurchaseStarted(string prodcutId) {
		#if (UNITY_IPHONE && !UNITY_EDITOR && SOOMLA) || SA_DEBUG_MODE
		_ISN_SM_PurchaseStarted(prodcutId);
		#endif
	}

	private static void PurchaseFinished(string prodcutId, string priceInMicros, string currency) {
		#if (UNITY_IPHONE && !UNITY_EDITOR && SOOMLA) || SA_DEBUG_MODE
		_ISN_SM_PurchaseFinished(prodcutId, priceInMicros, currency);
		#endif
	}

	private static void PurchaseCanceled(string prodcutId) {
		#if (UNITY_IPHONE && !UNITY_EDITOR && SOOMLA) || SA_DEBUG_MODE
		_ISN_SM_PurchaseCanceled(prodcutId);
		#endif
	}

	public static void SetPurhsesSupportedState(bool isSupported) {
		#if (UNITY_IPHONE && !UNITY_EDITOR && SOOMLA) || SA_DEBUG_MODE
		_ISN_SM_SetPurhsesSupportedState(isSupported);
		#endif
	}


	private static void PurchaseError() {
		#if (UNITY_IPHONE && !UNITY_EDITOR && SOOMLA) || SA_DEBUG_MODE
		_ISN_SM_PurchaseError();
		#endif
	}
	
	private static void VerificationFailed() {
		#if (UNITY_IPHONE && !UNITY_EDITOR && SOOMLA) || SA_DEBUG_MODE
		_ISN_SM_VerificationFailed();
		#endif
	}


	private static void RestoreStarted() {
		#if (UNITY_IPHONE && !UNITY_EDITOR && SOOMLA) || SA_DEBUG_MODE
		_ISN_SM_RestoreStarted();
		#endif
	}

	private static void RestoreFinished(bool state) {
		#if (UNITY_IPHONE && !UNITY_EDITOR && SOOMLA) || SA_DEBUG_MODE
		_ISN_SM_RestoreFinished(state);
		#endif
	}


	//--------------------------------------
	// Get / Set
	//--------------------------------------

	
	public static bool IsInitialized {
		get {
			return _IsInitialized;
		}
	}


	//--------------------------------------
	// Private Methods 
	//--------------------------------------
	


	//--------------------------------------
	// Events 
	//--------------------------------------

	private void OnHihgWayInitialized() {
		ActionInitialized();
	}

	private void OnHihgWayConnected() {
		ActionConnected();
	}

	private void OnHihgWayDisconnected() {
		ActionDisconnected();
	}


	private static void HandleOnInstagramPostResult (ISN_Result res) {
		if(res.IsSucceeded) {
			SocialAction(ISN_SoomlaEvent.FINISHED, ISN_SoomlaAction.UPDATE_STORY, ISN_SoomlaProvider.INSTAGRAM);
		} else {
			SocialAction(ISN_SoomlaEvent.FAILED, ISN_SoomlaAction.UPDATE_STORY, ISN_SoomlaProvider.INSTAGRAM);
		}
	}

	
	private static void HandleOnTwitterPostResult (ISN_Result res) {
		if(res.IsSucceeded) {
			SocialAction(ISN_SoomlaEvent.FINISHED, ISN_SoomlaAction.UPDATE_STORY, ISN_SoomlaProvider.TWITTER);
		} else {
			SocialAction(ISN_SoomlaEvent.FAILED, ISN_SoomlaAction.UPDATE_STORY, ISN_SoomlaProvider.TWITTER);
		}
		
	}

	private static void HandleOnInstagramPostStart () {
		SocialAction(ISN_SoomlaEvent.STARTED, ISN_SoomlaAction.UPDATE_STORY, ISN_SoomlaProvider.INSTAGRAM);
	}
	
	private static void HandleOnTwitterPostStart () {
		SocialAction(ISN_SoomlaEvent.STARTED, ISN_SoomlaAction.UPDATE_STORY, ISN_SoomlaProvider.TWITTER);
	}


	static void HandleOnFacebookPostStart () {
		SocialAction(ISN_SoomlaEvent.STARTED, ISN_SoomlaAction.UPDATE_STORY, ISN_SoomlaProvider.FACEBOOK);
	}
	
	private static void HandleOnFacebookPostResult (ISN_Result res) {
		if(res.IsSucceeded) {
			SocialAction(ISN_SoomlaEvent.FINISHED, ISN_SoomlaAction.UPDATE_STORY, ISN_SoomlaProvider.FACEBOOK);
		} else {
			SocialAction(ISN_SoomlaEvent.CANCELLED, ISN_SoomlaAction.UPDATE_STORY, ISN_SoomlaProvider.FACEBOOK);
		}
	}

}
