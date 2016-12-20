////////////////////////////////////////////////////////////////////////////////
//  
// @module IOS Native Plugin for Unity3D 
// @author Osipov Stanislav (Stan's Assets) 
// @support stans.assets@gmail.com 
//
////////////////////////////////////////////////////////////////////////////////


using UnityEngine;
using System.Collections;

public class AppEventHandlerExample : MonoBehaviour {

	//--------------------------------------
	// INITIALIZE
	//--------------------------------------

	void Awake() {

		//Action use example

		IOSNativeAppEvents.Instance.Subscribe();
		IOSNativeAppEvents.OnApplicationDidReceiveMemoryWarning += OnApplicationDidReceiveMemoryWarning;
		IOSNativeAppEvents.OnApplicationDidBecomeActive += HandleOnApplicationDidBecomeActive;
	}



	//--------------------------------------
	// EVENTS
	//--------------------------------------


	void HandleOnApplicationDidBecomeActive () {
		Debug.Log("Caught OnApplicationDidBecomeActive event");
	}


	private void OnApplicationDidReceiveMemoryWarning() {
		//Called when the application receives a memory warning from the system.

		Debug.Log ("Caught OnApplicationDidReceiveMemoryWarning event");
	}


	//--------------------------------------
	// PRIVATE METHODS
	//--------------------------------------
	
	//--------------------------------------
	// DESTROY
	//--------------------------------------
}
