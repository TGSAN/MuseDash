using UnityEngine;
using System.Collections;

public class UseResBox : MonoBehaviour {

	public UILabel m_Des;
	public UILabel m_RescastNumber;
	public UISprite m_ResIcon;

	Callback CancelFUn=null;
	Callback OKFun=null;

	bool afterOKDisable=true;
	/// <summary>
	/// Sets the data.
	/// </summary>
	/// <param name="_str">显示的内容</param>
	/// <param name="_ResNumber">需要资源的数量</param>
	/// <param name="type">资源类型</param>// 默认为0  表示钱 1 表示钻石
	/// <param name="_OKFun">OK fun.</param>
	/// <param name="_afterOKDisable">If set to <c>true</c> 点击OL后窗口是否可见disable.</param>
	/// <param name="_CancelFUn">Cancel F un.</param>
	public void SetData(string _str,int _ResNumber,int type=0,	Callback _OKFun=null,bool _afterOKDisable=true,Callback _CancelFUn=null)
	{
		//m_Des.text=_str;
		CommonPanel.GetInstance().SetBlurSub(null);
		m_RescastNumber.text=_ResNumber.ToString();
		switch(type)
		{
		case 0:
			m_ResIcon.spriteName="groove_sign_locked";
			break;
		case 1:
			m_ResIcon.spriteName="resourse_icon_crystal";
			break;
		}
		CancelFUn=_CancelFUn;
		OKFun=_OKFun;
		afterOKDisable=_afterOKDisable;
		this.gameObject.SetActive(true);
	}

	public void ClickOKbutton()
	{
		if(OKFun!=null)
		{
			OKFun();
			if(afterOKDisable)
				this.gameObject.SetActive(false);
		}
		else 
		{
			this.gameObject.SetActive(false);
		}
		CommonPanel.GetInstance().CloseBlur(null);
	}
	public void ClickCancelButton()
	{
		if(CancelFUn!=null)
		{
			CancelFUn();
		}
		CommonPanel.GetInstance().CloseBlur(null);
		this.gameObject.SetActive(false);
	}
	public void ClosBox()
	{
		this.gameObject.SetActive(false);
	}

}
