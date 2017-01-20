using Assets.Scripts.Common.Manager;
using FormulaBase;

/// UI分析工具自动生成代码
/// PnlAnnouncementUI主模块
///
using System;
using System.Linq;
using UnityEngine;

namespace PnlMessage
{
    public class PnlMessage : UIPhaseBase
    {
        public GameObject awardPre;
        public UIGrid grid;
        private static PnlMessage instance = null;

        public static PnlMessage Instance
        {
            get
            {
                return instance;
            }
        }

        public override void OnShow()
        {
            grid.transform.DestroyChildren();
            for (int i = 0; i < TaskManager.instance.awardTaskList.Count; i++)
            {
                var go = GameObject.Instantiate(awardPre, grid.transform, false);
                go.transform.localPosition = Vector3.zero;
            }
            grid.enabled = true;
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