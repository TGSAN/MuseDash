/// <summary>
/// Formula condiction object.
/// </summary>
using System;

namespace FormulaBase
{
    public class FormulaCondictionObject
    {
        private float dynamicValue = 0f;
        private FormulaHost host = null;
        private FormulaParamObject parentParam = null;
        private FormulaObject dynamicFormula = null;
        private FormulaObject dynamicFormulaComp = null;
        private FormulaParamCondictionStruct condiction;

        private string cfgname = null;
        private string rowname = null;

        public FormulaCondictionObject(FormulaParamObject parent, FormulaParamCondictionStruct cond)
        {
            this.parentParam = parent;
            this.condiction = cond;
            this.dynamicValue = 0f;
        }

        public int GetCondictionType()
        {
            return this.condiction.condictionType;
        }

        public int GetCondictionKey()
        {
            return this.condiction.condictionKeyIndex;
        }

        /// <summary>
        /// Gets the data.
        ///
        /// CONDICTION_TYPE_SIGN  		:  sign count * plus
        /// CONDICTION_TYPE_OVER_VALUE  :  if child-formula value more than config value, use config plus, else 0
        /// CONDICTION_TYPE_EQUAL_VALUE	:  if sign value == config value, use config plus, else 0
        /// </summary>
        /// <returns>The data.</returns>
        public float GetData()
        {
            return this.dynamicValue;
        }

        // By outside data.
        // It's not suggest to use directly.
        public void UpDataDynamicValue(float value)
        {
            this.dynamicValue = value;
        }

        // By condiction config, default use.
        public void UpDataDynamicValue()
        {
            switch (this.condiction.condictionType)
            {
                case FormulaBase.CONDICTION_TYPE_SIGN:
                    // Find sign count on host.
                    //this.dynamicValue
                    this.dynamicValue = 0f;
                    this.InitHost();
                    if (this.host != null)
                    {
                        this.dynamicValue = host.GetDynamicDataByKey(this.condiction.condictionKey);
                    }

                    if (this.condiction.condictionValue > 0)
                    {
                        this.dynamicValue = (int)Math.Min(this.dynamicValue, this.condiction.condictionValue) * this.condiction.plus;
                    }
                    else
                    {
                        this.dynamicValue = (int)(this.dynamicValue * this.condiction.plus);
                    }
                    break;

                case FormulaBase.CONDICTION_TYPE_OVER_VALUE:
                    {
                        this.InitQuoteFormula();
                        if (this.dynamicFormula == null)
                        {
                            return;
                        }

                        this.dynamicFormula.UpDataDynamicValue();
                        float value = this.dynamicFormula.Result();
                        if (value < this.condiction.condictionValue)
                        {
                            this.dynamicValue = 0f;
                        }
                        else
                        {
                            this.dynamicValue = this.condiction.plus;
                        }
                    }

                    break;

                case FormulaBase.CONDICTION_TYPE_BASE_VALUE:
                    this.dynamicValue = this.condiction.plus;
                    break;

                case FormulaBase.CONDICTION_TYPE_EQUAL_VALUE:
                    float _value = 0f;
                    this.dynamicValue = 0f;
                    this.InitHost();
                    if (this.host != null)
                    {
                        _value = this.host.GetDynamicDataByKey(this.condiction.condictionKey);
                    }

                    if (_value == this.condiction.condictionValue)
                    {
                        this.dynamicValue = this.condiction.plus;
                    }

                    break;

                case FormulaBase.CONDICTION_TYPE_OVER_RATE:
                    this.dynamicValue = 0f;
                    int _randValue = UnityEngine.Random.Range(0, 100);
                    if (_randValue >= this.condiction.condictionValue)
                    {
                        this.dynamicValue = this.condiction.plus;
                    }

                    break;

                case FormulaBase.CONDICTION_TYPE_RATE:
                    this.dynamicValue = UnityEngine.Random.Range(0, 100) * 0.01f;
                    break;

                case FormulaBase.CONDICTION_TYPE_OVER_FORMULA:
                    this.InitQuoteCompFormula();
                    if (this.dynamicFormula == null || this.dynamicFormulaComp == null)
                    {
                        return;
                    }

                    this.dynamicFormula.UpDataDynamicValue();
                    this.dynamicFormulaComp.UpDataDynamicValue();
                    float _valueL = this.dynamicFormula.Result();
                    float _valueR = this.dynamicFormulaComp.Result();
                    if (_valueL <= _valueR)
                    {
                        this.dynamicValue = this.condiction.plus;
                    }
                    else
                    {
                        this.dynamicValue = 0f;
                    }

                    break;

                case FormulaBase.CONDICTION_TYPE_OVER_FIX_VALUE:
                    this.InitHost();
                    if (this.host == null)
                    {
                        this.dynamicValue = 0f;
                    }
                    else
                    {
                        float _dvalue = this.host.GetDynamicDataByKey(this.condiction.condictionKey);
                        if (_dvalue < this.condiction.condictionValue)
                        {
                            this.dynamicValue = 0f;
                        }
                        else
                        {
                            this.dynamicValue = this.condiction.plus;
                        }
                    }

                    break;

                case FormulaBase.CONDICTION_TYPE_CFG_VALUE:
                    this.InitHost();
                    if (this.host == null)
                    {
                        this.dynamicValue = 0f;
                    }
                    else
                    {
                        this.InitConfig();
                        string signKey = FormulaData.Instance.DynamicParams[this.condiction.condictionKeyIndex];
                        string key = ((int)this.host.GetDynamicDataByKey(signKey)).ToString();
                        if (this.cfgname != null)
                        {
                            this.dynamicValue = float.Parse(ConfigPool.Instance.GetConfigValue(this.cfgname, key, this.rowname).ToJson());
                        }
                        else
                        {
                            this.dynamicValue = 0f;
                        }
                    }

                    break;
            }
        }

        private void InitConfig()
        {
            if (this.cfgname != null)
            {
                return;
            }

            string[] ss = this.condiction.condictionKey.Split('.');
            this.cfgname = ss[0];
            this.rowname = ss[1];
        }

        private void InitQuoteFormula()
        {
            if (this.dynamicFormula != null)
            {
                return;
            }

            if (this.parentParam == null)
            {
                return;
            }

            FormulaObject parent = this.parentParam.GetParent();
            if (parent == null)
            {
                return;
            }

            FormulaObject fo = parent.GetFormulaFromHost(this.condiction.condictionKey);
            if (fo == null)
            {
                return;
            }

            this.dynamicFormula = fo;
        }

        private void InitQuoteCompFormula()
        {
            if (this.dynamicFormula != null && this.dynamicFormulaComp != null)
            {
                return;
            }

            if (this.parentParam == null)
            {
                return;
            }

            FormulaObject parent = this.parentParam.GetParent();
            if (parent == null)
            {
                return;
            }

            string _fname = this.condiction.condictionKey;
            string _fnameComp = FormulaData.Instance.Formulas[(int)this.condiction.condictionValue].name;

            if (_fname.Contains("/"))
            {
                _fname = _fname.Split('/')[1];
            }

            if (_fnameComp.Contains("/"))
            {
                _fnameComp = _fnameComp.Split('/')[1];
            }

            FormulaObject fo = parent.GetFormulaFromHost(_fname);
            FormulaObject focomp = parent.GetFormulaFromHost(_fnameComp);
            if (fo == null || focomp == null)
            {
                return;
            }

            this.dynamicFormula = fo;
            this.dynamicFormulaComp = focomp;
        }

        private void InitHost()
        {
            if (this.host != null)
            {
                return;
            }

            if (this.parentParam == null)
            {
                return;
            }

            FormulaObject fpo = this.parentParam.GetParent();
            if (fpo == null)
            {
                return;
            }

            this.host = fpo.GetHost();
        }
    }
}