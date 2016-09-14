using UnityEngine;
using System.Collections;

/// <summary>
/// Http response delegate.
/// 
/// result is data base operate result.
/// 
/// </summary>
public delegate void HttpResponseDelegate(bool result);

/// <summary>
/// Http end response delegate.
/// 
/// response is call back params.
/// </summary>
public delegate void HttpEndResponseDelegate(cn.bmob.response.EndPointCallbackData<Hashtable> response);

/// <summary>
/// Login/Signup response delegate.
/// </summary>
public delegate void LoginResponseDelegate(bool result, cn.bmob.exception.BmobException excp, ExpandBmobUser user);

/// <summary>
/// Http add delegate.
/// </summary>
public delegate void HttpAddDelegate(FormulaBase.FormulaHost tempHost);

public class ExpandBmobCommon {
	private const string _uidHead = "user_";
	private static string _uid = null;
	public static void SetUid(string uid) {
		_uid = _uidHead + uid;
		Debug.Log ("Set uid : " + _uid);
	}

	public static string GetUid() {
		return _uid;
	}

	/// <summary>
	/// Determines if is handset the specified str_handset.
	/// 
	/// 验证手机号码
	/// </summary>
	/// <returns><c>true</c> if is handset the specified str_handset; otherwise, <c>false</c>.</returns>
	/// <param name="str_handset">Str_handset.</param>
	public static bool IsHandset(string str_handset) {
		return System.Text.RegularExpressions.Regex.IsMatch (str_handset, @"^[1]+[3,5]+\d{9}");
	}
}
