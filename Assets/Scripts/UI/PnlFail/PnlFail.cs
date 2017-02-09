/// UI分析工具自动生成代码
/// result_failUI主模块
///
using System;
using UnityEngine;
using FormulaBase;
using DYUnityLib;

namespace PnlFail
{
    public class PnlFail : UIPhaseBase
    {
        private static PnlFail instance = null;

        public static PnlFail Instance
        {
            get
            {
                return instance;
            }
        }

        public UITexture txrCharact;

        private void Start()
        {
            instance = this;
            this.SetTxrByCharacter();
        }

        private void OnEnable()
        {
            this.SetTxrByCharacter();
        }

        public override void OnShow()
        {
            this.SetTxrByCharacter();
            this.Pause();
        }

        public override void OnHide()
        {
            this.gameObject.SetActive(false);
        }

        private void Pause()
        {
            AudioManager.Instance.PauseBackGroundMusic();
            FixUpdateTimer.PauseTimer();
            StageBattleComponent.Instance.Pause();
            Time.timeScale = 0;
        }

        private void SetTxrByCharacter()
        {
            if (ResourceLoader.Instance == null)
            {
                return;
            }

            int heroIndex = RoleManageComponent.Instance.GetFightGirlIndex();
            string txrName = ConfigPool.Instance.GetConfigStringValue("char_info", heroIndex.ToString(), "image_fail");
            if (ResourceLoader.Instance == null || txrName == null || (this.txrCharact != null && txrName.Contains(this.txrCharact.name)))
            {
                return;
            }

            ResourceLoader.Instance.Load(txrName, this.__LoadTxr);
        }

        private void __LoadTxr(UnityEngine.Object resObj)
        {
            Texture t = resObj as Texture;
            if (t == null)
            {
                int heroIndex = RoleManageComponent.Instance.GetFightGirlIndex();
                string txrName = ConfigPool.Instance.GetConfigStringValue("char_info", heroIndex.ToString(), "image_fail");
                Debug.Log("Load char_info " + heroIndex + " PnlFail texture failed : " + txrName);
            }

            this.txrCharact.mainTexture = t;
        }
    }
}