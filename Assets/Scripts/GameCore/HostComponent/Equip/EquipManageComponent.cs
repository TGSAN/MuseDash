///自定义模块，可定制模块具体行为
using System;
using System.Collections.Generic;
using System.Linq;

namespace FormulaBase
{
    public class EquipManageComponent : CustomComponentBase
    {
        private static EquipManageComponent instance = null;
        private const int HOST_IDX = 5;

        public static EquipManageComponent Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new EquipManageComponent();
                }
                return instance;
            }
        }

        private List<RewardData> m_AllEquip = new List<RewardData>();
        private List<FormulaHost> m_EquipedEquipment = new List<FormulaHost>();     //装备的装备的链表

        /// <summary>
        /// 判断是否装备相同的装备
        /// </summary>
        /// <returns><c>true</c>, if have same equip was checked, <c>false</c> otherwise.</returns>
        /// <param name="_host">Host.</param>
        /// <param name="_index">Index.</param>
        public bool CheckHaveSameEquip(FormulaHost _host, int _index)
        {
            for (int i = 0, max = m_EquipedEquipment.Count; i < max; i++)
            {
                if (m_EquipedEquipment[i].GetDynamicIntByKey(SignKeys.EQUIPEDQUEUE) / 10 == RoleManageComponent.Instance.GetFightGirlIndex())
                {
                    if (_host.GetDynamicIntByKey(SignKeys.ID) == m_EquipedEquipment[i].GetDynamicIntByKey(SignKeys.ID))
                    {
                        if (_index == m_EquipedEquipment[i].GetDynamicIntByKey(SignKeys.EQUIPEDQUEUE))
                        {
                            return false;
                        }
                        else
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// 获取指定角色的装备 idx为角色id pos为类型 0时代表所有装备 isEquiping为false则所有装备 true代表仅装备的装备
        /// </summary>
        /// <param name="idx"></param>
        /// <returns></returns>
        public FormulaHost[] GetGirlEquipHosts(int idx, int typePos = 0, bool isEquiping = false)
        {
            var equipHosts = new List<FormulaHost>();
            var equipTypeList = new List<string>(GetGirlEquipTypes(idx));
            foreach (var formulaHost in HostList.Values)
            {
                var equipID = formulaHost.GetDynamicIntByKey(SignKeys.ID);
                var equipInfo = ConfigPool.Instance.GetConfigValue("items", equipID.ToString());
                if (equipInfo == null)
                {
                    continue;
                }
                var typeID = (string)equipInfo["type"];

                if (equipTypeList.Contains(typeID))
                {
                    var index = equipTypeList.IndexOf(typeID) + 1;
                    if (index == typePos || typePos == 0)
                    {
                        if (isEquiping)
                        {
                            if (formulaHost.GetDynamicIntByKey(SignKeys.WHO) != 0)
                            {
                                equipHosts.Add(formulaHost);
                            }
                        }
                        else
                        {
                            equipHosts.Add(formulaHost);
                        }
                    }
                }
            }
            return equipHosts.ToArray();
        }

        /// <summary>
        /// 获取指定角色装备类型的字符串
        /// </summary>
        /// <param name="idx"></param>
        /// <returns></returns>
        public string[] GetGirlEquipTypes(int idx)
        {
            var characterInfo = ConfigPool.Instance.GetConfigValue("char_info", idx.ToString());
            var typeIDs = new string[3];
            for (int i = 1; i < 4; i++)
            {
                string weaponName = "weapon_" + i.ToString();
                if (!characterInfo.Keys.Contains(weaponName))
                {
                    continue;
                }

                typeIDs[i - 1] = (string)characterInfo[weaponName];
            }
            return typeIDs;
        }

        /// <summary>
        /// 获取装备的人物ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public int GetEquipOwnerIdx(int id)
        {
            var itemInfo = ConfigPool.Instance.GetConfigValue("items", id.ToString());
            var typeID = (string)itemInfo["type"];
            for (int i = 1; i <= RoleManageComponent.Instance.GetRoleCount(); i++)
            {
                var tpyeList = new List<string>(GetGirlEquipTypes(i));
                if (tpyeList.Contains(typeID))
                {
                    return i;
                }
            }
            return 0;
        }

        /// <summary>
        /// 获取指定装备的装备
        /// </summary>
        /// <returns>The equiped equip.</returns>
        /// <param name="_index">Index.</param>
        public FormulaHost GetEquipedEquip(int _index)
        {
            for (int i = 0, max = m_EquipedEquipment.Count; i < max; i++)
            {
                if (_index + RoleManageComponent.Instance.GetFightGirlIndex() * 10 == m_EquipedEquipment[i].GetDynamicIntByKey(SignKeys.EQUIPEDQUEUE))
                {
                    return m_EquipedEquipment[i];
                }
            }
            return null;
        }

        /// <summary>
        /// 装备, isTo为true装备，false卸妆
        /// </summary>
        /// <param name="equipID"></param>
        /// <param name="isTo"></param>
        public void Equip(int equipID, bool isTo = true, HttpResponseDelegate func = null)
        {
            FormulaHost host = (from value in HostList.Values let id = value.GetDynamicIntByKey(SignKeys.ID) where id == equipID select value).FirstOrDefault();

            var ownerIdx = GetEquipOwnerIdx(equipID);
            if (isTo)
            {
                if (host != null) host.SetDynamicData(SignKeys.WHO, ownerIdx);
            }
            else
            {
                if (host != null) host.SetDynamicData(SignKeys.WHO, 0);
            }
            if (host != null)
            {
                host.Save();
                RoleManageComponent.Instance.Host = RoleManageComponent.Instance.GetRole(ownerIdx);
                RoleManageComponent.Instance.Equip(host, isTo, func);
            }
        }

        /// <summary>
        /// 获取同一套装内的所有拥有的装备
        /// </summary>
        /// <param name="host"></param>
        /// <returns></returns>
        public List<FormulaHost> GetEquipWithSameSuit(FormulaHost host)
        {
            var list = new List<FormulaHost>();
            var suit = host.GetDynamicStrByKey(SignKeys.SUIT);
            if (suit != "0")
            {
                var roleID = GetEquipOwnerIdx(host.GetDynamicIntByKey(SignKeys.ID));
                var allEquipHost = GetGirlEquipHosts(roleID);
                list.AddRange(allEquipHost.Where(equipHost => suit == equipHost.GetDynamicStrByKey(SignKeys.SUIT)));
            }
            return list;
        }

        /// <summary>
        /// 获取该装备的套装中所有装备名
        /// </summary>
        /// <param name="host"></param>
        /// <returns></returns>
        public List<string> GetEquipNameWithSameSuit(FormulaHost host)
        {
            var list = new List<string>();
            var suitName = host.GetDynamicStrByKey(SignKeys.SUIT);
            if (suitName != "0")
            {
                var itemInfo = ConfigPool.Instance.GetConfigByName("items");
                for (int i = 1; i < itemInfo.Count; i++)
                {
                    var sName = itemInfo[i.ToString()]["suit"].ToString();
                    if (suitName == sName)
                    {
                        list.Add(itemInfo[i.ToString()]["name"].ToString());
                        if (list.Count == 3)
                        {
                            break;
                        }
                    }
                }
            }
            return list;
        }

        /// <summary>
        /// 计算选择物品的经验
        /// </summary>
        public void CountExp()
        {
            int allExp = 0;
            List<FormulaHost> tChosedList = ItemManageComponent.Instance.GetChosedItem;
            for (int i = 0, max = tChosedList.Count; i < max; i++)
            {
                allExp += (int)tChosedList[i].Result(FormulaKeys.FORMULA_36) + tChosedList[i].GetDynamicIntByKey(SignKeys.EXP);
            }

            NGUIDebug.Log("总共的经验为:" + allExp);
        }

        /// <summary>
        /// 装备升级
        /// </summary>
        /// <param name="_host">Host.</param>
        /// <param name="_UpNumber">Up number.</param>
        public void EquipLevelUp(FormulaHost _host, int _UpNumber)
        {
            int tlevel = _host.GetDynamicIntByKey(SignKeys.LEVEL);
            int tMaxLevel = (int)_host.Result(FormulaKeys.FORMULA_23);
            tlevel += _UpNumber;
            if (tlevel > tMaxLevel)
            {
                tlevel = tMaxLevel;
                NGUIDebug.Log("装备等级到达上限");
            }
            _host.SetDynamicData(SignKeys.LEVEL, tlevel);
            CommonPanel.GetInstance().ShowWaittingPanel();
            _host.Save(new HttpResponseDelegate(EquipLevelUpCallback));
        }

        private void EquipLevelUpCallback(bool _success)
        {
            if (_success)
            {
                CommonPanel.GetInstance().ShowWaittingPanel(false);
                NGUIDebug.Log("接受装备升级的回调");
            }
            else
            {
                CommonPanel.GetInstance().ShowText("Conect is fail");
            }
        }

        /// <summary>
        /// 获取装备上的装备
        /// </summary>
        /// <value>The get equiped equip.</value>
        public List<FormulaHost> GetEquipedEquipList
        {
            get
            {
                return m_EquipedEquipment;
            }
            set
            {
                m_EquipedEquipment = value;
            }
        }

        /// <summary>
        /// 添加装备了的装备
        /// </summary>
        /// <param name="_host">Host.</param>
        public void AddEquipedItem(FormulaHost _host)
        {
            m_EquipedEquipment.Add(_host);
        }

        /// <summary>
        /// 移除装备了的装备
        /// </summary>
        /// <param name="h"></param>
        public void RemoveEquipItem(FormulaHost h)
        {
            m_EquipedEquipment.Remove(h);
        }

        /// <summary>
        /// 获取所有装备
        /// </summary>
        public void Init()
        {
            this.GetList("Equip");
            if (this.HostList != null)
            {
                foreach (FormulaHost host in this.HostList.Values)
                {
                    host.Result(FormulaKeys.FORMULA_19);
                }
            }

            LitJson.JsonData cfg = ConfigPool.Instance.GetConfigByName("Equipment_info");
            if (cfg == null)
            {
                return;
            }

            foreach (string id in cfg.Keys)
            {
                LitJson.JsonData _data = cfg[id];
                RewardData temp = new RewardData();
                temp.id = int.Parse(id);
                temp.type = int.Parse(_data["Type"].ToString());
                temp.Quality = int.Parse(_data["Quality"].ToString());
                m_AllEquip.Add(temp);
            }
        }

        /// <summary>
        /// 返回限定的物品的链表 默认返回表里的所有东西
        /// </summary>
        /// <returns>The limit item.</returns>
        /// <param name="Quality">Quality.</param>
        /// <param name="Type">Type.</param>
        public List<RewardData> GetLimitItem(int _Quality = -1, int _Type = -1)
        {
            List<RewardData> temp = new List<RewardData>();
            for (int i = 0, max = m_AllEquip.Count; i < max; i++)
            {
                if (_Quality == -1 && _Type == -1)
                {
                    temp.Add(m_AllEquip[i]);
                    continue;
                }

                if (_Quality == -1)
                {
                    if (_Type == m_AllEquip[i].type)
                    {
                        temp.Add(m_AllEquip[i]);
                        continue;
                    }
                }
                if (_Type == -1)
                {
                    if (_Quality == m_AllEquip[i].Quality)
                    {
                        temp.Add(m_AllEquip[i]);
                        continue;
                    }
                }
                if (_Quality == m_AllEquip[i].Quality && _Type == m_AllEquip[i].type)
                {
                    temp.Add(m_AllEquip[i]);
                }
            }
            return temp;
        }

        #region 升级相关

        public void GetExpAndCost(ref int Exp, ref int Cost)
        {
            Exp = 0;
            Cost = 0;
            FormulaHost tempEquip = new FormulaHost(HOST_IDX);
            List<FormulaHost> tList = ItemManageComponent.Instance.GetChosedItem;
            for (int i = 0, max = tList.Count; i < max; i++)
            {
                Cost += (int)tList[i].Result(FormulaKeys.FORMULA_37);
                Exp += (int)tList[i].Result(FormulaKeys.FORMULA_36) + tList[i].GetDynamicIntByKey(SignKeys.EXP);
                int tLevel = (int)tList[i].GetDynamicIntByKey(SignKeys.LEVEL);

                tempEquip.SetDynamicData(SignKeys.ID, tList[i].GetDynamicIntByKey(SignKeys.ID));
                for (int j = 1; j < tLevel; j++)
                {//大于1级的情况
                    tempEquip.SetDynamicData(SignKeys.LEVEL, j);
                    Exp += (int)tempEquip.Result(FormulaKeys.FORMULA_155);
                }
            }
        }

        /// <summary>
        /// 获取升级后的host
        /// </summary>
        public FormulaHost GetLevelUpHost(FormulaHost _host)
        {
            int Exp = 0;
            int Cost = 0;
            GetExpAndCost(ref Exp, ref Cost);
            FormulaHost thost = new FormulaHost(HOST_IDX);
            thost.SetDynamicData(SignKeys.ID, _host.GetDynamicIntByKey(SignKeys.ID));
            //			thost.SetDynamicData(SignKeys.LEVEL,Level);
            //			thost.SetDynamicData(SignKeys.EXP,Exp);
            //thost
            Exp += _host.GetDynamicIntByKey(SignKeys.EXP);
            //Exp+=1000;
            int LevelUpExp = (int)_host.Result(FormulaKeys.FORMULA_155);
            int Level = _host.GetDynamicIntByKey(SignKeys.LEVEL);
            while (LevelUpExp <= Exp)
            {
                Level++;
                Exp -= LevelUpExp;
                thost.SetDynamicData(SignKeys.LEVEL, Level);
                LevelUpExp = (int)thost.Result(FormulaKeys.FORMULA_155);
                if (Level == (int)thost.Result(FormulaKeys.FORMULA_23))
                {
                    NGUIDebug.Log("到达等级上限");
                    return thost;
                }
            }
            //thost.SetDynamicData(SignKeys.LEVEL,Level);
            thost.SetDynamicData(SignKeys.EXP, Exp);
            return thost;
        }

        #endregion 升级相关

        public FormulaHost CreateItem(int idx)
        {
            FormulaHost host = FomulaHostManager.Instance.CreateHost(HOST_IDX);
            if (host != null)
            {
                host.SetDynamicData(SignKeys.ID, idx);
                //ItemManageComponent.Instance.AddItem(host);
            }
            return host;
        }

        public void CreateItem(List<int> _listIndex)
        {
            List<FormulaHost> TempListItem = new List<FormulaHost>();
            for (int i = 0; i < _listIndex.Count; i++)
            {
                FormulaHost host = FomulaHostManager.Instance.CreateHost(HOST_IDX);
                if (host != null)
                {
                    host.SetDynamicData(SignKeys.ID, _listIndex[i]);
                    //	ItemManageComponent.Instance.AddItem(host);
                }
                TempListItem.Add(host);
            }
            ItemManageComponent.Instance.AddItemList(TempListItem);
        }

        public ushort GetRareType(FormulaHost _host)
        {
            return (ushort)_host.GetDynamicDataByKey(SignKeys.ID);
        }

        public void GetAllEquipedEquip(ref int _hp, ref int _df, ref int _att, ref int _crit)
        {
            for (int i = 0; i < m_EquipedEquipment.Count; i++)
            {
                FormulaHost host = m_EquipedEquipment[i];
                if (host == null)
                {
                    continue;
                }

                if (host.GetDynamicIntByKey(SignKeys.EQUIPEDQUEUE) / 10 == RoleManageComponent.Instance.GetFightGirlIndex())
                {
                    _hp += (int)host.Result(FormulaKeys.FORMULA_26);                    //Hp
                    _att += (int)host.Result(FormulaKeys.FORMULA_29);                   //Def
                    _df += (int)host.Result(FormulaKeys.FORMULA_32);    //Att
                    _crit += (int)host.Result(FormulaKeys.FORMULA_35);                  //Crt
                }
            }
        }
    }
}