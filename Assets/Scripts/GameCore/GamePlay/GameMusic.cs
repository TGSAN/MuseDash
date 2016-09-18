using UnityEngine;
using System.Collections;
using DYUnityLib;
using LitJson;
using System.Collections.Generic;
using FormulaBase;

namespace GameLogic {
	public class GameMusic {
		public const uint NONE = 0;
		public const uint MISS = 1;
		public const uint COOL = 2;
		public const uint GREAT = 3;
		public const uint PERFECT = 4;
		public const uint JUMPOVER = 5;
		public const uint CRITICAL = 6;

		public const uint TOUCH_ACTION_NONE = 0;
		public const uint TOUCH_ACTION_SIGNLE_PRESS = 1;
		public const uint TOUCH_ACTION_LONG_PRESS = 2;
		public const uint TOUCH_ACTION_SLIDE_UP = 4;
		public const uint TOUCH_ACTION_SLIDE_DOWN = 5;

		//private const float SLIDETOUCHOFFSETX = 500.0f;
		//public const float SLIDETOUCHOFFSETY = 20.0f;

		public const int COOL_RANGE_INDEX = 2;
		public const int GREAT_RANGE_INDEX = 1;
		public const int PERFECT_RANGE_INDEX = 0;

		public const int NODE_IS_NOT_A_BOSS = 0;
		public const int NODE_IS_BOSS_BODY_ATTACK = 1;
		public const int NODE_IS_BOSS_THROW_ATTACK = 2;

		//result after miss
		public const decimal MISS_AVOID_TIME = 1.2m;			//avoid is unbeatable after miss

		private const string DEFAULT_ANIMATION = "AttackTargetObject/EnemyNormal";

		private decimal endHardTime = -1m;
		// for check miss
		private FixUpdateTimer objTimer = null;
		// for check all cd, touch and debug message
		private FixUpdateTimer stepTimer = null;
		private FixUpdateTimer sceneObjTimer = null;
		// stage name
		private string currentFileName = null;
		// time step record base on music time
		private decimal lastMusicTick = 0m;
		// for debug
		private int debugTRIdx = 0;
		private decimal[] debugTouchRecord = null;
		private Dictionary<int, int> _missMap = null;

		// This is used for Fix update, with standard time is music time.
		// When music time grow with unit 0.01, FixUpdateTimer.RollTimer is called.
		public void GameMusicFixTimerUpdate(){
			if (!AudioManager.Instance.IsBackGroundMusicPlaying ()) {
				FixUpdateTimer.RollTimer ();
				return;
			}

			decimal bgts = (decimal)AudioManager.Instance.GetBackGroundMusicTime ();
			bgts = bgts - bgts % FixUpdateTimer.dInterval;
			int c = (int)((bgts - this.lastMusicTick) / FixUpdateTimer.dInterval);
			this.lastMusicTick = bgts;
			for (int i = 0; i < c; i++) {
				FixUpdateTimer.RollTimer ();
			}
		}

		// Call for process something about cool down, progress bar, debug show, ect
		public void TimerStepTrigger(object sender, uint triggerId, params object[] args) {
			if (this.endHardTime > 0) {
				this.endHardTime -= oTimer.dInterval;
				if (!AudioManager.Instance.IsBackGroundMusicPlaying ()) {
					this.endHardTime = -1m;
					StageBattleComponent.Instance.End ();
				}
			} else if (this.endHardTime == 0) {
				this.endHardTime = -1m;
				StageBattleComponent.Instance.End ();
			}

			decimal missHardTime = GameGlobal.gGameMissPlay.GetMissHardTime ();
			if (missHardTime > 0) {
				missHardTime -= oTimer.dInterval;
				GameGlobal.gGameMissPlay.SetMissHardTime (missHardTime);
			} else if (missHardTime == 0) {
				CharPanel.Instance.StopBeAttack ();
				ActerChangeColore.Instance.StopAnimation ();
			}

			StageBattleComponent.Instance.SetTimeProgress ();
			GameGlobal.gGameTouchPlay.TimeStep ();
			// music time will be the standard time of game instand of timer time.
			// update progress bar
			// Debug about
			if (GameGlobal.IS_DEBUG) {
				decimal tsDec = (decimal)args [0];
				if (tsDec % 1m == 0) {
					//GameGlobal.gBuffMgr.OnTick ();
				}

				float tsorg = (float)tsDec;
				float bgts = AudioManager.Instance.GetBackGroundMusicTime ();
				float totalMusicTime = AudioManager.Instance.GetBackGroundMusicLength ();
				if (AudioManager.Instance.IsBackGroundMusicPlaying ()) {
					var rate = bgts / totalMusicTime;
					// Progress bar
					FightMenuPanel.Instance.ChangeSongProgress (rate);
				}
				// Auto play by recorded play data(touch time).
				if (this.debugTouchRecord != null) {
					int l = this.debugTouchRecord.Length;
					if (l > 0 && this.debugTRIdx < l) {
						decimal touchtick = (decimal)this.debugTouchRecord [this.debugTRIdx];
						if (tsDec == touchtick) {
							this.debugTRIdx += 1;
							GameGlobal.gGameTouchPlay.TouchTrigger (sender, triggerId, gTrigger.DYUL_EVENT_TOUCH_BEGAN); 
						}
					}
				}
			}
		}

		// Call for process something about miss.
		public void TimerTrigger(object sender, uint triggerId, params object[] args){
			//float fts = AudioManager.Instance.GetBackGroundMusicTime();
			// args [0] is passed tick of this timer
			decimal ts = (decimal)args [0];
			int _missTickIdx = (int)(ts / FixUpdateTimer.dInterval);
			//decimal ts = ((decimal)(int)(fts * 100)) * 0.01m;
			if (!this._missMap.ContainsKey (_missTickIdx)) {
				Debug.Log ("Miss tick " + ts + " has no pair node");
				return;
			}

			int idx = this._missMap [_missTickIdx];
			GameGlobal.gGameMissPlay.MissCube (idx, ts);
		}

		// First tick of this.objTimer
		public void TimerStartTrigger(object sender, uint triggerId, params object[] args){
			this.PlayMusic ();
			decimal ts = (decimal)args [0];
			Debug.Log ("Music start at " + ts);
		}

		// Last tick of this.objTimer
		public void TimerEndTrigger(object sender, uint triggerId, params object[] args){
			if (args == null || args.Length <= 0) {
				return;
			}

			try {
				float ts = (float)args [0];
				Debug.Log ("Music end at " + ts);
			} catch (System.Exception) {
				return;
			}
		}

		public decimal GetMusicPassTick(){
			if (this.objTimer == null) {
				return 0;
			}

			return this.objTimer.GetPassTick ();
		}

		public void SetTimer(ref FixUpdateTimer timer){
			this.objTimer = timer;
		}

		public void SetStepTimer(ref FixUpdateTimer timer){
			this.stepTimer = timer;
		}

		public void SetSceneObjTimer(ref FixUpdateTimer timer){
			this.sceneObjTimer = timer;
		}

		public bool IsRunning(){
			if (this.objTimer == null) {
				return false;
			}

			return this.objTimer.IsRunning ();
		}

		public void Run(){
			if (this.objTimer == null) {
				Debug.Log("Run music with a null timer.");
				return;
			}
			
			this.objTimer.Run ();

			if (this.stepTimer == null) {
				Debug.Log("Run music with a null step timer.");
				return;
			}
			
			this.stepTimer.Run ();

			if (this.sceneObjTimer == null) {
				Debug.Log("Run music with a null scene obj timer.");
				return;
			}
			
			this.sceneObjTimer.Run ();
		}

		public void Stop(){
			if (this.objTimer == null) {
				Debug.Log ("Stop music with a null timer.");
				return;
			}

			this.objTimer.Cancle ();

			if (this.stepTimer == null) {
				Debug.Log ("Stop music with a null step timer.");
				return;
			}
			
			this.stepTimer.Cancle ();

			if (this.sceneObjTimer == null) {
				Debug.Log ("Stop music with a null scene obj timer.");
				return;
			}
			
			this.sceneObjTimer.Cancle ();
		}

		public void Reset(){
			decimal delay = 0.0m;
			decimal total = GameGlobal.DEFAULT_MUSIC_LEN;
			StageBattleComponent.Instance.SetCombo (0);
			GameGlobal.gGameMissPlay.Init ();
			GameGlobal.gGameTouchPlay.Init ();
			this.InitEventList ();
			this.InitData ();
			this.InitTimer (delay, total);
		}

		public void LoadMusicDataByFileName(ref string filename, ref string audioName){
			this.currentFileName = filename;
			AudioManager.Instance.PlayingMusic = audioName;
			this.InitEventTrigger ();
			this.Reset ();

			Debug.Log ("Load music " + audioName);
		}

		public void PlayMusic(){
			float volume = StageBattleComponent.Instance.GetVolume ();
			AudioManager.Instance.SetEffectVolume (volume);
			AudioManager.Instance.PlayBackGroundMusic ();
		}

		public void SetEndHardTime(decimal t) {
			this.endHardTime = t;
		}
		
		public decimal GetEndHardTime() {
			return this.endHardTime;
		}

		public int GetMusicIndexByGenTick(decimal genTick) {
			ArrayList musicData = StageBattleComponent.Instance.GetMusicData ();
			if (musicData == null) {
				return -1;
			}
			
			if (musicData.Count <= 0) {
				return -1;
			}

			for (int i = 0; i < musicData.Count; i++) {
				MusicData md = (MusicData)musicData [i];
				if (md.tick - GameGlobal.COMEOUT_TIME_MAX == genTick) {
					return md.objId;
				}
			}

			return -1;
		}

		public int GetNodeIdByIdx(int idx) {
			ArrayList musicData = StageBattleComponent.Instance.GetMusicData ();
			if (musicData == null) {
				return 0;
			}

			if (idx < 0 || idx >= musicData.Count) {
				return 0;
			}

			MusicData md = (MusicData)musicData [idx];
			return md.objId;
		}

		public string GetNodeAnimation(int idx) {
			ArrayList musicData = StageBattleComponent.Instance.GetMusicData ();
			if (musicData == null) {
				return DEFAULT_ANIMATION;
			}

			if (idx < 0 || idx >= musicData.Count) {
				return DEFAULT_ANIMATION;
			}

			// Check special node.
			MusicData md = (MusicData)musicData [idx];

			return md.nodeData.prefab_path;
		}

		public uint GetNodeActType(int idx) {
			ArrayList musicData = StageBattleComponent.Instance.GetMusicData ();
			if (musicData == null) {
				return NONE;
			}
			
			if (idx < 0 || idx >= musicData.Count) {
				return NONE;
			}
			
			// Check special node.
			MusicData md = (MusicData)musicData [idx];
			return md.nodeData.hit_type;
		}

		public bool LastOne(int idx) {
			ArrayList musicData = StageBattleComponent.Instance.GetMusicData ();
			if (idx < musicData.Count - 1) {
				return false;
			}

			decimal mtLeave = (decimal)AudioManager.Instance.GetBackGroundMusicLeave ();
			mtLeave = decimal.Round (mtLeave, 2);
			this.SetEndHardTime (mtLeave + GameGlobal.DEFAULT_END_CD + StageBattleComponent.Instance.GetEndTimePlus ());
			Debug.Log ("Last one! " + mtLeave);

			FormulaHost host = BattleRoleAttributeComponent.Instance.GetBattleRole ();
			int hpRate = (int)(BattleRoleAttributeComponent.Instance.HpRate (host) * 100);
			TaskStageTarget.Instance.AddHpRateCount (hpRate);

			return true;
		}

		public void AfterRevived() {
			GameGlobal.gGameMissPlay.SetMissHardTime (MISS_AVOID_TIME);
			int combo = StageBattleComponent.Instance.GetCombo ();
			if (combo < GameTouchPlay.COMBO_SHOW_MIN) {
				EffectManager.Instance.StopCombo ();
			} else {
				if (combo % GameTouchPlay.COMBO_EFFECT_PLAY_MIN == 0) {
					EffectManager.Instance.ShowCombo (combo);
				} else {
					EffectManager.Instance.ShowCombo (combo, false);
				}
			}
		}

		private void InitEventList() {
		}

		private void InitData() {
			this.endHardTime = -1m;
			// GameGlobal.gStage.InitData ();
			Boss.Instance.SearchBoss ();
			this.ResetMusicData ();
		}

		private void InitTimer(decimal delay, decimal total) {
			// Timer
			if (this.objTimer == null) {
				Debug.Log ("Load music with null timer, before LoadMusicDataByFileName, call method SetTimer.");
				return;
			}

			int tickLimit = 2;
			this.objTimer.ClearTickEvent ();
			this.objTimer.Init (total);
			// Music begin/end event
			this.objTimer.AddTickEvent (delay, GameGlobal.MUSIC_START_PRESS);
			this.objTimer.AddTickEvent (total, GameGlobal.MUSIC_END_PRESS);
			// Music play event
			this._missMap = new Dictionary<int, int> ();
			ArrayList musicData = StageBattleComponent.Instance.GetMusicData ();
			for (int i = 0; i < musicData.Count; i++) {
				MusicData _md = (MusicData)musicData [i];
				if (_md.tick < tickLimit) {
					continue;
				}

				int _missTickIdx = (int)(_md.tick / FixUpdateTimer.dInterval);
				this._missMap [_missTickIdx] = _md.objId;
				this.objTimer.AddTickEvent (_md.tick, GameGlobal.MUSIC_SINGLE_PRESS);
			}

			if (this.stepTimer == null) {
				Debug.Log ("Load music with null step timer, before LoadMusicDataByFileName, call method SetStepTimer.");
				return;
			}

			// decimal mtLeave = (decimal)AudioManager.Instance.GetBackGroundMusicLeave ();
			total += (StageBattleComponent.Instance.GetEndTimePlus () + GameGlobal.DEFAULT_END_CD);
			this.stepTimer.ClearTickEvent ();
			this.stepTimer.Init (total, FixUpdateTimer.TIMER_TYPE_STEP_ARRAY);
			this.stepTimer.AddTickEvent (0, GameGlobal.MUSIC_STEP_EVENT);

			this.sceneObjTimer.ClearTickEvent ();
			this.sceneObjTimer.Init (total, FixUpdateTimer.TIMER_TYPE_STEP_ARRAY);
			this.sceneObjTimer.AddTickEvent (0, GameGlobal.SCENO_OBJ_STEP_EVENT);
		}

		private void ResetMusicData() {
			BattleEnemyManager.Instance.SetCurrentPlayIndex (-1);
			StageBattleComponent.Instance.Reset ();
		}

		public void MusicJump(decimal tick) {
			this.lastMusicTick = tick - FixUpdateTimer.dInterval;
			this.lastMusicTick = (this.lastMusicTick < 0m) ? 0m : this.lastMusicTick;

			CharPanel.Instance.StopCombo ();
			// GameGlobal.gStage.SetCurrentGenIdxForce (0);
			
			this.objTimer.SetProgress (tick);
			this.stepTimer.SetProgress (tick);
			this.sceneObjTimer.SetProgress (tick);
			GameGlobal.gGameMusicScene.stepTimer.SetProgress (tick);
			
			this.ResetMusicData ();

			SceneObjectController.Instance.TimerJump ((float)tick);
			BattleEnemyManager.Instance.SetPlayResultAfterTime (tick, GameMusic.NONE);
			//if (GameGlobal.IS_DEBUG) {
			//	UserUI.Instance.SetDebugIdxArray (0);
			//}
		}

		public void MusicJumpByPercentage(float percentage) {
			float musicLen = AudioManager.Instance.GetBackGroundMusicLength ();
			decimal tick = (decimal)(percentage * musicLen);
			this.MusicJump (tick);
		}

		private void InitEventTrigger(){
			// First clear, ensure no repeat.
			gTrigger.UnRegEvent (GameGlobal.MUSIC_START_PRESS);
			gTrigger.UnRegEvent (GameGlobal.MUSIC_END_PRESS);
			gTrigger.UnRegEvent (GameGlobal.MUSIC_SINGLE_PRESS);
			gTrigger.UnRegEvent (GameGlobal.MUSIC_STEP_EVENT);

			// Timer event music start
			EventTrigger eStart = gTrigger.RegEvent (GameGlobal.MUSIC_START_PRESS);
			eStart.Trigger += TimerStartTrigger;
			
			// Timer event music end
			EventTrigger eEnd = gTrigger.RegEvent (GameGlobal.MUSIC_END_PRESS);
			eEnd.Trigger += TimerEndTrigger;
			
			// Timer event of Touch control
			EventTrigger eSinglePress = gTrigger.RegEvent (GameGlobal.MUSIC_SINGLE_PRESS);
			eSinglePress.Trigger += TimerTrigger;
			
			// Timer event of step
			EventTrigger stPress = gTrigger.RegEvent (GameGlobal.MUSIC_STEP_EVENT);
			stPress.Trigger += TimerStepTrigger;
		}
	}
}