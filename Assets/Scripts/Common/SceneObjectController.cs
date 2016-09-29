using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using DYUnityLib;
using GameLogic;
using System.Collections.Generic;
using System;

public class SceneObjectController : MonoBehaviour {
	private int idx;
	private int timeEventIndex;
	private float runtime;
	private StageEvent eventData;
	private Hashtable sceneObjectPool;

	private AudioSource sceneAudio;
	private List<GameObject> dymObjectList = null;

	private static SceneObjectController instance = null;
	public static SceneObjectController Instance {
		get {
			return instance;
		}
	}

	public Hashtable SceneObjectPool {
		get {
			return sceneObjectPool;
		}
	}

	// Use this for initialization
	void Start () {
		instance = this;
	}

	// void FixedUpdate() {
	public void TimerStepTrigger(object sender, uint triggerId, params object[] args){
		if (FixUpdateTimer.IsPausing ()) {
			return;
		}

		if (this.runtime < 0) {
			return;
		}

		if (this.timeEventIndex >= this.eventData.timeEvents.Length) {
			return;
		}

		//this.runtime += FixUpdateTimer.fInterval;
		this.runtime = (float)((decimal)args [0]);
		this.runtime = (int)(this.runtime * 100) * 0.01f;
		StageTimeEvent tEvent = this.eventData.timeEvents [this.timeEventIndex];
		if (this.runtime < tEvent.time) {
			return;
		}

		Debug.Log ("tEvent " + this.timeEventIndex + " time is " + tEvent.time + " real time is " + this.runtime);

		this.timeEventIndex += 1;
		for (int i = 0; i < tEvent.eventItems.Length; i++) {
			StageTimeEventItem eventItem = tEvent.eventItems [i];
			if (eventItem.sceneObject == null) {
				return;
			}

			string actKey = EditorData.Instance.SpineActionKeys [eventItem.actionIndex];
			if (actKey == null) {
				continue;
			}

			GameObject dymObj = this.CreateObj (eventItem.sceneObject);
			dymObj.SetActive (true);
			SpineActionController sac = dymObj.GetComponent<SpineActionController> ();
			if (sac != null) {
				sac.OnControllerStart ();
			}

			SpineActionController.Play (actKey, dymObj);
			this.ResetParticle (dymObj);
			if (eventItem.soundPath == null || eventItem.soundPath.Length == 0) {
				continue;
			}

			FormulaBase.SoundEffectComponent.Instance.Say (dymObj.name, eventItem.soundPath);
		}
	}

	public void Init(int idx, string audioLayerName = "BossLayer") {
		this.idx = idx;
		Debug.Log ("Use scene event config " + this.idx);

		this.runtime = -1f;
		this.timeEventIndex = 0;
		this.sceneObjectPool = new Hashtable ();
		this.eventData = EditorData.Instance.GetStageEventDataById (this.idx);
		this.sceneAudio = GameObject.Find (audioLayerName).GetComponent<AudioSource> ();
		this.PreLoad ();

		gTrigger.UnRegEvent (GameGlobal.SCENO_OBJ_STEP_EVENT);
		EventTrigger stPress = gTrigger.RegEvent (GameGlobal.SCENO_OBJ_STEP_EVENT);
		stPress.Trigger += TimerStepTrigger;
	}

	public void Run() {
		if (this.eventData.timeEvents == null) {
			return;
		}

		if (this.eventData.timeEvents.Length == 0) {
			return;
		}

		this.runtime = 0;
	}

	public void PreLoad() {
		AudioManager.Instance.InitSceneObjectAudio ();
		if (this.eventData.timeEvents == null) {
			return;
		}

		this.dymObjectList = new List<GameObject> ();
		for (int i = 0; i < this.eventData.timeEvents.Length; i++) {
			StageTimeEvent tEvent = this.eventData.timeEvents [i];
			for (int k = 0; k < tEvent.eventItems.Length; k++) {
				StageTimeEventItem eventItem = tEvent.eventItems [k];
				if (eventItem.sceneObject != null) {
					//Instantiate (eventItem.sceneObject);
					GameObject _preLoadObj = this.CreateObj (eventItem.sceneObject);
					_preLoadObj.SetActive (false);
					this.dymObjectList.Insert (this.dymObjectList.Count, _preLoadObj);
				}

				if (eventItem.soundPath == null || eventItem.soundPath.Length == 0) {
					continue;
				}

				AudioManager.Instance.PreLoadSceneObjectAudio (eventItem.soundPath);
			}
		}

		Resources.UnloadUnusedAssets ();
		System.GC.Collect ();
	}

	public void DoObjectComeoutEvent(string nodeId) {
		if (this.eventData.actionEvents == null) {
			return;
		}

		if (this.eventData.actionEvents.Length == 0) {
			return;
		}

		for (int i = 0; i < this.eventData.actionEvents.Length; i++) {
			StageActionEvent _event = this.eventData.actionEvents [i];
			if (_event.nodeUid != nodeId) {
				continue;
			}

			if (_event.sceneObject == null) {
				continue;
			}

			string actKey = EditorData.Instance.SpineActionKeys [_event.bornActionIndex];
			GameObject obj = this.CreateObj (_event.sceneObject);
			obj.SetActive(true);
			SpineActionController.Play (actKey, obj);
			this.ResetParticle (obj);
		}
	}

	public void DoObjectOnAttackedEvent(string nodeId) {
		if (this.eventData.actionEvents == null) {
			return;
		}

		if (this.eventData.actionEvents.Length == 0) {
			return;
		}

		for (int i = 0; i < this.eventData.actionEvents.Length; i++) {
			StageActionEvent _event = this.eventData.actionEvents [i];
			if (_event.nodeUid != nodeId) {
				continue;
			}
			
			if (_event.sceneObject == null) {
				continue;
			}
			
			string actKey = EditorData.Instance.SpineActionKeys [_event.hittedActionIndex];
			GameObject obj = this.CreateObj (_event.sceneObject);
			obj.SetActive(true);
			SpineActionController.Play (actKey, obj);
			this.ResetParticle (obj);
		}
	}

	public void DoObjectOnMissedEvent(string nodeId) {
		if (this.eventData.actionEvents == null) {
			return;
		}

		if (this.eventData.actionEvents.Length == 0) {
			return;
		}

		for (int i = 0; i < this.eventData.actionEvents.Length; i++) {
			StageActionEvent _event = this.eventData.actionEvents [i];
			if (_event.nodeUid != nodeId) {
				continue;
			}

			if (_event.sceneObject == null) {
				continue;
			}

			string actKey = EditorData.Instance.SpineActionKeys [_event.missActionIndex];
			GameObject obj = this.CreateObj (_event.sceneObject);
			obj.SetActive(true);
			SpineActionController.Play (actKey, obj);
			this.ResetParticle (obj);
		}
	}

	public float GetRunTime() {
		return this.runtime;
	}

	public AudioSource GetAudioSource() {
		return this.sceneAudio;
	}

	public void TimerJump(float tick) {
		this.runtime = (int)(tick * 100.0f) * 0.01f;
		if (this.eventData.timeEvents == null || this.eventData.timeEvents.Length == 0) {
			Debug.Log ("Scene object controller jump to " + this.runtime + " / " + this.timeEventIndex);
			return;
		}
		
		this.timeEventIndex = 0;
		for (int i = 0; i < this.eventData.timeEvents.Length; i++) {
			StageTimeEvent tEvent = this.eventData.timeEvents [this.timeEventIndex];
			if (this.runtime > tEvent.time) {
				this.timeEventIndex += 1;
			}
		}
		
		foreach (GameObject obj in this.dymObjectList) {
			if (obj == null) {
				continue;
			}
			
			obj.SetActive (false);
			AudioSource ac = obj.GetComponent<AudioSource> ();
			if (ac != null) {
				ac.time = 0;
				ac.Stop ();
			}
		}

		Debug.Log ("Scene object controller jump to " + this.runtime + " / " + this.timeEventIndex);
	}

	public List<GameObject> GetAllSceneEventObjects() {
		return this.dymObjectList;
	}

	public void ActiveObject(int idx) {
		if (this.dymObjectList == null || this.dymObjectList.Count < idx) {
			return;
		}

		GameObject obj = this.dymObjectList [idx];
		if (obj == null) {
			return;
		}

		obj.SetActive (true);
	}

	private GameObject CreateObj(GameObject sourceObject) {
		if (sourceObject == null) {
			return null;
		}

		string objName = sourceObject.name;
		GameObject buffObject = (GameObject)this.sceneObjectPool [objName];
		if (buffObject != null) {
			return buffObject;
		}

		int buffObjectIndexHead = 2000;
		buffObject = (GameObject)Instantiate (sourceObject);
		if (buffObject != null) {
			int _idx = buffObjectIndexHead + this.sceneObjectPool.Count;
			NodeInitController initController = buffObject.GetComponent<NodeInitController> ();
			if (initController == null) {
				initController = buffObject.AddComponent<NodeInitController> ();
			}

			initController.Init (_idx);
			
			SpineActionController sac = buffObject.GetComponent<SpineActionController> ();
			if (sac != null) {
				sac.Init (_idx);
			}
			
			buffObject.SetActive (true);
		}

		this.sceneObjectPool [objName] = buffObject;
		return buffObject;
	}

	private void ResetParticle(GameObject sceneObject) {
		if (sceneObject == null) {
			return;
		}

		ParticleSystem ps = sceneObject.GetComponent<ParticleSystem> ();
		if (ps == null) {
			return;
		}

		ps.Stop ();
		ps.Clear ();
		ps.Play ();
	}
}