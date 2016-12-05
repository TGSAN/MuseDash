using com.ootii.Messages;

///自定义模块，可定制模块具体行为
using System;
using UnityEngine;

namespace FormulaBase
{
    public class AccountGoldManagerComponent : CustomComponentBase
    {
        private static AccountGoldManagerComponent instance = null;
        private const int HOST_IDX = 1;

        public static AccountGoldManagerComponent Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new AccountGoldManagerComponent();
                }
                return instance;
            }
        }

        //---------------------------------------
        public int GetMaxMoney()
        {
            return 99999999;
        }

        public int GetMoney()
        {
            FormulaHost account = AccountManagerComponent.Instance.GetAccount();
            if (account == null)
            {
                return 0;
            }
            return (int)account.GetDynamicDataByKey(SignKeys.GOLD);
        }

        public void SetMoney(int money)
        {
            FormulaHost account = AccountManagerComponent.Instance.GetAccount();
            if (account == null)
            {
                return;
            }

            account.SetDynamicData(SignKeys.GOLD, money);
        }

        // TODO : add money here
        public bool ChangeMoney(int money, bool isave = true, HttpResponseDelegate rsp = null)
        {
            FormulaHost account = AccountManagerComponent.Instance.GetAccount();
            if (account == null)
            {
                return false;
            }

            CommonPanel.GetInstance().ShowWaittingPanel(true);
            bool result = account.AddDynamicValueRemote(SignKeys.GOLD, money, isave, new HttpResponseDelegate((bool _result) =>
            {
                this.ChangeMoneyCallBack(_result);
                if (rsp != null)
                {
                    rsp(_result);
                }
                if (_result)
                {
                    if (money > 0)
                    {
                        //MessageDispatcher.SendMessage("ADD_COIN");
                    }
                }
            }), true, 0, this.GetMaxMoney());

            if (!result)
            {
                CommonPanel.GetInstance().ShowWaittingPanel(false);
            }

            return result;
        }

        private void ChangeMoneyCallBack(bool _Success)
        {
            if (!_Success)
            {
                CommonPanel.GetInstance().ShowText("存储钱失败");
                return;
            }

            //Messenger.Broadcast (MainMenuPanel.BroadcastChangeMoney);
            //Messenger.Broadcast (MainMenuPanel.Broadcast_MainMenuChangeMoney);
            CommonPanel.GetInstance().ShowWaittingPanel(false);
        }
    }
}