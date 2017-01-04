using Assets.Scripts.Common.Manager;
using FormulaBase;
using LitJson;

/// UI分析工具自动生成代码
/// PnlCapsuleOpenUI主模块
///
using System;
using System.Linq;
using UnityEngine;

namespace PnlCapsuleOpen
{
    public class PnlCapsuleOpen : UIPhaseBase
    {
        public UILabel txtCharm, txtCharmMax;
        public UIButton btnOpen, btnPurchase, btnExit;
        public UITexture[] texItems;
        public GameObject[] capsules;
        public UILabel[] txtItemName;
        public Action onExit;
        private static PnlCapsuleOpen instance = null;
        private string m_AnimName = string.Empty;

        public Animator animator
        {
            get; private set;
        }

        public static PnlCapsuleOpen Instance
        {
            get
            {
                return instance;
            }
        }

        private void Awake()
        {
            animator = GetComponent<Animator>();
        }

        public override void OnShow()
        {
            gameObject.SetActive(true);
            animator.enabled = true;
            animator.Play("capsule_open_in");
            var curCapsule = CapsuleManager.instance.curCapsule;
            txtCharm.text = AccountCharmComponent.Instance.GetCharm().ToString();
            txtCharmMax.text = curCapsule.charmRequire.ToString();

            for (var i = 0; i < capsules.Length; i++)
            {
                capsules[i].SetActive(i == curCapsule.path);
            }

            texItems.ToList().ForEach(t => t.transform.parent.gameObject.SetActive(false));
            for (int i = 0, k = 0; i < curCapsule.itemsID.Count; i++, k++)
            {
                var id = curCapsule.itemsID[i];
                var itemConfig = ItemManageComponent.Instance.GetItemConfigByUID(id);
                var itemName = itemConfig["name"].ToString();
                txtItemName[k].text = itemName;
                var tex = texItems[k];

                tex.transform.parent.gameObject.SetActive(true);
                var iconPath = "items/icon/" + itemConfig["icon"].ToString();
                var quality = (int)itemConfig["quality"];
                ResourceLoader.Instance.Load(iconPath, res => tex.mainTexture = res as Texture);
                for (var j = 0; j < tex.transform.childCount; j++)
                {
                    var go = tex.transform.GetChild(j).gameObject;
                    go.SetActive((j + 1) == quality);
                }
            }
            m_AnimName = "capsule_open_item" + curCapsule.itemsID.Count.ToString();

            var isPurchase = AccountCharmComponent.Instance.GetCharm() < CapsuleManager.instance.curCapsule.charmRequire;
            btnPurchase.GetComponentInChildren<UILabel>().text =
                    ((int)
                        (CapsuleManager.instance.curCapsule.charmRequire *
                         StoreManageComponent.Instance.Host.Result(FormulaKeys.FORMULA_115))).ToString();
            btnOpen.gameObject.SetActive(!isPurchase);
            btnPurchase.gameObject.SetActive(isPurchase);

            UIEventListener.Get(btnExit.gameObject).onClick = go =>
            {
                if (onExit != null)
                {
                    onExit();
                }
            };
        }

        private void PlayAnimation()
        {
            animator.enabled = true;
            animator.Play(m_AnimName);
            var capsuleAnimator =
                capsules.ToList().Find(c => c.gameObject.activeSelf).GetComponentInChildren<Animator>();
            capsuleAnimator.enabled = true;
            capsuleAnimator.Play("capsule_open");
        }

        private void OnDisable()
        {
            btnPurchase.gameObject.SetActive(false);
            if (PnlMainMenu.PnlMainMenu.Instance != null)
            {
//                PnlMainMenu.PnlMainMenu.Instance.OnCharmUpdate(false, null, false);
            }
        }

        public override void BeCatched()
        {
            base.BeCatched();
            instance = this;
            btnOpen.onClick.Add(new EventDelegate(() =>
            {
                CapsuleManager.instance.OpenCapsule((result) =>
                {
                    if (result)
                    {
                        PlayAnimation();
                    }
                });
            }));

            btnPurchase.onClick.Add(new EventDelegate(() =>
            {
                var crystalRequired = CapsuleManager.instance.curCapsule.charmRequire * StoreManageComponent.Instance.charmRate;
                var curCrystal = AccountCrystalManagerComponent.Instance.GetCrystal();
                var text = "确认用" + crystalRequired.ToString() + "钻石购买吗？";
                Callback callback = () =>
                {
                    CapsuleManager.instance.OpenCapsule((result) =>
                    {
                        if (result)
                        {
                            PlayAnimation();
                            AccountCrystalManagerComponent.Instance.ChangeCrystal(-crystalRequired, true);
                        }
                    }, false);
                };
                if (crystalRequired > curCrystal)
                {
                    text = "钻石不足哦，是否前往商店购买呢？";
                    callback = () =>
                    {
                    };
                }

                CommonPanel.GetInstance().ShowYesNo(text, callback);
            }));
        }
    }
}