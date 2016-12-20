////////////////////////////////////////////////////////////////////////////////
//  
// @module IOS Native Plugin for Unity3D 
// @author Osipov Stanislav (Stan's Assets) 
// @support stans.assets@gmail.com 
//
////////////////////////////////////////////////////////////////////////////////


using UnityEngine;
using System.Collections;

public class GK_Score  {
	

	private int _Rank;
	private long _Score;

	private string _PlayerId;
	private string _LeaderboardId;

	private GK_CollectionType _Collection;
	private GK_TimeSpan _TimeSpan;


	public GK_Score(long vScore, int vRank, GK_TimeSpan vTimeSpan, GK_CollectionType sCollection, string lid, string pid) {
		_Score = vScore; 
		_Rank = vRank;
		
		_PlayerId = pid;
		_LeaderboardId = lid;
		
		
		_TimeSpan  = vTimeSpan;
		_Collection = sCollection;
		
	}


	//--------------------------------------
	// GET / SET
	//--------------------------------------
	


	public int Rank {
		get {
			return _Rank;
		}
	}
	
	public long LongScore {
		get {
			return _Score;
		}
	}


	
	public float CurrencyScore {
		get {
			return _Score / 100.0f;
		}
	}



	public float DecimalFloat_1 {
		get {
			return _Score / 10.0f;
		}
	}

	public float DecimalFloat_2 {
		get {
			return _Score / 100.0f;
		}
	}

	public float DecimalFloat_3 {
		get {
			return _Score / 100.0f;
		}
	}



	public System.TimeSpan Minutes {
		get {
			return System.TimeSpan.FromMinutes(_Score);
		}
	}

	public System.TimeSpan Seconds {
		get {
			
			return System.TimeSpan.FromSeconds(_Score);
		}
	}

	public System.TimeSpan Milliseconds {
		get {
			return System.TimeSpan.FromMilliseconds(_Score);
		}
	}




	
	public string PlayerId {
		get {
			return _PlayerId;
		}
	}

	public GK_Player Player {
		get {
			return GameCenterManager.GetPlayerById(PlayerId);
		}
	}
	
	public string LeaderboardId {
		get {
			return _LeaderboardId;
		}
	}

	public GK_Leaderboard Leaderboard {
		get {
			return GameCenterManager.GetLeaderboard(LeaderboardId);
		}
	}

	public GK_CollectionType Collection {
		get {
			return _Collection;
		}
	}
	
	public GK_TimeSpan TimeSpan {
		get {
			return _TimeSpan;
		}
	}



	
	//--------------------------------------
	// Deprectaed
	//--------------------------------------

	[System.Obsolete("rank is deprectaed, plase use Rank instead")]
	public int rank {
		get {
			return _Rank;
		}
	}

	[System.Obsolete("score is deprectaed, plase use LongScore instead")]
	public long score {
		get {
			return _Score;
		}
	}


	[System.Obsolete("playerId is deprectaed, plase use PlayerId instead")]
	public string playerId {
		get {
			return _PlayerId;
		}
	}
	
	[System.Obsolete("leaderboardId is deprectaed, plase use LeaderboardId instead")]
	public string leaderboardId {
		get {
			return _LeaderboardId;
		}
	}


	[System.Obsolete("timeSpan is deprectaed, plase use TimeSpan instead")]
	public GK_TimeSpan timeSpan {
		get {
			return _TimeSpan;
		}
	}


	[System.Obsolete("collection is deprectaed, plase use Collection instead")]
	public GK_CollectionType collection {
		get {
			return _Collection;
		}
	}
	


}

