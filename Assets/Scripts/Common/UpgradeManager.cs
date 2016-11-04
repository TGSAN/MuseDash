namespace Assets.Scripts.Common
{
    public enum UpgradeResultType
    {
        Good,
        Great,
        Perfect,
    }

    public class UpgradeInfo
    {
        public UpgradeResultType result;
        public float radio;
        public float probability;

        public UpgradeInfo(UpgradeResultType r, float ra, float p)
        {
            result = r;
            radio = ra;
            probability = p;
        }
    }

    public class UpgradeManager : Singleton<UpgradeManager>
    {
        private UpgradeInfo m_GoodInfo, m_GreatInfo, m_PerfectInfo;

        public UpgradeManager()
        {
            var good = ConfigPool.Instance.GetConfigByName("upgrade_info", "good");
            m_GoodInfo = new UpgradeInfo(UpgradeResultType.Good, (float)good["radio"], (float)good["probability"]);
            var great = ConfigPool.Instance.GetConfigByName("upgrade_info", "great");
            m_GreatInfo = new UpgradeInfo(UpgradeResultType.Great, (float)great["radio"], (float)great["probability"]);
            var perfect = ConfigPool.Instance.GetConfigByName("upgrade_info", "perfect");
            m_PerfectInfo = new UpgradeInfo(UpgradeResultType.Perfect, (float)perfect["radio"], (float)perfect["probability"]);
        }

        public UpgradeResultType GetUpgradeResult()
        {
            var random = UnityEngine.Random.Range(0, 10000);
            return UpgradeResultType.Good;
        }
    }
}