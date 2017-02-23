using DYUnityLib;
using GameLogic;
using LitJson;

///自定义模块，可定制模块具体行为
using System;
using System.Collections.Generic;
using System.Reflection;
using Assets.Scripts.Tools.Managers;
using UnityEngine;

namespace FormulaBase
{
    public class SkillEffectComponent : CustomComponentBase
    {
        private static SkillEffectComponent instance = null;
        private const int HOST_IDX = 12;

        public static SkillEffectComponent Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new SkillEffectComponent();
                }
                return instance;
            }
        }

        // ------------------------------------------------------------------------------------
        private const string SIGN_KEY_METHOD = "SIGN_KEY_METHOD";

        private const string EFFECT_SIGN = "EFFECT_SIGN";
        private const string CONDICTION_VALUE = "CONDICTION_VALUE";
        private const string EFFECT_VALUE = "EFFECT_VALUE";
        private static System.Reflection.Assembly assembly = System.Reflection.Assembly.Load("Assembly-CSharp");

        private List<FixUpdateTimer> timers = null;
        private Dictionary<int, FormulaHost> skillEffects = null;

        public void Init()
        {
            if (this.skillEffects != null && this.skillEffects.Count > 0)
            {
                return;
            }

            var config = ConfigManager.instance["skilleffect"];
            if (config == null || config.Count <= 0)
            {
                return;
            }

            this.skillEffects = new Dictionary<int, FormulaHost>();
            Type tpe = assembly.GetType("FormulaBase.SkillEffectComponent");

            for (int i = 0; i < config.Count; i++)
            {
                var _cfg = config[i];
                FormulaHost host = FomulaHostManager.Instance.CreateHost(HOST_IDX);

                if (_cfg.Keys.Contains("action"))
                {
                    string methodName = _cfg["action"].ToString();
                    MethodInfo method = tpe.GetMethod(methodName);
                    if (method == null)
                    {
                        Debug.Log("Skill effect init with method " + methodName + " not defined.");
                    }

                    host.SetDynamicData(SIGN_KEY_METHOD, method);
                }

                if (_cfg.Keys.Contains("sign") && _cfg["sign"] != null)
                {
                    string signName = _cfg["sign"].ToString();
                    host.SetDynamicData(EFFECT_SIGN, signName);
                }

                if (_cfg.Keys.Contains("condvalue") && _cfg["condvalue"] != null)
                {
                    float value = float.Parse(_cfg["condvalue"].ToString());
                    host.SetDynamicData(CONDICTION_VALUE, value);
                }

                if (_cfg.Keys.Contains("value") && _cfg["value"] != null)
                {
                    float value = float.Parse(_cfg["value"].ToString());
                    host.SetDynamicData(EFFECT_VALUE, value);
                }

                host.SetDynamicData(SignKeys.ID, i);
                this.skillEffects[i] = host;
            }

            this.timers = new List<FixUpdateTimer>();
        }

        public void DoSkillEffect(FormulaHost skillObject, int idx)
        {
            if (this.skillEffects == null || !this.skillEffects.ContainsKey(idx))
            {
                return;
            }

            FormulaHost host = this.skillEffects[idx];
            MethodInfo method = (MethodInfo)host.GetDynamicObjByKey(SIGN_KEY_METHOD);
            if (method == null)
            {
                return;
            }

            string signKey = host.GetDynamicStrByKey(EFFECT_SIGN);
            float condValue = host.GetDynamicDataByKey(CONDICTION_VALUE);
            float value = host.GetDynamicDataByKey(EFFECT_VALUE);
            skillObject.SetDynamicData(SkillComponent.SIGN_KEY_SKILL_FAIL, 0);
            method.Invoke(this, new object[] { idx, skillObject, signKey, condValue, value });
        }

        private FormulaHost HostCheck(FormulaHost skillObject)
        {
            if (skillObject == null)
            {
                return null;
            }

            FormulaHost host = (FormulaHost)skillObject.GetDynamicObjByKey(SkillComponent.SIGN_KEY_SKILLPARENT);
            if (host == null)
            {
                return null;
            }

            return host;
        }

        // ----------------------------  Skill effect methods   ---------------------------------------------
        /// <summary>
        /// Add/Set the sign value.
        ///
        /// 增加属性值
        /// </summary>
        /// <param name="skillObject">Skill object.</param>
        /// <param name="signKey">Sign key.</param>
        /// <param name="condValue">Cond value.</param>
        /// <param name="value">Value.</param>
        public void AddSignValue(int effectId, FormulaHost skillObject, string signKey, float condValue, float value)
        {
            if (signKey == null || signKey.Length < GameGlobal.SIGN_KEY_MIN_LEN)
            {
                return;
            }

            FormulaHost host = this.HostCheck(skillObject);
            if (host == null)
            {
                return;
            }

            float low = 0;
            float _val = host.GetDynamicDataByKey(signKey);
            _val += value;
            if (_val < low)
            {
                _val = low;
            }

            host.SetDynamicData(signKey, _val);
        }

        public void SetSignValue(int effectId, FormulaHost skillObject, string signKey, float condValue, float value)
        {
            if (signKey == null || signKey.Length < GameGlobal.SIGN_KEY_MIN_LEN)
            {
                return;
            }

            FormulaHost host = this.HostCheck(skillObject);
            if (host == null)
            {
                return;
            }

            host.SetDynamicData(signKey, value);
        }

        /// <summary>
        /// Removes the sign value.
        ///
        /// 移除属性值
        /// </summary>
        /// <param name="skillObject">Skill object.</param>
        /// <param name="signKey">Sign key.</param>
        /// <param name="condValue">Cond value.</param>
        /// <param name="value">Value.</param>
        public void RemoveSignValue(int effectId, FormulaHost skillObject, string signKey, float condValue, float value)
        {
            if (signKey == null || signKey.Length < GameGlobal.SIGN_KEY_MIN_LEN)
            {
                return;
            }

            FormulaHost host = this.HostCheck(skillObject);
            if (host == null)
            {
                return;
            }

            float _val = host.GetDynamicDataByKey(signKey);
            host.SetDynamicData(signKey, _val - value);
        }

        /// <summary>
        /// Sets/Add the switch sign value.
        ///
        /// 对基于技能效果的开关（统计）进行设置，value是加成值
        /// </summary>
        /// <param name="effectId">Effect identifier.</param>
        /// <param name="skillObject">Skill object.</param>
        /// <param name="signKey">Sign key.</param>
        /// <param name="condValue">Cond value.</param>
        /// <param name="value">Value.</param>
        public void SetSwitchSignValue(int effectId, FormulaHost skillObject, string signKey, float condValue, float value)
        {
            string _signkey = SkillComponent.SING_KEY_SWITCHS + signKey;
            Dictionary<int, float> switchs = (Dictionary<int, float>)skillObject.GetDynamicObjByKey(_signkey);
            if (switchs == null)
            {
                return;
            }

            int skId = skillObject.GetDynamicIntByKey(SignKeys.ID);
            switchs[skId] = value;
            skillObject.SetDynamicData(_signkey, switchs);
        }

        public void AddSwitchSignValue(int effectId, FormulaHost skillObject, string signKey, float condValue, float value)
        {
            string _signkey = SkillComponent.SING_KEY_SWITCHS + signKey;
            Dictionary<int, float> switchs = (Dictionary<int, float>)skillObject.GetDynamicObjByKey(_signkey);
            if (switchs == null)
            {
                return;
            }

            int skId = skillObject.GetDynamicIntByKey(SignKeys.ID);
            if (switchs.ContainsKey(skId))
            {
                switchs[skId] += value;
            }
            else
            {
                switchs[skId] = value;
            }

            skillObject.SetDynamicData(_signkey, switchs);
        }

        public void ReduceSwitchSignValue(int effectId, FormulaHost skillObject, string signKey, float condValue, float value)
        {
            string _signkey = SkillComponent.SING_KEY_SWITCHS + signKey;
            Dictionary<int, float> switchs = (Dictionary<int, float>)skillObject.GetDynamicObjByKey(_signkey);
            if (switchs == null)
            {
                return;
            }

            int skId = skillObject.GetDynamicIntByKey(SignKeys.ID);
            if (switchs.ContainsKey(skId))
            {
                switchs[skId] -= value;
            }
            else
            {
                switchs[skId] = -value;
            }

            skillObject.SetDynamicData(_signkey, switchs);
        }

        public void RemoveSwitchSignValue(int effectId, FormulaHost skillObject, string signKey, float condValue, float value)
        {
            string _signkey = SkillComponent.SING_KEY_SWITCHS + signKey;
            Dictionary<int, float> switchs = (Dictionary<int, float>)skillObject.GetDynamicObjByKey(_signkey);
            if (switchs == null)
            {
                return;
            }

            int skId = skillObject.GetDynamicIntByKey(SignKeys.ID);
            if (!switchs.ContainsKey(skId))
            {
                return;
            }

            switchs.Remove(skId);
        }

        /// <summary>
        /// Condictions the over.
        ///
        /// 大于某个条件值增加属性值，小于则移除
        /// </summary>
        /// <param name="skillObject">Skill object.</param>
        /// <param name="signKey">Sign key.</param>
        /// <param name="hostValue">Host value.</param>
        /// <param name="condValue">Cond value.</param>
        /// <param name="value">Value.</param>
        private bool CondictionOver(int effectId, FormulaHost skillObject, string signKey, float hostValue, float condValue, float value)
        {
            if (hostValue < condValue)
            {
                skillObject.SetDynamicData(SkillComponent.SIGN_KEY_SKILL_FAIL, 1);
                this.RemoveSignValue(effectId, skillObject, signKey, condValue, value);
                return false;
            }

            this.AddSignValue(effectId, skillObject, signKey, condValue, value);
            return true;
        }

        /// <summary>
        /// Condictions the less.
        ///
        /// 小于某个条件值增加属性值，大于则移除
        /// </summary>
        /// <param name="skillObject">Skill object.</param>
        /// <param name="signKey">Sign key.</param>
        /// <param name="hostValue">Host value.</param>
        /// <param name="condValue">Cond value.</param>
        /// <param name="value">Value.</param>
        private bool CondictionLess(int effectId, FormulaHost skillObject, string signKey, float hostValue, float condValue, float value)
        {
            if (hostValue > condValue)
            {
                skillObject.SetDynamicData(SkillComponent.SIGN_KEY_SKILL_FAIL, 1);
                this.RemoveSignValue(effectId, skillObject, signKey, condValue, value);
                return false;
            }

            this.AddSignValue(effectId, skillObject, signKey, condValue, value);
            return true;
        }

        /// <summary>
        /// Condictions the match.
        ///
        /// 为某值整数倍增加属性值，否则移除
        /// </summary>
        /// <param name="skillObject">Skill object.</param>
        /// <param name="signKey">Sign key.</param>
        /// <param name="hostValue">Host value.</param>
        /// <param name="condValue">Cond value.</param>
        /// <param name="value">Value.</param>
        private bool CondictionMatch(int effectId, FormulaHost skillObject, string signKey, float hostValue, float condValue, float value)
        {
            int iHostValue = (int)hostValue;
            int iCondValue = (int)condValue;
            if (iHostValue == iCondValue || (iHostValue % iCondValue) == 0)
            {
                this.AddSignValue(effectId, skillObject, signKey, condValue, value);
                skillObject.SetDynamicData(SkillComponent.SIGN_KEY_SKILL_FAIL, 0);
                return true;
            }

            skillObject.SetDynamicData(SkillComponent.SIGN_KEY_SKILL_FAIL, 1);
            this.RemoveSignValue(effectId, skillObject, signKey, condValue, value);
            return false;
        }

        /// <summary>
        /// Condictions the record.
        ///
        /// 条件计数器
        /// </summary>
        /// <param name="skillObject">Skill object.</param>
        /// <param name="signKey">Sign key.</param>
        /// <param name="hostValue">Host value.</param>
        /// <param name="condValue">Cond value.</param>
        /// <param name="value">Value.</param>
        private int CondictionRecord(int effectId, FormulaHost skillObject, string condSignKey, int value)
        {
            int _rvalue = skillObject.GetDynamicIntByKey(condSignKey) + value;
            skillObject.SetDynamicData(condSignKey, _rvalue);
            return _rvalue;
        }

        /// <summary>
        /// Condictions the record over.
        ///
        /// 条件计数器，大于触发效果
        /// </summary>
        /// <param name="skillObject">Skill object.</param>
        /// <param name="signKey">Sign key.</param>
        /// <param name="recordKey">Record key.</param>
        /// <param name="condValue">Cond value.</param>
        /// <param name="value">Value.</param>
        /// <param name="addVal">Add value.</param>
        private bool CondictionRecordOver(int effectId, FormulaHost skillObject, string signKey, string recordKey, float condValue, float value, int addVal)
        {
            FormulaHost host = this.HostCheck(skillObject);
            if (host == null)
            {
                return false;
            }

            int _condVal = this.CondictionRecord(effectId, skillObject, recordKey, addVal);
            return this.CondictionOver(effectId, skillObject, signKey, _condVal, condValue, value);
        }

        /// <summary>
        /// Condictions the record match.
        ///
        /// 条件计数器，倍数触发效果
        /// </summary>
        /// <param name="skillObject">Skill object.</param>
        /// <param name="signKey">Sign key.</param>
        /// <param name="recordKey">Record key.</param>
        /// <param name="condValue">Cond value.</param>
        /// <param name="value">Value.</param>
        /// <param name="addVal">Add value.</param>
        private bool CondictionRecordMatch(int effectId, FormulaHost skillObject, string signKey, string recordKey, float condValue, float value, int addVal)
        {
            FormulaHost host = this.HostCheck(skillObject);
            if (host == null)
            {
                return false;
            }

            int _condVal = this.CondictionRecord(effectId, skillObject, recordKey, addVal);
            return this.CondictionMatch(effectId, skillObject, signKey, _condVal, condValue, value);
        }

        /// <summary>
        /// Hit node by type.
        ///
        /// 恶灵、机械等怪物类型
        /// 吃到音符、金币、血瓶等物品时
        /// node表的node大分类表示物品类型
        /// condValue : 物品类型
        /// </summary>
        /// <param name="skillObject">Skill object.</param>
        /// <param name="signKey">Sign key.</param>
        /// <param name="condValue">Cond value.</param>
        /// <param name="value">Value.</param>
        private bool IsNodeType(FormulaHost skillObject, string signKey, float condValue, float value)
        {
            int idx = BattleEnemyManager.Instance.GetCurrentPlayIdx();
            MusicData md = StageBattleComponent.Instance.GetMusicDataByIdx(idx);
            return md.nodeData.type == (uint)condValue;
        }

        // -------------------------   Direct add value, condValue mostly is rate   -------------------------
        public void AddHp(int effectId, FormulaHost skillObject, string signKey, float condValue, float value)
        {
            if (UnityEngine.Random.Range(0, 1f) > condValue)
            {
                skillObject.SetDynamicData(SkillComponent.SIGN_KEY_SKILL_FAIL, 1);
                return;
            }

            BattleRoleAttributeComponent.Instance.AddHp((int)value);
        }

        public void AddHpPercent(int effectId, FormulaHost skillObject, string signKey, float condValue, float value)
        {
            if (UnityEngine.Random.Range(0, 1f) > condValue)
            {
                skillObject.SetDynamicData(SkillComponent.SIGN_KEY_SKILL_FAIL, 1);
                return;
            }

            float _val = BattleRoleAttributeComponent.Instance.GetHp() * value;
            BattleRoleAttributeComponent.Instance.AddHp((int)_val);
        }

        public void AddScore(int effectId, FormulaHost skillObject, string signKey, float condValue, float value)
        {
            if (UnityEngine.Random.Range(0, 1f) > condValue)
            {
                skillObject.SetDynamicData(SkillComponent.SIGN_KEY_SKILL_FAIL, 1);
                return;
            }

            TaskStageTarget.Instance.AddScore((int)value);

            FormulaHost host = this.HostCheck(skillObject);
            if (host != null)
            {
                host.AddDynamicValue(BattleRoleAttributeComponent.SK_TEMP_ADD_SCORE, value);
            }
        }

        public void AddGold(int effectId, FormulaHost skillObject, string signKey, float condValue, float value)
        {
            if (UnityEngine.Random.Range(0, 1f) > condValue)
            {
                skillObject.SetDynamicData(SkillComponent.SIGN_KEY_SKILL_FAIL, 1);
                return;
            }

            StageBattleComponent.Instance.AddGold((int)value);
        }

        public void AddCombo(int effectId, FormulaHost skillObject, string signKey, float condValue, float value)
        {
            if (UnityEngine.Random.Range(0, 1f) > condValue)
            {
                skillObject.SetDynamicData(SkillComponent.SIGN_KEY_SKILL_FAIL, 1);
                return;
            }

            int combo = StageBattleComponent.Instance.GetCombo();
            StageBattleComponent.Instance.SetCombo(combo + (int)value);
        }

        public void AddFever(int effectId, FormulaHost skillObject, string signKey, float condValue, float value)
        {
            if (UnityEngine.Random.Range(0, 1f) > condValue)
            {
                skillObject.SetDynamicData(SkillComponent.SIGN_KEY_SKILL_FAIL, 1);
                return;
            }

            GameKernel.Instance.AddFever(value);
            FightMenuPanel.Instance.SetFerver(GameKernel.Instance.GetFeverRate());
        }

        // ---------------------------  Complex condiction ------------------------------
        /// <summary>
        /// Combos the over.
        ///
        /// combo超过xx时触发属性特效
        /// </summary>
        /// <param name="skillObject">Skill object.</param>
        /// <param name="signKey">Sign key.</param>
        /// <param name="condValue">Cond value.</param>
        /// <param name="value">Value.</param>
        public void ComboOver(int effectId, FormulaHost skillObject, string signKey, float condValue, float value)
        {
            FormulaHost host = this.HostCheck(skillObject);
            if (host == null)
            {
                return;
            }

            int _hostValue = host.GetDynamicIntByKey(SignKeys.COMBO);
            this.CondictionOver(effectId, skillObject, signKey, _hostValue, condValue, value);
        }

        /// <summary>
        /// Combos the match.
        ///
        /// combo为xx整数倍时触发属性特效
        /// </summary>
        /// <param name="skillObject">Skill object.</param>
        /// <param name="signKey">Sign key.</param>
        /// <param name="condValue">Cond value.</param>
        /// <param name="value">Value.</param>
        public void ComboMatch(int effectId, FormulaHost skillObject, string signKey, float condValue, float value)
        {
            FormulaHost host = this.HostCheck(skillObject);
            if (host == null)
            {
                return;
            }

            int _hostValue = host.GetDynamicIntByKey(SignKeys.COMBO);
            this.CondictionMatch(effectId, skillObject, signKey, _hostValue, condValue, value);
        }

        private const string SKE_CT_PERFECT = "SKE_CT_PERFECT";

        /// <summary>
        /// Perfects the over.
        ///
        /// perfect计数触发效果
        /// </summary>
        /// <param name="skillObject">Skill object.</param>
        /// <param name="signKey">Sign key.</param>
        /// <param name="condValue">Cond value.</param>
        /// <param name="value">Value.</param>
        public void PerfectOver(int effectId, FormulaHost skillObject, string signKey, float condValue, float value)
        {
            int addValue = 1;
            this.CondictionRecordOver(effectId, skillObject, signKey, SKE_CT_PERFECT, condValue, value, addValue);
        }

        public void PerfectMatch(int effectId, FormulaHost skillObject, string signKey, float condValue, float value)
        {
            int addValue = 1;
            this.CondictionRecordMatch(effectId, skillObject, signKey, SKE_CT_PERFECT, condValue, value, addValue);
        }

        private const string SKE_CT_HIT_EMPTY = "SKE_CT_HIT_EMPTY";

        /// <summary>
        /// Perfects the over.
        ///
        /// 空挥计数触发效果
        /// </summary>
        /// <param name="skillObject">Skill object.</param>
        /// <param name="signKey">Sign key.</param>
        /// <param name="condValue">Cond value.</param>
        /// <param name="value">Value.</param>
        public void HitEmptyOver(int effectId, FormulaHost skillObject, string signKey, float condValue, float value)
        {
            int addValue = 1;
            this.CondictionRecordOver(effectId, skillObject, signKey, SKE_CT_HIT_EMPTY, condValue, value, addValue);
        }

        public void HitEmptyMatch(int effectId, FormulaHost skillObject, string signKey, float condValue, float value)
        {
            int addValue = 1;
            this.CondictionRecordMatch(effectId, skillObject, signKey, SKE_CT_HIT_EMPTY, condValue, value, addValue);
        }

        private const string SKE_CT_JUMP_OVER = "SKE_CT_JUMP_OVER";

        /// <summary>
        /// Perfects the over.
        ///
        /// 跳跃计数触发效果
        /// </summary>
        /// <param name="skillObject">Skill object.</param>
        /// <param name="signKey">Sign key.</param>
        /// <param name="condValue">Cond value.</param>
        /// <param name="value">Value.</param>
        public void JumpOverOver(int effectId, FormulaHost skillObject, string signKey, float condValue, float value)
        {
            int addValue = 1;
            this.CondictionRecordOver(effectId, skillObject, signKey, SKE_CT_JUMP_OVER, condValue, value, addValue);
        }

        public void JumpOverMatch(int effectId, FormulaHost skillObject, string signKey, float condValue, float value)
        {
            int addValue = 1;
            this.CondictionRecordMatch(effectId, skillObject, signKey, SKE_CT_JUMP_OVER, condValue, value, addValue);
        }

        public void JumpOverMatchAddHp(int effectId, FormulaHost skillObject, string signKey, float condValue, float value)
        {
            int addValue = 1;
            if (!this.CondictionRecordMatch(effectId, skillObject, signKey, SKE_CT_JUMP_OVER, condValue, value, addValue))
            {
                return;
            }
            this.AddHp(effectId, skillObject, signKey, condValue, value);
        }

        private const string SKE_CT_CRITICAL = "SKE_CT_PERFECT";

        /// <summary>
        /// Critcals the over.
        ///
        /// Critcals计数触发效果
        /// </summary>
        /// <param name="skillObject">Skill object.</param>
        /// <param name="signKey">Sign key.</param>
        /// <param name="condValue">Cond value.</param>
        /// <param name="value">Value.</param>
        public void CritcalOver(int effectId, FormulaHost skillObject, string signKey, float condValue, float value)
        {
            int addValue = 1;
            this.CondictionRecordOver(effectId, skillObject, signKey, SKE_CT_CRITICAL, condValue, value, addValue);
        }

        public void CritcalMatch(int effectId, FormulaHost skillObject, string signKey, float condValue, float value)
        {
            int addValue = 1;
            this.CondictionRecordMatch(effectId, skillObject, signKey, SKE_CT_CRITICAL, condValue, value, addValue);
        }

        /// <summary>
        /// Adds the type of the gold by node.
        ///
        /// 根据类型加金币
        /// </summary>
        /// <param name="skillObject">Skill object.</param>
        /// <param name="signKey">Sign key.</param>
        /// <param name="condValue">Cond value.</param>
        /// <param name="value">Value.</param>
        public void AddGoldByNodeType(int effectId, FormulaHost skillObject, string signKey, float condValue, float value)
        {
            if (!this.IsNodeType(skillObject, signKey, condValue, value))
            {
                return;
            }

            this.AddGold(effectId, skillObject, signKey, condValue, value);
        }

        /// <summary>
        /// Adds the type of the score by node.
        ///
        /// 根据类型加分数
        /// </summary>
        /// <param name="skillObject">Skill object.</param>
        /// <param name="signKey">Sign key.</param>
        /// <param name="condValue">Cond value.</param>
        /// <param name="value">Value.</param>
        public void AddScoreByNodeType(int effectId, FormulaHost skillObject, string signKey, float condValue, float value)
        {
            if (!this.IsNodeType(skillObject, signKey, condValue, value))
            {
                return;
            }

            this.AddScore(effectId, skillObject, signKey, condValue, value);
            CharPanel.Instance.SetHpScore((int)value);
        }

        /// <summary>
        /// Adds the hp timer step trigger.
        ///
        /// 定时恢复hp
        /// </summary>
        /// <param name="sender">Sender.</param>
        /// <param name="triggerId">Trigger identifier.</param>
        /// <param name="args">Arguments.</param>
        public void AddHpOnTimer(int effectId, FormulaHost skillObject, string signKey, float condValue, float value)
        {
            int _total = 300;
            FixUpdateTimer _timer = new FixUpdateTimer();
            _timer.Init(_total, FixUpdateTimer.TIMER_TYPE_EVENT_ARRAY);
            int _interval = (int)condValue;
            for (int i = 0; i < _total / _interval; i++)
            {
                _timer.AddTickEvent((decimal)(i * _interval), GameGlobal.SKILL_EVENT_ADD_HP);
            }

            EventTrigger stPress = gTrigger.RegEvent(GameGlobal.SKILL_EVENT_ADD_HP);
            stPress.Trigger += (object sender, uint triggerId, object[] args) =>
            {
                this.AddHp(effectId, skillObject, signKey, condValue, value);
            };

            this.timers.Add(_timer);
            _timer.Run();
        }

        /// <summary>
        /// Sets the type of the switch sign by node.
        ///
        /// 根据node类型设置开关值
        /// </summary>
        /// <param name="effectId">Effect identifier.</param>
        /// <param name="skillObject">Skill object.</param>
        /// <param name="signKey">Sign key.</param>
        /// <param name="condValue">Cond value.</param>
        /// <param name="value">Value.</param>
        public void SetSwitchSignByNodeType(int effectId, FormulaHost skillObject, string signKey, float condValue, float value)
        {
            if (!this.IsNodeType(skillObject, signKey, condValue, value))
            {
                this.SetSwitchSignValue(effectId, skillObject, signKey, condValue, 0);
                return;
            }

            this.SetSwitchSignValue(effectId, skillObject, signKey, condValue, value);
        }

        public void AddSwitchSignByNodeType(int effectId, FormulaHost skillObject, string signKey, float condValue, float value)
        {
            if (!this.IsNodeType(skillObject, signKey, condValue, value))
            {
                // this.SetSwitchSignValue (effectId, skillObject, signKey, condValue, 0);
                return;
            }

            this.AddSwitchSignValue(effectId, skillObject, signKey, condValue, value);
        }

        public void SetSwitchSignByScene(int effectId, FormulaHost skillObject, string signKey, float condValue, float value)
        {
            if ((int)condValue != StageBattleComponent.Instance.GetStageId())
            {
                this.SetSwitchSignValue(effectId, skillObject, signKey, condValue, 0);
                return;
            }

            this.SetSwitchSignValue(effectId, skillObject, signKey, condValue, value);
        }

        /// <summary>
        /// Sets the switch sign by hp over.
        ///
        /// 根据hp%设置开关值
        /// </summary>
        /// <param name="effectId">Effect identifier.</param>
        /// <param name="skillObject">Skill object.</param>
        /// <param name="signKey">Sign key.</param>
        /// <param name="condValue">Cond value.</param>
        /// <param name="value">Value.</param>
        public void SetSwitchSignByHpOver(int effectId, FormulaHost skillObject, string signKey, float condValue, float value)
        {
            FormulaHost host = this.HostCheck(skillObject);
            if (host == null)
            {
                return;
            }

            // int _hostValue = host.GetDynamicIntByKey (SignKeys.BATTLE_HP);
            float _hpRate = BattleRoleAttributeComponent.Instance.HpRate(host);
            if (this.CondictionOver(effectId, skillObject, null, _hpRate, condValue, value))
            {
                this.SetSwitchSignValue(effectId, skillObject, signKey, condValue, value);
            }
            else
            {
                this.SetSwitchSignValue(effectId, skillObject, signKey, condValue, 0);
            }
        }

        public void SetSwitchSignByHpLess(int effectId, FormulaHost skillObject, string signKey, float condValue, float value)
        {
            FormulaHost host = this.HostCheck(skillObject);
            if (host == null)
            {
                return;
            }

            // int _hostValue = host.GetDynamicIntByKey (SignKeys.BATTLE_HP);
            float _hpRate = BattleRoleAttributeComponent.Instance.HpRate(host);
            if (this.CondictionLess(effectId, skillObject, null, _hpRate, condValue, value))
            {
                this.SetSwitchSignValue(effectId, skillObject, signKey, condValue, value);
            }
            else
            {
                this.SetSwitchSignValue(effectId, skillObject, signKey, condValue, 0);
            }
        }

        /// <summary>
        /// Sets the switch sign by combo over.
        ///
        /// 根据combo设置开关值
        /// </summary>
        /// <param name="effectId">Effect identifier.</param>
        /// <param name="skillObject">Skill object.</param>
        /// <param name="signKey">Sign key.</param>
        /// <param name="condValue">Cond value.</param>
        /// <param name="value">Value.</param>
        public void SetSwitchSignByComboOver(int effectId, FormulaHost skillObject, string signKey, float condValue, float value)
        {
            FormulaHost host = this.HostCheck(skillObject);
            if (host == null)
            {
                return;
            }

            int _hostValue = host.GetDynamicIntByKey(SignKeys.COMBO);
            if (this.CondictionOver(effectId, skillObject, null, _hostValue, condValue, value))
            {
                this.SetSwitchSignValue(effectId, skillObject, signKey, condValue, value);
            }
            else
            {
                this.SetSwitchSignValue(effectId, skillObject, signKey, condValue, 0);
            }
        }

        public void SetSwitchSignByComboMatch(int effectId, FormulaHost skillObject, string signKey, float condValue, float value)
        {
            FormulaHost host = this.HostCheck(skillObject);
            if (host == null)
            {
                return;
            }

            int _hostValue = host.GetDynamicIntByKey(SignKeys.COMBO);
            this.SetSwitchSignByMatch(effectId, skillObject, signKey, condValue, value);
        }

        /// <summary>
        /// Adds the switch sign by match.
        ///
        /// 根据统计值设置开关值
        /// </summary>
        /// <param name="effectId">Effect identifier.</param>
        /// <param name="skillObject">Skill object.</param>
        /// <param name="signKey">Sign key.</param>
        /// <param name="condValue">Cond value.</param>
        /// <param name="value">Value.</param>
        public void SetSwitchSignByMatch(int effectId, FormulaHost skillObject, string signKey, float condValue, float value)
        {
            int addValue = 1;
            if (this.CondictionRecordMatch(effectId, skillObject, signKey, SKE_CT_PERFECT, condValue, value, addValue))
            {
                this.SetSwitchSignValue(effectId, skillObject, signKey, condValue, value);
            }
        }

        public void AddSwitchSignByMatch(int effectId, FormulaHost skillObject, string signKey, float condValue, float value)
        {
            int addValue = 1;
            if (this.CondictionRecordMatch(effectId, skillObject, signKey, SKE_CT_PERFECT, condValue, value, addValue))
            {
                this.AddSwitchSignValue(effectId, skillObject, signKey, condValue, value);
            }
        }

        /// <summary>
        /// Adds the type of the sign value by node.
        ///
        /// 通过node类型增加sign值
        /// </summary>
        /// <param name="effectId">Effect identifier.</param>
        /// <param name="skillObject">Skill object.</param>
        /// <param name="signKey">Sign key.</param>
        /// <param name="condValue">Cond value.</param>
        /// <param name="value">Value.</param>
        public void AddSignValueByNodeType(int effectId, FormulaHost skillObject, string signKey, float condValue, float value)
        {
            if (!this.IsNodeType(skillObject, signKey, condValue, value))
            {
                return;
            }

            this.AddSignValue(effectId, skillObject, signKey, condValue, value);
        }

        /// <summary>
        /// Attribute(defence/hp) to attack.
        ///
        /// %防御力/hp转化为攻击力
        /// </summary>
        /// <param name="effectId">Effect identifier.</param>
        /// <param name="skillObject">Skill object.</param>
        /// <param name="signKey">Sign key.</param>
        /// <param name="condValue">Cond value.</param>
        /// <param name="value">Value.</param>
        public void AttrToAttack(int effectId, FormulaHost skillObject, string signKey, float condValue, float value)
        {
            FormulaHost host = this.HostCheck(skillObject);
            if (host == null)
            {
                this.RemoveSwitchSignValue(effectId, skillObject, signKey, condValue, value);
                return;
            }

            int attr = host.GetDynamicIntByKey(signKey);
            int atkup = (int)(attr * value);
            this.SetSwitchSignValue(effectId, skillObject, signKey, condValue, atkup);
        }

        /// <summary>
        /// Uses the skill.
        ///
        /// 使用技能
        /// </summary>
        /// <param name="effectId">Effect identifier.</param>
        /// <param name="skillObject">Skill object.</param>
        /// <param name="signKey">Sign key.</param>
        /// <param name="condValue">Cond value.</param>
        /// <param name="value">Value.</param>
        public void UseSkill(int effectId, FormulaHost skillObject, string signKey, float condValue, float value)
        {
            FormulaHost host = this.HostCheck(skillObject);
            if (host == null)
            {
                return;
            }

            FormulaHost _skObj = SkillComponent.Instance.CreateSkill((int)value, host);
            if (_skObj != null)
            {
                SkillComponent.Instance.FireSkill(_skObj, SkillComponent.ON_START);
                Debug.Log("Use skill " + value + " by skill " + skillObject.GetDynamicIntByKey(SignKeys.ID));
            }
        }

        /// <summary>
        /// Creates the shield.
        ///
        /// 制造抵挡一次伤害的护盾
        /// </summary>
        /// <param name="effectId">Effect identifier.</param>
        /// <param name="skillObject">Skill object.</param>
        /// <param name="signKey">Sign key.</param>
        /// <param name="condValue">Cond value.</param>
        /// <param name="value">Value.</param>
        public void AddShield(int effectId, FormulaHost skillObject, string signKey, float condValue, float value)
        {
            FormulaHost host = this.HostCheck(skillObject);
            if (host == null)
            {
                // this.RemoveSwitchSignValue (effectId, skillObject, signKey, condValue, value);
                return;
            }

            List<FormulaHost> skObjShield = (List<FormulaHost>)host.GetDynamicObjByKey(SkillComponent.SIGN_KEY_SKILL_SHIELD);
            if (skObjShield == null)
            {
                skObjShield = new List<FormulaHost>();
            }

            if (skObjShield.Contains(skillObject))
            {
                return;
            }

            skObjShield.Add(skillObject);
            host.SetDynamicData(SkillComponent.SIGN_KEY_SKILL_SHIELD, skObjShield);
        }

        /// <summary>
        /// Removes the shield.
        ///
        /// 移除护盾
        /// </summary>
        /// <param name="effectId">Effect identifier.</param>
        /// <param name="skillObject">Skill object.</param>
        /// <param name="signKey">Sign key.</param>
        /// <param name="condValue">Cond value.</param>
        /// <param name="value">Value.</param>
        public void RemoveShield(int effectId, FormulaHost skillObject, string signKey, float condValue, float value)
        {
            FormulaHost host = this.HostCheck(skillObject);
            if (host == null)
            {
                return;
            }

            List<FormulaHost> skObjShield = (List<FormulaHost>)host.GetDynamicObjByKey(SkillComponent.SIGN_KEY_SKILL_SHIELD);
            if (skObjShield == null)
            {
                return;
            }

            if (!skObjShield.Contains(skillObject))
            {
                return;
            }

            SkillComponent.Instance.RemoveSkill(skillObject);
            skObjShield.Remove(skillObject);
            host.SetDynamicData(SkillComponent.SIGN_KEY_SKILL_SHIELD, skObjShield);
        }

        /// <summary>
        /// Adds the hp max.
        ///
        /// 总生命值增加x%
        /// </summary>
        /// <param name="effectId">Effect identifier.</param>
        /// <param name="skillObject">Skill object.</param>
        /// <param name="signKey">Sign key.</param>
        /// <param name="condValue">Cond value.</param>
        /// <param name="value">Value.</param>
        public void AddHpMax(int effectId, FormulaHost skillObject, string signKey, float condValue, float value)
        {
            FormulaHost host = this.HostCheck(skillObject);
            if (host == null)
            {
                return;
            }

            int addUp = (int)(BattleRoleAttributeComponent.Instance.GetHpMax() * value);
            this.AddSignValue(effectId, skillObject, SignKeys.BATTLE_ADD_UP_HPMAX, condValue, addUp);
        }

        /// <summary>
        /// Adds the hp max by no arm.
        ///
        /// 不携带宠物总生命值增加x%
        /// </summary>
        /// <param name="effectId">Effect identifier.</param>
        /// <param name="skillObject">Skill object.</param>
        /// <param name="signKey">Sign key.</param>
        /// <param name="condValue">Cond value.</param>
        /// <param name="value">Value.</param>
        public void AddHpMaxByNoArm(int effectId, FormulaHost skillObject, string signKey, float condValue, float value)
        {
            FormulaHost host = this.HostCheck(skillObject);
            if (host == null)
            {
                return;
            }

            if (BattlePetComponent.Instance.Count() > 0)
            {
                return;
            }

            this.AddHpMax(effectId, skillObject, signKey, condValue, value);
        }

        /// <summary>
        /// Adds the hp by hp leave rate.
        ///
        /// 根据hp余量回血
        /// </summary>
        /// <param name="effectId">Effect identifier.</param>
        /// <param name="skillObject">Skill object.</param>
        /// <param name="signKey">Sign key.</param>
        /// <param name="condValue">Cond value.</param>
        /// <param name="value">Value.</param>
        public void AddHpByHpLeaveRate(int effectId, FormulaHost skillObject, string signKey, float condValue, float value)
        {
            FormulaHost host = this.HostCheck(skillObject);
            if (host == null)
            {
                return;
            }

            if (BattleRoleAttributeComponent.Instance.HpRate() > condValue)
            {
                return;
            }

            this.AddHp(effectId, skillObject, signKey, 1, value);
        }

        /// <summary>
        /// Uses the type of the skill by node.
        ///
        /// 根据node类型使用技能
        /// </summary>
        /// <param name="effectId">Effect identifier.</param>
        /// <param name="skillObject">Skill object.</param>
        /// <param name="signKey">Sign key.</param>
        /// <param name="condValue">Cond value.</param>
        /// <param name="value">Value.</param>
        public void UseSkillByNodeType(int effectId, FormulaHost skillObject, string signKey, float condValue, float value)
        {
            if (!this.IsNodeType(skillObject, signKey, condValue, value))
            {
                return;
            }

            this.UseSkill(effectId, skillObject, signKey, condValue, value);
        }

        /// <summary>
        /// Changes the type of the node by node.
        ///
        /// 根据node类型转换为指定node
        /// </summary>
        /// <param name="effectId">Effect identifier.</param>
        /// <param name="skillObject">Skill object.</param>
        /// <param name="signKey">Sign key.</param>
        /// <param name="condValue">Cond value.</param>
        /// <param name="value">Value.</param>
        public void ChangeNodeByNodeType(int effectId, FormulaHost skillObject, string signKey, float condValue, float value)
        {
            List<int> enemies = BattleEnemyManager.Instance.GetCurrentSceneAliveObjectsIndexes();
            if (enemies == null || enemies.Count <= 0)
            {
                return;
            }

            int nodeId = (int)value;
            int checkType = (int)condValue;
            for (int i = 0; i < enemies.Count; i++)
            {
                int idx = enemies[i];
                if (BattleEnemyManager.Instance.GetNodeTypeByIdx(idx) != checkType)
                {
                    continue;
                }

                FormulaHost enemy = BattleEnemyManager.Instance.GetHost(idx);
                if (enemy == null)
                {
                    continue;
                }

                GameObject obj = BattleEnemyManager.Instance.GetObj(idx);
                if (obj == null)
                {
                    continue;
                }

                string _cfgname = ConfigManager.instance.GetConfigStringValue("notedata", "id", "animation", nodeId.ToString());
                if (_cfgname == null)
                {
                    continue;
                }

                float aniTick = 0;
                string filename = GameGlobal.PREFABS_PATH + _cfgname;
                GameObject objNew = StageBattleComponent.Instance.AddObjWithControllerInit(ref filename, idx);
                if (objNew == null)
                {
                    continue;
                }

                objNew.name += idx;
                enemy.SetDynamicData(SignKeys.GAME_OBJECT, objNew);
                NodeInitController initController = objNew.GetComponent<NodeInitController>();
                initController.Run();

                Animator ani = obj.GetComponent<Animator>();
                Animator aniNew = objNew.GetComponent<Animator>();
                if (ani != null && aniNew != null)
                {
                    aniNew.SetTime(ani.GetTime());
                }

                GameObject.Destroy(obj);
            }
        }
    }
}