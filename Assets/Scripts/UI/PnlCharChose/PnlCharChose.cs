/// UI分析工具自动生成代码
/// PnlCharChoseUI主模块
///
using System;
using System.Collections.Generic;
using UnityEngine;

namespace PnlCharChose
{
    public class PnlCharChose : UIPhaseBase
    {
        private static PnlCharChose instance = null;
        public UIButton btnLeft, btnRight;
        public GameObject pointPrefab;
        public Transform spiParent, pointParent;
        public UIButton btnApply, btnPurchase;
        public UILabel txtApplying;
        private readonly List<string> m_ActionPaths = new List<string>();
        private List<UIToggle> m_Points = new List<UIToggle>();

        public int choseType
        {
            get;
            private set;
        }

        public static PnlCharChose Instance
        {
            get
            {
                return instance;
            }
        }

        private void Start()
        {
            instance = this;
            this.InitData();
            this.InitUI();
            this.InitEvent();
        }

        #region Init

        /// <summary>
        /// 初始化数据
        /// </summary>
        private void InitData()
        {
            choseType = FormulaBase.RoleManageComponent.Instance.GetFightGirlIndex();
            FormulaBase.RoleManageComponent.Instance.GetRole(choseType).SetAsUINotifyInstance();
            for (int i = 1; i <= FormulaBase.RoleManageComponent.Instance.HostList.Count; i++)
            {
                var path = ConfigPool.Instance.GetConfigStringValue("character", i.ToString(), "char_show");
                m_ActionPaths.Add(path);
            }
        }

        /// <summary>
        /// 初始化点击事件
        /// </summary>
        private void InitEvent()
        {
            var count = FormulaBase.RoleManageComponent.Instance.HostList.Count;
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
                FormulaBase.RoleManageComponent.Instance.SetFightGirlIndex(choseType);
            }));
            btnPurchase.onClick.Add(new EventDelegate(() =>
            {
                FormulaBase.RoleManageComponent.Instance.UnlockRole(choseType);
            }));
            OnCharacterChange(choseType);
        }

        /// <summary>
        /// 初始化UI
        /// </summary>
        private void InitUI()
        {
            OnPointLoaded(choseType);
            OnCharacterChange(choseType);
        }

        #endregion Init

        #region OnEvent

        /// <summary>
        /// 同步读取Spi动画
        /// </summary>
        /// <param name="idx"></param>
        /// <returns></returns>
        private GameObject OnSpiAnimLoaded(int idx)
        {
			return null;
			/*
            if (spiParent.childCount > 0)
            {
                spiParent.DestroyChildren();
            }
            var path = m_ActionPaths[idx - 1];
            var template = Resources.Load(path) as GameObject;
            if (template)
            {
                var go = GameObject.Instantiate(Resources.Load(path)) as GameObject;
                go.transform.SetParent(spiParent, false);
                go.SetActive(true);
                go.transform.localPosition = Vector3.zero;
                go.transform.localEulerAngles = Vector3.zero;
                return go;
            }
            else
            {
                Debug.LogError("加载未获得对象");
                return null;
            }
            */
        }

        /// <summary>
        /// 读取下方点的预制
        /// </summary>
        /// <param name="idx"></param>
        private void OnPointLoaded(int idx)
        {
			if (pointParent == null) {
				return;
			}

            if (pointParent.childCount > 0)
            {
                pointParent.DestroyChildren();
                m_Points.Clear();
            }
            pointParent.GetComponent<UIGrid>().enabled = true;
            for (int i = 0; i < FormulaBase.RoleManageComponent.Instance.HostList.Count; i++)
            {
                var p = GameObject.Instantiate(pointPrefab) as GameObject;
                p.transform.SetParent(pointParent, false);
                var tgl = p.GetComponent<UIToggle>();
                m_Points.Add(tgl);
            }
        }

        /// <summary>
        /// 更换人物的事件
        /// </summary>
        /// <param name="curType"></param>
        private void OnCharacterChange(int curType)
        {
            FormulaBase.FormulaHost role = FormulaBase.RoleManageComponent.Instance.GetRole(curType);
            role.SetAsUINotifyInstance();
            Debug.Log("Selected role " + curType + " : " + role.GetDynamicStrByKey(FormulaBase.SignKeys.NAME));
            OnSpiAnimLoaded(curType);
            var isLocked = FormulaBase.RoleManageComponent.Instance.GetRoleLockedState(curType);
            var isCurCharacter = FormulaBase.RoleManageComponent.Instance.GetFightGirlIndex() == curType;
            btnApply.gameObject.SetActive(!isLocked && !isCurCharacter);
            btnPurchase.gameObject.SetActive(isLocked && !isCurCharacter);
            txtApplying.gameObject.SetActive(isCurCharacter);
            m_Points[curType - 1].value = true;
        }

        public override void OnShow()
        {
            choseType = FormulaBase.RoleManageComponent.Instance.GetFightGirlIndex();
            OnCharacterChange(choseType);
        }

        public override void OnHide()
        {
        }

        #endregion OnEvent
    }
}