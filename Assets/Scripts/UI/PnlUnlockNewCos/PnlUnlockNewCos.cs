using FormulaBase;

/// UI分析工具自动生成代码
/// PnlUnlockNewCosUI主模块
///
using System;
using System.Collections.Generic;
using UnityEngine;

namespace PnlUnlockNewCos
{
    public class PnlUnlockNewCos : UIPhaseBase
    {
        public Transform spiParent;
        public UILabel txtName;
        public UIButton btnCheck, btnNext;
        private readonly List<CharCos> m_Coss = new List<CharCos>();
        private static PnlUnlockNewCos instance = null;

        public static PnlUnlockNewCos Instance
        {
            get
            {
                return instance;
            }
        }

        public override void BeCatched()
        {
            instance = this;
        }

        public void OnShow(CharCos cos)
        {
            m_Coss.Add(cos);
            Callback<CharCos> updateInfo = (charCos) =>
            {
                txtName.text = charCos.name;
                spiParent.DestroyChildren();
                ResourceLoader.Instance.Load(charCos.path, res =>
                {
                    if (res != null)
                    {
                        var go = Instantiate(res) as GameObject;
                        go.transform.SetParent(spiParent, false);
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
                    }
                });
                btnNext.gameObject.SetActive(m_Coss.Count > 1);
                btnCheck.gameObject.SetActive(m_Coss.Count <= 1);
                gameObject.SetActive(true);
            };
            updateInfo(cos);
            UIEventListener.Get(btnCheck.gameObject).onClick = go =>
            {
                var curCos = m_Coss[m_Coss.Count - 1];
                m_Coss.Remove(curCos);
                PnlChar.PnlChar.Instance.OnShowCos(curCos);
            };
            UIEventListener.Get(btnNext.gameObject).onClick = go =>
            {
                var curCos = m_Coss[m_Coss.Count - 1];
                m_Coss.Remove(curCos);
                updateInfo(m_Coss[m_Coss.Count - 1]);
            };
        }
    }
}