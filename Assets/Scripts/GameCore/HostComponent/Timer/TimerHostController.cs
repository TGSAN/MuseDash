using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DYUnityLib;
using FormulaBase;

public class TimerHostController : MonoBehaviour {
	private static TimerHostController instance;
	public static TimerHostController Instance {
		get {
			return instance;
		}
	}

	private static int ONE_SECOND = (int)(1 / FixUpdateTimer.dInterval);
	private int secondCounter = 0;

	private List<FormulaHost> _list;

	void OnEnable () {
		this.secondCounter = 0;
		instance = this;

		TimerComponent.Instance.Init ();
	}

	/*
	// Use this for initialization
	void Start () {
		this.secondCounter = 0;
		instance = this;

		TimerComponent.Instance.Init ();
	}
	*/
	
	void OnDestory() {
		this._list.Clear ();
		this._list = null;
	}
	
	void FixedUpdate() {
		// one second
		this.secondCounter += 1;
		if (this.secondCounter < ONE_SECOND) {
			return;
		}

		this.secondCounter = 0;

		// check timers every one second
		if (this._list == null || this._list.Count <= 0) {
			return;
		}

		for (int i = 0; i < this._list.Count; i++) {
			FormulaHost _timerHost = this._list [i];
			if (_timerHost == null) {
				continue;
			}

			if (_timerHost.GetRealTimeCountDownNow () > 0) {
				continue;
			}

			this._list [i] = null;
			TimerComponent.Instance.TimeUp (_timerHost);
		}
	}

	public void AddTimerHost(FormulaHost host) {
		if (this._list == null) {
			this._list = new List<FormulaHost> ();
		}

		if (this._list.Contains (host)) {
			return;
		}

		this._list.Add (host);
	}
}
