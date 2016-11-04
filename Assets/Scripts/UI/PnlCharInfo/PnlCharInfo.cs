using FormulaBase;

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
        public UIButton btnFeed, btnApply, btnConfirm;
        public UILabel txtNextVigour, txtNextStamina, txtNextStrengh, txtNextLuck, txtNextLvl;
        public UILabel txtCurVigour, txtCurStamina, txtCurStrengh, txtCurLuck, txtCurLvl;
        public UISprite sprExpCurBar, sprExpNextBar;
        public List<UITexture> upgradeTexs = new List<UITexture>();
        public Transform upgradeItemsParent;
        public UIButton btnBack;
        private Animator m_Animtor;
        private bool m_IsUpgrade = false;

        public static PnlCharInfo Instance
        {
            get
            {
                return instance;
            }
        }

        public bool isUpgrade
        {
            get { return m_IsUpgrade; }
            set
            {
                m_IsUpgrade = value;
                txtNextVigour.transform.parent.gameObject.SetActive(m_IsUpgrade);
                txtNextStamina.transform.parent.gameObject.SetActive(m_IsUpgrade);
                txtNextStrengh.transform.parent.gameObject.SetActive(m_IsUpgrade);
                txtNextLuck.transform.parent.gameObject.SetActive(m_IsUpgrade);
                txtNextLvl.transform.parent.gameObject.SetActive(m_IsUpgrade);
                sprExpNextBar.gameObject.SetActive(m_IsUpgrade);
                PnlSuitcase.PnlSuitcase.Instance.isUpgrade = isUpgrade;
                OnUpgradeItemsRefresh();
            }
        }

        public override void BeCatched()
        {
            m_Animtor = GetComponent<Animator>();
            instance = this;
        }

        public void OnEnter()
        {
            m_Animtor.Play("char_info_in");
        }

        public void OnExit()
        {
            m_Animtor.Play("pnl_items_choose_out");
        }

        public override void OnShow()
        {
            UpdateUI();
            InitEvent();
        }

        public override void OnHide()
        {
            isUpgrade = false;
        }

        public void OnUpgradeItemsRefresh()
        {
            if (!gameObject.activeSelf)
            {
                return;
            }
            var hostList = PnlSuitcase.PnlSuitcase.Instance.upgradeSelectedHost;
            for (int i = 0; i < upgradeTexs.Count; i++)
            {
                var tex = upgradeTexs[i];
                if (i < hostList.Count)
                {
                    var h = hostList[i];
                    var texName = h.GetDynamicStrByKey(SignKeys.ICON);
                    ResourceLoader.Instance.Load(texName, resObj => tex.mainTexture = resObj as Texture);
                }
                else
                {
                    tex.mainTexture = null;
                }
            }

            var curRoleHost = RoleManageComponent.Instance.GetGirlByIdx(PnlChar.PnlChar.Instance.curRoleIdx);
            if (curRoleHost != null)
            {
                var originLvl = curRoleHost.GetDynamicIntByKey(SignKeys.LEVEL);
                var originExp = curRoleHost.GetDynamicIntByKey(SignKeys.EXP);
                var originVigour = (int)curRoleHost.Result(FormulaKeys.FORMULA_38);
                var originStamina = (int)curRoleHost.Result(FormulaKeys.FORMULA_40);
                var originStrengh = (int)curRoleHost.Result(FormulaKeys.FORMULA_39);
                var originLuck = (int)curRoleHost.Result(FormulaKeys.FORMULA_18);
                var originRequiredExp = ConfigPool.Instance.GetConfigIntValue("experience", originLvl.ToString(), "char_exp");
                var originExpPercent = (float)originExp / (float)originRequiredExp;

                RoleManageComponent.Instance.LevelUp(curRoleHost, PnlSuitcase.PnlSuitcase.Instance.upgradeSelectedHost, null, false);

                var afterLvl = curRoleHost.GetDynamicIntByKey(SignKeys.LEVEL);
                var afterExp = curRoleHost.GetDynamicIntByKey(SignKeys.EXP);
                var vigourTo = (int)curRoleHost.Result(FormulaKeys.FORMULA_38);
                var staminaTo = (int)curRoleHost.Result(FormulaKeys.FORMULA_40);
                var strenghTo = (int)curRoleHost.Result(FormulaKeys.FORMULA_39);
                var luckTo = (int)curRoleHost.Result(FormulaKeys.FORMULA_18);
                var curExpPercent = (float)afterExp / (float)originRequiredExp;
                curExpPercent = afterLvl != originLvl ? 1.0f : curExpPercent;

                txtCurLvl.text = originLvl.ToString();
                txtCurVigour.text = originVigour.ToString();
                txtCurStamina.text = originStamina.ToString();
                txtCurStrengh.text = originStrengh.ToString();
                txtCurLuck.text = originLuck.ToString();

                txtNextVigour.text = vigourTo.ToString();
                txtNextStamina.text = staminaTo.ToString();
                txtNextStrengh.text = strenghTo.ToString();
                txtNextLuck.text = luckTo.ToString();
                txtNextLvl.text = afterLvl.ToString();

                sprExpCurBar.fillAmount = originExpPercent;
                sprExpNextBar.fillAmount = curExpPercent;

                curRoleHost.SetDynamicData(SignKeys.LEVEL, originLvl);
                curRoleHost.SetDynamicData(SignKeys.EXP, originExp);
            }
        }

        public void UpdateItemList(int selectID)
        {
            foreach (var item in m_ItemList)
            {
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
            DOTweenUtils.Delay(InitPnlItemsChoose, Time.deltaTime);
            DOTweenUtils.Delay(OnUpgradeItemsRefresh, 0.4f);
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

        private void InitEvent()
        {
            UIEventListener.Get(btnFeed.gameObject).onClick = (go) =>
            {
                var curRoleHost = RoleManageComponent.Instance.GetGirlByIdx(PnlChar.PnlChar.Instance.curRoleIdx);
                if (RoleManageComponent.Instance.IsItemLvlMax(curRoleHost))
                {
                    CommonPanel.GetInstance().ShowText("人物已达最高等级，无法升级");
                }
                else
                {
                    isUpgrade = true;
                    m_Animtor.Play("cos_change");
                    PnlSuitcase.PnlSuitcase.Instance.gameObject.SetActive(true);
                    PnlChar.PnlChar.Instance.gameObject.SetActive(false);
                }
            };
            UIEventListener.Get(btnBack.gameObject).onClick = (go) =>
            {
                isUpgrade = false;
                var isShow = PnlMainMenu.PnlMainMenu.Instance.goSelectedSuitcase.activeSelf;
                PnlSuitcase.PnlSuitcase.Instance.gameObject.SetActive(isShow);
                PnlSuitcase.PnlSuitcase.Instance.OnHide();
            };
            UIEventListener.Get(btnConfirm.gameObject).onClick = (go) =>
            {
                var hosts = PnlSuitcase.PnlSuitcase.Instance.upgradeSelectedHost;
                if (hosts.Count > 0)
                {
                    var curRoleHost = RoleManageComponent.Instance.GetGirlByIdx(PnlChar.PnlChar.Instance.curRoleIdx);
                    if (curRoleHost != null)
                    {
                        RoleManageComponent.Instance.LevelUp(curRoleHost, hosts, (result) =>
                        {
                            PnlSuitcase.PnlSuitcase.Instance.SetUpgradeSelectedCell(null);
                            PnlSuitcase.PnlSuitcase.Instance.OnShow();
                            OnUpgradeItemsRefresh();
                            isUpgrade = false;
                            m_Animtor.Play("cos_change_out");
                            PnlChar.PnlChar.Instance.gameObject.SetActive(true);
                            var isShow = PnlMainMenu.PnlMainMenu.Instance.goSelectedSuitcase.activeSelf;
                            PnlSuitcase.PnlSuitcase.Instance.gameObject.SetActive(isShow);
                            PnlSuitcase.PnlSuitcase.Instance.OnHide();
                        });
                    }
                }
            };
        }
    }
}