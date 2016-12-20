using UnityEngine;
using System.Collections;

public class ISN_CheckUrlResult : ISN_Result {

	private string _url;


	public ISN_CheckUrlResult(string checkedUrl, bool IsResultSucceeded):base(IsResultSucceeded) {
		_url = checkedUrl;
	}


	public string url {
		get {
			return _url;
		}
	}
}
