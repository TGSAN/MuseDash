using FormulaBase;
using System;
using System.Collections.Generic;

namespace Assets.Scripts.Common
{
    public enum UpgradeResultType
    {
        Cool,
        Great,
        Perfect,
    }

    public class UpgradeInfo
    {
        public UpgradeResultType result;
        public float radio;
        public float probability;

        public UpgradeInfo(UpgradeResultType r)
        {
            var strName = r.ToString().ToLower();
            var jsonData = ConfigPool.Instance.GetConfigValue("upgrade_info", strName);
            result = r;
            radio = (float)((double)jsonData["radio"]);
            probability = (float)((double)jsonData["probability"]);
        }
    }

    public class UpgradeManager : Singleton<UpgradeManager>
    {
        private readonly UpgradeInfo m_CoolInfo, m_GreatInfo, m_PerfectInfo;

        public UpgradeManager()
        {
            var good = ConfigPool.Instance.GetConfigByName("upgrade_info", "cool");
            m_CoolInfo = new UpgradeInfo(UpgradeResultType.Cool);
            m_GreatInfo = new UpgradeInfo(UpgradeResultType.Great);
            m_PerfectInfo = new UpgradeInfo(UpgradeResultType.Perfect);
        }

        public UpgradeInfo GetUpgradeResult()
        {
            var result = m_CoolInfo;
            RandomUtils.RandomEvent(new[] { m_CoolInfo.probability, m_GreatInfo.probability, m_PerfectInfo.probability },
                new Action[]
                {
                    () =>
                    {
                        result = m_CoolInfo;
                    },
                    () =>
                    {
                         result = m_GreatInfo;
                    },
                    () =>
                    {
                         result = m_PerfectInfo;
                    }
                });
            return result;
        }

        public bool IsRoleLvlMax(FormulaHost host)
        {
            var lvlMax = (int)host.Result(FormulaKeys.FORMULA_11);
            var curLvl = host.GetDynamicIntByKey(SignKeys.LEVEL);
            return curLvl >= lvlMax;
        }

        public FormulaHost RoleLevelUp(FormulaHost host, List<FormulaHost> expHosts, HttpResponseDelegate callFunc = null, bool isSave = true)
        {
            var exp = 0;
            expHosts.ForEach(h =>
            {
                exp += (int)h.Result(FormulaKeys.FORMULA_57);
            });
            var originLvl = host.GetDynamicIntByKey(SignKeys.LEVEL);
            var originExp = host.GetDynamicIntByKey(SignKeys.EXP);
            var lvl = host.GetDynamicIntByKey(SignKeys.LEVEL);
            Action lvlUp = null;
            var firstTimeExp = originExp;
            var upgradeResult = GetUpgradeResult();
            exp = (int)((float)exp * upgradeResult.radio);
            lvlUp = () =>
            {
                var expRequired = ConfigPool.Instance.GetConfigIntValue("experience", lvl.ToString(), "char_exp") - firstTimeExp;
                exp -= expRequired;
                if (exp >= 0)
                {
                    lvl++;
                    firstTimeExp = 0;
                    if (!IsRoleLvlMax(host))
                    {
                        lvlUp();
                    }
                }
                else
                {
                    exp += expRequired + firstTimeExp;
                    host.SetDynamicData(SignKeys.LEVEL, lvl);
                    host.SetDynamicData(SignKeys.EXP, exp);
                }
            };
            lvlUp();
            if (isSave)
            {
                CommonPanel.GetInstance().ShowWaittingPanel(true);
                host.Save((result) =>
                {
                    if (!result)
                    {
                        host.SetDynamicData(SignKeys.LEVEL, originLvl);
                        host.SetDynamicData(SignKeys.EXP, originExp);
                    }
                    else
                    {
                        if (originLvl != host.GetDynamicIntByKey(SignKeys.LEVEL))
                        {
                            PnlCharUpgrade.PnlCharUpgrade.Instance.OnShow(host, expHosts.ToArray(), upgradeResult);
                            PnlMainMenu.PnlMainMenu.Instance.OnEnergyUpdate(true);
                        }
                    }
                    if (callFunc != null)
                    {
                        callFunc(result);
                    }
                    CommonPanel.GetInstance().ShowWaittingPanel(false);
                });
            }
            return host;
        }

        public bool IsItemLvlMax(FormulaHost host)
        {
            var lvlMax = (int)host.Result(FormulaKeys.FORMULA_14);
            var curLvl = host.GetDynamicIntByKey(SignKeys.LEVEL);
            return curLvl >= lvlMax;
        }

        public FormulaHost ItemLevelUp(FormulaHost host, List<FormulaHost> expHosts, HttpResponseDelegate callFunc = null, bool isSave = true)
        {
            var exp = 0;
            expHosts.ForEach(h =>
            {
                exp += (int)h.Result(FormulaKeys.FORMULA_57);
            });
            var originLvl = host.GetDynamicIntByKey(SignKeys.LEVEL);
            var originExp = host.GetDynamicIntByKey(SignKeys.EXP);
            var lvl = host.GetDynamicIntByKey(SignKeys.LEVEL);
            Action lvlUp = null;
            var firstTimeExp = originExp;
            var upgradeResult = GetUpgradeResult();
            exp = (int)((float)exp * upgradeResult.radio);
            lvlUp = () =>
            {
                var expRequired = ConfigPool.Instance.GetConfigIntValue("experience", lvl.ToString(), "eqpt_exp") - firstTimeExp;
                exp -= expRequired;
                if (exp >= 0)
                {
                    lvl++;
                    firstTimeExp = 0;
                    if (!IsItemLvlMax(host))
                    {
                        lvlUp();
                    }
                }
                else
                {
                    exp += expRequired + firstTimeExp;
                    host.SetDynamicData(SignKeys.LEVEL, lvl);
                    host.SetDynamicData(SignKeys.EXP, exp);
                }
            };
            lvlUp();
            if (isSave)
            {
                CommonPanel.GetInstance().ShowWaittingPanel(true);
                host.Save((result) =>
                {
                    if (result)
                    {
                        ItemManageComponent.Instance.DeleteListItem(expHosts);
                        if (originLvl != host.GetDynamicIntByKey(SignKeys.LEVEL))
                        {
                            PnlItemUpgrade.PnlItemUpgrade.Instance.OnShow(host, expHosts.ToArray(), upgradeResult);
                        }
                    }
                    else
                    {
                        host.SetDynamicData(SignKeys.LEVEL, originLvl);
                        host.SetDynamicData(SignKeys.EXP, originExp);
                    }
                    if (callFunc != null)
                    {
                        callFunc(true);
                    }
                    CommonPanel.GetInstance().ShowWaittingPanel(false);
                });
            }
            return host;
        }
    }
}