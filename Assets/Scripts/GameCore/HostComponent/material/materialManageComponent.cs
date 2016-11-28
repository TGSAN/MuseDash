using FormulaBase;

///自定义模块，可定制模块具体行为
using System;
using System.Collections.Generic;

namespace FormulaBase
{
    public class MaterialManageComponent : CustomComponentBase
    {
        private static MaterialManageComponent instance = null;
        private const int HOST_IDX = 6;

        public static MaterialManageComponent Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new MaterialManageComponent();
                }
                return instance;
            }
        }

        public FormulaHost CreateItem(int idx)
        {
            if (ItemManageComponent.Instance.ContainsType(idx))
            {
                var h = ItemManageComponent.Instance.GetHostItem(idx);
                return h;
            }
            FormulaHost host = FomulaHostManager.Instance.CreateHost(HOST_IDX);
            if (host != null)
            {
                host.SetDynamicData("ID", idx);
            }
            return host;
        }

        #region 升级相关

        /// <summary>
        /// 获取获得的经验个钱
        /// </summary>
        /// <param name="Exp">Exp.</param>
        /// <param name="Cost">Cost.</param>
        public void GetExpAndCost(ref int Exp, ref int Cost)
        {
            Exp = 0;
            Cost = 0;
            List<FormulaHost> tlist = ItemManageComponent.Instance.GetChosedItem;
            for (int i = 0, max = tlist.Count; i < max; i++)
            {
                //Exp+=(int)tlist[i].Result(FormulaKeys.FORMULA_40)*tlist[i].GetDynamicIntByKey(SignKeys.CHOSED);
                //Cost+=(int)tlist[i].Result(FormulaKeys.FORMULA_41)*tlist[i].GetDynamicIntByKey(SignKeys.CHOSED);
            }
        }

        #endregion 升级相关

        #region 材料堆叠

        /// <summary>
        /// 判断材料中有没相同的ID 或者不能堆叠
        /// </summary>
        /// <returns>The the same I.</returns>
        /// <param name="_ID">I.</param>
        public FormulaHost HaveTheSameID(int _ID)
        {
            int targetId = 0;
            List<FormulaHost> templist = ItemManageComponent.Instance.GetMaterialList;
            for (int i = 0, max = templist.Count; i < max; i++)
            {
                targetId = (int)templist[i].GetDynamicDataByKey("ID");
                if (_ID == targetId)
                {
                    if (templist[i].GetDynamicIntByKey(SignKeys.STACK_NUMBER) != 1)//可以堆叠
                    {
                        int stackNumber = templist[i].GetDynamicIntByKey(SignKeys.STACKITEMNUMBER);
                        stackNumber++;
                        templist[i].SetDynamicData(SignKeys.STACKITEMNUMBER, stackNumber);
                        return templist[i];
                    }
                }
            }
            return null;
        }

        public void CreateItem(List<int> _listIndex)
        {
            //	if(ItemManageComponent.Instance.GetMaterialList.f)
            List<FormulaHost> TempListItem = new List<FormulaHost>();
            for (int i = 0; i < _listIndex.Count; i++)
            {
                FormulaHost temp = HaveTheSameID(_listIndex[i]);
                if (temp == null)//没有相同的ID
                {
                    FormulaHost host = FomulaHostManager.Instance.CreateHost(HOST_IDX);
                    if (host != null)
                    {
                        host.SetDynamicData("ID", _listIndex[i]);
                        //	ItemManageComponent.Instance.AddItem(host);
                    }
                    TempListItem.Add(host);
                }
                else
                {
                    TempListItem.Add(temp);
                }
            }
            ItemManageComponent.Instance.AddItemList(TempListItem);
        }

        #endregion 材料堆叠

        public ushort GetRareType(FormulaHost _host)
        {
            return (ushort)_host.GetDynamicDataByKey("ID");
        }

        #region 宝箱开启用

        private List<RewardData> m_AllMaterial = new List<RewardData>();//所有的素材

        public void Init()
        {
            LitJson.JsonData cfg = ConfigPool.Instance.GetConfigByName("item");
            if (cfg == null)
            {
                return;
            }

            foreach (string id in cfg.Keys)
            {
                LitJson.JsonData _data = cfg[id];
                RewardData temp = new RewardData();
                temp.id = int.Parse(id);
                temp.type = int.Parse(_data["SmallType"].ToString());
                temp.Quality = int.Parse(_data["Quality"].ToString());
                m_AllMaterial.Add(temp);
            }
        }

        public List<RewardData> GetLimitItem(int _Quality = -1, int _Type = -1)
        {
            List<RewardData> temp = new List<RewardData>();

            for (int i = 0, max = m_AllMaterial.Count; i < max; i++)
            {
                if (_Quality == -1 && _Type == -1)
                {
                    temp.Add(m_AllMaterial[i]);
                    continue;
                }

                if (_Quality == -1)
                {
                    if (_Type == m_AllMaterial[i].type)
                    {
                        temp.Add(m_AllMaterial[i]);
                        continue;
                    }
                }
                if (_Type == -1)
                {
                    if (_Quality == m_AllMaterial[i].Quality)
                    {
                        temp.Add(m_AllMaterial[i]);
                        continue;
                    }
                }
                if (_Quality == m_AllMaterial[i].Quality && _Type == m_AllMaterial[i].type)
                {
                    temp.Add(m_AllMaterial[i]);
                }
            }
            return temp;
        }

        #endregion 宝箱开启用
    }
}