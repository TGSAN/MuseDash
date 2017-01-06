using UnityEngine;
using System.Collections;

public class AnimatorController : MonoBehaviour {

	public Animator animator;

	void Start () {
		animator = GetComponent<Animator>();
	}

	void Update () {
		if (Input.GetKeyDown (KeyCode.A)) {
			animator.Play ("Spin", -1, 0);
		}

		if (Input.GetKeyDown (KeyCode.S)) {
			animator.Play ("Move", -1, 0f);
		}

		if (Input.GetKey (KeyCode.D)) {
			animator.Play ("Idle", -1, 0f);
		}
	}
}