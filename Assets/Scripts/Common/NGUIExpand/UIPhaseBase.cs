using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FormulaBase;

public class UIPhaseBase : MonoBehaviour {
	private Animation _ani;
	private Animator _animator;
	private UIRootHelper _urh;

	[SerializeField]
	/// <summary>
	/// The show animation.
	/// 显示该界面的动画名
	/// </summary>
	public string ShowAnimation;

	/// <summary>
	/// The hide animation.
	/// 隐藏该界面的动画名
	/// </summary>
	public string HideAnimation;

	/// <summary>
	/// Raises the show event.
	/// 
	/// 重载方法，在Show完成显示操作后调用
	/// </summary>
	public virtual void OnShow (){}

	/// <summary>
	/// Raises the hide event.
	/// 
	/// 重载方法，在Hide完成隐藏操作后调用
	/// </summary>
	public virtual void OnHide (){}

	private void Init() {
		if (this._urh == null) {
			this._urh = this.gameObject.GetComponent<UIRootHelper> ();
		}

		if (this._ani == null) {
			this._ani = this.gameObject.GetComponent<Animation> ();
		}

		if (this._animator == null) {
			this._animator = this.gameObject.GetComponent<Animator> ();
		}
	}

	public void Show(string showAnimation = null) {
		this.Init ();
		if (this._urh == null) {
			return;
		}

		this.gameObject.SetActive (true);
		this._urh.AutoSetLabelHostForTable ();
		string _showAni = showAnimation;
		if (_showAni == null || _showAni == string.Empty) {
			_showAni = this.ShowAnimation;
		}

		if (_showAni == null || _showAni == string.Empty) {
			this.OnShow ();
			return;
		}

		if (this._ani == null && this._animator == null) {
			this.OnShow ();
			return;
		}

		if (this._animator != null) {
			this._animator.Stop ();
			this._animator.Rebind ();
			this._animator.Play (_showAni);

			AnimationClip ac = this.GetClipInAnimator (_showAni);
			if (ac == null) {
				this.gameObject.SetActive (true);
				this.OnShow ();
				return;
			}

			if (ac.events == null || ac.events.Length <= 0) {
				AnimationEvent ae = new AnimationEvent ();
				ae.time = ac.length;
				ae.functionName = "OnShow";
				ac.AddEvent (ae);
			}

			return;
		}

		if (this._ani != null) {
			AnimationClip ac = this._ani.GetClip (_showAni);
			if (ac == null) {
				this.OnShow ();
				return;
			}

			if (ac.events == null || ac.events.Length <= 0) {
				AnimationEvent ae = new AnimationEvent ();
				ae.time = ac.length;
				ae.functionName = "OnShow";
				ac.AddEvent (ae);
			}

			this._ani.Stop ();
			this._ani.Play (_showAni);
		}
	}

	public void Hide(string hideAnimation = null) {
		this.Init ();
		if (this._urh == null) {
			this.gameObject.SetActive (false);
			return;
		}

		string _hideAni = hideAnimation;
		if (_hideAni == null || _hideAni == string.Empty) {
			_hideAni = this.HideAnimation;
		}

		if (_hideAni == null || _hideAni == string.Empty) {
			this.OnHide ();
			this.gameObject.SetActive (false);
			return;
		}

		if (this._ani == null && this._animator == null) {
			this.OnHide ();
			this.gameObject.SetActive (false);
			return;
		}

		if (this._animator != null) {
			this._animator.Stop ();
			this._animator.Rebind ();
			this._animator.Play (_hideAni);

			AnimationClip ac = this.GetClipInAnimator (_hideAni);
			if (ac == null) {
				this.OnHide ();
				this.gameObject.SetActive (false);
				return;
			}

			if (ac.events == null || ac.events.Length <= 0) {
				AnimationEvent ae = new AnimationEvent ();
				ae.time = ac.length;
				ae.functionName = "OnHide";
				ac.AddEvent (ae);
			}

			return;
		}

		if (this._ani != null) {
			AnimationClip ac = this._ani.GetClip (_hideAni);
			if (ac == null) {
				this.OnHide ();
				return;
			}

			if (ac.events == null || ac.events.Length <= 0) {
				AnimationEvent ae = new AnimationEvent ();
				ae.time = ac.length;
				ae.functionName = "OnHide";
				ac.AddEvent (ae);
			}

			this._ani.Stop ();
			this._ani.Play (_hideAni);
		}
	}

	private AnimationClip GetClipInAnimator(string animateName) {
		if (this._animator == null) {
			return null;
		}

		AnimationClip ac = null;
		for (int i = 0; i < this._animator.layerCount; i++) {
			AnimatorClipInfo[] acInfos = this._animator.GetCurrentAnimatorClipInfo (i);
			if (acInfos == null || acInfos.Length <= 0) {
				return null;
			}

			foreach (AnimatorClipInfo _acInfo in acInfos) {
				if (_acInfo.clip.name == animateName) {
					ac = _acInfo.clip;
					break;
				}
			}

			if (ac != null) {
				return ac;
			}
		}

		return null;
	}
}
