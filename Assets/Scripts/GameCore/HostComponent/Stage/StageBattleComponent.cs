using Assets.Scripts.Common.Manager;
using Assets.Scripts.NGUI;
using DYUnityLib;
using FormulaBase;
using GameLogic;

///自定义模块，可定制模块具体行为
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace FormulaBase
{
    public class TimeNodeOrder
    {
        public int idx;
        public uint result;
        public bool mustJump;
        public bool enableJump;
        public bool isPerfectNode;
        public bool isLongPressStart;
        public bool isLongPressEnd;
        public bool isLongPress;
    }

    public class StageBattleComponent : CustomComponentBase
    {
        private static StageBattleComponent instance = null;
        private const int HOST_IDX = 3;

        public static StageBattleComponent Instance
        {
            get
            {
                if (instance == null)
                {
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
        public int curLPSIdx = 0;
        public int musicStartTime = 0;

        public float timeFromMusicStart
        {
            get { return (realTimeTick - musicStartTime) / 1000f; }
        }

        public int realTimeTick
        {
            get { return Mathf.RoundToInt(GameGlobal.stopwatch.ElapsedMilliseconds); }
        }

        public MusicData neareastMusicData
        {
            get
            {
                var curTick = GameGlobal.gGameMusic.GetMusicPassTick();
                var maxTick = (decimal)AudioManager.Instance.GetBackGroundMusicLength();
                var nextMd = new MusicData();
                nextMd.objId = -1;
                var tick = curTick;
                while (nextMd.objId == -1 && tick < maxTick)
                {
                    nextMd = GameGlobal.gGameMusic.GetMusicDataByTick(tick);
                    tick += FixUpdateTimer.dInterval;
                }

                var preMd = new MusicData();
                preMd.objId = -1;
                tick = curTick;
                var isDead = false;
                while (preMd.objId == -1 && tick > 0 && !isDead)
                {
                    preMd = GameGlobal.gGameMusic.GetMusicDataByTick(tick);
                    tick -= FixUpdateTimer.dInterval;
                    if (preMd.objId != -1)
                    {
                        isDead = BattleEnemyManager.Instance.IsDead(preMd.objId);
                    }
                }

                var outMd = Mathf.Abs((float)(preMd.tick - curTick)) > Mathf.Abs((float)(nextMd.tick - curTick)) ? nextMd : preMd;

                return outMd;
            }
        }

        public TimeNodeOrder curTimeNodeOrder
        {
            get
            {
                var idx = curIdx;
                if (_timeNodeOrder.ContainsKey(idx))
                {
                    return this._timeNodeOrder[idx][0];
                }
                return null;
            }
        }

        public int curIdx
        {
            get
            {
                var curTick = GameGlobal.gGameMusic.GetMusicPassTick();
                var idx = curTick / FixUpdateTimer.dInterval;
                return (int)idx;
            }
        }

        public uint GetCurLPSPlayResult()
        {
            if (curLPSIdx == -1 || GameKernel.Instance.IsLongPressFailed)
            {
                return GameMusic.GREAT;
            }
            var result = BattleEnemyManager.Instance.GetPlayResult(curLPSIdx);
            if (result != GameMusic.PERFECT)
            {
                return GameMusic.GREAT;
            }
            return result;
        }

        public void InitById(int idx)
        {
            if (this.Host == null || this.Host.GetDynamicIntByKey(SignKeys.ID) != idx)
            {
                this.Host = FomulaHostManager.Instance.LoadHost(HOST_IDX);
                this.Host.SetDynamicData(SignKeys.ID, idx);
                this.Host.SetDynamicData(SignKeys.NAME, stageNoSong);
            }

            TaskStageTarget.Instance.Init(idx);
            FormulaHost task = TaskStageTarget.Instance.Host;

            int diff = task.GetDynamicIntByKey(SignKeys.DIFFCULT);
            this.Host.SetDynamicData(SignKeys.DIFFCULT, diff);

            this.Host.Result(FormulaKeys.FORMULA_7);

            int energy = (int)this.Host.Result(FormulaKeys.FORMULA_20);
            this.Host.SetDynamicData(SignKeys.ENERGY, energy);

            int targetScore = (int)this.Host.Result(FormulaKeys.FORMULA_87);
            if (targetScore != 0)
            {
                task.SetDynamicData(TaskStageTarget.TASK_SIGNKEY_SCORE + TaskStageTarget.TASK_SIGNKEY_COUNT_TARGET_TAIL, targetScore);
                this.Host.SetDynamicData(TaskStageTarget.TASK_SIGNKEY_SCORE + TaskStageTarget.TASK_SIGNKEY_COUNT_TARGET_TAIL, targetScore);
            }

            this.Host.SetAsUINotifyInstance();
        }

        public int GetStageId()
        {
            if (this.Host == null)
            {
                return -1;
            }

            return this.Host.GetDynamicIntByKey(SignKeys.ID);
        }

        public int GetCurrentStageId()
        {
            if (this.Host == null)
            {
                return (int)GameGlobal.DEBUG_DEFAULT_STAGE;
            }

            return (int)this.Host.GetDynamicDataByKey(SignKeys.ID);
        }

        public uint GetDiffcult()
        {
            if (this.Host == null)
            {
                return 0;
            }

            uint diff = (uint)this.Host.GetDynamicDataByKey(SignKeys.DIFFCULT);
            if (diff < GameGlobal.DIFF_LEVEL_NORMAL)
            {
                return GameGlobal.DIFF_LEVEL_NORMAL;
            }

            if (diff > GameGlobal.DIFF_LEVEL_SUPER)
            {
                return GameGlobal.DIFF_LEVEL_SUPER;
            }

            return diff;
        }

        public int GetChapterId(int stageId)
        {
            return ConfigPool.Instance.GetConfigIntValue("stage", stageId.ToString(), "chapter");
        }

        public List<int> GetStageIdsInChapter(int chapterId)
        {
            LitJson.JsonData jData = ConfigPool.Instance.GetConfigByName("stage");
            if (jData == null || jData.Count <= 0)
            {
                return null;
            }

            List<int> list = new List<int>();
            for (int i = 0; i < jData.Count + 1; i++)
            {
                int cid = this.GetChapterId(i);
                if (cid != chapterId)
                {
                    continue;
                }

                list.Add(i);
            }

            return list;
        }

        public bool IsAllCombo()
        {
            var musicData = GetMusicData();
            var count = musicData.Cast<MusicData>().Count(d => d.nodeData.addCombo);
            return GetCombo() >= count;
        }

        public string GetStageName()
        {
            if (this.Host == null)
            {
                return null;
            }

            return this.Host.GetDynamicStrByKey(SignKeys.NAME).ToString();
        }

        public string GetStageDesName()
        {
            if (this.Host == null)
            {
                return null;
            }

            return this.Host.GetDynamicStrByKey(SignKeys.STAGE_NAME).ToString();
        }

        public string GetStageAuthorName()
        {
            if (this.Host == null)
            {
                return null;
            }

            return this.Host.GetDynamicStrByKey(SignKeys.AUTHOR).ToString();
        }

        public string GetStageIconName()
        {
            if (this.Host == null)
            {
                return null;
            }

            return this.Host.GetDynamicStrByKey(SignKeys.MUSICICON).ToString();
        }

        public string GetMusicName()
        {
            if (this.Host == null)
            {
                return null;
            }

            int sid = this.GetId();
            return ConfigPool.Instance.GetConfigStringValue("stage", sid.ToString(), "music");
        }

        public string GetSceneName()
        {
            if (this.Host == null)
            {
                return null;
            }

            return this.Host.GetDynamicStrByKey(SignKeys.SCENE_NAME).ToString();
        }

        public string GetBossName()
        {
            if (this.Host == null)
            {
                return null;
            }

            return this.Host.GetDynamicStrByKey(SignKeys.BOSS_NAME).ToString();
        }

        public float GetVolume()
        {
            if (this.Host == null)
            {
                return 0f;
            }

            return this.Host.Result(FormulaKeys.FORMULA_6);
        }

        public int GetCombo()
        {
            if (this.Host == null)
            {
                return 0;
            }

            return (int)this.Host.GetDynamicDataByKey(SignKeys.COMBO);
        }

        public int GetSorceBest()
        {
            return 0;
        }

        public ArrayList GetMusicData()
        {
            return this.musicTickData;
        }

        public MusicData GetMusicDataByIdx(int idx)
        {
            ArrayList musicDatas = this.GetMusicData();
            if (musicDatas == null || idx < 0 || idx >= musicDatas.Count)
            {
                return new MusicData();
            }

            return (MusicData)this.musicTickData[idx];
        }

        public decimal GetEndTimePlus()
        {
            return 0.5m;
        }

        public int GetStageCount()
        {
            return ConfigPool.Instance.GetConfigByName("stage").Count;
        }

        public uint GetPlayResultLock()
        {
            return 0;
        }

        public bool IsChapterCleared(int chapterId)
        {
            if (chapterId <= 1)
            {
                return true;
            }

            List<int> _listStageIndex = StageBattleComponent.Instance.GetStageIdsInChapter(chapterId);
            if (_listStageIndex == null || _listStageIndex.Count <= 0)
            {
                return true;
            }

            for (int i = 0, max = _listStageIndex.Count; i < max; i++)
            {
                int sid = _listStageIndex[i];
                if (sid < 1)
                {
                    continue;
                }

                if (TaskStageTarget.Instance.IsLock(sid))
                {
                    return false;
                }
            }

            return true;
        }

        public bool IsAutoPlay()
        {
            return this.isAutoPlay;
        }

        // ------------------------------------------------------------// ------------------------------------------------------------
        public void SetStageId(uint id)
        {
            if (this.Host == null)
            {
                return;
            }

            this.Host.SetDynamicData(SignKeys.ID, id);
            this.Host.Result(FormulaKeys.FORMULA_7);
        }

        public void SetPlayResultLock(uint result, uint lockLevel)
        {
        }

        public void UnLockPlayResult()
        {
        }

        public void SetCombo(int combo)
        {
            if (this.Host == null)
            {
                return;
            }

            this.Host.SetDynamicData(SignKeys.COMBO, combo);
            TaskStageTarget.Instance.AddComboMax(combo);
            BattleRoleAttributeComponent.Instance.FireSkill(SkillComponent.ON_COMBO);
        }

        public void SetAutoPlay(bool val)
        {
            this.isAutoPlay = val;
            Debug.Log("Set auto play : " + val);
        }

        public void SetTimeProgress()
        {
            if (this.Host == null)
            {
                return;
            }

            float _t = AudioManager.Instance.GetBackGroundMusicTime();
            float _tTotal = AudioManager.Instance.GetBackGroundMusicLength();
            this.Host.SetDynamicData(SignKeys.BATTLE_ADD_UP_CTR_DYNAMIC, _tTotal);
            this.Host.SetDynamicData(SignKeys.BATTLE_ADD_UP_CTR, _t);
        }

        // ------------------------------------------------------------// ------------------------------------------------------------
        public void Enter(uint id, uint diff)
        {
            //扣体力回调
            var r = AccountPhysicsManagerComponent.Instance.ChangePhysical(-(int)Host.Result(FormulaKeys.FORMULA_20), false, true,
               result =>
               {
                   if (PnlScrollCircle.instance != null)
                   {
                       SceneAudioManager.Instance.bgm.clip = PnlScrollCircle.instance.CatchClip;
                   }

                   if (UISceneHelper.Instance != null)
                   {
                       UISceneHelper.Instance.HideWidget();
                   }
                   Debug.Log("Enter Stage " + id + " with diffcult " + diff + " !");
                   GameKernel.Instance.InitGameKernel();

                   this.Host.SetDynamicData(SignKeys.DIFFCULT, diff);
                   this.Enter(id);
               });
            if (r == 0)
            {
                CommonPanel.GetInstance().ShowText("体力不足哦~");
            }
        }

        public void Enter(uint id)
        {
            if (!this.EnterCheck(id))
            {
                return;
            }

            this.Host.SetDynamicData(SignKeys.ID, id);
            this.Host.Result(FormulaKeys.FORMULA_7);

            string _scenename = "GameScene";
            SceneLoader.SetLoadInfo(ref _scenename);
            SceneManager.LoadScene(GameGlobal.LOADING_SCENE_NAME);
            CommonPanel.GetInstance().ShowWaittingPanel(false);
        }

        private bool EnterCheck(uint id)
        {
            if (this.Host == null)
            {
                return false;
            }

            //uint cid = (uint)this.stage.GetDynamicDataByKey (SignKeys.ID);
            //if (cid == id) {
            //	return false;
            //}

            return true;
        }

        // New GameObject dynamic by camera.
        public GameObject AddObj(ref string filename)
        {
            if (this.prefabCatchObj == null)
            {
                Debug.Log("Stage prefabCatchObj not init.");
                return null;
            }
            GameObject catchObj = this.prefabCatchObj[filename] as GameObject;
            if (catchObj == null)
            {
                ResourceLoader.Instance.Load(filename, res => catchObj = res as GameObject);
                if (catchObj == null)
                {
                    // Debug.Log ("obj is empty");
                    return null;
                }

                this.prefabCatchObj[filename] = catchObj;
            }

            GameObject obj = (GameObject)UnityEngine.Object.Instantiate(catchObj);

            return obj;
        }

        public GameObject AddObjWithControllerInit(ref string filename, int idx)
        {
            GameObject obj = this.AddObj(ref filename);
            if (obj != null)
            {
                NodeInitController initController = obj.GetComponent<NodeInitController>();
                if (initController == null)
                {
                    initController = obj.AddComponent<NodeInitController>();
                }
                initController.Init(idx);

                SpineActionController sac = obj.GetComponent<SpineActionController>();
                sac.Init(idx);

                obj.SetActive(true);
            }

            return obj;
        }

        /// <summary>
        /// Clears the near by object.
        ///
        /// near by range is SLOW_DOWN_WHOLE_TIME (now_time + SLOW_DOWN_WHOLE_TIME)
        /// </summary>
        public void ClearNearByObject()
        {
            ArrayList musicData = StageBattleComponent.Instance.GetMusicData();
            if (musicData == null)
            {
                return;
            }

            Dictionary<int, FormulaHost> enemies = BattleEnemyManager.Instance.Enemies;
            if (enemies == null)
            {
                return;
            }

            int longPressIdx = -1;
            for (int i = 0; i < enemies.Count; i++)
            {
                if (!enemies.ContainsKey(i))
                {
                    continue;
                }

                FormulaHost host = enemies[i];
                if (host == null)
                {
                    continue;
                }

                MusicData md = (MusicData)musicData[i];
                if (md.nodeData.hit_type == GameMusic.NONE)
                {
                    continue;
                }

                GameObject _obj = (GameObject)host.GetDynamicObjByKey(SignKeys.GAME_OBJECT);
                if (_obj != null)
                {
                    SpineActionController _sac = _obj.GetComponent<SpineActionController>();
                    if (_sac != null)
                    {
                        if (_sac.actionMode == GameGlobal.LONG_PRESS_NODE_TYPE)
                        {
                            longPressIdx = i;
                        }
                        else
                        {
                            longPressIdx = -1;
                        }
                    }

                    GameObject.Destroy(_obj);
                }
            }

            if (longPressIdx < 0)
            {
                return;
            }

            // If tail with long press node, clear until long press list end.
            for (int i = longPressIdx; i < enemies.Count; i++)
            {
                if (!enemies.ContainsKey(i))
                {
                    continue;
                }

                FormulaHost host = enemies[i];
                if (host == null)
                {
                    continue;
                }

                GameObject obj = GameGlobal.gGameMusicScene.GetPreLoadGameObject(i);
                if (obj == null)
                {
                    continue;
                }

                SpineActionController sac = obj.GetComponent<SpineActionController>();
                if (sac == null)
                {
                    continue;
                }

                if (sac.actionMode != GameGlobal.LONG_PRESS_NODE_TYPE)
                {
                    break;
                }
                else
                {
                    BattleEnemyManager.Instance.SetPlayResult(i, GameMusic.MISS);
                    GameObject.Destroy(obj);
                }
            }
        }

        public void End()
        {
            // 结算统计
            bool isNewRank = TaskStageTarget.Instance.OnStageFinished();
            //成就统计
            AchievementManager.instance.SetAchievement();
            // 结算UI数据 奖励
            StageRewardComponent.Instance.StageReward(this.Host, isNewRank);

            TaskStageTarget.Instance.Save();
            GameKernel.Instance.InitGameKernel();

            // TODO : 结算UI表现
            EffectManager.Instance.StopCombo();
            FightMenuPanel.Instance.OnStageEnd();
            UISceneHelper.Instance.HideUi("PnlBattle");
            UISceneHelper.Instance.ShowUi("PnlVictory");
        }

        public void Dead()
        {
            Debug.Log("Player dead.");
            int payBackPhysical = 0;
            AccountPhysicsManagerComponent.Instance.ChangePhysical(payBackPhysical, false);
            EffectManager.Instance.StopCombo();
            SoundEffectComponent.Instance.SayByCurrentRole(GameGlobal.SOUND_TYPE_DEAD);
        }

        public bool IsEndSettlement()
        {
            return UISceneHelper.Instance.IsUiActive("PnlVictory");
        }

        public void GameStart(object sender, uint triggerId, params object[] args)
        {
            if (GameGlobal.gGameMusic == null)
            {
                return;
            }

            if (GameGlobal.gGameMusic.IsRunning())
            {
                return;
            }

            GameGlobal.gGameMusic.PlayMusic();
            AudioManager.Instance.SetBgmVolume(0f);
            DelayStartGame();
            //UserUI.Instance.SetGUIActive (false);
        }

        private void DelayStartGame()
        {
            var delayTimer = new FixUpdateTimer();
            var dt = 0f;

#if UNITY_ANDROID && !UNITY_EDITOR
            dt = (float)GameGlobal.DELAY_FOR_ANDRIOD;
#endif
            delayTimer.Init(10m);
            delayTimer.Run();
            var musicDelay = GameGlobal.DELAY_FOR_MUSIC;
            var gameDelay = GameGlobal.DELAY_FOR_GAMESTART;
            var gameEventID = GameGlobal.DELAY_START_GAME;
            var musicEventID = GameGlobal.DELAY_PLAY_MUSIC;

            EventTrigger.TriggerHandler startGame = (s, t, a) =>
            {
                GameGlobal.gGameMusic.Run();
                GameGlobal.gGameMusicScene.Run();
                CommonPanel.GetInstance().DebugInfo("Game start at time: " + StageBattleComponent.Instance.timeFromMusicStart);
            };

            EventTrigger.TriggerHandler startMusic = (s, t, a) =>
            {
                AudioManager.Instance.SetBackGroundMusicProgress(dt);
                AudioManager.Instance.SetBgmVolume(1.0f);
                musicStartTime = StageBattleComponent.Instance.realTimeTick + Mathf.RoundToInt(dt * 1000f);
                CommonPanel.GetInstance().DebugInfo("Music start at time: " + 0);
            };

            gTrigger.UnRegEvent(musicEventID);
            var musicTrigger = gTrigger.RegEvent(musicEventID);
            musicTrigger.Trigger += startMusic;

            if (musicDelay == gameDelay)
            {
                musicTrigger.Trigger += startGame;
            }
            else
            {
                gTrigger.UnRegEvent(gameEventID);
                var trigger = gTrigger.RegEvent(gameEventID);
                trigger.Trigger += startGame;
                delayTimer.AddTickEvent(gameDelay, gameEventID);
            }
            delayTimer.AddTickEvent(musicDelay, musicEventID);
        }

        // ResetMusicData
        public void Reset()
        {
            FixUpdateTimer.ResumeTimer();
            BattleEnemyManager.Instance.Init();
            BattleRoleAttributeComponent.Instance.Revive(true);
            this.InitData();
        }

        public void Revive()
        {
            AudioManager.Instance.ResumeBackGroundMusic();
            AudioManager.Instance.SetBgmVolume(1f);
            BattleRoleAttributeComponent.Instance.Revive();
            GameGlobal.gGameMusic.AfterRevived();
        }

        // Reset start stage.
        public void ReEnter()
        {
            string sceneName = "GameScene";
            this.Exit(sceneName, false, true);
        }

        // Exit stage.
        public void Exit(string sceneName = "ChooseSongs", bool isFinish = false, bool isRestart = false)
        {
            Debug.Log("Stage Exit.");
            Action callFunc = () =>
            {
                UISceneHelper.Instance.HideUi("PnlBattle");
                GameGlobal.gGameMusic.Stop();
                GameGlobal.gGameMusicScene.Stop();
                gTrigger.ClearEvent();
                GameGlobal.gTouch.ClearCustomEvent();
                GameGlobal.gCamera.gameObject.SetActive(false);
                SceneLoader.SetLoadInfo(ref sceneName);
                Time.timeScale = GameGlobal.TIME_SCALE;
                CommonPanel.GetInstance().SetMask(true, this.OnExit);
            };
            if (!isFinish && !isRestart)
            {
                AccountPhysicsManagerComponent.Instance.ChangePhysical((int)Host.Result(FormulaKeys.FORMULA_20), false,
                    true,
                    result =>
                    {
                        callFunc();
                    });
            }
            else
            {
                callFunc();
            }
        }

        private void OnExit()
        {
			PnlHome.PnlHome.backFromBattle = true;
            SceneManager.LoadScene(GameGlobal.LOADING_SCENE_NAME);
        }

        /// <summary>
        /// Init this instance.
        ///
        /// 关卡初始化入口
        /// </summary>
        public void Init()
        {
            StageTeachComponent.Instance.Init();

            Time.timeScale = GameGlobal.TIME_SCALE;
            this.prefabCatchObj = new Hashtable();

            GameGlobal.gGameMissPlay = new GameMissPlay();
            GameGlobal.gGameTouchPlay = new GameTouchPlay();
            GameGlobal.gGameMusic = new GameMusic();
            GameGlobal.gGameMusicScene = new GameMusicScene();

            FixUpdateTimer tGm = new FixUpdateTimer();
            FixUpdateTimer tStepGm = new FixUpdateTimer();
            FixUpdateTimer tStepOm = new FixUpdateTimer();
            FixUpdateTimer tGms = new FixUpdateTimer();
            GameGlobal.gGameMusic.SetTimer(ref tGm);
            GameGlobal.gGameMusic.SetStepTimer(ref tStepGm);
            GameGlobal.gGameMusic.SetSceneObjTimer(ref tStepOm);
            GameGlobal.gGameMusicScene.SetTimer(ref tGms);

            GameGlobal.gTouch.OnStart();

            string sceneName = this.GetSceneName();
            string musicName = this.GetMusicName();
            string fileName = this.GetStageName();
            this.Load(ref fileName, ref sceneName, ref musicName);

            TaskStageTarget.Instance.OnStageStarted();
            SkillComponent.Instance.Init();
            BattleRoleAttributeComponent.Instance.Init();
        }

        private void Load(ref string fileName, ref string sceneName, ref string audioName)
        {
            int sid = this.Host.GetDynamicIntByKey(SignKeys.ID);
            Debug.Log("Load stage (" + sid + ") " + sceneName + " ===== " + fileName + " ===== " + audioName);
            AudioManager.Instance.ReloadAllResource();
            //AudioManager.Instance.PlayingMusic = sceneName;
            AudioManager.Instance.SetBackGroundMusic(audioName);
            // For ensure music start with NO any delay.
            AudioManager.Instance.ResetBackGroundMusic();
            GameGlobal.gGameMusic.LoadMusicDataByFileName(ref sceneName, ref audioName);

            this.InitTimeNodeOrder();
            GameGlobal.gGameMusicScene.LoadScene((int)sid, sceneName);
        }

        private void InitTimeNodeOrder()
        {
            this._timeNodeOrder = new Dictionary<int, List<TimeNodeOrder>>();
            for (int i = 0; i < this.musicTickData.Count; i++)
            {
                MusicData md = (MusicData)this.musicTickData[i];
                if (md.nodeData.hit_type <= GameMusic.NONE || (md.nodeData.type == GameGlobal.NODE_TYPE_BOSS && i == 1))
                {
                    continue;
                }
                int s = (int)(md.tick / FixUpdateTimer.dInterval);
                if (!md.isLongPress)
                {
                    int aIvrtotal = (int)(md.nodeData.hitRangeA / FixUpdateTimer.dInterval);
                    var bIvrtotal = (int)(md.nodeData.hitRangeB / FixUpdateTimer.dInterval);
                    decimal aPerfectRange = md.nodeData.a_perfect_range;
                    decimal aGreatRange = md.nodeData.a_perfect_range + md.nodeData.a_great_range;
                    decimal bPerfectRange = md.nodeData.b_perfect_range;
                    decimal bGreatRange = md.nodeData.b_perfect_range + md.nodeData.b_great_range;
                    for (int j = 0; j <= aIvrtotal; j++)
                    {
                        TimeNodeOrder tno = new TimeNodeOrder();
                        tno.idx = md.objId;
                        tno.mustJump = md.nodeData.jump_note;
                        tno.isLongPressEnd = md.isLongPressEnd;
                        tno.isLongPressStart = md.isLongPressStart;
                        tno.isLongPress = md.isLongPress;
                        tno.enableJump = (md.nodeData.enable_jump == 1);
                        tno.result = GameMusic.PERFECT;
                        tno.isPerfectNode = j == 0;
                        tno.isLongPressEnd = md.isLongPressEnd;
                        decimal r = j * FixUpdateTimer.dInterval;
                        if (r > aPerfectRange)
                        {
                            tno.result = GameMusic.GREAT;
                        }
                        if (r > aGreatRange)
                        {
                            tno.result = GameMusic.COOL;
                        }
                        int tnoIdx = s + j;
                        if (!this._timeNodeOrder.ContainsKey(tnoIdx))
                        {
                            this._timeNodeOrder[tnoIdx] = new List<TimeNodeOrder>();
                        }

                        this._timeNodeOrder[tnoIdx].Add(tno);
                    }
                    for (int j = -bIvrtotal; j < 0; j++)
                    {
                        TimeNodeOrder tno = new TimeNodeOrder();
                        tno.idx = md.objId;
                        tno.mustJump = md.nodeData.jump_note;
                        tno.enableJump = (md.nodeData.enable_jump == 1);
                        tno.result = GameMusic.PERFECT;
                        tno.mustJump = md.nodeData.jump_note;
                        tno.isLongPressEnd = md.isLongPressEnd;
                        tno.isLongPressStart = md.isLongPressStart;
                        tno.isLongPress = md.isLongPress;
                        tno.isPerfectNode = false;
                        decimal _r = j * FixUpdateTimer.dInterval;
                        if (-_r > bPerfectRange)
                        {
                            tno.result = GameMusic.GREAT;
                        }
                        if (-_r > bGreatRange)
                        {
                            tno.result = GameMusic.COOL;
                        }
                        int tnoIdx = s + j;
                        if (!this._timeNodeOrder.ContainsKey(tnoIdx))
                        {
                            this._timeNodeOrder[tnoIdx] = new List<TimeNodeOrder>();
                        }

                        this._timeNodeOrder[tnoIdx].Add(tno);
                    }
                }
                else
                {
                    TimeNodeOrder tno = new TimeNodeOrder();
                    tno.idx = md.objId;
                    tno.mustJump = md.nodeData.jump_note;
                    tno.enableJump = (md.nodeData.enable_jump == 1);
                    tno.result = GameMusic.GREAT;
                    tno.isPerfectNode = true;
                    tno.mustJump = md.nodeData.jump_note;
                    tno.isLongPressEnd = md.isLongPressEnd;
                    tno.isLongPressStart = md.isLongPressStart;
                    tno.isLongPress = md.isLongPress;
                    var tnoIdx = s;
                    if (!this._timeNodeOrder.ContainsKey(tnoIdx))
                    {
                        this._timeNodeOrder[tnoIdx] = new List<TimeNodeOrder>();
                    }

                    this._timeNodeOrder[tnoIdx].Add(tno);
                }
            }
        }

        private void InitData()
        {
            this.LoadMusicData();
            foreach (object t in this.musicTickData)
            {
                MusicData md = (MusicData)t;
                AudioManager.Instance.AddAudioResource(md.nodeData.key_audio);
            }
            AudioManager.Instance.AddEmptyAudio();
        }

        private void LoadMusicData()
        {
            string name = this.GetStageName();
            string cfgName = name + 1;

            this.musicTickData = MusicConfigReader.Instance.GetData(ref cfgName);
            if (this.musicTickData == null || this.musicTickData.Count <= 0)
            {
                Debug.Log("Stage config is lost : " + cfgName);
                cfgName = name + 1;
                this.musicTickData = MusicConfigReader.Instance.GetData(ref cfgName);
            }
        }

        public void AddGold(int value)
        {
            int _gold = this.Host.GetDynamicIntByKey(SignKeys.GOLD) + value;
            this.Host.SetDynamicData(SignKeys.GOLD, _gold);
        }

        public List<TimeNodeOrder> GetTimeNodeByTick(decimal tick)
        {
            int _t = (int)(tick / FixUpdateTimer.dInterval);
            if (!this._timeNodeOrder.ContainsKey(_t))
            {
                return null;
            }

            return this._timeNodeOrder[_t];
        }

        public int GetPerfectIdxByTick(decimal tick)
        {
            var t = (int)(tick / FixUpdateTimer.dInterval);
            var t1 = t;
            while (!(this._timeNodeOrder.ContainsKey(t1) && this._timeNodeOrder[t1].Exists(n => n.isPerfectNode)) && t1 < 20000)
            {
                t1++;
            }
            var t2 = t;
            while (!(this._timeNodeOrder.ContainsKey(t2) && this._timeNodeOrder[t2].Exists(n => n.isPerfectNode)) && t2 > 0)
            {
                t2--;
            }
            if (Mathf.Abs(t - t1) < Mathf.Abs(t - t2))
            {
                return t1;
            }
            return t2;
        }

        public int GetStagePhysical()
        {
            return 0;
        }

        public string GetSatageAuthor()
        {
            this.Host.Result(FormulaKeys.FORMULA_7);
            return this.Host.GetDynamicStrByKey(SignKeys.AUTHOR);
        }
    }
}