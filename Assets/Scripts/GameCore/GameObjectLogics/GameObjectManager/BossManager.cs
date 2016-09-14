using UnityEngine;
using System.Collections;
using GameLogic;
using FormulaBase;

public class Boss {
	private static Boss instance = null;
	private GameObject bossObject = null;
	private string bossName = null;

	public static Boss Instance {
		get {
			if (instance == null) {
				instance = new Boss ();
			}
			return instance;
		}
	}
	
	public Boss(){
//		this.life = life;
//		this.attack = attack;
	}

	public GameObject GetGameObject() {
		return this.bossObject;
	}

	public string GetBossName() {
		return this.bossName;
	}

	public void SearchBoss() {
		GameObject bossLayer = GameObject.Find ("BossLayer");
		this.bossName = StageBattleComponent.Instance.GetBossName ();
		this.bossObject = StageBattleComponent.Instance.AddObj (ref this.bossName);
		if (this.bossObject == null) {
			Debug.Log ("No such boss " + this.bossName);
			return;
		}

		Debug.Log ("Load boss : " + this.bossName);
		this.bossObject.transform.SetParent (bossLayer.transform, false);

		SpineActionController sac = this.bossObject.GetComponent<SpineActionController> ();
		sac.Init (-1);
		this.bossObject.SetActive (true);
	}

 	public bool IsDead() {
		return false;
	}

	public void Play(string key) {
		SpineActionController.Play (key, this.bossObject);
	}

	public void ReinitMaterials () {
		HighlightingController hc = this.bossObject.GetComponent<HighlightingController> ();
		if (hc == null) {
			return;
		}

		hc.ReinitMaterials ();
	}


	public void PullBack() {
		if (this.bossObject == null) {
			return;
		}


	}
}