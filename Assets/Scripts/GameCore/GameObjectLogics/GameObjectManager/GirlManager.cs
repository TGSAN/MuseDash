using UnityEngine;
using System.Collections;
using GameLogic;
using FormulaBase;
using DYUnityLib;

public class GirlManager : MonoBehaviour {
	private const string GILR_PATH = "char/";
	private const string ARM_PATH = "servant/";
	private const decimal COMEOUT_HARD_TIME = 2m;
	private static GirlManager instacne = null;
	private GameObject[] girls;
	private GameObject[] arms;
	// private Coroutine jumpCoroutine;
	private float autoReduceEnergyDuringTime = 0f;
	private bool hasGameBeenStarted = false;
	private bool isJumpingAction = false;

	public GameObject[] Girls {
		get {
			return this.girls;
		}
	}

	public GameObject[] Arms {
		get {
			return this.arms;
		}
	}

	[SerializeField]
	public string[] girlnames;
	public string[] armnames;

	void Start() {
		instacne = this;
	}

	private void ReduceCharLifePerFixedTime() {
		if (StageTeachComponent.Instance.IsTeachingStage ()) {
			return;
		}

		if (GameKernel.Instance.IsOnFeverState ()) {
			return;
		}

		if (!GameGlobal.gGameMusic.IsRunning ()) {
			return;
		}

		if (this.autoReduceEnergyDuringTime < GameGlobal.REDUCE_ENERGY_TIME) {
			this.autoReduceEnergyDuringTime += Time.fixedDeltaTime;
			return;
		}

		this.autoReduceEnergyDuringTime = 0f;
		BattleRoleAttributeComponent.Instance.AddHp (-1, false);
	}

	private void ReduceFeverEnergyPerFixedTime() {
		var reduceValue = Time.deltaTime / GameGlobal.FEVER_LAST_TIME * -100;
		GameKernel.Instance.AddFever (reduceValue);
		FightMenuPanel.Instance.SetFerver (GameKernel.Instance.GetFeverRate ());
	}

	public void StartAutoReduceEnergy(){
		this.hasGameBeenStarted = true;
	}

	public void StopAutoReduceEnergy(){
		this.hasGameBeenStarted = false;
	}

	void Update(){
		if (Time.timeScale <= 0) {
			return;
		}

		if (FixUpdateTimer.IsPausing ()) {
			return;
		}

		if (this.hasGameBeenStarted) {
			this.ReduceCharLifePerFixedTime ();
		}

		if (GameKernel.Instance.IsOnFeverState ()) {
			this.ReduceFeverEnergyPerFixedTime ();
		}
	}

	public static GirlManager Instance {
		get {
			return instacne;
		}
	}

	public static string GetCharactPath(string name) {
		return GILR_PATH + name + "/prefabs/";
	}

	public static string GetPetPath(string name) {
		return ARM_PATH + name + "/prefabs/";
	}


	public void Reset() {
		BattlePetComponent.Instance.Init ();
		this.isJumpingAction = false;
		//FightMenuPanel.Instance.SetFerver (0);
		this.girls = new GameObject[this.girlnames.Length];

		int heroIndex = RoleManageComponent.Instance.GetFightGirlIndex ();
		if (heroIndex < 0) {
			heroIndex = 0;
#if UNITY_EDITOR || UNITY_EDITOR_OSX || UNITY_EDITOR_64
			heroIndex = AdminData.Instance.DefaultRoleIdx;
#endif
		}

		Debug.Log ("Battle with hero : " + heroIndex);
		FormulaHost hero = RoleManageComponent.Instance.GetRole (heroIndex);
		int clothIdx = 0;
		string clothPath = null;
		if (hero != null) {
			clothIdx = hero.GetDynamicIntByKey (SignKeys.CLOTH);
			//clothPath = ConfigPool.Instance.GetConfigStringValue ("clothing", clothIdx.ToString (), "path");
			clothPath = ConfigPool.Instance.GetConfigStringValue ("clothing", "uid", "path", clothIdx);
		}

#if UNITY_EDITOR || UNITY_EDITOR_OSX || UNITY_EDITOR_64
		if (clothPath == null || clothPath == string.Empty) {
			clothIdx = ConfigPool.Instance.GetConfigIntValue ("character", heroIndex.ToString (), "character");
			clothPath = ConfigPool.Instance.GetConfigStringValue ("clothing", "uid", "path", clothIdx);
			if (GameGlobal.DEBUG_CLOTH_UID > 0) {
				clothPath = ConfigPool.Instance.GetConfigStringValue ("clothing", "uid", "path", GameGlobal.DEBUG_CLOTH_UID);
			}
		}
#endif
		this.girlnames [0] = clothPath;
		string[] _armnames = BattlePetComponent.Instance.GetPetPerfabNames ();
		if (_armnames != null) {
			this.armnames = _armnames;
		}

		this.arms = new GameObject[this.armnames.Length];
		// Girl init.
		for (int i = 0; i < this.girlnames.Length; i++) {
			string _gilrName = this.girlnames [i];
			if (_gilrName == null || _gilrName == string.Empty) {
				continue;
			}

			this.StartCoroutine (this.__ReloadGirl (i, _gilrName));
		}

		// Arm init.
		for (int i = 0; i < this.armnames.Length; i++) {
			string _armName = this.armnames [i];
			if (_armName == null || _armName == string.Empty) {
				continue;
			}

			this.StartCoroutine (this.__ReloadArm (i, _armName));
		}

		EffectManager.Instance.SetEffectByCharact (heroIndex);
	}

	private IEnumerator __ReloadGirl(int girlIdx, string pathName) {
		GameObject _girl = StageBattleComponent.Instance.AddObj (ref pathName);
		if (_girl == null) {
			Debug.Log (pathName + " is null.");
			yield return null;
		}
/*
		WWW streamGirl = new WWW (AssetBundleFileMangager.FileLoadResPath + "/girl111.ab");
		yield return streamGirl;

		_girl.SetActive (false);
		_girl = GameObject.Instantiate (streamGirl.assetBundle.LoadAsset<UnityEngine.Object> ("assets/resources/prefabs/girls/buruo23333.prefab")) as GameObject;
*/
		_girl.transform.SetParent (this.gameObject.transform, false);
		SpineActionController sac = _girl.GetComponent<SpineActionController> ();
		sac.Init (girlIdx);
		SpineActionController.SetSynchroObjectsActive (_girl, false);
		//TODO : girl be generated here

		this.girls [girlIdx] = _girl;
	}

	private IEnumerator __ReloadArm(int armIdx, string pathName) {
		GameObject _arm = StageBattleComponent.Instance.AddObj (ref pathName);
		if (_arm == null) {
			Debug.Log (pathName + " is null.");
			yield return null;
		}

		_arm.transform.SetParent (this.gameObject.transform, false);
		SpineActionController sacArm = _arm.GetComponent<SpineActionController> ();
		sacArm.Init (armIdx);

		this.arms [armIdx] = _arm;
		BattlePetComponent.Instance.SetGameObject (armIdx, _arm);

		TaskStageTarget.Instance.AddArmCount (1);
	}

	private int RandomAttackIndex(int roleActIndex) {
		if (roleActIndex > 0) {
			return roleActIndex;
		}

		return Random.Range (0, 4);
	}

	public void ComeOut() {
		GameGlobal.gGameTouchPlay.SetPressHardTime (COMEOUT_HARD_TIME);
		this.StartCoroutine (this.AfterComeOut ());
	}

	private IEnumerator AfterComeOut () {
		yield return new WaitForSeconds (0.1f);

		this.StartAutoReduceEnergy ();
		for (int i = 0; i < this.girls.Length; i++) {
			GameObject _girl = this.girls [i];
			if (_girl == null) {
				continue;
			}

			this.girls [i].SetActive (true);
			GirlActionController gac = this.girls [i].GetComponent<GirlActionController> ();
			if (gac != null) {
				gac.OnControllerStart ();
			}
		}

		BattlePetComponent.Instance.SwitchPet ();
	}
	
	public void ResetAttacker() {
		/*
		for (int i = 0; i < this.girls.Length; i++) {
			GameObject _girl = this.girls [i];
			if (_girl == null) {
				continue;
			}

			GirlActionController _girlAction = _girl.GetComponent<GirlActionController> ();
			_girlAction.ResetAttacker ();
		}
		*/
	}

	public void UnLockActionProtect() {
		for (int i = 0; i < this.girls.Length; i++) {
			GameObject _girl = this.girls [i];
			if (_girl == null) {
				continue;
			}

			SpineActionController sac = _girl.GetComponent<SpineActionController> ();
			if (sac != null) {
				sac.SetProtectLevel (0);
				sac.SetCurrentActionName (null);
			}
		}
	}

	public void AttacksWithoutExchange(uint result, string actKey = null) {
		for (int i = 0; i < this.girls.Length; i++) {
			GameObject _girl = this.girls [i];
			if (_girl == null) {
				continue;
			}

			GirlActionController _girlAction = _girl.GetComponent<GirlActionController> ();
			_girlAction.AttackQuick (actKey, result);
		}
	}

	public void AttackWithExchange(uint result ,string actKey = null) {
		if (this.girls.Length <= 1) {
			this.AttacksWithoutExchange (result, actKey);
			return;
		}

		for (int i = 0; i < this.girls.Length; i++) {
			GameObject _girl = this.girls [i];
			if (_girl == null) {
				continue;
			}

			GirlActionController _girlAction = _girl.GetComponent<GirlActionController> ();
			_girlAction.Attack (actKey, result);
		}
	}

	public void BeAttackEffect() {
		for (int i = 0; i < this.girls.Length; i++) {
			GameObject _girl = this.girls [i];
			if (_girl == null) {
				continue;
			}

			GirlActionController _girlAction = _girl.GetComponent<GirlActionController> ();

			// First state judge.
			if (!this.IsJumpingAction ()) {
				_girlAction.AttackQuick (ACTION_KEYS.HURT, GameMusic.MISS);
			} else {
				float tick = 0;
				Animator ani = _girl.GetComponent<Animator> ();
				if (ani != null) {
					tick = (float)ani.GetTime ();
				}

				_girlAction.AttackQuick (ACTION_KEYS.JUMP_HURT, GameMusic.MISS, tick);
			}

			// Then state set.
			//this.SetJumpingAction (false);
		}

		CharPanel.Instance.BeAttack ();
		ActerChangeColore.Instance.PlayAnimation ();
		// sound effect
		AudioManager.Instance.PlayHurtEffect ();
		SoundEffectComponent.Instance.SayByCurrentRole (GameGlobal.SOUND_TYPE_HURT);
	}

	public void StopBeAttckedEffect() {
	}

	public void SetJumpingAction(bool value) {
		this.isJumpingAction = value;
	}

	public bool IsJumpingAction() {
		return this.isJumpingAction;
	}

	public void JumpBeatPause() {
		foreach (var girl in this.girls) {
			if (girl == null) {
				continue;
			}

			CharactorJumpAssert cja = girl.GetComponent<CharactorJumpAssert> ();
			if (cja == null) {
				continue;
			}

			cja.DoDelay ();
		}
	}

	public void PlayGirlDeadAnimation() {
		foreach (var girl in this.girls) {
			if (girl == null) {
				continue;
			}

			SpineActionController.Play (ACTION_KEYS.CHAR_DEAD, girl);
		}
	}

	public void StopPhysicDetect() {
		foreach (var girl in this.girls) {
			if (girl == null) {
				continue;
			}

			var spineMountController = girl.GetComponent<SpineMountController> ();
			if (spineMountController == null) {
				continue;
			}

			foreach (var detectScript in spineMountController.GetMountObjects()) {
				if (detectScript == null) {
					continue;
				}

				var detect = detectScript.GetComponent<GirlCollisionDetectNodeController> ();
				if (detect == null) {
					continue;
				}

				detect.DisableDetect ();
			}
		}
	}

	public void StartPhysicDetect() {
		foreach (var girl in this.girls) {
			if (girl == null) {
				continue;
			}
			
			var spineMountController = girl.GetComponent<SpineMountController> ();
			if (spineMountController == null) {
				continue;
			}
			
			foreach (var detectScript in spineMountController.GetMountObjects()) {
				if (detectScript == null) {
					continue;
				}
				
				var detect = detectScript.GetComponent<GirlCollisionDetectNodeController> ();
				if (detect == null) {
					continue;
				}
				
				detect.EnableDetect ();
			}
		}
	}

	public Vector3 GetCurrentGirlPositon() {
		if (this.girls == null || this.girls.Length <= 0) {
			return Vector3.zero;
		}

		GameObject girl = this.girls [0];
		if (girl == null) {
			return Vector3.zero;
		}

		return girl.transform.position;
	}
}