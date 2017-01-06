using UnityEngine;
using System.Collections;

public class Spawner : MonoBehaviour {

	public GameObject[] path;
	public float delay = 2.0f;
	public bool active = true;
	public Vector2 delayRange = new Vector2 (1, 2);

	void ResetDelay(){
		delay = Random.Range (delayRange.x, delayRange.y);
	}

	// Use this for initialization
	void Start () {
		ResetDelay ();
		StartCoroutine (pathGenerator ());
	}

	IEnumerator pathGenerator(){
		yield return new WaitForSeconds (delay);

		if (active) {
			var newTransform = transform;
			Instantiate (path [Random.Range (0, path.Length)], newTransform.position, Quaternion.identity);
			ResetDelay ();
		}
		StartCoroutine (pathGenerator ());
	}
}
