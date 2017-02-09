/// UI分析工具自动生成代码
/// 暂停按钮
///
using System;
using DYUnityLib;
using UnityEngine;
using FormulaBase;

namespace PnlBattle
{
    public class OnClickBtnPause : UIPhaseOnClickBase
    {
        public static void Do(GameObject gameObject)
        {
            if (BattleRoleAttributeComponent.Instance.IsDead())
            {
                return;
            }

            AudioSource clip = gameObject.GetComponent<AudioSource>();
            if (clip != null)
            {
                clip.Play();
            }

            if (Time.timeScale > 0)
            {
                Time.timeScale = 0;
                //UserUI.Instance.SetGUIActive (true);
                AudioManager.Instance.PauseBackGroundMusic();
                StageBattleComponent.Instance.Pause();
            }

            OnDo(gameObject);
        }
    }
}