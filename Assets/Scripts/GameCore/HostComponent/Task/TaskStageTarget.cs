using Assets.Scripts.Common.Manager;
using GameLogic;
using LitJson;

///自定义模块，可定制模块具体行为
using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Tools.Managers;
using UnityEngine;

namespace FormulaBase
{
    public class TaskStageTarget : CustomComponentBase
    {
        private static TaskStageTarget instance = null;
        private const int HOST_IDX = 14;
        public static bool isNextUnlock = false;
        public static int nextUnlockIdx = 1;
        public static bool isChange = false;
        public static bool isInEarphone = true;

        public static TaskStageTarget Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new TaskStageTarget();
                }
                return instance;
            }
        }

        // ------------------------------------------------------
        // 关卡目标数据缓存
        public struct Target
        {
            public int id;
            public int value;
            public string strValue;
            public string signKey;
            public bool result;
            public bool dymResult;
            public bool dymFailed;
        }

        private const string CFG_HEAD_VALUE = "goal_";

        // 下面所有统计值的历史最大值后缀
        public const string TASK_SIGNKEY_COUNT_MAX_TAIL = "_XMAX";

        // 旧最大值，用于结算比较，不做存储
        private const string TASK_SIGNKEY_COUNT_OLD_MAX_TAIL = "_XOLDMAX";

        // 统计值都是基于加值，带_LESSTHAN的表示取反值
        public const string TASK_SIGNKEY_COUNT_LESSTHAN_TAIL = "_LESSTHAN";

        // 目标值
        public const string TASK_SIGNKEY_COUNT_TARGET_TAIL = "_TARGET";

        // 关卡id，对应stage表
        public const string TASK_SIGNKEY_STAGEID = "TASK_SIGNKEY_STAGEID";

        /// <summary>
        /// node点有效打击次数统计，后缀对应nodedata配置索引
        /// </summary>
        public const string TASK_SIGNKEY_NODE_IDX_HEAD = "NDX_";

        /// <summary>
        /// 打击评价统计，后缀对应评价值，有效跳跃为6
        /// </summary>
        public const string TASK_SIGNKEY_EVLUATE_HEAD = "PLAY_EVLUATE_";

        /// <summary>
        /// 关卡评价
        /// </summary>
        public const string TASK_SIGNKEY_STAGE_EVLUATE = "TASK_SIGNKEY_STAGE_EVLUATE";

        /// <summary>
        /// 吃能量包个数统计
        /// </summary>
        public const string TASK_SIGNKEY_ENERGY_ITEM_COUNT = "TASK_SIGNKEY_ENERGY_ITEM_COUNT";

        /// <summary>
        /// 吃金币个数统计
        /// </summary>
        public const string TASK_SIGNKEY_GOLD_ITEM_COUNT = "TASK_SIGNKEY_GOLD_ITEM_COUNT";

        /// <summary>
        /// 吃音符个数统计
        /// </summary>
        public const string TASK_SIGNKEY_NOTE_ITEM_COUNT = "TASK_SIGNKEY_NOTE_ITEM_COUNT";

        /// <summary>
        /// 挥空次数统计
        /// </summary>
        public const string TASK_SIGNKEY_HITEMPTY_ITEM_COUNT = "TASK_SIGNKEY_HITEMPTY_ITEM_COUNT";

        /// <summary>
        /// 暴击次数统计
        /// </summary>
        public const string TASK_SIGNKEY_CRT_COUNT = "TASK_SIGNKEY_CRT_COUNT";

        /// <summary>
        /// 有效打击次数统计
        /// </summary>
        public const string TASK_SIGNKEY_HIT_COUNT = "TASK_SIGNKEY_HIT_COUNT";

        /// <summary>
        /// 跳跃次数统计
        /// </summary>
        public const string TASK_SIGNKEY_JUMP_COUNT = "TASK_SIGNKEY_JUMP_COUNT";

        /// <summary>
        /// 打中boss攻击次数统计
        /// </summary>
        public const string TASK_SIGNKEY_HIT_BOSS_COUNT = "TASK_SIGNKEY_HIT_BOSS_COUNT";

        /// <summary>
        /// 跳过boss攻击次数统计
        /// </summary>
        public const string TASK_SIGNKEY_JUMPOVER_BOSS_COUNT = "TASK_SIGNKEY_JUMPOVER_BOSS_COUNT";

        /// <summary>
        /// 有效成功次数统计
        /// </summary>
        public const string TASK_SIGNKEY_LONGPRESS_FIN_COUNT = "TASK_SIGNKEY_LONGPRESS_FIN_COUNT";

        /// <summary>
        /// 最大combo
        /// </summary>
        public const string TASK_SIGNKEY_MAX_COMBO = "TASK_SIGNKEY_MAX_COMBO";

        /// <summary>
        /// 关卡总得分
        /// </summary>
        public const string TASK_SIGNKEY_SCORE = "TASK_SIGNKEY_SCORE";

        /// <summary>
        /// 回复量统计
        /// </summary>
        public const string TASK_SIGNKEY_RECOVER = "TASK_SIGNKEY_RECOVER";

        /// <summary>
        /// 最大单击伤害（分数）
        /// </summary>
        public const string TASK_SIGNKEY_MAX_DAMAGE = "TASK_SIGNKEY_MAX_DAMAGE";

        /// <summary>
        /// 僚机数量统计
        /// </summary>
        public const string TASK_SIGNKEY_ARM_COUNT = "TASK_SIGNKEY_ARM_COUNT";

        /// <summary>
        /// HP余量比率
        /// </summary>
        public const string TASK_SIGNKEY_HP_RATE_COUNT = "TASK_SIGNKEY_HP_RATE_COUNT";

        /// <summary>
        /// 通关次数统计
        /// </summary>
        public const string TASK_SIGNKEY_STAGE_CLEAR_COUNT = "TASK_SIGNKEY_STAGE_CLEAR_COUNT";

        /// <summary>
        /// 隐藏node统计
        /// </summary>
        public const string TASK_SIGNKEY_HIDE_NODE_COUNT = "TASK_SIGNKEY_HIDE_NODE_COUNT";

        /// <summary>
        /// 歌曲解锁事件
        /// </summary>
        public static Action<int> onSongUnlock;

        public void Init(int stageId)
        {
            this.Host = this.GetTaskByStageId(stageId);
            if (this.Host == null)
            {
                string fileName = FomulaHostManager.Instance.GetFileNameByHostType(HOST_IDX);
                this.Host = FomulaHostManager.Instance.LoadHost(fileName);
            }

            int diff = this.Host.GetDynamicIntByKey(SignKeys.DIFFCULT);
            if (diff < GameGlobal.DIFF_LEVEL_NORMAL)
            {
                diff = GameGlobal.DIFF_LEVEL_NORMAL;
                this.Host.SetDynamicData(SignKeys.DIFFCULT, GameGlobal.DIFF_LEVEL_NORMAL);
            }

            int lockValue = this.Host.GetDynamicIntByKey(SignKeys.LOCKED, -1);
            if (lockValue < 0)
            {
                var isLock = IsLock() ? 1 : 0;
                this.Host.SetDynamicData(SignKeys.LOCKED, isLock);
            }

            this.Host.SetDynamicData(SignKeys.ID, stageId);
            this.Host.SetDynamicData(TASK_SIGNKEY_STAGEID, stageId);
            this.OnInit(stageId);
        }

        public string[] GetAllMusicNames()
        {
            var names = new List<string>();
            var musicName = ConfigManager.instance["stage"];
            for (int i = 0; i < musicName.Count; i++)
            {
                var name = musicName[i]["name"].ToString();
                names.Add(name);
            }
            return names.ToArray();
        }

        public FormulaHost GetTask(int idx)
        {
            return (HostList ?? GetList("Task")).Values.ToList().Single(h => h.GetDynamicIntByKey(SignKeys.ID) == idx);
        }

        /// <summary>
        /// 获取当前玩家获得的奖杯总数
        /// </summary>
        /// <returns></returns>
        public int GetTotalTrophy()
        {
            var hostList = HostList ?? GetList("Task");
            return hostList.Sum(host => host.Value.GetDynamicIntByKey(TaskStageTarget.TASK_SIGNKEY_STAGE_EVLUATE + TaskStageTarget.TASK_SIGNKEY_COUNT_MAX_TAIL));
        }

        /// <summary>
        /// Raises the init event.
        ///
        /// 关卡目标数据初始化后的界面设置
        /// </summary>
        /// <param name="stageId">Stage identifier.</param>
        private void OnInit(int stageId)
        {
            //this.OnTargetCheck (0);
            this.Host.SetAsUINotifyInstance();
        }

        /// <summary>
        /// Determines whether this instance is lock.
        /// 本关卡锁定状态
        /// 关卡1默认开启
        /// </summary>
        /// <returns><c>true</c> if this instance is lock; otherwise, <c>false</c>.</returns>
        public bool IsLock()
        {
            if (this.GetId() <= 1)
            {
                return false;
            }
            var unlockTrophy = Host.GetDynamicIntByKey(SignKeys.UnlockTrophy);
            return unlockTrophy > GetTotalTrophy();
        }

        /// <summary>
        /// 根据所给索引遍历判断是否解锁，不要在遍历中使用
        /// </summary>
        /// <param name="stageId"></param>
        /// <returns></returns>
        public bool IsLock(int stageId)
        {
            if (stageId <= 1)
            {
                return false;
            }
            var hosts = GetList("Task");
            foreach (var formulaHost in hosts.Values)
            {
                if (formulaHost.GetDynamicIntByKey(SignKeys.ID) == stageId)
                {
                    var unlockTrophy = formulaHost.GetDynamicIntByKey(SignKeys.UnlockTrophy);
                    return unlockTrophy > GetTotalTrophy();
                }
            }
            return true;
        }

        public bool IsUnLockAllDiff(FormulaHost host = null)
        {
            host = host ?? this.Host;
            if (host == null) return false;
            return host.GetDynamicIntByKey(SignKeys.DIFFCULT) > 3;
        }

        public bool IsAchieve(FormulaHost host = null)
        {
            host = host ?? this.Host;
            return
                host.GetDynamicIntByKey(TaskStageTarget.TASK_SIGNKEY_SCORE) >=
                host.GetDynamicIntByKey(TaskStageTarget.TASK_SIGNKEY_SCORE +
                                        TaskStageTarget.TASK_SIGNKEY_COUNT_TARGET_TAIL);
        }

        public FormulaHost GetStageByIdx(int idx)
        {
            var hostList = HostList ?? GetList("Task");
            return hostList.Values.ToList().Find(formulaHost => formulaHost.GetDynamicIntByKey(SignKeys.ID) == idx);
        }

        /// <summary>
        /// 获取解锁歌曲列表
        /// </summary>
        /// <returns></returns>
        public bool[] GetLockList()
        {
            var idxs = new bool[StageBattleComponent.Instance.GetStageCount()];
            var accountLvl = int.Parse(CallbackManager.instance.GetValue("Account_Level").ToString());
            for (int i = 0; i < idxs.Length; i++)
            {
                var unlockLvl = (int)ConfigManager.instance["stage"][i]["unlock_level"];
                idxs[i] = unlockLvl > accountLvl;
                if (GameGlobal.IS_UNLOCK_ALL_STAGE)
                {
                    idxs[i] = false;
                }
            }
            return idxs;
        }

        /// <summary>
        /// 获取解锁下一首歌曲所需奖杯值
        /// </summary>
        /// <returns></returns>
        public int GetNextUnlockTrophy(ref int idx)
        {
            var trophyTotal = GetTotalTrophy();
            var trophyRequest = 0;
            for (int i = 1; trophyRequest < trophyTotal; i++)
            {
                trophyRequest = ConfigManager.instance.GetConfigIntValue("stage", i, "unlock_Lev");
                idx = i;
            }
            return trophyRequest;
        }

        public int GetScoreTarget()
        {
            return this.Host.GetDynamicIntByKey(TaskStageTarget.TASK_SIGNKEY_SCORE + TaskStageTarget.TASK_SIGNKEY_COUNT_TARGET_TAIL);
        }

        public float GetStageRewardRank()
        {
            // rank 来自 score和performance的比率 : 1, 0.8, 0.6, 0.4, 0.2
            var stageHost = this.Host;
            var performanceScore = ConfigManager.instance.GetConfigIntValue("stage", stageHost.GetDynamicIntByKey(SignKeys.ID), "performance");
            var rank = ((float)stageHost.GetDynamicIntByKey(TASK_SIGNKEY_MAX_COMBO) * 0.5f + (float)stageHost.GetDynamicIntByKey(TASK_SIGNKEY_HIDE_NODE_COUNT) * 0.1f + (float)stageHost.GetDynamicIntByKey(TaskStageTarget.TASK_SIGNKEY_EVLUATE_HEAD + GameMusic.PERFECT) * 0.4f) / performanceScore;
            return rank;
        }

        public string GetStagePJ()
        {
            var rank = GetStageRewardRank();
            if (rank <= 0.2f)
            {
                return "d";
            }
            else if (rank <= 0.4f)
            {
                return "c";
            }
            else if (rank <= 0.6f)
            {
                return "b";
            }
            else if (rank <= 0.8f)
            {
                return "a";
            }
            else
            {
                return "s";
            }
        }

        public void OnStageStarted()
        {
            // Only reset not _XMAX value
            Dictionary<string, object> _signs = this.Host.GetSigns();
            if (_signs == null || _signs.Keys == null || _signs.Count <= 0)
            {
                return;
            }
            /*
			List<string> _keys = new List<string> (_signs.Keys);
			for (int i = 0; i < _keys.Count; i++) {
				string sKey = _keys [i];
				if (sKey.Contains (TASK_SIGNKEY_COUNT_TARGET_TAIL)) {
					continue;
				}

				if (sKey.Contains (TASK_SIGNKEY_COUNT_MAX_TAIL)) {
					//string oldMaxKey = sKey + TASK_SIGNKEY_COUNT_OLD_MAX_TAIL;
					//this.Host.SetDynamicData (oldMaxKey, this.Host.GetDynamicObjByKey (sKey));
					continue;
				}

				this.Host.SetDynamicData (sKey, 0);
			}
			*/
            this.Host.SetDynamicData(TASK_SIGNKEY_SCORE, 0);
            this.Host.SetDynamicData(TASK_SIGNKEY_EVLUATE_HEAD + GameMusic.PERFECT, 0);
            this.Host.SetDynamicData(TASK_SIGNKEY_HIDE_NODE_COUNT, 0);
            this.Host.SetDynamicData(TASK_SIGNKEY_MAX_COMBO, 0);
            isInEarphone = true;
        }

        public bool OnStageFinished()
        {
            if (this.Host == null)
            {
                return false;
            }

            this.AddStageClearCount(1);

            var score = GetScore();
            var scoreTarget = GetScoreTarget();
            isChange = false;
            int diff = this.Host.GetDynamicIntByKey(SignKeys.DIFFCULT);

            //日常任务相关
            //在任意关卡中获得1次S评价
            TaskManager.instance.DetectValue(s => GetStagePJ() == "s", TaskManager.S_IDX);
            //在关卡（MusicName）中获得1次S评价或a评价
            TaskManager.instance.DetectValue(s => Host.GetDynamicIntByKey(SignKeys.ID) == int.Parse(s) && (GetStagePJ() == "a" || GetStagePJ() == "s"), TaskManager.A_IDX);
            //戴耳机完成任意5个不用关卡
            if (isInEarphone)
            {
                TaskManager.instance.AddValue(1, TaskManager.EARPHONE_IDX);
            }
            //累计收集30个音符
            TaskManager.instance.AddValue(GetNoteItemCount(), TaskManager.MUSIC_NOTE_IDX);
            //累计收集N个金币
            TaskManager.instance.AddValue(GetGoldItemCount(), TaskManager.COIN_IDX);
            //累计击打500个perfect
            TaskManager.instance.AddValue(Host.GetDynamicIntByKey(TaskStageTarget.TASK_SIGNKEY_EVLUATE_HEAD + GameMusic.PERFECT + TaskStageTarget.TASK_SIGNKEY_COUNT_MAX_TAIL), TaskManager.PERFECT_IDX);
            //在任意关卡获得N连击数
            TaskManager.instance.DetectValue(s => GetComboMax() >= int.Parse(s), TaskManager.COMBO_IDX);
            //累计击中N个会隐形的星星
            TaskManager.instance.AddValue(GetHideNodeCount(), TaskManager.HIDE_NOTE_IDX);

            // 完成难度分数目标后，自动增加难度，同时增加奖杯
            if (score >= scoreTarget && diff <= 3)
            {
                isChange = true;

                this.Host.SetDynamicData(SignKeys.DIFFCULT, ++diff);

                int evlua = this.GetStageEvluateMax();
                Debug.Log("evlua" + evlua);
                this.SetStageEvluateMax(evlua + 1);
                return true;
            }
            return false;

            /*
			if (this.targets == null || this.targets.Length <= 0) {
				return;
			}

			for (int i = 0; i < this.targets.Length; i++) {
				Target _t = this.targets [i];
				if (_t.signKey == null) {
					continue;
				}

				if (!_t.signKey.Contains (TASK_SIGNKEY_COUNT_LESSTHAN_TAIL)) {
					continue;
				}

				string _normalKey = _t.signKey.Replace (TASK_SIGNKEY_COUNT_LESSTHAN_TAIL, "");
				string _lessMaxKey = _t.signKey + TASK_SIGNKEY_COUNT_MAX_TAIL;
				int currentLess = this.Host.GetDynamicIntByKey (_normalKey, 0);
				int recordLess = this.Host.GetDynamicIntByKey (_lessMaxKey, GameGlobal.LIMITE_INT);
				int less = Math.Min (currentLess, recordLess);
				this.Host.SetDynamicData (_lessMaxKey, less);
				Debug.Log ("Stage finished less than check : " + _lessMaxKey + " " + less);
			}
			*/
        }

        public void Save(int stageId = -1)
        {
            FormulaHost _task = null;
            if (stageId < 0)
            {
                _task = this.Host;
            }
            else
            {
                _task = this.GetTaskByStageId(stageId);
            }

            if (_task == null)
            {
                return;
            }

            if (_task.GetSigns() == null)
            {
                return;
            }

            List<string> _signKeys = new List<string>(_task.GetSigns().Keys);
            if (_signKeys == null || _signKeys.Count <= 0)
            {
                return;
            }

            for (int i = 0; i < _signKeys.Count; i++)
            {
                string _sk = _signKeys[i];
                if (_sk == null)
                {
                    continue;
                }

                if (!_sk.Contains(TASK_SIGNKEY_COUNT_OLD_MAX_TAIL))
                {
                    continue;
                }

                _task.RemoveDynamicData(_sk);
            }

            _task.Save(new HttpResponseDelegate(this.AfterSave));
        }

        private void AfterSave(bool _Success)
        {
            return;
            //检验下首歌曲是否解锁
            if (isChange)
            {
                var trophyNext = GetNextUnlockTrophy(ref nextUnlockIdx);
                var trophyTotal = GetTotalTrophy();
                isNextUnlock = trophyTotal == trophyNext;
            }
        }

        public bool Contains(int idx)
        {
            var hostList = HostList ?? GetList("Task");
            return hostList.Values.ToList().Any(k => k.GetDynamicIntByKey(SignKeys.ID) == idx);
        }

        /// <summary>
        /// Targets the string value.
        ///
        /// 部分关卡目标需要额外附带一些文字描述信息
        /// </summary>
        /// <returns>The string value.</returns>
        /// <param name="key">Key.</param>
        /// <param name="value">Value.</param>
        private string TargetStrValue(string key, int value)
        {
            if (key == null)
            {
                return null;
            }

            if (key == TASK_SIGNKEY_STAGE_EVLUATE)
            {
                if (value >= 0 && value < GameGlobal.STAGE_EVLUATE_MAP.Length)
                {
                    return GameGlobal.STAGE_EVLUATE_MAP[value];
                }

                return null;
            }

            return null;
        }

        /// <summary>
        /// Gets the stage targets by stage identifier.
        ///
        /// 利用各个记录项目的_XMAX值进行比较以获得关卡任务目标完成状况
        /// </summary>
        /// <returns>The stage targets by stage identifier.</returns>
        /// <param name="stageId">Stage identifier.</param>
        public Target[] GetStageTargetsByStageId(int stageId)
        {
            FormulaHost _task = this.GetTaskByStageId(stageId);
            if (_task == null)
            {
                return null;
            }

            // 只放置单一难度的关卡分数目标
            int diff = this.Host.GetDynamicIntByKey(SignKeys.DIFFCULT);
            Debug.Log("获得关卡" + stageId + "任务目标完成状况 ");
            Target[] targets = new Target[GameGlobal.DIFF_LEVEL_SUPER];
            for (int i = GameGlobal.DIFF_LEVEL_NORMAL; i < GameGlobal.DIFF_LEVEL_SUPER; i++)
            {
                if (i != diff)
                {
                    continue;
                }

                Target _t = new Target();
                int targetIdx = i + 1;
                int goalValue = ConfigManager.instance.GetConfigIntValue("stage", "id", CFG_HEAD_VALUE + targetIdx,
                    stageId);

                string maxSignKey = null;
                bool isLessThan = false;
                // string signKey = ConfigPool.Instance.GetConfigStringValue ("Level_Goal", goalId.ToString (), "signkey");
                string signKey = TASK_SIGNKEY_SCORE;
                if (signKey != null)
                {
                    maxSignKey = signKey + TASK_SIGNKEY_COUNT_MAX_TAIL;
                    if (signKey.Contains(TASK_SIGNKEY_COUNT_LESSTHAN_TAIL))
                    {
                        isLessThan = true;
                    }
                }

                _t.id = targetIdx;
                _t.value = goalValue;
                _t.signKey = signKey;
                _t.strValue = this.TargetStrValue(signKey, goalValue);
                _t.dymResult = false;
                _t.dymFailed = false;
                _t.result = this.IsTaskFinished(stageId, maxSignKey, goalValue, isLessThan);

                targets[i] = _t;

                // string des = ConfigPool.Instance.GetConfigStringValue ("Level_Goal", goalId.ToString (), "goal");
                Debug.Log("  : 分数达到 " + _t.result);
            }

            return targets;
        }

        private FormulaHost GetTaskByStageId(int stageId)
        {
            string fileName = FomulaHostManager.Instance.GetFileNameByHostType(HOST_IDX);
            Dictionary<string, FormulaHost> tasks = FomulaHostManager.Instance.GetHostListByFileName(fileName);
            if (tasks == null)
            {
                tasks = FomulaHostManager.Instance.AddHostListByFileName(fileName);
                if (tasks == null)
                {
                    return FomulaHostManager.Instance.CreateHost(HOST_IDX);
                }
            }

            foreach (FormulaHost _task in tasks.Values)
            {
                if (_task.GetDynamicIntByKey(TASK_SIGNKEY_STAGEID, -1) == stageId)
                {
                    return _task;
                }
            }

            return FomulaHostManager.Instance.CreateHost(HOST_IDX);
        }

        /// ----------------------------  目标达成判断  ----------------------------

        /// <summary>
        /// Determines whether this instance is task finished the specified stageId checkKey checkValue.
        /// </summary>
        /// <returns><c>true</c> if this instance is task finished the specified stageId checkKey checkValue; otherwise, <c>false</c>.</returns>
        /// <param name="stageId">Stage identifier.</param>
        /// <param name="checkKey">Check key.</param>
        /// <param name="checkValue">Check value.</param>
        public bool IsTaskFinished(int stageId, string checkKey, int checkValue, bool isLessThan = false)
        {
            FormulaHost task = this.GetTaskByStageId(stageId);
            if (task == null)
            {
                return false;
            }

            if (isLessThan)
            {
                // 未通关
                if (task.GetDynamicIntByKey(TASK_SIGNKEY_STAGE_CLEAR_COUNT + TASK_SIGNKEY_COUNT_MAX_TAIL, -1) <= 0)
                {
                    return false;
                }

                return task.GetDynamicIntByKey(checkKey) <= checkValue;
            }

            return task.GetDynamicIntByKey(checkKey, -1) >= checkValue;
        }

        /// <summary>
        /// Determines whether this instance is task finished the specified stageId checkKey checkValue.
        /// </summary>
        /// <returns><c>true</c> if this instance is task finished the specified stageId checkKey checkValue; otherwise, <c>false</c>.</returns>
        /// <param name="stageId">Stage identifier.</param>
        /// <param name="checkKey">Check key.</param>
        /// <param name="checkValue">Check value.</param>
        public bool IsTaskFinished(int stageId, string checkKey, string checkValue, bool isLessThan = false)
        {
            FormulaHost task = this.GetTaskByStageId(stageId);
            if (task == null)
            {
                return false;
            }

            if (isLessThan)
            {
                // 未通关
                if (task.GetDynamicIntByKey(TASK_SIGNKEY_STAGE_CLEAR_COUNT + TASK_SIGNKEY_COUNT_MAX_TAIL, -1) <= 0)
                {
                    return false;
                }
            }

            return task.GetDynamicStrByKey(checkKey) == checkValue;
        }

        /// <summary>
        /// Determines whether this instance is task finished the specified checkKey checkValue.
        /// </summary>
        /// <returns><c>true</c> if this instance is task finished the specified checkKey checkValue; otherwise, <c>false</c>.</returns>
        /// <param name="checkKey">Check key.</param>
        /// <param name="checkValue">Check value.</param>
        public bool IsTaskFinished(string checkKey, int checkValue, bool isLessThan = false)
        {
            if (this.Host == null)
            {
                return false;
            }

            if (isLessThan)
            {
                // 未通关
                if (this.Host.GetDynamicIntByKey(TASK_SIGNKEY_STAGE_CLEAR_COUNT + TASK_SIGNKEY_COUNT_MAX_TAIL, -1) <= 0)
                {
                    return false;
                }

                return this.Host.GetDynamicIntByKey(checkKey) <= checkValue;
            }

            return this.Host.GetDynamicIntByKey(checkKey, -1) >= checkValue;
        }

        /// <summary>
        /// Determines whether this instance is task finished the specified checkKey checkValue.
        /// </summary>
        /// <returns><c>true</c> if this instance is task finished the specified checkKey checkValue; otherwise, <c>false</c>.</returns>
        /// <param name="checkKey">Check key.</param>
        /// <param name="checkValue">Check value.</param>
        public bool IsTaskFinished(string checkKey, string checkValue, bool isLessThan = false)
        {
            if (this.Host == null)
            {
                return false;
            }

            if (isLessThan)
            {
                // 未通关
                if (this.Host.GetDynamicIntByKey(TASK_SIGNKEY_STAGE_CLEAR_COUNT + TASK_SIGNKEY_COUNT_MAX_TAIL, -1) <= 0)
                {
                    return false;
                }
            }

            return this.Host.GetDynamicStrByKey(checkKey) == checkValue;
        }

        /// <summary>
        /// Gets the X max.
        ///
        /// 获取当前关卡对象某个统计值的历史最大值
        /// </summary>
        /// <returns>The X max.</returns>
        /// <param name="signKey">Sign key.</param>
        public int GetXMax(string signKey, int defaultValue = 0)
        {
            if (this.Host == null)
            {
                return defaultValue;
            }

            return this.Host.GetDynamicIntByKey(signKey + TASK_SIGNKEY_COUNT_MAX_TAIL, defaultValue);
        }

        /// <summary>
        /// Gets the old X max.
        ///
        /// 获取当前关卡对象某个统计值的缓存最大值
        /// </summary>
        /// <returns>The old X max.</returns>
        /// <param name="signKey">Sign key.</param>
        /// <param name="defaultValue">Default value.</param>
        public int GetOldXMax(string signKey, int defaultValue = 0)
        {
            if (this.Host == null)
            {
                return defaultValue;
            }

            return this.Host.GetDynamicIntByKey(signKey + TASK_SIGNKEY_COUNT_MAX_TAIL + TASK_SIGNKEY_COUNT_OLD_MAX_TAIL, defaultValue);
        }

        /// <summary>
        /// Gets the X less.
        ///
        /// 获取当前关卡对象某个统计值的历史最小值
        /// </summary>
        /// <returns>The X less.</returns>
        /// <param name="signKey">Sign key.</param>
        public int GetXLess(string signKey, int defaultValue = 0)
        {
            if (this.Host == null)
            {
                return defaultValue;
            }

            return this.Host.GetDynamicIntByKey(signKey + TASK_SIGNKEY_COUNT_LESSTHAN_TAIL + TASK_SIGNKEY_COUNT_MAX_TAIL, defaultValue);
        }

        /// <summary>
        /// Gets the old X less.
        ///
        /// 获取当前关卡对象某个统计值的缓存最小值
        /// </summary>
        /// <returns>The old X less.</returns>
        /// <param name="signKey">Sign key.</param>
        /// <param name="defaultValue">Default value.</param>
        public int GetOldXLess(string signKey, int defaultValue = 0)
        {
            if (this.Host == null)
            {
                return defaultValue;
            }

            return this.Host.GetDynamicIntByKey(signKey + TASK_SIGNKEY_COUNT_LESSTHAN_TAIL + TASK_SIGNKEY_COUNT_MAX_TAIL + TASK_SIGNKEY_COUNT_OLD_MAX_TAIL, defaultValue);
        }

        /// ----------------------------  以下是统计行为  ----------------------------
        /// <summary>
        /// Sets the play result.
        /// </summary>
        /// <param name="idx">Index.</param>
        /// <param name="result">Result.</param>
        public void SetPlayResult(int idx, uint result)
        {
            if (result <= GameMusic.NONE)
            {
                return;
            }

            int hittedAdd = 0;
            if (result > GameMusic.MISS)
            {
                hittedAdd = 1;
            }

            int nodeType = BattleEnemyManager.Instance.GetNodeTypeByIdx(idx);

            // 打击评价次数记录
            string key = TASK_SIGNKEY_EVLUATE_HEAD + result;
            if (result == GameMusic.MISS && GameGlobal.NODE_TYPES_NO_MISS.Contains((uint)nodeType))
            {
                // 某些node类型miss不算伤害
            }
            else
            {
                this.AddCount(key, 1);
            }

            // 有效打击node类型次数记录 有效打击总次数
            if (hittedAdd > 0)
            {
                string nodeId = BattleEnemyManager.Instance.GetNodeUidByIdx(idx);
                string ntKey = TASK_SIGNKEY_NODE_IDX_HEAD + nodeId;
                this.AddCount(ntKey, hittedAdd);
                this.AddCount(TASK_SIGNKEY_HIT_COUNT, hittedAdd);

                if (nodeType == GameGlobal.NODE_TYPE_HIDE)
                {
                    this.AddHideNodeCount(hittedAdd);
                }

                //int bossActionType = BattleEnemyManager.Instance.GetNodeBossActionTypeByIdx (idx);
                //if (bossActionType == GameMusic.NODE_IS_BOSS_BODY_ATTACK) {
                //	this.AddCount (TASK_SIGNKEY_HIT_BOSS_COUNT, hittedAdd);
                //}

                //if (bossActionType == GameMusic.NODE_IS_BOSS_THROW_ATTACK) {
                //	this.AddCount (TASK_SIGNKEY_JUMPOVER_BOSS_COUNT, hittedAdd);
                //}
            }
        }

        /// <summary>
        /// Adds the item count.
        ///
        /// 增减数量计数，同时自动更新历史最大获取数
        /// </summary>
        /// <param name="key">Key.</param>
        /// <param name="value">Value.</param>
        private void AddCount(string key, int value)
        {
            if (this.Host == null)
            {
                return;
            }

            int c = this.Host.GetDynamicIntByKey(key) + value;
            this.SetCount(key, c);
        }

        private void SetCount(string key, int value)
        {
            this.Host.SetDynamicData(key, value);
            int sid = this.Host.GetDynamicIntByKey(TASK_SIGNKEY_STAGEID);
            // Add less than max count record.
            string lessThanMaxKey = key + TASK_SIGNKEY_COUNT_LESSTHAN_TAIL + TASK_SIGNKEY_COUNT_MAX_TAIL;
            int ltMax = this.Host.GetDynamicIntByKey(lessThanMaxKey, GameGlobal.LIMITE_INT);
            //Debug.Log ("-->> " + lessThanMaxKey + " " + ltMax + " value " + value);
            if (value < ltMax)
            {
                this.Host.SetDynamicData(lessThanMaxKey, value);
            }

            // Add max count record.
            string maxKey = key + TASK_SIGNKEY_COUNT_MAX_TAIL;
            int cMax = this.Host.GetDynamicIntByKey(maxKey);
            if (value < cMax)
            {
                return;
            }

            this.Host.SetDynamicData(maxKey, value);
        }

        /// <summary>
        /// Gets the boss hit count.
        /// </summary>
        /// <returns>The boss hit count.</returns>
        public int GetBossHitCount()
        {
            if (this.Host == null)
            {
                return 0;
            }

            return this.Host.GetDynamicIntByKey(TASK_SIGNKEY_HIT_BOSS_COUNT);
        }

        /// <summary>
        /// Gets the hit count.
        /// </summary>
        /// <returns>The hit count.</returns>
        public int GetHitCount()
        {
            if (this.Host == null)
            {
                return 0;
            }

            return this.Host.GetDynamicIntByKey(TASK_SIGNKEY_HIT_COUNT);
        }

        /// <summary>
        /// Gets the hit count by result.
        /// </summary>
        /// <returns>The hit count by result.</returns>
        /// <param name="result">Result.</param>
        public int GetHitCountByResult(uint result)
        {
            if (this.Host == null)
            {
                return 0;
            }

            string key = TASK_SIGNKEY_EVLUATE_HEAD + result;
            return this.Host.GetDynamicIntByKey(key);
        }

        /// <summary>
        /// Gets the index of the hit count by node.
        /// </summary>
        /// <returns>The hit count by node index.</returns>
        /// <param name="idx">Index.</param>
        public int GetHitCountByNodeIdx(int idx)
        {
            if (this.Host == null)
            {
                return 0;
            }

            string key = TASK_SIGNKEY_NODE_IDX_HEAD + idx;
            return this.Host.GetDynamicIntByKey(key);
        }

        /// <summary>
        /// Adds the energy item count.
        /// </summary>
        /// <param name="value">Value.</param>
        public void AddEnergyItemCount(int value)
        {
            this.AddCount(TASK_SIGNKEY_ENERGY_ITEM_COUNT, value);
        }

        /// <summary>
        /// Gets the energy item count.
        /// </summary>
        /// <returns>The energy item count.</returns>
        public int GetEnergyItemCount()
        {
            if (this.Host == null)
            {
                return 0;
            }

            return this.Host.GetDynamicIntByKey(TASK_SIGNKEY_ENERGY_ITEM_COUNT);
        }

        /// <summary>
        /// Adds the gold item count.
        /// </summary>
        /// <param name="value">Value.</param>
        public void AddGoldItemCount(int value)
        {
            this.AddCount(TASK_SIGNKEY_GOLD_ITEM_COUNT, value);
        }

        /// <summary>
        /// Gets the gold item count.
        /// </summary>
        /// <returns>The gold item count.</returns>
        public int GetGoldItemCount()
        {
            if (this.Host == null)
            {
                return 0;
            }

            return this.Host.GetDynamicIntByKey(TASK_SIGNKEY_GOLD_ITEM_COUNT);
        }

        /// <summary>
        /// Adds the note item count.
        /// </summary>
        /// <param name="value">Value.</param>
        public void AddNoteItemCount(int value)
        {
            this.AddCount(TASK_SIGNKEY_NOTE_ITEM_COUNT, value);
        }

        /// <summary>
        /// Gets the note item count.
        /// </summary>
        /// <returns>The note item count.</returns>
        public int GetNoteItemCount()
        {
            if (this.Host == null)
            {
                return 0;
            }

            return this.Host.GetDynamicIntByKey(TASK_SIGNKEY_NOTE_ITEM_COUNT);
        }

        /// <summary>
        /// Adds the hit empty count.
        /// </summary>
        /// <param name="value">Value.</param>
        public void AddHitEmptyCount(int value)
        {
            this.AddCount(TASK_SIGNKEY_HITEMPTY_ITEM_COUNT, value);
        }

        /// <summary>
        /// Gets the hit empty count.
        /// </summary>
        /// <returns>The hit empty count.</returns>
        public int GetHitEmptyCount()
        {
            if (this.Host == null)
            {
                return 0;
            }

            return this.Host.GetDynamicIntByKey(TASK_SIGNKEY_HITEMPTY_ITEM_COUNT);
        }

        /// <summary>
        /// Adds the jump count.
        /// </summary>
        /// <returns>The jump count.</returns>
        /// <param name="value">Value.</param>
        public void AddJumpCount(int value)
        {
            this.AddCount(TASK_SIGNKEY_JUMP_COUNT, value);
        }

        /// <summary>
        /// Gets the jump count.
        /// </summary>
        /// <returns>The jump count.</returns>
        public int GetJumpCount()
        {
            if (this.Host == null)
            {
                return 0;
            }

            return this.Host.GetDynamicIntByKey(TASK_SIGNKEY_JUMP_COUNT);
        }

        /// <summary>
        /// Adds the combo max.
        /// </summary>
        /// <param name="value">Value.</param>
        public void AddComboMax(int value)
        {
            if (this.Host == null)
            {
                return;
            }

            int cmax = this.Host.GetDynamicIntByKey(TASK_SIGNKEY_MAX_COMBO);
            if (value < cmax)
            {
                return;
            }

            this.SetCount(TASK_SIGNKEY_MAX_COMBO, value);
        }

        /// <summary>
        /// Gets the combo max.
        /// </summary>
        /// <returns>The combo max.</returns>
        public int GetComboMax()
        {
            return this.Host == null ? 0 : this.Host.GetDynamicIntByKey(TASK_SIGNKEY_MAX_COMBO);
        }

        public int GetPerfectMax()
        {
            return this.Host == null ? 0 : this.Host.GetDynamicIntByKey(TASK_SIGNKEY_EVLUATE_HEAD + GameMusic.PERFECT + TASK_SIGNKEY_COUNT_MAX_TAIL);
        }

        /// <summary>
        /// Adds the damage max.
        /// </summary>
        /// <param name="value">Value.</param>
        public void AddDamageMax(int value)
        {
            if (this.Host == null)
            {
                return;
            }

            int cmax = this.Host.GetDynamicIntByKey(TASK_SIGNKEY_MAX_DAMAGE);
            if (value < cmax)
            {
                return;
            }

            this.SetCount(TASK_SIGNKEY_MAX_DAMAGE, value);
        }

        /// <summary>
        /// Gets the damage max.
        /// </summary>
        /// <returns>The damage max.</returns>
        public int GetDamageMax()
        {
            if (this.Host == null)
            {
                return 0;
            }

            return this.Host.GetDynamicIntByKey(TASK_SIGNKEY_MAX_DAMAGE);
        }

        /// <summary>
        /// Set the stage evluate max.
        /// </summary>
        /// <param name="value">Value.</param>
        public bool SetStageEvluateMax(int value)
        {
            if (this.Host == null)
            {
                return false;
            }

            int cmax = this.Host.GetDynamicIntByKey(TASK_SIGNKEY_STAGE_EVLUATE);
            Debug.Log("Add stage evluate : " + value + " / " + cmax);
            if (value < cmax)
            {
                return false;
            }

            this.SetCount(TASK_SIGNKEY_STAGE_EVLUATE, value);
            return true;
        }

        /// <summary>
        /// Gets the stage evluate max.
        /// </summary>
        /// <returns>The stage evluate max.</returns>
        public int GetStageEvluateMax()
        {
            if (this.Host == null)
            {
                return 0;
            }
            return this.Host.GetDynamicIntByKey(TASK_SIGNKEY_STAGE_EVLUATE);
        }

        /// <summary>
        /// Adds the crt count.
        /// </summary>
        /// <param name="value">Value.</param>
        public void AddCrtCount(int value)
        {
            this.AddCount(TASK_SIGNKEY_CRT_COUNT, value);
        }

        /// <summary>
        /// Gets the crt count.
        /// </summary>
        /// <returns>The crt count.</returns>
        public int GetCrtCount()
        {
            if (this.Host == null)
            {
                return 0;
            }

            return this.Host.GetDynamicIntByKey(TASK_SIGNKEY_CRT_COUNT);
        }

        /// <summary>
        /// Adds the score.
        /// </summary>
        /// <param name="value">Value.</param>
        public void AddScore(int value)
        {
            this.AddDamageMax(value);
            this.AddCount(TASK_SIGNKEY_SCORE, value);
        }

        /// <summary>
        /// Gets the score.
        /// </summary>
        /// <returns>The score.</returns>
        public int GetScore()
        {
            if (this.Host == null)
            {
                return 0;
            }

            return this.Host.GetDynamicIntByKey(TASK_SIGNKEY_SCORE);
        }

        /// <summary>
        /// Adds the recover.
        /// </summary>
        /// <param name="value">Value.</param>
        public void AddRecover(int value)
        {
            this.AddCount(TASK_SIGNKEY_RECOVER, value);
        }

        /// <summary>
        /// Gets the recover.
        /// </summary>
        /// <returns>The recover.</returns>
        public int GetRecover()
        {
            if (this.Host == null)
            {
                return 0;
            }

            return this.Host.GetDynamicIntByKey(TASK_SIGNKEY_RECOVER);
        }

        /// <summary>
        /// Adds the long press finish count.
        /// </summary>
        /// <param name="value">Value.</param>
        public void AddLongPressFinishCount(int value)
        {
            this.AddCount(TASK_SIGNKEY_LONGPRESS_FIN_COUNT, value);
        }

        /// <summary>
        /// Gets the long press finish count.
        /// </summary>
        /// <returns>The long press finish count.</returns>
        public int GetLongPressFinishCount()
        {
            if (this.Host == null)
            {
                return 0;
            }

            return this.Host.GetDynamicIntByKey(TASK_SIGNKEY_LONGPRESS_FIN_COUNT);
        }

        /// <summary>
        /// Adds the arm count.
        /// </summary>
        /// <param name="value">Value.</param>
        public void AddArmCount(int value)
        {
            this.AddCount(TASK_SIGNKEY_ARM_COUNT, value);
        }

        /// <summary>
        /// Gets the arm count.
        /// </summary>
        /// <returns>The arm count.</returns>
        public int GetArmCount()
        {
            if (this.Host == null)
            {
                return 0;
            }

            return this.Host.GetDynamicIntByKey(TASK_SIGNKEY_ARM_COUNT);
        }

        /// <summary>
        /// Adds the hp rate count.
        /// </summary>
        /// <param name="value">Value.</param>
        public void AddHpRateCount(int value)
        {
            this.AddCount(TASK_SIGNKEY_HP_RATE_COUNT, value);
        }

        /// <summary>
        /// Gets the hp rate count.
        /// </summary>
        /// <returns>The hp rate count.</returns>
        public int GetHpRateCount()
        {
            if (this.Host == null)
            {
                return 0;
            }

            return this.Host.GetDynamicIntByKey(TASK_SIGNKEY_HP_RATE_COUNT);
        }

        /// <summary>
        /// Adds the stage clear count.
        /// </summary>
        /// <param name="value">Value.</param>
        public void AddStageClearCount(int value)
        {
            this.AddCount(TASK_SIGNKEY_STAGE_CLEAR_COUNT, value);
        }

        /// <summary>
        /// Gets the stage clear count.
        /// </summary>
        /// <returns>The stage clear count.</returns>
        public int GetStageClearCount()
        {
            if (this.Host == null)
            {
                return 0;
            }

            return this.Host.GetDynamicIntByKey(TASK_SIGNKEY_STAGE_CLEAR_COUNT);
        }

        public int GetStageClearCount(int idx)
        {
            var host = GetStageByIdx(idx);
            if (host == null) return 0;
            return host.GetDynamicIntByKey(TASK_SIGNKEY_STAGE_CLEAR_COUNT);
        }

        /// <summary>
        /// Adds the hide node count.
        /// </summary>
        /// <param name="value">Value.</param>
        public void AddHideNodeCount(int value)
        {
            this.AddCount(TASK_SIGNKEY_HIDE_NODE_COUNT, value);
        }

        /// <summary>
        /// Gets the hide node count.
        /// </summary>
        /// <returns>The hide node count.</returns>
        public int GetHideNodeCount()
        {
            if (this.Host == null)
            {
                return 0;
            }

            return this.Host.GetDynamicIntByKey(TASK_SIGNKEY_HIDE_NODE_COUNT);
        }
    }
}