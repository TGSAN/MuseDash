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

public class PopUpExamples : BaseIOSFeaturePreview {

	//--------------------------------------
	// INITIALIZE
	//--------------------------------------

	void Awake() {
		IOSNativePopUpManager.showMessage ("Welcome", "Hey there, welcome to the Pop-ups testing scene!");
	}

	//--------------------------------------
	//  PUBLIC METHODS
	//--------------------------------------
	

	void OnGUI() {

		UpdateToStartPos();
		
		GUI.Label(new Rect(StartX, StartY, Screen.width, 40), "Native Pop-ups", style);

		StartY+= YLableStep;
		if(GUI.Button(new Rect(StartX, StartY, buttonWidth, buttonHeight), "Rate Pop-up with events")) {
			IOSRateUsPopUp rate = IOSRateUsPopUp.Create("Like this game?", "Please rate to support future updates!");
			rate.OnComplete += onRatePopUpClose;
		
		}
		

		StartX += XButtonStep;
		if(GUI.Button(new Rect(StartX, StartY, buttonWidth, buttonHeight), "Dialog Pop-up")) {
			IOSDialog dialog = IOSDialog.Create("Dialog Title", "Dialog message");
			dialog.OnComplete += onDialogClose;

		}


		StartX += XButtonStep;
		if(GUI.Button(new Rect(StartX, StartY, buttonWidth, buttonHeight), "Message Pop-up")) {
			IOSMessage msg = IOSMessage.Create("Message Title", "Message body");
			msg.OnComplete += onMessageClose;

		}


		StartX = XStartPos;
		StartY+= YButtonStep;
		if(GUI.Button(new Rect(StartX, StartY, buttonWidth, buttonHeight), "Dismissed Pop-up")) {
			Invoke ("dismissAlert", 2f);
			IOSMessage.Create("Hello", "I will die in 2 sec");
		}


		StartX += XButtonStep;
		if(GUI.Button(new Rect(StartX, StartY, buttonWidth, buttonHeight), "Open App Store")) {
			IOSNativeUtility.RedirectToAppStoreRatingPage();
		}


		StartX = XStartPos;
		StartY+= YButtonStep;
		if(GUI.Button(new Rect(StartX, StartY, buttonWidth, buttonHeight), "Show Preloader ")) {
			IOSNativeUtility.ShowPreloader();
			Invoke("HidePreloader", 3f);
		}
		
		
		StartX += XButtonStep;
		if(GUI.Button(new Rect(StartX, StartY, buttonWidth, buttonHeight), "Hide Preloader")) {
			HidePreloader();
		}

		StartX += XButtonStep;
		if(GUI.Button(new Rect(StartX, StartY, buttonWidth, buttonHeight), "Get Locale")) {
			IOSNativeUtility.OnLocaleLoaded += GetLocale;
			IOSNativeUtility.Instance.GetLocale();
		}


	}
	

	//--------------------------------------
	//  GET/SET
	//--------------------------------------
	
	//--------------------------------------
	//  EVENTS
	//--------------------------------------

	private void HidePreloader() {
		IOSNativeUtility.HidePreloader();
	}

	private void dismissAlert() {
		IOSNativePopUpManager.dismissCurrentAlert ();
	}
	
	private void onRatePopUpClose(IOSDialogResult result) {
		switch(result) {
		case IOSDialogResult.RATED:
			Debug.Log ("Rate button pressed");
			break;
		case IOSDialogResult.REMIND:
			Debug.Log ("Remind button pressed");
			break;
		case IOSDialogResult.DECLINED:
			Debug.Log ("Decline button pressed");
			break;
			
		}

		IOSNativePopUpManager.showMessage("Result", result.ToString() + " button pressed");
	}
	
	private void onDialogClose(IOSDialogResult result) {

		//parsing result
		switch(result) {
		case IOSDialogResult.YES:
			Debug.Log ("Yes button pressed");
			break;
		case IOSDialogResult.NO:
			Debug.Log ("No button pressed");
			break;

		}

		IOSNativePopUpManager.showMessage("Result", result.ToString() + " button pressed");
	}
	
	private void onMessageClose() {
		Debug.Log("Message was just closed");
		IOSNativePopUpManager.showMessage("Result", "Message Closed");
	}


	private void GetLocale (ISN_Locale locale){
		Debug.Log ("GetLocale");
		Debug.Log (locale.DisplayCountry);
			IOSNativePopUpManager.showMessage("Locale Info:", "Country:" + locale.CountryCode + "/" 
		                          + locale.DisplayCountry + "  :   " + "Language:" 
		                          + locale.LanguageCode + "/" 
		                          + locale.DisplayLanguage);
		IOSNativeUtility.OnLocaleLoaded -= GetLocale;
	}

	
	//--------------------------------------
	//  PRIVATE METHODS
	//--------------------------------------
	
	//--------------------------------------
	//  DESTROY
	//--------------------------------------


}
