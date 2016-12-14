using Boo.Lang;
using Newtonsoft.Json;
using UnityEngine;

namespace Assets.Scripts.Common.Manager
{
    public class DailyTask
    {
        public string description;
        public int coinAward;
        public int crystalAward;
        public string icon;
    }

    public class DailyTaskManager : Singleton<DailyTaskManager>
    {
        private List<DailyTask> m_TaskConfig;

        public DailyTaskManager()
        {
            ResourceLoader.Instance.Load("config/daily_task", res =>
            {
                var txt = res as TextAsset;
                m_TaskConfig = JsonConvert.DeserializeObject<List<DailyTask>>(txt.text);
            });
        }
    }
}