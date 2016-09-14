using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using GameLogic;
using FormulaBase;
using DYUnityLib;

public class GameKernel{
	public GameKernel(){
		InitGameKernel ();
	}

	private static GameKernel instance = null;
	public static GameKernel Instance {
		get {
			if (instance == null) {
				instance = new GameKernel ();
			}
			return instance;
		}
	}

	#region system value here

	private float[] rates = new float[4];
	private bool isUnderLongPress = false;
	private bool isLongPressFailed = false;

	public bool IsUnderLongPress {
		get {
			return this.isUnderLongPress;
		}

		set {
			this.isUnderLongPress = value;
		}
	}

	public bool IsLongPressFailed {
		get {
			return this.isLongPressFailed;
		}
		
		set {
			this.isLongPressFailed = value;
		}
	}

	//About Fever:
	private const float MAX_FEVER = 100;
	private float wholeFeverValue = 0;
	private bool isActivateFever;

	#endregion

	// call this func when enter a new stage
	public void InitGameKernel(){

		wholeFeverValue = 0;
		isActivateFever = false;
		isUnderLongPress = false;
		isLongPressFailed = false;

		for (int i = 0; i < rates.Length; i++) {
			rates [i] = 0;
		}
	}

	#region fever funcs
	public float GetFeverRate(){
		return wholeFeverValue / MAX_FEVER;
	}
	
	public float GetWholeFever(){
		return wholeFeverValue;
	}
	
	public void AddFever(float value) {
		// If is teach stage, fever is lock.
		if (StageTeachComponent.Instance.IsTeachingStage ()) {
			return;
		}

		if (wholeFeverValue + value <= MAX_FEVER) {
			if (wholeFeverValue + value < 0) {
				isActivateFever = false;
				wholeFeverValue = 0;
				BattleRoleAttributeComponent.Instance.FireSkill (SkillComponent.ON_FEVER_END);
				FeverEffectManager.Instance.CancelFeverEffect ();
			} else {
				wholeFeverValue += value;
			}
		} else {
			wholeFeverValue = MAX_FEVER;
			isActivateFever = true;
			BattleRoleAttributeComponent.Instance.FireSkill (SkillComponent.ON_FEVER);
			FeverEffectManager.Instance.ActivateFever ();
			SoundEffectComponent.Instance.SayByCurrentRole (GameGlobal.SOUND_TYPE_FEVER);
		}
	}
	
	public bool IsOnFeverState(){
		return isActivateFever;
	}
	#endregion
}
