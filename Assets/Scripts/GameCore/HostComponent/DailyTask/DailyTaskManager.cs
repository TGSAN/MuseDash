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
    }

    public class DailyTaskManager : CustomComponentBase
    {
        private static int DAILY_TASK_UPDATE_TIME = 20;
        private List<DailyTask> m_TaskConfig;
        private const int HOST_IDX = 19;
        private static DailyTaskManager m_Instance = null;

        public static DailyTaskManager instance
        {
            get
            {
                if (m_Instance == null)
                {
                    m_Instance = new DailyTaskManager();
                }
                return m_Instance;
            }
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
            var dt = DAILY_TASK_UPDATE_TIME - (curTime - finishTime);
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
            FomulaHostManager.Instance.AddHost(dailyTask);
            HostList.Add(taskData.uid.ToString(), dailyTask);
        }
    }
}