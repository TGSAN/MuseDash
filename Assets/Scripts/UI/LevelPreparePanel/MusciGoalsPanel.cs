using UnityEngine;
using System.Collections;

public class MusciGoalsPanel : MonoBehaviour {

	public 	GameObject m_panel;
	// Use this for initialization
	void Start () {
	
	}

	void OnEnable()
	{
		m_panel.SetActive(false);
	}

	void OnDisable()
	{
		m_panel.SetActive(true);
		
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
