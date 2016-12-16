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
            for (int i = 0; i < 3; i++)
            {
                InitTaskBox(i);
            }
        }

        public override void OnHide()
        {
        }

        private void InitTaskBox(int idx)
        {
            var dailyTask = DailyTaskManager.instance.curTaskList[idx];
            var host = DailyTaskManager.instance.GetFormulaHost(int.Parse(dailyTask.uid));
            sprIcons[idx].spriteName = dailyTask.icon;
            sprPercent[idx].width = 720;
            txtDecs[idx].text = dailyTask.description;
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