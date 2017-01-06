using UnityEngine;
using System.Collections;

public class ColorChange : MonoBehaviour {
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown (KeyCode.R)) {
			gameObject.GetComponent<Renderer>().material.color = Color.red; 
		}
		if (Input.GetKeyDown (KeyCode.G)) {
			gameObject.GetComponent<Renderer>().material.color = Color.green;
		}
		if (Input.GetKeyDown (KeyCode.B)) {
			gameObject.GetComponent<Renderer>().material.color = Color.blue;
		}
	}
}
