using UnityEngine;
using System.Collections;

public class CharactorJumpAssert : MonoBehaviour {
	public float delay;
	/*
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	*/

	public void DoDelay() {
	}

	private IEnumerator __DoDelay() {
		yield return new WaitForSeconds (this.delay);
	}
}
