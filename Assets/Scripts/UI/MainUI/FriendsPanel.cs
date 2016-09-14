using UnityEngine;
using System.Collections;

public class FriendsPanel : MonoBehaviour {


	public CommonPanel m_CommonPanel;
	// Use this for initialization
	void Start () {
	//	m_CommonPanel=gameObject.transform.Find("")
	
	}


	public void ClickAword()
	{
		m_CommonPanel.ShowOkBox();
	}
	public void ClickAddFriend()
	{
		m_CommonPanel.ShowOkBox();
	}
	// Update is called once per frame
	void Update () {
	
	}
}
