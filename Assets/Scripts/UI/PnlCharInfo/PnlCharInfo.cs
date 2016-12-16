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
        public UIButton btnBack;
        public Transform tglsParent;
        public UISprite sprVigour, sprStamina, sprStrengh, sprLuck;
        public UILabel txtEquipInfo, txtServantInfo;

        private Animator m_Animator;
        private CharCos m_SelectedCos;
        private List<SprCos> m_SprCosList = new List<SprCos>();

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
            m_SelectedCos = RoleManageComponent.Instance.GetCurCloth(RoleManageComponent.Instance.GetChoseRoleIdx());
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
            if (!gameObject || !gameObject.activeSelf)
            {
                return;
            }
            var hostList = PnlSuitcase.PnlSuitcase.Instance.upgradeSelectedHost;
            for (var i = 0; i < upgradeTexs.Count; i++)
            {
                var tex = upgradeTexs[i];
                var sprite = tex.transform.parent.GetComponent<UISprite>();
                if (i < hostList.Count)
                {
                    var h = hostList[i];
                    ResourceLoader.Instance.LoadItemIcon(h, tex);
                    sprite.spriteName = "groove_" + h.GetDynamicIntByKey(SignKeys.QUALITY).ToString();
                }
                else
                {
                    tex.mainTexture = null;
                    sprite.spriteName = "groove_space";
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

                sprExpCurBar.transform.localScale = new Vector3(originExpPercent, 1f, 1f);
                sprExpNextBar.transform.localScale = new Vector3(curExpPercent, 1f, 1f);

                curRoleHost.SetDynamicData(SignKeys.LEVEL, originLvl);
                curRoleHost.SetDynamicData(SignKeys.EXP, originExp);
            }
        }

        public void UpdateItemList(int selectID)
        {
            foreach (var item in m_ItemList)
            {
                var itemID = item.host.GetDynamicIntByKey(FormulaBase.SignKeys.BAGINID);
                item.isSelected = selectID == itemID;
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
                itemCellCharInfo.isSelected = equipment.GetDynamicIntByKey(FormulaBase.SignKeys.WHO) != 0;
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
                var idx = PnlChar.PnlChar.Instance.curRoleIdx;
                var roleHost = RoleManageComponent.Instance.GetRole(idx);
                var clothStr = m_SelectedCos.uid;
                RoleManageComponent.Instance.SetFightGirlIndex(idx, () =>
                {
                    roleHost.SetDynamicData(SignKeys.CLOTH, clothStr);
                    roleHost.Save(result =>
                    {
                        if (result)
                        {
                            OnTglChange(idx);
                            OnApplyShow(idx);
                        }
                    });
                });
            };

            foreach (var t in m_SprCosList)
            {
                var tgl = t.selectedTgl;
                tgl.onChange.Clear();
                tgl.onChange.Add(new EventDelegate(() =>
                {
                    OnSelectChange(tgl.transform);
                }));
            }
        }

        public void OnRoleChange(int idx)
        {
            OnTglChange(idx);
            OnApplyShow(idx);
            OnUpgradeItemsRefresh();
            OnPropertyBarShow(idx);
            var isLock = RoleManageComponent.Instance.GetRoleLockedState(idx);
            btnFeed.gameObject.SetActive(!isLock);
            if (PnlChar.PnlChar.Instance.isChangeRole)
            {
                OnExit();
            }
        }

        private void OnTglChange(int idx)
        {
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
                    sprCos.isEquiped = charCos.uid == RoleManageComponent.Instance.GetCurCloth(idx).uid;
                    sprCos.isLock = charCos.isLock;
                }
                else
                {
                    sprCos.isLock = true;
                    sprCos.isEquiped = false;
                    sprCos.isSelected = false;
                }
            }
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
            var isLock = RoleManageComponent.Instance.GetRoleLockedState(idx);
            if (isLock)
            {
                btnApply.gameObject.SetActive(false);
                txtApply.gameObject.SetActive(false);
                return;
            }

            var curSuit = RoleManageComponent.Instance.GetRole(idx).GetDynamicIntByKey(SignKeys.CLOTH);
            if (RoleManageComponent.Instance.GetFightGirlIndex() != idx)
            {
                btnApply.gameObject.SetActive(true);
            }
            else
            {
                btnApply.gameObject.SetActive(curSuit != m_SelectedCos.uid);
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

            if (sprCos.isSelected)
            {
                m_SelectedCos = charCos;
            }
            PnlChar.PnlChar.Instance.OnSpiAnimLoad(PnlChar.PnlChar.Instance.curRoleIdx, m_SelectedCos.path);
            OnApplyShow(PnlChar.PnlChar.Instance.curRoleIdx);
        }
    }
}