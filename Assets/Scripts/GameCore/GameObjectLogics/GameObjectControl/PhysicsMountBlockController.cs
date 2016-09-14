using UnityEngine;
using System.Collections;
using GameLogic;

public class PhysicsMountBlockController : BaseSpineObjectController {
	private GameObject parentObject = null;

	public override void SetIdx (int idx){
	}

	public override void Init () {
	}

	public override void OnControllerAttacked (int result, bool isDeaded) {
	}

	public override void OnControllerStart () {
	}

	public override bool ControllerMissCheck (int idx, decimal currentTick) {
		return true;
	}

	public override bool OnControllerMiss (int idx) {
		return false;
	}

	// Use this for initialization
	void Start () {
		this.InitParentObject ();
		if (this.GetRect () == null) {
			Debug.LogError ("PhysicsMountBlockController has no RectTransform.");
		}
//		Vector2 v = hero.GetRect().position;
//		if (gold.ContainsPoint(v))
	}

//	void Destory() {
//	}

	/*
	void Update() {
		if (this.parentObject == null) {
			return;
		}

		if (!this.parentObject.activeSelf) {
			return;
		}

		SpineActionController sap = this.parentObject.GetComponent<SpineActionController> ();
		if (sap != null) {
			Debug.Log (sap.objController.idx + " : " + " ----> " + this.GetRect().position.x);
		}
	}
	*/

	public void InitParentObject() {
		BoneFollower boneFollow = this.gameObject.GetComponent<BoneFollower> ();
		if (boneFollow == null) {
			GameObject.Destroy (this.gameObject);
			return;
		}
		
		this.parentObject = boneFollow.skeletonRenderer.gameObject;
		if (this.parentObject == null) {
			GameObject.Destroy (this.gameObject);
			return;
		}
	}

	public RectTransform GetRect() {
		RectTransform rt = this.gameObject.GetComponent<RectTransform> ();
		return rt;
	}

	public GameObject GetParentObject() {
		return this.parentObject;
	}

	/// <summary>
	/// Containses the point.
	/// 
	/// Check point in rect, point must be part of mount in spine.
	/// </summary>
	/// <returns><c>true</c>, if point was containsed, <c>false</c> otherwise.</returns>
	/// <param name="point">Point.</param>
	public bool ContainsPoint(Vector2 point) {
		RectTransform rt = this.GetRect ();
		if (rt == null) {
			return false;
		}

		return rt.rect.Contains (point);
	}
}