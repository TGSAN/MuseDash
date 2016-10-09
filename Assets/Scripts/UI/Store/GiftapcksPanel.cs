using UnityEngine;
using System.Collections;

public class GiftapcksPanel : MonoBehaviour {


	public UIPlayTween m_PlayTween;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnEnable()
	{
		m_PlayTween.Play(true);
	}
}
