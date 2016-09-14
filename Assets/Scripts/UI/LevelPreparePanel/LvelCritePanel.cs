using UnityEngine;
using System.Collections;

public class LvelCritePanel : MonoBehaviour {


	public UILabel m_label;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	public void ClickBack()
	{
		this.gameObject.SetActive(false);
	}

	public void SetText(string _str)
	{
		if(_str=="")
		{
			m_label.gameObject.SetActive(false);
		}
		m_label.gameObject.SetActive(true);
		m_label.text=_str;
	}
}
