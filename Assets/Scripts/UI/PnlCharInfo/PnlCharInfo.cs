using Assets.Scripts.Common;
using FormulaBase;

/// UI分析工具自动生成代码
/// PnlCharInfoUI主模块
///
using System;
using System.Collections.Generic;
using System.Linq;
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
        public UILabel txtNextVigour, txtNextStamina, txtNextStrengh, txtNextLuck, txtNextLvl, txtApply;
        public UILabel txtCurVigour, txtCurStamina, txtCurStrengh, txtCurLuck, txtCurLvl;
        public UISprite sprExpCurBar, sprExpNextBar;
        public List<UITexture> upgradeTexs = new List<UITexture>();
        public Transform upgradeItemsParent;
        public UIButton btnBack, btnCosChangeBack;
        public Transform tglsParent;
        public UISprite sprVigour, sprStamina, sprStrengh, sprLuck;
        public UILabel txtEquipInfo, txtServantInfo;

        private Animator m_Animator;
        private List<CharCos> m_SelectedCosList = new List<CharCos>();
        private CharCos m_SelectedCos;
        private List<SprCos> m_SprCosList = new List<SprCos>();
        private int m_LastSelected = 1;

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

        public void OnEnter()
        {
            m_Animator.Play("char_info_in");
        }

        public void OnExit()
        {
            if (m_Animator.GetCurrentAnimatorStateInfo(0).IsName("pnl_items_choose_in"))
            {
                m_Animator.Play("pnl_items_choose_out");
            }
        }

        public override void OnShow()
        {
            UpdateUI();
            InitEvent();
        }

        public override void BeCatched()
        {
            m_Animator = GetComponent<Animator>();
            instance = this;
            m_SprCosList = tglsParent.GetComponentsInChildren<SprCos>().ToList();
            m_SelectedCosList = RoleManageComponent.Instance.GetClothList(RoleManageComponent.Instance.GetChoseRoleIdx());
            m_SelectedCos = m_SelectedCosList[0];
        }

        private void OnEnable()
        {
            OnShow();
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
            for (var i = 0; i < upgradeTexs.Count; i++)
            {
                var tex = upgradeTexs[i];
                if (i < hostList.Count)
                {
                    var h = hostList[i];
                    ResourceLoader.Instance.LoadItemIcon(h, tex);
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

                UpgradeManager.instance.RoleLevelUp(curRoleHost, PnlSuitcase.PnlSuitcase.Instance.upgradeSelectedHost, null, false);

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
                var itemID = item.host.GetDynamicIntByKey(FormulaBase.SignKeys.BAGINID);
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
            cellItemParent.parent.gameObject.SetActive(allEquipments.Length != 0);
            txtEquipInfo.gameObject.SetActive(allEquipments.Length == 0);
        }

        private void InitEvent()
        {
            UIEventListener.Get(btnFeed.gameObject).onClick = (go) =>
            {
                var curRoleHost = RoleManageComponent.Instance.GetGirlByIdx(PnlChar.PnlChar.Instance.curRoleIdx);
                if (UpgradeManager.instance.IsRoleLvlMax(curRoleHost))
                {
                    CommonPanel.GetInstance().ShowText("人物已达最高等级，无法升级");
                }
                else
                {
                    isUpgrade = true;
                    gameObject.SetActive(false);
                    DOTweenUtils.Delay(() =>
                    {
                        gameObject.SetActive(true);
                        m_Animator.Play("cos_change");
                        PnlSuitcase.PnlSuitcase.Instance.gameObject.SetActive(true);
                        PnlChar.PnlChar.Instance.gameObject.SetActive(false);
                    }, 0.1f);
                }
                PnlSuitcase.PnlSuitcase.Instance.ResetPos();
            };
            UIEventListener.Get(btnBack.gameObject).onClick = (go) =>
            {
                DOTweenUtils.Delay(() =>
                {
                    m_Animator.Play("char_info_in");
                }, 0.1f);
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
                        UpgradeManager.instance.RoleLevelUp(curRoleHost, hosts, (result) =>
                        {
                            PnlSuitcase.PnlSuitcase.Instance.SetUpgradeSelectedCell(null);
                            PnlSuitcase.PnlSuitcase.Instance.OnShow();
                            OnUpgradeItemsRefresh();
                            isUpgrade = false;
                            m_Animator.Play("cos_change_out");
                            PnlChar.PnlChar.Instance.gameObject.SetActive(true);
                            var isShow = PnlMainMenu.PnlMainMenu.Instance.goSelectedSuitcase.activeSelf;
                            PnlSuitcase.PnlSuitcase.Instance.gameObject.SetActive(isShow);
                            PnlSuitcase.PnlSuitcase.Instance.OnHide();
                        });
                    }
                }
                else
                {
                    CommonPanel.GetInstance().ShowText("请在左边手提箱中选择食物");
                }
            };
            UIEventListener.Get(btnApply.gameObject).onClick = (go) =>
            {
                var roleHost = RoleManageComponent.Instance.GetRole(PnlChar.PnlChar.Instance.curRoleIdx);
                var clothStr = m_SelectedCosList.Aggregate(string.Empty, (current, charCose) => current + (charCose.uid + ","));
                clothStr = clothStr.Substring(0, clothStr.Length - 1);
                roleHost.SetDynamicData(SignKeys.SUIT_GROUP, clothStr);
                roleHost.Save();
            };

            foreach (var t in m_SprCosList)
            {
                var tgl = t.selectedTgl;
                tgl.onChange.Clear();
                tgl.onChange.Add(new EventDelegate(() =>
                {
                    DOTweenUtils.Delay(() =>
                    {
                        OnSelectChange(tgl.transform);
                    }, Time.deltaTime);
                }));
            }
        }

        public void OnRoleChange(int idx)
        {
            m_SelectedCosList = RoleManageComponent.Instance.GetClothList(idx);
            var allCharCos = RoleManageComponent.Instance.GetCloths(idx);
            var curPath = PnlChar.PnlChar.Instance.animPath[idx - 1];
            var selectedSpiIdx = allCharCos.FindIndex(c => c.path == curPath);
            selectedSpiIdx = selectedSpiIdx == -1 ? 0 : selectedSpiIdx;
            m_SelectedCos = allCharCos[selectedSpiIdx];
            for (var i = 0; i < tglsParent.childCount; i++)
            {
                var sprCos = m_SprCosList[i];
                if (i < allCharCos.Count)
                {
                    var charCos = allCharCos[i];
                    sprCos.isSelected = charCos.uid == m_SelectedCos.uid;
                    sprCos.isInGroup = m_SelectedCosList.Exists(c => c.uid == charCos.uid);
                    sprCos.isLock = charCos.isLock;
                }
                else
                {
                    sprCos.isInGroup = false;
                    sprCos.isLock = true;
                    sprCos.isSelected = false;
                }
            }
            OnApplyShow(idx);
            OnUpgradeItemsRefresh();
            OnPropertyBarShow(idx);
            OnExit();
            var isLock = RoleManageComponent.Instance.GetRoleLockedState(idx);
            txtApply.gameObject.SetActive(!isLock);
            btnFeed.gameObject.SetActive(!isLock);
        }

        private void OnPropertyBarShow(int idx)
        {
            var role = RoleManageComponent.Instance.GetRole(idx);
            var maxVigour = 0;
            var maxStamina = 0;
            var maxStrengh = 0;
            var curVigour = role.Result(FormulaKeys.FORMULA_0);
            var curStamina = role.Result(FormulaKeys.FORMULA_36);
            var curStrengh = role.Result(FormulaKeys.FORMULA_37);

            RoleManageComponent.Instance.GetMaxLevelProperties(role, ref maxVigour, ref maxStamina, ref maxStrengh);
            sprVigour.transform.localScale = new Vector3((float)curVigour / (float)maxVigour, 1.0f, 1.0f);
            sprStamina.transform.localScale = new Vector3((float)curStamina / (float)maxStamina, 1.0f, 1.0f);
            sprStrengh.transform.localScale = new Vector3((float)curStrengh / (float)maxStrengh, 1.0f, 1.0f);
            sprLuck.gameObject.SetActive(false);
        }

        private void OnApplyShow(int idx)
        {
            var clothStr = m_SelectedCosList.Aggregate(string.Empty, (current, charCose) => current + (charCose.uid + ","));
            if (clothStr.Length > 0)
            {
                clothStr = clothStr.Substring(0, clothStr.Length - 1);
            }
            var suitStr = RoleManageComponent.Instance.GetRole(idx).GetDynamicStrByKey(SignKeys.SUIT_GROUP);
            btnApply.gameObject.SetActive(suitStr != clothStr && !RoleManageComponent.Instance.GetRoleLockedState(idx));
            if ((suitStr == "0" && clothStr == (idx * 10).ToString()) || string.IsNullOrEmpty(clothStr))
            {
                btnApply.gameObject.SetActive(false);
            }
            txtApply.gameObject.SetActive(!btnApply.gameObject.activeSelf);
        }

        public void OnSelectChange(Transform t)
        {
            var allCharCos = RoleManageComponent.Instance.GetCloths(PnlChar.PnlChar.Instance.curRoleIdx);
            var idx = t.GetSiblingIndex();
            var sprCos = t.GetComponent<SprCos>();
            if (idx >= allCharCos.Count) return;
            var charCos = allCharCos[idx];
            for (var i = 0; i < m_SprCosList.Count; i++)
            {
                if (m_SprCosList[i].isInGroup)
                {
                    m_LastSelected = i;
                }
            }
            if (m_SprCosList.All(sprCose => !sprCose.isInGroup))
            {
                m_SprCosList[m_LastSelected].isInGroup = true;
            }

            if (sprCos.isSelected)
            {
                m_SelectedCos = charCos;
            }

            if (sprCos.isInGroup)
            {
                if (!m_SelectedCosList.Exists(c => c.uid == charCos.uid))
                {
                    m_SelectedCosList.Add(charCos);
                }
            }
            else
            {
                m_SelectedCosList.RemoveAll(c => c.uid == charCos.uid);
            }
            PnlChar.PnlChar.Instance.OnSpiAnimLoad(PnlChar.PnlChar.Instance.curRoleIdx, m_SelectedCos.path);
            var roleHost = RoleManageComponent.Instance.GetRole(PnlChar.PnlChar.Instance.curRoleIdx);
            roleHost.SetDynamicData(SignKeys.CLOTH, m_SelectedCos.uid);
            m_SelectedCosList.Sort((l, r) => l.uid - r.uid);
            OnApplyShow(PnlChar.PnlChar.Instance.curRoleIdx);
        }
    }
}