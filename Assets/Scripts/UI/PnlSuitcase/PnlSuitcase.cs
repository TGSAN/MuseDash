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
        public List<FormulaHost> upgradeSelectedHost = new List<FormulaHost>();
        private bool m_IsUpgrade = false;

        public bool isUpgrade
        {
            get { return m_IsUpgrade; }
            set
            {
                m_IsUpgrade = value;
                if (!value)
                {
                    foreach (var itemImageEquip in m_Cells)
                    {
                        itemImageEquip.isUpgradeSelected = false;
                    }
                    upgradeSelectedHost.Clear();
                    SetTypeActive(true, true, true);
                }
                else
                {
                    OnShow();
                }
                DOTweenUtils.Delay(() =>
                {
                    if (m_IsUpgrade)
                    {
                        SetTypeActive(false, true, false);
                    }
                    tglEquip.enabled = !m_IsUpgrade;
                    tglFood.enabled = !m_IsUpgrade;
                    tglServant.enabled = !m_IsUpgrade;
                }, 0.1f);
            }
        }

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

        public void SetLock(bool isTo)
        {
            m_Cells.ForEach(itemImageEquip =>
            itemImageEquip.isLock = isTo && !upgradeSelectedHost.Contains(itemImageEquip.host));
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
            m_Cells.ForEach(cell => cell.OnShow(cell.host));
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
                    cellScript.txtType.gameObject.SetActive(false);
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
                if (h == null)
                {
                    itemImageEquip.isUpgradeSelected = false;
                }
            }
            m_SelectedHost = h;
        }

        public void SetUpgradeSelectedCell(FormulaHost h)
        {
            foreach (var itemImageEquip in m_Cells)
            {
                itemImageEquip.isUpgradeSelected = h == itemImageEquip.host;
            }
            PnlEquipInfo.PnlEquipInfo.Instance.OnUpgradeItemsRefresh();
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
                        PnlFoodInfo.PnlFoodInfo.Instance.OnExit();
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
                        PnlServantInfo.PnlServantInfo.Instance.OnHide();
                    }
                    else
                    {
                        PnlServantInfo.PnlServantInfo.Instance.OnExit();
                    }
                }
                cell.gameObject.SetActive(isEquip || isFood || isServant);
            }
        }
    }
}