using UnityEngine;
using System.Collections;

public class GK_TBM_EndTrunResult : ISN_Result {
	
	private GK_TBM_Match _Match = null;

	public GK_TBM_EndTrunResult(GK_TBM_Match match):base(true) {
		_Match = match;
	}
	
	public GK_TBM_EndTrunResult(string errorData):base(errorData) {
	}
	
	
	public GK_TBM_Match Match {
		get {
			return _Match;
		}
	}
}