///自定义模块，可定制模块具体行为
using System;

namespace FormulaBase
{
    public class StoreManageComponent : CustomComponentBase
    {
        private static StoreManageComponent instance = null;
        private const int HOST_IDX = 9;

        public static StoreManageComponent Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new StoreManageComponent();
                    instance.Host = FomulaHostManager.Instance.CreateHost(HOST_IDX);
                }
                return instance;
            }
        }

        // ----------------------------------------------------------------------
        public const int STORE_COST_TYPE_PHY = 1;

        public const int STORE_COST_TYPE_DIAMOND = 2;
        public const int STORE_COST_TYPE_GOLD = 3;

        public int charmRate
        {
            get
            {
                return (int)this.Host.Result(FormulaKeys.FORMULA_115);
            }
        }

        public ushort GetShopType(int _id)
        {
            this.Host.SetDynamicData("ID", _id);
            return (ushort)this.Host.Result(FormulaKeys.FORMULA_27);
        }

        public ushort UseMoneyType(int _id)
        {
            return (ushort)this.Host.Result(FormulaKeys.FORMULA_28);
        }

        public ushort UseMoneyNumber(int _id)
        {
            return (ushort)this.Host.Result(FormulaKeys.FORMULA_29);
        }

        public ushort GetMoneyNumber(int _id)
        {
            return (ushort)this.Host.Result(FormulaKeys.FORMULA_30);
        }

        public ushort GetDiamondNumber(int _id)
        {
            return (ushort)this.Host.Result(FormulaKeys.FORMULA_31);
        }

        public ushort GetPhysicalNumber(int _id)
        {
            return (ushort)this.Host.Result(FormulaKeys.FORMULA_32);
        }

        public string GetImageName(int _id)
        {
            return ConfigPool.Instance.GetConfigStringValue("Store", _id.ToString(), "Image");
        }

        public void UseMoneyBuySomething(int _buystyel, int _buyNumber, int _getStyel, int _getNumber)
        {
            bool _result = true;
            FormulaHost account = AccountManagerComponent.Instance.GetAccount();
            switch (_buystyel)
            {
                case STORE_COST_TYPE_PHY:
                    //if()
                    break;

                case STORE_COST_TYPE_DIAMOND:
                    _result = AccountCrystalManagerComponent.Instance.ChangeCrystal(_buyNumber, true, new HttpResponseDelegate(((bool result) =>
                    {
                        if (!result)
                        {
                            CommonPanel.GetInstance().ShowTextLackDiamond();
                            return;
                        }

                        this.GetRes(_getStyel, _getNumber);
                        //Messenger.Broadcast (MainMenuPanel.Broadcast_MainMenuChangeDiamond);
                    })));

                    if (!_result)
                    {
                        CommonPanel.GetInstance().ShowTextLackDiamond();
                    }

                    break;

                case STORE_COST_TYPE_GOLD:
                    _result = AccountGoldManagerComponent.Instance.ChangeMoney(_buyNumber, true, new HttpResponseDelegate(((bool result) =>
                    {
                        if (!result)
                        {
                            CommonPanel.GetInstance().ShowTextLackMoney();
                            return;
                        }

                        this.GetRes(_getStyel, _getNumber);
                        //Messenger.Broadcast (MainMenuPanel.Broadcast_MainMenuChangeMoney);
                    })));

                    if (!_result)
                    {
                        CommonPanel.GetInstance().ShowTextLackMoney();
                    }
                    break;
            }

            //Messenger.Broadcast (MainMenuPanel.BroadcastChangeMoney);
            //Messenger.Broadcast (MainMenuPanel.BroadcastChangeDiamond);
            //Messenger.Broadcast (MainMenuPanel.BroadcastChangePhysical);
            CommonPanel.GetInstance().ShowWaittingPanel(true);
        }

        private void GetRes(int _getStyel, int _getNumber)
        {
            FormulaHost account = AccountManagerComponent.Instance.GetAccount();
            //	int _money = this.GetMoney ();
            switch (_getStyel)
            {
                case STORE_COST_TYPE_PHY:
                    int _money = AccountGoldManagerComponent.Instance.GetMoney();
                    _money += _getNumber;
                    _money = Math.Min(AccountGoldManagerComponent.Instance.GetMaxMoney(), _money);
                    AccountGoldManagerComponent.Instance.SetMoney(_money);
                    //Messenger.Broadcast (MainMenuPanel.Broadcast_MainMenuChangeMoney);
                    break;

                case STORE_COST_TYPE_DIAMOND:
                    int _diamond = AccountCrystalManagerComponent.Instance.GetCrystal();
                    _diamond += _getNumber;
                    AccountCrystalManagerComponent.Instance.SetDiamond(_diamond);
                    //Messenger.Broadcast (MainMenuPanel.Broadcast_MainMenuChangeDiamond);
                    break;

                case STORE_COST_TYPE_GOLD:
                    int Physical = (int)(account.GetDynamicDataByKey(SignKeys.PHYSICAL));
                    int _result = Physical + _getNumber;
                    account.SetDynamicData(SignKeys.PHYSICAL, _result);
                    //Messenger.Broadcast (MainMenuPanel.Broadcast_MainMenuChangePhysical);
                    break;
            }

            account.Save(new HttpResponseDelegate(this.BuyResCallBack));
        }

        private void BuyResCallBack(bool _success)
        {
            if (_success)
            {
                CommonPanel.GetInstance().ShowWaittingPanel(false);
            }
            else
            {
                CommonPanel.GetInstance().ShowText("Connect is fail");
            }
        }
    }
}