///自定义模块，可定制模块具体行为
using System;
using Assets.Scripts.Tools.Managers;
using UnityEngine;

namespace FormulaBase
{
    public class AccountPhysicsManagerComponent : CustomComponentBase
    {
        private static AccountPhysicsManagerComponent instance = null;
        private const int HOST_IDX = 1;

        public static AccountPhysicsManagerComponent Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new AccountPhysicsManagerComponent();
                }
                return instance;
            }
        }

        // ------------------------------------------------------------------
        public const string SIGN_KEY_TXT_ENERGY = "TXT_ENERGY";

        private bool m_IsAdd = false;

        public void DetectAdd(int num = 0)
        {
            if (m_IsAdd && PnlMainMenu.PnlMainMenu.Instance != null)
            {
                m_IsAdd = false;
                //PnlMainMenu.PnlMainMenu.Instance.energy.FlyAll();
                PnlMainMenu.PnlMainMenu.Instance.OnEnergyUpdate();
            }
        }

        public void Init()
        {
            this.SetPhysicText();
        }

        public void AutoPhysicalRecover()
        {
            Debug.Log("开启体力自动恢复计时");
            FormulaHost apHost = TimerComponent.Instance.GetAccountPhysicalTimer();
            // 下线期间体力恢复
            if (apHost != null)
            {
                int _t = apHost.GetRealTimeCountDownNow();
                if (_t < 0)
                {
                    int v = Math.Abs(_t / TimerComponent.AP_CD) * TimerComponent.AP_VALUE;
                    this.ChangePhysical(v, false, false);
                    Debug.Log("线下体力补充" + _t + " / " + v);
                }
            }

            TimerComponent.Instance.SetAccountPhysicalTimer(false);
            AccountManagerComponent.Instance.Host.SetAsUINotifyInstance();
        }

        /// <summary>
        /// 获取玩家的最大体力
        /// </summary>
        public int GetMaxPhysical()
        {
            return GetAccountPhysical() + RoleManageComponent.Instance.GetRolePhysicalAdd();
        }

        public int IsMaxPhysicalChange()
        {
            var physicsMax = AccountManagerComponent.Instance.GetAccount().GetDynamicIntByKey(SignKeys.PHYSICALMAX);
            return GetMaxPhysical() - physicsMax;
        }

        public int GetAccountPhysical()
        {
            if (AccountManagerComponent.Instance.GetAccount() != null)
            {
                return ConfigManager.instance.GetConfigIntValue("experience",
                    AccountManagerComponent.Instance.GetAccount().GetDynamicIntByKey(SignKeys.LEVEL) - 1, "energy");
            }
            return 0;
        }

        /// <summary>
        /// 获取玩家体力
        /// </summary>
        /// <returns>The max physical.</returns>
        public int GetPhysical()
        {
            FormulaHost account = AccountManagerComponent.Instance.GetAccount();
            if (account == null)
            {
                return 0;
            }
            var changeValue = IsMaxPhysicalChange();
            if (changeValue != 0)
            {
                AccountManagerComponent.Instance.Host.AddDynamicValue(SignKeys.PHYSICAL, changeValue);
                AccountManagerComponent.Instance.Host.AddDynamicValue(SignKeys.PHYSICALMAX, changeValue);
                AccountManagerComponent.Instance.Host.Save();
            }
            return (int)account.GetDynamicDataByKey(SignKeys.PHYSICAL);
        }

        public void ChangeEnergyToMax()
        {
            var max = GetMaxPhysical();
            var cur = GetPhysical();
            ChangePhysical(max - cur, false, true, result =>
            {
                CommonPanel.GetInstance().ShowWaittingPanel(false);
            });
        }

        /// <summary>
        /// 改变玩家体力
        ///
        /// Sample :
        /// AccountManagerComponent.Instance.ChangePhysical (60, true, true, new HttpResponseDelegate (((bool result) => {
        /// 	Debug.Log ("On Change Physical : " + result);
        /// })));
        /// </summary>
        /// <param name="_value">改变数</param>
        /// <param name="_crash">是否能爆表<c>true</c> crash.</param>	//可能超过上下限时全部设置crash为True
        public ushort ChangePhysical(int _value, bool _isMatchLimit = false, bool isave = true, HttpResponseDelegate rsp = null)
        {
            FormulaHost account = AccountManagerComponent.Instance.GetAccount();
            if (account == null)
            {
                return 0;
            }

            CommonPanel.GetInstance().ShowWaittingPanel(true);

            bool result = account.AddDynamicValueRemote(SignKeys.PHYSICAL, _value, isave, new HttpResponseDelegate((bool _result) =>
            {
                this.ChangePhysicalCallBack(_result);
                if (_value > 0)
                {
                    m_IsAdd = _result;
                    DetectAdd(_value);
                }
                if (rsp != null)
                {
                    rsp(_result);
                }
            }), _isMatchLimit, 0, this.GetMaxPhysical());

            if (!result)
            {
                CommonPanel.GetInstance().ShowWaittingPanel(false);
                return 0;
            }

            return 1;
        }

        private void SetPhysicText()
        {
            /*FormulaHost account = AccountManagerComponent.Instance.GetAccount();
            if (account == null)
            {
                return;
            }

            string txtPhy = this.GetPhysical() + "/" + this.GetMaxPhysical();
            account.SetDynamicData(SIGN_KEY_TXT_ENERGY, txtPhy);*/
        }

        /// <summary>
        /// 接受改变体力的反馈
        /// </summary>
        private void ChangePhysicalCallBack(bool _Success)
        {
            if (!_Success)
            {
                CommonPanel.GetInstance().ShowText("连接失败");
                return;
            }

            this.SetPhysicText();
            //Messenger.Broadcast (MainMenuPanel.BroadcastChangePhysical);
            //Messenger.Broadcast (MainMenuPanel.Broadcast_MainMenuChangePhysical);
        }
    }
}