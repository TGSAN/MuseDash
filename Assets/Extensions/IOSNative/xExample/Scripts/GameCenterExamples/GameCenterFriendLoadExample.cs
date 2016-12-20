using UnityEngine;
using System.Collections;

public class GameCenterFriendLoadExample : MonoBehaviour {

	private string ChallengeLeaderboard =  "your.leaderboard2.id.here";
	private string ChallengeAchievement =   "your.achievement.id.here ";
	
	
	public GUIStyle headerStyle;
	public GUIStyle boardStyle;
	


	private bool renderFriendsList = false;
	
	void Awake() {

		GameCenterManager.OnAuthFinished += HandleOnAuthFinished;
		
		//Initializing Game Center class. This action will trigger authentication flow
		GameCenterManager.Init();


	}






	void OnGUI() {
		
		GUI.Label(new Rect(10, 20, 400, 40), "Friend List Load Example", headerStyle);
		
		if(GUI.Button(new Rect(300, 10, 150, 50), "Load Friends")) {
			GameCenterManager.OnFriendsListLoaded += OnFriendsListLoaded;
			GameCenterManager.RetrieveFriends();
		}


		if(!renderFriendsList) {
			return;
		}

		if(GUI.Button(new Rect(500, 10, 180, 50), "Leaberboard Challenge All")) {
			GameCenterManager.IssueLeaderboardChallenge(ChallengeLeaderboard, "Your message here", GameCenterManager.FriendsList.ToArray());
		}
		

		if(GUI.Button(new Rect(730, 10, 180, 50), "Achievement Challenge All")) {
			GameCenterManager.IssueAchievementChallenge(ChallengeAchievement, "Your message here", GameCenterManager.FriendsList.ToArray());
		}
		

		GUI.Label(new Rect(10,  90, 100, 40), "id", boardStyle);
		GUI.Label(new Rect(150, 90, 100, 40), "name", boardStyle);;
		GUI.Label(new Rect(300, 90, 100, 40), "avatar ", boardStyle);

		int i = 1;
		foreach(string FriendId in GameCenterManager.FriendsList) {

			GK_Player player = GameCenterManager.GetPlayerById(FriendId);
			if(player != null) {
				GUI.Label(new Rect(10,  90 + 70 * i, 100, 40), player.Id, boardStyle);
				GUI.Label(new Rect(150, 90 + 70 * i , 100, 40), player.Alias, boardStyle);
				if(player.SmallPhoto != null) {
					GUI.DrawTexture(new Rect(300, 75 + 70 * i, 50, 50), player.SmallPhoto);
				} else  {
					GUI.Label(new Rect(300, 90 + 70 * i, 100, 40), "no photo ", boardStyle);
				}

				if(GUI.Button(new Rect(450, 90 + 70 * i, 150, 30), "Challenge Leaderboard")) {
					GameCenterManager.IssueLeaderboardChallenge(ChallengeLeaderboard, "Your message here", FriendId);
				}

				if(GUI.Button(new Rect(650, 90 + 70 * i, 150, 30), "Challenge Achievement")) {
					GameCenterManager.IssueAchievementChallenge(ChallengeAchievement, "Your message here", FriendId);
				}


				i++;
			}

		}


	}

	void HandleOnAuthFinished (ISN_Result result){
		if (result.IsSucceeded) {
			Debug.Log("Player Authed");
		} else {
			IOSNativePopUpManager.showMessage("Game Center ", "Player authentication failed");
		}
	}
	

	private void OnFriendsListLoaded (ISN_Result result) {
		GameCenterManager.OnFriendsListLoaded -= OnFriendsListLoaded;
		if(result.IsSucceeded) {
			renderFriendsList = true;
		}
				
	}
}
