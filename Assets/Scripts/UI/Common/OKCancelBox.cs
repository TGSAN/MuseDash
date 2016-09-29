using UnityEngine;
using System.Collections;

public class OKCancelBox : MonoBehaviour {


public delegate void CallBackFun();
	private CallBackFun m_OKFun;
	private CallBackFun m_CancelFun;

	public UIButton btnOk;
	public UIButton btnCancle;
	public UILabel m_label;

	public void ShowOkCancelBox(string _str="",CallBackFun _okCallBackFun=null,CallBackFun _CancelCallBackFun=null) {
		if (_str.Length > 0) {
			this.m_label.text = _str;
		} else {
			//Use diff str
			this.m_label.text = "";
			this.m_OKFun = _okCallBackFun;
			this.m_CancelFun = _CancelCallBackFun;
		}

		if (this.btnOk.onClick == null || this.btnOk.onClick.Count <= 0) {
			EventDelegate _dlgOk = new EventDelegate ();
			_dlgOk.Set (this, "ClickOkButton");
			this.btnOk.onClick.Add (_dlgOk);
		}

		if (this.btnCancle.onClick == null || this.btnCancle.onClick.Count <= 0) {
			EventDelegate _dlgCancle = new EventDelegate ();
			_dlgCancle.Set (this, "ClickCancelButton");
			this.btnCancle.onClick.Add (_dlgCancle);
		}

		this.gameObject.SetActive (true);
	}

	public void ClickOkButton() {
		if (this.m_OKFun != null)
			this.m_OKFun ();
		
		this.gameObject.SetActive (false);
	}

	public void ClickCancelButton() {
		if (this.m_CancelFun != null)
			this.m_OKFun ();
		
		this.gameObject.SetActive (false);
	}
}
