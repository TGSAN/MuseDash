using Assets.Scripts.Common.Manager;
using FormulaBase;

/// UI分析工具自动生成代码
/// PnlAchievementUI主模块
///
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace PnlAchievement
{
    public class PnlAchievement : UIPhaseBase
    {
        private static PnlAchievement instance = null;
        public Transform highScore, maxCombo, clear;
        public UILabel txtHighScore, txtMaxCombo, txtClear;
//        private readonly List<BtnItemBoard.BtnItemBoard> m_BtnDic = new List<BtnItemBoard.BtnItemBoard>();

        public static PnlAchievement Instance
        {
            get
            {
                return instance;
            }
        }

        private void Awake()
        {
			//highScore.GetComponentsInChildren<BtnItemBoard.BtnItemBoard>().ToList().ForEach(btnItemBoard => m_BtnDic.Add(btnItemBoard));
			//maxCombo.GetComponentsInChildren<BtnItemBoard.BtnItemBoard>().ToList().ForEach(btnItemBoard => m_BtnDic.Add(btnItemBoard));
			//clear.GetComponentsInChildren<BtnItemBoard.BtnItemBoard>().ToList().ForEach(btnItemBoard => m_BtnDic.Add(btnItemBoard));
        }

        public override void OnShow()
        {
            var stageID = TaskStageTarget.Instance.GetId();
//            txtHighScore.text = AchievementManager.instance.GetHighScore(stageID).ToString();
			txtMaxCombo.text = AchievementManager.instance.GetComboMax(stageID).ToString();
            txtClear.text = AchievementManager.instance.GetClearCount(stageID).ToString();

            var allAchievements = AchievementManager.instance.GetAchievements();
//            for (var i = 0; i < m_BtnDic.Count; i++)
            {
//                var btn = m_BtnDic[i];
//                var ach = allAchievements[i];
//                btn.OnShow(ach);
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