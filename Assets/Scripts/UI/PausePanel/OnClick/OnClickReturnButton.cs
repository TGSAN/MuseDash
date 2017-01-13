using FormulaBase;

/// UI分析工具自动生成代码
/// 未有描述
///
using System;
using UnityEngine;

namespace PausePanel
{
    public class OnClickReturnButton : UIPhaseOnClickBase
    {
        public static void Do(GameObject gameObject)
        {
            StageBattleComponent.Instance.Exit();
            OnDo(gameObject);
        }
    }
}