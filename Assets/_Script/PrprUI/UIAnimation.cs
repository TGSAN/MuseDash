using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class UIAnimation : MonoBehaviour {

	public AnimationCurve moveIn;
	public AnimationCurve moveOut;
	public float duration = 0.3f;

	private int offset = 1080;

	IEnumerator Start () {

		gameObject.transform.Translate (0,-offset,0);
		
		yield return new WaitForSeconds(0.2f);

		transform.DOMoveY (offset, duration).SetEase (moveIn).SetRelative ();
	}
	
	// Update is called once per frame
	public void Out () {
		transform.DOMoveY (-offset, duration).SetEase (moveOut).SetRelative ();
	}
}
