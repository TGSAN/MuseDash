using FormulaBase;
using System.Collections.Generic;
using System.Linq;
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

        public static string ConfigToString()
        {
            var capsuleStr = "0,";
            var config = ConfigPool.Instance.GetConfigByName("capsule_combination");
            var lvlDic = new Dictionary<int, int>();
            for (int i = 0; i < config.Count; i++)
            {
                var lvl = (int)config[i]["account_level"];
                var id = (int)config[i]["id"];
                lvlDic.Add(id, lvl);
            }
            var curLvl = AccountManagerComponent.Instance.Host.GetDynamicIntByKey(SignKeys.LEVEL);
            var curIdx = 1;
            foreach (var pair in lvlDic)
            {
                if (curLvl <= pair.Value)
                {
                    curIdx = pair.Key;
                    break;
                }
            }
            var capsuleIdxs = new List<int>();
            var capsuleConfig = config[curIdx.ToString()];
            for (int i = 1; i <= 20; i++)
            {
                var key = "capsule" + i.ToString();
                var value = (int)capsuleConfig[key];
                if (value != 0)
                {
                    capsuleIdxs.Add(value);
                }
            }
            for (var i = 0; i < capsuleIdxs.Count; i++)
            {
                capsuleStr += capsuleIdxs[i].ToString() + (i == capsuleIdxs.Count - 1 ? string.Empty : ",");
            }
            return capsuleStr;
        }

        public static List<Capsule> StringToList(string str)
        {
            var strArray = str.Split(',');
            return strArray.Select(s => new Capsule(int.Parse(s))).ToList();
        }

        public static string ListToString(List<Capsule> list)
        {
            string str = string.Empty;
            for (var i = 0; i < list.Count; i++)
            {
                var uid = list[i].uid;
                str += uid + (i == list.Count - 1 ? string.Empty : ",");
            }
            return str;
        }
    }

    public class CapsuleManager : Singleton<CapsuleManager>
    {
        /*  public Capsule curCapsule
          {
          }*/

        public Capsule RandomCapsule()
        {
            var capsuleStr = AccountManagerComponent.Instance.GetCapsuleStr();
            var list = Capsule.StringToList(capsuleStr);
            var idx = UnityEngine.Random.Range(1, list.Count - 1);
            var capsule = list[idx];
            list.RemoveAt(idx);
            if (list.Count > 0)
            {
                list[0] = capsule;
            }
            var str = Capsule.ListToString(list);
            AccountManagerComponent.Instance.SetCapsuleStr(str);
            return capsule;
        }
    }
}