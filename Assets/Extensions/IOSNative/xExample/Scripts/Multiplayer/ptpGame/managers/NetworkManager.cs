////////////////////////////////////////////////////////////////////////////////
//  
// @module IOS Native Plugin for Unity3D 
// @author Osipov Stanislav (Stan's Assets) 
// @support stans.assets@gmail.com 
//
////////////////////////////////////////////////////////////////////////////////



using UnityEngine;
using System.Collections;

public class NetworkManager  {

	//--------------------------------------
	// INITIALIZE
	//--------------------------------------



	//--------------------------------------
	//  PUBLIC METHODS
	//--------------------------------------

	public static void send(BasePackage pack) {
		GameCenter_RTM.instance.SendDataToAll (pack.getBytes(), GK_MatchSendDataMode.RELIABLE);
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
