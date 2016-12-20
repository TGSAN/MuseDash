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
using System.Collections.Generic;


public class IOSInAppPurchaseManager : ISN_Singleton<IOSInAppPurchaseManager> {


	public const string APPLE_VERIFICATION_SERVER   = "https://buy.itunes.apple.com/verifyReceipt";
	public const string SANDBOX_VERIFICATION_SERVER = "https://sandbox.itunes.apple.com/verifyReceipt";
	

	//Actions
	public static event Action OnRestoreStarted 				= delegate{};
	public static event Action<string> OnTransactionStarted 	= delegate{};


	public static event Action<IOSStoreKitResult> OnTransactionComplete = delegate{};
	public static event Action<IOSStoreKitRestoreResult> OnRestoreComplete = delegate{};
	public static event Action<ISN_Result> OnStoreKitInitComplete = delegate{};
	public static event Action<bool> OnPurchasesStateSettingsLoaded = delegate{};
	public static event Action<IOSStoreKitVerificationResponse> OnVerificationComplete = delegate{};

	
	private bool _IsStoreLoaded = false;
	private bool _IsWaitingLoadResult = false;
	private static int _nextId = 1;


	private Dictionary<int, IOSStoreProductView> _productsView =  new Dictionary<int, IOSStoreProductView>(); 

	private static string lastPurchasedProduct;
	
	//--------------------------------------
	// INITIALIZE
	//--------------------------------------
	

	void Awake() {
		DontDestroyOnLoad(gameObject);
	}


	//--------------------------------------
	//  PUBLIC METHODS
	//--------------------------------------



	[System.Obsolete("loadStore is deprecated, please use LoadStore instead.")]
	public void loadStore() {
		LoadStore();
	}

	public void LoadStore() {

		if(_IsStoreLoaded) {
			Invoke("FireSuccessInitEvent", 1f);
			return;
		}

		if(_IsWaitingLoadResult) {
			return;
		}

		_IsWaitingLoadResult = true;


		string ids = "";
		int len = Products.Count;
		for(int i = 0; i < len; i++) {
			if(i != 0) {
				ids += ",";
			}
			
			ids += Products[i].Id;
		}

		ISN_SoomlaGrow.Init();

		#if !UNITY_EDITOR
		IOSNativeMarketBridge.loadStore(ids);
		#else
		if(IOSNativeSettings.Instance.SendFakeEventsInEditor) {
			Invoke("EditorFakeInitEvent", 1f);
		}
		#endif
		
	}
	

	[System.Obsolete("buyProduct is deprecated, please use BuyProduct instead.")]
	public void buyProduct(string productId) {
		BuyProduct(productId);
	}

	public void BuyProduct(string productId) {



		#if !UNITY_EDITOR

		OnTransactionStarted(productId);
	
		if(!_IsStoreLoaded) {

			if(!IOSNativeSettings.Instance.DisablePluginLogs) 
				Debug.LogWarning("buyProduct shouldn't be called before StoreKit is initialized"); 
			SendTransactionFailEvent(productId, "StoreKit not yet initialized", IOSTransactionErrorCode.SKErrorPaymentNotAllowed);

			return;
		} 

		IOSNativeMarketBridge.buyProduct(productId);

		#else
		if(IOSNativeSettings.Instance.SendFakeEventsInEditor) {
			FireProductBoughtEvent(productId, "", "", "", false);
		}
		#endif
	}


	[System.Obsolete("addProductId is deprecated, please use AddProductId instead.")]
	public void addProductId(string productId) {
		AddProductId(productId);
	}
	
	public void AddProductId(string productId) {

		IOSProductTemplate tpl  = new IOSProductTemplate();
		tpl.Id = productId;
		AddProductId(tpl);
	}


	public void AddProductId(IOSProductTemplate product) {

		bool IsPordcutAlreadyInList = false;
		int replaceIndex = 0;
		foreach(IOSProductTemplate p in Products) {
			if(p.Id.Equals(product.Id)) {
				IsPordcutAlreadyInList = true;
				replaceIndex = Products.IndexOf(p);
				break;
			}
		}

		if(IsPordcutAlreadyInList) {
			Products[replaceIndex] = product;
		} else {
			Products.Add(product);
		}
	}

	public IOSProductTemplate GetProductById(string prodcutId) {
		foreach(IOSProductTemplate p in Products) {
			if(p.Id.Equals(prodcutId)) {
				return p;
			}
		} 

		IOSProductTemplate tpl =  new IOSProductTemplate();
		tpl.Id = prodcutId;
		Products.Add(tpl);

		return tpl;
	}
	

	[System.Obsolete("restorePurchases is deprecated, please use RestorePurchases instead.")]
	public void restorePurchases() {
		RestorePurchases();
	}

	public void RestorePurchases() {

		if(!_IsStoreLoaded) {

			ISN_Error e = new ISN_Error((int) IOSTransactionErrorCode.SKErrorPaymentServiceNotInitialized, "Store Kit Initilizations required"); 

			IOSStoreKitRestoreResult r =  new IOSStoreKitRestoreResult(e);
			OnRestoreComplete(r);
			return;
		}

		OnRestoreStarted();

		#if !UNITY_EDITOR
		IOSNativeMarketBridge.restorePurchases();
		#else
		if(IOSNativeSettings.Instance.SendFakeEventsInEditor) {
			foreach(IOSProductTemplate product in Products) {
				if(product.ProductType == ISN_InAppType.NonConsumable) {
					Debug.Log("Restored: " + product.Id);
					FireProductBoughtEvent(product.Id, "", "", "", true);
				}
			}

			FireRestoreCompleteEvent();
		}
		#endif
	}


	[System.Obsolete("verifyLastPurchase is deprecated, please use VerifyLastPurchase instead.")]
	public void verifyLastPurchase(string url) {
		VerifyLastPurchase(url);
	}

	public void VerifyLastPurchase(string url) {
		IOSNativeMarketBridge.verifyLastPurchase (url);
	}

	public void RegisterProductView(IOSStoreProductView view) {
		view.SetId(NextId);
		_productsView.Add(view.id, view);
	}
	
	
	//--------------------------------------
	//  GET/SET
	//--------------------------------------

	[System.Obsolete("products is deprecated, please use Products instead.")]
	public List<IOSProductTemplate> products {
		get {
			return Products;
		}
	}

	public List<IOSProductTemplate> Products {
		get {
			return IOSNativeSettings.Instance.InAppProducts;
		}
	}

	public bool IsStoreLoaded {
		get {
			return _IsStoreLoaded;
		}
	}

	public bool IsInAppPurchasesEnabled {
		get {
			return IOSNativeMarketBridge.ISN_InAppSettingState();
		}
	}

	public bool IsWaitingLoadResult {
		get {
			return _IsWaitingLoadResult;
		}
	}

	private static int NextId {
		get {
			_nextId++;
			return _nextId;
		}
	}

	
	//--------------------------------------
	//  EVENTS
	//--------------------------------------


	private void OnStoreKitInitFailed(string data) {

		ISN_Error e =  new ISN_Error(data);

		_IsStoreLoaded = false;
		_IsWaitingLoadResult = false;


		ISN_Result res = new ISN_Result (false);
		res.SetError(e);
		OnStoreKitInitComplete (res);


		if(!IOSNativeSettings.Instance.DisablePluginLogs) 
			Debug.Log("STORE_KIT_INIT_FAILED Error: " + e.Description);
	}
	
	private void onStoreDataReceived(string data) {
		if(data.Equals(string.Empty)) {
			Debug.Log("InAppPurchaseManager, no products avaiable");
			ISN_Result res = new ISN_Result(true);
			OnStoreKitInitComplete(res);
			return;
		}


		string[] storeData = data.Split(IOSNative.DATA_SPLITTER);
		
		for(int i = 0; i < storeData.Length; i+=7) {
			string prodcutId = storeData[i];
			IOSProductTemplate tpl =  GetProductById(prodcutId);


			tpl.DisplayName 	= storeData[i + 1];
			tpl.Description 	= storeData[i + 2];
			tpl.LocalizedPrice 	= storeData[i + 3];
			tpl.Price 			= System.Convert.ToSingle(storeData[i + 4]);
			tpl.CurrencyCode 	= storeData[i + 5];
			tpl.CurrencySymbol 	= storeData[i + 6];
			tpl.IsAvaliable = true;
			
		}
		
		Debug.Log("InAppPurchaseManager, total products in settings: " + Products.Count.ToString());


		int avaliableProductsCount = 0;
		foreach(IOSProductTemplate tpl in Products) {
			if(tpl.IsAvaliable) {
				avaliableProductsCount++;
			}
		}

		Debug.Log("InAppPurchaseManager, total avaliable products" + avaliableProductsCount);
		FireSuccessInitEvent();
	}
	
	private void onProductBought(string array) {

		string[] data;
		data = array.Split("|" [0]);

		bool IsRestored = false;
		if(data [1].Equals("0")) {
			IsRestored = true;
		}

		string productId = data [0];



		FireProductBoughtEvent(productId, data [2], data [3], data [4], IsRestored);

	}

	private void onProductStateDeferred(string productIdentifier) {
		IOSStoreKitResult response = new IOSStoreKitResult (productIdentifier, InAppPurchaseState.Deferred);


		OnTransactionComplete (response);
	}

	
	private void onTransactionFailed(string array) {

		string[] data;
		data = array.Split("|" [0]);

		string prodcutId = data [0];
		int e = System.Convert.ToInt32(data [2]);
		IOSTransactionErrorCode erroCode = (IOSTransactionErrorCode) e;


		SendTransactionFailEvent(prodcutId, data [1], erroCode);
	}
	
	
	private void onVerificationResult(string array) {

		string[] data;
		data = array.Split("|" [0]);

		IOSStoreKitVerificationResponse response = new IOSStoreKitVerificationResponse ();
		response.status = System.Convert.ToInt32(data[0]);
		response.originalJSON = data [1];
		response.receipt = data [2];
		response.productIdentifier = lastPurchasedProduct;

		OnVerificationComplete (response);

	}

	public void onRestoreTransactionFailed(string array) {

		ISN_Error e = new ISN_Error(array);

		IOSStoreKitRestoreResult r =  new IOSStoreKitRestoreResult(e);

		OnRestoreComplete (r);
	}

	public void onRestoreTransactionComplete(string array) {
		FireRestoreCompleteEvent();
	}



	private void OnProductViewLoaded(string viewId) {
		int id = System.Convert.ToInt32(viewId);
		if(_productsView.ContainsKey(id)) {
			_productsView[id].OnContentLoaded();
		}
	}

	private void OnProductViewLoadedFailed(string viewId) {
		int id = System.Convert.ToInt32(viewId);
		if(_productsView.ContainsKey(id)) {
			_productsView[id].OnContentLoadFailed();
		}
	}

	private void OnProductViewDismissed(string viewId) {
		int id = System.Convert.ToInt32(viewId);
		if(_productsView.ContainsKey(id)) {
			_productsView[id].OnProductViewDismissed();
		}
	}

	//--------------------------------------
	//  PRIVATE METHODS
	//--------------------------------------

	private void FireSuccessInitEvent() {
		_IsStoreLoaded = true;
		_IsWaitingLoadResult = false;
		ISN_Result r = new ISN_Result(true);
		OnStoreKitInitComplete(r);
	}


	private void FireRestoreCompleteEvent() {

		IOSStoreKitRestoreResult r =  new IOSStoreKitRestoreResult(true);

		OnRestoreComplete (r);
	}

	private void FireProductBoughtEvent(string productIdentifier, string applicationUsername, string receipt, string transactionIdentifier, bool IsRestored) {

		InAppPurchaseState state;
		if(IsRestored) {
			state = InAppPurchaseState.Restored;
		} else {
			state = InAppPurchaseState.Purchased;
		}

		IOSStoreKitResult response = new IOSStoreKitResult (productIdentifier, state, applicationUsername, receipt, transactionIdentifier);

	
		
		lastPurchasedProduct = response.ProductIdentifier;
		OnTransactionComplete (response);
	}


	private void SendTransactionFailEvent(string productIdentifier, string errorDescribtion, IOSTransactionErrorCode errorCode) {
		IOSStoreKitResult response = new IOSStoreKitResult (productIdentifier, new ISN_Error((int) errorCode, errorDescribtion));

		OnTransactionComplete (response);
	}

	//--------------------------------------
	//  UNITY EDITOR FAKE SECTION
	//--------------------------------------

	private void EditorFakeInitEvent() {


		FireSuccessInitEvent();
	}


	
	//--------------------------------------
	//  DESTROY
	//--------------------------------------

}
