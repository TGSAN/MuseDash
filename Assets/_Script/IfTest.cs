using UnityEngine;
using System.Collections;

public class IfTest : MonoBehaviour {

	public int days = 0;
	public bool toggle=false;

	// Use this for initialization
	void Start () {
		while (toggle = true) {
			days += 1;
			Debug.Log (days);
		}
	}
	
	// Update is called once per frame
	void Update () {
//		if (toggle = true) {
//			days += 1;
//			Debug.Log (days);
//		}


	}
}
