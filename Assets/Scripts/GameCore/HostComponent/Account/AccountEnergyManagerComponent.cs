///自定义模块，可定制模块具体行为
using System;
using UnityEngine;

namespace FormulaBase
{
    public class AccountEnergyManagerComponent : CustomComponentBase
    {
        private static AccountEnergyManagerComponent instance = null;
        private const int HOST_IDX = 1;

        public static AccountEnergyManagerComponent Instance
        {
            get { return instance ?? (instance = new AccountEnergyManagerComponent()); }
        }
    }
}