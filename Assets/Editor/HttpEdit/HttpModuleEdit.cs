using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;
using System.Collections;
using System.Text.RegularExpressions;
using LitJson;
/// <summary>
/// Formula edit.
/// </summary>
using System;
using FormulaBase;
using cn.bmob.json;
using cn.bmob.io;
using cn.bmob.api;

public class ExpandVerificationGameObject : BmobTable {
	public String vcode { get; set; }
	public BmobBoolean is_used { get; set; }
	public String phone_number { get; set; }
	
	public override void readFields(BmobInput input) {
		base.readFields (input);
		
		this.vcode = input.getString ("vcode");
		this.is_used = input.getBoolean ("is_used");
		this.phone_number = input.getString ("phone_number");
	}
	
	public override void write(BmobOutput output, Boolean all) {
		base.write (output, all);
		
		output.Put ("vcode", this.vcode);
		output.Put ("is_used", this.is_used);
		output.Put ("phone_number", this.phone_number);
	}
}

public class HttpModuleEdit : EditorWindow {
	private Vector2 scorllVecAction;
	//private bool isSaving = false;
	private Dictionary<int, String> genVcode = null;
	private FormulaBase.FormulaHost tempHost;
	[MenuItem("RHY/Http模块编辑")]
	static void Init () {
		HttpModuleEdit window = (HttpModuleEdit)EditorWindow.GetWindow (typeof(HttpModuleEdit));
		window.Show ();
	}

	void OnGUI() {
		//this.isSaving = false;
		EditorGUILayout.BeginVertical ();
		this.MkTitle ();
		EditorGUILayout.EndVertical ();
	}

	private void MkTitle() {
		EditorGUILayout.BeginHorizontal ();
		EditorGUILayout.LabelField ("协议刷新");
		if (GUILayout.Button ("R")) {
			Debug.Log ("Count : " + FormulaBase.FomulaHostManager.Instance.Count);
			/*
			//string _param = "f=rhygame.ProtocalUpdate.Update&p=";
			HttpResponseDelegate rsp = new HttpResponseDelegate (this.HttpDelegate);
			string _param = "f=rhygame.ProtocalUpdate.Update";
			HttpModule.Instance.Request (_param, rsp);
			*/
		}

		if (GUILayout.Button ("Test")) {
		}

		EditorGUILayout.EndHorizontal ();

		EditorGUILayout.BeginHorizontal ();
		if (GUILayout.Button ("Login")) {
			ExpandBmobLogin.Instance.Login ();
		}

		if (GUILayout.Button ("Load")) {
			FormulaBase.FomulaHostManager.Instance.LoadAllHost ();
		}

		if (GUILayout.Button ("Add")) {
			FormulaBase.FormulaHost th = FormulaBase.FomulaHostManager.Instance.CreateHost (FormulaBase.HostKeys.HOST_0);
			th.Save ();
			this.tempHost = th;
		}

		if (GUILayout.Button ("Del")) {
			if (this.tempHost != null) {
				this.tempHost.Delete ();
			}

			this.tempHost = null;
		}

		if (GUILayout.Button ("DeleteAll")) {
			FormulaBase.FomulaHostManager.Instance.DeleteAllHost ();
		}

		EditorGUILayout.EndHorizontal ();


		EditorGUILayout.BeginHorizontal ();
		// 验证码生成
		if (GUILayout.Button ("Verification Code Generator")) {
			int genCount = 10;
			this.genVcode = new Dictionary<int, String> ();
			for (int i = 0; i < genCount; i++) {
				this.genVcode [i] = UnityEngine.Random.Range (100000, 999999).ToString ();
			}

			HttpEndResponseDelegate rsp = new HttpEndResponseDelegate (this.VerificationCodeLoadDelegate);
			ExpandBmobData.Instance.LoadAll ("VerificationCode", rsp);
		}

		EditorGUILayout.EndHorizontal ();

		EditorGUILayout.BeginHorizontal ();
		// user_81b4ca2d24
		if (GUILayout.Button ("Save list")) {
			List<FormulaHost> list = new List<FormulaHost> ();
			Dictionary<string, FormulaHost> hosts = FomulaHostManager.Instance.GetHostListByFileName ("Pet");
			if (hosts == null || hosts.Count <= 0) {
				for (int i = 0; i < 3; i++) {
					list.Add (FomulaHostManager.Instance.CreateHost (HostKeys.HOST_10));
				}
			} else {
				int k = 0;
				foreach (FormulaHost _h in hosts.Values) {
					int cc = (int)_h.GetDynamicDataByKey (SignKeys.CHOSED) + k;
					_h.SetDynamicData (SignKeys.CHOSED, cc);
					list.Add (_h);
					k += 1;
				}
			}

			HttpEndResponseDelegate rsp = new HttpEndResponseDelegate (this.ttttt);
			FormulaHost.SaveList (list, rsp);
		}

		if (GUILayout.Button ("Delete list")) {
			Dictionary<string, FormulaHost> hosts = FomulaHostManager.Instance.GetHostListByFileName ("Pet");
			HttpEndResponseDelegate rsp = new HttpEndResponseDelegate (this.ttttt);
			FormulaHost.DeleteList (hosts.Values.ToList (), rsp);
		}

		EditorGUILayout.EndHorizontal ();

		EditorGUILayout.BeginHorizontal ();
		if (GUILayout.Button ("Real time count down set.")) {
			FormulaHost h = FomulaHostManager.Instance.LoadHost (HostKeys.HOST_16);
			//h.SetRealTimeCountDown (TimeZone.CurrentTimeZone.ToLocalTime (new System.DateTime (2016, 6, 30, 18, 0, 0, 0)));
			h.SetRealTimeCountDown (3600);
			//System.DateTime dt = new DateTime (2016, 6, 30, 18, 15, 15);
			//h.SetRealTimeCountDown (dt);
			h.Save ();
		}

		if (GUILayout.Button ("Real time count down Load.")) {
			FormulaHost h = FomulaHostManager.Instance.LoadHost (HostKeys.HOST_16);
			Debug.Log (": time passed " + h.GetRealTimeCountDownPass () + " / " + h.GetRealTimeCountDownNow ());
		}

		EditorGUILayout.EndHorizontal ();
	}

	private void ttttt(cn.bmob.response.EndPointCallbackData<Hashtable> response) {
		Dictionary<string, FormulaHost> hosts = FomulaHostManager.Instance.GetHostListByFileName ("Pet");
		if (hosts == null || hosts.Count <= 0) {
			Debug.Log ("Delete all Pet !!");
		}

		foreach (FormulaHost _h in hosts.Values) {
			Debug.Log (_h.objectID + "  " + _h.SignToJson ().ToJson ());
		}
	}

	// --------------------------------------------------------------------------------------------------
	private void HttpDelegate(JsonData response) {
		if (response != null) {
			HttpGenerator.WriteCode (response);
		}
	}

	/// <summary>
	/// Verifications the code load delegate.
	/// 
	/// 验证码生成
	/// </summary>
	/// <param name="resp">Resp.</param>
	private void VerificationCodeLoadDelegate(cn.bmob.response.EndPointCallbackData<Hashtable> resp) {
		if (resp == null) {
			return;
		}
		
		SimpleJson.JsonArray _resultList = resp.data ["results"] as SimpleJson.JsonArray;
		if (_resultList != null || _resultList.Count > 0) {
			foreach (SimpleJson.JsonObject _result in _resultList) {
				String _oid = _result ["objectId"] as String;
				if (_oid == null) {
					continue;
				}
				
				String _vcode = _result ["vcode"] as String;
				// Debug.Log ("vcode : " + _result ["vcode"] + " / " + _result ["vcode"].GetType ());
				
				for (int i = 0; i < this.genVcode.Count; i++) {
					if (!this.genVcode.ContainsKey (i)) {
						continue;
					}
					
					String _gencode = this.genVcode [i];
					if (_gencode != _vcode) {
						continue;
					}
					
					this.genVcode [i] = null;
				}
			}
		}

		string forPrint = "";
		BmobUnity Bmob = GameObject.Find ("Bmob").gameObject.GetComponent<BmobUnity> ();
		for (int i = 0; i < this.genVcode.Count; i++) {
			if (!this.genVcode.ContainsKey (i)) {
				continue;
			}
			
			String _gencode = this.genVcode [i];
			if (_gencode == null) {
				continue;
			}

			forPrint += (" " + _gencode);

			ExpandVerificationGameObject data = new ExpandVerificationGameObject();
			data.vcode = _gencode;
			data.is_used = false;
			Bmob.Create ("VerificationCode", data, (rresp, exception) => {
				if (exception != null) {
					string failMsg = "添加数据失败, 失败原因为： " + exception.Message;
					Debug.Log (failMsg);
					return;
				}
				
				Debug.Log ("创建成功, @" + rresp.createdAt);
			});
		}

		Debug.Log("New Verification code : " + forPrint);
	}
}