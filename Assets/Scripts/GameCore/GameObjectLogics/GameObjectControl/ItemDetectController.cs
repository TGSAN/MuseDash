using UnityEngine;
using System.Collections;

public class ItemDetectController : PhysicsMountBlockController {
	public bool enableConflict = false;

	public override void OnControllerStart ()
	{
		this.enableConflict = true;
		GirlCollisionDetectNodeController.RegForConllision (this);
	}
}
