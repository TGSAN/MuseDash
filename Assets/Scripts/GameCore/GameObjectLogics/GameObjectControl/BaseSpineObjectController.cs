using UnityEngine;
using System.Collections;

public abstract class BaseSpineObjectController : MonoBehaviour {
	public int idx = -1;
	public abstract void SetIdx (int idx);
	public abstract void Init();

	public abstract bool ControllerMissCheck(int idx, decimal currentTick);
	public abstract void OnControllerStart ();
	public abstract bool OnControllerMiss (int idx);
	public abstract void OnControllerAttacked (int result, bool isDeaded);

	public int GetIdx() {
		return this.idx;
	}
}