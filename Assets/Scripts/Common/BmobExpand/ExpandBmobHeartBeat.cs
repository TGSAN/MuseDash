using UnityEngine;
using System;
using System.Collections;
using cn.bmob.api;
using cn.bmob.io;
using FormulaBase;
using cn.bmob.json;

public class ExpandBmobHeartBeat : MonoBehaviour {
	private BmobUnity Bmob;
	
	private static ExpandBmobHeartBeat instance = null;
	public static ExpandBmobHeartBeat Instance {
		get {
			return instance;
		}
	}

	private const string DES1 = "很遗憾，服务器连接失败，请重新登录";
	private const string DES2 = "连接恢复";
	// update等待，对于某个scene内还有update中的数据，则需要在全部update等待结束后才能进行场景切换
	// Update检测到有等待时会触发计时器，一定时间后会表示等待超时 
	private int updateWaits = 0;
	// 连接超时状态
	private bool disConnectsed = false;

	// 超时计时器相关
	private int waitCounter = 0;
	[SerializeField]
	public int WAIT_COUNT = 0;

	// Use this for initialization
	void Start () {
		instance = this;
		this.disConnectsed = false;
		this.updateWaits = 0;
		this.waitCounter = 0;
		this.Bmob = this.gameObject.GetComponent<BmobUnity> ();
	}

	void Update() {
		if (!this.IsUpdateWaitting ()) {
			this.waitCounter = 0;
			return;
		}

		if (this.waitCounter < WAIT_COUNT) {
			this.waitCounter += 1;
			return;
		}

		this.waitCounter = 0;
		this.disConnectsed = true;
		if (CommonPanel.GetInstance ()) {
			CommonPanel.GetInstance ().ShowOkBox (DES1, this.RestartGame);
		}
	}

	private void RestartGame() {
		UIManageSystem.g_Instance.RemoveToRoot ();
		Application.LoadLevel (0);
	}

	public bool IsUpdateWaitting() {
		return this.updateWaits > 0;
	}

	public void AddWait() {
		this.updateWaits += 1;
	}

	public void DelWait() {
		this.updateWaits -= 1;
		if (!this.IsUpdateWaitting ()) {
			this.waitCounter = 0;
			if (this.disConnectsed) {
				this.disConnectsed = false;
				if (CommonPanel.GetInstance ()) {
					CommonPanel.GetInstance ().ShowOkBox (DES2);
				}
			}
		}
	}
}
