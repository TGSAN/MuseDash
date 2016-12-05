///自定义模块，可定制模块具体行为
using System;

namespace FormulaBase
{
    public class AccountCharmComponent : CustomComponentBase
    {
        private static AccountCharmComponent instance = null;
        private const int HOST_IDX = 1;

        public static AccountCharmComponent Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new AccountCharmComponent();
                }
                return instance;
            }
        }

        private bool m_IsAdd = false;

        public void DetectAdd(int num = 0)
        {
            if (m_IsAdd && PnlMainMenu.PnlMainMenu.Instance != null)
            {
                m_IsAdd = false;
                PnlMainMenu.PnlMainMenu.Instance.charm.FlyAll();
                PnlMainMenu.PnlMainMenu.Instance.OnCharmUpdate();
            }
        }

        //---------------------------------------
        public int GetMaxCharm()
        {
            return 9999;
        }

        public int GetCharm()
        {
            FormulaHost account = AccountManagerComponent.Instance.GetAccount();
            if (account == null)
            {
                return 0;
            }
            return (int)account.GetDynamicDataByKey(SignKeys.CHARM);
        }

        public void SetCharm(int charm)
        {
            FormulaHost account = AccountManagerComponent.Instance.GetAccount();
            if (account == null)
            {
                return;
            }

            account.SetDynamicData(SignKeys.CHARM, charm);
        }

        // TODO : add money here
        public bool ChangeCharm(int charm, bool isave = true, HttpResponseDelegate rsp = null)
        {
            FormulaHost account = AccountManagerComponent.Instance.GetAccount();
            if (account == null)
            {
                return false;
            }

            CommonPanel.GetInstance().ShowWaittingPanel(true);
            bool result = account.AddDynamicValueRemote(SignKeys.CHARM, charm, isave, new HttpResponseDelegate((bool _result) =>
            {
                this.ChangeMoneyCallBack(_result);
                if (rsp != null)
                {
                    rsp(_result);
                }
                if (charm > 0)
                {
                    m_IsAdd = _result;
                    DetectAdd(charm);
                }
            }), true, 0, this.GetMaxCharm());

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
                CommonPanel.GetInstance().ShowText("存储魔能失败");
                return;
            }

            //Messenger.Broadcast (MainMenuPanel.BroadcastChangeMoney);
            //Messenger.Broadcast (MainMenuPanel.Broadcast_MainMenuChangeMoney);
            CommonPanel.GetInstance().ShowWaittingPanel(false);
        }
    }
}