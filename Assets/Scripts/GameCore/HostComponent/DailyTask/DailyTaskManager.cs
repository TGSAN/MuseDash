using Newtonsoft.Json;

///自定义模块，可定制模块具体行为
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

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
                var taskGroup = Host.GetDynamicStrByKey(SignKeys.TASK_GROUP);
                var groups = taskGroup.Split(',').ToList();
                if (groups.Count < 3)
                {
                    InitTaskList();
                    taskGroup = Host.GetDynamicStrByKey(SignKeys.TASK_GROUP);
                    groups = taskGroup.Split(',').ToList();
                }
                var taskList = groups.Select(int.Parse).Select(idx => m_TaskConfig[idx - 1]).ToList();
                return taskList;
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

            if (this.Host == null)
            {
                Host = FomulaHostManager.Instance.LoadHost(HOST_IDX);
                Host.SetAsUINotifyInstance();
                InitTaskList();
            }
        }

        public DailyTask GetRandomDailyTask()
        {
            var randomList = new List<DailyTask>(m_TaskConfig);
            foreach (var dailyTask in curTaskList)
            {
                randomList.RemoveAll(d => d.uid == dailyTask.uid);
            }
            var randomIdx = UnityEngine.Random.Range(0, randomList.Count - 1);
            return randomList[randomIdx];
        }

        private void InitTaskList()
        {
            var randomList = new List<DailyTask>(m_TaskConfig);
            var taskGroup = string.Empty;
            for (int i = 0; i < 3; i++)
            {
                var randomIdx = UnityEngine.Random.Range(0, randomList.Count - 1);
                var value = randomList[randomIdx];
                taskGroup += value.uid + (i == 2 ? string.Empty : ",");
                randomList.Remove(value);
            }
            Host.SetDynamicData(SignKeys.TASK_GROUP, taskGroup);
            Host.Save();
        }
    }
}