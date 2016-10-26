using FormulaBase;

/// UI分析工具自动生成代码
/// PnlSuitcaseUI主模块
///
using System;
using System.Collections.Generic;
using UnityEngine;

namespace PnlSuitcase
{
    public class PnlSuitcase : UIPhaseBase
    {
        private static PnlSuitcase instance = null;
        public UIGrid grid;
        public UIToggle tglEquip, tglFood, tglServant;
        public Action onTypeChange;
        public GameObject cellPreb;
        private List<ItemImageEquip.ItemImageEquip> m_Cells;

        public int selectedTglIdx
        {
            get;
            private set;
        }

        public static PnlSuitcase Instance
        {
            get
            {
                return instance;
            }
        }

        private void Awake()
        {
            selectedTglIdx = 0;
            m_Cells = new List<ItemImageEquip.ItemImageEquip>();
            onTypeChange += OnTypeChange;
        }

        private void Start()
        {
            instance = this;
        }

        public override void OnShow()
        {
            grid.enabled = true;
            m_Cells.RemoveAll(cell =>
            {
                if (!ItemManageComponent.Instance.Contains(cell.host.GetDynamicIntByKey(SignKeys.ID)))
                {
                    Destroy(cell.gameObject);
                    return true;
                }
                return false;
            });
        }

        public override void OnHide()
        {
            PnlEquipInfo.PnlEquipInfo.Instance.OnHide();
            PnlFoodInfo.PnlFoodInfo.Instance.OnHide();
            PnlServantInfo.PnlServantInfo.Instance.OnHide();
            SetSelectedCell(null);
        }

        public override void BeCatched()
        {
            ItemManageComponent.Instance.SortAllQuality();
            var items = ItemManageComponent.Instance.GetAllItem;
            foreach (var item in items)
            {
                var cell = Instantiate(cellPreb, grid.transform) as GameObject;
                if (cell != null)
                {
                    cell.AddComponent<UIDragScrollView>();
                    cell.transform.localScale = Vector3.one;
                    var cellScript = cell.GetComponent<ItemImageEquip.ItemImageEquip>();
                    cellScript.OnShow(item);
                    m_Cells.Add(cellScript);
                }
            }
            grid.enabled = true;
        }

        public void SetSelectedCell(ItemImageEquip.ItemImageEquip cell)
        {
            foreach (var itemImageEquip in m_Cells)
            {
                itemImageEquip.OnSelected(cell == itemImageEquip);
            }
        }

        public void OnTypeChange()
        {
            grid.enabled = true;
            foreach (var cell in m_Cells)
            {
                var isEquip = false;
                var isFood = false;
                var isServant = false;
                if (tglEquip.value)
                {
                    isEquip = ItemManageComponent.Instance.IsEquipment(cell.host);
                }
                else
                {
                    PnlEquipInfo.PnlEquipInfo.Instance.OnHide();
                }
                if (tglFood.value)
                {
                    isFood = ItemManageComponent.Instance.isFood(cell.host);
                }
                else
                {
                    PnlFoodInfo.PnlFoodInfo.Instance.OnHide();
                }
                if (tglFood.value)
                {
                    isServant = ItemManageComponent.Instance.isServant(cell.host);
                }
                else
                {
                    PnlServantInfo.PnlServantInfo.Instance.OnHide();
                }
                cell.gameObject.SetActive(isEquip || isFood || isServant);
            }
        }
    }
}