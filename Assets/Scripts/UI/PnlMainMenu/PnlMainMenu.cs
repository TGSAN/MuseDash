using Assets.Scripts.Common.Manager;
using FormulaBase;
using PnlStore;

/// UI分析工具自动生成代码
/// PnlMainMenuUI主模块
///
using System;
using UnityEngine;

namespace PnlMainMenu
{
    public class PnlMainMenu : UIPhaseBase
    {
        private static PnlMainMenu instance = null;
        public GameObject goSelectedSuitcase;
        public UILabel txtEnergy, txtCharm, txtCharmMax;
        public UISprite sprRecoverTime, sprCharmBar;
        public UITweener twnEnergy, twnCoin, twnCrystal;
        public GameObject[] capsules;
        private Animator m_CapsuleAnimator;

        public static PnlMainMenu Instance
        {
            get
            {
                return instance;
            }
        }

        public override void BeCatched()
        {
            instance = this;
            base.BeCatched();
        }

        public override void OnShow()
        {
            OnUpdateInfo();
        }

        public void OnUpdateInfo()
        {
            OnEnergyUpdate();
            OnCrystalUpdate();
            OnCoinUpdate();
            OnCharmUpdate();
        }

        public void OnCharmUpdate(bool isUpdate = false, Action callFunc = null)
        {
            GameObject capsule = null;
            var curCapsule = CapsuleManager.instance.curCapsule;
            var curCharm = AccountCharmComponent.Instance.GetCharm();
            var maxCharm = curCapsule.charmRequire;
            txtCharm.text = curCharm.ToString();
            txtCharmMax.text = maxCharm.ToString();
            sprCharmBar.width = (int)(300f * Mathf.Min((float)curCharm / (float)maxCharm, 1.0f));

            for (var i = 0; i < capsules.Length; i++)
            {
                if (i == curCapsule.path)
                {
                    capsule = capsules[i];
                }
                capsules[i].SetActive(i == curCapsule.path);
            }
            if (capsule != null)
            {
                m_CapsuleAnimator = capsule.transform.GetChild(0).gameObject.GetComponent<Animator>();
                m_CapsuleAnimator.enabled = true;
                if (m_CapsuleAnimator.isActiveAndEnabled)
                {
                    m_CapsuleAnimator.Play(curCharm >= maxCharm ? "capsule_unlocked" : "capsule_in_nolight");
                }
            }
        }

        public void OnEnergyUpdate(bool isUpdate = false, Action callFunc = null)
        {
            var physicalMax = 0;
            physicalMax = AccountPhysicsManagerComponent.Instance.GetMaxPhysical();
            var physical = AccountPhysicsManagerComponent.Instance.GetPhysical();
            txtEnergy.text = physical.ToString() + "/" + physicalMax.ToString();
            sprRecoverTime.fillAmount = (float)physical / (float)physicalMax;
            if (isUpdate)
            {
                if (twnEnergy != null)
                {
                    twnEnergy.enabled = true;
                    twnEnergy.ResetToBeginning();
                    twnEnergy.Play(true);
                    twnEnergy.onFinished.Add(new EventDelegate(() =>
                    { if (callFunc != null) callFunc(); }));
                }
            }
        }

        public void OnCrystalUpdate(bool isUpdate = false, Action callFunc = null)
        {
            if (isUpdate)
            {
                twnCrystal.Play(true);
                twnCrystal.onFinished.Add(new EventDelegate(() =>
                { if (callFunc != null) callFunc(); }));
            }
        }

        public void OnCoinUpdate(bool isUpdate = false, Action callFunc = null)
        {
            if (isUpdate)
            {
                twnCoin.Play(true);
                twnCoin.onFinished.Add(new EventDelegate(() =>
                { if (callFunc != null) callFunc(); }));
            }
        }

        public override void OnHide()
        {
        }
    }
}