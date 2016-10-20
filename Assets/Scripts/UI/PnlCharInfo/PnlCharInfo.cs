/// UI分析工具自动生成代码
/// PnlCharInfoUI主模块
///
using System;
using System.Collections.Generic;
using UnityEngine;

namespace PnlCharInfo
{
    public class PnlCharInfo : UIPhaseBase
    {
        private static PnlCharInfo instance = null;
        public Transform cellItemParent;
        public GameObject cellItem;
        private readonly List<ItemCellCharInfo.ItemCellCharInfo> m_ItemList = new List<ItemCellCharInfo.ItemCellCharInfo>();
        public UIButton btnComfirm, btnFeed, btnApply;
        public UISprite sprArrowLvl, sprArrowVigour, sprArrowStramina, sprArrowStrengh, sprArrowLuck;
        public UILabel txtNextName, txtNextLvl, txtNextVigour, txtNextStamina, txtNextStrengh, txtNextLuck;

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
        }

        public override void OnShow()
        {
            UpdateUI();
        }

        public override void OnHide()
        {
        }

        public void UpdateItemList(int selectID)
        {
            for (int i = 0; i < m_ItemList.Count; i++)
            {
                var item = m_ItemList[i];
                var itemID = item.host.GetDynamicIntByKey(FormulaBase.SignKeys.ID);
                item.SetSelected(selectID == itemID);
            }
        }

        private void UpdateUI()
        {
            InitUI();
        }

        private void InitUI()
        {
            DOTweenUtil.Delay(InitPnlItemsChoose, Time.deltaTime);
        }

        private void InitPnlItemsChoose()
        {
            cellItemParent.GetComponent<UIGrid>().enabled = true;
            cellItemParent.DestroyChildren();
            var allEquipments =
                FormulaBase.EquipManageComponent.Instance.GetGirlEquipHosts(PnlChar.PnlChar.Instance.curRoleIdx,
                    PnlChar.PnlChar.Instance.curEquipTypeIdx);
            foreach (var equipment in allEquipments)
            {
                var cell = GameObject.Instantiate(cellItem) as GameObject;
                var itemCellCharInfo = cell.GetComponent<ItemCellCharInfo.ItemCellCharInfo>();
                m_ItemList.Add(itemCellCharInfo);
                itemCellCharInfo.OnShow(equipment);
                itemCellCharInfo.SetSelected(equipment.GetDynamicIntByKey(FormulaBase.SignKeys.WHO) != 0);
                cell.transform.SetParent(cellItemParent, false);
            }
        }
    }
}