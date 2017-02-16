using FormulaBase;

/// UI分析工具自动生成代码
/// PnlMuseUI主模块
///
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace PnlMuse
{
    public class PnlMuse : UIPhaseBase
    {
        private static PnlMuse instance = null;
        public UIButton btnLeft, btnRight;
//        public GameObject pointPrefab;
        public Transform spiParent;
        public UIButton btnApply, btnPurchase;
        public UILabel txtApplying;
//        public Transform btnsParent;
        public int clothIdx = 0;
        private readonly List<string> m_ActionPaths = new List<string>();
        private readonly List<UIToggle> m_Points = new List<UIToggle>();
        private readonly Dictionary<int, GameObject> m_SpiAniGODic = new Dictionary<int, GameObject>();

        public int choseType
        {
            private set; get;
        }

        public static PnlMuse Instance
        {
            get
            {
                return instance;
            }
        }

        private void Start()
        {
            instance = this;
            //this.InitData();
            //this.InitUI();
            //this.InitEvent();
        }

        #region Init

        /// <summary>
        /// 初始化数据
        /// </summary>
        private void InitData()
        {
            choseType = FormulaBase.RoleManageComponent.Instance.GetFightGirlIndex();
            FormulaBase.FormulaHost role = FormulaBase.RoleManageComponent.Instance.GetRole(choseType);
            if (role == null)
            {
                return;
            }

            role.SetAsUINotifyInstance();
            for (int i = 1; i <= FormulaBase.RoleManageComponent.Instance.GetRoleCount(); i++)
            {
                var path = ConfigPool.Instance.GetConfigStringValue("char_info", i.ToString(), "char_show");
                m_ActionPaths.Add(path);
            }
        }

        /// <summary>
        /// 初始化点击事件
        /// </summary>
        private void InitEvent()
        {
            var count = FormulaBase.RoleManageComponent.Instance.HostList.Count;
//            UIEventListener.Get(btnBack.gameObject).onClick = go =>
//            {
//                PnlMainMenu.PnlMainMenu.Instance.OnShow();
//                OnHide();
//                if (!PnlMuse.PnlMuse.Instance.gameObject.activeSelf)
//                {
//                    PnlHome.PnlHome.Instance.OnShow();
//                }
//                else
//                {
//                    PnlChar.PnlChar.Instance.onRoleChange(PnlChar.PnlChar.Instance.curRoleIdx);
//                }
//            };
            btnLeft.onClick.Add(new EventDelegate(() =>
            {
                --choseType;
                if (choseType < 1)
                {
                    choseType = 1;
                }
                else
                {
                    OnCharacterChange(choseType);
                }
            }));
            btnRight.onClick.Add(new EventDelegate(() =>
            {
                ++choseType;
                if (choseType > count)
                {
                    choseType = count;
                }
                else
                {
                    OnCharacterChange(choseType);
                }
            }));
            btnApply.onClick.Add(new EventDelegate(() =>
            {
                CommonPanel.GetInstance().ShowWaittingPanel();
                FormulaBase.RoleManageComponent.Instance.SetFightGirlIndex(choseType, this.SetFightGirlCallback);
            }));

            btnPurchase.onClick.Add(new EventDelegate(() =>
            {
                CommonPanel.GetInstance().ShowWaittingPanel();
                FormulaBase.RoleManageComponent.Instance.PurchaseRole(choseType, this.UnLockRoleCallback);
            }));

//            var btns = btnsParent.GetComponentsInChildren<UIButton>();
//            foreach (var uiButton in btns)
//            {
//                var btn = uiButton;
//                uiButton.onClick.Add(new EventDelegate(() =>
//                {
//                    var suits = FormulaBase.RoleManageComponent.Instance.GetCloths(choseType);
//                    var idx = btn.transform.GetSiblingIndex();
//                    clothIdx = idx;
//                    if (clothIdx < suits.Count)
//                    {
//                        var suit = suits[clothIdx];
//                        PnlCharCosChose.PnlCharCosChose.Instance.OnShow(suit);
//                    }
//                }));
//            }
        }

        /// <summary>
        /// 初始化UI
        /// </summary>
        private void InitUI()
        {
//            OnPointLoaded(choseType);
            OnCharacterChange(choseType);
        }

        #endregion Init

        #region OnEvent

        /// <summary>
        /// 同步读取Spi动画
        /// </summary>
        /// <param name="idx"></param>
        /// <returns></returns>
        private void OnSpiAnimLoaded(int idx)
        {
            if (!m_SpiAniGODic.ContainsKey(idx))
            {
                var path = m_ActionPaths[idx - 1];
                GameObject go = null;
                ResourceLoader.Instance.Load(path, res => go = Instantiate(res) as GameObject);
                go.transform.SetParent(spiParent, false);
                go.SetActive(true);
                go.transform.localPosition = Vector3.zero;
                go.transform.localEulerAngles = Vector3.zero;
                m_SpiAniGODic.Add(idx, go);
            }
            foreach (var pair in m_SpiAniGODic)
            {
                pair.Value.SetActive(pair.Key == idx);
            }
        }

        /// <summary>
        /// 读取下方点的预制
        /// </summary>
        /// <param name="idx"></param>
//        private void OnPointLoaded(int idx)
//        {
//            if (pointParent == null)
//            {
//                return;
//            }
//
//            if (pointParent.childCount > 0)
//            {
//                pointParent.DestroyChildren();
//                m_Points.Clear();
//            }
//            pointParent.GetComponent<UIGrid>().enabled = true;
//            for (int i = 0; i < FormulaBase.RoleManageComponent.Instance.GetRoleCount(); i++)
//            {
//                var p = GameObject.Instantiate(pointPrefab) as GameObject;
//                p.transform.SetParent(pointParent, false);
//                var tgl = p.GetComponent<UIToggle>();
//                m_Points.Add(tgl);
//            }
//        }

        /// <summary>
        /// 更换人物的事件
        /// </summary>
        /// <param name="curType"></param>
        private void OnCharacterChange(int curType)
        {
            FormulaBase.FormulaHost role = FormulaBase.RoleManageComponent.Instance.GetRole(curType);
            if (role == null)
            {
                return;
            }

            role.SetAsUINotifyInstance();
            Debug.Log("Selected role " + curType + " : " + role.GetDynamicStrByKey(FormulaBase.SignKeys.NAME));
            OnSpiAnimLoaded(curType);
            var isLocked = FormulaBase.RoleManageComponent.Instance.GetRoleLockedState(curType);
            var isCurCharacter = FormulaBase.RoleManageComponent.Instance.GetFightGirlIndex() == curType;
            btnApply.gameObject.SetActive(!isLocked && !isCurCharacter);
            btnPurchase.gameObject.SetActive(isLocked && !isCurCharacter);
            txtApplying.gameObject.SetActive(isCurCharacter);
            m_Points.ToList().ForEach(p => p.value = false);
            m_Points[curType - 1].value = true;
        }

        public override void OnShow(int choseIdx = 0)
        {
            gameObject.SetActive(true);
            choseType = choseIdx > 0 ? choseIdx : FormulaBase.RoleManageComponent.Instance.GetFightGirlIndex();
            OnCharacterChange(choseType);
        }

        public override void OnHide()
        {
            gameObject.SetActive(false);
        }

        #endregion OnEvent

        private void SetFightGirlCallback()
        {
            CommonPanel.GetInstance().ShowWaittingPanel(false);
            btnApply.gameObject.SetActive(false);
            btnPurchase.gameObject.SetActive(false);
            txtApplying.gameObject.SetActive(true);
        }

        private void UnLockRoleCallback()
        {
            btnApply.gameObject.SetActive(true);
            btnPurchase.gameObject.SetActive(false);
            txtApplying.gameObject.SetActive(false);
        }
    }
}