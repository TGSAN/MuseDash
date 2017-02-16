using UnityEngine;
using System.Collections;
using DYUnityLib;
using GameLogic;
using FormulaBase;

public class FreeController: BaseEnemyObjectController {
	public override void OnControllerStart () {
	}

	public override void OnControllerAttacked (int result, bool isDeaded) {
	}

	public override bool OnControllerMiss (int idx) {
		return false;
	}
}