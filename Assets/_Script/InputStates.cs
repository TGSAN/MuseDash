using UnityEngine;
using System.Collections;

public class InpupStates : MonoBehaviour {

	public bool actionButtonL = false;
	public bool actionButtonR = false;
	public bool actionButtonM = false;

	void Update () {
		
		//KeyboardControler
		if (Input.GetKeyDown (KeyCode.F)||Input.GetButtonDown("ButtonL")) {
			actionButtonL = true;
		}
		if (Input.GetKeyDown (KeyCode.J)||Input.GetButtonDown("ButtonR")) {
			actionButtonR = true;
		}
		if (Input.GetKeyDown (KeyCode.Space)||Input.GetButtonDown("ButtonM")) {
			actionButtonM = true;
		}
	}
}
