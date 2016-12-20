////////////////////////////////////////////////////////////////////////////////
//  
// @module IOS Native Plugin for Unity3D 
// @author Osipov Stanislav (Stan's Assets) 
// @support stans.assets@gmail.com 
//
////////////////////////////////////////////////////////////////////////////////



using UnityEngine;
using System.Collections;

public class MultiplayerManagerExample : MonoBehaviour {

	//--------------------------------------
	// INITIALIZE
	//--------------------------------------

	void Awake() {

		GameCenterManager.Init();

		GameCenter_RTM.ActionMatchStarted += HandleActionMatchStarted;
		GameCenter_RTM.ActionPlayerStateChanged += HandleActionPlayerStateChanged;
		GameCenter_RTM.ActionDataReceived += HandleActionDataReceived;


		GameCenterInvitations.ActionPlayerRequestedMatchWithRecipients += HandleActionPlayerRequestedMatchWithRecipients;
		GameCenterInvitations.ActionPlayerAcceptedInvitation += HandleActionPlayerAcceptedInvitation;
		

	}

	void HandleActionPlayerAcceptedInvitation (GK_MatchType math, GK_Invite invite) {
		GameCenter_RTM.Instance.StartMatchWithInvite(invite, true);
	}



	void HandleActionPlayerRequestedMatchWithRecipients (GK_MatchType matchType, string[] recepientIds, GK_Player[] recepients) {
		Debug.Log("inictation received");
		if(matchType == GK_MatchType.RealTime) {
			//Optionally you can provide and invitation message
			string invitationMessage = "Come play with me, bro.";
			
			GameCenter_RTM.Instance.FindMatchWithNativeUI(recepientIds.Length, recepientIds.Length, invitationMessage, recepientIds);
		}
	}




	//--------------------------------------
	//  PUBLIC METHODS
	//--------------------------------------

	void OnGUI() {


#if (UNITY_IPHONE && !UNITY_EDITOR) || SA_DEBUG_MODE

		GUI.enabled = true;

		if(!GameCenterManager.IsPlayerAuthenticated) {
			GUI.enabled = false;
		}



		if(GUI.Button(new Rect(170, 70, 150, 50), "Find Match")) {
			GameCenter_RTM.Instance.FindMatch(2, 2);
		}

		if(GUI.Button(new Rect(170, 130, 150, 50), "Find Match Natvie UI")) {
			GameCenter_RTM.Instance.FindMatchWithNativeUI(2, 2);
		}

		if(GameCenter_RTM.Instance.CurrentMatch == null ) {
			GUI.enabled = false;
		} else {
			if(GameCenter_RTM.Instance.CurrentMatch.ExpectedPlayerCount > 0) {
				GUI.enabled = false;
			}
		}



		if(GUI.Button(new Rect(170, 190, 150, 50), "Send Data to All")) {
			string msg = "hello world";
			System.Text.UTF8Encoding  encoding = new System.Text.UTF8Encoding();
			byte[] data = encoding.GetBytes(msg);
			GameCenter_RTM.Instance.SendDataToAll(data, GK_MatchSendDataMode.RELIABLE);
		}


		if(GUI.Button(new Rect(170, 250, 150, 50), "Send to Player")) {
			string msg = "hello world";
			System.Text.UTF8Encoding  encoding = new System.Text.UTF8Encoding();
			byte[] data = encoding.GetBytes(msg);

			GameCenter_RTM.Instance.SendData(data, GK_MatchSendDataMode.RELIABLE, GameCenter_RTM.Instance.CurrentMatch.Players[0]);

		}

		if(GUI.Button(new Rect(170, 310, 150, 50), "Disconnect")) {
			GameCenter_RTM.Instance.Disconnect();
		}
#endif
	}
	
	//--------------------------------------
	//  GET/SET
	//--------------------------------------
	
	//--------------------------------------
	//  EVENTS
	//--------------------------------------



	void HandleActionPlayerStateChanged (GK_Player player, GK_PlayerConnectionState state, GK_RTM_Match match) {


		Debug.Log("Player State Changed " +  player.Alias + " state: " + state.ToString() + "\n  ExpectedPlayerCount: " + match.ExpectedPlayerCount);

	}
	

	void HandleActionMatchStarted (GK_RTM_MatchStartedResult result) {
		IOSNativePopUpManager.dismissCurrentAlert();
		if(result.IsSucceeded) {
			IOSNativePopUpManager.showMessage ("Match Started", "let's play now\n  Others players count: " + result.Match.Players.Count);
		} else {
			IOSNativePopUpManager.showMessage ("Match Started Error", result.Error.Description);
		}
	}

	void HandleActionDataReceived (GK_Player player, byte[] data) {
		IOSNativePopUpManager.dismissCurrentAlert();

		#if (UNITY_IPHONE && !UNITY_EDITOR) || SA_DEBUG_MODE

		
		System.Text.UTF8Encoding enc = new System.Text.UTF8Encoding();
		string str = enc.GetString(data);


		IOSNativePopUpManager.dismissCurrentAlert();

		IOSNativePopUpManager.showMessage ("Data received", "player ID: " + player.Id + " \n " + "data: " + str);
		#endif
	}

	


	
	//--------------------------------------
	//  PRIVATE METHODS
	//--------------------------------------
	
	//--------------------------------------
	//  DESTROY
	//--------------------------------------

}
