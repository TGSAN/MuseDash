using UnityEngine;
using System.Collections;

public class UIOtherPanel : UIPanelBase {
	
	public override void ShowPanel(int _state=1,FormulaBase.FormulaHost _host=null,int _Layer=0)
	{
		this.gameObject.SetActive(true);
		SetPanelLayer(_Layer);
		//Debug.LogError(" use Father fun");
	}

	/// <summary>
	/// 每个界面自己的Back事件  Example：背包界面出售按钮显示Back
	/// </summary>
//	public override void PanelClickBack()
//	{
//		UIManageSystem.g_Instance.RomoveUI();
//	}
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
