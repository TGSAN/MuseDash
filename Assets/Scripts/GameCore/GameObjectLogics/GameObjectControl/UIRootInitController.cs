using UnityEngine;
using System.Collections;
using GameLogic;

public class UIRootInitController : MonoBehaviour {

	// Use this for initialization
	void Start () {
		UICamera uc = this.gameObject.GetComponent<UICamera> ();
		GameGlobal.gTouch.RegUICamera (uc);
	}
}
