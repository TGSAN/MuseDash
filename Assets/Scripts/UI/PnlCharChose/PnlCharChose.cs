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
        }

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
                    FormulaBase.RoleManageComponent.Instance.GetRole(choseType).SetAsUINotifyInstance();
                    LoadSpiAnim(choseType);
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
                    FormulaBase.RoleManageComponent.Instance.GetRole(choseType).SetAsUINotifyInstance();
                    LoadSpiAnim(choseType);
                }
            }));
        }

        private void InitUI()
        {
            LoadPoint(choseType);
            LoadSpiAnim(choseType);
        }

        private GameObject LoadSpiAnim(int idx)
        {
            if (spiParent.childCount > 0)
            {
                spiParent.DestroyChildren();
            }
            m_Points[idx - 1].value = true;
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
        }

        private void LoadPoint(int idx)
        {
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

        public override void OnShow()
        {
            this.InitData();
            this.InitUI();
            this.InitEvent();
        }

        public override void OnHide()
        {
        }
    }
}