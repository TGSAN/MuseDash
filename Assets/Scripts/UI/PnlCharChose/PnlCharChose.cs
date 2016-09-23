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
        public Transform spiParent;
        private readonly List<string> m_ActionPaths = new List<string>();

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
            LoadSpiAnim(choseType);
            btnLeft.onClick.Add(new EventDelegate(() =>
            {
                choseType = --choseType < 1 ? 1 : choseType;
                FormulaBase.RoleManageComponent.Instance.GetRole(choseType).SetAsUINotifyInstance();
                LoadSpiAnim(choseType);
            }));
            btnRight.onClick.Add(new EventDelegate(() =>
            {
                choseType = ++choseType > count ? count : choseType;
                FormulaBase.RoleManageComponent.Instance.GetRole(choseType).SetAsUINotifyInstance();
                LoadSpiAnim(choseType);
            }));
        }

        private GameObject LoadSpiAnim(int idx)
        {
            var path = m_ActionPaths[idx - 1];
            var template = Resources.Load(path) as GameObject;
            if (template)
            {
                var go = GameObject.Instantiate(Resources.Load(path)) as GameObject;
                go.transform.SetParent(spiParent, false);
                go.SetActive(true);
                //go.transform.localScale = Vector3.one;
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

        public override void OnShow()
        {
            this.InitData();
            this.InitEvent();
        }

        public override void OnHide()
        {
        }
    }
}