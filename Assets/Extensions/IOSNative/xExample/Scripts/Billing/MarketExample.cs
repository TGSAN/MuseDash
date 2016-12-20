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

public class MarketExample : BaseIOSFeaturePreview {

	//--------------------------------------
	// INITIALIZE
	//--------------------------------------
	
	void Awake() {

		//Best practise is to init billing on app launch
		//But for this example we will use a button for initialization
		//PaymentManagerExample.init();
	}

	//--------------------------------------
	//  PUBLIC METHODS
	//--------------------------------------
	
	void OnGUI() {




		UpdateToStartPos();
		
		GUI.Label(new Rect(StartX, StartY, Screen.width, 40), "In-App Purchases", style);



		StartY+= YLableStep;
		if(GUI.Button(new Rect(StartX, StartY, buttonWidth, buttonHeight), "Init")) {
			PaymentManagerExample.init();
		}


		if(IOSInAppPurchaseManager.Instance.IsStoreLoaded) {
			GUI.enabled = true;
		} else {
			GUI.enabled = false;
		}


		StartX = XStartPos;
		StartY+= YButtonStep;

		if(GUI.Button(new Rect(StartX, StartY, buttonWidth, buttonHeight), "Perform Buy #1")) {
			PaymentManagerExample.buyItem(PaymentManagerExample.SMALL_PACK);

		}

		StartX += XButtonStep;
		if(GUI.Button(new Rect(StartX, StartY, buttonWidth, buttonHeight), "Perform Buy #2")) {
			PaymentManagerExample.buyItem(PaymentManagerExample.NC_PACK);
		}

		StartX += XButtonStep;
		if(GUI.Button(new Rect(StartX, StartY, buttonWidth, buttonHeight), "Restore Purchases")) {
			IOSInAppPurchaseManager.Instance.RestorePurchases();

		}


		StartX = XStartPos;
		StartY+= YButtonStep;
		if(GUI.Button(new Rect(StartX, StartY, buttonWidth, buttonHeight), "Verify Last Purchases")) {
			IOSInAppPurchaseManager.Instance.VerifyLastPurchase(IOSInAppPurchaseManager.SANDBOX_VERIFICATION_SERVER);
		}

		StartX += XButtonStep;
		if(GUI.Button(new Rect(StartX, StartY, buttonWidth, buttonHeight), "Load Product View")) {
			IOSStoreProductView view =  new IOSStoreProductView("333700869");
			view.Dismissed += StoreProductViewDisnissed;
			view.Load();
		}


		StartX += XButtonStep;
		if(GUI.Button(new Rect(StartX, StartY, buttonWidth, buttonHeight), "Is Payments Enabled On device")) {
			IOSNativePopUpManager.showMessage("Payments Settings State", "Is Payments Enabled: " + IOSInAppPurchaseManager.Instance.IsInAppPurchasesEnabled);
		}


		StartX = XStartPos;
		StartY+= YButtonStep;
		StartY+= YLableStep;

		GUI.enabled = true;
		GUI.Label(new Rect(StartX, StartY, Screen.width, 40), "Local Receipt Validation", style);
		
		StartY+= YLableStep;
		if(GUI.Button(new Rect(StartX, StartY, buttonWidth + 10, buttonHeight), "Load Receipt")) {
			ISN_Security.OnReceiptLoaded += OnReceiptLoaded;
			ISN_Security.Instance.RetrieveLocalReceipt();
		}


	}

	void StoreProductViewDisnissed () {
		Debug.Log("Store Product View was Disnissed");
	}	


	
	//--------------------------------------
	//  GET/SET
	//--------------------------------------
	
	//--------------------------------------
	//  EVENTS
	//--------------------------------------
	


	void OnReceiptLoaded (ISN_LocalReceiptResult result) {
		Debug.Log("OnReceiptLoaded");
		ISN_Security.OnReceiptLoaded -= OnReceiptLoaded;
		if(result.Receipt != null) {

			IOSNativePopUpManager.showMessage("Success", "Receipt loaded, byte array length: " + result.Receipt.Length);
		} else {
			IOSDialog dialog =  IOSDialog.Create("Failed", "No Receipt found on the device. Would you like to refresh local Receipt?");
			dialog.OnComplete += OnComplete;

		}
	}

	void OnComplete (IOSDialogResult res) {
		if(res == IOSDialogResult.YES) {
			ISN_Security.OnReceiptRefreshComplete += OnReceiptRefreshComplete;
			ISN_Security.Instance.StartReceiptRefreshRequest();
		}
	}

	void OnReceiptRefreshComplete (ISN_Result res) {
		if(res.IsSucceeded) {

			IOSDialog dialog =  IOSDialog.Create("Success", "Receipt Refreshed, would you like to check it again?");
			dialog.OnComplete += Dialog_RetrieveLocalReceipt;
			


		} else {
			IOSNativePopUpManager.showMessage("Fail", "Receipt Refresh Failed");
		}


	}

	void Dialog_RetrieveLocalReceipt (IOSDialogResult res) {
		if(res == IOSDialogResult.YES) {
			ISN_Security.OnReceiptLoaded += OnReceiptLoaded;
			ISN_Security.Instance.RetrieveLocalReceipt();
		}
	}

	
	//--------------------------------------
	//  PRIVATE METHODS
	//--------------------------------------
	
	//--------------------------------------
	//  DESTROY
	//--------------------------------------

}
