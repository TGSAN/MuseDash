using Assets.Scripts.Common.Manager;
using FormulaBase;

/// UI分析工具自动生成代码
/// PnlAchievementInfoUI主模块
///
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace PnlAchievementInfo
{
    public class PnlAchievementInfo : UIPhaseBase
    {
        private static PnlAchievementInfo instance = null;
        public Transform combo, perfect, stars, clear;
        public UILabel txtCombo, txtPerfect, txtStar, txtClear;
        private readonly List<BtnItemBoard.BtnItemBoard> m_BtnDic = new List<BtnItemBoard.BtnItemBoard>();

        public static PnlAchievementInfo Instance
        {
            get
            {
                return instance;
            }
        }

        private void Awake()
        {
            perfect.GetComponentsInChildren<BtnItemBoard.BtnItemBoard>().ToList().ForEach(btnItemBoard => m_BtnDic.Add(btnItemBoard));
            combo.GetComponentsInChildren<BtnItemBoard.BtnItemBoard>().ToList().ForEach(btnItemBoard => m_BtnDic.Add(btnItemBoard));
            stars.GetComponentsInChildren<BtnItemBoard.BtnItemBoard>().ToList().ForEach(btnItemBoard => m_BtnDic.Add(btnItemBoard));
            clear.GetComponentsInChildren<BtnItemBoard.BtnItemBoard>().ToList().ForEach(btnItemBoard => m_BtnDic.Add(btnItemBoard));
        }

        public override void OnShow()
        {
            var stageID = TaskStageTarget.Instance.GetId();
            txtCombo.text = AchievementManager.instance.GetComboMax(stageID).ToString();
            txtPerfect.text = AchievementManager.instance.GetPerfectMax(stageID).ToString();
            txtStar.text = AchievementManager.instance.GetStarMax(stageID).ToString();
            txtClear.text = AchievementManager.instance.GetClearCount(stageID).ToString();

            var allAchievements = AchievementManager.instance.GetAchievements();
            for (var i = 0; i < m_BtnDic.Count; i++)
            {
                var btn = m_BtnDic[i];
                btn.OnShow(allAchievements[i]);
            }
        }

        public override void OnHide()
        {
        }

        public override void BeCatched()
        {
            instance = this;
        }
    }
}