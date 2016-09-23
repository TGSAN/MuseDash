using UnityEngine;
using System.Collections;

public class UIMask : MonoBehaviour {
	private bool isWhiteToBlack;
	private float reduce;
	private UISprite m_Sprite;
	private Callback m_CallBack = null;

	public float time;

	void Start() {
		this.SetMask (false);
	}

	void Update() {
		if (this.m_Sprite == null) {
			return;
		}

		if (this.m_Sprite.alpha < 0) {
			return;
		}

		this.m_Sprite.alpha += this.reduce;
		if (this.isWhiteToBlack) {
			if (this.m_Sprite.alpha >= 1f) {
				this.m_Sprite.alpha = -1f;
				if (this.m_CallBack != null) {
					this.m_CallBack ();
					this.m_CallBack = null;
				}
			}
		} else {
			if (this.m_Sprite.alpha <= 0f) {
				this.m_Sprite.alpha = -1f;
				if (this.m_CallBack != null) {
					this.m_CallBack ();
					this.m_CallBack = null;
				}
			}
		}
	}

	private void InitSprite() {
		if (this.m_Sprite != null) {
			return;
		}

		//this.reduce = this.time / Application.targetFrameRate;
		this.m_Sprite = this.gameObject.GetComponent<UISprite> ();
	}

	/// <summary>
	/// Sets the mask black to white.
	/// </summary>
	/// <param name="_CallBack">Call.</param>
	/// <param name="Blur">If set to <c>true</c> 是否在毛玻璃前面.</param>
	public void SetMask(bool WhiteToBlack = true, Callback _CallBack = null, bool Blur=false) {
		this.InitSprite ();
		if (this.m_Sprite == null) {
			return;
		}

		this.m_CallBack = _CallBack;
		this.isWhiteToBlack = WhiteToBlack;
		if (this.isWhiteToBlack) {
			this.reduce = 0.02f;
			this.m_Sprite.alpha = 0f;
		} else {
			this.reduce = -0.02f;
			this.m_Sprite.alpha = 1f;
		}
	}

	public void MaskAnimationFinish() {
		if (this.m_CallBack != null) {
			this.m_CallBack ();
		}
	}

	public void Reset() {
		this.InitSprite ();
		this.m_Sprite.alpha = 1f;
	}
}
