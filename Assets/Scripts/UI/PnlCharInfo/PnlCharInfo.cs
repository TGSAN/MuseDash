/// UI分析工具自动生成代码
/// PnlCharInfoUI主模块
///
using System;
using UnityEngine;

namespace PnlCharInfo
{
    public class PnlCharInfo : UIPhaseBase
    {
        private static PnlCharInfo instance = null;
        public Transform cellItemParent;
        public GameObject cellItem;

        public static PnlCharInfo Instance
        {
            get
            {
                return instance;
            }
        }

        private void Start()
        {
            instance = this;
            InitPnlItemsChoose();
        }

        public override void OnShow()
        {
        }

        public override void OnHide()
        {
        }

        private void InitPnlItemsChoose()
        {
            var allEquipments = FormulaBase.EquipManageComponent.Instance.HostList;
            foreach (var equipment in allEquipments)
            {
                var cell = GameObject.Instantiate(cellItem) as GameObject;
                cell.transform.SetParent(cellItemParent, false);
            }
        }
    }
}