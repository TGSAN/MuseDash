using UnityEngine;
using System.Collections;
using LitJson;

/// <summary>
/// Base data reader.
/// 配置读取基类
/// 对于派生类，必须自定义内容包括：
/// 配置名：
/// 	private static string CONFIG_NAME = “configname”;
/// 简化版单例：
/// 	private Class(){}
/// 	public static Class Instance = new Class();
/// Init方法重载：
/// 	具体可参照ToyConfigReader等示例
/// 
/// 自定义配置表对应的数据结构
/// 数据结构成员名必须和配置表对应列名完全一致
/// 
/// 强烈建议一个配置表对应一个派生类
/// 派生类命名建议 模块+ConfigReader
/// 相应的数据结构命名建议 模块+ConfigData
/// 派生类所属 GameData/ConfigReader/
/// 
/// 应用对象建议为单例
/// </summary>
public class BaseConfigReader : DataPhaser {
	private ArrayList data = new ArrayList();

	public virtual void Init() {
	}

	public virtual void Init(ref string filename) {
	}

	public virtual ArrayList GetData() {
		return this.data;
	}

	public virtual object GetData(int idx) {
		return null;
	}

	public virtual ArrayList GetData(ref string key) {
		return null;
	}

	public virtual int GetDataCount() {
		return this.data.Count;
	}

	public virtual int GetDataCount(int idx) {
		return -1;
	}

	public virtual void SetData(ArrayList list) {
		this.data = list;
	}

	public virtual void ClearData() {
		this.data.Clear ();
	}

	public virtual int GetDataCount(ref string key) {
		return -1;
	}

	public void Add(System.Object obj) {
		this.data.Add (obj);
	}

	public JsonData GetJsonConfig(string filename) {
	JsonData cfg = GameLogic.GameGlobal.gConfigLoader.LoadAsJsonData (filename);
		if (cfg == null) {
			return null;
		}

		JsonData _data = GameLogic.GameGlobal.gConfigLoader.JsonFromExcelParse (cfg);
		return _data;
	}

	public System.Object ConfigToObject(JsonData jsonData, System.Object obj) {
		return this.JsonToObject (jsonData, obj);
	}
}