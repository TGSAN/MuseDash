using UnityEngine;
using System;
using System.Collections;
using cn.bmob.api;
using cn.bmob.io;

// 如果程序需要为用户添加额外的字段，需要继承BmobUser
public class ExpandBmobUser : BmobUser {
	public String did { get; set; }
	
	public override void write(BmobOutput output, bool all) {
		this.did = UnityEngine.SystemInfo.deviceUniqueIdentifier;

		base.write(output, all);

		output.Put("did", this.did);
	}
	
	public override void readFields(BmobInput input) {
		base.readFields(input);
		
		this.did = input.getString("did");
	}
}

public class ExpandBmobLogin : MonoBehaviour {
	public static String ServerTime = null;
	public static BmobInt ServerTimeStamp = -1;
	public static float ServerTimeRefleshAt = -1f;

	private BmobUnity Bmob;

	private static ExpandBmobLogin instance = null;
	public static ExpandBmobLogin Instance {
		get {
			return instance;
		}
	}

	[SerializeField]
	public string username;
	public string password;
	// Use this for initialization
	void Start () {
		instance = this;
		Bmob = gameObject.GetComponent<BmobUnity> ();
	}
	
	public void Signup(string _username = null, string _password = null, LoginResponseDelegate rsp = null) {
		if (_username != null) {
			this.username = _username;
		}

		if (_password != null) {
			this.password = _password;
		}

		if (this.username == null || this.password == null) {
			Debug.Assert (false, "Signup with null username or password.");
			return;
		}

		Debug.Log ("Signup with " + this.username + " / " + this.password);
		ExpandBmobUser user = new ExpandBmobUser ();
		user.username = this.username;
		user.password = this.password;
		// user.email = "support@bmob.cn";
		
		Bmob.Signup<ExpandBmobUser> (user, (resp, exception) => {
			if (exception != null) {
				string failMsg = "注册失败";//, 失败原因为： " + exception.Message;
				print (failMsg);
				//CommonPanel.GetInstance ().ShowText (failMsg);
				if (rsp != null) {
					rsp (false, exception, resp);
				}

				return;
			}
			
			print ("注册成功" + resp);
			ExpandBmobCommon.SetUid (resp.objectId);
			if (rsp != null) {
				rsp (true, exception, resp);
			}
		});
	}

	public void Login(string _username = null, string _password = null, LoginResponseDelegate rsp = null) {
		if (_username != null) {
			this.username = _username;
		}
		
		if (_password != null) {
			this.password = _password;
		}

		if (this.username == null || this.password == null) {
			Debug.Assert (false, "Login with null username or password.");
			return;
		}

		Debug.Log ("Login with " + this.username + " / " + this.password);
		Bmob.Login<ExpandBmobUser> (this.username, this.password, (resp, exception) => {
			if (exception != null) {
				string failMsg = "登录失败";//, 失败原因为 :" + exception.Message;
				print (failMsg + "失败原因为 :" + exception.Message);
				//CommonPanel.GetInstance ().ShowText (failMsg);
				if (rsp != null) {
					rsp (false, exception, resp);
				}
				return;
			}
			
			print ("登录成功, @" + resp.username + "(" + resp.objectId + ")$[" + resp.sessionToken + "]");
			print ("登录成功, 当前用户对象Session： " + BmobUser.CurrentUser.sessionToken);
			this.GetSeverTimer ();
			ExpandBmobCommon.SetUid (resp.objectId);
			if (rsp != null) {
				rsp (true, exception, resp);
			}
		});
	}

	public void GetSeverTimer() {
		Bmob.Timestamp ((resp, exception) => {
			if (exception != null) {
				print ("请求失败, 失败原因为： " + exception.Message);
				return;
			}
			//返回服务器时间（单位：秒）
			ExpandBmobLogin.ServerTime = resp.datetime;
			ExpandBmobLogin.ServerTimeStamp = resp.timestamp;
			ExpandBmobLogin.ServerTimeRefleshAt = Time.realtimeSinceStartup;
			print ("当前服务器时间： " + ExpandBmobLogin.ServerTime + " / " + ExpandBmobLogin.ServerTimeStamp + "(" + ExpandBmobLogin.ServerTimeRefleshAt);
			//print ("当前系统器时间： " + System.DateTime.Now.ToString () + " / " + System.DateTime.Now.Ticks.ToString ());
		});
	}
}
