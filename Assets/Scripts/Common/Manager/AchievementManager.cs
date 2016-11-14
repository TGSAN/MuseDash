using FormulaBase;
using GameLogic;
using LitJson;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.Common.Manager
{
    public enum AchievementType
    {
        Perfect,
        Combo,
        Star,
        Clear,
    }

    public enum AchievementGoal
    {
        C,
        B,
        A,
        S,
    }

    public enum AwardType
    {
        Coin,
        Crystal,
    }

    public class Achievement
    {
        public int id;
        public int uid;
        public AchievementGoal achGoalType;
        public AchievementType achType;
        public int goal;
        public int award;
        public AwardType awdType;
        public bool isGet;

        public bool isReach
        {
            get
            {
                var host = TaskStageTarget.Instance.GetStageByIdx(uid);
                if (host == null) return false;
                var achs = Achievement.ToArray(host.GetDynamicStrByKey(SignKeys.ACHIEVEMENT));
                return achs.Any(ach => ach.id == id && ach.achGoalType == achGoalType);
            }
        }

        public static Achievement[] ToArray(string str)
        {
            var list = new List<Achievement>();
            if (string.IsNullOrEmpty(str) || str == "0")
            {
                return list.ToArray();
            }
            var strArray = str.Split(',');
            var lvl = "a_goal";
            foreach (var s in strArray)
            {
                var sIn = s.Split('/');
                var idx = sIn[0];
                for (var i = 1; i < sIn.Length; i++)
                {
                    if (sIn[i].Contains("_goal"))
                    {
                        lvl = sIn[i];
                    }

                    var isGet = true;
                    if (bool.TryParse(sIn[i], out isGet))
                    {
                        var achieveStr = idx + "/" + lvl + "/" + isGet.ToString();
                        list.Add(new Achievement(achieveStr));
                    }
                }
            }
            return list.ToArray();
        }

        public Achievement(string str)
        {
            if (str == "0")
            {
                return;
            }
            var achiConfig = ConfigPool.Instance.GetConfigByName("achievement");
            var strArray = str.Split('/');
            for (var i = 0; i < strArray.Length; i++)
            {
                switch (i)
                {
                    case 0:
                        id = int.Parse(strArray[i]);
                        achiConfig = achiConfig[id.ToString()];
                        break;

                    case 1:
                        goal = (int)achiConfig[strArray[i]];
                        var typeStr = strArray[i].Replace("_goal", string.Empty);
                        typeStr = typeStr.ToUpper();
                        achGoalType = (AchievementGoal)Enum.Parse(typeof(AchievementGoal), typeStr);
                        break;

                    case 2:
                        isGet = bool.Parse(strArray[i]);
                        break;
                }
            }

            var achTypeStr = (string)achiConfig["type"];
            achType = (AchievementType)Enum.Parse(typeof(AchievementType), achTypeStr.Substring(0, 1).ToUpper() + achTypeStr.Substring(1));

            uid = (int)achiConfig["uid"];

            var awardStr = achiConfig[achGoalType.ToString().ToLower() + "_award"].ToString();
            var awdTypeStr = awardStr.Split('_')[0];
            awdType = (AwardType)Enum.Parse(typeof(AwardType), awdTypeStr.Substring(0, 1).ToUpper() + awdTypeStr.Substring(1));

            award = int.Parse(awardStr.Split('_')[1]);
        }

        public override string ToString()
        {
            return id.ToString() + "/" + achGoalType.ToString().ToLower() + "_goal/" + isGet.ToString();
        }
    }

    public class AchievementManager : Singleton<AchievementManager>
    {
        public bool AddAchievement(FormulaHost stageHost, int id, string lvl)
        {
            var curAchStr = stageHost.GetDynamicStrByKey(SignKeys.ACHIEVEMENT);
            var strArray = string.IsNullOrEmpty(curAchStr) ? new[] { string.Empty } : curAchStr.Split(',');

            for (var i = 0; i < strArray.Length; i++)
            {
                var array = strArray[i].Split('/').ToList();
                if (array.Contains(id.ToString()))
                {
                    if (array.Contains(lvl))
                    {
                        return false;
                    }
                    else
                    {
                        strArray[i] += "/" + lvl + "/true";
                        break;
                    }
                }
                else
                {
                    if (i != strArray.Length - 1) continue;
                    var list = strArray.ToList();
                    list.Add(id.ToString() + "/" + lvl + "/true");
                    strArray = list.ToArray();
                    break;
                }
            }

            curAchStr = string.Empty;
            foreach (var s in strArray)
            {
                if (s == string.Empty || s == "0") continue;
                curAchStr += s;
                if (strArray.Last<string>() != s)
                {
                    curAchStr += ",";
                }
            }
            stageHost.SetDynamicData(SignKeys.ACHIEVEMENT, curAchStr);
            return true;
        }

        public void ReceieveAchievement(FormulaHost stageHost)
        {
            var achs = stageHost.GetDynamicStrByKey(SignKeys.ACHIEVEMENT);
            var achievements = Achievement.ToArray(achs);
            var coins = 0;
            var crystals = 0;
            foreach (var achievement in achievements)
            {
                var awdType = achievement.awdType;
                if (awdType == AwardType.Coin && !achievement.isGet)
                {
                    coins += achievement.award;
                }
                else if (awdType == AwardType.Crystal && !achievement.isGet)
                {
                    crystals += achievement.award;
                }
            }

            var strArray = achs.Split(',');
            for (var i = 0; i < strArray.Length; i++)
            {
                strArray[i] = strArray[i].Replace("true", "false");
            }
            achs = strArray.TakeWhile((t, i) => i != strArray.Length - 1).Aggregate(string.Empty, (current, t) => current + (t + ","));
            stageHost.SetDynamicData(SignKeys.ACHIEVEMENT, achs);
            AccountGoldManagerComponent.Instance.ChangeMoney(coins);
            AccountCrystalManagerComponent.Instance.ChangeDiamond(crystals);
        }

        public void SetAchievement(FormulaHost stageHost)
        {
            var clearCount = stageHost.GetDynamicIntByKey(TaskStageTarget.TASK_SIGNKEY_STAGE_CLEAR_COUNT);
            var perfectMaxCount = stageHost.GetDynamicIntByKey(TaskStageTarget.TASK_SIGNKEY_EVLUATE_HEAD + GameMusic.PERFECT + TaskStageTarget.TASK_SIGNKEY_COUNT_MAX_TAIL);
            var comboCount = stageHost.GetDynamicIntByKey(TaskStageTarget.TASK_SIGNKEY_MAX_COMBO);
            var starCount = stageHost.GetDynamicIntByKey(TaskStageTarget.TASK_SIGNKEY_HIDE_NODE_COUNT + TaskStageTarget.TASK_SIGNKEY_COUNT_MAX_TAIL);
            var achievementConfig = ConfigPool.Instance.GetConfigByName("achievement");
            var count = 0;
            var achieveTpyeNum = Enum.GetNames(typeof(AchievementType)).Length;
            var stageIdx = stageHost.GetDynamicIntByKey(SignKeys.ID);
            var goalType = Enum.GetNames(typeof(AchievementGoal));
            Action<JsonData, int, int> resultFunc = (ach, value, i) =>
            {
                foreach (string t in goalType)
                {
                    var goal = t.ToLower() + "_goal";
                    var goalNum = (int)ach[goal];
                    if (value >= goalNum)
                    {
                        AddAchievement(stageHost, i, goal);
                    }
                }
            };
            for (int i = 1; i < achievementConfig.Count; i++)
            {
                var ach = achievementConfig[i.ToString()];
                if ((int)ach["uid"] == stageIdx)
                {
                    var achTypeStr = ach["type"].ToString();

                    var achType = (AchievementType)Enum.Parse(typeof(AchievementType),
                        achTypeStr.Substring(0, 1).ToUpper() + achTypeStr.Substring(1));
                    switch (achType)
                    {
                        case AchievementType.Perfect:
                            {
                                resultFunc(ach, perfectMaxCount, i);
                            }
                            break;

                        case AchievementType.Combo:
                            {
                                resultFunc(ach, comboCount, i);
                            }
                            break;

                        case AchievementType.Star:
                            {
                                resultFunc(ach, starCount, i);
                            }
                            break;

                        case AchievementType.Clear:
                            {
                                resultFunc(ach, clearCount, i);
                            }
                            break;

                        default
                            :
                            break;
                    }
                    if (count++ == achieveTpyeNum)
                    {
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// 获得所有成就
        /// </summary>
        /// <returns></returns>
        public Achievement[] GetAllAchievements()
        {
            var list = new List<Achievement>();
            var hostList = TaskStageTarget.Instance.HostList ?? TaskStageTarget.Instance.GetList("Task");
            foreach (var value in hostList.Values)
            {
                var idx = value.GetDynamicIntByKey(SignKeys.ID);
                var achs = GetAchievements(idx);
                achs.ToList().ForEach(list.Add);
            }
            return list.ToArray();
        }

        /// <summary>
        /// 获得当前关卡的成就
        /// </summary>
        /// <returns></returns>
        public Achievement[] GetAchievements()
        {
            var idx = TaskStageTarget.Instance.Host.GetDynamicIntByKey(SignKeys.ID);
            return GetAchievements(idx);
        }

        /// <summary>
        /// 获得某个关卡的成就
        /// </summary>
        /// <returns></returns>
        public Achievement[] GetAchievements(int idx)
        {
            var list = new List<Achievement>();
            var achConfig = ConfigPool.Instance.GetConfigByName("achievement");
            for (var i = 1; i <= achConfig.Count; i++)
            {
                var ach = achConfig[i.ToString()];
                if ((int)ach["uid"] != idx) continue;
                list.Add(new Achievement(ach["id"].ToString() + "/c_goal/true"));
                list.Add(new Achievement(ach["id"].ToString() + "/b_goal/true"));
                list.Add(new Achievement(ach["id"].ToString() + "/a_goal/true"));
                list.Add(new Achievement(ach["id"].ToString() + "/s_goal/true"));
            }
            list.Sort((l, r) =>
            {
                if (l.id != r.id)
                {
                    return l.id - r.id;
                }
                return l.achGoalType - r.achGoalType;
            });
            return list.ToArray();
        }

        public int GetComboMax(int idx)
        {
            var host = TaskStageTarget.Instance.GetStageByIdx(idx);
            return host == null ? 0 : host.GetDynamicIntByKey(TaskStageTarget.TASK_SIGNKEY_MAX_COMBO);
        }

        public int GetClearCount(int idx)
        {
            var host = TaskStageTarget.Instance.GetStageByIdx(idx);
            return host == null ? 0 : host.GetDynamicIntByKey(TaskStageTarget.TASK_SIGNKEY_STAGE_CLEAR_COUNT);
        }

        public int GetPerfectMax(int idx)
        {
            var host = TaskStageTarget.Instance.GetStageByIdx(idx);
            return host == null ? 0 : host.GetDynamicIntByKey(TaskStageTarget.TASK_SIGNKEY_EVLUATE_HEAD + GameMusic.PERFECT + TaskStageTarget.TASK_SIGNKEY_COUNT_MAX_TAIL);
        }

        public int GetStarMax(int idx)
        {
            var host = TaskStageTarget.Instance.GetStageByIdx(idx);
            return host == null ? 0 : host.GetDynamicIntByKey(TaskStageTarget.TASK_SIGNKEY_HIDE_NODE_COUNT + TaskStageTarget.TASK_SIGNKEY_COUNT_MAX_TAIL);
        }
    }
}