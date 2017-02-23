using FormulaBase;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Assets.Scripts.Tools.Managers;

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
            var jsonData = ConfigManager.instance["upgrade_info"][strName];
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
                var expRequired = ConfigManager.instance.GetConfigIntValue("experience", lvl, "char_exp") - firstTimeExp;
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
                        //                        PnlCharUpgrade.PnlCharUpgrade.Instance.OnShow(host, expHosts.ToArray(), upgradeResult);
                        PnlMainMenu.PnlMainMenu.Instance.OnEnergyUpdate(true);
                        TaskManager.instance.AddValue(expHosts.Count, TaskManager.FOOD_IDX);
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

        public int GetItemExp(FormulaHost host)
        {
            var sum = host.GetDynamicIntByKey(SignKeys.EXP);
            var id = host.GetDynamicIntByKey(SignKeys.ID);
            var lvl = host.GetDynamicIntByKey(SignKeys.LEVEL);
            var baseExp = ConfigManager.instance.GetConfigIntValue("items", id, "experience_base");
            for (int i = 1; i < lvl; i++)
            {
                sum += ConfigManager.instance.GetConfigIntValue("experience", i, "eqpt_exp");
            }
            sum = baseExp + (int)((float)sum / 2f);
            return sum;
        }

        public FormulaHost ItemLevelUp(FormulaHost host, List<FormulaHost> expHosts, HttpResponseDelegate callFunc = null, bool isSave = true)
        {
            var exp = 0;
            expHosts.ForEach(h =>
            {
                exp += GetItemExp(h);
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
                var expRequired = ConfigManager.instance.GetConfigIntValue("experience", lvl, "eqpt_exp") - firstTimeExp;
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
                        //                        ItemManageComponent.Instance.DeleteListItem(expHosts);
                        //                        PnlItemUpgrade.PnlItemUpgrade.Instance.OnShow(host, expHosts.ToArray(), upgradeResult);
                        TaskManager.instance.AddValue(expHosts.Count, TaskManager.UPGRADE_ITEM_IDX);
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