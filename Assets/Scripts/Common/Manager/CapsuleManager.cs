using DG.Tweening.Plugins;
using FormulaBase;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.Common.Manager
{
    public class ItemID
    {
        public int id;

        public ItemID(int i)
        {
            id = i;
        }
    }

    public class Capsule
    {
        public int id;
        public int uid;
        public Dictionary<int, Dictionary<ItemID, int>> items;
        public int charmRequire;
        public int path;
        public string name;
        public string description;
        public List<int> itemsID;

        public Capsule(int i)
        {
            var config = ConfigPool.Instance.GetConfigByName("capsule");
            uid = i;
            id = 1;
            var capsuleConfig = config[0];
            for (var j = 0; j < config.Count; j++)
            {
                capsuleConfig = config[j];
                if ((int)(capsuleConfig["uid"]) != i) continue;
                id = (int)capsuleConfig["id"];
                break;
            }

            charmRequire = (int)capsuleConfig["charm_require"];
            path = (int)capsuleConfig["path"];
            name = capsuleConfig["name"].ToString();
            description = capsuleConfig["description"].ToString();
            items = new Dictionary<int, Dictionary<ItemID, int>>();

            for (int j = 0; j < config.Count; j++)
            {
                var capsuleItem = config[j];
                if ((int)capsuleItem["uid"] == uid)
                {
                    for (int k = 1; k <= 5; k++)
                    {
                        if (!items.ContainsKey(k))
                        {
                            items.Add(k, new Dictionary<ItemID, int>());
                        }
                        var keyV = "probability" + k;
                        var keyP = "item" + k;
                        var valueP = (int)capsuleConfig[keyP];
                        var valueV = (int)capsuleConfig[keyV];
                        if (valueP != 0 && valueV != 0)
                        {
                            var idValue = new ItemID(valueP);
                            items[k].Add(idValue, valueV);
                        }
                    }
                }
            }

            itemsID = new List<int>();
            for (int j = 1; j <= 5; j++)
            {
                var itemAndProbability = items[j];
                var random = UnityEngine.Random.Range(0, 10000);
                var proAdd = 0;
                foreach (var pair in itemAndProbability)
                {
                    var itemID = pair.Key;
                    if (itemID.id == 0)
                    {
                        break;
                    }
                    var probability = pair.Value;
                    proAdd += probability;
                    if (random <= proAdd)
                    {
                        itemsID.Add(itemID.id);
                        break;
                    }
                }
            }
        }

        public static string ConfigToString()
        {
            var capsuleStr = string.Empty;
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
            capsuleIdxs = ArrayUtils<int>.RandomSort(capsuleIdxs.ToArray()).ToList();
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
        public Capsule curCapsule
        {
            get
            {
                var capsuleStr = AccountManagerComponent.Instance.GetCapsuleStr();
                var list = Capsule.StringToList(capsuleStr);
                return list.FirstOrDefault();
            }
        }

        public void PopCapsule()
        {
            var capsuleStr = AccountManagerComponent.Instance.GetCapsuleStr();
            var list = Capsule.StringToList(capsuleStr);
            list.RemoveAt(0);
            list = ArrayUtils<Capsule>.RandomSort(list.ToArray()).ToList();
            var str = Capsule.ListToString(list);
            AccountManagerComponent.Instance.SetCapsuleStr(str);
        }

        public void OpenCapsule(HttpResponseDelegate callFunc = null)
        {
            AccountCharmComponent.Instance.ChangeCharm(-curCapsule.charmRequire, true, result =>
            {
                if (result)
                {
                    //日常任务：开启胶囊
                    DailyTaskManager.instance.AddValue(1, 9);

                    var texList = (from id in curCapsule.itemsID select ItemManageComponent.Instance.CreateItemByUID(id) into host where host != null select ResourceLoader.Instance.LoadItemTexture(host)).ToList();
                    PnlMainMenu.PnlMainMenu.Instance.item.SetAllItem(texList.ToArray());
                    PnlCapsuleOpen.PnlCapsuleOpen.Instance.onExit = new Action(() =>
                    {
                        PnlMainMenu.PnlMainMenu.Instance.item.FlyAllItem();
                    });

                    PopCapsule();
                }
                if (callFunc != null) callFunc(result);
            });
        }
    }
}