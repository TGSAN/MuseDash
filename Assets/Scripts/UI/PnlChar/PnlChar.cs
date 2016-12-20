using FormulaBase;
using PnlMainMenu;

/// UI分析工具自动生成代码
/// PnlCharUI主模块
///
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using FormulaBase = FormulaBase.FormulaBase;

namespace PnlChar
{
    public class PnlChar : UIPhaseBase
    {
        private static PnlChar instance = null;

        public Transform spiAnimParent;
        public UIButton btnLeft, btnRight, btnUnlock;
        public ItemImageEquip.ItemImageEquip[] items;
        public GameObject goGrdGroove, goGrdServent;
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

        public bool isChangeRole
        {
            get { return m_PreRoleIdx != curRoleIdx; }
        }

        private int m_PreRoleIdx = 0;
        private List<FormulaHost> m_Equipments = new List<FormulaHost>();
        private readonly List<string> m_AnimPath = new List<string>();

        public List<string> animPath
        {
            get { return m_AnimPath; }
        }

        public Action<int> onRoleChange = null;

        public static PnlChar Instance
        {
            get
            {
                return instance;
            }
        }

        public void OnShowCos(CharCos cos)
        {
            curRoleIdx = cos.ownerIdx;
            OnClickBtnEnhancement.Do(PnlMainMenu.PnlMainMenu.Instance.goBtnEnhancement);
            DOTweenUtils.Delay(() =>
            {
                animPath[cos.ownerIdx - 1] = cos.path;
                m_PreRoleIdx = 0;
                PnlCharInfo.PnlCharInfo.Instance.OnRoleChange(cos.ownerIdx);
            }, 0.2f);
        }

        public override void OnShow()
        {
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
            curRoleIdx = RoleManageComponent.Instance.GetFightGirlIndex();
            InitInfo();
            InitEvent();
        }

        #region Update更新

        private void UpdateUI()
        {
            InitUI();
        }

        #endregion Update更新

        #region Init初始化

        private void InitInfo()
        {
            m_AnimPath.Clear();
            for (int i = 1; i <= RoleManageComponent.Instance.GetRoleCount(); i++)
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
            for (int i = 1; i <= RoleManageComponent.Instance.GetRoleCount(); i++)
            {
                OnSpiAnimLoad(i);
            }
        }

        private void InitEvent()
        {
            onRoleChange = new Action<int>(idx =>
            {
                if (isChangeRole)
                {
                    PnlCharInfo.PnlCharInfo.Instance.OnExit();
                }
            });
            onRoleChange += PnlCharInfo.PnlCharInfo.Instance.OnRoleChange;
            onRoleChange += OnRoleChange;
            var maxCount = RoleManageComponent.Instance.GetRoleCount();
            UIEventListener.Get(btnLeft.gameObject).onClick += go =>
               {
                   if (--curRoleIdx < 1)
                   {
                       curRoleIdx = maxCount;
                   }
                   onRoleChange(curRoleIdx);
               };

            UIEventListener.Get(btnRight.gameObject).onClick += go =>
            {
                if (++curRoleIdx > maxCount)
                {
                    curRoleIdx = 1;
                }
                onRoleChange(curRoleIdx);
            };

            UIEventListener.Get(btnUnlock.gameObject).onClick += go =>
            {
                PnlMainMenu.PnlMainMenu.Instance.gameObject.SetActive(false);
                PnlCharChose.PnlCharChose.Instance.OnShow(curRoleIdx);
            };

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
            foreach (var item in items)
            {
                item.gameObject.SetActive(true);
            }
            if (onRoleChange != null)
            {
                onRoleChange(curRoleIdx);
            }
        }

        #endregion Init初始化

        #region On事件

        private void OnRoleChange(int roleIdx)
        {
            var isLock = RoleManageComponent.Instance.GetRoleLockedState(curRoleIdx);
            goGrdGroove.SetActive(!isLock);
            goGrdServent.SetActive(!isLock);
            btnUnlock.gameObject.SetActive(isLock);
            if (m_PreRoleIdx == roleIdx)
            {
                return;
            }
            m_PreRoleIdx = roleIdx;
            curEquipTypeIdx = 0;
            RoleManageComponent.Instance.GetRole(roleIdx).SetAsUINotifyInstance();
            OnEquipLoad(roleIdx);
            OnSpiAnimLoad(roleIdx);
        }

        public void OnEquipLoad(int idx)
        {
            Debug.Log("Get Girl Equip");
            var curEquipHosts = EquipManageComponent.Instance.GetGirlEquipHosts(idx, 0, true);
            Debug.Log("Get Girl Equip Finished");
            for (int i = 0; i < items.Length; i++)
            {
                var types = EquipManageComponent.Instance.GetGirlEquipTypes(idx);
                FormulaHost host = null;
                if (i < types.Length)
                {
                    host = curEquipHosts.ToList().Find(h => h.GetDynamicStrByKey(SignKeys.TYPE) == types[i]);
                }
                var item = items[i];
                if (host == null)
                {
                    if (i < types.Length)
                    {
                        item.OnShow(types[i]);
                    }
                }
                else
                {
                    item.OnShow(host);
                }

                if (i != 3) continue;
                var servantHost = PetManageComponent.Instance.GetEquipedPet(idx);
                if (servantHost != null)
                {
                    item.OnShow(servantHost);
                }
                else
                {
                    item.OnShow("Servant");
                }
            }
        }

        public void OnSpiAnimLoad(int idx, string p = null)
        {
            var path = string.Empty;
            if (p != null)
            {
                path = p;
            }
            else
            {
                if (m_AnimPath.Count > idx - 1)
                {
                    path = m_AnimPath[idx - 1];
                }
            }
            if (string.IsNullOrEmpty(path))
            {
                return;
            }
            if (m_SpiAniGODic.ContainsKey(idx))
            {
                var goName = StringUtils.LastAfter(path, '/') + "(Clone)";
                var curGoName = m_SpiAniGODic[idx].name;
                if (goName == curGoName)
                {
                    spiAnimParent.transform.GetComponentsInChildren<SkeletonAnimation>().ToList().ForEach(g => g.gameObject.SetActive(false));
                    m_SpiAniGODic[idx].SetActive(true);
                    return;
                }
            }

            GameObject go = null;
            ResourceLoader.Instance.Load(path, res =>
            {
                if (res == null) return;
                go = Instantiate(res) as GameObject;
                if (go != null)
                {
                    go.transform.SetParent(spiAnimParent, false);
                    go.transform.localPosition = Vector3.zero;
                    go.transform.localScale = Vector3.one * 140f;
                    go.transform.localEulerAngles = Vector3.zero;
                    var skeletonAnim = go.GetComponent<SkeletonAnimation>();
                    skeletonAnim.loop = true;
                    skeletonAnim.AnimationName = "run";
                    DOTweenUtils.Delay(() =>
                    {
                        skeletonAnim.AnimationName = "standby";
                    }, Time.deltaTime);
                    go.GetComponent<SpineSynchroObjects>().enabled = false;
                    go.GetComponent<SpineMountController>().enabled = false;
                    go.GetComponent<Renderer>().sortingOrder = 50;
                    if (m_SpiAniGODic.ContainsKey(idx))
                    {
                        Destroy(m_SpiAniGODic[idx]);
                        m_SpiAniGODic[idx] = go;
                    }
                    else
                    {
                        m_SpiAniGODic.Add(idx, go);
                    }
                }
                m_AnimPath[idx - 1] = path;
                foreach (var pair in m_SpiAniGODic)
                {
                    pair.Value.SetActive(pair.Key == idx);
                }
            });
        }

        #endregion On事件
    }
}