using UnityEngine;
using System.Collections;
using GameLogic;

public class CharactorJumpFollow : MonoBehaviour {
	private static CharactorJumpFollow instance = null;
	public static CharactorJumpFollow Instance {
		get {
			return instance;
		}
	}

	private float _offset;
	// Use this for initialization
	void OnEnable() {
		instance = this;
		Vector3 pos = GirlManager.Instance.GetCurrentGirlPositon ();
		this._offset = pos.y - this.gameObject.transform.position.y;
	}
	
	// Update is called once per frame
	void Update () {
		Vector3 pos = GirlManager.Instance.GetCurrentGirlPositon ();
		Vector3 fPos = this.gameObject.transform.position;
		this.gameObject.transform.position = new Vector3 (fPos.x, pos.y, fPos.z);
	}
}
