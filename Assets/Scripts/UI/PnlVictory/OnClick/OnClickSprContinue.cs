using FormulaBase;

/// UI分析工具自动生成代码
/// 未有描述
///
using System;
using UnityEngine;

namespace PnlVictory
{
    public class OnClickSprContinue : UIPhaseOnClickBase
    {
        private const string ROOT_PANEL_NAME = "PnlVictory";
        private const string TAG_NEXT_END = "UI";
        private const string ANI_NAME_END = "score2out";
        private const string ANI_NAME_CHANGE = "score_change";

        public static void Do(GameObject gameObject)
        {
            OnDo(gameObject);
            FormulaHost stageHost = StageBattleComponent.Instance.Host;
            int step = stageHost.GetDynamicIntByKey(SignKeys.LEVEL_STAR);
            if (gameObject.tag == TAG_NEXT_END || step <= 0)
            {
                gameObject.tag = "Untagged";
                UISceneHelper.Instance.HideUi(ROOT_PANEL_NAME, ANI_NAME_END);
                StageBattleComponent.Instance.Exit("ChooseSongs", true);
                return;
            }

            gameObject.tag = TAG_NEXT_END;
            UISceneHelper.Instance.ShowUi(ROOT_PANEL_NAME, ANI_NAME_CHANGE);
        }
    }
}