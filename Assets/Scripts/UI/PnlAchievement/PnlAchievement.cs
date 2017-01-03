using Assets.Scripts.NGUI;
using FormulaBase;
using GameLogic;

/// UI分析工具自动生成代码
/// PnlAchievementUI主模块
///
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace PnlAchievement
{
    public class PnlAchievement : UIPhaseBase
    {
        private static PnlAchievement instance = null;
        private int m_Idx;

        public static PnlAchievement Instance
        {
            get
            {
                return instance;
            }
        }

        public int sliderWidth;
        public UISprite nextTrophyShow;
        public List<GameObject> trophys;
        public UILabel[] labels;
        public UILabel txtEnergyCost;
        public UILabel txtTargetScore;
        public GameObject goTrophyScore;
        public UIButton btnTip;

        public TweenFill slideCombo;
        public TweenFill slidePerfect;
        public TweenFill slideStars;
        public TweenFill slideClear;

        private void Start()
        {
            this.slideCombo.enabled = false;
            this.slidePerfect.enabled = false;
            this.slideStars.enabled = false;
            this.slideClear.enabled = false;
        }

        public override void OnShow(int idx)
        {
            var stageHost = TaskStageTarget.Instance.GetStageByIdx(idx);
            if (!TaskStageTarget.Instance.Contains(idx))
            {
                labels.ToList().ForEach(l => l.text = "0");
                txtTargetScore.text = ConfigPool.Instance.GetConfigIntValue("stage_value", idx.ToString(), "goal_1").ToString();
            }
            foreach (GameObject t in this.trophys)
            {
                t.SetActive(false);
            }
            if (stageHost != null)
            {
                goTrophyScore.SetActive(!TaskStageTarget.Instance.IsUnLockAllDiff(stageHost));
            }
            UIEventListener.Get(btnTip.gameObject).onClick = go =>
            {
                CommonPanel.GetInstance().ShowText("当前歌曲已获得奖杯数：" + (stageHost == null ? "0" : stageHost.GetDynamicIntByKey(TaskStageTarget.TASK_SIGNKEY_STAGE_EVLUATE + TaskStageTarget.TASK_SIGNKEY_COUNT_MAX_TAIL).ToString()).ToString());
            };
            gameObject.SetActive(true);
            m_Idx = idx;
            this.StartCoroutine(this.__OnShow(0.1f));
            var energyCost = ConfigPool.Instance.GetConfigIntValue("stage_value", idx.ToString(), "energy");
            txtEnergyCost.text = energyCost.ToString();
        }

        public override void OnHide()
        {
            if (PnlScrollCircle.instance != null)
            {
                PnlScrollCircle.instance.FinishEnter = true;
                Debug.Log("Back to PnlScrollCircle.");
            }

            this.gameObject.SetActive(false);
        }

        private IEnumerator __OnShow(float sec)
        {
            yield return new WaitForSeconds(sec);

            int rank = TaskStageTarget.Instance.GetStageEvluateMax();
            rank = rank > 3 ? 3 : rank;
            for (int i = 0; i < rank; i++)
            {
                GameObject t = this.trophys[i];
                t.SetActive(i < rank);
            }

            var stageHost = TaskStageTarget.Instance.GetStageByIdx(m_Idx);
            if (stageHost != null)
            {
                var stageID = stageHost.GetDynamicIntByKey(SignKeys.ID);
                var maxCombo = (float)ConfigPool.Instance.GetConfigIntValue("stage", stageID.ToString(), "all_combo");
                var maxPerfect = (float)ConfigPool.Instance.GetConfigIntValue("stage", stageID.ToString(), "all_perfect");
                var maxStar = (float)ConfigPool.Instance.GetConfigIntValue("stage", stageID.ToString(), "all_star");
                var maxClear = 0f;
                var clearConfig = ConfigPool.Instance.GetConfigByName("achievement");
                for (var i = 0; i < clearConfig.Count; i++)
                {
                    var table = clearConfig[i];
                    if ((int)table["uid"] != stageID) continue;
                    if ((string)table["type"] != "clear") continue;
                    maxClear = (float)((int)table["s_goal"]);
                    break;
                }

                var perfectMaxCount = (float)stageHost.GetDynamicIntByKey(TaskStageTarget.TASK_SIGNKEY_EVLUATE_HEAD + GameMusic.PERFECT + TaskStageTarget.TASK_SIGNKEY_COUNT_MAX_TAIL);
                var comboCount = (float)stageHost.GetDynamicIntByKey(TaskStageTarget.TASK_SIGNKEY_MAX_COMBO);
                var starCount = (float)stageHost.GetDynamicIntByKey(TaskStageTarget.TASK_SIGNKEY_HIDE_NODE_COUNT + TaskStageTarget.TASK_SIGNKEY_COUNT_MAX_TAIL);
                var clearCount = (float)stageHost.GetDynamicIntByKey(TaskStageTarget.TASK_SIGNKEY_STAGE_CLEAR_COUNT);

                PlayProgress(comboCount / maxCombo, slideCombo);
                PlayProgress(perfectMaxCount / maxPerfect, slidePerfect);
                PlayProgress(starCount / maxStar, slideStars);
                PlayProgress(clearCount / maxClear, slideClear);
            }
            /* this.ShowRankProgress(TaskStageTarget.TASK_SIGNKEY_MAX_COMBO, "Combo_", this.slideCombo);
            this.ShowRankProgress(TaskStageTarget.TASK_SIGNKEY_EVLUATE_HEAD + GameMusic.PERFECT, "Perfect_", this.slidePerfect);
            this.ShowRankProgress(TaskStageTarget.TASK_SIGNKEY_HIDE_NODE_COUNT, "Star_", this.slideStars);
            this.ShowRankProgress(TaskStageTarget.TASK_SIGNKEY_STAGE_CLEAR_COUNT, "Clear_", this.slideClear);*/
        }

        public override void BeCatched()
        {
            instance = this;
        }

        private void PlayProgress(float to, TweenFill twn)
        {
            twn.enabled = true;
            twn.from = 0;
            twn.to = to;
            twn.ResetToBeginning();
            twn.PlayForward();
        }

        private void ShowRankProgress(string taskKey, string cfgKey, TweenWidth slider)
        {
            float rateBase = 0.2f;
            float rank = 0;
            int sid = StageBattleComponent.Instance.GetId();
            string strSid = sid.ToString();
            int vmax = TaskStageTarget.Instance.GetXMax(taskKey);
            foreach (string strRank in GameGlobal.STAGE_EVLUATE_MAP)
            {
                string _cfgKey = cfgKey + strRank;
                int _cfgValue = ConfigPool.Instance.GetConfigIntValue(StageRewardComponent.REWARD_CONFIG_NAME, strSid, _cfgKey);
                if (vmax <= _cfgValue)
                {
                    break;
                }

                rank += 1;
            }

            int rate = (int)(rateBase * rank * this.sliderWidth);
            slider.enabled = true;
            slider.from = 0;
            slider.to = rate;
            slider.ResetToBeginning();
            slider.PlayForward();
        }
    }
}