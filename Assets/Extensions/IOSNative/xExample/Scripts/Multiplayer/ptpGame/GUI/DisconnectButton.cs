////////////////////////////////////////////////////////////////////////////////
//  
// @module IOS Native Plugin for Unity3D 
// @author Osipov Stanislav (Stan's Assets) 
// @support stans.assets@gmail.com 
//
////////////////////////////////////////////////////////////////////////////////



using UnityEngine;
using System.Collections;

public class DisconnectButton : MonoBehaviour {

	//--------------------------------------
	// INITIALIZE
	//--------------------------------------

	private float w;
	private float h;

	private Rect r;

	void Start() {
		w = Screen.width * 0.2f;
		h = Screen.height * 0.1f;

		r.x = w * 0.1f;
		r.y = h * 0.1f;

		r.width = w;
		r.height = h;
	}

	//--------------------------------------
	//  PUBLIC METHODS
	//--------------------------------------

	void OnGUI() {
		if(GUI.Button(r, "Disconnect")) {
			GameCenter_RTM.Instance.Disconnect();
		}
	}
	//--------------------------------------
	//  GET/SET
	//--------------------------------------
	
	//--------------------------------------
	//  EVENTS
	//--------------------------------------
	
	//--------------------------------------
	//  PRIVATE METHODS
	//--------------------------------------
	
	//--------------------------------------
	//  DESTROY
	//--------------------------------------

}
