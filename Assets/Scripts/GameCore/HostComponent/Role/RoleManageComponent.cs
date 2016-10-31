using GameLogic;

///自定义模块，可定制模块具体行为
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace FormulaBase
{
    public class RoleManageComponent : CustomComponentBase
    {
        private static RoleManageComponent instance = null;
        private const int HOST_IDX = 0;

        public static RoleManageComponent Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new RoleManageComponent();
                }
                return instance;
            }
        }

        // -----------------
        public static int RoleIndexToId(int idx)
        {
            return idx;
        }

        /// <summary>
        /// 获取指定的角色
        /// </summary>
        /// <returns>The role.</returns>
        /// <param name="idx">Index.</param>
        public FormulaHost GetRole(int idx = 0)
        {
            return this.GetHostByKeyValue(SignKeys.ID, idx);
        }

        public FormulaHost GetGirlByIdx(int idx)
        {
            return HostList.Values.FirstOrDefault(value => value.GetDynamicIntByKey(SignKeys.ID) == idx);
        }

        public int GetRoleCount()
        {
            return HostList.Count;
        }

        public int GetUnlockRoleCount()
        {
            var count = 0;
            for (int i = 1; i <= GetRoleCount(); i++)
            {
                if (!GetRoleLockedState(i))
                {
                    count++;
                }
            }
            return count;
        }

        public int GetChoseRoleIdx()
        {
            var idx = 1;
            foreach (var formulaHost in HostList.Values)
            {
                if (formulaHost.GetDynamicIntByKey(SignKeys.FIGHTHERO) == 1)
                {
                    idx = formulaHost.GetDynamicIntByKey(SignKeys.ID);
                    break;
                }
            }
            return idx;
        }

        public FormulaHost GetRole(string name)
        {
            return this.GetHostByKeyValue(SignKeys.NAME, name);
        }

        public void BuyHeroCallBack(cn.bmob.response.EndPointCallbackData<Hashtable> response)
        {
            CommonPanel.GetInstance().ShowWaittingPanel(false);
        }

        public void InitRole()
        {
            Debugger.Log("初始化角色数据");
            if (this.HostList == null)
            {
                this.GetList("Role");
            }

            List<FormulaHost> hostList = null;
            // 如果没有服务器数据,用配置生成本地对象
            if (this.HostList == null || this.HostList.Count <= 0)
            {
                hostList = new List<FormulaHost>();
                LitJson.JsonData roleCfg = ConfigPool.Instance.GetConfigByName("char_info");
                foreach (string key in roleCfg.Keys)
                {
                    var role = FomulaHostManager.Instance.CreateHost("Role");
                    role.SetDynamicData(SignKeys.ID, int.Parse(key));
                    role.Result(FormulaKeys.FORMULA_178);
                    FomulaHostManager.Instance.AddHost(role);
                    var formulaHosts = HostList;
                    if (formulaHosts != null) formulaHosts[key] = role;
                    hostList.Add(role);
                }
            }
            //初始化角色信息
            UpdateRoleInfo(hostList);
            // 初始化战斗角色
            this.SetFightGirlIndex(GetChoseRoleIdx(), () =>
            {
                this.SetFightGirlCallBack(null);
            });
            this.SetFightGirlClothByOrder(1);
        }

        /// <summary>
        /// 更新角色信息 hostList为空表示信息来源于服务器，不空则来源于本地Json
        /// </summary>
        /// <param name="hostList"></param>
        public void UpdateRoleInfo(List<FormulaHost> hostList = null)
        {
            if (hostList != null)
            {
                foreach (var formulaHost in hostList)
                {
                    var id = formulaHost.GetDynamicIntByKey(SignKeys.ID).ToString();
                    FomulaHostManager.Instance.AddHost(formulaHost);
                    HostList[id] = formulaHost;
                }
            }

            foreach (FormulaHost role in HostList.Values)
            {
                if (role == null)
                {
                    continue;
                }
                if (role.GetDynamicDataByKey(SignKeys.ID) == 1)
                {
                    role.SetDynamicData(SignKeys.LOCKED, 0);
                }
                // 基本配置属性
                role.Result(FormulaKeys.FORMULA_178);

                // 更新动态属性
                UpdateProperty(role);
            }
        }

        public void UpdateProperty(FormulaHost role)
        {
            // 最终血量
            var maxVigour = (int)role.Result(FormulaKeys.FORMULA_186);
            role.SetDynamicData(SignKeys.MAX_VIGOUR, maxVigour);
            //最终耐久
            var maxStamina = (int)role.Result(FormulaKeys.FORMULA_188);
            role.SetDynamicData(SignKeys.MAX_STAMINA, maxStamina);
            //最终攻击
            var maxAttack = (int)role.Result(FormulaKeys.FORMULA_187);
            role.SetDynamicData(SignKeys.MAX_STRENGH, maxAttack);
            //最终暴击率
            var maxLuck = (int)role.Result(FormulaKeys.FORMULA_254);
            role.SetDynamicData(SignKeys.MAX_LUCK, maxLuck);

            //装备类型
            var cloth = role.GetDynamicIntByKey(SignKeys.CLOTH);
            if (cloth <= 0)
            {
                cloth = ConfigPool.Instance.GetConfigIntValue("char_info", role.GetDynamicIntByKey(SignKeys.ID).ToString(), "char_info");
                role.SetDynamicData(SignKeys.CLOTH, cloth);
            }
        }

        public void Equip(FormulaHost equipHost, bool isTo, HttpResponseDelegate func = null)
        {
            var equipType = equipHost.GetDynamicStrByKey(SignKeys.TYPE);
            if (equipType == "necklace" || equipType == "toy" || equipType == "fiara")
            {
                //仅necklace、toy、fiara
                var value = isTo ? (int)equipHost.Result(FormulaKeys.FORMULA_258) : 0;
                Host.SetDynamicData(SignKeys.VIGOUR_FROM_EQUIP, value);
            }
            else if (equipType == "gloves" || equipType == "panty" || equipType == "ring")
            {
                //仅gloves、panty、ring类型装备增加耐力
                var value = isTo ? (int)equipHost.Result(FormulaKeys.FORMULA_261) : 0;
                Host.SetDynamicData(SignKeys.STAMINA_FROM_EQUIP, value);
            }
            else
            {
                //其他类型装备增加攻击
                var value = isTo ? (int)equipHost.Result(FormulaKeys.FORMULA_264) : 0;
                Host.SetDynamicData(SignKeys.STRENGH_FROM_EQUIP, value);
            }
            UpdateRoleInfo();
            Host.Save(func);
        }

        public void GetExpAndCost(ref int Exp, ref int Cost)
        {
            Exp = 0;
            Cost = 0;
            FormulaHost tempEquip = new FormulaHost(HOST_IDX);
            List<FormulaHost> tList = ItemManageComponent.Instance.GetChosedItem;
            for (int i = 0, max = tList.Count; i < max; i++)
            {
                FormulaHost _item = tList[i];
                if (_item == null)
                {
                    continue;
                }

                Exp += (int)_item.Result(FormulaKeys.FORMULA_40) * _item.GetDynamicIntByKey(SignKeys.CHOSED);
                Cost += (int)_item.Result(FormulaKeys.FORMULA_41) * _item.GetDynamicIntByKey(SignKeys.CHOSED);
            }
        }

        /// <summary>
        /// 获取升级后的host
        /// </summary>
        public FormulaHost GetLevelUpHost(FormulaHost _host)
        {
            int exp = 0;
            int cost = 0;
            GetExpAndCost(ref exp, ref cost);
            FormulaHost thost = new FormulaHost(HOST_IDX);
            thost.SetDynamicData(SignKeys.ID, _host.GetDynamicIntByKey(SignKeys.ID));
            thost.SetDynamicData(SignKeys.LEVEL_STAR, _host.GetDynamicIntByKey(SignKeys.LEVEL_STAR));
            thost.SetDynamicData(SignKeys.LEVEL, _host.GetDynamicIntByKey(SignKeys.LEVEL));
            exp += _host.GetDynamicIntByKey(SignKeys.EXP);

            int levelUpExp = (int)_host.Result(FormulaKeys.FORMULA_15);
            int level = _host.GetDynamicIntByKey(SignKeys.LEVEL);
            while (levelUpExp <= exp)
            {
                level++;
                exp -= levelUpExp;
                thost.SetDynamicData(SignKeys.LEVEL, level);
                levelUpExp = (int)thost.Result(FormulaKeys.FORMULA_15);
                if (level == (int)thost.Result(FormulaKeys.FORMULA_14))
                {
                    NGUIDebug.Log("到达等级上限");
                    return thost;
                }
            }

            //thost.SetDynamicData(SignKeys.LEVEL,Level);
            thost.SetDynamicData(SignKeys.EXP, exp);
            return thost;
        }

        public bool IsItemLvlMax(FormulaHost host)
        {
            var lvlMax = (int)host.Result(FormulaKeys.FORMULA_14);
            var curLvl = host.GetDynamicIntByKey(SignKeys.LEVEL);
            return curLvl >= lvlMax;
        }

        public FormulaHost LevelUp(FormulaHost host, List<FormulaHost> expHosts, HttpResponseDelegate callFunc = null, bool isSave = true)
        {
            var exp = 0;
            expHosts.ForEach(h =>
            {
                exp += (int)h.Result(FormulaKeys.FORMULA_265);
            });
            var originLvl = host.GetDynamicIntByKey(SignKeys.LEVEL);
            var originExp = host.GetDynamicIntByKey(SignKeys.EXP);
            var lvl = host.GetDynamicIntByKey(SignKeys.LEVEL);
            Action lvlUp = null;
            var firstTimeExp = originExp;
            lvlUp = () =>
            {
                var expRequired = ConfigPool.Instance.GetConfigIntValue("experience", lvl.ToString(), "char_exp") - firstTimeExp;
                exp -= expRequired;
                if (exp >= 0)
                {
                    lvl++;
                    firstTimeExp = 0;
                    if (!IsItemLvlMax(host))
                    {
                        lvlUp();
                    }
                }
                else
                {
                    exp += expRequired;
                    host.SetDynamicData(SignKeys.LEVEL, lvl);
                    host.SetDynamicData(SignKeys.EXP, exp);
                }
            };
            lvlUp();
            if (isSave)
            {
                CommonPanel.GetInstance().ShowWaittingPanel(true);
                host.Save((result) =>
                {
                    if (!result)
                    {
                        host.SetDynamicData(SignKeys.LEVEL, originLvl);
                        host.SetDynamicData(SignKeys.EXP, originExp);
                    }
                    else
                    {
                        ItemManageComponent.Instance.DeleteListItem(expHosts);
                    }
                    if (callFunc != null)
                    {
                        callFunc(result);
                    }
                    CommonPanel.GetInstance().ShowWaittingPanel(false);
                });
            }
            return host;
        }

        public void UnlockRole(int _index, Callback _callBack = null)
        {
            int ttype = 0;
            int tcost = 0;
            bool _result = true;
            GetUnLockRoleMoeny(_index, ref ttype, ref tcost);
            if (ttype == GameGlobal.RESOURCE_TYPE_GOLD)
            {
                _result = AccountGoldManagerComponent.Instance.ChangeMoney(tcost, true, new HttpResponseDelegate(((bool result) =>
                {
                    if (!result)
                    {
                        CommonPanel.GetInstance().ShowTextLackMoney();
                        return;
                    }

                    FormulaHost host = GetRole(_index);
                    host.SetDynamicData(SignKeys.LOCKED, 0);
                    //Messenger.Broadcast (MainMenuPanel.BroadcastChangePhysical);
                    host.Save(new HttpResponseDelegate(this.UnlockedHeroCallBack));
                    if (_callBack != null)
                    {
                        _callBack();
                    }

                    CommonPanel.GetInstance().ShowWaittingPanel();
                })));
            }
            else if (ttype == GameGlobal.RESOURCE_TYPE_DIAMOND)
            {
                _result = AccountCrystalManagerComponent.Instance.ChangeDiamond(tcost, true, new HttpResponseDelegate(((bool result) =>
                {
                    if (!result)
                    {
                        CommonPanel.GetInstance().ShowTextLackDiamond();
                        return;
                    }

                    FormulaHost host = GetRole(_index);
                    host.SetDynamicData(SignKeys.LOCKED, 0);
                    //Messenger.Broadcast (MainMenuPanel.BroadcastChangePhysical);
                    host.Save(new HttpResponseDelegate(this.UnlockedHeroCallBack));
                    if (_callBack != null)
                    {
                        _callBack();
                    }

                    CommonPanel.GetInstance().ShowWaittingPanel();
                })));
            }
        }

        public void UnlockedHeroCallBack(bool _success)
        {
            if (_success)
            {
                CommonPanel.GetInstance().ShowWaittingPanel(false);
            }
            else
            {
                CommonPanel.GetInstance().ShowText("connect is fail");
            }

            CommonPanel.GetInstance().ShowWaittingPanel(false);
        }

        /// <summary>
        /// Gets the state of the role locked.
        /// </summary>
        /// <returns><c>true</c>, false==解锁 true==锁定 <c>false</c> otherwise.</returns>
        /// <param name="_index">Index.</param>
        public bool GetRoleLockedState(int _index)
        {
            FormulaHost _role = this.GetRole(_index);
            if (_role == null)
            {
                return true;
            }

            return _role.GetDynamicIntByKey(SignKeys.LOCKED) == 1;
        }

        /// <summary>
        /// 返回当前战斗Girl ID
        /// </summary>
        /// <returns>The fight girl index.</returns>
        public int GetFightGirlIndex()
        {
            if (this.Host == null)
            {
                return -1;
            }

            return this.Host.GetDynamicIntByKey(SignKeys.ID);
        }

        /// <summary>
        /// 设置战斗女孩的索引
        /// </summary>
        /// <param name="_index">Index.</param>
        public void SetFightGirlIndex(int _index, Callback _callBack = null)
        {
            if (this.HostList == null)
            {
                return;
            }

            foreach (string oid in this.HostList.Keys)
            {
                FormulaHost _role = this.HostList[oid];
                if (_role == null)
                {
                    continue;
                }

                if (_index == _role.GetDynamicIntByKey(SignKeys.ID))
                {
                    _role.SetDynamicData(SignKeys.FIGHTHERO, 1);
                    this.Host = _role;
                    this.Host.SetAsUINotifyInstance();
                    Debugger.Log("Set " + _role.GetDynamicStrByKey(SignKeys.NAME) + "(" + _index + ") for fight.");
                }
                else
                {
                    _role.SetDynamicData(SignKeys.FIGHTHERO, 0);
                }
            }

            //Messenger.Broadcast (AdvenTure5.AdvenTure5BraodChangeHero);
            FormulaHost.SaveList(new List<FormulaHost>(this.HostList.Values), new HttpEndResponseDelegate(SetFightGirlCallBack));
            if (CommonPanel.GetInstance() != null)
            {
                CommonPanel.GetInstance().ShowWaittingPanel();
            }
            if (PnlAdventure.PnlAdventure.Instance != null)
            {
                PnlAdventure.PnlAdventure.Instance.ChoseGirl();
            }
            if (_callBack != null)
            {
                _callBack();
            }
        }

        public void SetFightGirlCallBack(cn.bmob.response.EndPointCallbackData<Hashtable> response)
        {
            if (CommonPanel.GetInstance() != null)
            {
                CommonPanel.GetInstance().ShowWaittingPanel(false);
            }
        }

        public void SetFightGirlClothByOrder(int order)
        {
            int idx = this.GetFightGirlIndex();
            if (idx <= 0)
            {
                Debugger.Log("No fight girl selected.");
                return;
            }

            FormulaHost role = this.GetRole(idx);
            if (role == null)
            {
                Debugger.Log("Role " + idx + " has no data.");
                return;
            }

            string name = role.GetDynamicStrByKey(SignKeys.NAME);
            if (name == null || name == string.Empty)
            {
                Debugger.Log("Role " + idx + " has no NAME.");
                return;
            }

            int clothUid = idx * 10 + (order - 1);
            string clothName = ConfigPool.Instance.GetConfigStringValue("clothing", "uid", "name", clothUid);
            if (clothName == null)
            {
                clothUid = idx * 10;
            }

            role.SetDynamicData(SignKeys.CLOTH, clothUid);
            Debugger.Log("Set " + name + " with cloth uid : " + clothUid);
            CommonPanel.GetInstance().ShowText("换装:" + clothName);
        }

        public string GetName(int _index)
        {
            FormulaHost thost = GetRole(_index);
            thost.Result(FormulaKeys.FORMULA_178);
            return thost.GetDynamicStrByKey(SignKeys.NAME);
        }

        public string GetDes(int _index)
        {
            FormulaHost thost = GetRole(_index);
            thost.Result(FormulaKeys.FORMULA_178);
            return thost.GetDynamicStrByKey(SignKeys.DESCRIPTION);
        }

        public void GetUnLockRoleMoeny(int _index, ref int _type, ref int _Cost)
        {
            _type = (int)GetRole(_index).Result(FormulaKeys.FORMULA_4);
            _Cost = (int)GetRole(_index).Result(FormulaKeys.FORMULA_3);
        }

        public ChoseHeroDefine.RESULT_EQUIP GetRoleState(int _index)
        {
            FormulaHost thost = GetRole(_index);
            int Locked = thost.GetDynamicIntByKey(SignKeys.LOCKED);
            int Chose = thost.GetDynamicIntByKey(SignKeys.FIGHTHERO);
            if (Locked == 0)
            {//解锁
                if (Chose == 1)
                {//战斗
                    return ChoseHeroDefine.RESULT_EQUIP.EQUIP_GET;
                }
                else
                {
                    return ChoseHeroDefine.RESULT_EQUIP.BUY_GET;
                }
            }
            else
            {
                return ChoseHeroDefine.RESULT_EQUIP.NO_GET;
            }
        }

        /// <summary>
        /// 获取角色的体力加成
        /// </summary>
        /// <returns>The role physical add.</returns>
        public int GetRolePhysicalAdd()
        {
            int tPhysical = 0;
            foreach (FormulaHost _role in this.HostList.Values)
            {
                if (_role == null)
                {
                    continue;
                }

                tPhysical += _role.GetDynamicIntByKey(SignKeys.PHYSICAL);
            }

            return tPhysical;
        }

        public void SetHeroAttribute(int _index, ref int _effect1, ref int _effect2, ref int effect3, ref int effect4)
        {
            int _idx = _index + 1;
            FormulaHost thost = new FormulaHost(HostKeys.HOST_0);
            thost.SetDynamicData(SignKeys.ID, _idx);
            thost.SetDynamicData(SignKeys.WHO, _idx);
            _effect1 = (int)thost.Result(FormulaKeys.FORMULA_14);
            _effect2 = (int)thost.Result(FormulaKeys.FORMULA_184);
            effect3 = 0;
            effect4 = _index > 0 ? 20 : 10;
        }
    }
}