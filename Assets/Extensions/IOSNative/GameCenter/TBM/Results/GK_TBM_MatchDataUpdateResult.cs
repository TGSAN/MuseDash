using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class GK_TBM_MatchDataUpdateResult : ISN_Result {

	private GK_TBM_Match _Match = null;


	public GK_TBM_MatchDataUpdateResult():base(false) {

	}

	public GK_TBM_MatchDataUpdateResult(GK_TBM_Match updatedMatch):base(true) {
		_Match = updatedMatch;
	}

	public GK_TBM_MatchDataUpdateResult(string errorData):base(errorData) {
	}

	public GK_TBM_Match Match {
		get {
			return _Match;
		}
	}
}
