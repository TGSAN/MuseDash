using UnityEngine;
using System.Collections;

public class Move : MonoBehaviour {

	public float movespeed = -1.0f;

	// Update is called once per frame
	void Update () {
		transform.Translate (0, movespeed * Time.deltaTime, 0);
	}
}
