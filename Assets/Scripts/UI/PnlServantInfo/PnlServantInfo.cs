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
        public UILabel txtName, txtLvl, txtDescription, txtLv;
        public UISprite sprVigour, sprStamina, sprStrengh;
        public UIButton btnApply, btnUpgrade;
        public UISprite sprArrow;

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
            if (h.GetDynamicStrByKey(SignKeys.TYPE) == "servant")
            {
                animator.Play("pnl_servant_info_in");
            }
            else
            {
                animator.Play("pnl_servantsoul_info_in");
            }
            var itemName = h.GetDynamicStrByKey(SignKeys.NAME);
            var description = h.GetDynamicStrByKey(SignKeys.DESCRIPTION);
            var lvl = h.GetDynamicIntByKey(SignKeys.LEVEL);
            var quality = h.GetDynamicIntByKey(SignKeys.QUALITY);

            var color = PnlEquipInfo.PnlEquipInfo.Instance.colorName[quality - 1];
            txtName.text = itemName;
            txtName.color = color;
            txtLv.color = color;
            txtLvl.color = color;
            txtDescription.text = description;
            txtLvl.text = lvl.ToString();

            sprVigour.gameObject.SetActive(false);
            sprStamina.gameObject.SetActive(false);
            sprStrengh.gameObject.SetActive(false);
            sprArrow.gameObject.SetActive(false);

            var isOwned = h.GetDynamicIntByKey(SignKeys.WHO) == 1;
            btnUpgrade.gameObject.SetActive(isOwned);
            btnApply.gameObject.SetActive(!isOwned);
        }

        public override void OnHide()
        {
        }
    }
}