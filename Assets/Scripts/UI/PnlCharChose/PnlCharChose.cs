/// UI分析工具自动生成代码
/// PnlCharChoseUI主模块
///
using System;
using UnityEngine;

namespace PnlCharChose
{
    public class PnlCharChose : UIPhaseBase
    {
        private static PnlCharChose instance = null;
        public UIButton btnLeft, btnRight;
        public Transform spiParent;

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
        }

        private void InitEvent()
        {
            var count = FormulaBase.RoleManageComponent.Instance.HostList.Count;

            btnLeft.onClick.Add(new EventDelegate(() =>
            {
                choseType = --choseType < 1 ? 1 : choseType;
                FormulaBase.RoleManageComponent.Instance.GetRole(choseType).SetAsUINotifyInstance();
            }));
            btnRight.onClick.Add(new EventDelegate(() =>
            {
                choseType = ++choseType > count ? count : choseType;
                FormulaBase.RoleManageComponent.Instance.GetRole(choseType).SetAsUINotifyInstance();
            }));
        }

        private GameObject LoadSpiAnim(int idx)
        {
            /*FormulaBase.RoleManageComponent.Instance.
            GameObject obj = ChoseHeroManager.Get().LoadPlayerActor();
            if (obj != null)
            {
                GameObject spiAnim = Instantiate<GameObject>(obj);
                spiAnim.transform.parent = spiParent;
                spiAnim.transform.localScale = Vector3.one;
                spiAnim.transform.localPosition = Vector3.zero;
                spiAnim.SetActive(true);
            }
            else
            {
                Debug.LogError("加载未获得对象");
            }*/
            return null;
        }

        public override void OnShow() {
			this.InitData();
			this.InitEvent();
        }

        public override void OnHide()
        {
        }
    }
}