////////////////////////////////////////////////////////////////////////////////
//  
// @module IOS Native Plugin for Unity3D 
// @author Osipov Stanislav (Stan's Assets) 
// @support stans.assets@gmail.com 
//
////////////////////////////////////////////////////////////////////////////////



using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PaymentManagerExample {
	
	
	//--------------------------------------
	// INITIALIZE
	//--------------------------------------
	
	public const string SMALL_PACK 	=  "your.product.id1.here";
	public const string NC_PACK 	=  "your.product.id2.here";
	


	private static bool IsInitialized = false;
	public static void init() {


		if(!IsInitialized) {

			//You do not have to add products by code if you already did it in seetings guid
			//Windows -> IOS Native -> Edit Settings
			//Billing tab.
			IOSInAppPurchaseManager.Instance.AddProductId(SMALL_PACK);
			IOSInAppPurchaseManager.Instance.AddProductId(NC_PACK);
			


			//Event Use Examples
			IOSInAppPurchaseManager.OnVerificationComplete += HandleOnVerificationComplete;
			IOSInAppPurchaseManager.OnStoreKitInitComplete += OnStoreKitInitComplete;


			IOSInAppPurchaseManager.OnTransactionComplete += OnTransactionComplete;
			IOSInAppPurchaseManager.OnRestoreComplete += OnRestoreComplete;


			IsInitialized = true;

		} 

		IOSInAppPurchaseManager.Instance.LoadStore();


	}



	//--------------------------------------
	//  PUBLIC METHODS
	//--------------------------------------
	
	
	public static void buyItem(string productId) {
		IOSInAppPurchaseManager.Instance.BuyProduct(productId);
	}
	
	//--------------------------------------
	//  GET/SET
	//--------------------------------------
	
	//--------------------------------------
	//  EVENTS
	//--------------------------------------


	private static void UnlockProducts(string productIdentifier) {
		switch(productIdentifier) {
		case SMALL_PACK:
			//code for adding small game money amount here
			break;
		case NC_PACK:
			//code for unlocking cool item here
			break;
			
		}
	}

	private static void OnTransactionComplete (IOSStoreKitResult result) {

		Debug.Log("OnTransactionComplete: " + result.ProductIdentifier);
		Debug.Log("OnTransactionComplete: state: " + result.State);

		switch(result.State) {
		case InAppPurchaseState.Purchased:
		case InAppPurchaseState.Restored:
			//Our product been succsesly purchased or restored
			//So we need to provide content to our user depends on productIdentifier
			UnlockProducts(result.ProductIdentifier);
			break;
		case InAppPurchaseState.Deferred:
			//iOS 8 introduces Ask to Buy, which lets parents approve any purchases initiated by children
			//You should update your UI to reflect this deferred state, and expect another Transaction Complete  to be called again with a new transaction state 
			//reflecting the parent’s decision or after the transaction times out. Avoid blocking your UI or gameplay while waiting for the transaction to be updated.
			break;
		case InAppPurchaseState.Failed:
			//Our purchase flow is failed.
			//We can unlock intrefase and repor user that the purchase is failed. 
			Debug.Log("Transaction failed with error, code: " + result.Error.Code);
			Debug.Log("Transaction failed with error, description: " + result.Error.Description);


			break;
		}

		if(result.State == InAppPurchaseState.Failed) {
			IOSNativePopUpManager.showMessage("Transaction Failed", "Error code: " + result.Error.Code + "\n" + "Error description:" + result.Error.Description);
		} else {
			IOSNativePopUpManager.showMessage("Store Kit Response", "product " + result.ProductIdentifier + " state: " + result.State.ToString());
		}

	}
 

	private static void OnRestoreComplete (IOSStoreKitRestoreResult res) {
		if(res.IsSucceeded) {
			IOSNativePopUpManager.showMessage("Success", "Restore Compleated");
		} else {
			IOSNativePopUpManager.showMessage("Error: " + res.Error.Code, res.Error.Description);
		}
	}	


	static void HandleOnVerificationComplete (IOSStoreKitVerificationResponse response) {
		IOSNativePopUpManager.showMessage("Verification", "Transaction verification status: " + response.status.ToString());
		
		Debug.Log("ORIGINAL JSON: " + response.originalJSON);
	}
	

	private static void OnStoreKitInitComplete(ISN_Result result) {

		if(result.IsSucceeded) {

			int avaliableProductsCount = 0;
			foreach(IOSProductTemplate tpl in IOSInAppPurchaseManager.instance.Products) {
				if(tpl.IsAvaliable) {
					avaliableProductsCount++;
				}
			}

			IOSNativePopUpManager.showMessage("StoreKit Init Succeeded", "Available products count: " + avaliableProductsCount);
			Debug.Log("StoreKit Init Succeeded Available products count: " + avaliableProductsCount);
		} else {
			IOSNativePopUpManager.showMessage("StoreKit Init Failed",  "Error code: " + result.Error.Code + "\n" + "Error description:" + result.Error.Description);
		}
	}

	
	//--------------------------------------
	//  PRIVATE METHODS
	//--------------------------------------
	
	//--------------------------------------
	//  DESTROY
	//--------------------------------------


}
