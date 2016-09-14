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

	public IEnumerator DoDelay() {
		yield return new WaitForSeconds (this.delay);
	}
}
