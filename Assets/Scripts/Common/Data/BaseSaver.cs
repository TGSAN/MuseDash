using UnityEngine;
using System.Collections;
using DYUnityLib;
using LitJson;

/// <summary>
/// Base saver.
/// 数据存读器基类，负责处理本地、远程数据存读操作
/// 
/// SAVE_FILE_NAME	基于账号的游戏存储数据堆的根索引
/// saveData		基于SAVE_FILE_NAME索引下的全部存档数据
/// 
/// 该存档方案采用一维结构
/// 在根索引下，每个存储key对应一个数据结构
/// 例如
/// 关卡1，难度1的数据存储方式是 : key = stage + diff + id
/// { key : StageSaveData }
/// 
/// 强烈建议一个存储结构对应一个派生类
/// 派生类命名建议 模块+Saver
/// 相应的数据结构命名建议 模块+SaveData
/// 派生类所属 GameData/Saver/
/// 
/// 在应用模块建立派生类的 saver 统一命名对象，即可负责全部数据存读操作
/// </summary>
public class BaseSaver : DataPhaser {
	// this maybe instanded by account name
	private const string SAVE_FILE_NAME = "dyrhythmgirlfile";
	private static JsonData saveData = null;

	private object data;
	private string saveKey = null;
	public virtual void Save(string key = null, bool isForceSave = false) {
		if (key == null) {
			if (this.saveKey == null) {
				return;
			}

			key = this.saveKey;
		}

		JsonData jData = this.ObjectToJson (this.data);
		if (jData == null) {
			return;
		}

		saveData [key] = jData;
		if (isForceSave) {
			this.__Save ();
		}
	}

	public virtual void Load(string key = null) {
		if (key == null) {
			if (this.saveKey == null) {
				return;
			}

			key = this.saveKey;
		}

		if (saveData == null) {
			this.__Load ();
			if (saveData == null) {
				return;
			}
		}

		if (!saveData.Keys.Contains (key)) {
			return;
		}

		JsonData jData = saveData [key];
		if (jData == null) {
			return;
		}
		
		this.data = this.JsonToObject (jData, this.data);
	}

	public virtual void SetData(System.Object data) {
		this.data = data;
	}

	public virtual void Clear() {
		GameLogic.GameGlobal.gConfigLoader.ClearPrefs ();
	}

	public virtual System.Object GetDataObject() {
		return this.data;
	}

	public virtual void SetSaveKey(string key) {
		this.saveKey = key;
	}

	private void __Load() {
		saveData = GameLogic.GameGlobal.gConfigLoader.LoadPrefs (SAVE_FILE_NAME);
	}
	
	private void __Save() {
		if (saveData == null) {
			saveData = new JsonData ();
		}

		GameLogic.GameGlobal.gConfigLoader.SavePrefs (SAVE_FILE_NAME, saveData);
	}
}