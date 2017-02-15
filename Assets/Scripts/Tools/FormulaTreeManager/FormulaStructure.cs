using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.Tools.FormulaTreeManager
{
    [System.Serializable]
    public class FormulaObject
    {
        [SerializeField]
        public string value;

        public FormulaObject(string v)
        {
            value = v;
        }

        public FormulaObject()
        {
            value = string.Empty;
        }

        public FormulaObject Operate(FormulaObject obj, FormulaNodeType type)
        {
            switch (type)
            {
                case FormulaNodeType.Add:
                    {
                        return this + obj;
                    }

                case FormulaNodeType.Minus:
                    {
                        return this - obj;
                    }
                case FormulaNodeType.Mul:
                    {
                        return this * obj;
                    }
                case FormulaNodeType.Divid:
                    {
                        return this / obj;
                    }
                case FormulaNodeType.Pow:
                    {
                        return Pow(obj);
                    }
                case FormulaNodeType.Sqrt:
                    {
                        return Log(new FormulaObject("2.0"));
                    }
                case FormulaNodeType.Log:
                    {
                        return Log(obj);
                    }
            }
            return this;
        }

        public FormulaObject Log(FormulaObject obj)
        {
            obj = new FormulaObject("1.0") / obj;
            return Pow(obj);
        }

        public FormulaObject Pow(FormulaObject obj)
        {
            var intResult = 0;
            var result = Mathf.Pow(float.Parse(obj.value), float.Parse(value));
            if (int.TryParse(value, out intResult))
            {
                intResult = (int)result;
            }
            return new FormulaObject(intResult.ToString());
        }

        public static FormulaObject operator +(FormulaObject lhs, FormulaObject rhs)
        {
            var intResult = 0;
            var floatResult = 0f;
            if (int.TryParse(lhs.value, out intResult))
            {
                intResult += int.Parse(rhs.value);
                return new FormulaObject(intResult.ToString());
            }
            else if (float.TryParse(lhs.value, out floatResult))
            {
                floatResult += float.Parse(rhs.value);
                return new FormulaObject(floatResult.ToString(CultureInfo.InvariantCulture));
            }
            return new FormulaObject(lhs.value + rhs.value);
        }

        public static FormulaObject operator -(FormulaObject lhs, FormulaObject rhs)
        {
            var opposite = new FormulaObject("-" + rhs.value);
            return lhs + opposite;
        }

        public static FormulaObject operator *(FormulaObject lhs, FormulaObject rhs)
        {
            var intResult = 0;
            var floatResult = 0f;
            if (int.TryParse(lhs.value, out intResult))
            {
                intResult *= int.Parse(rhs.value);
                return new FormulaObject(intResult.ToString());
            }
            else if (float.TryParse(lhs.value, out floatResult))
            {
                floatResult *= float.Parse(rhs.value);
                return new FormulaObject(floatResult.ToString(CultureInfo.InvariantCulture));
            }
            return new FormulaObject(lhs.value);
        }

        public static FormulaObject operator /(FormulaObject lhs, FormulaObject rhs)
        {
            var floatResult = 0f;
            if (float.TryParse(rhs.value, out floatResult))
            {
                floatResult = 1.0f / floatResult;
                Debug.Log(floatResult.ToString(CultureInfo.InvariantCulture));
                return lhs * new FormulaObject(floatResult.ToString(CultureInfo.InvariantCulture));
            }
            return new FormulaObject(lhs.value);
        }
    }

    [System.Serializable]
    public class FormulaNode
    {
        public string key;
        public FormulaNodeType type;

        [SerializeField]
        private string m_Uid;

        [SerializeField]
        private string m_ParentUid;

        [SerializeField]
        private List<string> m_ChildUids;

        public FormulaNode parent
        {
            get { return FormulaTree.instance.nodes.Find(n => n.m_Uid == m_ParentUid); }
        }

        public List<FormulaNode> childs
        {
            get { return m_ChildUids.Select(u => FormulaTree.instance.nodes.Find(n => n.m_Uid == u)).ToList(); }
        }

        public FormulaObject value
        {
            get
            {
                switch (type)
                {
                    case FormulaNodeType.Variable:
                        {
                            return GetSelfValue();
                        }
                    case FormulaNodeType.JsonData:
                        {
                            var idx = "0";
                            var parentArray = GetParent(node => node.type == FormulaNodeType.Array);
                            idx = parentArray.GetChild(child => child.key == "Index").value.value;
                            //return new FormulaObject((string)ConfigManager.ConfigManager.instance.pool[idx][key]);
                            return new FormulaObject(string.Empty);
                        }
                    case FormulaNodeType.Constance:
                        {
                            return new FormulaObject(key);
                        }
                    case FormulaNodeType.Array:
                        {
                            return new FormulaObject(key);
                        }
                    default:
                        {
                            return CalculateValue();
                        }
                }
            }
        }

        public FormulaNode()
        {
            key = string.Empty;
            m_Uid = System.Guid.NewGuid().ToString();
            m_ParentUid = string.Empty;
            m_ChildUids = new List<string>();
            type = FormulaNodeType.None;
        }

        public FormulaNode(string k, FormulaNode p, List<FormulaNode> list, FormulaNodeType t)
        {
            key = k;
            m_ParentUid = p.m_Uid;
            m_ChildUids = list.Select(l => l.m_Uid).ToList();
            type = t;
            p.childs.Add(this);
        }

        public void AddSelfToList()
        {
            FormulaTree.instance.nodes.Add(this);
        }

        public void RemoveSelfFromList()
        {
            FormulaTree.instance.nodes.Remove(this);
        }

        public void AddChild(string u)
        {
            m_ChildUids.Add(u);
        }

        public void RemvoeChild(string u)
        {
            m_ChildUids.RemoveAll(n => n == u);
        }

        public void SetParent(string u)
        {
            m_ParentUid = u;
        }

        public string GetUid()
        {
            return m_Uid;
        }

        private FormulaObject GetSelfValue()
        {
            var formulaObj = FormulaTree.instance.formulaObjs[key];
            return formulaObj;
        }

        private FormulaObject CalculateValue()
        {
            if (childs.Count == 0) return new FormulaObject("0");
            var result = new FormulaObject(childs[0].value.value.ToString());
            for (var i = 1; i < childs.Count; i++)
            {
                var child = childs[i];
                result = result.Operate(child.value, type);
            }
            return result;
        }

        private FormulaNode GetParent(Func<FormulaNode, bool> matchFunc)
        {
            var p = parent;
            if (p == null)
            {
                return p;
            }
            var isMatch = matchFunc(parent);
            if (!isMatch)
            {
                return p.GetParent(matchFunc);
            }
            return p;
        }

        private FormulaNode GetChild(Func<FormulaNode, bool> matchFunc)
        {
            if (childs == null)
            {
                return null;
            }
            foreach (var child in childs)
            {
                var isMatch = matchFunc(child);
                if (isMatch)
                {
                    return child;
                }
                else
                {
                    return child.GetChild(matchFunc);
                }
            }
            return null;
        }

        public bool isData
        {
            get
            {
                if (type == FormulaNodeType.Constance || type == FormulaNodeType.JsonData || type == FormulaNodeType.Variable || type == FormulaNodeType.Array)
                {
                    return true;
                }
                return false;
            }
        }
    }

    [System.Serializable]
    public class FormulaObjPair
    {
        public string key;
        public string type;
        public FormulaObject value;

        public FormulaObjPair()
        {
            key = string.Empty;
            type = string.Empty;
            value = new FormulaObject();
        }

        public FormulaObjPair(string k, string t, FormulaObject f)
        {
            key = k;
            type = t;
            value = f;
        }
    }

    [System.Serializable]
    public class FormulaObjDictionary
    {
        public List<FormulaObjPair> pairs;

        public FormulaObject this[string idx]
        {
            get
            {
                return (from formulaPair in pairs where formulaPair.key == idx select formulaPair.value).FirstOrDefault();
            }
            set
            {
                foreach (var formulaPair in pairs)
                {
                    if (formulaPair.key == idx)
                    {
                        formulaPair.value = value;
                        break;
                    }
                }
            }
        }
    }
}