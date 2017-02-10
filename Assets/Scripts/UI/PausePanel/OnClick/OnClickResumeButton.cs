/// UI分析工具自动生成代码
/// 未有描述
///
using System;
using FormulaBase;
using UnityEngine;
using GameLogic;

namespace PausePanel
{
    public class OnClickResumeButton : UIPhaseOnClickBase
    {
        public static void Do(GameObject gameObject)
        {
            OnDo(gameObject);

            Time.timeScale = GameGlobal.TIME_SCALE;
            AudioManager.Instance.ResumeBackGroundMusic();
            AudioManager.Instance.SetBackGroundMusicTimeScale(GameGlobal.TIME_SCALE);
        }
    }
}