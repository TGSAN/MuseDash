using FormulaBase;

/// UI分析工具自动生成代码
/// PnlFoodInfoUI主模块
///
using System;
using UnityEngine;

namespace PnlFoodInfo
{
    public class PnlFoodInfo : UIPhaseBase
    {
        public UILabel txtName;
        public UILabel txtCurLvl, txtSaleCoins;
        public UILabel txtDiscription;
        public UILabel txtExp;
        public UIButton btnSale;
        private static PnlFoodInfo instance = null;
        private Animator m_Animator;

        public static PnlFoodInfo Instance
        {
            get
            {
                return instance;
            }
        }

        public void OnEnter()
        {
            m_Animator.Play("pnl_food_info_in");
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
            var exp = host.GetDynamicIntByKey(SignKeys.EXP);
            var description = host.GetDynamicStrByKey(SignKeys.DESCRIPTION);
            var cost = host.Result(FormulaKeys.FORMULA_89);

            txtName.text = name;
            txtCurLvl.text = curLvl;
            txtExp.text = exp.ToString();
            txtDiscription.text = description;
            txtSaleCoins.text = cost.ToString();

            btnSale.onClick.Clear();
            btnSale.onClick.Add(new EventDelegate(() =>
            {
                ItemManageComponent.Instance.SaleItem(host, (result) =>
                {
                    if (result)
                    {
                        OnExit();

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