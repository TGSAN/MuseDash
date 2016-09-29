using FormulaBase;
using GameLogic;

/// UI分析工具自动生成代码
/// StageDiscUI主模块
///
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StageDisc
{
    public class StageDisc : UIPhaseBase
    {
        private static Dictionary<int, StageDisc> _discTable = null;

        private static StageDisc instance = null;
        public GameObject lockGO;
        public Color lockColor;

        public static StageDisc Instance
        {
            get
            {
                return instance;
            }
        }

        private Coroutine _loadTxrCoroutine;
        private FormulaHost stageHost;

        public int staegId;
        public GameObject unLock;
        public UITexture txrDisc;

        public static void LoadAllDiscCover()
        {
            if (_discTable == null)
            {
                return;
            }

            SetTxrByOrder(1);
        }

        public static void SetTxrByOrder(int idx)
        {
            if (idx > _discTable.Count)
            {
                return;
            }

            _discTable[idx].SetTxrByOrder();
        }

        private void Start()
        {
            instance = this;
        }

        public override void OnShow()
        {
        }

        public override void OnHide()
        {
        }

        public void SetStageId(int idx)
        {
            if (this.staegId > 0)
            {
                return;
            }

            this.staegId = idx;
            this.InitStageInfo();
            UIRootHelper urh = this.gameObject.GetComponent<UIRootHelper>();
            if (urh != null)
            {
                this.stageHost.SetAsUINotifyInstance(urh);
            }

            if (_discTable == null)
            {
                _discTable = new Dictionary<int, StageDisc>();
            }

            _discTable[this.staegId] = this;
            //this.SetTxrByStage ();
        }

        public void Lock(bool isTo)
        {
            lockGO.SetActive(isTo);
        }

        private void InitStageInfo()
        {
            this.stageHost = FomulaHostManager.Instance.CreateHost("Stage");
            this.stageHost.SetDynamicData(SignKeys.ID, this.staegId);
            this.stageHost.SetDynamicData(SignKeys.DIFFCULT, GameGlobal.DIFF_LEVEL_NORMAL);
            this.stageHost.Result(FormulaKeys.FORMULA_9);
        }

        private void SetTxrByStage()
        {
            //yield return new WaitForSeconds (0.1f);
            string txrName = ConfigPool.Instance.GetConfigStringValue("stage", this.staegId.ToString(), "icon");
            this._loadTxrCoroutine = ResourceLoader.Instance.Load(txrName, this.__LoadTxr);
        }

        private void SetTxrByOrder()
        {
            string txrName = ConfigPool.Instance.GetConfigStringValue("stage", this.staegId.ToString(), "icon");
            this._loadTxrCoroutine = ResourceLoader.Instance.Load(txrName, this.__LoadTxrByOrder);
        }

        private void __LoadTxr(UnityEngine.Object resObj)
        {
            Texture t = resObj as Texture;
            if (t == null)
            {
                string txrName = ConfigPool.Instance.GetConfigStringValue("stage", this.staegId.ToString(), "icon");
                Debug.Log("Load stage icon " + this.staegId + " StageDisc texture failed : " + txrName);
            }

            this.txrDisc.mainTexture = t;
        }

        private void __LoadTxrByOrder(UnityEngine.Object resObj)
        {
            Texture t = resObj as Texture;
            if (t == null)
            {
                string txrName = ConfigPool.Instance.GetConfigStringValue("stage", this.staegId.ToString(), "icon");
                Debug.Log("Load stage icon " + this.staegId + " StageDisc texture failed : " + txrName);
            }

            this.txrDisc.mainTexture = t;
            StageDisc.SetTxrByOrder(this.staegId + 1);

            if (this._loadTxrCoroutine != null)
            {
                ResourceLoader.Instance.StopCoroutine(this._loadTxrCoroutine);
            }
        }
    }
}