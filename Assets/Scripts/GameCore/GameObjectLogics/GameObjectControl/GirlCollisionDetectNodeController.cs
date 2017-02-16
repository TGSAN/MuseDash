using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DYUnityLib;
using GameLogic;
using FormulaBase;

public class GirlCollisionDetectNodeController : PhysicsMountBlockController {
	private static List<ItemDetectController> checkTransforms = null;

	private const int fixResutl = (int)GameMusic.COOL;

	public void DisableDetect(){
		this.enabled = false;
	}

	public void EnableDetect(){
		this.enabled = true;
	}

	public static void RegForConllision(ItemDetectController itemDectecController) {
		if (checkTransforms == null) {
			checkTransforms = new List<ItemDetectController> ();
		}

		checkTransforms.Add (itemDectecController);
	}

	void Update() {
		if (checkTransforms == null || checkTransforms.Count <= 0) {
			return;
		}

		for (int i = 0; i < checkTransforms.Count; i++) {
			ItemDetectController itemDectecController = checkTransforms [i];
			if (itemDectecController == null || !itemDectecController.enableConflict) {
				continue;
			}

			this.CollisionDetect (itemDectecController);
		}
	}

	private void CollisionDetect(ItemDetectController itemDetectController) {
		var rectTransForm1 = itemDetectController.GetRect ();
		if (rectTransForm1 == null || rectTransForm1.transform == null || rectTransForm1.position == Vector3.zero) {
			return;
		}
		
		// item detect girl hit
		var isCollided = false;
		var rectTransForm2 = this.GetRect ();
		var minX = Mathf.Max (rectTransForm1.offsetMin.x, rectTransForm2.offsetMin.x);
		var minY = Mathf.Max (rectTransForm1.offsetMin.y, rectTransForm2.offsetMin.y);
		var maxX = Mathf.Min (rectTransForm1.offsetMax.x, rectTransForm2.offsetMax.x);
		var maxY = Mathf.Min (rectTransForm1.offsetMax.y, rectTransForm2.offsetMax.y);
		
		if (maxX >= minX && maxY >= minY) {
			isCollided = true;
		}
		
		if (!isCollided) {
			return;
		}

		GameObject parentObj = itemDetectController.GetParentObject ();
		if (parentObj == null) {
			return;
		}

		var controller = parentObj.GetComponent<BaseEnemyObjectController> ();
		if (controller == null) {
			return;
		}

		controller.OnControllerAttacked (fixResutl, true);
	}
}