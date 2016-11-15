using Assets.Scripts.Common.Manager;

/// UI分析工具自动生成代码
/// BtnItemBoardUI主模块
///
using System;
using UnityEngine;

namespace BtnItemBoard
{
    public class BtnItemBoard : UIPhaseBase
    {
        private static BtnItemBoard instance = null;
        public UILabel txtGoal, txtType, txtCoin, txtCrystal;
        public GameObject goCoin, goCrystal;
        public GameObject goIcon;
        private Collider m_Collider;

        public bool isReach
        {
            set
            {
                goIcon.SetActive(value);
                m_Collider.enabled = !value;
            }
        }

        public static BtnItemBoard Instance
        {
            get
            {
                return instance;
            }
        }

        private void Start()
        {
            instance = this;
            m_Collider = GetComponent<Collider>();
        }

        public override void OnShow()
        {
        }

        public override void OnHide()
        {
        }

        public void OnShow(Achievement ach)
        {
            txtGoal.text = ach.goal.ToString();
            txtType.text = ach.achType.ToString();
            if (ach.awdType == AwardType.Coin)
            {
                goCoin.SetActive(true);
                goCrystal.SetActive(false);
                txtCoin.text = ach.award.ToString();
            }
            else
            {
                goCoin.SetActive(false);
                goCrystal.SetActive(true);
                txtCrystal.text = ach.award.ToString();
            }
            isReach = ach.isReach;
        }
    }
}