////////////////////////////////////////////////////////////////////////////////
//  
// @module IOS Native Plugin for Unity3D 
// @author Osipov Stanislav (Stan's Assets) 
// @support stans.assets@gmail.com 
//
////////////////////////////////////////////////////////////////////////////////



using UnityEngine;

public class JSHelper : MonoBehaviour {
	
	private string leaderboardId =  "your.leaderboard1.id.here";


	private string TEST_ACHIEVEMENT_1_ID = "your.achievement.id1.here";
	private string TEST_ACHIEVEMENT_2_ID = "your.achievement.id2.here";

	//--------------------------------------
	// INITIALIZE
	//--------------------------------------




	void InitGameCneter() {


		//Achievement registration. If you will skipt this step GameCenterManager.achievements array will contain only achievements with reported progress 
		GameCenterManager.RegisterAchievement (TEST_ACHIEVEMENT_1_ID);
		GameCenterManager.RegisterAchievement (TEST_ACHIEVEMENT_2_ID);


		//Listen for the Game Center events
		GameCenterManager.OnAchievementsLoaded += HandleOnAchievementsLoaded;
		GameCenterManager.OnAchievementsProgress += HandleOnAchievementsProgress;
		GameCenterManager.OnAchievementsReset += HandleOnAchievementsReset;


		GameCenterManager.OnScoreSubmitted += OnScoreSubmitted;
		GameCenterManager.OnAuthFinished += HandleOnAuthFinished;
	

		DontDestroyOnLoad (gameObject);

		GameCenterManager.Init();
		
		Debug.Log("InitGameCenter");
	}










	

	private void SubmitScore(int val) {
		Debug.Log("SubmitScore");
		GameCenterManager.ReportScore(val, leaderboardId);
	}
	
	private void SubmitAchievement(string data) {
		
		string[] arr;
		arr = data.Split("|" [0]);
		
		float percent = System.Convert.ToSingle(arr[0]);
		string achievementId = arr[1];

		
		
		Debug.Log("SubmitAchievement: " + achievementId + "  " + percent.ToString());
		GameCenterManager.SubmitAchievement(percent, achievementId);
	}
	
	
	
	//--------------------------------------
	//  GET/SET
	//--------------------------------------
	
	//--------------------------------------
	//  EVENTS
	//--------------------------------------
	
	void HandleOnAchievementsLoaded (ISN_Result res) {
		Debug.Log ("Achievements loaded from iOS Game Center");
		
		foreach(GK_AchievementTemplate tpl in GameCenterManager.Achievements) {
			Debug.Log (tpl.Id + ":  " + tpl.Progress);
		}
	}

	void HandleOnAchievementsProgress (GK_AchievementProgressResult progress) {
		Debug.Log ("OnAchievementProgress");
		
		GK_AchievementTemplate tpl = progress.Achievement;
		Debug.Log (tpl.Id + ":  " + tpl.Progress.ToString());
	}

	void HandleOnAchievementsReset (ISN_Result res) {
		Debug.Log ("All Achievements were reset");
	}

	void OnScoreSubmitted (GK_LeaderboardResult result) {
		if(result.IsSucceeded) {
			GK_Score score = result.Leaderboard.GetCurrentPlayerScore(GK_TimeSpan.ALL_TIME, GK_CollectionType.GLOBAL);
			IOSNativePopUpManager.showMessage("Leaderboard " + score.LeaderboardId, "Score: " + score.LongScore + "\n" + "Rank:" + score.Rank);
		}
	}

	void HandleOnAuthFinished (ISN_Result r) {
		if (r.IsSucceeded) {
			IOSNativePopUpManager.showMessage("Player Authenticated", "ID: " + GameCenterManager.Player.Id + "\n" + "Name: " + GameCenterManager.Player.DisplayName);
		}
	}
	

}
