using UnityEngine;
using System.Collections;

public class OkBox : MonoBehaviour {

	public UILabel m_label;

//	public delegate void ClickOk();
	Callback m_callBackFun=null;
	public void showBox(string _label,Callback _CallFun)
	{
		this.gameObject.SetActive(true);
		m_callBackFun=null;
		if(_label.Length>0)
			m_label.text=_label;
		else
		{
			m_label.text="功能正在完善中";
		}
		if(_CallFun!=null)
			m_callBackFun=_CallFun;
			
	}
	public void ClickOkbutton()
	{

		this.gameObject.SetActive(false);
		if(m_callBackFun!=null)
			m_callBackFun();
		this.gameObject.SetActive(false);
	}
}
