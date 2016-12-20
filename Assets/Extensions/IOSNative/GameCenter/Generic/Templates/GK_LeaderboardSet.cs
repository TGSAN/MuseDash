using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GK_LeaderboardSet  {

	public string Title;
	public string Identifier;
	public string GroupIdentifier;

	public List<GK_LeaderBoardInfo> _BoardsInfo =  new List<GK_LeaderBoardInfo>();

	public event Action<ISN_LoadSetLeaderboardsInfoResult> OnLoaderboardsInfoLoaded = delegate {};


	public void LoadLeaderBoardsInfo() {
		GameCenterManager.LoadLeaderboardsForSet(Identifier);
	}


	public void AddBoardInfo(GK_LeaderBoardInfo info) {
		_BoardsInfo.Add(info);
	}

	public void SendFailLoadEvent() {
		ISN_LoadSetLeaderboardsInfoResult res =  new ISN_LoadSetLeaderboardsInfoResult(this, false);
		OnLoaderboardsInfoLoaded(res);
	}

	public void SendSuccessLoadEvent() {
		ISN_LoadSetLeaderboardsInfoResult res =  new ISN_LoadSetLeaderboardsInfoResult(this, true);
		OnLoaderboardsInfoLoaded(res);
	}





	
	public List<GK_LeaderBoardInfo> BoardsInfo {
		get {
			return _BoardsInfo;
		}
	}
}
