using UnityEngine;
using System.Collections;

public class OKCancelBox : MonoBehaviour {


public delegate void CallBackFun();
	CallBackFun m_OKFun;
	CallBackFun m_CancelFun;
	public UILabel m_label;
	public void ShowOkCancelBox(string _str="",CallBackFun _okCallBackFun=null,CallBackFun _CancelCallBackFun=null)
	{
		if(_str.Length>0)
		{
			m_label.text=_str;
		}
		else
		{
			//Use diff str
			m_label.text="";
			m_OKFun=_okCallBackFun;
			m_CancelFun=_CancelCallBackFun;
		}
		this.gameObject.SetActive(true);
	}


	public void ClickOkButton()
	{
		if(m_OKFun!=null)
			m_OKFun();
			this.gameObject.SetActive(false);

	}
	public void ClickCancelButton()
	{
		if(m_CancelFun!=null)
			m_OKFun();
			this.gameObject.SetActive(false);
	}
}
