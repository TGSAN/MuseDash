using FormulaBase;

/// UI分析工具自动生成代码
/// PnlCharUI主模块
///
using System;
using System.Collections.Generic;
using UnityEngine;

namespace PnlChar
{
    public class PnlChar : UIPhaseBase
    {
        private static PnlChar instance = null;

        public Transform spiAnimParent;
        public UIButton btnLeft, btnRight;
        public ItemImageEquip.ItemImageEquip[] items;
        private readonly Dictionary<int, GameObject> m_SpiAniGODic = new Dictionary<int, GameObject>();

        public int curRoleIdx
        {
            private set;
            get;
        }

        public int curEquipTypeIdx
        {
            private set;
            get;
        }

        private int m_PreRoleIdx = 0;
        private List<FormulaBase.FormulaHost> m_Equipments = new List<FormulaBase.FormulaHost>();
        private readonly List<string> m_AnimPath = new List<string>();
        public Action<int> onRoleChange;

        public static PnlChar Instance
        {
            get
            {
                return instance;
            }
        }

        private void Start()
        {
            Init();
        }

        public override void OnShow()
        {
            UpdateInfo();
            UpdateUI();
        }

        public override void OnHide()
        {
            PnlEquipInfo.PnlEquipInfo.Instance.OnHide();
            PnlFoodInfo.PnlFoodInfo.Instance.OnHide();
            PnlServantInfo.PnlServantInfo.Instance.OnHide();
        }

        public override void BeCatched()
        {
            instance = this;
        }

        #region Update更新

        private void UpdateInfo()
        {
            m_AnimPath.Clear();
            InitInfo();
        }

        private void UpdateUI()
        {
            InitUI();
        }

        #endregion Update更新

        #region Init初始化

        private void Init()
        {
            curRoleIdx = FormulaBase.RoleManageComponent.Instance.GetFightGirlIndex();
            onRoleChange += idx => PnlCharInfo.PnlCharInfo.Instance.OnUpgradeItemsRefresh();
            onRoleChange += PnlCharInfo.PnlCharInfo.Instance.OnRoleChange;
            onRoleChange += idx => PnlEquipInfo.PnlEquipInfo.Instance.OnExit();
            InitInfo();
            InitEvent();
            InitUI();
        }

        private void InitInfo()
        {
            for (int i = 1; i <= FormulaBase.RoleManageComponent.Instance.GetRoleCount(); i++)
            {
                var pathIdx = ConfigPool.Instance.GetConfigStringValue("char_info", i.ToString(), "character");
                var clothingConfig = ConfigPool.Instance.GetConfigByName("char_cos");
                for (int j = 0; j < clothingConfig.Count; j++)
                {
                    var jdata = clothingConfig[j];
                    if (jdata["uid"].ToString() == pathIdx)
                    {
                        var path = jdata["path"].ToString();
                        m_AnimPath.Add(path);
                        break;
                    }
                }
            }
        }

        private void InitEvent()
        {
            onRoleChange += OnRoleChange;
            var maxCount = FormulaBase.RoleManageComponent.Instance.GetRoleCount();
            Action onLeftClick = null;
            onLeftClick = () =>
            {
                if (--curRoleIdx < 1)
                {
                    curRoleIdx = maxCount;
                }
                if (FormulaBase.RoleManageComponent.Instance.GetRoleLockedState(curRoleIdx))
                {
                    onLeftClick();
                }
                onRoleChange(curRoleIdx);
            };

            btnLeft.onClick.Add(new EventDelegate(() =>
            {
                onLeftClick();
            }));

            Action onRightClick = null;
            onRightClick = () =>
            {
                if (++curRoleIdx > maxCount)
                {
                    curRoleIdx = 1;
                }
                if (FormulaBase.RoleManageComponent.Instance.GetRoleLockedState(curRoleIdx))
                {
                    onRightClick();
                }
                onRoleChange(curRoleIdx);
            };
            btnRight.onClick.Add(new EventDelegate(() =>
            {
                onRightClick();
            }));

            for (int i = 0; i < items.Length; i++)
            {
                var btn = items[i].transform.parent.gameObject.GetComponent<UIButton>();
                var idx = i + 1;
                btn.onClick.Add(new EventDelegate(() =>
                {
                    var animator = PnlCharInfo.PnlCharInfo.Instance.GetComponent<Animator>();
                    var clipName = "pnl_items_choose_in";
                    if (!animator.GetCurrentAnimatorStateInfo(0).IsName(clipName))
                    {
                        animator.Play(clipName);
                    }
                    if (curEquipTypeIdx != idx)
                    {
                        PnlCharInfo.PnlCharInfo.Instance.cellItemParent.DestroyChildren();
                        curEquipTypeIdx = idx;
                        PnlCharInfo.PnlCharInfo.Instance.OnShow();
                    }
                    PnlEquipInfo.PnlEquipInfo.Instance.OnHide();
                }));
            }
        }

        private void InitUI()
        {
            var unlockCount = FormulaBase.RoleManageComponent.Instance.GetUnlockRoleCount();
            btnLeft.gameObject.SetActive(unlockCount > 1);
            btnRight.gameObject.SetActive(unlockCount > 1);
            foreach (var item in items)
            {
                item.gameObject.SetActive(true);
            }
            DOTweenUtils.Delay(() =>
            {
                onRoleChange(curRoleIdx);
            }, Time.deltaTime);
        }

        #endregion Init初始化

        #region On事件

        private void OnRoleChange(int roleIdx)
        {
            if (m_PreRoleIdx == roleIdx)
            {
                return;
            }
            m_PreRoleIdx = roleIdx;
            curEquipTypeIdx = 0;
            FormulaBase.RoleManageComponent.Instance.GetRole(roleIdx).SetAsUINotifyInstance();

            OnSpiAnimLoad(roleIdx);
            OnEquipLoad(roleIdx);
        }

        public void OnEquipLoad(int idx)
        {
            var curEquipHosts = FormulaBase.EquipManageComponent.Instance.GetGirlEquipHosts(idx, 0, true);
            for (int i = 0; i < items.Length; i++)
            {
                FormulaBase.FormulaHost host = null;
                if (i < curEquipHosts.Length)
                {
                    host = curEquipHosts[i];
                }
                var item = items[i];
                if (host == null)
                {
                    var types = FormulaBase.EquipManageComponent.Instance.GetGirlEquipTypes(idx);
                    if (i < types.Length)
                    {
                        item.OnShow(types[i]);
                    }
                }
                else
                {
                    item.OnShow(host);
                }

                if (i == 3)
                {
                    var servantHost = PetManageComponent.Instance.GetEquipedPet(idx);
                    item.OnShow(servantHost);
                }
            }
        }

        public void OnSpiAnimLoad(int idx, string p = null)
        {
            var path = m_AnimPath[idx - 1];
            if (p == path)
            {
                return;
            }
            GameObject go = null;
            ResourceLoader.Instance.Load(p ?? path, res => go = Instantiate(res) as GameObject);
            go.transform.SetParent(spiAnimParent, false);
            go.SetActive(true);
            go.transform.localPosition = Vector3.zero;
            go.transform.localScale = Vector3.one * 140f;
            go.transform.localEulerAngles = Vector3.zero;
            var skeletonAnim = go.GetComponent<SkeletonAnimation>();
            skeletonAnim.loop = true;
            skeletonAnim.AnimationName = "standby";
            go.GetComponent<SpineSynchroObjects>().enabled = false;
            go.GetComponent<SpineMountController>().enabled = false;
            go.GetComponent<Renderer>().sortingOrder = 100;
            if (m_SpiAniGODic.ContainsKey(idx))
            {
                Destroy(m_SpiAniGODic[idx]);
                m_SpiAniGODic[idx] = go;
            }
            else
            {
                m_SpiAniGODic.Add(idx, go);
            }
            m_AnimPath[idx - 1] = p ?? path;
            foreach (var pair in m_SpiAniGODic)
            {
                pair.Value.SetActive(pair.Key == idx);
            }
        }

        #endregion On事件
    }
}