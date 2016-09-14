using FormulaBase;
using GameLogic;

/// UI分析工具自动生成代码
/// 未有描述
///
using System;
using UnityEngine;

namespace PnlAdventure
{
    public class OnClickBtnBattle : UIPhaseOnClickBase
    {
        public static void Do(GameObject gameObject)
        {
            SceneAudioManager.Instance.bgm.clip = null;
            OnDo(gameObject);
        }
    }
}