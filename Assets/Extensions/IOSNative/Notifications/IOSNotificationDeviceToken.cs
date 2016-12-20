////////////////////////////////////////////////////////////////////////////////
//  
// @module IOS Native Plugin for Unity3D 
// @author Osipov Stanislav (Stan's Assets) 
// @support stans.assets@gmail.com 
//
////////////////////////////////////////////////////////////////////////////////



using UnityEngine;
using System.Collections;

public class IOSNotificationDeviceToken  {

	private string _tokenString;
	private byte[] _tokenBytes;


	//--------------------------------------
	// INITIALIZE
	//--------------------------------------

	public IOSNotificationDeviceToken(byte[] token)  {
		_tokenBytes = token;

		_tokenString =  System.BitConverter.ToString(token).Replace("-", "").ToLower();
	}



	//--------------------------------------
	//  GET/SET
	//--------------------------------------

	public string tokenString {
		get {
			return _tokenString;
		}
	}


	public byte[] tokenBytes {
		get {
			return _tokenBytes;
		}
	}

}
