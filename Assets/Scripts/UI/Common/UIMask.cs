using UnityEngine;
using System.Collections;

public class UIMask : MonoBehaviour {
	Callback m_CallBack=null;
	public TweenAlpha m_Mask;

	void Start() {
		//this.m_Mask.enabled = false;
	}

	/// <summary>
	/// Sets the mask black to white.
	/// </summary>
	/// <param name="_CallBack">Call.</param>
	/// <param name="Blur">If set to <c>true</c> 是否在毛玻璃前面.</param>
	public void SetMask(bool WhiteToBlack=true,Callback _CallBack=null,bool Blur=false) {
		/*
		if (Blur) {
			this.gameObject.layer = 17;
		} else {
			this.gameObject.layer = 5;
		}
		*/
		//	this.gameObject.SetActive(true);
		this.m_CallBack = _CallBack;
		this.m_Mask.enabled = true;
		this.m_Mask.ResetToBeginning ();
		if (WhiteToBlack) {
			this.m_Mask.from = 0;
			this.m_Mask.to = 1f;
			//m_Mask.ResetToBeginning();
			//m_Mask.PlayForward();
		} else {
			this.m_Mask.from = 1f;
			this.m_Mask.to = 0;
			//m_Mask.PlayReverse();
		}

		this.m_Mask.Play ();
	}

	public void MaskAnimationFinish() {
		if (this.m_CallBack != null) {
			this.m_CallBack ();
		}
	}

	public void Reset() {
		this.m_CallBack = null;
		this.m_Mask.enabled = true;
		this.m_Mask.from = 1f;
		this.m_Mask.to = 0f;
		this.m_Mask.ResetToBeginning ();
		this.m_Mask.PlayForward ();
	}
}
