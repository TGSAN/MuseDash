using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using GameLogic;
using DYUnityLib;
using FormulaBase;

public class ShowFPS : MonoBehaviour {
	private static ShowFPS instance = null;
	public static ShowFPS Instance {
		get {
			return instance;
		}
	}

	private static int ONE_SECOND = (int)(1 / FixUpdateTimer.dInterval);
	private int secondCounter = 0;

	private float lastInterval; // Last interval end time
	private int frames = 0; // Frames over current interval
	private float fps = -1f;

	// Use this for initialization
	void Start () {
		this.frames = 0;
		this.lastInterval = Time.realtimeSinceStartup;
		instance = this;
	}
	
	// Update is called once per frame
	void Update () {
		this.frames++;
	}

	void FixedUpdate() {
		this.secondCounter += 1;
		if (this.secondCounter < ONE_SECOND) {
			return;
		}

		this.secondCounter = 0;
		this.UpdateFps ();
	}

	private void UpdateFps() {
		float timeNow = Time.realtimeSinceStartup;
		if (timeNow < this.lastInterval) {
			return;
		}

		float _interval = timeNow - lastInterval;
		this.fps = frames / _interval;
		this.frames = 0;
		this.lastInterval = timeNow;
	}

	public float GetFps() {
		return this.fps;
	}
}
