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
        public UILabel txtSaleCoins;
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
            PnlSuitcase.PnlSuitcase.Instance.SetSelectedCell(null);
            OnExit();
        }

        public override void OnShow(FormulaHost h)
        {
            gameObject.SetActive(true);
            m_Animator.enabled = true;
            OnEnter();
            var name = h.GetDynamicStrByKey(SignKeys.NAME);
            var exp = h.Result(FormulaKeys.FORMULA_57);
            var description = h.GetDynamicStrByKey(SignKeys.DESCRIPTION);
            var cost = ItemManageComponent.Instance.GetItemMoney(h);

            txtName.text = name;
            txtExp.text = exp.ToString();
            txtDiscription.text = description;
            txtSaleCoins.text = cost.ToString();

            btnSale.onClick.Clear();
            btnSale.onClick.Add(new EventDelegate(() =>
            {
                ItemManageComponent.Instance.SaleItem(h, (result) =>
                {
                    if (result)
                    {
                        if (h.GetDynamicIntByKey(SignKeys.STACKITEMNUMBER) < 1)
                        {
                            OnExit();
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