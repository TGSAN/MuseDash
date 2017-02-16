using UnityEngine;
using System.Collections;

public class MyDragScrowView : UIDragScrollView {

	//public MainPanetPanel m_MainPlanet;



	void OnDrag (Vector2 delta)
	{
		if (scrollView && NGUITools.GetActive(this))
		{

		//	Debug.Log("scrollView not null");

			//m_MainPlanet.MovePlanet(delta);
			//m_MainPlanet.GetComponent<MainPanetPanel>().MovePlanet();
			scrollView.Drag();
		}
	}
	void Start()
	{
		
//		StartCoroutine("InitScrollView");

	}
	void Update()
	{
	//	m_MainPlanet.GetComponent<MainPanetPanel>().MovePlanet();
	}

	//void 
}
