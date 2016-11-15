using GameLogic;

///自定义模块，可定制模块具体行为
using System;
using System.Collections.Generic;
using UnityEngine;

namespace FormulaBase
{
    public class StageRewardComponent : CustomComponentBase
    {
        private static StageRewardComponent instance = null;
        private const int HOST_IDX = 3;

        public static StageRewardComponent Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new StageRewardComponent();
                }
                return instance;
            }
        }

        // ------------------------------------------------------------------------------------
        public const string REWARD_CONFIG_NAME = "stage_value";

        public const string ACHIEVEMENT_CONFIG_NAME = "achievement";

        public static string[] ACHIEVEMENT_GOAL_MAP = new string[] { "", "c_goal", "b_goal", "a_goal", "s_goal" };
        public static string[] ACHIEVEMENT_REWARD_MAP = new string[] { "", "c_award", "b_award", "a_award", "s_award" };

        public FormulaHost stage
        {
            private set;
            get;
        }

        public void SetStage(FormulaHost host)
        {
            this.stage = host;
        }

        /// <summary>
        /// Stages the reward.
        ///
        /// 关卡奖励
        /// </summary>
        /// <param name="rank">Rank.</param>
        /// <param name="rankStr">Rank string.</param>
        /// <param name="rechargeData">Recharge data.</param>
        public void StageReward(FormulaHost stageHost, bool isNewRank)
        {
            this.SetStage(stageHost);

            int id = this.stage.GetDynamicIntByKey(SignKeys.ID);
            string sid = this.stage.GetDynamicStrByKey(SignKeys.ID);
            int score = TaskStageTarget.Instance.GetScore();

            // 通关基础奖励
            int performanceScore = ConfigPool.Instance.GetConfigIntValue(REWARD_CONFIG_NAME, sid, "performance");
            float rwRate = score * 1.0f / performanceScore;
            int rwExp = (int)(ConfigPool.Instance.GetConfigIntValue(REWARD_CONFIG_NAME, sid, "exp") * rwRate);
            int rwGold = (int)(ConfigPool.Instance.GetConfigIntValue(REWARD_CONFIG_NAME, sid, "coin") * rwRate);
            int rwCharm = (int)(ConfigPool.Instance.GetConfigIntValue(REWARD_CONFIG_NAME, sid, "charm") * rwRate);
            // reward gold
            stageHost.SetDynamicData(SignKeys.GOLD, rwGold);
            // reward exp
            stageHost.SetDynamicData(SignKeys.EXP, rwExp);
            // reward diamond
            stageHost.SetDynamicData(SignKeys.DIAMOND, rwCharm);

            // 通关成就奖励
            // rank 来自 score和performance的比率 : 1, 0.8, 0.6, 0.4, 0.2
            var rank = ((float)TaskStageTarget.Instance.GetComboMax() * 0.5f + (float)TaskStageTarget.Instance.GetHideNodeCount() * 0.1f + (float)stageHost.GetDynamicIntByKey(SignKeys.FULL_PERFECT) * 0.4f) / performanceScore;
            // 每个关卡在 Achievement 表有连续4行对应
            int achiLen = 4;
            int achiId = (id - 1) * achiLen + 1;
            PnlVictory.PnlVictory.Instance.rank = rank;
            Debug.Log("Stage " + sid + " reward form " + achiId + " with rank : " + rank);
            for (int i = achiId; i < achiId + achiLen; i++)
            {
                string achiType = ConfigPool.Instance.GetConfigStringValue(ACHIEVEMENT_CONFIG_NAME, i.ToString(), "type");
                if (achiType == "perfect")
                {
                    int perfect = TaskStageTarget.Instance.Host.GetDynamicIntByKey(TaskStageTarget.TASK_SIGNKEY_EVLUATE_HEAD + GameMusic.PERFECT);
                    int perfuctFull = stageHost.GetDynamicIntByKey(SignKeys.FULL_PERFECT);
                    stageHost.SetDynamicData(SignKeys.ITEMEFFECT2, ((perfect >= perfuctFull) ? 1 : 0));

                    this._RewardByConfig(sid, perfect, i);
                    continue;
                }

                if (achiType == "combo")
                {
                    int combo = TaskStageTarget.Instance.GetComboMax();
                    int comboFull = stageHost.GetDynamicIntByKey(SignKeys.FULL_COMBO);
                    stageHost.SetDynamicData(SignKeys.ITEMEFFECT1, ((combo >= comboFull) ? 1 : 0));
                    this._RewardByConfig(sid, combo, i);
                    continue;
                }

                if (achiType == "star")
                {
                    int stars = TaskStageTarget.Instance.GetHideNodeCount();
                    int starsFull = stageHost.GetDynamicIntByKey(SignKeys.FULL_STAR);
                    stageHost.SetDynamicData(SignKeys.ITEMEFFECT3, ((stars >= starsFull) ? 1 : 0));
                    this._RewardByConfig(sid, stars, i);
                    continue;
                }

                if (achiType == "clear")
                {
                    int clearCount = TaskStageTarget.Instance.GetStageClearCount();
                    int clearMax = ConfigPool.Instance.GetConfigIntValue(ACHIEVEMENT_CONFIG_NAME, i.ToString(), "s_goal");
                    this._RewardByConfig(sid, clearCount, i);
                    continue;
                }
            }

            stageHost.SetDynamicData(SignKeys.LEVEL_STAR, (isNewRank ? 1 : 0));
        }

        private void _RewardByConfig(string stageId, int value, int cfgId)
        {
            // 反向查找，从大的开始比较
            for (int i = ACHIEVEMENT_GOAL_MAP.Length - 1; i >= 0; i--)
            {
                string cfgGoalKey = ACHIEVEMENT_GOAL_MAP[i];
                if (cfgGoalKey == "")
                {
                    continue;
                }

                // 奖励最高得分的项目
                int cfgValue = ConfigPool.Instance.GetConfigIntValue(ACHIEVEMENT_CONFIG_NAME, cfgId.ToString(), cfgGoalKey);
                if (value >= cfgValue)
                {
                    string cfgRwKey = ACHIEVEMENT_REWARD_MAP[i];
                    string rwStr = ConfigPool.Instance.GetConfigStringValue(ACHIEVEMENT_CONFIG_NAME, cfgId.ToString(), cfgRwKey);
                    string[] rwTypeValue = rwStr.Split('_');
                    string rwType = rwTypeValue[0];
                    int rwValue = int.Parse(rwTypeValue[1]);
                    if (rwType == "coin")
                    {
                        AccountGoldManagerComponent.Instance.ChangeMoney(rwValue, false);
                    }

                    if (rwType == "crystal")
                    {
                        AccountCrystalManagerComponent.Instance.ChangeDiamond(rwValue, false);
                    }

                    return;
                }
            }
        }

        // ------------------------------------------------------------------------------------------------------------
    }
}