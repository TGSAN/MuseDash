using UnityEngine;
using System.Collections;

public class GK_AchievementProgressResult : ISN_Result {


	private GK_AchievementTemplate _tpl;

	public GK_AchievementProgressResult(GK_AchievementTemplate tpl):base(true) {
		_tpl = tpl;
	}


	public GK_AchievementTemplate info {
		get {
			return _tpl;
		}
	}

	public GK_AchievementTemplate Achievement {
		get {
			return _tpl;
		}
	}
}
