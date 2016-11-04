using FormulaBase;

/// UI分析工具自动生成代码
/// PnlEquipInfoUI主模块
///
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace PnlEquipInfo
{
    public class PnlEquipInfo : UIPhaseBase
    {
        public UILabel txtVigour, txtStamina, txtStrengh;
        public UILabel txtVigourTo, txtStaminaTo, txtStrenghTo;
        public UILabel txtName, txtType;
        public UILabel txtCurLvl, txtNextLvl, txtSaleCoins;
        public UILabel txtDiscription;
        public UILabel txtSuicaseName;
        public UILabel txtSuicaseEffect;
        public UIGrid grdEquips;
        public UIButton btnSale;
        public UIButton btnUpgrade;
        public UIButton btnUpgradeBack;
        public UIButton btnConfirm;
        public UISprite sprExpCurBar, sprExpNextBar;
        public Transform star;
        public GameObject charBack, suitcaseBack, itemUpgrade;
        public Transform upgradeItemsParent;
        private static PnlEquipInfo instance = null;
        private bool m_IsUpgrade = false;
        public List<UITexture> upgradeTexs = new List<UITexture>();
        public List<UILabel> upgradeTxts = new List<UILabel>();

        public static PnlEquipInfo Instance
        {
            get
            {
                return instance;
            }
        }

        public Animator animator
        {
            get;
            private set;
        }

        public FormulaHost host
        {
            private set;
            get;
        }

        public bool isUpgrade
        {
            get { return m_IsUpgrade; }
            set
            {
                m_IsUpgrade = value;
                UpgradeInfoActive(m_IsUpgrade);
                PnlSuitcase.PnlSuitcase.Instance.isUpgrade = m_IsUpgrade;
                sprExpNextBar.gameObject.SetActive(m_IsUpgrade);
                if (m_IsUpgrade)
                {
                    PnlChar.PnlChar.Instance.gameObject.SetActive(false);
                }
                else
                {
                    OnUpgradeItemsRefresh();
                }
            }
        }

        public void Play(string animName)
        {
            animator.Play(animName);
        }

        public void OnEnter()
        {
            Play("pnl_equip_info_in");
        }

        public void OnExit()
        {
            Play("pnl_item_info_out");
        }

        private void Awake()
        {
            animator = GetComponent<Animator>();
        }

        private void Start()
        {
            DOTweenUtils.Delay(() =>
            {
                PnlChar.PnlChar.Instance.onRoleChange += idx => OnExit();
            }, Time.deltaTime);
        }

        private void Update()
        {
            charBack.SetActive(PnlChar.PnlChar.Instance != null && PnlChar.PnlChar.Instance.gameObject.activeSelf && !itemUpgrade.activeSelf);
            suitcaseBack.SetActive(PnlSuitcase.PnlSuitcase.Instance != null && PnlSuitcase.PnlSuitcase.Instance.gameObject.activeSelf);
        }

        public override void OnShow()
        {
        }

        public override void BeCatched()
        {
            instance = this;
        }

        public override void OnHide()
        {
            //btnBack调用此处，使手提包界面选择取消
            PnlSuitcase.PnlSuitcase.Instance.SetSelectedCell(null);
            //退出升级状态
            isUpgrade = false;
            //退出动画
            OnExit();
        }

        public void OnUpgradeState(bool isEnter)
        {
            UpgradeInfoActive(isEnter);
            PnlSuitcase.PnlSuitcase.Instance.isUpgrade = isEnter;
            if (isEnter)
            {
                PnlChar.PnlChar.Instance.gameObject.SetActive(false);
            }
            else
            {
                OnUpgradeItemsRefresh();
            }
        }

        public override void OnShow(FormulaHost h)
        {
            gameObject.SetActive(true);
            animator.enabled = true;
            OnEnter();

            Action updateInfo = () =>
            {
                var name = h.GetDynamicStrByKey(SignKeys.NAME);
                var type = h.GetDynamicStrByKey(SignKeys.TYPE);
                var curLvl = h.GetDynamicStrByKey(SignKeys.LEVEL);
                var vigour = h.Result(FormulaKeys.FORMULA_50);
                var stamina = h.Result(FormulaKeys.FORMULA_51);
                var strengh = h.Result(FormulaKeys.FORMULA_56);
                var description = h.GetDynamicStrByKey(SignKeys.DESCRIPTION);
                var cost = ItemManageComponent.Instance.GetItemMoney(h);
                var effect = h.GetDynamicStrByKey(SignKeys.SUIT_EFFECT_DESC);

                txtVigour.transform.parent.gameObject.SetActive(vigour > 0);
                txtStamina.transform.parent.gameObject.SetActive(stamina > 0);
                txtStrengh.transform.parent.gameObject.SetActive(strengh > 0);

                txtName.text = name;
                txtType.text = type;
                txtCurLvl.text = curLvl;
                txtVigour.text = vigour.ToString();
                txtStamina.text = stamina.ToString();
                txtStrengh.text = strengh.ToString();
                txtDiscription.text = description;
                txtSaleCoins.text = cost.ToString();
                txtSuicaseEffect.text = effect;
                var allName = EquipManageComponent.Instance.GetEquipNameWithSameSuit(h);
                if (allName.Count == 0)
                {
                    txtSuicaseName.gameObject.SetActive(false);
                }
                else
                {
                    txtSuicaseName.gameObject.SetActive(true);
                    var suitcases = EquipManageComponent.Instance.GetEquipWithSameSuit(h);
                    txtSuicaseName.text = h.GetDynamicStrByKey(SignKeys.SUIT);
                    var idx = 0;
                    for (int i = 0; i < grdEquips.transform.childCount; i++)
                    {
                        var equipGO = grdEquips.transform.GetChild(i).gameObject;
                        var label = equipGO.GetComponent<UILabel>();
                        label.text = allName[i];
                        var inTxt = equipGO.transform.GetChild(0).gameObject.GetComponent<UILabel>();
                        inTxt.text = allName[i];
                        if (idx < suitcases.Count)
                        {
                            if (suitcases[idx].GetDynamicStrByKey(SignKeys.NAME) == allName[i])
                            {
                                idx++;
                                inTxt.gameObject.SetActive(true);
                            }
                            else
                            {
                                inTxt.gameObject.SetActive(false);
                            }
                        }
                        else
                        {
                            inTxt.gameObject.SetActive(false);
                        }
                    }
                }
            };
            updateInfo();

            btnSale.onClick.Clear();
            btnSale.onClick.Add(new EventDelegate(() =>
            {
                ItemManageComponent.Instance.SaleItem(h, (result) =>
                {
                    if (result)
                    {
                        PnlCharInfo.PnlCharInfo.Instance.OnShow();
                        PnlCharInfo.PnlCharInfo.Instance.OnExit();
                        OnExit();
                        if (PnlChar.PnlChar.Instance != null)
                        {
                            PnlChar.PnlChar.Instance.OnEquipLoad(PnlChar.PnlChar.Instance.curRoleIdx);
                        }

                        if (PnlSuitcase.PnlSuitcase.Instance.gameObject.activeSelf)
                        {
                            PnlSuitcase.PnlSuitcase.Instance.OnShow();
                        }
                        CommonPanel.GetInstance().ShowWaittingPanel(false);
                    }
                });
            }));

            UIEventListener.Get(btnUpgrade.gameObject).onClick = (go) =>
            {
                if (ItemManageComponent.Instance.IsItemLvlMax(host))
                {
                    CommonPanel.GetInstance().ShowText("物品已达最高等级，无法升级");
                }
                else
                {
                    Play("item_upgrade_in");
                    isUpgrade = true;
                }
            };
            UIEventListener.VoidDelegate callFunc = (go) =>
            {
                var isShow = PnlMainMenu.PnlMainMenu.Instance.goSelectedSuitcase.activeSelf;
                PnlChar.PnlChar.Instance.gameObject.SetActive(!isShow);
                PnlSuitcase.PnlSuitcase.Instance.gameObject.SetActive(isShow);
                isUpgrade = false;
            };
            UIEventListener.Get(btnUpgradeBack.gameObject).onClick = callFunc;
            UIEventListener.Get(btnConfirm.gameObject).onClick = (go) =>
            {
                var hosts = PnlSuitcase.PnlSuitcase.Instance.upgradeSelectedHost;
                if (hosts.Count > 0)
                {
                    ItemManageComponent.Instance.ItemLevelUp(h, hosts, (result) =>
                    {
                        PnlSuitcase.PnlSuitcase.Instance.SetUpgradeSelectedCell(null);
                        PnlSuitcase.PnlSuitcase.Instance.OnShow();
                        updateInfo();
                        callFunc(gameObject);
                    });
                }
            };
            host = h;
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
                var txt = upgradeTxts[i];
                if (i < hostList.Count)
                {
                    var h = hostList[i];
                    var texName = h.GetDynamicStrByKey(SignKeys.ICON);
                    var lvl = h.GetDynamicIntByKey(SignKeys.LEVEL);
                    ResourceLoader.Instance.Load(texName, resObj => tex.mainTexture = resObj as Texture);
                    txt.text = lvl.ToString();
                }
                else
                {
                    tex.mainTexture = null;
                }
                txt.transform.parent.GetChild(1).gameObject.SetActive(i < hostList.Count);
                txt.gameObject.SetActive(i < hostList.Count);
            }
            if (host != null)
            {
                var originLvl = host.GetDynamicIntByKey(SignKeys.LEVEL);
                var originExp = host.GetDynamicIntByKey(SignKeys.EXP);
                var originRequiredExp = ConfigPool.Instance.GetConfigIntValue("experience", originLvl.ToString(), "eqpt_exp");
                var originExpPercent = (float)originExp / (float)originRequiredExp;

                host = ItemManageComponent.Instance.ItemLevelUp(host, hostList, null, false);

                var afterLvl = host.GetDynamicIntByKey(SignKeys.LEVEL);
                var afterExp = host.GetDynamicIntByKey(SignKeys.EXP);
                var vigourTo = (int)host.Result(FormulaKeys.FORMULA_50);
                var staminaTo = (int)host.Result(FormulaKeys.FORMULA_51);
                var strenghTo = (int)host.Result(FormulaKeys.FORMULA_56);
                var curExpPercent = (float)afterExp / (float)originRequiredExp;
                curExpPercent = afterLvl != originLvl ? 1.0f : curExpPercent;

                txtNextLvl.text = afterLvl.ToString();
                txtVigourTo.text = vigourTo.ToString();
                txtStaminaTo.text = staminaTo.ToString();
                txtStrenghTo.text = strenghTo.ToString();

                sprExpCurBar.fillAmount = originExpPercent;
                sprExpNextBar.fillAmount = curExpPercent;

                host.SetDynamicData(SignKeys.LEVEL, originLvl);
                host.SetDynamicData(SignKeys.EXP, originExp);
            }
        }

        private void UpgradeInfoActive(bool enter)
        {
            txtNextLvl.transform.parent.gameObject.SetActive(enter);
            txtVigourTo.transform.parent.gameObject.SetActive(enter);
            txtStaminaTo.transform.parent.gameObject.SetActive(enter);
            txtStrenghTo.transform.parent.gameObject.SetActive(enter);
        }
    }
}