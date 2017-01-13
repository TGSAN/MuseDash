using UnityEngine;
using System.Collections;

public class AttackFxController : MonoBehaviour {

	void OnEnable () {
		float angle = Random.Range (0f, 360f);
		transform.Rotate (Vector3.forward, angle);
	
	}

}
