using Assets.Scripts.Common.Manager;
using FormulaBase;

/// UI分析工具自动生成代码
/// PnlDailyTaskUI主模块
///
using System;
using UnityEngine;

namespace PnlDailyTask
{
    public class PnlDailyTask : UIPhaseBase
    {
        public UISprite[] sprIcons, sprPercent;
        public UILabel[] txtDecs, txtValueTo, txtValueMax, txtAward;
        public GameObject[] coins, crystals;
        public UILabel txtFinishTaskNum, txtAllTaskNum;
        public UISprite sprTaskNumBar;

        private static PnlDailyTask instance = null;

        public static PnlDailyTask Instance
        {
            get
            {
                return instance;
            }
        }

        public override void BeCatched()
        {
            instance = this;
        }

        public override void OnShow()
        {
            var curCount = DailyTaskManager.instance.endTaskList.Count;
            var maxCount = 10;
            txtAllTaskNum.text = maxCount.ToString();
            txtFinishTaskNum.text = curCount.ToString();
            sprTaskNumBar.transform.localScale = curCount == 0 ? new Vector3(0, 1, 1) : Vector3.one;
            sprTaskNumBar.width = (int)(680f * ((float)curCount / (float)maxCount));
            for (var i = 0; i < 3; i++)
            {
                InitTaskBox(i);
            }
        }

        public override void OnHide()
        {
        }

        private void InitTaskBox(int idx)
        {
            sprIcons[idx].transform.parent.gameObject.SetActive(false);
            var taskList = DailyTaskManager.instance.curTaskList;
            if (idx >= taskList.Count) return;
            sprIcons[idx].transform.parent.gameObject.SetActive(true);
            var dailyTask = taskList[idx];
            var host = DailyTaskManager.instance.GetFormulaHost(int.Parse(dailyTask.uid));
            var targetValue = host.GetDynamicIntByKey(SignKeys.DT_TARGET);
            var curValue = host.GetDynamicIntByKey(SignKeys.DT_VALUE);
            sprIcons[idx].spriteName = dailyTask.icon;
            sprPercent[idx].transform.localScale = curValue == 0 ? new Vector3(0, 1, 1) : Vector3.one;
            sprPercent[idx].width = (int)(720f * (float)curValue / (float)targetValue);
            if (dailyTask.description.Contains("(N)"))
            {
                txtDecs[idx].text = dailyTask.description.Replace("(N)", targetValue.ToString());
            }
            if (dailyTask.description.Contains("(S)"))
            {
                txtDecs[idx].text = dailyTask.description.Replace("(S)", TaskStageTarget.Instance.GetAllMusicNames()[targetValue - 1]);
            }
            txtValueTo[idx].text = curValue.ToString();
            txtValueMax[idx].text = targetValue.ToString();
            if (dailyTask.coinAward > 0)
            {
                txtAward[idx].text = dailyTask.coinAward.ToString();
                coins[idx].SetActive(true);
                crystals[idx].SetActive(false);
            }
            else
            {
                txtAward[idx].text = dailyTask.crystalAward.ToString();
                coins[idx].SetActive(false);
                crystals[idx].SetActive(true);
            }
        }
    }
}