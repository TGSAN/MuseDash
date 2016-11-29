using FormulaBase;

/// UI分析工具自动生成代码
/// PnlServantInfoUI主模块
///
using System;
using UnityEngine;

namespace PnlServantInfo
{
    public class PnlServantInfo : UIPhaseBase
    {
        private static PnlServantInfo instance = null;
        public UILabel txtInfo, txtName, txtExp, txtDescription, txtSaleCoins;
        public UIButton btnSale;

        public Animator animator
        {
            private set; get;
        }

        public static PnlServantInfo Instance
        {
            get
            {
                return instance;
            }
        }

        public void OnEnter()
        {
            animator.Play("pnl_servant_info_in");
        }

        public void OnExit()
        {
            animator.Play("pnl_item_info_out");
        }

        private void Start()
        {
            instance = this;
            animator = GetComponent<Animator>();
        }

        public override void OnShow(FormulaHost h)
        {
            gameObject.SetActive(true);
            animator.enabled = true;
            OnEnter();
            var itemName = h.GetDynamicStrByKey(SignKeys.NAME);
            var exp = (int)h.Result(FormulaKeys.FORMULA_57);
            var description = h.GetDynamicStrByKey(SignKeys.DESCRIPTION);
            var cost = ItemManageComponent.Instance.GetItemMoney(h);

            txtName.text = itemName;
            txtExp.text = exp.ToString();
            txtDescription.text = description;
            txtSaleCoins.text = cost.ToString();

            btnSale.onClick.Clear();
            btnSale.onClick.Add(new EventDelegate(() =>
            {
                PnlItemSale.PnlItemSale.Instance.OnShow(h);
            }));
        }

        public override void OnHide()
        {
        }
    }
}