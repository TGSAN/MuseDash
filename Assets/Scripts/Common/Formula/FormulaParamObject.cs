using LitJson;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace FormulaBase
{
    /// <summary>
    /// Formula parameter object.
    /// </summary>
    public class FormulaParamObject
    {
        private const char CONFIG_SPLITE_DOT = '.';
        private const string STR_ZERO = "0";

        /// <summary>
        /// The dynamic value.
        ///
        /// DATA_SOURCE_CONFIG			:	Dynamic value is index of config, from sum of condiction.
        /// 								GetData is config value.
        ///
        /// DATA_SOURCE_QUOTE_FORMULA	:	Dynamic value is sum of condiction, with dynamic formula Result
        /// 								fill in condictions to be dynamic value.
        /// 								GetData is condiction sum from dynamic fromula Result.
        ///
        /// DATA_SOURCE_DYNAMIC		:	Dynamic value is sum of condiction.
        /// 								GetData is condiction sum.
        ///
        /// DATA_SOURCE_BASE_VALUE		:	Dynamic value is base value.
        /// 								GetData is base value + condiction sum.
        /// </summary>
        private float dynamicValue = 0f;

        private string configName = null;
        private string configKey = null;
        private string signKey = null;

        private FormulaObject parent = null;
        private FormulaParamStruct param;
        private FormulaObject dynamicFormula = null;

        private FormulaCondictionObject[] condictions = null;

        public FormulaParamObject(FormulaObject parent, FormulaParamStruct paramStruct)
        {
            this.parent = parent;
            this.param = paramStruct;
            this.dynamicValue = 0f;

            // Auto sign key.
            if (FormulaBase.IsSignMatch(this.param.paramname))
            {
                this.signKey = this.param.paramname;
            }

            // Same type same condiction, auto group.
            if (this.param.condictions != null)
            {
                this.condictions = new FormulaCondictionObject[this.param.condictions.Length];
                for (int i = 0; i < this.condictions.Length; i++)
                {
                    FormulaCondictionObject fco = new FormulaCondictionObject(this, this.param.condictions[i]);
                    this.condictions[i] = fco;
                }
            }

            /*
			switch (this.param.dataSourceType) {
			case FormulaBase.DATA_SOURCE_CONFIG:
				break;

			case FormulaBase.DATA_SOURCE_QUOTE_FORMULA:
				break;

			case FormulaBase.DATA_SOURCE_CONDICTION:
				break;

			case FormulaBase.DATA_SOURCE_BASE_VALUE:
				break;
			}
			*/
        }

        /// <summary>
        /// Ups the data dynamic value.
        ///
        /// 配置数据(条件输入值为配置索引,默认0)
        /// 引用公式(条件输入值为公式结果)
        /// 动态数据(条件输入值为动态数据)
        /// </summary>
        public void UpDataDynamicValue()
        {
            float value = 0f;
            switch (this.param.dataSourceType)
            {
                case FormulaBase.DATA_SOURCE_CONFIG:
                    value = this.GetCondictionValue();
                    string d = this.GetParamByConfig((int)value);
                    float _out = 0f;
                    if (d != null && float.TryParse(d, out _out))
                    {
                        this.dynamicValue = float.Parse(d);
                    }
                    break;

                case FormulaBase.DATA_SOURCE_QUOTE_FORMULA:
                    this.dynamicValue = this.GetValueByFormula();
                    this.__UpDataCondictionByInput(this.dynamicValue);
                    value = this.GetCondictionValue();
                    this.dynamicValue += value;
                    break;

                case FormulaBase.DATA_SOURCE_DYNAMIC:
                    FormulaHost host = this.parent.GetHost();
                    this.dynamicValue = 0f;
                    float dyValue = 0f;
                    if (host != null)
                    {
                        dyValue = host.GetDynamicDataByKey(this.param.dataSourceName);
                    }
                    /*
                    if (host == null) {
                        this.dynamicValue = 0f;
                    } else {
                        this.dynamicValue = host.GetDynamicDataByKey (this.param.dataSourceName);
                    }
                    */

                    this.__UpDataCondictionByInput(dyValue);
                    value = this.GetCondictionValue();
                    this.dynamicValue += value;

                    break;

                case FormulaBase.DATA_SOURCE_BASE_VALUE:
                    this.dynamicValue = this.param.dataSourceValue + this.GetCondictionValue();
                    break;

                case FormulaBase.DATA_SOURCE_PARAM_CONFIG:
                    value = this.GetCondictionValue();
                    this.dynamicValue = this.GetParamByConfigedConfig((int)value, this.param.dataSourceName);
                    break;
            }
        }

        private void __UpDataCondictionByInput(float input)
        {
            if (this.condictions == null || this.condictions.Length <= 0)
            {
                return;
            }

            for (int i = 0; i < this.condictions.Length; i++)
            {
                FormulaCondictionObject fco = this.condictions[i];
                fco.UpDataDynamicValue(input);
            }
        }

        private float GetCondictionValue()
        {
            float v = 0f;
            if (this.condictions == null || this.condictions.Length <= 0)
            {
                return v;
            }

            for (int i = 0; i < this.condictions.Length; i++)
            {
                FormulaCondictionObject fco = this.condictions[i];
                fco.UpDataDynamicValue();
                v += fco.GetData();
            }

            return v;
        }

        public float GetData()
        {
            return this.dynamicValue;
        }

        public FormulaObject GetParent()
        {
            return this.parent;
        }

        // ------------------------------------------------------
        private void InitConfigFormula()
        {
            if (this.configName != null)
            {
                return;
            }

            if (this.param.dataSourceName == null)
            {
                return;
            }

            if (!this.param.dataSourceName.Contains(CONFIG_SPLITE_DOT.ToString()))
            {
                UnityEngine.Debug.Log("Formula data source of config name error " + this.param.dataSourceName);
                return;
            }

            string[] ss = this.param.dataSourceName.Split(new char[] { CONFIG_SPLITE_DOT });
            this.configName = ss[0];
            this.configKey = ss[1];
        }

        private void InitQuoteFormula()
        {
            if (this.dynamicFormula != null)
            {
                return;
            }

            if (this.parent == null)
            {
                return;
            }

            FormulaObject fo = this.parent.GetFormulaFromHost(this.param.dataSourceName);
            if (fo == null)
            {
                FormulaHost host = this.parent.GetHost();
                fo = FormulaManager.Instance.CreateFormula(host, this.param.dataSourceName);
            }

            this.dynamicFormula = fo;
        }

        // ------------------------------------------------------
        private float GetParamByConfigedConfig(int key, string cfgkey)
        {
            string[] _cr = cfgkey.Split('/');
            string[] cfgparams = _cr[1].Split('_');
            int ii = 0;
            string[] _kl = new string[cfgparams.Length];
            foreach (string _cfp in cfgparams)
            {
                if (!FormulaBase.IsSignMatch(_cfp))
                {
                    _kl[ii] = _cfp;
                }
                else
                {
                    string _v = this.parent.GetHost().GetDynamicStrByKey(_cfp);
                    _kl[ii] = _v;
                }

                ii++;
            }

            float _out = 0f;
            string k = string.Join("_", _kl);
            string sv = ConfigPool.Instance.GetConfigStringValue(_cr[0], key.ToString(), k);
            if (sv != null && float.TryParse(sv, out _out))
            {
                return float.Parse(sv);
            }

            return _out;
        }

        private string GetParamByConfig(int key)
        {
            this.InitConfigFormula();
            if (this.configName == null)
            {
                return STR_ZERO;
            }

            JsonData data = ConfigPool.Instance.GetConfigValue(this.configName, key.ToString(), this.configKey);
            if (this.signKey != null)
            {
                FormulaHost host = this.parent.GetHost();
                if (host != null && data != null)
                {
                    host.SetDynamicData(this.signKey, data.ToString());
                    // UnityEngine.Debug.Log(this.signKey + "     " + data.ToString());
                }
            }

            if (data == null)
            {
                return null;
            }

            return data.ToString();
        }

        private float GetValueByFormula()
        {
            this.InitQuoteFormula();
            if (this.dynamicFormula == null)
            {
                return 0f;
            }

            this.dynamicFormula.UpDataDynamicValue();

            return this.dynamicFormula.Result();
        }

        private int GetCombineKey(FormulaCondictionObject fco)
        {
            int ct = fco.GetCondictionType();
            int ck = fco.GetCondictionKey();
            int k = ct * 10 + ck;

            return k;
        }
    }
}