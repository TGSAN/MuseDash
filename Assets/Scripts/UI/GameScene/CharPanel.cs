using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using GameLogic;
using System.Collections.Generic;
using FormulaBase;

public class CharPanel : MonoBehaviour {
	private Animator comboTextAnimator;

	private UILabel comboText;
	private Text socreText;

	private static CharPanel instance = null;
	private Transform scoreAnimLabelParent = null;

	private List<GameObject> ComboNumberShowPreloads = null;

	private bool isShowingCombo = false;

	[SerializeField]
	public GameObject ScoreObject;
	public UnityEngine.Object ComboNumberShow;
	public UILabel ComboTextNormal;
	public UILabel ComboTextHigh;
	public TweenAlpha missReactionBoard;

	public bool IsShowingCombo{
		set{
			isShowingCombo = value;
		}
		get{
			return isShowingCombo;
		}
	}


	public CharPanel() {
		instance = this;
	}

	public static CharPanel Instance {
		get {
			return instance;
		}
	}

	// Use this for initialization
	void Start () {
		int defaultNumberCount = 10;
		this.missReactionBoard.gameObject.SetActive (false);
		this.ComboTextNormal.gameObject.SetActive (false);
		this.ComboTextHigh.gameObject.SetActive (false);
		this.ComboNumberShowPreloads = new List<GameObject> ();
		for (int i = 0; i < defaultNumberCount; i++) {
			GameObject nObj = GameObject.Instantiate (this.ComboNumberShow) as GameObject;
			nObj.SetActive (false);
			this.ComboNumberShowPreloads.Add (nObj);
		}

		scoreAnimLabelParent = this.ScoreObject.transform;
		this.comboText = ComboTextNormal;
		this.comboTextAnimator = this.comboText.gameObject.GetComponent<Animator> ();
		this.comboTextAnimator.SetBool ("IfDisappear", true);
	}

	public void HideCombo() {
		this.comboText.enabled = false;
	}

	public void ShowCombo(int number, bool isPlayComboEffect = true) {
		this.ComboTextNormal.gameObject.SetActive (false);
		this.ComboTextHigh.gameObject.SetActive (false);

		if (number < EffectManager.COMBO_CHANGE_LIM) {
			this.comboText = this.ComboTextNormal;
		} else {
			this.comboText = this.ComboTextHigh;
		}

		this.comboText.gameObject.SetActive (true);
		this.comboTextAnimator = this.comboText.gameObject.GetComponent<Animator> ();
		this.comboText.enabled = true;
		this.comboTextAnimator.Rebind ();
		this.comboTextAnimator.Play ("NumberShow");
		this.comboTextAnimator.SetBool ("IfShow", true);
		this.comboText.text = "" + number;
		/*
		if (number < EffectManager.COMBO_CHANGE_LIM) {
			this.SetNumberColor (new Color (164 / 255f, 241 / 255f, 1f, 1f), new Color (0, 168 / 255f, 223 / 255f, 1f));
		} else {
			this.SetNumberColor(new Color(1f, 233/255f, 50/255f, 1f), new Color(254/255f, 149/255f, 0, 1f));
		}
*/
		isShowingCombo = true;

		if (number == GameGlobal.COMBO_INTERVAL || number % GameGlobal.COMBO_INTERVAL == 0) {
			SoundEffectComponent.Instance.SayByCurrentRole (GameGlobal.SOUND_TYPE_ON_TEN_COMBO);
		}
	}

	public void StopCombo() {
		if (this.ComboTextNormal.gameObject.activeSelf) {
			Animator ani = this.ComboTextNormal.gameObject.GetComponent<Animator> ();
			if (ani != null && !ani.GetBool ("IfDisappear")) {
				ani.Play ("NumberDisappear");
				ani.SetBool ("IfDisappear", true);
				ani.SetBool ("IfShow", false);
			}
		}

		if (this.ComboTextHigh.gameObject.activeSelf) {
			Animator ani = this.ComboTextHigh.gameObject.GetComponent<Animator> ();
			if (ani != null && !ani.GetBool ("IfDisappear")) {
				ani.Play ("NumberDisappear");
				ani.SetBool ("IfDisappear", true);
				ani.SetBool ("IfShow", false);
			}
		}
	}

	private GameObject GetUsefulNumberShow() {
		if (this.ComboNumberShowPreloads == null) {
			return null;
		}

		for (int i = 0; i < this.ComboNumberShowPreloads.Count; i++) {
			GameObject _obj = this.ComboNumberShowPreloads [i];
			if (_obj == null) {
				continue;
			}

			Animation _ani = _obj.GetComponent<Animation> ();
			if (_ani == null) {
				GameObject.Destroy (_obj);
				continue;
			}

			if (!_obj.activeSelf) {
				_obj.SetActive (true);
				return _obj;
			}


			if (!_ani.isPlaying) {
				return _obj;
			}
		}

		GameObject nObj = GameObject.Instantiate (this.ComboNumberShow) as GameObject;
		this.ComboNumberShowPreloads.Add (nObj);
		return nObj;
	}

	public void BeAttack() {
		if (this.missReactionBoard == null) {
			return;
		}

		this.missReactionBoard.gameObject.SetActive (true);
		this.missReactionBoard.ResetToBeginning ();
		this.missReactionBoard.Play ();
	}

	public void StopBeAttack() {
		if (this.missReactionBoard == null) {
			return;
		}

		this.missReactionBoard.gameObject.SetActive (false);
	}

	// 打击得分
	public void SetScore(uint result, int value) {
		var newNumber = this.GetUsefulNumberShow ();
		if (newNumber == null) {
			return;
		}

		var socreText = this.GetScoreText (newNumber);
		if (socreText == null) {
			return;
		}

		global::ComboNumberShow.FontType ft = global::ComboNumberShow.FontType.FONT_1;
		switch (result) {
		case GameMusic.COOL:
		case GameMusic.GREAT:
			ft = global::ComboNumberShow.FontType.FONT_1;
			break;
		case GameMusic.PERFECT:
			ft = global::ComboNumberShow.FontType.FONT_2;
			break;
		default:
			break;
		}

		FormulaBase.FormulaHost battleRole = FormulaBase.BattleRoleAttributeComponent.Instance.GetBattleRole ();
		if (battleRole.GetDynamicIntByKey (FormulaBase.SignKeys.CTR) > 0) {
			ft = global::ComboNumberShow.FontType.FONT_3;
		}

		socreText.SetText (value.ToString (), ft);
	}

	// 加血
	public void SetHpChange(int value) {
		var newNumber = this.GetUsefulNumberShow ();
		if (newNumber == null) {
			return;
		}

		var socreText = this.GetScoreText (newNumber);
		if (socreText == null) {
			return;
		}

		if (value > 0) {
			socreText.SetText ("+" + value, global::ComboNumberShow.FontType.FONT_5);
		} else {
			socreText.SetText (value.ToString (), global::ComboNumberShow.FontType.FONT_4);
		}
	}

	// 加分6
	public void SetHpScore(int value) {
		var newNumber = this.GetUsefulNumberShow ();
		if (newNumber == null) {
			return;
		}

		var socreText = this.GetScoreText (newNumber);
		if (socreText == null) {
			return;
		}

		socreText.SetText ("+" + value, global::ComboNumberShow.FontType.FONT_6);
	}

	// 加金币7
	public void SetGoldChange(int value) {
		var newNumber = this.GetUsefulNumberShow ();
		if (newNumber == null) {
			return;
		}

		var socreText = this.GetScoreText (newNumber);
		if (socreText == null) {
			return;
		}

		socreText.SetText ("+" + value, global::ComboNumberShow.FontType.FONT_7);
	}

	private ComboNumberShow GetScoreText(GameObject newNumber) {
		var socreText = newNumber.GetComponent<ComboNumberShow> ();
		if (this.scoreAnimLabelParent != null) {
			var tempLocal = newNumber.transform.localPosition;
			var tempScale = newNumber.transform.localScale; 
			newNumber.transform.SetParent (this.scoreAnimLabelParent);
			newNumber.transform.localPosition = Vector3.zero;
			newNumber.transform.localScale = tempScale;
		}

		return socreText;
	}
}
