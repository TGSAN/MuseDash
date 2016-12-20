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

public class LeaderboardCustomGUIExample : MonoBehaviour {

	private string leaderboardId_1 =  "your.ios.leaderbord1.id";
	public int hiScore = 100;


	public GUIStyle headerStyle;
	public GUIStyle boardStyle;


	private GK_Leaderboard loadedLeaderboard = null;

	private GK_CollectionType displayCollection = GK_CollectionType.FRIENDS;

	void Awake() {

		GameCenterManager.OnAuthFinished += OnAuthFinished;



		GameCenterManager.OnScoresListLoaded += OnScoresListLoaded;

		//Initializing Game Center class. This action will trigger authentication flow
		GameCenterManager.Init();
	}
	

	void OnGUI() {

		GUI.Label(new Rect(10, 20, 400, 40), "Custom Leaderboard GUI Example", headerStyle);

		if(GUI.Button(new Rect(400, 10, 150, 50), "Load Friends Scores")) {
			GameCenterManager.LoadScore(leaderboardId_1, 1, 10, GK_TimeSpan.ALL_TIME, GK_CollectionType.FRIENDS);
		}

		if(GUI.Button(new Rect(600, 10, 150, 50), "Load Global Scores")) {
			GameCenterManager.LoadScore(leaderboardId_1, 1, 20, GK_TimeSpan.ALL_TIME, GK_CollectionType.GLOBAL);
		}


		Color defaultColor = GUI.color;

		if(displayCollection == GK_CollectionType.GLOBAL) {
			GUI.color = Color.green;
		}
		if(GUI.Button(new Rect(800, 10, 170, 50), "Displaying Global Scores")) {
			displayCollection = GK_CollectionType.GLOBAL;
		}
		GUI.color = defaultColor;



		if(displayCollection == GK_CollectionType.FRIENDS) {
			GUI.color = Color.green;
		}
		if(GUI.Button(new Rect(800, 70, 170, 50), "Displaying Friends Scores")) {
			displayCollection = GK_CollectionType.FRIENDS;
		}
		GUI.color = defaultColor;

		GUI.Label(new Rect(10,  90, 100, 40), "rank", boardStyle);
		GUI.Label(new Rect(100, 90, 100, 40), "score", boardStyle);
		GUI.Label(new Rect(200, 90, 100, 40), "playerId", boardStyle);
		GUI.Label(new Rect(400, 90, 100, 40), "name ", boardStyle);
		GUI.Label(new Rect(550, 90, 100, 40), "avatar ", boardStyle);

		if(loadedLeaderboard != null) {
			for(int i = 1; i < 10; i++) {
				GK_Score score = loadedLeaderboard.GetScore(i, GK_TimeSpan.ALL_TIME, displayCollection);
				if(score != null) {
					GUI.Label(new Rect(10,  90 + 70 * i, 100, 40), i.ToString(), boardStyle);
					GUI.Label(new Rect(100, 90 + 70 * i, 100, 40), score.LongScore.ToString() , boardStyle);
					GUI.Label(new Rect(200, 90 + 70 * i, 100, 40), score.PlayerId, boardStyle);


					GK_Player player = GameCenterManager.GetPlayerById(score.PlayerId);
					if(player != null) {
						GUI.Label(new Rect(400, 90 + 70 * i , 100, 40), player.Alias, boardStyle);
						if(player.SmallPhoto != null) {
							GUI.DrawTexture(new Rect(550, 75 + 70 * i, 50, 50), player.SmallPhoto);
						} else  {
							GUI.Label(new Rect(550, 90 + 70 * i, 100, 40), "no photo ", boardStyle);
						}
					}

					if(GUI.Button(new Rect(650, 90 + 70 * i, 100, 30), "Challenge")) {
						GameCenterManager.IssueLeaderboardChallenge(leaderboardId_1, "Your message here", score.PlayerId);
					}

				}

			}
		}



	}

	private void OnScoresListLoaded (ISN_Result res) {
		if(res.IsSucceeded) {

			loadedLeaderboard = GameCenterManager.GetLeaderboard(leaderboardId_1);
			IOSMessage.Create("Success", "Scores loaded");
		} else  {
			IOSMessage.Create("Fail", "Failed to load scores");
		}
	}





	private void OnAuthFinished (ISN_Result res) {
		if (res.IsSucceeded) {
			IOSNativePopUpManager.showMessage("Player Authed ", "ID: " + GameCenterManager.Player.Id + "\n" + "Name: " + GameCenterManager.Player.DisplayName);
		} else {
			IOSNativePopUpManager.showMessage("Game Center ", "Player authentication failed");
		}
	}



}
