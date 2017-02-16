using Assets.Scripts.Common.Manager;
using Assets.Scripts.Common.Tools;
using com.ootii.Messages;
using DG.Tweening;
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
//        public GameObject goSelectedSuitcase, goBtnEnhancement;
        public UILabel txtEnergy;
        public UISprite sprRecoverTime;
        public UITweener twnEnergy, twnCoin, twnCrystal;
        public RandomObjFly coin, crystal, energy, item;
        public float barTime = 1.0f;
        public AnimationCurve barCurve;
//        private bool m_IsUnlockNextSong = false;

        public static PnlMainMenu Instance
        {
            get
            {
                return instance;
            }
        }

//        public override void BeCatched()
//        {
//            instance = this;
//            m_IsUnlockNextSong = TaskStageTarget.isNextUnlock;
//
//        }

        public override void OnShow()
        {
            gameObject.SetActive(true);
            OnUpdateInfo();

//            if (!m_IsUnlockNextSong)
//            {
//                DOTweenUtils.Delay(UpdateEvent, 1.0f);
//            }
//            else
//            {
//                m_IsUnlockNextSong = false;
//                DOTweenUtils.Delay(() =>
//                {
//                    PnlUnlockSong.PnlUnlockSong.Instance.onDisable += UpdateEvent;
//                }, 1.0f);
//            }
        }

        private void UpdateEvent()
        {
            AccountGoldManagerComponent.Instance.DetectAdd();
            AccountPhysicsManagerComponent.Instance.DetectAdd();
            AccountCharmComponent.Instance.DetectAdd();
            AccountCrystalManagerComponent.Instance.DetectAdd();
            AccountLevelManagerComponent.Instance.DetectAdd();
            if (PnlUnlockSong.PnlUnlockSong.Instance != null)
            {
                PnlUnlockSong.PnlUnlockSong.Instance.onDisable = null;
            }
        }

        public void OnUpdateInfo()
        {
            OnEnergyUpdate();
            OnCrystalUpdate();
            OnCoinUpdate();
//            OnCharmUpdate();
//            OnExpUpdate();
        }

//        public void OnExpUpdate()
//        {
//            var expNextLvl = AccountLevelManagerComponent.Instance.NextLvlExp();
//            var curExp = AccountLevelManagerComponent.Instance.GetExp();
//            sprExpBar.transform.localScale = new Vector3(Mathf.Min((float)curExp / (float)expNextLvl, 1f), 1f, 1f);
//        }

//        public void OnCharmUpdate(bool isUpdate = false, Action callFunc = null, bool isCapsuleShake = true)
//        {
//            GameObject capsule = null;
//            var curCapsule = CapsuleManager.instance.curCapsule;
//            var curCharm = AccountCharmComponent.Instance.GetCharm();
//            var maxCharm = curCapsule.charmRequire;
//            txtCharm.text = curCharm.ToString();
//            txtCharmMax.text = maxCharm.ToString();
//            var barValue = 300f * Mathf.Min((float)curCharm / (float)maxCharm, 1.0f);
//            DOTween.To(() => sprCharmBar.width, x => sprCharmBar.width = (int)x, barValue, barTime);
//            for (var i = 0; i < capsules.Length; i++)
//            {
//                if (i == curCapsule.path)
//                {
//                    capsule = capsules[i];
//                }
//                capsules[i].SetActive(i == curCapsule.path);
//            }
//            if (capsule != null)
//            {
//                m_CapsuleAnimator = capsule.transform.GetChild(0).gameObject.GetComponent<Animator>();
//                m_CapsuleAnimator.enabled = true;
//                if (m_CapsuleAnimator.isActiveAndEnabled)
//                {
//                    m_CapsuleAnimator.Play(curCharm >= maxCharm && isCapsuleShake ? "capsule_unlocked" : "capsule_in_nolight");
//                }
//            }
//        }

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