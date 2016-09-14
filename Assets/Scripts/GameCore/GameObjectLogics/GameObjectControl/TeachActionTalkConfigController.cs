using UnityEngine;
using System.Collections;
using FormulaBase;

[System.Serializable]
public struct TeachActionTalkStruct {
	public int[] AttackTalks;
	public int[] MissTalks;
}

public class TeachActionTalkConfigController : MonoBehaviour {
	private int talkIdx;
	private int attackIdx;
	private int missIdx;
	private static TeachActionTalkConfigController instance = null;
	public static TeachActionTalkConfigController Instance {
		get {
			return instance;
		}
	}

	[SerializeField]
	public TeachActionTalkStruct[] talks;

	void Start () {
		this.talkIdx = 0;
		this.attackIdx = 0;
		this.missIdx = 0;
		instance = this;
		StageTeachComponent.Instance.SetIsTeachingStage (true);
	}

	public void OnAttacked() {
		if (this.talkIdx >= this.talks.Length) {
			Debug.Log ("TeachActionTalkConfigController OnAttacked with over talk " + this.talkIdx);
			return;
		}

		TeachActionTalkStruct data = this.talks [this.talkIdx];
		this.Talk (data.AttackTalks [this.attackIdx]);

		this.attackIdx += 1;
		if (this.attackIdx >= data.AttackTalks.Length) {
			this.attackIdx = data.AttackTalks.Length - 1;
		}
		/*
		// If all attack talk is ok, then next talk data.
		if (this.attackIdx >= data.AttackTalks.Length) {
			this.attackIdx = 0;
			this.talkIdx += 1;
		}
		*/
	}

	public void OnMissed() {
		if (this.talkIdx >= this.talks.Length) {
			Debug.Log ("TeachActionTalkConfigController OnMissed with over talk " + this.talkIdx);
			return;
		}

		TeachActionTalkStruct data = this.talks [this.talkIdx];
		this.Talk (data.MissTalks [this.missIdx]);

		this.missIdx += 1;
		if (this.missIdx >= data.MissTalks.Length) {
			this.missIdx = data.MissTalks.Length - 1;
		}
	}

	public void NextTalk() {
		this.attackIdx = 0;
		this.missIdx = 0;
		this.talkIdx += 1;
	}

	private void Talk(int talkId = 0) {
		string message = ConfigPool.Instance.GetConfigValue ("talk", talkId.ToString (), "message").ToString ();
		CommonPanel.GetInstance ().ShowText (message);
	}
}