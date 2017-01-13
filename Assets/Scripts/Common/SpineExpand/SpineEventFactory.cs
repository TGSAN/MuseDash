using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GameLogic;
using System.Reflection.Emit;

public class SpineEventFactory : MonoBehaviour {
	private int idx;
	private static Type[] TYPE_POLL;
	private DoNothing[] eventObjects;
	private static string GAME_SPACE = "GameLogic.";
	private static System.Reflection.Assembly assembly = System.Reflection.Assembly.Load("Assembly-CSharp");
	// Use this for initialization
	void Start () {
		this.Init ();
	}

	public void Init() {
		if (TYPE_POLL == null || TYPE_POLL.Length == 0) {
			TYPE_POLL = new Type[EditorData.Instance.SpineEventName.Length];
			for (int i = 0; i < EditorData.Instance.SpineEventName.Length; i++) {
				string moduleName = EditorData.Instance.SpineEventName [i];
				TYPE_POLL [i] = assembly.GetType (GAME_SPACE + moduleName);
			}
		}

		if (this.eventObjects != null && this.eventObjects.Length > 0) {
			return;
		}

		this.eventObjects = new DoNothing[EditorData.Instance.SpineEventName.Length];
		for (int i = 0; i < TYPE_POLL.Length; i++) {
			Type moduleType = TYPE_POLL [i];
			if (moduleType == null) {
				continue;
			}

			DoNothing dnObj = (DoNothing)moduleType.Assembly.CreateInstance (moduleType.ToString ());
			dnObj.SetIdx (this.idx);
			dnObj.SetGameObject (this.gameObject);
			this.eventObjects [i] = dnObj;
		}
	}

	public void SetIdx(int idx) {
		this.idx = idx;
		this.Init ();
		for (int j = 0; j < this.eventObjects.Length; j++) {
			DoNothing _dnObj = this.eventObjects [j];
			if (_dnObj == null) {
				continue;
			}

			_dnObj.SetIdx (this.idx);
		}
	}

	public int DataCount() {
		if (this.eventObjects == null) {
			return 0;
		}
		
		return this.eventObjects.Length;
	}

	public Spine.AnimationState.CompleteDelegate GetFunc(int idx) {
		if (this.eventObjects == null || idx >= this.eventObjects.Length) {
			return null;
		}

		return this.eventObjects [idx].Do;
	}

	public static Spine.AnimationState.CompleteDelegate GetFunction(GameObject obj, int idx) {
		if (obj == null) {
			return null;
		}

		SpineEventFactory sef = obj.GetComponent<SpineEventFactory> ();
		if (sef == null) {
			return null;
		}

		return sef.GetFunc (idx);
	}
}