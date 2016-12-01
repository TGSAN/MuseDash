using DYUnityLib;
using LitJson;
using System.Collections;
using UnityEngine;

/// <summary>
/// Data phaser.
/// 数据处理基类
/// 自带struct对JsonData的相互转换功能
/// 已支持嵌套struct, 数组, ArrayList
/// </summary>
public class DataPhaser
{
    private const char JSON_START = '{';

    public JsonData ObjectToJson(System.Object obj, string defaultString = null)
    {
        System.Object box = obj;
        JsonData jsonData = new JsonData();
        System.Reflection.MemberInfo[] memberInfos = obj.GetType().GetMembers();
        for (int j = 0; j < memberInfos.Length; j++)
        {
            System.Reflection.MemberInfo m = memberInfos[j];
            System.Reflection.FieldInfo fieldInfo = obj.GetType().GetField(m.Name);
            if (fieldInfo == null)
            {
                continue;
            }

            System.Object objValue = fieldInfo.GetValue(box);
            if (!fieldInfo.FieldType.IsValueType)
            {
                if (fieldInfo.FieldType == typeof(string))
                {
                    if (objValue == null)
                    {
                        jsonData[m.Name] = defaultString;
                        continue;
                    }

                    jsonData[m.Name] = objValue.ToString();
                    continue;
                }
                else if (fieldInfo.FieldType == typeof(JsonData))
                {
                    if (objValue == null)
                    {
                        continue;
                    }
                }
                else
                {
                    continue;
                }
            }

            jsonData[m.Name] = objValue.ToString();
        }

        return jsonData;
    }

    public System.Object JsonToObject(JsonData jsonData, System.Object obj)
    {
        System.Object box = obj;
        System.Reflection.MemberInfo[] memberInfos = obj.GetType().GetMembers();
        for (int j = 0; j < memberInfos.Length; j++)
        {
            System.Reflection.MemberInfo m = memberInfos[j];
            if (!jsonData.Keys.Contains(m.Name))
            {
                // Debug.Log ("------ no such json key " + m.Name);
                continue;
            }
            string strValue = jsonData[m.Name].ToString();
            System.Reflection.FieldInfo fieldInfo = obj.GetType().GetField(m.Name);
            if (fieldInfo.FieldType == typeof(string))
            {
                fieldInfo.SetValue(box, strValue);
                continue;
            }

            if (fieldInfo.FieldType == typeof(JsonData))
            {
                fieldInfo.SetValue(box, jsonData[m.Name]);
                continue;
            }

            if (fieldInfo.FieldType == typeof(int))
            {
                fieldInfo.SetValue(box, int.Parse(strValue));
                continue;
            }

            if (fieldInfo.FieldType == typeof(uint))
            {
                fieldInfo.SetValue(box, uint.Parse(strValue));
                continue;
            }

            if (fieldInfo.FieldType == typeof(float))
            {
                fieldInfo.SetValue(box, float.Parse(strValue));
                continue;
            }

            if (fieldInfo.FieldType == typeof(double))
            {
                fieldInfo.SetValue(box, double.Parse(strValue));
                continue;
            }

            if (fieldInfo.FieldType == typeof(decimal))
            {
                fieldInfo.SetValue(box, decimal.Parse(strValue));
                continue;
            }

            if (fieldInfo.FieldType == typeof(bool))
            {
                fieldInfo.SetValue(box, bool.Parse(strValue));
                continue;
            }

            if (strValue.Length > 0 && strValue[0] == JSON_START)
            {
                string tName = fieldInfo.FieldType.Name;
                JsonData jd = JsonMapper.ToObject(strValue);
                System.Object _valueArray = this.JsonToDataArray(tName, jd);
                if (_valueArray != null)
                {
                    fieldInfo.SetValue(box, _valueArray);
                    continue;
                }

                System.Object _obj = fieldInfo.FieldType.Assembly.CreateInstance(tName);
                System.Object _valueObj = this.JsonToObject(jd, _obj);
                fieldInfo.SetValue(box, _valueObj);
                continue;
            }
        }

        return box;
    }

    /// <summary>
    /// Jsons to data array.
    /// 数组配置格式 : {1:xxx,2:xxx,...}
    /// 数组必须顺序，中间不能跳过顺序，数组开始索引是0
    /// 返回的Object根据需要转换成数组类型，例如int[] v = (int[])obj
    ///
    /// ps : 使用值类型名作为类型判断分类不是个有逼格的做法，不过不想把代码写的过于难懂就算了
    /// </summary>
    /// <returns>The to data array.</returns>
    /// <param name="typeName">Type name.</param>
    /// <param name="jData">J data.</param>
    public System.Object JsonToDataArray(string typeName, JsonData jData)
    {
        if (jData.Count <= 0)
        {
            return null;
        }

        if (typeName == "System.Int32[]" || typeName == "System.Int16[]" || typeName == "System.Int64[]")
        {
            int[] __valueArray = new int[jData.Count];
            for (int i = 0; i < jData.Count; i++)
            {
                __valueArray[i] = int.Parse(jData[i.ToString()].ToString());
            }

            return __valueArray;
        }

        if (typeName == "System.UInt32[]" || typeName == "System.UInt16[]" || typeName == "System.UInt64[]")
        {
            uint[] __valueArray = new uint[jData.Count];
            for (int i = 0; i < jData.Count; i++)
            {
                __valueArray[i] = uint.Parse(jData[i.ToString()].ToString());
            }

            return __valueArray;
        }

        if (typeName == "System.Char[]")
        {
            char[] __valueArray = new char[jData.Count];
            for (int i = 0; i < jData.Count; i++)
            {
                __valueArray[i] = char.Parse(jData[i.ToString()].ToString());
            }

            return __valueArray;
        }

        if (typeName == "System.String[]")
        {
            string[] __valueArray = new string[jData.Count];
            for (int i = 0; i < jData.Count; i++)
            {
                __valueArray[i] = jData[i.ToString()].ToString();
            }

            return __valueArray;
        }

        if (typeName == "System.Boolean[]")
        {
            bool[] __valueArray = new bool[jData.Count];
            for (int i = 0; i < jData.Count; i++)
            {
                __valueArray[i] = bool.Parse(jData[i.ToString()].ToString());
            }

            return __valueArray;
        }

        if (typeName == "System.Decimal[]")
        {
            decimal[] __valueArray = new decimal[jData.Count];
            for (int i = 0; i < jData.Count; i++)
            {
                __valueArray[i] = decimal.Parse(jData[i.ToString()].ToString());
            }

            return __valueArray;
        }

        // float
        if (typeName == "System.Single[]")
        {
            float[] __valueArray = new float[jData.Count];
            for (int i = 0; i < jData.Count; i++)
            {
                __valueArray[i] = float.Parse(jData[i.ToString()].ToString());
            }

            return __valueArray;
        }

        if (typeName == "System.Double[]")
        {
            double[] __valueArray = new double[jData.Count];
            for (int i = 0; i < jData.Count; i++)
            {
                __valueArray[i] = double.Parse(jData[i.ToString()].ToString());
            }

            return __valueArray;
        }

        if (typeName == "System.Collections.ArrayList")
        {
            ArrayList __valueArray = new ArrayList();
            for (int i = 0; i < jData.Count; i++)
            {
                __valueArray.Add(jData[i.ToString()].ToString());
            }

            return __valueArray;
        }

        return null;
    }
}