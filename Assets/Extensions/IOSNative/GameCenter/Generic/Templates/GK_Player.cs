////////////////////////////////////////////////////////////////////////////////
//  
// @module IOS Native Plugin for Unity3D 
// @author Osipov Stanislav (Stan's Assets) 
// @support stans.assets@gmail.com 
//
////////////////////////////////////////////////////////////////////////////////



using System;
using UnityEngine;

public class GK_Player {

	private string _PlayerId;
	private string _DisplayName;
	private string _Alias;


	private Texture2D _SmallPhoto = null;
	private Texture2D _BigPhoto = null;


	public event Action<GK_UserPhotoLoadResult> OnPlayerPhotoLoaded =  delegate {};


	//--------------------------------------
	// INITIALIZE
	//--------------------------------------

	public GK_Player (string pId, string pName, string pAlias) {
		_PlayerId = pId;
		_DisplayName = pName;
		_Alias = pAlias;

		if(IOSNativeSettings.Instance.AutoLoadUsersBigImages) {
			LoadPhoto(GK_PhotoSize.GKPhotoSizeNormal);
		}

		if(IOSNativeSettings.Instance.AutoLoadUsersSmallImages) {
			LoadPhoto(GK_PhotoSize.GKPhotoSizeSmall);
		}

	}


	//--------------------------------------
	// Public Methods
	//--------------------------------------

	public void LoadPhoto(GK_PhotoSize size) {
		if(size == GK_PhotoSize.GKPhotoSizeSmall) {
			if(_SmallPhoto != null) {
				return;
			}
		} else {
			if(_BigPhoto != null) {
				return;
			}
		}
		GameCenterManager.LoadGKPlayerPhoto(Id, size);
	}


	//--------------------------------------
	// Do not use this methods, plugin internal use only
	//--------------------------------------


	public void SetPhotoData(GK_PhotoSize size, string base64String) {

		if(base64String.Length == 0) {
			return;
		}

		byte[] decodedFromBase64 = System.Convert.FromBase64String(base64String);

		Texture2D loadedPhoto = new Texture2D(1, 1);
		loadedPhoto.LoadImage(decodedFromBase64);

		if(size == GK_PhotoSize.GKPhotoSizeSmall) {
			_SmallPhoto = loadedPhoto;
		} else {
			_BigPhoto = loadedPhoto;
		}

		GK_UserPhotoLoadResult result = new GK_UserPhotoLoadResult(size, loadedPhoto);
		OnPlayerPhotoLoaded(result);
	}

	public void SetPhotoLoadFailedEventData(GK_PhotoSize size, string errorData) {
		GK_UserPhotoLoadResult result = new GK_UserPhotoLoadResult(size, errorData);
		OnPlayerPhotoLoaded(result);
	}


	//--------------------------------------
	// GET / SET
	//--------------------------------------

	public string Id {
		get {
			return _PlayerId;
		}
	}

	public string Alias {
		get {
			return _Alias;
		}
	}


	public string DisplayName {
		get {
			return _DisplayName;
		}
	}




	public Texture2D SmallPhoto {
		get {
			return _SmallPhoto;
		}
	}

	public Texture2D BigPhoto {
		get {
			return _BigPhoto;
		}
	}
}


