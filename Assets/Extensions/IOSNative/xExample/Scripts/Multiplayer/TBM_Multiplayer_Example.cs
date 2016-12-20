using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TBM_Multiplayer_Example : BaseIOSFeaturePreview {

	private static bool IsInitialized = false;


	void Awake() {
		if(!IsInitialized) {



			//Initializing Game Center class. This action will trigger authentication flow
			GameCenterManager.Init();
			GameCenterManager.OnAuthFinished += OnAuthFinished;
			IsInitialized = true;
		}

		int ROLE_WIZARD = 0x4; // 100 in binary
		GameCenter_RTM.instance.SetPlayerAttributes (ROLE_WIZARD);



		//Required
		int minPlayers = 2;
		int maxPlayers = 2;

		//Optionally you can provide and invitation message
		string invitationMessage = "Come play with me, bro.";

		//Optinally you can predefine invited friends list to the match
		//Teh code bellow assumes that player has atleast one friend, and you already loaded the friend list
		//so we can send an invite to the first player in the firendlist
		string[] invitations  = new string[] { GameCenterManager.FriendsList[0] };


		GameCenter_RTM.Instance.FindMatchWithNativeUI(minPlayers, maxPlayers, invitationMessage, invitations);

		GK_Player player = GameCenterManager.Player;
		player.OnPlayerPhotoLoaded += HandleOnPlayerPhotoLoaded;
		player.LoadPhoto(GK_PhotoSize.GKPhotoSizeNormal);

		GameCenter_RTM.ActionMatchStarted += HandleActionMatchStarted;
		GameCenterInvitations.ActionPlayerRequestedMatchWithRecipients += HandleActionPlayerRequestedMatchWithRecipients;
		GameCenterInvitations.ActionPlayerAcceptedInvitation += HandleActionPlayerAcceptedInvitation;

	
		GameCenter_RTM.ActionNearbyPlayerStateUpdated += HandleActionNearbyPlayerStateUpdated;
		GameCenter_RTM.Instance.StartBrowsingForNearbyPlayers();



	}

	void HandleActionNearbyPlayerStateUpdated (GK_Player player, bool IsAvaliable) {
		Debug.Log("Player: " + player.DisplayName + "IsAvaliable: " + IsAvaliable);
		Debug.Log("Nearby Players Count: " + GameCenter_RTM.Instance.NearbyPlayers.Count);
	}

	void HandleActionPlayerAcceptedInvitation (GK_MatchType matchType, GK_Invite invite) {
		if(matchType == GK_MatchType.RealTime) {
			bool useNativeUI = true;
			GameCenter_RTM.Instance.StartMatchWithInvite(invite, useNativeUI);
		}
	}

	void HandleActionPlayerRequestedMatchWithRecipients (GK_MatchType matchType, string[] recepientIds, GK_Player[] recepients) {
		if(matchType == GK_MatchType.RealTime) {
			//Optionally you can provide and invitation message
			string invitationMessage = "Come play with me, bro.";

			GameCenter_RTM.Instance.FindMatchWithNativeUI(recepientIds.Length, recepientIds.Length, invitationMessage, recepientIds);
		}
	}

	void HandleActionMatchStarted (GK_RTM_MatchStartedResult result) {
		if(result.IsSucceeded) {
			Debug.Log("Match is successfully created");
			if(result.Match.ExpectedPlayerCount == 0) {
				//we should start the match
			}
		} else {
			Debug.Log("Match is creation failed with error: " + result.Error.Description);
		}
	}

	void HandleOnPlayerPhotoLoaded (GK_UserPhotoLoadResult result) {
		if(result.IsSucceeded) {
			Debug.Log(result.Photo);
			Debug.Log(GameCenterManager.Player.BigPhoto);
		}
	}



	void OnGUI() {
		UpdateToStartPos();

		if(!GameCenterManager.IsPlayerAuthenticated) {
			GUI.Label(new Rect(StartX, StartY, Screen.width, 40), "TBM Multiplayer Example Scene, Authentication....", style);
			GUI.enabled = false;
		} else {
			GUI.Label(new Rect(StartX, StartY, Screen.width, 40), "TBM Multiplayer Example Scene.", style);
			GUI.enabled = true; 
		}

		GUI.enabled = true;
		
		StartY+= YLableStep;
		
		if(GUI.Button(new Rect(StartX, StartY, buttonWidth, buttonHeight), "Load Matches Info")) {
			GameCenter_TBM.Instance.LoadMatchesInfo();
			GameCenter_TBM.ActionMatchesInfoLoaded += ActionMatchesResultLoaded;

		//	string data = "8baca524-b695-4d38-b802-b93df33b24ea|2||2015-06-13 21:33:34|||g:2130381CB8EA2C314BD259CA218844C7|5|6|1970-01-01 00:00:00|2015-06-13 21:35:27|G:2004563794|5|0|1970-01-01 00:00:00|1970-01-01 00:00:00|endofline|%|9d629bb4-4b1d-4fdc-af72-b4d5e93733e1|2||2015-06-14 15:07:00|||g:2130381CB8EA2C314BD259CA218844C7|5|6|1970-01-01 00:00:00|2015-06-14 15:08:35|G:2004563794|5|0|1970-01-01 00:00:00|1970-01-01 00:00:00|endofline|%|endofline";
		//	GameCenter_TBM.Instance.OnLoadMatchesResult(data);
		}

		StartX += XButtonStep;
		
		if(GUI.Button(new Rect(StartX, StartY, buttonWidth, buttonHeight), "Create Match")) {

			GameCenter_TBM.Instance.FindMatch(2, 2);
			GameCenter_TBM.ActionMatchFound += ActionMatchFound;
			
		}

		StartX += XButtonStep;
		
		if(GUI.Button(new Rect(StartX, StartY, buttonWidth, buttonHeight), "Create Match With Native UI")) {
			
			GameCenter_TBM.Instance.FindMatchWithNativeUI(2, 2);
			GameCenter_TBM.ActionMatchFound += ActionMatchFound;
			
		}


		if(CurrentMatch == null) {
			GUI.enabled = false;
		} else {
			GUI.enabled = true;
		}

		StartX = XStartPos;
		StartY+= YButtonStep;
		
		if(GUI.Button(new Rect(StartX, StartY, buttonWidth, buttonHeight), "Update Match Data")) {
			byte[] data = System.Text.Encoding.UTF8.GetBytes(CurrentMatch.UTF8StringData + "X");
			GameCenter_TBM.Instance.SaveCurrentTurn(CurrentMatch.Id, data);
			GameCenter_TBM.ActionMatchDataUpdated += ActionMatchDataUpdated;

		}


		StartX += XButtonStep;

		if(GUI.Button(new Rect(StartX, StartY, buttonWidth, buttonHeight), "Make A Trunn")) {
			byte[] data = System.Text.Encoding.UTF8.GetBytes("Some trun data");

			CurrentMatch.CurrentParticipant.SetOutcome(GK_TurnBasedMatchOutcome.First);
			foreach(GK_TBM_Participant p in CurrentMatch.Participants)  {
				if(!p.PlayerId.Equals(CurrentMatch.CurrentParticipant.PlayerId)) {
					GameCenter_TBM.Instance.EndTurn(CurrentMatch.Id, data, p.PlayerId);
					GameCenter_TBM.ActionTrunEnded += ActionTrunEnded;
					return;
				}
			}
		}


		StartX += XButtonStep;
		
		if(GUI.Button(new Rect(StartX, StartY, buttonWidth, buttonHeight), "End Match")) {
			byte[] data = System.Text.Encoding.UTF8.GetBytes("End match data");


			CurrentMatch.Participants[0].SetOutcome(GK_TurnBasedMatchOutcome.Won);
			CurrentMatch.Participants[1].SetOutcome(GK_TurnBasedMatchOutcome.Lost);
		
			GameCenter_TBM.Instance.EndMatch(CurrentMatch.Id, data);
			GameCenter_TBM.ActionMacthEnded += ActionMacthEnded;
			
		}


		StartX += XButtonStep;
		
		if(GUI.Button(new Rect(StartX, StartY, buttonWidth, buttonHeight), "Remove Match")) {
			GameCenter_TBM.Instance.RemoveMatch(CurrentMatch.Id);
			GameCenter_TBM.ActionMacthRemoved += ActionMacthRemoved;
			
		}
	}
	 
	void OnAuthFinished (ISN_Result res) {
		Debug.Log("Auth IsSucceeded: " + res.IsSucceeded.ToString());
	}


	public void ActionMatchesResultLoaded (GK_TBM_LoadMatchesResult res) {
		GameCenter_TBM.ActionMatchesInfoLoaded -= ActionMatchesResultLoaded;
		Debug.Log("ActionMatchesResultLoaded: " + res.IsSucceeded);

		if(res.IsFailed) {
			return;
		}

		if(res.LoadedMatches.Count == 0) {
			return;
		}


		foreach(KeyValuePair<string, GK_TBM_Match> pair in res.LoadedMatches) {
			GK_TBM_Match m = pair.Value;
			GameCenter_TBM.PrintMatchInfo(m);
		}
	}

	void ActionMatchDataUpdated (GK_TBM_MatchDataUpdateResult res) {
		GameCenter_TBM.ActionMatchDataUpdated -= ActionMatchDataUpdated;
		Debug.Log("ActionMatchDataUpdated: " + res.IsSucceeded);
		if(res.IsFailed) {
			Debug.Log(res.Error.Description);
		} else {
			GameCenter_TBM.PrintMatchInfo(res.Match);
		}
	}



	void ActionTrunEnded (GK_TBM_EndTrunResult result) {
		GameCenter_TBM.ActionTrunEnded -= ActionTrunEnded;
		Debug.Log("ActionTrunEnded IsSucceeded: " + result.IsSucceeded);

		if(result.IsFailed) {
			IOSMessage.Create("ActionTrunEnded", result.Error.Description);
			Debug.Log(result.Error.Description);
		} else {
			GameCenter_TBM.PrintMatchInfo(result.Match);
		}
	}

	void ActionMacthEnded (GK_TBM_MatchEndResult result) {
		GameCenter_TBM.ActionMacthEnded -= ActionMacthEnded;
		Debug.Log("ActionMacthEnded IsSucceeded: " + result.IsSucceeded);

		if(result.IsFailed) {
			Debug.Log(result.Error.Description);
		} else {
			GameCenter_TBM.PrintMatchInfo(result.Match);
		}
	}

	void ActionMacthRemoved (GK_TBM_MatchRemovedResult result) {
		GameCenter_TBM.ActionMacthRemoved -= ActionMacthRemoved;
		Debug.Log("ActionMacthRemoved IsSucceeded: " + result.IsSucceeded);

		if(result.IsFailed) {
			Debug.Log(result.Error.Description);
		} else {
			Debug.Log("Match Id: " + result.MatchId);
		}
	}	

	public GK_TBM_Match CurrentMatch {
		get {
			if(GameCenter_TBM.Instance.Matches.Count > 0) {
				return GameCenter_TBM.Instance.MatchesList[0];
			} else {
				return null;
			}
		}
	}

	void ActionMatchFound (GK_TBM_MatchInitResult result) {
		GameCenter_TBM.ActionMatchFound -= ActionMatchFound;
		Debug.Log("ActionMatchFound IsSucceeded: " + result.IsSucceeded);
		
		if(result.IsFailed) {
			Debug.Log(result.Error.Description);
		} else {
			GameCenter_TBM.PrintMatchInfo(result.Match);
		}
	}

}
