using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Common;
using Assets.Scripts.NewUI;
using FormulaBase;
using UnityEngine;

namespace Assets.Scripts.Tools.Managers
{
    public class CallbackManager : Singleton<CallbackManager>
    {
        public object this[string key]
        {
            get
            {
                if (m_CallbackDic.ContainsKey(key))
                {
                    return m_CallbackDic[key];
                }
                return null;
            }
        }

        public string GetValue(string key, object param = null)
        {
            var callback = this[key] as Func<string>;
            if (callback != null) return callback();
            var callbackWithParam = this[key] as Func<object, string>;
            return callbackWithParam != null ? callbackWithParam(param) : string.Empty;
        }

        public string[] keys
        {
            get { return m_CallbackDic.Keys.ToArray(); }
        }

        private readonly Dictionary<string, object> m_CallbackDic = new Dictionary<string, object>
        {
            {"Account_Level", (new Func<string>(()=>AccountLevelManagerComponent.Instance.GetLvl().ToString()))},
            {"Account_Exp", (new Func<string>(()=>AccountLevelManagerComponent.Instance.GetExp().ToString()))},
            {"Account_Energy", (new Func<string>(()=>AccountPhysicsManagerComponent.Instance.GetPhysical().ToString()))},
            {"Account_MaxEnergy", (new Func<string>(()=>AccountPhysicsManagerComponent.Instance.GetMaxPhysical().ToString()))},

            {"Stage_ClearCount", (new Func<object, string>(idx =>
            {
                var index = 0;
                return int.TryParse(idx.ToString(), out index) ? TaskStageTarget.Instance.GetStageClearCount(index).ToString() : string.Empty;
            }))},

            {"Game_BattleStart", (new Action<object>(param =>
            {
                var index = int.Parse(param.ToString());
                GameMain.instance.BattleStart(index);
            }))},

            {"Game_Login", (new Action<object>(param =>
            {
                var index = int.Parse(param.ToString());
                GameMain.instance.Login(index);
            }))},

            {"UI_ShowPanel", (new Action<object>(param =>
            {
                var pnlName = param.ToString();
                UIManager.instance[pnlName].SetActive(true);
            }))},
        };

        public class FuncParam
        {
            public List<object> paramList = new List<object>();

            public FuncParam(params object[] objs)
            {
                objs.ToList().Add(paramList);
            }
        }
    }
}