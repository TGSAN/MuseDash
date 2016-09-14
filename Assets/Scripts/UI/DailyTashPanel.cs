using UnityEngine;
using System.Collections;

public class DailyTashPanel : UIPanelBase {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	public override void ShowPanel(int _state=1,FormulaBase.FormulaHost _host=null,int _Layer=0)
	{
		Debug.Log("显示活跃面板");
		SetPanelLayer(_Layer);
		//Debug.LogError(" use Father fun");
	}
}
