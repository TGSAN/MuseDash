///自定义模块，可定制模块具体行为
using System;
using UnityEngine;

namespace FormulaBase
{
    public class BagManageComponent : CustomComponentBase
    {
        private static BagManageComponent instance = null;
        private const int HOST_IDX = 8;

        public static BagManageComponent Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new BagManageComponent();
                }
                return instance;
            }
        }

        private FormulaHost host = null;

        public void Init()
        {
            this.host = FomulaHostManager.Instance.LoadHost(HOST_IDX);
        }

        public FormulaHost GetBagInfo()
        {
            return this.host;
        }

        /// <summary>
        /// 获取背包的大小
        /// </summary>
        /// <returns>The bag size.</returns>
        public ushort GetBagSize()
        {
            return 100;
            //return (ushort)host.Result(FormulaKeys.FORMULA_82);
        }

        /// <summary>
        /// 获取升级背包需要的钻石
        /// </summary>
        /// <returns>The up diamond.</returns>
        public int GetUpDiamond()
        {
            int current = ((int)host.GetDynamicDataByKey("ID")) + 1;
            LitJson.JsonData d = ConfigPool.Instance.GetConfigValue("bag", current.ToString(), "DiamondCost");
            if (d == null)
            {
                return -1;//到达上线
            }
            else
            {
                return int.Parse(d.ToString());
            }
        }

        /// <summary>
        /// 升级背包
        /// </summary>
        public void UpBag()
        {
            int current = ((int)host.GetDynamicDataByKey("ID")) + 1;
            int cost = -1 * ConfigPool.Instance.GetConfigIntValue("bag", current.ToString(), "DiamondCost");
            if (cost <= 0)
            {
                return;//到达上线
            }

            AccountCrystalManagerComponent.Instance.ChangeCrystal(cost, true, new HttpResponseDelegate(((bool result) =>
            {
                host.SetDynamicData(SignKeys.ID, current);
                host.Save();

                Messenger.Broadcast(bagPanel2.BroadcastBagEmptyReSet);
            })));
        }

        public void DeleteBagData()
        {
            this.host = FomulaHostManager.Instance.LoadHost(HOST_IDX);
            //	FomulaHostManager.Instance.DeleteHost(host);
            //	this.host = FomulaHostManager.Instance.LoadHost (HOST_IDX);
            //			host.Save();
            //	this.host = FomulaHostManager.Instance.CreateHost (HOST_IDX);
        }

        /// <summary>
        /// 判断背包是否满
        /// </summary>
        /// <returns><c>true</c>, if bagis full was checked, <c>false</c> otherwise.</returns>
        public bool CheckBagisFull()
        {
            if (GetBagSize() > ItemManageComponent.Instance.GetAllItemCount())
            {
                Debug.Log("背包未满");
                return true;
            }
            else
            {
                Debug.Log("背包已满");
                return false;
            }
        }

        public bool GetBagNewItem()
        {
            if (host != null)
                return host.GetDynamicIntByKey(SignKeys.BAG_HAVENEWITEM) != 0;
            else
                return false;
        }

        public void SetBagHaveNew(int Show = 1)
        {
            if (host == null)
            {
                return;
            }
            host.SetDynamicData(SignKeys.BAG_HAVENEWITEM, Show);
            //Messenger.Broadcast(MainMenuPanel.Broadcast_BagHaveNewItem);
            host.Save(new HttpResponseDelegate(SetBagHaveNewCallBack));
        }

        public void SetBagHaveNewCallBack(bool _success)
        {
            if (_success)
            {
                Debug.Log("Bag new Save Success");
            }
            else
            {
                CommonPanel.GetInstance().ShowText("connect is fail");
            }
        }
    }

    //GetItems()提取所有能放背包的数据
    //
    //
}