using UnityEngine;
using System.Collections;
using GameLogic;
using FormulaBase;
using DYUnityLib;

/// <summary>
/// Mimic enemy controller.
/// 
/// 宝箱怪
/// </summary>
public class MimicEnemyController: BaseEnemyObjectController {
	private const float THROW_ITEM_DELAY = 0.01f;
	private FormulaHost throwItem = null;
	void Update() {
		if (this.throwItem == null) {
			return;
		}

		float t = this.throwItem.GetDynamicDataByKey (SignKeys.BATTLE_HP);
		if (t > 0) {
			this.throwItem.SetDynamicData (SignKeys.BATTLE_HP, t - FixUpdateTimer.fInterval);
			return;
		}

		this.ThrowItem ();
	}

	public override void OnControllerStart () {
		base.OnControllerStart ();
		Animator ani = this.gameObject.GetComponent<Animator> ();
		if (ani != null) {
			ani.StartRecording (0);
		}

		this.throwItem = MimicParentController.Instance.GetNextItem ();
	}

	/// <summary>
	/// Raises the controller attacked event.
	/// 
	/// On catched Mimic.
	/// </summary>
	/// <param name="result">Result.</param>
	/// <param name="isDeaded">If set to <c>true</c> is deaded.</param>
	public override void OnControllerAttacked (int result, bool isDeaded) {
		this.Catched ();
	}

	/// <summary>
	/// Raises the controller miss event.
	/// 
	/// On mimic excape.
	/// </summary>
	/// <param name="idx">Index.</param>
	public override bool OnControllerMiss (int idx) {
		this.Excaped ();
		return false;
	}

	/// <summary>
	/// Throws the item.
	/// 
	/// Mimic throw gold, score, hp-bag and so on.
	/// </summary>
	private void ThrowItem() {
		if (this.throwItem == null) {
			return;
		}

		SpineActionController.Play (ACTION_KEYS.SUMMON1, this.gameObject);
		GameObject obj = (GameObject)this.throwItem.GetDynamicObjByKey (SignKeys.GAME_OBJECT);
		StartCoroutine (this.OnThrowItem (obj));
		Animator ani = this.gameObject.GetComponent<Animator> ();
		if (ani != null) {
			ani.speed = 1;
		}

		this.throwItem = MimicParentController.Instance.GetNextItem ();
	}

	private IEnumerator OnThrowItem(GameObject obj) {
		yield return new WaitForSeconds (THROW_ITEM_DELAY);

		// Choose item and run it.
		if (obj != null) {
			obj.SetActive (true);
			BaseEnemyObjectController controller = obj.GetComponent<BaseEnemyObjectController> ();
			if (controller != null) {
				controller.OnControllerStart ();
			}
		}
	}

	/// <summary>
	/// Catched this instance.
	/// 
	/// On mimic comein finished, be catched.
	/// </summary>
	private void Catched() {
		if (this.gameObject == null) {
			return;
		}

		this.StopPhysicBlock ();
		SpineActionController.Play (ACTION_KEYS.CHAR_DEAD, this.gameObject);
	}

	/// <summary>
	/// Excaped this instance.
	/// 
	/// After play die, mimic will excape with no come back.
	/// </summary>
	private void Excaped() {
		if (this.gameObject == null) {
			return;
		}

		this.StopPhysicBlock ();
		SpineActionController.Play (ACTION_KEYS.COMEOUT, this.gameObject);

		// Play back comein unity animation.
		Animator ani = this.gameObject.GetComponent<Animator> ();
		if (ani != null) {
			ani.StopRecording ();
			ani.StartPlayback ();
			ani.speed = -1;

			StartCoroutine (this.ExcapeEnded ((float)ani.GetTime ()));
		}
	}

	private IEnumerator ExcapeEnded(float t) {
		yield return new WaitForSeconds (t);

		Animator ani = this.gameObject.GetComponent<Animator> ();
		if (ani != null) {
			ani.StopPlayback ();
		}

		GameObject.Destroy (this.gameObject);
	}

	private void StopPhysicBlock() {
		if (this.gameObject == null) {
			return;
		}

		SpineMountController smc = this.gameObject.GetComponent<SpineMountController> ();
		if (smc != null) {
			GameObject mountObj = smc.GetMountObjectByIdx (0);
			if (mountObj != null) {
				mountObj.SetActive (false);
			}
		}
	}
}