using cn.bmob.api;
using cn.bmob.io;
using Newtonsoft.Json;

///自定义模块，可定制模块具体行为
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityScript.Lang;

namespace FormulaBase
{
    public class DailyTask
    {
        public string uid;
        public string description;
        public int coinAward;
        public int crystalAward;
        public string icon;
        public int target;
    }

    public class DailyTaskManager : CustomComponentBase
    {
        private List<DailyTask> m_TaskConfig;
        private const int HOST_IDX = 19;
        private static DailyTaskManager m_Instance = null;

        public const int FOOD_IDX = 1;
        public const int UPGRADE_ITEM_IDX = 2;
        public const int S_IDX = 3;
        public const int A_IDX = 4;
        public const int EARPHONE_IDX = 5;
        public const int MUSIC_NOTE_IDX = 6;
        public const int COIN_IDX = 7;
        public const int PERFECT_IDX = 8;
        public const int CAPSULE_IDX = 9;
        public const int COMBO_IDX = 10;
        public const int HIDE_NOTE_IDX = 11;

        public void AddValue(int value, int taskID)
        {
            var strName = string.Empty;
            switch (taskID)
            {
                case FOOD_IDX:
                    {
                        strName = SignKeys.DT_FOOD;
                    }
                    break;

                case UPGRADE_ITEM_IDX:
                    {
                        strName = SignKeys.DT_UPGRADE_ITEM;
                    }
                    break;

                case S_IDX:
                    {
                        strName = SignKeys.DT_S;
                    }
                    break;

                case A_IDX:
                    {
                        strName = SignKeys.DT_A;
                    }
                    break;

                case EARPHONE_IDX:
                    {
                        strName = SignKeys.DT_EARPHONE;
                    }
                    break;

                case MUSIC_NOTE_IDX:
                    {
                        strName = SignKeys.DT_MUSIC_NOTE;
                    }
                    break;

                case COIN_IDX:
                    {
                        strName = SignKeys.DT_COIN;
                    }
                    break;

                case PERFECT_IDX:
                    {
                        strName = SignKeys.DT_PERFECT;
                    }
                    break;

                case CAPSULE_IDX:
                    {
                        strName = SignKeys.DT_CAPSULE;
                    }
                    break;

                case COMBO_IDX:
                    {
                        strName = SignKeys.DT_COMBO;
                    }
                    break;

                case HIDE_NOTE_IDX:
                    {
                        strName = SignKeys.DT_HIDE_NOTE;
                    }
                    break;
            }
            foreach (var host in HostList.Values.ToList().Where(host => host.GetDynamicIntByKey(SignKeys.ID) == taskID))
            {
                var id = host.GetDynamicIntByKey(SignKeys.ID);
                var originValue = host.GetDynamicIntByKey(strName);
                var afterValue = originValue + value;
                var targetValue = host.GetDynamicIntByKey(SignKeys.DT_TARGET);
                host.SetDynamicData(strName, afterValue);
                host.SetDynamicData(SignKeys.DT_VALUE, afterValue);
                host.Save(result =>
                {
                    if (!result) return;
                    if (afterValue >= targetValue)
                    {
                        FinishDailyTask(id);
                    }
                });
            }
        }

        public static DailyTaskManager instance
        {
            get { return m_Instance ?? (m_Instance = new DailyTaskManager()); }
        }

        public List<DailyTask> curTaskList
        {
            get
            {
                return HostList.Values.Select(h => h.GetDynamicIntByKey(SignKeys.ID)).Select(idx => m_TaskConfig[idx - 1]).ToList();
            }
        }

        public void Init()
        {
            ResourceLoader.Instance.Load("config/daily_task", res =>
            {
                var txt = res as TextAsset;
                if (txt != null)
                {
                    m_TaskConfig = JsonConvert.DeserializeObject<List<DailyTask>>(txt.text);
                }
            });

            if (HostList == null)
            {
                this.GetList("DailyTask");
            }
            // 如果没有服务器数据,用配置生成本地对象
            if (HostList != null && HostList.Count <= 0)
            {
                for (int i = 0; i < 3; i++)
                {
                    NewDailyTask();
                }
                FormulaHost.SaveList(HostList.Values.ToList());
            }
            else
            {
                BmobUnity.instance.Timestamp((resp, exception) =>
                {
                    for (int i = 0; i < HostList.Count; i++)
                    {
                        CheckDailyTask(HostList.Values.ToList()[i], resp.timestamp.Get());
                    }
                });
            }
        }

        public FormulaHost GetFormulaHost(int id)
        {
            return HostList.Values.ToList().FirstOrDefault(host => host.GetDynamicIntByKey(SignKeys.ID) == id);
        }

        public void FinishDailyTask(int uid)
        {
            if (HostList == null) return;
            var host = HostList.Values.ToList().Find(h => h.GetDynamicIntByKey(SignKeys.ID) == uid);
            BmobUnity.instance.Timestamp((resp, exception) =>
            {
                host.SetDynamicData(SignKeys.FINISH_TIME, resp.timestamp.Get());
                host.Save(result =>
                {
                    CheckDailyTask(host, resp.timestamp.Get());
                });
            });
        }

        private void CheckDailyTask(FormulaHost host, int curTime)
        {
            var finishTime = host.GetDynamicIntByKey(SignKeys.FINISH_TIME);
            if (finishTime == 0) return;
            Action finishFunc = () =>
            {
                host.Delete(result =>
                {
                    if (result)
                    {
                        NewDailyTask();
                        HostList.Remove(host.objectID);
                        FormulaHost.SaveList(HostList.Values.ToList());
                    }
                });
            };
            var dt = (int)host.Result(FormulaKeys.FORMULA_116) - (curTime - finishTime);
            if (dt <= 0)
            {
                finishFunc();
            }
            else
            {
                host.SetRealTimeCountDown(dt);
                TimerHostController.Instance.AddTimerHost(host, () =>
                {
                    finishFunc();
                });
            }
        }

        private DailyTask GetRandomDailyTaskData()
        {
            var randomList = new List<DailyTask>(m_TaskConfig);
            foreach (var dailyTask in curTaskList)
            {
                randomList.RemoveAll(d => d.uid == dailyTask.uid);
            }
            var randomIdx = UnityEngine.Random.Range(0, randomList.Count - 1);
            return randomList[randomIdx];
        }

        private void NewDailyTask()
        {
            var taskData = GetRandomDailyTaskData();
            var dailyTask = FomulaHostManager.Instance.CreateHost("DailyTask");
            dailyTask.SetDynamicData(SignKeys.ID, taskData.uid);
            dailyTask.SetDynamicData(SignKeys.DT_TARGET, taskData.target);
            FomulaHostManager.Instance.AddHost(dailyTask);
            HostList.Add(taskData.uid.ToString(), dailyTask);
        }
    }
}