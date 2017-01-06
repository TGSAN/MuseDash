using UnityEngine;
using System.Collections;

public class SetSpriteColor : MonoBehaviour {

	public Color[] colorlist;
	private Color color;

	void Start () {
		color = colorlist [0];
	}

	void Update () {
		if (Input.GetKeyDown (KeyCode.Space)) {
			if (color == colorlist [0]) {
				color = colorlist [1];
			} 

			else if (color == colorlist [1]) {
				color = colorlist [2];
			}

			else if (color == colorlist [2]) {
				color = colorlist [3];
			}

			else if (color == colorlist [3]) {
				color = colorlist [4];
			}

			else if (color == colorlist [4]) {
				color = colorlist [5];
			}

			else if (color == colorlist [5]) {
				color = colorlist [6];
			}

			else if (color == colorlist [6]) {
				color = colorlist [0];
			}

			GetComponent<SpriteRenderer> ().color = color;
		}
	}
}
