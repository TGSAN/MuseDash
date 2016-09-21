///自定义模块，可定制模块具体行为
using System;
using GameLogic;
using UnityEngine;
using System.Collections;
using DYUnityLib;
using FormulaBase;
using System.Collections.Generic;

namespace FormulaBase {
	public struct TimeNodeOrder {
		public int idx;
		public uint result;
		public bool mustJump;
		public bool enableJump;
	}

	public class StageBattleComponent : CustomComponentBase {

		private static StageBattleComponent instance = null;
		private const int HOST_IDX = 3;
		public static StageBattleComponent Instance {
			get {
				if(instance == null) {
					instance = new StageBattleComponent();
				}
			return instance;
			}
		}

		/// <summary>
		/// The stage.
		/// </summary>
		/// // ------------------------------------------------------------// ------------------------------------------------------------
		private const string stageNoSong = "NoSong";

		private bool isAutoPlay = false;
		private Hashtable prefabCatchObj = null;
		// music config data of game stage
		private ArrayList musicTickData = null;
		private Dictionary<int, List<TimeNodeOrder>> _timeNodeOrder = null;

		public void InitById(int idx) {
			if (this.Host == null || this.Host.GetDynamicIntByKey (SignKeys.ID) != idx) {
				this.Host = FomulaHostManager.Instance.LoadHost (HOST_IDX);
				this.Host.SetDynamicData (SignKeys.ID, idx);
				this.Host.SetDynamicData (SignKeys.NAME, stageNoSong);
			}

			TaskStageTarget.Instance.Init (idx);
			FormulaHost task = TaskStageTarget.Instance.Host;

			int diff = task.GetDynamicIntByKey (SignKeys.DIFFCULT);
			this.Host.SetDynamicData (SignKeys.DIFFCULT, diff);

			this.Host.Result (FormulaKeys.FORMULA_9);

			int energy = (int)this.Host.Result (FormulaKeys.FORMULA_330);
			this.Host.SetDynamicData (SignKeys.ENERGY, energy);

			int targetScore = (int)this.Host.Result (FormulaKeys.FORMULA_329);
			task.SetDynamicData (TaskStageTarget.TASK_SIGNKEY_SCORE + TaskStageTarget.TASK_SIGNKEY_COUNT_TARGET_TAIL, targetScore);
			this.Host.SetDynamicData (TaskStageTarget.TASK_SIGNKEY_SCORE + TaskStageTarget.TASK_SIGNKEY_COUNT_TARGET_TAIL, targetScore);

			this.Host.SetAsUINotifyInstance ();
		}

		public int GetStageId() {
			if (this.Host == null) {
				return -1;
			}

			return this.Host.GetDynamicIntByKey (SignKeys.ID);
		}

		public int GetCurrentStageId() {
			if (this.Host == null) {
				return (int)GameGlobal.DEBUG_DEFAULT_STAGE;
			}

			return (int)this.Host.GetDynamicDataByKey(SignKeys.ID);
		}

		public uint GetDiffcult() {
			if (this.Host == null) {
				return 0;
			}
			
			uint diff = (uint)this.Host.GetDynamicDataByKey (SignKeys.DIFFCULT);
			if (diff < GameGlobal.DIFF_LEVEL_NORMAL) {
				return GameGlobal.DIFF_LEVEL_NORMAL;
			}

			if (diff > GameGlobal.DIFF_LEVEL_SUPER) {
				return GameGlobal.DIFF_LEVEL_SUPER;
			}

			return diff;
		}

		public int GetChapterId(int stageId) {
			return ConfigPool.Instance.GetConfigIntValue("stage", stageId.ToString(), "chapter");
		}

		public List<int> GetStageIdsInChapter(int chapterId) {
			LitJson.JsonData jData = ConfigPool.Instance.GetConfigByName("stage");
			if (jData == null || jData.Count <= 0) {
				return null;
			}

			List<int> list = new List<int>();
			for (int i = 0; i < jData.Count + 1; i++) {
				int cid = this.GetChapterId(i);
				if (cid != chapterId) {
					continue;
				}

				list.Add(i);
			}

			return list;
		}

		public string GetStageName() {
			if (this.Host == null) {
				return null;
			}

			return this.Host.GetDynamicStrByKey (SignKeys.NAME).ToString ();
		}

		public string GetStageDesName() {
			if (this.Host == null) {
				return null;
			}

			return this.Host.GetDynamicStrByKey (SignKeys.STAGE_NAME).ToString ();
		}

		public string GetStageAuthorName() {
			if (this.Host == null) {
				return null;
			}

			return this.Host.GetDynamicStrByKey (SignKeys.AUTHOR).ToString ();
		}

		public string GetStageIconName()
		{
			if (this.Host == null) {
				return null;
			}
			
			return this.Host.GetDynamicStrByKey (SignKeys.MUSICICON).ToString ();
		}

		public string GetMusicName() {
			if (this.Host == null) {
				return null;
			}

			int sid = this.GetId ();
			uint diff = this.GetDiffcult ();
			string songKey = "FileName_" + diff;
			return ConfigPool.Instance.GetConfigStringValue ("stage", sid.ToString (), songKey);
		}

		public string GetSceneName() {
			if (this.Host == null) {
				return null;
			}
			
			return this.Host.GetDynamicStrByKey (SignKeys.SCENE_NAME).ToString ();
		}

		public string GetBossName() {
			if (this.Host == null) {
				return null;
			}
			
			return this.Host.GetDynamicStrByKey (SignKeys.BOSS_NAME).ToString ();
		}

		public float GetVolume() {
			if (this.Host == null) {
				return 0f;
			}
			
			return this.Host.Result (FormulaKeys.FORMULA_8);
		}

		public int GetCombo() {
			if (this.Host == null) {
				return 0;
			}
			
			return (int)this.Host.GetDynamicDataByKey (SignKeys.COMBO);
		}

		public int GetSorceBest() {
			return 0;
		}

		public ArrayList GetMusicData() {
			return this.musicTickData;
		}

		public MusicData GetMusicDataByIdx(int idx) {
			ArrayList musicDatas = this.GetMusicData ();
			if (musicDatas == null || idx < 0 || idx >= musicDatas.Count) {
				return new MusicData ();
			}

			return (MusicData)this.musicTickData [idx];
		}

		public decimal GetEndTimePlus() {
			return 0.5m;
		}

		public uint GetPlayResultLock() {
			return 0;
		}

		public bool IsChapterCleared(int chapterId) {
			if (chapterId <= 1) {
				return true;
			}

			List<int> _listStageIndex = StageBattleComponent.Instance.GetStageIdsInChapter (chapterId);
			if (_listStageIndex == null || _listStageIndex.Count <= 0) {
				return true;
			}

			for (int i = 0, max = _listStageIndex.Count; i < max; i++) {
				int sid = _listStageIndex [i];
				if (sid < 1) {
					continue;
				}

				if (TaskStageTarget.Instance.IsLock (sid)) {
					return false;
				}
			}

			return true;
		}

		public bool IsAutoPlay() {
			return this.isAutoPlay;
		}
		// ------------------------------------------------------------// ------------------------------------------------------------
		public void SetStageId(uint id) {
			if (this.Host == null) {
				return;
			}

			this.Host.SetDynamicData (SignKeys.ID, id);
			this.Host.Result (FormulaKeys.FORMULA_9);
		}

		public void SetPlayResultLock(uint result, uint lockLevel) {
		}

		public void UnLockPlayResult() {
		}

		public void SetCombo(int combo) {
			if (this.Host == null) {
				return;
			}
			
			this.Host.SetDynamicData (SignKeys.COMBO, combo);
			TaskStageTarget.Instance.AddComboMax (combo);
			BattleRoleAttributeComponent.Instance.FireSkill (SkillComponent.ON_COMBO);
		}

		public void SetAutoPlay(bool val) {
			this.isAutoPlay = val;
			Debug.Log ("Set auto play : " + val);
		}

		public void SetTimeProgress() {
			if (this.Host == null) {
				return;
			}

			float _t = AudioManager.Instance.GetBackGroundMusicTime ();
			float _tTotal = AudioManager.Instance.GetBackGroundMusicLength ();
			this.Host.SetDynamicData (SignKeys.BATTLE_ADD_UP_CTR_DYNAMIC, _tTotal);
			this.Host.SetDynamicData (SignKeys.BATTLE_ADD_UP_CTR, _t);
		}
		// ------------------------------------------------------------// ------------------------------------------------------------
		public void Enter(uint id, uint diff) {
			Debug.Log ("Enter Stage " + id + " with diffcult " + diff + " !");
			GameKernel.Instance.InitGameKernel ();

			this.Host.SetDynamicData (SignKeys.DIFFCULT, diff);
			this.Enter (id);
		}

		public void Enter(uint id) {
			if (!this.EnterCheck (id)) {
				return;
			}

			this.Host.SetDynamicData (SignKeys.ID, id);
			this.Host.Result (FormulaKeys.FORMULA_9);
			
			string _scenename = "GameScene";
			SceneLoader.SetLoadInfo (ref _scenename);
			Application.LoadLevel (GameGlobal.LOADING_SCENE_NAME);
		}

		private bool EnterCheck(uint id){
			if (this.Host == null) {
				return false;
			}

			//uint cid = (uint)this.stage.GetDynamicDataByKey (SignKeys.ID);
			//if (cid == id) {
			//	return false;
			//}
			
			return true;
		}

		// New GameObject dynamic by camera.
		public GameObject AddObj(ref string filename) {
			if (this.prefabCatchObj == null) {
				Debug.Log ("Stage prefabCatchObj not init.");
				return null;
			}
			
			GameObject catchObj = this.prefabCatchObj [filename] as GameObject;
			if (catchObj == null) {
				catchObj = Resources.Load (filename) as GameObject;
				if (catchObj == null) {
					// Debug.Log ("obj is empty");
					return null;
				}
				
				this.prefabCatchObj [filename] = catchObj;
			}
			
			GameObject obj = (GameObject)UnityEngine.Object.Instantiate (catchObj);
			
			return obj;
		}

		public GameObject AddObjWithControllerInit(ref string filename, int idx) {
			GameObject obj = this.AddObj(ref filename);
			if (obj != null) {
				NodeInitController initController = obj.GetComponent<NodeInitController> ();
				if (initController == null) {
					initController = obj.AddComponent<NodeInitController> ();
				}

				initController.Init (idx);
				
				SpineActionController sac = obj.GetComponent<SpineActionController> ();
				sac.Init (idx);
				
				obj.SetActive (true);
			}

			return obj;
		}

		/// <summary>
		/// Clears the near by object.
		/// 
		/// near by range is SLOW_DOWN_WHOLE_TIME (now_time + SLOW_DOWN_WHOLE_TIME)
		/// </summary>
		public void ClearNearByObject() {
			ArrayList musicData = StageBattleComponent.Instance.GetMusicData ();
			if (musicData == null) {
				return;
			}

			Dictionary<int, FormulaHost> enemies = BattleEnemyManager.Instance.Enemies;
			if (enemies == null) {
				return;
			}

			int longPressIdx = -1;
			for (int i = 0; i < enemies.Count; i++) {
				if (!enemies.ContainsKey (i)) {
					continue;
				}

				FormulaHost host = enemies [i];
				if (host == null) {
					continue;
				}

				MusicData md = (MusicData)musicData [i];
				if (md.nodeData.hit_type == GameMusic.NONE) {
					continue;
				}

				GameObject _obj = (GameObject)host.GetDynamicObjByKey (SignKeys.GAME_OBJECT);
				if (_obj != null) {
					SpineActionController _sac = _obj.GetComponent<SpineActionController> ();
					if (_sac != null) {
						if (_sac.actionMode == GameGlobal.LONG_PRESS_NODE_TYPE) {
							longPressIdx = i;
						} else {
							longPressIdx = -1;
						}
					}

					GameObject.Destroy (_obj);
				}
			}

			if (longPressIdx < 0) {
				return;
			}

			// If tail with long press node, clear until long press list end.
			for (int i = longPressIdx; i < enemies.Count; i++) {
				if (!enemies.ContainsKey (i)) {
					continue;
				}
				
				FormulaHost host = enemies [i];
				if (host == null) {
					continue;
				}

				GameObject obj = GameGlobal.gGameMusicScene.GetPreLoadGameObject (i);
				if (obj == null) {
					continue;
				}
				
				SpineActionController sac = obj.GetComponent<SpineActionController> ();
				if (sac == null) {
					continue;
				}
				
				if (sac.actionMode != GameGlobal.LONG_PRESS_NODE_TYPE) {
					break;
				} else {
					BattleEnemyManager.Instance.SetPlayResult (i, GameMusic.MISS);
					GameObject.Destroy (obj);
				}
			}
		}

		public void End() {
			// 结算统计
			bool isNewRank = TaskStageTarget.Instance.OnStageFinished ();
			// 先在stage host上设置好各种数据, 然后再显示ui
			// 结算UI数据 奖励
			StageRewardComponent.Instance.StageReward (this.Host, isNewRank);

			TaskStageTarget.Instance.Save ();
			GameKernel.Instance.InitGameKernel ();

			// TODO : 结算UI表现
			EffectManager.Instance.StopCombo ();
			FightMenuPanel.Instance.OnStageEnd ();
			UISceneHelper.Instance.ShowUi ("PnlVictory");
		}

		public void Dead() {
			int payBackPhysical = (int)(this.Host.Result (FormulaKeys.FORMULA_72) * 0.5);
			AccountPhysicsManagerComponent.Instance.ChangePhysical (payBackPhysical, false);
			EffectManager.Instance.StopCombo ();
			SoundEffectComponent.Instance.SayByCurrentRole (GameGlobal.SOUND_TYPE_DEAD);
		}

		public bool IsEndSettlement() {
			return UISceneHelper.Instance.IsUiActive ("PnlVictory");
		}

		public void GameStart(object sender, uint triggerId, params object[] args){
			if (GameGlobal.gGameMusic == null) {
				return;
			}
			
			if (GameGlobal.gGameMusic.IsRunning ()) {
				return;
			}

			GameGlobal.gGameMusic.Run ();
			GameGlobal.gGameMusicScene.Run ();
			GameGlobal.gGameMusic.PlayMusic ();
			Debug.Log ("Stage start");
			//UserUI.Instance.SetGUIActive (false);
		}

		// ResetMusicData
		public void Reset() {
			FixUpdateTimer.ResumeTimer();
			BattleEnemyManager.Instance.Init ();
			BattleRoleAttributeComponent.Instance.Revive (true);
			this.InitData ();
		}

		public void Revive() {
			AudioManager.Instance.ResumeBackGroundMusic ();
			AudioManager.Instance.SetBgmVolume (1f);
			BattleRoleAttributeComponent.Instance.Revive ();
			GameGlobal.gGameMusic.AfterRevived ();
		}

		// Reset start stage.
		public void ReEnter() {
			string sceneName = "GameScene";
			this.Exit (sceneName);
		}

		// Exit stage.
		public void Exit(string sceneName = "ChooseSongs") {
			Debug.Log ("Stage Exit.");
			GameGlobal.gGameMusic.Stop ();
			GameGlobal.gGameMusicScene.Stop ();
			gTrigger.ClearEvent ();
			GameGlobal.gTouch.ClearCustomEvent ();
			GameGlobal.gCamera.gameObject.SetActive (false);
			SceneLoader.SetLoadInfo (ref sceneName);
			Time.timeScale = GameGlobal.TIME_SCALE;
			CommonPanel.GetInstance ().SetMask (true, this.OnExit);
		}

		private void OnExit() {
			PnlAdventure.PnlAdventure.backFromBattle = true;
			Application.LoadLevel (GameGlobal.LOADING_SCENE_NAME);
		}

		/// <summary>
		/// Init this instance.
		/// 
		/// 关卡初始化入口
		/// </summary>
		public void Init(){
			StageTeachComponent.Instance.Init ();

			Time.timeScale = GameGlobal.TIME_SCALE;
			this.prefabCatchObj = new Hashtable ();

			GameGlobal.gGameMissPlay = new GameMissPlay ();
			GameGlobal.gGameTouchPlay = new GameTouchPlay ();
			GameGlobal.gGameMusic = new GameMusic ();
			GameGlobal.gGameMusicScene = new GameMusicScene ();
			
			FixUpdateTimer tGm = new FixUpdateTimer ();
			FixUpdateTimer tStepGm = new FixUpdateTimer ();
			FixUpdateTimer tStepOm = new FixUpdateTimer ();
			FixUpdateTimer tGms = new FixUpdateTimer ();
			GameGlobal.gGameMusic.SetTimer (ref tGm);
			GameGlobal.gGameMusic.SetStepTimer (ref tStepGm);
			GameGlobal.gGameMusic.SetSceneObjTimer (ref tStepOm);
			GameGlobal.gGameMusicScene.SetTimer (ref tGms);
			
			GameGlobal.gTouch.OnStart ();

			string sceneName = this.GetSceneName ();
			string musicName = this.GetMusicName ();
			string fileName = this.GetStageName ();
			this.Load (ref fileName, ref sceneName, ref musicName);

			TaskStageTarget.Instance.OnStageStarted ();
			SkillComponent.Instance.Init ();
			BattleRoleAttributeComponent.Instance.Init ();
		}

		private void Load(ref string fileName, ref string sceneName, ref string audioName) {
			int sid = this.Host.GetDynamicIntByKey (SignKeys.ID);
			Debug.Log ("Load stage (" + sid + ") " + sceneName + " ===== " + fileName + " ===== " + audioName);
			AudioManager.Instance.ReloadAllResource ();
			//AudioManager.Instance.PlayingMusic = sceneName;
			AudioManager.Instance.SetBackGroundMusic (audioName);
			// For ensure music start with NO any delay.
			AudioManager.Instance.ResetBackGroundMusic ();
			GameGlobal.gGameMusic.LoadMusicDataByFileName (ref sceneName, ref audioName);

			this.InitTimeNodeOrder ();
			GameGlobal.gGameMusicScene.LoadScene ((int)sid, sceneName);
		}

		private void InitTimeNodeOrder() {
			this._timeNodeOrder = new Dictionary<int, List<TimeNodeOrder>> ();
			for (int i = 0; i < this.musicTickData.Count; i++) {
				MusicData md = (MusicData)this.musicTickData [i];
				if (md.nodeData.hit_type <= GameMusic.NONE) {
					continue;
				}

				int _s = (int)(md.tick / FixUpdateTimer.dInterval);
				int _ivrtotal = (int)(md.nodeData.hitRangeAll / FixUpdateTimer.dInterval);
				decimal hrPerfect = md.nodeData.perfect_range;
				decimal hrGreate = md.nodeData.perfect_range + md.nodeData.great_range;

				for (int _i = 0; _i < _ivrtotal; _i++) {
					TimeNodeOrder _tno = new TimeNodeOrder ();
					_tno.idx = md.objId;
					_tno.mustJump = md.nodeData.jump_note;
					_tno.enableJump = md.nodeData.enable_jump;
					_tno.result = GameMusic.PERFECT;

					decimal _r = _i * FixUpdateTimer.dInterval;
					if (_r > hrPerfect) {
						_tno.result = GameMusic.GREAT;
					}

					if (_r > hrGreate) {
						_tno.result = GameMusic.COOL;
					}

					int _tnoIdx = _s - _i;

					if (!this._timeNodeOrder.ContainsKey (_tnoIdx)) {
						this._timeNodeOrder [_tnoIdx] = new List<TimeNodeOrder> ();
					}

					this._timeNodeOrder [_tnoIdx].Add (_tno);
				}
			}
		}

		private void InitData() {
			this.LoadMusicData ();
			for (int i = 0; i < this.musicTickData.Count; i++) {
				MusicData md = (MusicData)this.musicTickData [i];
				AudioManager.Instance.AddAudioResource (md.nodeData.key_audio);
			}
		}

		private void LoadMusicData() {
			uint diff = this.GetDiffcult ();
			string name = this.GetStageName ();
			string cfgName = name + diff;

			this.musicTickData = MusicConfigReader.Instance.GetData (ref cfgName);
		}

		public void AddGold(int value) {
			int _gold = this.Host.GetDynamicIntByKey (SignKeys.GOLD) + value;
			this.Host.SetDynamicData (SignKeys.GOLD, _gold);
		}

		public List<TimeNodeOrder> GetTimeNodeByTick(decimal tick) {
			int _t = (int)(tick / FixUpdateTimer.dInterval);
			if (!this._timeNodeOrder.ContainsKey (_t)) {
				return null;
			}

			return this._timeNodeOrder [_t];
		}

		public int GetStagePhysical() {
			return (int)this.Host.Result(FormulaKeys.FORMULA_72);
		}

		public string GetSatageAuthor() {
			this.Host.Result(FormulaKeys.FORMULA_9);
			return this.Host.GetDynamicStrByKey(SignKeys.AUTHOR);
		}
	}
}