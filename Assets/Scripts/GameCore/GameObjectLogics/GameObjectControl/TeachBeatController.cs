using UnityEngine;
using System.Collections;

public class TeachBeatController : MonoBehaviour {

	// Use this for initialization
	void Start () {
		FormulaBase.StageTeachComponent.Instance.RegBeatObj (this.gameObject);
	}
}
