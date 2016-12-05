///自定义模块，可定制模块具体行为
using System;
using UnityEngine;

namespace FormulaBase
{
    public class AccountLevelManagerComponent : CustomComponentBase
    {
        private static AccountLevelManagerComponent instance = null;
        private const int HOST_IDX = 1;

        public static AccountLevelManagerComponent Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new AccountLevelManagerComponent();
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
                PnlMainMenu.PnlMainMenu.Instance.exp.FlyAll();
                PnlMainMenu.PnlMainMenu.Instance.OnExpUpdate();
            }
        }

        public int GetExp()
        {
            FormulaHost account = AccountManagerComponent.Instance.GetAccount();
            if (account == null)
            {
                return 0;
            }

            return account.GetDynamicIntByKey(SignKeys.EXP);
        }

        public int GetLvl()
        {
            FormulaHost account = AccountManagerComponent.Instance.GetAccount();
            if (account == null)
            {
                return 1;
            }

            return account.GetDynamicIntByKey(SignKeys.LEVEL);
        }

        public int NextLvlExp()
        {
            return ConfigPool.Instance.GetConfigIntValue("experience", GetLvl().ToString(), "account_exp");
        }

        // TODO : add exp of account
        public bool ChangeExp(int _exp, bool isave = true, HttpResponseDelegate rsp = null)
        {
            FormulaHost account = AccountManagerComponent.Instance.GetAccount();
            if (account == null)
            {
                return false;
            }

            CommonPanel.GetInstance().ShowWaittingPanel(true);
            bool result = account.AddDynamicValueRemote(SignKeys.EXP, _exp, isave, new HttpResponseDelegate((bool _result) =>
            {
                this.ChangeExpCallBack(_result);
                if (_exp > 0)
                {
                    m_IsAdd = _result;
                    DetectAdd(_exp);
                }
                if (rsp != null)
                {
                    rsp(_result);
                }
            }));

            if (!result)
            {
                CommonPanel.GetInstance().ShowWaittingPanel(false);
            }

            return result;
        }

        private void ChangeExpCallBack(bool _Success)
        {
            if (!_Success)
            {
                CommonPanel.GetInstance().ShowText("存储经验值失败");
                return;
            }

            CommonPanel.GetInstance().ShowWaittingPanel(false);
        }

        public bool LevelUp(FormulaHost host)
        {
            int current = host.GetDynamicIntByKey(SignKeys.LEVEL) + 1;
            Debug.Log("account level up : " + current);
            host.SetDynamicData(SignKeys.LEVEL, current);

            return host.GetDynamicIntByKey(SignKeys.LEVEL) == current;
        }

        public bool LevelDown(FormulaHost host)
        {
            int current = host.GetDynamicIntByKey(SignKeys.LEVEL) - 1;
            Debug.Log("account level down : " + current);
            host.SetDynamicData(SignKeys.LEVEL, current);

            return host.GetDynamicIntByKey(SignKeys.LEVEL) == current;
        }
    }
}