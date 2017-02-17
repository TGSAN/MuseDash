using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Common;
using FormulaBase;

namespace Assets.Scripts.Tools.Managers
{
    public class ConstanceManager : Singleton<ConstanceManager>
    {
        public object this[string key]
        {
            get
            {
                if (m_EnumValue.ContainsKey(key))
                {
                    return m_EnumValue[key];
                }
                return null;
            }
        }

        public string[] keys
        {
            get { return m_EnumValue.Keys.ToArray(); }
        }

        private readonly Dictionary<string, object> m_EnumValue = new Dictionary<string, object>
        {
            {"Account_Level", (new Func<string>(()=>AccountLevelManagerComponent.Instance.GetLvl().ToString()))},
            {"Account_Exp", (new Func<string>(()=>AccountLevelManagerComponent.Instance.GetExp().ToString()))},

            {"Stage_ClearCount", (new  Func<object, string>(idx =>
            {
                var index = 0;
                return int.TryParse(idx.ToString(), out index) ? TaskStageTarget.Instance.GetStageClearCount(index).ToString() : string.Empty;
            }))},
        };
    }
}