using FormulaBase;

/// UI分析工具自动生成代码
/// PnlCharCosChoseUI主模块
///
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace PnlCharCosChose
{
    public class PnlCharCosChose : UIPhaseBase
    {
        public Transform spiPoint;
        public UIButton btnLeft, btnRight;
        public UILabel txtName, txtDescription, txtIdx;
        public Transform[] suits;
        private CharCos m_CharCos;
        private static PnlCharCosChose instance = null;
        private Animator m_Animator;

        public static PnlCharCosChose Instance
        {
            get
            {
                return instance;
            }
        }

        public override void OnShow()
        {
        }

        public override void OnHide()
        {
        }

        public override void BeCatched()
        {
            instance = this;
            m_Animator = GetComponent<Animator>();
            btnLeft.onClick.Add(new EventDelegate(() =>
            {
                var idx = PnlCharChose.PnlCharChose.Instance.clothIdx;
                var cloths = FormulaBase.RoleManageComponent.Instance.GetCloths(PnlCharChose.PnlCharChose.Instance.choseType);
                if (--idx < 0)
                {
                    idx = cloths.Count - 1;
                }
                var suit = cloths[idx];
                PnlCharChose.PnlCharChose.Instance.clothIdx = idx;
                OnShow(suit);
            }));
            btnRight.onClick.Add(new EventDelegate(() =>
            {
                var idx = PnlCharChose.PnlCharChose.Instance.clothIdx;
                var cloths = FormulaBase.RoleManageComponent.Instance.GetCloths(PnlCharChose.PnlCharChose.Instance.choseType);
                if (++idx >= cloths.Count)
                {
                    idx = 0;
                }
                var suit = cloths[idx];
                PnlCharChose.PnlCharChose.Instance.clothIdx = idx;
                OnShow(suit);
            }));
        }

        public void OnShow(CharCos c)
        {
            m_Animator.enabled = true;
            m_Animator.Play("char_cos_chose_in");
            gameObject.SetActive(true);
            m_CharCos = c;
            UpdatePanel();
        }

        private void UpdatePanel()
        {
            txtName.text = m_CharCos.name;
            txtDescription.text = m_CharCos.description;

            if (m_CharCos.uid % 10 == 0)
            {
                foreach (var suit in suits)
                {
                    suit.gameObject.SetActive(false);
                }
            }
            else
            {
                var allName = EquipManageComponent.Instance.GetEquipNameInSuit(m_CharCos.name);
                var ownName = m_CharCos.host.Select(formulaHost => formulaHost.GetDynamicStrByKey(SignKeys.NAME)).ToList();
                for (var i = 0; i < allName.Count; i++)
                {
                    var suit = suits[i];
                    suit.gameObject.SetActive(true);
                    var txt = suit.GetComponent<UILabel>();
                    txt.text = allName[i];
                    suit.GetChild(0).gameObject.GetComponent<UILabel>().text = allName[i];
                    suit.GetChild(0).gameObject.SetActive(ownName.Contains(allName[i]));
                }
            }
            spiPoint.DestroyChildren();
            GameObject go = null;
            ResourceLoader.Instance.Load(m_CharCos.path, res =>
            {
                go = Instantiate(res) as GameObject;
                go.transform.SetParent(spiPoint);
                go.GetComponent<Animator>().enabled = false;
                go.GetComponent<SpineSynchroObjects>().enabled = false;
                go.GetComponent<SpineMountController>().enabled = false;
                go.SetActive(true);
                go.transform.localPosition = Vector3.zero;
                go.transform.localEulerAngles = Vector3.zero;
                var skeletonAnim = go.GetComponent<SkeletonAnimation>();
                skeletonAnim.loop = true;
                skeletonAnim.AnimationName = "standby";
                go.GetComponent<Renderer>().sortingOrder = 100;
            }, ResourceLoader.RES_FROM_LOCAL);

            txtIdx.text = "SKIN " + (PnlCharChose.PnlCharChose.Instance.clothIdx + 1).ToString();
        }
    }
}