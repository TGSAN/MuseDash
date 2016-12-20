using UnityEngine;
using System.Collections;

public class GK_TBM_MatchQuitResult : ISN_Result {

	private string _MatchId = null;
	
	
	
	public GK_TBM_MatchQuitResult(string matchId):base(true) {
		_MatchId = matchId;
	}
	
	public GK_TBM_MatchQuitResult():base(false) {
	
	}
	
	public string MatchId {
		get {
			return _MatchId;
		}
	}
}
