using UnityEngine;
using System.Collections;
using GameLogic;

public class ActerChangeColore : MonoBehaviour {
	private const float FADE_STEP_FIX = 30f;

	private float _dymFixStep = 0f;
	private float _fadeStep = -FADE_STEP_FIX;
	private float _ColorValue = 0f;
	private Coroutine hurtEffectCoroutine = null;

	public Material _HandleMat = null;
	public bool isChangeColor = false;
	public float fadeOutLimite = 1f;

	private static ActerChangeColore instance = null;
	public static ActerChangeColore Instance {
		get {
			return instance;
		}
	}

	// Use this for initialization
	void Start () {
		instance = this;
		this._ColorValue = 0f;
		this.isChangeColor = false;
		this._dymFixStep = this.fadeOutLimite / FADE_STEP_FIX;
		this._HandleMat.SetFloat ("_TextureFade", 0f);
		this._HandleMat.SetFloat ("_TextureColor", 0f);
		this._HandleMat.SetColor ("_Color", Color.white);
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		if (!this.isChangeColor) {
			return;
		}

		this._HandleMat.SetFloat ("_TextureColor", this._ColorValue);
		this._ColorValue += this._fadeStep;
		if (this._ColorValue < 0) {
			this._fadeStep = this._dymFixStep;
			this._ColorValue = 0f;
		}

		if (this._ColorValue > this.fadeOutLimite) {
			this._fadeStep = -this._dymFixStep;
			this._ColorValue = this.fadeOutLimite;
		}
	}

	/// <summary>
	/// 播放动画
	/// </summary>
	public void PlayAnimation() {
		this.isChangeColor = true;
		this._fadeStep = -this._dymFixStep;
		this._ColorValue = this.fadeOutLimite;
		this._HandleMat.SetFloat ("_TextureColor", this.fadeOutLimite);
		this._HandleMat.SetColor ("_Color", Color.red);

		if (this.hurtEffectCoroutine != null) {
			this.StopCoroutine (this.hurtEffectCoroutine);
		}

		this.hurtEffectCoroutine = this.StartCoroutine (this.AfterHurtEffect ());
	}

	public float GetHurtAniTime() {
		return 0.2f;
	}

	/// <summary>
	/// 停止动画
	/// </summary>
	public void StopAnimation() {
		this.isChangeColor = false;
		this._HandleMat.SetFloat ("_TextureColor", 0f);
		if (this.hurtEffectCoroutine != null) {
			this.StopCoroutine (this.hurtEffectCoroutine);
		}
	}

	/// <summary>
	/// 播放无敌动画 
	/// </summary>
	public void PlayWuDiAnimation(bool Loop=false,float _LoopTime=0f) {
		this.isChangeColor = true;
		this._fadeStep = -this._dymFixStep;
		this._ColorValue = this.fadeOutLimite;
		this._HandleMat.SetFloat ("_TextureColor", this.fadeOutLimite);
		this._HandleMat.SetColor ("_Color", Color.white);
	}

	private IEnumerator AfterHurtEffect() {
		yield return new WaitForSeconds (this.GetHurtAniTime ());

		this.PlayWuDiAnimation (true, (float)GameMusic.MISS_AVOID_TIME);
	}
}
