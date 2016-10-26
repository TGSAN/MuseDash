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
        private FormulaHost m_SelectedHost;

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

        public override void OnShow()
        {
            gameObject.SetActive(true);
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
            instance = this;
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

        public void SetSelectedCell(FormulaHost h)
        {
            foreach (var itemImageEquip in m_Cells)
            {
                itemImageEquip.OnSelected(h == itemImageEquip.host);
            }
            m_SelectedHost = h;
        }

        public void SetTypeActive(bool equip, bool food, bool servant)
        {
            tglEquip.value = equip;
            tglFood.value = food;
            tglServant.value = servant;
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
                    if (!PnlEquipInfo.PnlEquipInfo.Instance.itemUpgrade.activeSelf)
                    {
                        if (m_SelectedHost != null && m_SelectedHost.GetFileName() == "Equip")
                        {
                            PnlEquipInfo.PnlEquipInfo.Instance.OnHide();
                        }
                        else
                        {
                            PnlEquipInfo.PnlEquipInfo.Instance.OnExit();
                        }
                    }
                }
                if (tglFood.value)
                {
                    isFood = ItemManageComponent.Instance.isFood(cell.host);
                }
                else
                {
                    if (m_SelectedHost != null && m_SelectedHost.GetFileName() == "Material")
                    {
                        PnlFoodInfo.PnlFoodInfo.Instance.OnHide();
                    }
                    else
                    {
                        PnlEquipInfo.PnlEquipInfo.Instance.OnExit();
                    }
                }
                if (tglFood.value)
                {
                    isServant = ItemManageComponent.Instance.isServant(cell.host);
                }
                else
                {
                    if (m_SelectedHost != null && m_SelectedHost.GetFileName() == "Servant")
                    {
                        PnlFoodInfo.PnlFoodInfo.Instance.OnHide();
                    }
                    else
                    {
                        PnlFoodInfo.PnlFoodInfo.Instance.OnExit();
                    }
                }
                cell.gameObject.SetActive(isEquip || isFood || isServant);
            }
        }
    }
}