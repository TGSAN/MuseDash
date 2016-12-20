////////////////////////////////////////////////////////////////////////////////
//  
// @module IOS Native Plugin for Unity3D 
// @author Osipov Stanislav (Stan's Assets) 
// @support stans.assets@gmail.com 
//
////////////////////////////////////////////////////////////////////////////////



using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameCenterExample : BaseIOSFeaturePreview {
	
	private int hiScore = 200;
	
	
	private string TEST_LEADERBOARD_1 = "your.ios.leaderbord1.id";
	private string TEST_LEADERBOARD_2 = "combined.board.1";
	
	
	
	private string TEST_ACHIEVEMENT_1_ID = "your.achievement.id1.here";
	private string TEST_ACHIEVEMENT_2_ID = "your.achievement.id2.here";
	
	
	private static bool IsInitialized = false;
	private static long LB2BestScores = 0;
	
	
	//--------------------------------------
	// INITIALIZE
	//--------------------------------------
	
	
	
	
	
	void Awake() {

		if(!IsInitialized) {
			
			//Achievement registration. If you skip this step GameCenterManager.achievements array will contain only achievements with reported progress 
			GameCenterManager.RegisterAchievement (TEST_ACHIEVEMENT_1_ID);
			GameCenterManager.RegisterAchievement (TEST_ACHIEVEMENT_2_ID);
			
			
			//Listen for the Game Center events
			GameCenterManager.OnAchievementsProgress += HandleOnAchievementsProgress;
			GameCenterManager.OnAchievementsReset += HandleOnAchievementsReset;
			GameCenterManager.OnAchievementsLoaded += OnAchievementsLoaded;
			

			GameCenterManager.OnScoreSubmitted += OnScoreSubmitted;
			GameCenterManager.OnLeadrboardInfoLoaded += OnLeadrboardInfoLoaded;



			GameCenterManager.OnAuthFinished += OnAuthFinished;
			

			//Initializing Game Center class. This action will trigger authentication flow
			GameCenterManager.Init();
			IsInitialized = true;
		}
		
		
		
	}






	
	//--------------------------------------
	//  PUBLIC METHODS
	//--------------------------------------
	
	void OnGUI() {
		

		UpdateToStartPos();
		
		if(GameCenterManager.Player != null) {
			GUI.Label(new Rect(100, 10, Screen.width, 40), "ID: " + GameCenterManager.Player.Id);
			GUI.Label(new Rect(100, 20, Screen.width, 40), "Name: " +  GameCenterManager.Player.DisplayName);
			GUI.Label(new Rect(100, 30, Screen.width, 40), "Alias: " +  GameCenterManager.Player.Alias);
			
			
			//avatar loading will take a while after the user is connectd
			//so we will draw it only after instantiation to avoid null pointer check
			//if you whant to know exaxtly when the avatar is loaded, you can subscribe on 
			//GameCenterManager.OnUserInfoLoaded action  			
			//and checl for a spesific user ID
			if(GameCenterManager.Player.SmallPhoto != null) {
				GUI.DrawTexture(new Rect(10, 10, 75, 75), GameCenterManager.Player.SmallPhoto);
			}
		}
		
		StartY+= YLableStep;
		StartY+= YLableStep;
		StartY+= YLableStep;
		
		
		GUI.Label(new Rect(StartX, StartY, Screen.width, 40), "Game Center Leaderboards", style);
		

		StartY+= YLableStep;
		
		if(GUI.Button(new Rect(StartX, StartY, buttonWidth, buttonHeight), "Show Leaderboards")) {
			GameCenterManager.ShowLeaderboards ();
		}

		StartX += XButtonStep;
		if(GUI.Button(new Rect(StartX, StartY, buttonWidth, buttonHeight), "Load Sets Info")) {
			GameCenterManager.OnLeaderboardSetsInfoLoaded += OnLeaderboardSetsInfoLoaded;
			GameCenterManager.LoadLeaderboardSetInfo();
		}

		StartX = XStartPos;
		StartY+= YButtonStep;
		if(GUI.Button(new Rect(StartX, StartY, buttonWidth, buttonHeight), "Show Leaderboard 1")) {
			GameCenterManager.ShowLeaderboard(TEST_LEADERBOARD_1);
		}
		
		StartX += XButtonStep;
		if(GUI.Button(new Rect(StartX, StartY, buttonWidth, buttonHeight), "Report Score LB 1")) {
			hiScore++;
			GameCenterManager.ReportScore(hiScore, TEST_LEADERBOARD_1);
			
		}
		
		StartX += XButtonStep;
		if(GUI.Button(new Rect(StartX, StartY, buttonWidth, buttonHeight), "Get Score LB 1")) {
			GameCenterManager.LoadLeaderboardInfo(TEST_LEADERBOARD_1);
		}
		
		
		StartX = XStartPos;
		StartY+= YButtonStep;
		
		StartY+= YLableStep;
		GUI.Label(new Rect(StartX, StartY, Screen.width, 40), "Leaderboard #2, user best score: " + LB2BestScores.ToString(), style);
		
		
		StartY+= YLableStep;
		if(GUI.Button(new Rect(StartX, StartY, buttonWidth, buttonHeight), "Show Leader Board2")) {
			GameCenterManager.ShowLeaderboard(TEST_LEADERBOARD_2);

		}
		
		
		StartX += XButtonStep;
		if(GUI.Button(new Rect(StartX, StartY, buttonWidth, buttonHeight), "Show Leaderboard 2 Today")) {
			GameCenterManager.ShowLeaderboard(TEST_LEADERBOARD_2, GK_TimeSpan.TODAY);
		}
		
		StartX += XButtonStep;
		if(GUI.Button(new Rect(StartX, StartY, buttonWidth, buttonHeight), "Report Score LB2")) {
			hiScore++;
			
			GameCenterManager.OnScoreSubmitted += OnScoreSubmitted;
			GameCenterManager.ReportScore(hiScore, TEST_LEADERBOARD_2);
		}
		
		
		StartX += XButtonStep;
		if(GUI.Button(new Rect(StartX, StartY, buttonWidth, buttonHeight), "Get Score LB 2")) {
			GameCenterManager.LoadLeaderboardInfo(TEST_LEADERBOARD_2);
		}
		
		
		StartX += XButtonStep;
		if(GUI.Button(new Rect(StartX, StartY, buttonWidth, buttonHeight), "Send Challenge")) {
			GameCenterManager.IssueLeaderboardChallenge(TEST_LEADERBOARD_2, "Here's a tiny challenge for you");
		}
		
		
		
		StartX = XStartPos;
		StartY+= YButtonStep;
		StartY+= YLableStep;
		GUI.Label(new Rect(StartX, StartY, Screen.width, 40), "Game Center Achievements", style);
		
		StartY+= YLableStep;
		if(GUI.Button(new Rect(StartX, StartY, buttonWidth, buttonHeight), "Show Achievements")) {
			GameCenterManager.ShowAchievements();
		}
		
		StartX += XButtonStep;
		if(GUI.Button(new Rect(StartX, StartY, buttonWidth, buttonHeight), "Reset Achievements")) {
			GameCenterManager.ResetAchievements();
		}
		
		StartX += XButtonStep;
		if(GUI.Button(new Rect(StartX, StartY, buttonWidth, buttonHeight), "Submit Achievements1")) {
			GameCenterManager.SubmitAchievement(GameCenterManager.GetAchievementProgress(TEST_ACHIEVEMENT_1_ID) + 2.432f, TEST_ACHIEVEMENT_1_ID);
		}
		
		StartX += XButtonStep;
		if(GUI.Button(new Rect(StartX, StartY, buttonWidth, buttonHeight), "Submit Achievements2")) {
			GameCenterManager.SubmitAchievement(88.66f, TEST_ACHIEVEMENT_2_ID, false);
		}
		
		StartX += XButtonStep;
		if(GUI.Button(new Rect(StartX, StartY, buttonWidth, buttonHeight), "Send Challenge")) {
			GameCenterManager.IssueAchievementChallenge(TEST_ACHIEVEMENT_1_ID, "Here's a tiny challenge for you");
		}

	

		
		
		StartX = XStartPos;
		StartY+= YButtonStep;
		StartY+= YLableStep;
		GUI.Label(new Rect(StartX, StartY, Screen.width, 40), "More", style);
		
		StartY+= YLableStep;
		if(GUI.Button(new Rect(StartX, StartY, buttonWidth, buttonHeight), "Retrieve Signature")) {
			GameCenterManager.RetrievePlayerSignature();
			GameCenterManager.OnPlayerSignatureRetrieveResult += OnPlayerSignatureRetrieveResult;
		}
		
	}
	
	//--------------------------------------
	//  GET/SET
	//--------------------------------------
	
	//--------------------------------------
	//  EVENTS
	//--------------------------------------
	
	private void OnAchievementsLoaded(ISN_Result result) {
		
		Debug.Log("OnAchievementsLoaded");
		Debug.Log(result.IsSucceeded);
		
		if(result.IsSucceeded) {
			Debug.Log ("Achievements were loaded from iOS Game Center");
			
			foreach(GK_AchievementTemplate tpl in GameCenterManager.Achievements) {
				Debug.Log (tpl.Id + ":  " + tpl.Progress);
			}
		}
		
	}

	void OnLeaderboardSetsInfoLoaded (ISN_Result res) {
		Debug.Log("OnLeaderboardSetsInfoLoaded");
		GameCenterManager.OnLeaderboardSetsInfoLoaded -= OnLeaderboardSetsInfoLoaded;
		if(res.IsSucceeded) {
			foreach(GK_LeaderboardSet s in GameCenterManager.LeaderboardSets) {
				Debug.Log(s.Title);
				Debug.Log(s.Identifier);
				Debug.Log(s.GroupIdentifier);
			}
		}


		if(GameCenterManager.LeaderboardSets.Count == 0) {
			return;
		}

		GK_LeaderboardSet LeaderboardSet = GameCenterManager.LeaderboardSets[0];
		LeaderboardSet.OnLoaderboardsInfoLoaded += OnLoaderboardsInfoLoaded;
		LeaderboardSet.LoadLeaderBoardsInfo();

	}

	void OnLoaderboardsInfoLoaded (ISN_LoadSetLeaderboardsInfoResult res) {
		res.LeaderBoardsSet.OnLoaderboardsInfoLoaded -= OnLoaderboardsInfoLoaded;

		if(res.IsSucceeded) {
			foreach(GK_LeaderBoardInfo l in res.LeaderBoardsSet.BoardsInfo) {
				Debug.Log(l.Title);
				Debug.Log(l.Description);
				Debug.Log(l.Identifier);
			}
		}

	}

	void HandleOnAchievementsReset (ISN_Result obj){
		Debug.Log ("All Achievements were reset");
	}


	private void HandleOnAchievementsProgress (GK_AchievementProgressResult result) {
		if(result.IsSucceeded) {
			GK_AchievementTemplate tpl = result.Achievement;
			Debug.Log (tpl.Id + ":  " + tpl.Progress.ToString());
		}
	}

	void OnScoreSubmitted (GK_LeaderboardResult result) {
		if(result.IsSucceeded) {
			GK_Score score = result.Leaderboard.GetCurrentPlayerScore(GK_TimeSpan.ALL_TIME, GK_CollectionType.GLOBAL);
			IOSNativePopUpManager.showMessage("Leaderboard " + score.LongScore, "Score: " + score.LongScore + "\n" + "Rank:" + score.Rank);
		}
	}

	
	private void OnLeadrboardInfoLoaded (GK_LeaderboardResult result) {
		if(result.IsSucceeded) {
			GK_Score score = result.Leaderboard.GetCurrentPlayerScore(GK_TimeSpan.ALL_TIME, GK_CollectionType.GLOBAL);
			IOSNativePopUpManager.showMessage("Leaderboard " + score.LeaderboardId, "Score: " + score.LongScore + "\n" + "Rank:" + score.Rank);
			
			Debug.Log("double score representation: " + score.DecimalFloat_2);
			Debug.Log("long score representation: " + score.LongScore);
			
			if(score.LeaderboardId.Equals(TEST_LEADERBOARD_2)) {
				Debug.Log("Updating leaderboard 2 score");
				LB2BestScores = score.LongScore;
				
			}
		}
	}
	
	private void OnScoreSubmitted (ISN_Result result) {
		GameCenterManager.OnScoreSubmitted -= OnScoreSubmitted;
		
		if(result.IsSucceeded)  {
			Debug.Log("Score Submitted");
		} else {
			Debug.Log("Score Submit Failed");
		}
	}
	
	
	void OnAuthFinished (ISN_Result res) {
		if (res.IsSucceeded) {
			IOSNativePopUpManager.showMessage("Player Authed ", "ID: " + GameCenterManager.Player.Id + "\n" + "Alias: " + GameCenterManager.Player.Alias);
			GameCenterManager.LoadLeaderboardInfo(TEST_LEADERBOARD_1);
		} else {
			IOSNativePopUpManager.showMessage("Game Center ", "Player authentication failed");
		}
	}
	
	
	void OnPlayerSignatureRetrieveResult (GK_PlayerSignatureResult result) {
		Debug.Log("OnPlayerSignatureRetrieveResult");
		
		if(result.IsSucceeded) {
			
			Debug.Log("PublicKeyUrl: " + result.PublicKeyUrl);
			Debug.Log("Signature: " + result.Signature);
			Debug.Log("Salt: " + result.Salt);
			Debug.Log("Timestamp: " + result.Timestamp);

		} else {
			Debug.Log("Error code: " + result.Error.Code);
			Debug.Log("Error description: " + result.Error.Description);
		}
		
		GameCenterManager.OnPlayerSignatureRetrieveResult -= OnPlayerSignatureRetrieveResult;



	}


	
	
	
	//--------------------------------------
	//  PRIVATE METHODS
	//--------------------------------------
	
	//--------------------------------------
	//  DESTROY
	//--------------------------------------
	
	

}
