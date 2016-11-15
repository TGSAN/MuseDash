using FormulaBase;
using GameLogic;

/// UI分析工具自动生成代码
/// PnlVictoryUI主模块
///
using System;
using System.Linq;
using UnityEngine;

namespace PnlVictory
{
    public class PnlVictory : UIPhaseBase
    {
        private static PnlVictory instance = null;

        public static PnlVictory Instance
        {
            get
            {
                return instance;
            }
        }

        private bool isSaid = false;

        public UISprite sprGrade;
        public UITexture txrCharact;

        [HideInInspector]
        public float rank;

        private void Start()
        {
            this.SetTxrByCharacter();
        }

        public override void BeCatched()
        {
            instance = this;
            sprGrade.gameObject.SetActive(false);
        }

        private void OnEnable()
        {
            this.SetTxrByCharacter();
        }

        public override void OnShow()
        {
            if (isSaid)
            {
                return;
            }

            this.isSaid = true;
            SoundEffectComponent.Instance.SayByCurrentRole(GameGlobal.SOUND_TYPE_LAST_NODE);
            this.SetTexByGrade();
        }

        public override void OnHide()
        {
        }

        private void SetTexByGrade()
        {
            sprGrade.spriteName = "grade_" +
                                  TaskStageTarget.Instance.GetStagePJ();
            sprGrade.gameObject.SetActive(true);
        }

        private void SetTxrByCharacter()
        {
            int heroIndex = RoleManageComponent.Instance.GetFightGirlIndex();
            string txrName = ConfigPool.Instance.GetConfigStringValue("char_info", heroIndex.ToString(), "image_victory");
            if (txrName == null || ResourceLoader.Instance == null)
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
                string txrName = ConfigPool.Instance.GetConfigStringValue("char_info", heroIndex.ToString(), "image_victory");
                Debug.Log("Load char_info " + heroIndex + " PnlVictory texture failed : " + txrName);
            }

            this.txrCharact.mainTexture = t;
        }
    }
}