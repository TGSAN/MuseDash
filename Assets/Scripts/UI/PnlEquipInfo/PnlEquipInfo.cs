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
        public UILabel txtCurLvl, txtNextLvl;
        public UILabel txtDiscription;
        public UIGrid grdEquips;
        public UIButton btnSale;
        public UIButton btnUpgrade;
        public Transform star;
        private static PnlEquipInfo instance = null;
        private Animator m_Animator;

        public static PnlEquipInfo Instance
        {
            get
            {
                return instance;
            }
        }

        private void Awake()
        {
            m_Animator = GetComponent<Animator>();
            btnSale.onClick.Add(new EventDelegate()=>
            {

            });
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
        }

        public override void OnShow(FormulaHost host)
        {
            gameObject.SetActive(true);
            m_Animator.enabled = true;
            m_Animator.Play("pnl_equip_info_in");
            var name = host.GetDynamicStrByKey(SignKeys.NAME);
            var curLvl = host.GetDynamicStrByKey(SignKeys.LEVEL);
            var vigour = host.Result(FormulaKeys.FORMULA_258);
            var stamina = host.Result(FormulaKeys.FORMULA_259);
            var strengh = host.Result(FormulaKeys.FORMULA_264);
            var description = host.GetDynamicStrByKey(SignKeys.DESCRIPTION);

            txtVigour.transform.parent.gameObject.SetActive(vigour > 0);
            txtStamina.transform.parent.gameObject.SetActive(stamina > 0);
            txtStrengh.transform.parent.gameObject.SetActive(strengh > 0);

            txtName.text = name;
            txtCurLvl.text = curLvl;
            txtVigour.text = vigour.ToString();
            txtStamina.text = stamina.ToString();
            txtStrengh.text = strengh.ToString();
            txtDiscription.text = description;
            
        }
    }
}