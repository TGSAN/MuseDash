using System;
using UnityEngine;
using System.Collections;
using GameLogic;
using FormulaBase;
using LitJson;
using cn.bmob.io;

public class VerificationBmobObject : BmobTable {
	public String vcode { get; set; }
	public String owner { get; set; }
	public String phone_number { get; set; }
	public BmobBoolean is_used { get; set; }
	
	public override void readFields(BmobInput input) {
		base.readFields (input);
		
		this.vcode = input.getString ("vcode");
		this.owner = input.getString ("owner");
		this.phone_number = input.getString ("phone_number");
		this.is_used = input.getBoolean ("is_used");
	}
	
	public override void write(BmobOutput output, Boolean all) {
		base.write (output, all);
		
		output.Put ("vcode", this.vcode);
		output.Put ("owner", this.owner);
		output.Put ("phone_number", this.phone_number);
		output.Put ("is_used", this.is_used);
	}
}

/// <summary>
/// Verification script.
/// 
/// 验证码、邀请码模块
/// </summary>
public class VerificationScript : MonoBehaviour {
	private const float startWait = 0.1f;
	private const string TABLE_NAME = "VerificationCode";

	private static VerificationScript instance = null;
	public static VerificationScript Instance {
		get {
			return instance;
		}
	}

	private string vCodeOid = null;

	[SerializeField]
	public string phoneNumber;
	public string verificateCode;
	// Use this for initialization
	void Start () {
		instance = this;
		// StartCoroutine (this.__Verification ());
	}

	private IEnumerator __Verification() {
		yield return new WaitForSeconds (startWait);

		this.GetVerificationCode (this.verificateCode, this.phoneNumber);
	}

	/// <summary>
	/// Gets the verification code.
	/// 
	/// 获得验证码数据
	/// </summary>
	/// <param name="phoneNumber">Phone number.</param>
	public void GetVerificationCode(string vcode, string phoneNumber = null) {
		if (phoneNumber == null) {
			phoneNumber = this.phoneNumber;
		}

		if (phoneNumber == null || !ExpandBmobCommon.IsHandset (phoneNumber)) {
			// send msg
			LoginPanel.Instance.LoginBox.SetActive (true);
			CommonPanel.GetInstance ().ShowText ("无效手机号码");
			return;
		}

		HttpEndResponseDelegate rsp = new HttpEndResponseDelegate (this.GetVerificationCodeResponse);
		SimpleJson.JsonObject param = null;
		if (vcode != null) {
			param = new SimpleJson.JsonObject ();
			param.Add ("vcode", vcode);
		}

		Debug.Log ("Send verification code " + vcode + " to server.");
		ExpandBmobCall.Instance.Call ("GetVerificationCode", param, rsp);
		CommonPanel.GetInstance ().ShowText ("验证码别扭地校验中，给我等");
	}

	public void SetVerificationCodeOwner(string userName) {
		Debug.Log ("Verification code " + this.verificateCode + " is owned by " + userName);
		VerificationBmobObject vObj = new VerificationBmobObject ();
		vObj.is_used = true;
		vObj.owner = userName;

		ExpandBmobData.Instance.UpdateRow (TABLE_NAME, this.vCodeOid, vObj, null);
	}

	/// <summary>
	/// Uses the verification code.
	/// 
	/// 使用验证码
	/// </summary>
	/// <param name="objectId">Object identifier.</param>
	/// <param name="phoneNumber">Phone number.</param>
	private void UseVerificationCode(string objectId, string phoneNumber) {
		this.vCodeOid = objectId;
		HttpResponseDelegate rsp = new HttpResponseDelegate (this.UseVerificationCodeResponse);
		VerificationBmobObject vObj = new VerificationBmobObject ();
		vObj.is_used = true;
		vObj.phone_number = this.phoneNumber;

		ExpandBmobData.Instance.UpdateRow (TABLE_NAME, objectId, vObj, rsp);
	}

	private void GetVerificationCodeResponse(cn.bmob.response.EndPointCallbackData<Hashtable> resp) {
		if (resp == null) {
			LoginPanel.Instance.LoginBox.SetActive (true);
			CommonPanel.GetInstance ().ShowText ("验证码无效啊喂");
			return;
		}

		SimpleJson.JsonArray _resultList = resp.data ["results"] as SimpleJson.JsonArray;
		if (_resultList == null || _resultList.Count == 0) {
			if (resp.data.ContainsKey ("msg") && resp.data ["msg"] != null) {
				CommonPanel.GetInstance ().ShowText (resp.data ["msg"].ToString ());
			} else {
				CommonPanel.GetInstance ().ShowText ("验证码无效或者已被NTR，请节哀");
			}

			LoginPanel.Instance.LoginBox.SetActive (true);

			return;
		}

		String vOid = null;
		SimpleJson.JsonObject vResult = null;
		foreach (SimpleJson.JsonObject _result in _resultList) {
			String _oid = _result ["objectId"] as String;
			if (_oid == null) {
				continue;
			}

			vOid = _oid;
			vResult = _result;
			break;
		}

		if (vResult == null) {
			LoginPanel.Instance.LoginBox.SetActive (true);
			CommonPanel.GetInstance ().ShowText ("验证码无效或者已被NTR，请节哀");
			return;
		}

		String _vcode = vResult ["vcode"] as String;
		this.verificateCode = _vcode;
		Debug.Log ("Get Verification Code " + _vcode);

		this.UseVerificationCode (vOid, this.phoneNumber);
	}

	private void UseVerificationCodeResponse(bool result) {
		if (result) {
			WelComeScript.Instance.SetVerficationCode (this.verificateCode);
			CommonPanel.GetInstance ().ShowText ("验证码好不容易地使用成功，鼓掌");
			WelComeScript.Instance.Login ();
			LoginPanel.Instance.LoginBox.SetActive (false);
		} else {
			this.verificateCode = null;
			// CommonPanel.GetInstance ().ShowText ("验证码使用失败");
			LoginPanel.Instance.LoginBox.SetActive (true);
		}
	}
}