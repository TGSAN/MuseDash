using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class GK_TBM_LoadMatchesResult : ISN_Result {

	public Dictionary<string, GK_TBM_Match> LoadedMatches = new Dictionary<string, GK_TBM_Match>();

	public GK_TBM_LoadMatchesResult(bool IsResultSucceeded):base(IsResultSucceeded) {

	}

	public GK_TBM_LoadMatchesResult(string errorData):base(errorData) {
		
	}
}
