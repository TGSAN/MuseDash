using UnityEngine;
using System;
using System.Collections;
using cn.bmob.api;
using cn.bmob.io;
using FormulaBase;
using cn.bmob.json;

public class ExpandBmobCall : MonoBehaviour {
	private BmobUnity Bmob;
	
	private static ExpandBmobCall instance = null;
	public static ExpandBmobCall Instance {
		get {
			return instance;
		}
	}

	// Use this for initialization
	void Start () {
		instance = this;
		this.Bmob = this.gameObject.GetComponent<BmobUnity> ();
	}

	public void Call(string funcName, SimpleJson.JsonObject param, HttpEndResponseDelegate delgat) {
		ExpandBmobHeartBeat.Instance.AddWait ();
		Bmob.Endpoint<Hashtable> (funcName, param, (resp, exception) => {
			ExpandBmobHeartBeat.Instance.DelWait ();
			if (exception != null) {
				print ("调用失败, 失败原因为： " + exception.Message);
				return;
			}

			if (delgat != null) {
				delgat (resp);
			}
		});
	}

	/*
	SimpleJson.JsonArray rq = new SimpleJson.JsonArray ();

	SimpleJson.JsonObject _p1 = new SimpleJson.JsonObject ();
	_p1.Add ("method", "GET");
	_p1.Add ("path", "/1/schemas/VerificationCode");
	// _p1.Add ("body", "/1/schemas/");

	SimpleJson.JsonObject _p2 = new SimpleJson.JsonObject ();
	_p2.Add ("method", "GET");
	_p2.Add ("path", "/1/schemas/user_81b4ca2d24");

	rq.Add (_p1);

	SimpleJson.JsonObject param = new SimpleJson.JsonObject ();
	param.Add ("rqs", rq);

	ExpandBmobCall.Instance.Call ("BatchExec", param, null);
	*/
}
