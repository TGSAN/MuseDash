using UnityEngine;
using System.Collections;

public class ISN_LoadSetLeaderboardsInfoResult : ISN_Result {

	public GK_LeaderboardSet _LeaderBoardsSet;

	public ISN_LoadSetLeaderboardsInfoResult(GK_LeaderboardSet lbset, bool IsResultSucceeded):base(IsResultSucceeded) {
		_LeaderBoardsSet = lbset;
	}

	public GK_LeaderboardSet LeaderBoardsSet {
		get {
			return _LeaderBoardsSet;
		}
	}
}
