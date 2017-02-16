//EDIT: SF 2016/6/30
//UI panel的基类
using UnityEngine;
using System.Collections;
using FormulaBase;
public class UIPanelBase : MonoBehaviour {


	#region 测试功能
	/// <summary>
	/// 显示UI 的测试
	/// </summary>
	public void ShowUITest()
	{
		this.gameObject.SetActive(true);
		Debug.Log("显示UI ");
	}
	/// <summary>
	/// Sets the DAT.
	/// </summary>
	/// <param name="_Stateid">Stateid.</param>
	public virtual void SetDATA(int _Stateid=0)
	{
		
	}


	#endregion
	public virtual void ShowPanel(int _state=1,FormulaHost _host=null,int _Layer=-1)
	{
		Debug.LogError(" use Father fun");

		SetPanelLayer(_Layer);
		//this.gameObject.GetComponent<UIPanel>().depth;
		//this.gameObject.GetComponent<UIPanel>().sortingOrder;
	}

	public void SetPanelLayer(int _layer,int _add=0)
	{
		if(_layer==-1)
			return ;
		this.gameObject.GetComponent<UIPanel>().depth=_layer*5+_add;
		this.gameObject.GetComponent<UIPanel>().sortingOrder=_layer*5+_add;
	}
	public void SetPanelState(int _state)
	{
		ShowPanel(_state);
	}

	/// <summary>
	/// 每个界面自己的Back事件  Example：背包界面出售按钮显示Back
	/// </summary>
	public virtual void PanelClickBack()
	{
		UIManageSystem.g_Instance.RomoveUI();
	}
	/// <summary>
	/// 播放开场动画
	/// </summary>
	public virtual void PlayInAnimation()
	{
		Debug.LogError(" use Father fun");
	}
	/// <summary>
	/// 播放退场动画
	/// </summary>
	public virtual void PlayOutAnimation()
	{
		Debug.LogError(" use Father fun");
	}
	/// <summary>
	/// 退场动画完成
	/// </summary>
	public virtual void OutAniMationFinish()
	{
		Debug.LogError(" use Father fun");
	}
}
