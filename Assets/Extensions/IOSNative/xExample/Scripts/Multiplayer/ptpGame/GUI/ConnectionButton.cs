////////////////////////////////////////////////////////////////////////////////
//  
// @module IOS Native Plugin for Unity3D 
// @author Osipov Stanislav (Stan's Assets) 
// @support stans.assets@gmail.com 
//
////////////////////////////////////////////////////////////////////////////////



using UnityEngine;
using System.Collections;

public class ConnectionButton : MonoBehaviour {

	//--------------------------------------
	// INITIALIZE
	//--------------------------------------

	private float w;
	private float h;

	private Rect r;

	void Start() {
		w = Screen.width * 0.2f;
		h = Screen.height * 0.1f;

		r.x = (Screen.width - w) / 2f;
		r.y = (Screen.height - h) / 2f;

		r.width = w;
		r.height = h;
	}

	//--------------------------------------
	//  PUBLIC METHODS
	//--------------------------------------

	void OnGUI() {
		if(GUI.Button(r, "Find Match")) {
			GameCenter_RTM.Instance.FindMatch (2, 2);
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
