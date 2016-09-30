using UnityEngine;
using System.Collections;

public class StorePanel : UIPanelBase {
	public UIToggle m_FirstToggle;
	public UIToggle m_FirstToggle2;	
	public UIToggle m_FirstToggle3;	
	public UIToggle m_FirstToggle4;

	public GameObject temp1;
	public GameObject temp2;
	public GameObject temp3;
	public GameObject temp4;
	// Use this for initialization
	void OnEnable()
	{
		m_FirstToggle.Set(true);
		temp1.SetActive(true);
		temp2.SetActive(false);
		temp3.SetActive(false);
		temp4.SetActive(false);
		m_FirstToggle2.Set(false);
		m_FirstToggle3.Set(false);
		m_FirstToggle4.Set(false);
//		m_FirstToggle.value=true;
//		m_FirstToggle2.value=false;
//		m_FirstToggle3.value=false;
//		m_FirstToggle4.value=false;
	}
	void OnDisable()
	{
		m_FirstToggle.Set(true);
		//m_FirstToggle.gameObject.GetComponent<UIToggledObjects>().
		m_FirstToggle2.Set(false);
		m_FirstToggle3.Set(false);
		m_FirstToggle4.Set(false);
	}
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	public override void ShowPanel(int _state=1,FormulaBase.FormulaHost _host=null,int _Layer=-1)
	{
		if(_Layer!=-1)
		{
			SetPanelLayer(_Layer);
			temp1.GetComponent<UIPanel>().sortingOrder=_Layer*5+1;
			temp2.GetComponent<UIPanel>().sortingOrder=_Layer*5+1;
			temp3.GetComponent<UIPanel>().sortingOrder=_Layer*5+1;
		//	temp4.GetComponent<UIPanel>().sortingOrder=_Layer*5+1;
		}

		//Debug.LogError(" use Father fun");
	}
}
