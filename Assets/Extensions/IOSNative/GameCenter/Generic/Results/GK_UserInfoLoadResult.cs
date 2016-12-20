using UnityEngine;
using System.Collections;

public class GK_UserInfoLoadResult : ISN_Result {

	private string _playerId;
	private GK_Player _tpl = null;
	
	
	public GK_UserInfoLoadResult(string id):base(false) {
		_playerId = id;
	}
	
	public GK_UserInfoLoadResult(GK_Player tpl):base(true) {
		_tpl = tpl;
	}
	

	
	public string playerId {
		get {
			return _playerId;
		}
	}	
	
	public GK_Player playerTemplate {
		get {
			return _tpl;
		}
	}
}
