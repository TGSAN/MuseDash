using UnityEngine;
using System.Collections;

public class PlayerControler : MonoBehaviour {

	private bool isRight = false;
	public Vector3 right = new Vector3 (1, 0, 0);
	public Vector3 left = new Vector3 (-1, 0, 0);
	private Vector3 direction = new Vector3 (0, 0, 0);

	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown (KeyCode.Space)) {
			direction = isRight ? left : right;
			isRight = !isRight;
		}
		transform.Translate (direction);
	}
}