using UnityEngine;
using System.Collections;
using GameLogic;

public class ObstacleController: BaseEnemyObjectController {
	public override void OnAttackDestory () {
		if (!this.IsEmptyNode ()) {
			return;
		}
		
		GameObject.Destroy (this.gameObject);
	}

	public override void OnControllerStart () {
		base.OnControllerStart ();
	}

	public override void OnControllerAttacked (int result, bool isDeaded) {
	}
}