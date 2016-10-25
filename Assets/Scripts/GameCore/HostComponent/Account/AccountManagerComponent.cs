using LitJson;

///自定义模块，可定制模块具体行为
using System;
using System.Collections;
using UnityEngine;

namespace FormulaBase
{
    public class AccountManagerComponent : CustomComponentBase
    {
        private static AccountManagerComponent instance = null;
        private const int HOST_IDX = 1;

        public static AccountManagerComponent Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new AccountManagerComponent();
                }
                return instance;
            }
        }

        // ------------------------------------------------------------------------------------------------
        private const string LOGIN_COUNT = "LOGIN_COUNT";

        public void Init()
        {
            if (this.Host == null)
            {
                this.Host = FomulaHostManager.Instance.LoadHost(HOST_IDX);
                this.Host.SetAsUINotifyInstance();
            }

            //Messenger.Broadcast (MainMenuPanel.BroadcastChangeMoney);
            //Messenger.Broadcast (MainMenuPanel.BroadcastChangeDiamond);
            //Messenger.Broadcast (MainMenuPanel.BroadcastChangePhysical);
        }

        public FormulaHost GetAccount()
        {
            return this.Host;
        }

        public void DeletePlayerData(HttpEndResponseDelegate _callBack)
        {
            ItemManageComponent.Instance.GetItemTimeId = 0;
            FomulaHostManager.Instance.DeleteAllHost(_callBack);
        }

        public void AddLoginCount(int count)
        {
            FormulaHost account = this.GetAccount();
            if (account == null)
            {
                return;
            }

            int c = account.GetDynamicIntByKey(LOGIN_COUNT);
            account.SetDynamicData(LOGIN_COUNT, c + 1);
            account.Save(new HttpResponseDelegate(this.OnAddLoginCount));
        }

        private void OnAddLoginCount(bool result)
        {
            if (!result)
            {
                Debug.Log("Add login count failed.");
                return;
            }
        }

        // ------------------------------------------------------------------------------------------------
        // ------------------------------------------------------------------------------------------------
        // ------------------------------------------------------------------------------------------------

        #region 章节

        public int GetOpenedChapter()
        {
            FormulaHost account = this.GetAccount();
            return account.GetDynamicIntByKey(SignKeys.OPENEDCHAPTER);
        }

        #endregion 章节

        #region 获取宝箱相关

        //（1）1个免费，5个钻石付费解锁，但前2个的付费解锁代价很低，依次增多：免费、1钻石、10钻石、100钻石、150钻石、200钻石（配置表实现）

        /// <summary>
        /// 获取宝箱栏位
        /// </summary>
        /// <returns>The chest gird number.</returns>
        public int GetChestGirdNumber()
        {
            return (int)this.Host.GetDynamicDataByKey(SignKeys.CHESTGRID);
        }

        /// <summary>
        /// 设置宝箱栏位的数量
        /// </summary>
        public void SetChestGirdNumber(int number, HttpResponseDelegate _CallBackFun)
        {
            CommonPanel.GetInstance().ShowWaittingPanel(true);
            //	FormulaHost account = this.GetAccount ();
            int NowEmpty = (int)this.Host.GetDynamicDataByKey(SignKeys.CHESTGRID);
            Debug.Log("设置箱子栏位的个数：" + (NowEmpty + number));
            if (NowEmpty + number <= 0)
            {//什么状况出现小于一个的情况默认设置格子为1
                this.Host.SetDynamicData(SignKeys.CHESTGRID, 1);
            }
            else
            { //设置账户宝箱栏位的个数
                this.Host.SetDynamicData(SignKeys.CHESTGRID, NowEmpty + number);
            }

            this.Host.Save(_CallBackFun);
        }

        #endregion 获取宝箱相关
    }
}