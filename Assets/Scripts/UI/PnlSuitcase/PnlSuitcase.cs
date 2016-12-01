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
        public UIToggle tglEquip, tglFood, tglServant, tglAll;
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
                    SetTypeActive(true, false, false, false);
                }
                else
                {
                    OnShow();
                }
                DOTweenUtils.Delay(() =>
                {
                    if (m_IsUpgrade)
                    {
                        if (PnlEquipInfo.PnlEquipInfo.Instance.gameObject.activeSelf)
                        {
                            SetTypeActive(false, true, false, false);
                        }
                        else
                        {
                            SetTypeActive(false, false, true, false);
                        }
                    }
                    tglEquip.enabled = !m_IsUpgrade;
                    tglFood.enabled = !m_IsUpgrade;
                    tglServant.enabled = !m_IsUpgrade;
                    tglAll.enabled = !m_IsUpgrade;
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
            ResetPos();
            gameObject.SetActive(true);
            grid.enabled = true;
            m_Cells.RemoveAll(cell =>
            {
                if (!ItemManageComponent.Instance.Contains(cell.host.GetDynamicIntByKey(SignKeys.BAGINID)))
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
            UpdateSuitcase();
        }

        public void UpdateSuitcase()
        {
            var items = ItemManageComponent.Instance.SortAllCur();
            grid.transform.DestroyChildren();
            m_Cells.Clear();
            foreach (var item in items)
            {
                var cell = Instantiate(cellPreb, grid.transform) as GameObject;
                if (cell == null) continue;
                cell.AddComponent<UIDragScrollView>();
                cell.transform.localScale = Vector3.one;
                var cellScript = cell.GetComponent<ItemImageEquip.ItemImageEquip>();
                cellScript.OnShow(item);
                cellScript.txtType.gameObject.SetActive(false);
                m_Cells.Add(cellScript);
            }
            grid.enabled = true;
        }

        public void SetSelectedCell(FormulaHost h)
        {
            m_SelectedHost = h;
            foreach (var itemImageEquip in m_Cells)
            {
                var host = itemImageEquip.host;
                if (m_SelectedHost != null)
                {
                    itemImageEquip.OnSelected(m_SelectedHost.GetDynamicIntByKey(SignKeys.BAGINID) ==
                                              host.GetDynamicIntByKey(SignKeys.BAGINID));
                }
                else
                {
                    itemImageEquip.OnSelected(false);
                }
            }
        }

        public void SetUpgradeSelectedCell(FormulaHost h)
        {
            foreach (var itemImageEquip in m_Cells)
            {
                itemImageEquip.isUpgradeSelected = h == itemImageEquip.host;
            }
            PnlEquipInfo.PnlEquipInfo.Instance.OnUpgradeItemsRefresh();
        }

        public void SetTypeActive(bool all, bool equip, bool food, bool servant)
        {
            //tglAll.value = all;
            tglEquip.value = equip;
            tglFood.value = food;
            tglServant.value = servant;
        }

        public void OnTypeChange()
        {
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
                    if (!isUpgrade)
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
                    isFood = ItemManageComponent.Instance.IsFood(cell.host);
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
                if (tglServant.value)
                {
                    isServant = ItemManageComponent.Instance.IsServant(cell.host) || ItemManageComponent.Instance.IsServantDebris(cell.host);
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
                cell.gameObject.SetActive(isEquip || isFood || isServant || tglAll.value);
            }
            grid.enabled = true;
            ResetPos();
        }

        public void ResetPos()
        {
            var scrollViewGO = grid.transform.parent.gameObject;
            scrollViewGO.transform.position = new Vector3(scrollViewGO.transform.position.x, 0,
                scrollViewGO.transform.position.z);
            scrollViewGO.GetComponent<UIPanel>().clipOffset =
                new Vector2(scrollViewGO.GetComponent<UIPanel>().clipOffset.x, 0);
        }
    }
}