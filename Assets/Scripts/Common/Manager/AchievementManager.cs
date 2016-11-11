using FormulaBase;
using GameLogic;
using LitJson;
using System;
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
        A,
        B,
        C,
        S,
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
                array.ForEach(Debug.Log);
                Debug.Log("===========1");
                if (array.Contains(id.ToString()))
                {
                    Debug.Log(2 + "===========");
                    if (array.Contains(lvl))
                    {
                        return false;
                    }
                    else
                    {
                        Debug.Log(lvl + "===========");
                        strArray[i] += "/" + lvl + "/true";
                        break;
                    }
                }
                else
                {
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
            Debug.Log(curAchStr);
            stageHost.SetDynamicData(SignKeys.ACHIEVEMENT, curAchStr);
            return true;
        }

        //public void ResultAchievement(FormulaHost stageHost, int id)
        public void GetAchievement(FormulaHost stageHost)
        {
            var clearCount = stageHost.GetDynamicIntByKey(TaskStageTarget.TASK_SIGNKEY_STAGE_CLEAR_COUNT);
            var perfectMaxCount = stageHost.GetDynamicIntByKey(TaskStageTarget.TASK_SIGNKEY_EVLUATE_HEAD + GameMusic.PERFECT + TaskStageTarget.TASK_SIGNKEY_COUNT_MAX_TAIL);
            var comboCount = stageHost.GetDynamicIntByKey(TaskStageTarget.TASK_SIGNKEY_HIT_COUNT + TaskStageTarget.TASK_SIGNKEY_COUNT_MAX_TAIL);
            var starCount = stageHost.GetDynamicIntByKey(TaskStageTarget.TASK_SIGNKEY_HIDE_NODE_COUNT + TaskStageTarget.TASK_SIGNKEY_COUNT_MAX_TAIL);
            var achievementConfig = ConfigPool.Instance.GetConfigByName("achievement");
            var count = 0;
            var achieveTpyeNum = Enum.GetNames(typeof(AchievementType)).Length;
            var stageIdx = stageHost.GetDynamicIntByKey(SignKeys.ID);
            var goalType = Enum.GetNames(typeof(AchievementGoal));
            Action<JsonData, int, int> resultFunc = (ach, value, i) =>
            {
                for (var j = 0; j < goalType.Length; j++)
                {
                    var goal = goalType[j].ToLower() + "_goal";
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
    }
}