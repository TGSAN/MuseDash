using UnityEngine;
using System.Collections;

public class TeachJumpController : MonoBehaviour {

	// Use this for initialization
	void Start () {
		FormulaBase.StageTeachComponent.Instance.RegJumpObj (this.gameObject);
	}
}
