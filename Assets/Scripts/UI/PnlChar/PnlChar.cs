/// UI分析工具自动生成代码
/// PnlCharUI主模块
///
using System;
using System.Collections.Generic;
using UnityEngine;

namespace PnlChar
{
    public class PnlChar : UIPhaseBase
    {
        private static PnlChar instance = null;

        public Transform spiAnimParent;
        public ItemImageEquip.ItemImageEquip[] items;
        private int m_CurRoleIdx = 1;
        private List<FormulaBase.FormulaHost> m_Equipments = new List<FormulaBase.FormulaHost>();

        public static PnlChar Instance
        {
            get
            {
                return instance;
            }
        }

        private void Start()
        {
            instance = this;
            Init();
        }

        public override void OnShow()
        {
            InitInfo();
        }

        public override void OnHide()
        {
        }

        private void Init()
        {
        }

        private void InitInfo()
        {
            m_CurRoleIdx = FormulaBase.RoleManageComponent.Instance.GetFightGirlIndex();
            OnRoleChange(m_CurRoleIdx);
        }

        private void OnRoleChange(int roleIdx)
        {
            m_Equipments = FormulaBase.EquipManageComponent.Instance.GetRoleEquipedItem(roleIdx);
        }
    }
}