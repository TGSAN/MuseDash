using UnityEngine;
using System.Collections;
using GameLogic;
using FormulaBase;

public class ArmActionController : BaseSpineObjectController {
	private static ArmActionController instance = null;
	public static ArmActionController Instance {
		get {
			return instance;
		}

		set {
			instance = value;
		}
	}

	public override void SetIdx (int idx) {
		this.idx = idx;
	}

	public override void OnControllerStart () {
		this.ComeOutCheck ();
	}

	public override void OnControllerAttacked (int result, bool isDeaded) {
		if (!isDeaded) {
			return;
		}

		this.FireCheck ((uint)result);
	}

	public override bool ControllerMissCheck (int idx, decimal currentTick) {
		this.FireCheck (GameMusic.MISS);
		return true;
	}

	public override bool OnControllerMiss (int idx) {
		return true;
	}

	public override void Init () {
		// instance = this;
		this.gameObject.SetActive (false);
	}

	public void UseSkill() {
		if (SpineActionController.CurrentProtectLevel (this.gameObject) > 0) {
			return;
		}

		if (SpineActionController.CurrentProtectLevel (ArmActionController.instance.gameObject) > 0) {
			return;
		}

		if (BattlePetComponent.Instance.GetCurrentSkill () != null) {
			return;
		}

		float duration = BattlePetComponent.Instance.UseSkill ();
		if (duration > 0) {
			this.StartCoroutine (this.EndSkill (duration));
		}
		
		SpineActionController.Play (ACTION_KEYS.PET_SKILL, this.gameObject);
	}

	private void FireCheck(uint resultCode) {
		if (!this.gameObject.activeSelf) {
			return;
		}
		
		if (resultCode < GameMusic.PERFECT) {
			return;
		}
		
		this.FireSkill (resultCode);
	}

	private void ComeOutCheck() {
		if (this.gameObject.activeSelf) {
			return;
		}
		
		this.ComeOut ();
	}

	private void GoBackCheck() {
		if (!this.gameObject.activeSelf) {
			return;
		}
		
		this.GoBack ();
	}

	private void ComeOut() {
		SpineActionController.Play (ACTION_KEYS.COMEIN, this.gameObject);
		this.gameObject.SetActive (true);
	}

	private void GoBack() {
		BattlePetComponent.Instance.SwitchPet ();
		SpineActionController.Play (ACTION_KEYS.COMEOUT, this.gameObject);
	}

	private void FireSkill(uint resultCode) {
		BattlePetComponent.Instance.FireSkill (resultCode);
	}

	private IEnumerator EndSkill(float second) {
		yield return new WaitForSeconds (second);

		BattlePetComponent.Instance.EndSkill ();
		this.GoBackCheck ();
	}
}
