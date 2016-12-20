using UnityEngine;
using System.Collections;

public class GK_TBM_MatchRemovedResult : ISN_Result {

	private string _MatchId = null;
	
	
	
	public GK_TBM_MatchRemovedResult(string matchId):base(true) {
		_MatchId = matchId;
	}
	
	public GK_TBM_MatchRemovedResult():base(false) {
	
	}
	
	public string MatchId {
		get {
			return _MatchId;
		}
	}
}
