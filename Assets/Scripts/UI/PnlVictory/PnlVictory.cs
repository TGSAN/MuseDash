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
        private static PnlVictory m_Instance = null;
        public GameObject[] goAll, goNoneAll;
        public GameObject best, trophyTaskFalse, trophyTaskTrue;
        public UIButton btn;
        private bool m_Flag = false;

        public static PnlVictory Instance
        {
            get
            {
                return m_Instance;
            }
        }

        private bool isSaid = false;

        public UISprite sprGrade;
        public UITexture txrCharact;

        private void Start()
        {
            this.SetTxrByCharacter();
            var callFunc = btn.onClick[0];
            btn.onClick.Clear();
            UIEventListener.Get(btn.gameObject).onClick = (go) =>
            {
                GetComponent<Animator>().Play("score_change");
                UIEventListener.Get(btn.gameObject).onClick = null;
                btn.onClick.Add(callFunc);
            };
        }

        public override void BeCatched()
        {
            m_Instance = this;
            sprGrade.gameObject.SetActive(false);
        }

        private void OnEnable()
        {
            this.SetTxrByCharacter();

            if (m_Flag)
            {
                this.SetTexByGrade();
                var isAllCombo = StageBattleComponent.Instance.IsAllCombo();

                goAll.ToList().ForEach(go => go.SetActive(isAllCombo));
                goNoneAll.ToList().ForEach(go => go.SetActive(!isAllCombo));
            }

            m_Flag = true;
        }

        public override void OnShow()
        {
            if (isSaid)
            {
                return;
            }

            this.isSaid = true;
            SoundEffectComponent.Instance.SayByCurrentRole(GameGlobal.SOUND_TYPE_LAST_NODE);

            var isClearAllDiff = FightMenuPanel.Instance.isAchieve;
            var isAchieve = TaskStageTarget.Instance.IsAchieveNow();
            best.SetActive(isClearAllDiff);
            trophyTaskTrue.SetActive(!isClearAllDiff && isAchieve);
            trophyTaskFalse.SetActive(!trophyTaskTrue.activeSelf);
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