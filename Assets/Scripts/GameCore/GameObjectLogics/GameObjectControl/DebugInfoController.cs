using UnityEngine;
using System.Collections;
using GameLogic;
using Spine.Unity;

public class DebugInfoController : MonoBehaviour {
	private GameObject parentObject = null;
	// Use this for initialization
	void Start () {
		this.InitParentObject ();
		if (!GameGlobal.IS_NODE_DEBUG) {
			GameObject.Destroy (this.gameObject);
			return;
		}

		TextMesh tm = this.GetComponent<TextMesh> ();
		if (tm == null) {
			return;
		}

		SpineActionController sac = parentObject.GetComponent<SpineActionController> ();
		if (sac == null) {
			GameObject.Destroy (this.gameObject);
			return;
		}

		tm.text = sac.objController.GetIdx ().ToString ();
	}

	void InitParentObject() {
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
}