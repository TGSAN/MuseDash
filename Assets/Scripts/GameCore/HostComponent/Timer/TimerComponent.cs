///自定义模块，可定制模块具体行为
using System;
using System.Collections.Generic;
using UnityEngine;

namespace FormulaBase
{
    public class TimerComponent : CustomComponentBase
    {
        private static TimerComponent instance = null;
        private const int HOST_IDX = 15;

        public static TimerComponent Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new TimerComponent();
                }
                return instance;
            }
        }

        // ------------------------------------------------------------------------
        // 已经被设置为时间到
        private const string SIGN_KEY_TIME_UP = "SIGN_KEY_TIME_UP";

        // 账号体力 Account Physical
        public const string AP_TIMER = "AP_TIMER";

        public const int AP_CD = 300;
        public const int AP_VALUE = 1;

        private delegate void DlgTimeUp(FormulaHost host);

        private Dictionary<string, DlgTimeUp> _timeUpFunctionMap = null;

        public TimerComponent()
        {
            this.GetList(HOST_IDX);
            if (this._timeUpFunctionMap == null)
            {
                this._timeUpFunctionMap = new Dictionary<string, DlgTimeUp>();

                // 注册不同倒计时 timeup 处理的功能函数
                this._timeUpFunctionMap[AP_TIMER] = (DlgTimeUp)Delegate.CreateDelegate(typeof(DlgTimeUp), this, "AccountPhysicalTimeUp");
            }
        }

        public void Init()
        {
            if (this.HostList == null)
            {
                return;
            }

            foreach (string oid in this.HostList.Keys)
            {
                FormulaHost _timerHost = this.HostList[oid];
                if (_timerHost == null)
                {
                    continue;
                }

                this.RunTimer(_timerHost);
            }
        }

        /// <summary>
        /// Times up.
        ///
        /// 倒计时间到
        /// </summary>
        /// <param name="host">Host.</param>
        public void TimeUp(FormulaHost host)
        {
            if (host == null)
            {
                return;
            }

            host.SetDynamicData(SIGN_KEY_TIME_UP, 1);
            string tname = host.GetDynamicStrByKey(SignKeys.NAME);
            if (tname == null)
            {
                return;
            }

            Debug.Log("Host Timer " + tname + " TimeUp");
            if (this._timeUpFunctionMap == null || !this._timeUpFunctionMap.ContainsKey(tname))
            {
                return;
            }

            Delegate dlg = this._timeUpFunctionMap[tname];
            dlg.DynamicInvoke(new object[] { host });
        }

        /// <summary>
        /// Sets the timer.
        ///
        /// 设置倒计时
        /// </summary>
        /// <param name="host">Host.</param>
        /// <param name="name">Name.</param>
        /// <param name="cd">Cd.</param>
        /// <param name="isave">If set to <c>true</c> isave.</param>
        private void SetTimer(FormulaHost host, string name, int cd, bool isave)
        {
            host.SetDynamicData(SignKeys.NAME, name);
            host.SetRealTimeCountDown(cd);
            host.RemoveDynamicData(SIGN_KEY_TIME_UP);

            if (isave)
            {
                host.Save(new HttpResponseDelegate((bool result) =>
                {
                    this.RunTimer(host);
                }));
            }
            else
            {
                this.RunTimer(host);
            }
        }

        /// <summary>
        /// Runs the timer.
        ///
        /// 运行倒计时
        /// </summary>
        /// <param name="host">Host.</param>
        private void RunTimer(FormulaHost host)
        {
            if (TimerHostController.Instance == null)
            {
                return;
            }

            if (host.GetDynamicIntByKey(SIGN_KEY_TIME_UP, -1) > 0)
            {
                return;
            }

            TimerHostController.Instance.AddTimerHost(host);
        }

        /// <summary>
        /// Sets the account physical timer.
        ///
        /// 设置账号体力计时器
        /// </summary>
        /// <param name="account">Account.</param>
        public void SetAccountPhysicalTimer(bool isave = true)
        {
            FormulaHost apHost = this.GetHostByKeyValue(SignKeys.NAME, AP_TIMER);
            if (apHost == null)
            {
                apHost = FomulaHostManager.Instance.CreateHost(HOST_IDX);
            }

            this.SetTimer(apHost, AP_TIMER, AP_CD, isave);
        }

        public FormulaHost GetAccountPhysicalTimer()
        {
            return this.GetHostByKeyValue(SignKeys.NAME, AP_TIMER);
        }

        public void AccountPhysicalTimeUp(FormulaHost timerHost)
        {
            AccountPhysicsManagerComponent.Instance.ChangePhysical(AP_VALUE);
            this.SetAccountPhysicalTimer(true);
        }
    }
}