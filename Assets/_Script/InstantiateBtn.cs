using UnityEngine;
using System.Collections;

public class InstantiateBtn : MonoBehaviour {

	public int stageAmount;
	public GameObject stage;

	void Start () {
		for (int i = 0; i < stageAmount; i++) {
			var goBtn = GameObject.Instantiate (stage);
			goBtn.transform.SetParent (transform);
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
