using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Common.Manager
{
    public class Capsule
    {
        public int id;
        public int uid;
        public Dictionary<int, Dictionary<int, int>> items;
        public int charmRequire;
        public string path;
        public string name;
        public string description;

        public GameObject go
        {
            get
            {
                GameObject g = null;
                ResourceLoader.Instance.Load(path, res => g = res as GameObject);
                return g;
            }
        }

        public Capsule(int i)
        {
            var config = ConfigPool.Instance.GetConfigByName("Capsule");
            uid = i;
            id = 1;
            for (var j = 1; j <= config.Count; j++)
            {
                if ((int)config[j.ToString()]["uid"] != i) continue;
                id = j;
                break;
            }
            var capsuleConfig = config[i.ToString()];
            charmRequire = (int)capsuleConfig["charm_require"];
            path = capsuleConfig["path"].ToString();
            name = capsuleConfig["name"].ToString();
            description = capsuleConfig["description"].ToString();
            items = new Dictionary<int, Dictionary<int, int>>();

            for (int j = 1; j <= config.Count; j++)
            {
                var capsuleItem = config[j.ToString()];
                if ((int)capsuleItem["uid"] == uid)
                {
                    for (int k = 1; k <= 5; k++)
                    {
                        if (!items.ContainsKey(k))
                        {
                            items.Add(k, new Dictionary<int, int>());
                        }
                        var keyV = "probability" + k;
                        var keyP = "item" + k;
                        var valueP = (int)capsuleConfig[keyP];
                        var valueV = (int)capsuleConfig[keyV];
                        if (valueP != 0 && valueV != 0)
                        {
                            items[k].Add(valueP, valueV);
                        }
                    }
                }
            }
        }
    }

    public class CapsuleManager
    {
    }
}