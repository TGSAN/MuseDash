using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public struct FormulaParamCondictionStruct
{
    public int condictionType;

    public int condictionKeyIndex;
    public string condictionKey;

    public float condictionValue;
    public float plus;
}

[System.Serializable]
public struct FormulaParamStruct
{
    public int idx;
    public int dataSourceType;
    public int dataSourceIndex;
    public float dataSourceValue;
    public string dataSourceName;

    public bool collapsed;
    public string paramname;

    // FormulaParamCondictionStruct only
    public FormulaParamCondictionStruct[] condictions;

    public FormulaParamStruct(int idx = 0)
    {
        this.idx = idx;

        this.dataSourceType = 0;
        this.dataSourceIndex = 0;
        this.dataSourceValue = 0f;
        this.dataSourceName = null;

        this.collapsed = false;
        this.paramname = null;
        this.condictions = null;
    }
}

[System.Serializable]
public struct FormulaStruct
{
    public int idx;
    public int typeIdx;
    public string name;
    public string formula;
    public FormulaParamStruct[] fparams;
}

[System.Serializable]
public struct FormulaHostStruct
{
    public string name;
    public string fileName;

    public bool collapsedFormula;
    public bool collapsedSign;
    public bool collapsedComponent;

    public int[] formulaSet;
    public int[] singSet;
    public float[] signValueSet;
    public string[] componentNames;
}

[System.Serializable]
public class FormulaData : MonoBehaviour
{
    private static string PATH = "Prefabs/FormulaData";
    private static GameObject dataObject;
    private static FormulaData instance;

    [SerializeField]
    public string componentModulePath;

    [Tooltip("动态参数")]
    public string[] DynamicParams;

    public string[] ConfigNames;
    public string[] FormulaTypeNames;
    public FormulaStruct[] Formulas;
    public FormulaHostStruct[] Hosts;
    public List<string> ConfigReaders;

    public static FormulaData Instance
    {
        get
        {
            if (instance == null)
            {
                dataObject = Resources.Load(PATH) as GameObject;
                instance = dataObject.GetComponent<FormulaData>();
            }

            return instance;
        }
    }

    public static GameObject GetDataObject()
    {
        return dataObject;
    }

    public void AddStringListItem(ref string item, ref string[] strList)
    {
        List<string> _list;
        if (strList != null && strList.Length > 0)
        {
            _list = strList.ToList();
        }
        else
        {
            _list = new List<string>();
        }

        _list.Add(item);

        strList = _list.ToArray();
    }

    public void DelStringListItem(int idx, ref string[] strList)
    {
        if (strList == null)
        {
            return;
        }

        if (strList.Length <= idx)
        {
            return;
        }

        List<string> _list = strList.ToList();
        _list.RemoveAt(idx);
        strList = _list.ToArray();
    }

    public void AddIntListItem(int item, ref int[] intList)
    {
        List<int> _list;
        if (intList != null && intList.Length > 0)
        {
            _list = intList.ToList();
        }
        else
        {
            _list = new List<int>();
        }

        _list.Add(item);

        intList = _list.ToArray();
    }

    public void DelIntListItem(int idx, ref int[] intList)
    {
        if (intList == null)
        {
            return;
        }

        if (intList.Length <= idx)
        {
            return;
        }

        List<int> _list = intList.ToList();
        _list.RemoveAt(idx);
        intList = _list.ToArray();
    }

    public void AddFloatListItem(float item, ref float[] floatList)
    {
        List<float> _list;
        if (floatList != null && floatList.Length > 0)
        {
            _list = floatList.ToList();
        }
        else
        {
            _list = new List<float>();
        }

        _list.Add(item);

        floatList = _list.ToArray();
    }

    public void DelFloatListItem(int idx, ref float[] floatList)
    {
        if (floatList == null)
        {
            return;
        }

        if (floatList.Length <= idx)
        {
            return;
        }

        List<float> _list = floatList.ToList();
        _list.RemoveAt(idx);
        floatList = _list.ToArray();
    }

    public void AddGameObjectListItem(ref GameObject item, ref GameObject[] objList)
    {
        List<GameObject> _list;
        if (objList != null && objList.Length > 0)
        {
            _list = objList.ToList();
        }
        else
        {
            _list = new List<GameObject>();
        }

        _list.Add(item);

        objList = _list.ToArray();
    }

    public void DelGameObjectListItem(int idx, ref GameObject[] objList)
    {
        if (objList == null)
        {
            return;
        }

        if (objList.Length <= idx)
        {
            return;
        }

        List<GameObject> _list = objList.ToList();
        _list.RemoveAt(idx);
        objList = _list.ToArray();
    }

    public void AfterSave()
    {
        instance = null;
    }

    // -----------------  For formula data ---------------------
    public FormulaStruct GetFormulaById(int idx)
    {
        if (this.Formulas.Length <= idx)
        {
            for (int i = this.Formulas.Length; i < idx + 2; i++)
            {
                FormulaStruct item = new FormulaStruct();
                this.AddFormula(ref item);
            }
        }

        return this.Formulas[idx];
    }

    public void AddFormula(ref FormulaStruct item)
    {
        List<FormulaStruct> _list;
        if (this.Formulas != null && this.Formulas.Length > 0)
        {
            _list = this.Formulas.ToList();
        }
        else
        {
            _list = new List<FormulaStruct>();
        }

        _list.Add(item);

        this.Formulas = _list.ToArray();
    }

    // -----------------  For host data ---------------------
    public void AddHost(ref FormulaHostStruct item)
    {
        List<FormulaHostStruct> _list;
        if (this.Hosts != null && this.Hosts.Length > 0)
        {
            _list = this.Hosts.ToList();
        }
        else
        {
            _list = new List<FormulaHostStruct>();
        }

        _list.Add(item);

        this.Hosts = _list.ToArray();
    }

    public void DelHost(int idx)
    {
        if (this.Hosts == null)
        {
            return;
        }

        if (this.Hosts.Length <= idx)
        {
            return;
        }

        List<FormulaHostStruct> _list = this.Hosts.ToList();
        _list.RemoveAt(idx);
        this.Hosts = _list.ToArray();
    }
}