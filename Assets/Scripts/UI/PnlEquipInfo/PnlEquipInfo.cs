using FormulaBase;

/// UI分析工具自动生成代码
/// PnlEquipInfoUI主模块
///
using System;
using UnityEngine;

namespace PnlEquipInfo
{
    public class PnlEquipInfo : UIPhaseBase
    {
        public UILabel txtVigour, txtStamina, txtStrengh;
        public UILabel txtVigourTo, txtStaminaTo, txtStrenghTo;
        public UILabel txtName;
        public UILabel txtCurLvl, txtNextLvl, txtSaleCoins;
        public UILabel txtDiscription;
        public UILabel txtSuicaseName;
        public UIGrid grdEquips;
        public UIButton btnSale;
        public UIButton btnUpgrade;
        public Transform star;
        private static PnlEquipInfo instance = null;
        private Animator m_Animator;
        public UIButton btnBack;

        public static PnlEquipInfo Instance
        {
            get
            {
                return instance;
            }
        }

        public void OnEnter()
        {
            m_Animator.Play("pnl_equip_info_in");
        }

        public void OnExit()
        {
            m_Animator.Play("pnl_item_info_out");
        }

        private void Awake()
        {
            m_Animator = GetComponent<Animator>();
        }

        private void Start()
        {
            instance = this;
            DOTweenUtil.Delay(() =>
            {
                PnlChar.PnlChar.Instance.onRoleChange += idx => OnExit();
            }, Time.deltaTime);
        }

        public override void OnShow()
        {
        }

        public override void OnHide()
        {
            //btnBack调用此处，使手提包界面选择取消
            if (PnlSuitcase.PnlSuitcase.Instance != null)
            {
                PnlSuitcase.PnlSuitcase.Instance.SetSelectedCell(null);
            }
            OnExit();
        }

        public override void OnShow(FormulaHost host)
        {
            gameObject.SetActive(true);
            m_Animator.enabled = true;
            OnEnter();
            var name = host.GetDynamicStrByKey(SignKeys.NAME);
            var curLvl = host.GetDynamicStrByKey(SignKeys.LEVEL);
            var vigour = host.Result(FormulaKeys.FORMULA_258);
            var stamina = host.Result(FormulaKeys.FORMULA_259);
            var strengh = host.Result(FormulaKeys.FORMULA_264);
            var description = host.GetDynamicStrByKey(SignKeys.DESCRIPTION);
            var cost = host.Result(FormulaKeys.FORMULA_89);

            txtVigour.transform.parent.gameObject.SetActive(vigour > 0);
            txtStamina.transform.parent.gameObject.SetActive(stamina > 0);
            txtStrengh.transform.parent.gameObject.SetActive(strengh > 0);

            txtName.text = name;
            txtCurLvl.text = curLvl;
            txtVigour.text = vigour.ToString();
            txtStamina.text = stamina.ToString();
            txtStrengh.text = strengh.ToString();
            txtDiscription.text = description;
            txtSaleCoins.text = cost.ToString();

            var allName = EquipManageComponent.Instance.GetEquipNameWithSameSuit(host);
            if (allName.Count == 0)
            {
                txtSuicaseName.gameObject.SetActive(false);
            }
            else
            {
                txtSuicaseName.gameObject.SetActive(true);
                var suitcases = EquipManageComponent.Instance.GetEquipWithSameSuit(host);
                txtSuicaseName.text = host.GetDynamicStrByKey(SignKeys.SUIT);
                for (int i = 0; i < grdEquips.transform.childCount; i++)
                {
                    var equipGO = grdEquips.transform.GetChild(i).gameObject;
                    var label = equipGO.GetComponent<UILabel>();
                    label.text = allName[i];
                    var inTxt = equipGO.transform.GetChild(0).gameObject.GetComponent<UILabel>();
                    inTxt.text = allName[i];
                    inTxt.gameObject.SetActive(i < suitcases.Count);
                }
            }

            btnSale.onClick.Clear();
            btnSale.onClick.Add(new EventDelegate(() =>
            {
                ItemManageComponent.Instance.SaleItem(host, (result) =>
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
        }
    }
}