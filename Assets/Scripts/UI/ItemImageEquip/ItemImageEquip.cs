using FormulaBase;

/// UI分析工具自动生成代码
/// ItemImageEquipUI主模块
///
using System;
using UnityEngine;

namespace ItemImageEquip
{
    public class ItemImageEquip : UIPhaseBase
    {
        private static ItemImageEquip instance = null;
        public UISprite sprSelected, sprOn, sprLock, sprBkg;
        public UILabel txtCount;

        public static ItemImageEquip Instance
        {
            get
            {
                return instance;
            }
        }

        public UILabel txtLvl, txtLv;
        public UITexture texIcon;
        public UILabel txtType;

        public string infoPanelName;

        public FormulaBase.FormulaHost host
        {
            private set;
            get;
        }

        public bool isSelected
        {
            get { return sprSelected.gameObject.activeSelf; }
            set
            {
                sprSelected.gameObject.SetActive(value);
            }
        }

        public bool isUpgradeSelected
        {
            get { return sprOn.gameObject.activeSelf; }
            set
            {
                if (value)
                {
                    if (PnlSuitcase.PnlSuitcase.Instance.upgradeSelectedHost.Count < 3)
                    {
                        sprOn.gameObject.SetActive(true);
                        PnlSuitcase.PnlSuitcase.Instance.upgradeSelectedHost.Add(host);
                    }
                }
                else
                {
                    sprOn.gameObject.SetActive(false);
                    PnlSuitcase.PnlSuitcase.Instance.upgradeSelectedHost.Remove(host);
                }
                PnlSuitcase.PnlSuitcase.Instance.SetLock(PnlSuitcase.PnlSuitcase.Instance.upgradeSelectedHost.Count >= 3);
            }
        }

        public bool isLock
        {
            get { return sprLock.gameObject.activeSelf; }
            set
            {
                sprLock.gameObject.SetActive(value);
            }
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

        public void OnShow(string type)
        {
            txtType.text = type;
            sprBkg.spriteName = "groove_space";
            txtLvl.gameObject.SetActive(false);
            txtLv.gameObject.SetActive(false);
            texIcon.gameObject.SetActive(false);
        }

        public override void OnShow(FormulaBase.FormulaHost h)
        {
            host = h;
            SetTexByHost();
            SetTxtByHost();
            UIEventListener.Get(gameObject).onClick = (go) =>
            {
                if (isSelected || isLock)
                {
                    return;
                }
                var ac = GetComponent<AudioSource>();
                if (ac)
                {
                    ac.Play();
                }
                if (!PnlSuitcase.PnlSuitcase.Instance.isUpgrade)
                {
                    if (ItemManageComponent.Instance.IsEquipment(host))
                    {
                        PnlEquipInfo.PnlEquipInfo.Instance.OnShow(host);
                        PnlFoodInfo.PnlFoodInfo.Instance.OnExit();
                        PnlServantInfo.PnlServantInfo.Instance.OnExit();
                    }
                    else if (ItemManageComponent.Instance.IsFood(host))
                    {
                        PnlFoodInfo.PnlFoodInfo.Instance.OnShow(host);
                        PnlEquipInfo.PnlEquipInfo.Instance.OnExit();
                        PnlServantInfo.PnlServantInfo.Instance.OnExit();
                    }
                    else
                    {
                        PnlServantInfo.PnlServantInfo.Instance.OnShow();
                        PnlEquipInfo.PnlEquipInfo.Instance.OnExit();
                        PnlFoodInfo.PnlFoodInfo.Instance.OnExit();
                    }
                    if (!PnlChar.PnlChar.Instance.gameObject.activeSelf)
                    {
                        PnlSuitcase.PnlSuitcase.Instance.SetSelectedCell(h);
                        isSelected = true;
                    }
                }
                else
                {
                    OnUpgradeSelected();
                    PnlEquipInfo.PnlEquipInfo.Instance.OnUpgradeItemsRefresh();
                    PnlCharInfo.PnlCharInfo.Instance.OnUpgradeItemsRefresh();
                }
            };
        }

        public void OnSelected(bool selected)
        {
            isSelected = selected;
        }

        public void OnUpgradeSelected()
        {
            if (PnlSuitcase.PnlSuitcase.Instance.isUpgrade)
            {
                isUpgradeSelected = !isUpgradeSelected;
            }
        }

        private void SetTxtByHost()
        {
            if (host == null)
            {
                txtLvl.gameObject.SetActive(false);
                txtLv.gameObject.SetActive(false);
                texIcon.gameObject.SetActive(false);
                if (txtCount != null)
                {
                    txtCount.gameObject.SetActive(false);
                }

                return;
            }
            txtLvl.gameObject.SetActive(true);
            txtLv.gameObject.SetActive(true);
            texIcon.gameObject.SetActive(true);
            if (txtCount != null)
            {
                txtCount.gameObject.SetActive(true);
            }
            var lvl = host.GetDynamicIntByKey(FormulaBase.SignKeys.LEVEL);
            var type = host.GetDynamicStrByKey(FormulaBase.SignKeys.TYPE);
            txtLvl.text = lvl.ToString();
            txtType.text = type;
            if (txtCount != null)
            {
                txtCount.text = host.GetDynamicIntByKey(SignKeys.STACKITEMNUMBER).ToString();
            }
        }

        private void SetTexByHost()
        {
            if (host == null)
            {
                texIcon.mainTexture = null;
                sprBkg.spriteName = "groove_space";
                return;
            }
            string texName = host.GetDynamicStrByKey(FormulaBase.SignKeys.ICON);
            if (texName == null || ResourceLoader.Instance == null)
            {
                return;
            }
            ResourceLoader.Instance.LoadItemIcon(host, texIcon);

            var quality = host.GetDynamicIntByKey(SignKeys.QUALITY);
            sprBkg.spriteName = "groove_" + quality.ToString();
        }
    }
}