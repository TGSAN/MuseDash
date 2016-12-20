////////////////////////////////////////////////////////////////////////////////
//  
// @module IOS Native Plugin for Unity3D 
// @author Osipov Stanislav (Stan's Assets) 
// @support stans.assets@gmail.com 
//
////////////////////////////////////////////////////////////////////////////////



using UnityEngine;
using System.Collections;

public class iAdUseExample : MonoBehaviour {



	private GUIStyle style;
	private GUIStyle style2;

	private iAdBanner banner1;
	private iAdBanner banner2;

	public GameObject		Quad;
	//--------------------------------------
	// INITIALIZE
	//--------------------------------------

	void Start() {

		Quad.SetActive (false);

		//using events example
		iAdBannerController.InterstitialAdDidLoadAction += HandleInterstitialAdDidLoadAction;
		iAdBannerController.InterstitialAdDidFinishAction += HandleInterstitialAdDidFinishAction;



		InitStyles();
	}




	private void InitStyles () {
		style =  new GUIStyle();
		style.normal.textColor = Color.white;
		style.fontSize = 16;
		style.fontStyle = FontStyle.BoldAndItalic;
		style.alignment = TextAnchor.UpperLeft;
		style.wordWrap = true;
		
		
		style2 =  new GUIStyle();
		style2.normal.textColor = Color.white;
		style2.fontSize = 12;
		style2.fontStyle = FontStyle.Italic;
		style2.alignment = TextAnchor.UpperLeft;
		style2.wordWrap = true;
	}

	//--------------------------------------
	//  PUBLIC METHODS
	//--------------------------------------


	void Update() {

		/*
		if(Input.GetMouseButtonDown(0)) {
			Debug.Log("click");
			Debug.Log(Input.mousePosition);
		} 

		if (Input.touchCount > 0) {
			Debug.Log("touch");
			Debug.Log(Input.GetTouch(0).position); 

		}
		*/
	}


	void FixedUpdate() {
		//Debug.Log(iAdBannerController.Instance.IsPresentingFullScreenAd);
	}

	void OnApplicationPause(bool pauseStatus) {
		Debug.Log(pauseStatus);
	}


	void OnGUI() {
		
		float StartY = 20;
		float StartX = 10;
		GUI.Label(new Rect(StartX, StartY, Screen.width, 40), "Interstitial Example", style);

		StartY+= 40;
		if(GUI.Button(new Rect(StartX, StartY, 150, 50), "Start Interstitial Ad")) {
			iAdBannerController.instance.StartInterstitialAd ();
		}

		StartX += 170;
		if(GUI.Button(new Rect(StartX, StartY, 150, 50), "Load Interstitial Ad")) {
			iAdBannerController.instance.LoadInterstitialAd ();
		}


		StartX += 170;
		GUI.enabled = iAdBannerController.Instance.IsInterstisialsAdReady;
		if(GUI.Button(new Rect(StartX, StartY, 150, 50), "Show Interstitial Ad")) {
			iAdBannerController.instance.ShowInterstitialAd ();
		}
		GUI.enabled  = true;


		StartY+= 80;
		StartX = 10;
		GUI.Label(new Rect(StartX, StartY, Screen.width, 40), "Banners Example", style);

		GUI.enabled = false;
		if(banner1 == null) {
			GUI.enabled  = true;
		}

		StartY+= 40;
		if(GUI.Button(new Rect(StartX, StartY, 150, 50), "Banner Custom Pos")) {
			banner1 = iAdBannerController.instance.CreateAdBanner(300, 100);
		}

		StartY+= 80;
		StartX = 10;
		if(GUI.Button(new Rect(StartX, StartY, 150, 50), "Banner Top Left")) {
			banner1 = iAdBannerController.instance.CreateAdBanner(TextAnchor.UpperLeft);
		}


		StartX += 170;
		if(GUI.Button(new Rect(StartX, StartY, 150, 50), "Banner Top Center")) {
			banner1 = iAdBannerController.instance.CreateAdBanner(TextAnchor.UpperCenter);
		}

		StartX += 170;
		if(GUI.Button(new Rect(StartX, StartY, 150, 50), "Banner Top Right")) {
			banner1 = iAdBannerController.instance.CreateAdBanner(TextAnchor.UpperRight);
		}

		StartY+= 80;
		StartX = 10;
		if(GUI.Button(new Rect(StartX, StartY, 150, 50), "Banner Bottom Left")) {
			banner1 = iAdBannerController.instance.CreateAdBanner(TextAnchor.LowerLeft);
		}
		
		StartX += 170;
		if(GUI.Button(new Rect(StartX, StartY, 150, 50), "Banner Bottom Center")) {
			banner1 = iAdBannerController.instance.CreateAdBanner(TextAnchor.LowerCenter);
		}
		
		StartX += 170;
		if(GUI.Button(new Rect(StartX, StartY, 150, 50), "Banner Bottom Right")) {
			banner1 = iAdBannerController.instance.CreateAdBanner(TextAnchor.LowerRight);
		}


		StartY+= 80;
		StartX = 10;


		GUI.enabled  = false;
		if(banner1 != null) {
			if(banner1.IsLoaded && banner1.IsOnScreen) {
				GUI.enabled  = true;
			}
		}
	
		if(GUI.Button(new Rect(StartX, StartY, 150, 50), "Hide")) {
			banner1.Hide();
		}


		GUI.enabled  = false;
		if(banner1 != null) {
			if(banner1.IsLoaded && !banner1.IsOnScreen) {
				GUI.enabled  = true;
			}
		}
		StartX += 170;
		if(GUI.Button(new Rect(StartX, StartY, 150, 50), "Show")) {
			banner1.Show();
		}



		GUI.enabled  = false;
		if(banner1 != null) {
			GUI.enabled  = true;
		}
		StartX += 170;
		if(GUI.Button(new Rect(StartX, StartY, 150, 50), "Destroy")) {
			Debug.Log("Destroy banner with ID: " + banner1.id);
			iAdBannerController.instance.DestroyBanner(banner1.id);
			banner1 = null;

		}

		GUI.enabled  = true;

	}
	
	//--------------------------------------
	//  GET/SET
	//--------------------------------------


	//--------------------------------------
	//  Actions 
	//--------------------------------------

	void HandleInterstitialAdDidFinishAction () {
		Debug.Log("OnInterstitialFinish action fired");
		IOSMessage.Create("Ad Event", "Ad Did Finish");
	}
	
	void HandleInterstitialAdDidLoadAction (){
		Debug.Log("HandleInterstitialAdDidLoadAction event fired");
	}
	

	//--------------------------------------
	//  PRIVATE METHODS
	//--------------------------------------
	
	//--------------------------------------
	//  DESTROY
	//--------------------------------------

}
