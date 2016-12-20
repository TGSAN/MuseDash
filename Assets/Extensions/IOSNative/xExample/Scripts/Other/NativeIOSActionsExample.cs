using UnityEngine;
using System.Collections;

public class NativeIOSActionsExample : BaseIOSFeaturePreview {

	public Texture2D hello_texture;
	public Texture2D drawTexture = null;


	void Awake() {


		IOSSharedApplication.OnUrlCheckResultAction += OnUrlCheckResultAction;


		IOSDateTimePicker.instance.OnDateChanged += OnDateChanged;
		IOSDateTimePicker.instance.OnPickerClosed += OnPickerClosed;
	}



	void OnGUI() {
		UpdateToStartPos();


		GUI.Label(new Rect(StartX, StartY, Screen.width, 40), "Using URL Scheme", style);
		
		
		StartY+= YLableStep;
		if(GUI.Button(new Rect(StartX, StartY, buttonWidth, buttonHeight), "Check if FB App exists")) {
			IOSSharedApplication.instance.CheckUrl("fb://");
		}
		
		StartX += XButtonStep;
		if(GUI.Button(new Rect(StartX, StartY, buttonWidth, buttonHeight), "Open FB Profile")) {
			IOSSharedApplication.instance.OpenUrl("fb://profile");
		}
		
		StartX += XButtonStep;
		if(GUI.Button(new Rect(StartX, StartY, buttonWidth, buttonHeight), "Open App Store")) {
			IOSSharedApplication.instance.OpenUrl("itms-apps://");
		}

		StartX += XButtonStep;
		if(GUI.Button(new Rect(StartX, StartY, buttonWidth, buttonHeight), "Get IFA")) {
			IOSSharedApplication.OnAdvertisingIdentifierLoadedAction += OnAdvertisingIdentifierLoadedAction;
			IOSSharedApplication.instance.GetAdvertisingIdentifier();
		}

		StartX = XStartPos;
		StartY+= YButtonStep;
		if(GUI.Button(new Rect(StartX, StartY, buttonWidth, buttonHeight), "Set App Bages Count")) {
			IOSNativeUtility.SetApplicationBagesNumber(10);
		}

		StartX += XButtonStep;
		if(GUI.Button(new Rect(StartX, StartY, buttonWidth, buttonHeight), "Clear Application Bages")) {
			IOSNativeUtility.SetApplicationBagesNumber(0);
		}

		StartX += XButtonStep;
		if(GUI.Button(new Rect(StartX, StartY, buttonWidth, buttonHeight), "Show Device Info")) {
			ShowDevoceInfo();
		}



		StartX = XStartPos;
		StartY+= YButtonStep;
		StartY+= YLableStep;
		
		
		
		GUI.Label(new Rect(StartX, StartY, Screen.width, 40), "Date Time Picker", style);

		StartY+= YLableStep;
		if(GUI.Button(new Rect(StartX, StartY, buttonWidth, buttonHeight), "Time")) {
			IOSDateTimePicker.instance.Show(IOSDateTimePickerMode.Time);
		}


		StartX += XButtonStep;
		if(GUI.Button(new Rect(StartX, StartY, buttonWidth, buttonHeight), "Date")) {
			IOSDateTimePicker.instance.Show(IOSDateTimePickerMode.Date);
		}

		StartX += XButtonStep;
		if(GUI.Button(new Rect(StartX, StartY, buttonWidth, buttonHeight), "Date And Time")) {
			IOSDateTimePicker.instance.Show(IOSDateTimePickerMode.DateAndTime);
		}

		StartX += XButtonStep;
		if(GUI.Button(new Rect(StartX, StartY, buttonWidth, buttonHeight), "Countdown Timer")) {
			IOSDateTimePicker.instance.Show(IOSDateTimePickerMode.CountdownTimer);
		}



		StartX = XStartPos;
		StartY+= YButtonStep;
		StartY+= YLableStep;


		GUI.Label(new Rect(StartX, StartY, Screen.width, 40), "Video", style);
		
		StartY+= YLableStep;
		if(GUI.Button(new Rect(StartX, StartY, buttonWidth, buttonHeight), "Player Streamed video")) {
			IOSVideoManager.instance.PlayStreamingVideo("https://dl.dropboxusercontent.com/u/83133800/Important/Doosan/GT2100-Video.mov");
		}
		
		StartX += XButtonStep;
		if(GUI.Button(new Rect(StartX, StartY, buttonWidth, buttonHeight), "Open YouTube Video")) {
			IOSVideoManager.instance.OpenYouTubeVideo("xzCEdSKMkdU");
		}


		
		StartX = XStartPos;
		StartY+= YButtonStep;
		StartY+= YLableStep;



		
		GUI.Label(new Rect(StartX, StartY, Screen.width, 40), "Camera Roll", style);
		
		StartY+= YLableStep;
		if(GUI.Button(new Rect(StartX, StartY, buttonWidth + 10, buttonHeight), "Save Screenshot To Camera Roll")) {
			IOSCamera.OnImageSaved += OnImageSaved;
			IOSCamera.Instance.SaveScreenshotToCameraRoll();
		}


		StartX += XButtonStep;
		if(GUI.Button(new Rect(StartX, StartY, buttonWidth, buttonHeight), "Save Texture To Camera Roll")) {
			IOSCamera.OnImageSaved += OnImageSaved;
			IOSCamera.Instance.SaveTextureToCameraRoll(hello_texture);
		}


		StartX += XButtonStep;
		if(GUI.Button(new Rect(StartX, StartY, buttonWidth, buttonHeight), "Get Image From Camera")) {
			IOSCamera.OnImagePicked += OnImage;
			IOSCamera.Instance.PickImage(ISN_ImageSource.Camera);
			
		}

		StartX += XButtonStep;
		if(GUI.Button(new Rect(StartX, StartY, buttonWidth, buttonHeight), "Get Image From Album")) {
			IOSCamera.OnImagePicked += OnImage;
			IOSCamera.Instance.PickImage(ISN_ImageSource.Album);

		}





		StartX = XStartPos;
		StartY+= YButtonStep;
		StartY+= YLableStep;
		GUI.Label(new Rect(StartX, StartY, Screen.width, 40), "PickedImage", style);
		StartY+= YLableStep;

		if(drawTexture != null) {
			GUI.DrawTexture(new Rect(StartX, StartY, buttonWidth, buttonWidth), drawTexture);
		}
	

	}

	private void ShowDevoceInfo() {
		ISN_Device device = ISN_Device.CurrentDevice;

		IOSMessage.Create("Device Info", "Name: "  + device.Name + "\n"
		                  + "System Name: " + device.SystemName + "\n"
		                  + "Model: " + device.Model + "\n"
		                  + "Localized Model: " + device.LocalizedModel + "\n"
		                  + "System Version: " + device.SystemVersion + "\n"
		                  + "Major System Version: " + device.MajorSystemVersion + "\n"
		                  + "User Interface Idiom: " + device.InterfaceIdiom + "\n"
		                  + "GUID in Base64: " + device.GUID.Base64String  );
	}

	void OnDateChanged (System.DateTime time) {
		Debug.Log("OnDateChanged: " + time.ToString());
	}

	void OnPickerClosed (System.DateTime time) {
		Debug.Log("OnPickerClosed: " + time.ToString());
	}
	

	private void OnImage (IOSImagePickResult result) {
		if(result.IsSucceeded) {

			//destroying old texture
			Destroy(drawTexture);

			//applaying new texture
			drawTexture = result.Image;
			IOSMessage.Create("Success", "Image Successfully Loaded, Image size: " + result.Image.width + "x" + result.Image.height);
		} else {
			IOSMessage.Create("ERROR", "Image Load Failed");
		}

		IOSCamera.OnImagePicked -= OnImage;
	}

	private void OnImageSaved (ISN_Result result) {
		IOSCamera.OnImageSaved -= OnImageSaved;
		if(result.IsSucceeded) {
			IOSMessage.Create("Success", "Image Successfully saved to Camera Roll");
		} else {
			IOSMessage.Create("ERROR", "Image Save Failed");
		}
	}

	private void OnUrlCheckResultAction (ISN_CheckUrlResult result) {

		if(result.IsSucceeded) {
			IOSMessage.Create("Success", "The " + result.url + " is registered" );
		} else {
			IOSMessage.Create("ERROR", "The " + result.url + " wasn't registered");
		}
	}

	void OnAdvertisingIdentifierLoadedAction (string Identifier) {
		IOSMessage.Create("Identifier Loaded", Identifier);
	}
}
