using UnityEngine;
using System.Collections;

public class PendulumBobController : BaseEnemyObjectController {

	public override void OnControllerStart () {

	}

	
	public override void OnControllerAttacked (int result, bool isDeaded) {
	
	}
	
	public override bool OnControllerMiss (int idx) {
		return false;
	}


}
