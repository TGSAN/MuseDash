////////////////////////////////////////////////////////////////////////////////
//  
// @module IOS Native Plugin for Unity3D 
// @author Osipov Stanislav (Stan's Assets) 
// @support stans.assets@gmail.com 
//
////////////////////////////////////////////////////////////////////////////////


using UnityEngine;
using System.Collections;

public class IOSSocialUseExample : MonoBehaviour {
	
	private GUIStyle style;
	private GUIStyle style2;
	public Texture2D drawTexture = null;
	public Texture2D textureForPost;
	
	
	void Awake() {
		
		IOSSocialManager.OnFacebookPostResult += HandleOnFacebookPostResult;
		IOSSocialManager.OnTwitterPostResult += HandleOnTwitterPostResult;
		IOSSocialManager.OnInstagramPostResult += HandleOnInstagramPostResult;
		
		//actions use example:
		IOSSocialManager.OnMailResult += OnMailResult;
		
		
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
	
	
	void OnGUI() {
		
		float StartY = 20;
		float StartX = 10;
		GUI.Label(new Rect(StartX, StartY, Screen.width, 40), "Twitter", style);
		
		StartY+= 40;
		if(GUI.Button(new Rect(StartX, StartY, 150, 50), "Post")) {
			IOSSocialManager.instance.TwitterPost("Twitter posting test");
		}
		
		StartX += 170;
		if(GUI.Button(new Rect(StartX, StartY, 150, 50), "Post Screenshot")) {
			StartCoroutine(PostTwitterScreenshot());
		}
		
		
		
		StartY+= 80;
		StartX = 10;
		GUI.Label(new Rect(StartX, StartY, Screen.width, 40), "Facebook", style);
		
		
		StartY+= 40;
		if(GUI.Button(new Rect(StartX, StartY, 150, 50), "Post")) {
			IOSSocialManager.instance.FacebookPost("Facebook posting test");
		}
		
		StartX += 170;
		
		if(GUI.Button(new Rect(StartX, StartY, 150, 50), "Post Screenshot")) {
			StartCoroutine(PostFBScreenshot());
		}
		
		
		StartX += 170;
		
		if(GUI.Button(new Rect(StartX, StartY, 150, 50), "Post Image")) {
			IOSSocialManager.instance.FacebookPost("Hello world", "https://www.assetstore.unity3d.com/en/#!/publisher/2256", textureForPost);
		}
		
		
		StartY+= 80;
		StartX = 10;
		GUI.Label(new Rect(StartX, StartY, Screen.width, 40), "Native Sharing", style);
		
		
		StartY+= 40;
		if(GUI.Button(new Rect(StartX, StartY, 150, 50), "Text")) {
			IOSSocialManager.instance.ShareMedia("Some text to share");
		}
		
		StartX += 170;
		
		if(GUI.Button(new Rect(StartX, StartY, 150, 50), "Screenshot")) {
			StartCoroutine(PostScreenshot());
		}
		
		
		StartX += 170;
		
		if(GUI.Button(new Rect(StartX, StartY, 150, 50), "Send Mail")) {
			IOSSocialManager.instance.SendMail("Mail Subject", "Mail Body  <strong> text html </strong> ", "mail1@gmail.com, mail2@gmail.com", textureForPost);
		}
		
		StartY+= 80;
		StartX = 10;
		GUI.Label(new Rect(StartX, StartY, Screen.width, 40), "Instagram", style);
		
		StartY+= 40;
		if(GUI.Button(new Rect(StartX, StartY, 150, 50), "Post image from camera")) {
			IOSCamera.OnImagePicked += OnPostImageInstagram;
			IOSCamera.Instance.PickImage(ISN_ImageSource.Camera);
		}

		StartX += 170;
		
		if(GUI.Button(new Rect(StartX, StartY, 150, 50), "Post Screenshot")) {
			StartCoroutine(PostScreenshotInstagram());
		}



		StartY+= 80;
		StartX = 10;
		GUI.Label(new Rect(StartX, StartY, Screen.width, 40), "WhatsApp", style);
		
		StartY+= 40;
		if(GUI.Button(new Rect(StartX, StartY, 150, 50), "Share Text")) {
			IOSSocialManager.Instance.WhatsAppShareText("Some text");
		}
		
		StartX += 170;
		
		if(GUI.Button(new Rect(StartX, StartY, 150, 50), "Share Image")) {
			IOSSocialManager.Instance.WhatsAppShareImage(textureForPost);
		}

		
		
	}


	private void OnPostImageInstagram (IOSImagePickResult result) {
		if(result.IsSucceeded) {

			Destroy(drawTexture);

			drawTexture = result.Image;
		} else {
			IOSMessage.Create("ERROR", "Image Load Failed");
		}
		IOSSocialManager.instance.InstagramPost(drawTexture ,"Some text to share");
		IOSCamera.OnImagePicked -= OnPostImageInstagram;
	}

	private IEnumerator PostScreenshotInstagram() {
		
		yield return new WaitForEndOfFrame();
		// Create a texture the size of the screen, RGB24 format
		int width = Screen.width;
		int height = Screen.height;
		Texture2D tex = new Texture2D( width, height, TextureFormat.RGB24, false );
		// Read screen contents into the texture
		tex.ReadPixels( new Rect(0, 0, width, height), 0, 0 );
		tex.Apply();
		
		IOSSocialManager.instance.InstagramPost(tex, "Some text to share");
		
		Destroy(tex);
		
	}


	private IEnumerator PostScreenshot() {
		
		yield return new WaitForEndOfFrame();
		// Create a texture the size of the screen, RGB24 format
		int width = Screen.width;
		int height = Screen.height;
		Texture2D tex = new Texture2D( width, height, TextureFormat.RGB24, false );
		// Read screen contents into the texture
		tex.ReadPixels( new Rect(0, 0, width, height), 0, 0 );
		tex.Apply();
		
		IOSSocialManager.instance.ShareMedia("Some text to share", tex);
		
		Destroy(tex);
		
	}
	
	private IEnumerator PostTwitterScreenshot() {
		
		yield return new WaitForEndOfFrame();
		// Create a texture the size of the screen, RGB24 format
		int width = Screen.width;
		int height = Screen.height;
		Texture2D tex = new Texture2D( width, height, TextureFormat.RGB24, false );
		// Read screen contents into the texture
		tex.ReadPixels( new Rect(0, 0, width, height), 0, 0 );
		tex.Apply();
		
		IOSSocialManager.instance.TwitterPost("My app Screenshot", null,  tex);
		
		Destroy(tex);
		
	}
	
	private IEnumerator PostFBScreenshot() {
		
		
		yield return new WaitForEndOfFrame();
		// Create a texture the size of the screen, RGB24 format
		int width = Screen.width;
		int height = Screen.height;
		Texture2D tex = new Texture2D( width, height, TextureFormat.RGB24, false );
		// Read screen contents into the texture
		tex.ReadPixels( new Rect(0, 0, width, height), 0, 0 );
		tex.Apply();
		
		IOSSocialManager.instance.FacebookPost("My app Screenshot", null, tex);
		
		Destroy(tex);
		
	}
	

	
	void HandleOnInstagramPostResult (ISN_Result res){
		if (res.IsSucceeded) {
			IOSNativePopUpManager.showMessage ("Posting example", "Post Success!");
		} else {
			IOSNativePopUpManager.showMessage ("Posting example", "Post Failed :(");
		}
	}
	
	
	void HandleOnTwitterPostResult (ISN_Result res){
		if(res.IsSucceeded) {
			IOSNativePopUpManager.showMessage("Posting example", "Post Success!");
		} else {
			IOSNativePopUpManager.showMessage("Posting example", "Post Failed :(");
		}
	}
	
	void HandleOnFacebookPostResult (ISN_Result res) {
		if(res.IsSucceeded) {
			IOSNativePopUpManager.showMessage("Posting example", "Post Success!");
		} else {
			IOSNativePopUpManager.showMessage("Posting example", "Post Failed :(");
		}
	}
	
	
	private void OnMailResult (ISN_Result result) {
		if(result.IsSucceeded) {

			IOSNativePopUpManager.showMessage("Posting example", "Mail Sent");
		} else {
			IOSNativePopUpManager.showMessage("Posting example", "Mail Failed :(");
		}
	}


}

