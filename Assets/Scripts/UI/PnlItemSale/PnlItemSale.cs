using FormulaBase;

/// UI分析工具自动生成代码
/// PnlItemSaleUI主模块
///
using System;
using UnityEngine;

namespace PnlItemSale
{
    public class PnlItemSale : UIPhaseBase
    {
        private static PnlItemSale instance = null;
        public UIButton btnCancell, btnSale;
        public UILabel txtName, txtSaleNum, txtSum, txtLvl, txtSalePrice;
        public UITexture texItem;
        public UISlider sldSale;
        private bool m_IsChange = false;
        private int m_ItemCount = 0;

        public static PnlItemSale Instance
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

        public override void OnShow(FormulaHost host)
        {
            m_IsChange = false;
            gameObject.SetActive(true);
            GetComponent<Animator>().enabled = true;
            var itemName = host.GetDynamicStrByKey(SignKeys.NAME);
            m_ItemCount = host.GetDynamicIntByKey(SignKeys.STACKITEMNUMBER);
            var lvl = host.GetDynamicIntByKey(SignKeys.LEVEL);
            var price = ItemManageComponent.Instance.GetItemMoney(host);
            UIEventListener.Get(btnSale.gameObject).onClick = go =>
            {
                GetComponent<Animator>().enabled = false;
                var saleNum = int.Parse(txtSaleNum.text);
                ItemManageComponent.Instance.SaleItem(host, (result) =>
                {
                    m_IsChange = true;
                    m_ItemCount -= saleNum;

                    if (m_ItemCount < 1)
                    {
                        GetComponent<Animator>().enabled = false;
                        btnCancell.gameObject.GetComponent<UIPlayAnimation>().Play(true);
                        PnlFoodInfo.PnlFoodInfo.Instance.OnExit();
                    }
                    else
                    {
                        txtSum.text = m_ItemCount.ToString();
                        sldSale.value = 1f / (float)m_ItemCount;
                    }

                    CommonPanel.GetInstance().ShowWaittingPanel(false);
                    OnDisable();
                }, int.Parse(txtSaleNum.text));
            };
            UIEventListener.Get(btnCancell.gameObject).onClick = go =>
            {
                GetComponent<Animator>().enabled = false;
            };
            sldSale.onChange.Add(new EventDelegate(() =>
            {
                txtSaleNum.text = Mathf.CeilToInt((float)m_ItemCount * sldSale.value).ToString();
                txtSalePrice.text = (int.Parse(txtSaleNum.text) * price).ToString();
                if (txtSalePrice.text == "0")
                {
                    sldSale.value = 1f / (float)m_ItemCount;
                }
            }));
            txtName.text = itemName;
            txtSum.text = m_ItemCount.ToString();
            txtLvl.text = lvl.ToString();
            ResourceLoader.Instance.LoadItemIcon(host, texItem);
            sldSale.value = 1f / (float)m_ItemCount;
        }

        public override void OnHide()
        {
        }

        private void OnDisable()
        {
            if (PnlSuitcase.PnlSuitcase.Instance)
            {
                if (PnlSuitcase.PnlSuitcase.Instance.gameObject.activeSelf && m_IsChange)
                {
                    PnlSuitcase.PnlSuitcase.Instance.OnShow();
                }
            }
        }
    }
}