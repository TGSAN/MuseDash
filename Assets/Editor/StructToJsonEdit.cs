using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;
using System.Collections;

struct SampleStruct {
	public string action;
	public string sign;
	public float value;
}

/// <summary>
/// Struct to json edit.
/// 把需要导出成json格式的struct在上面的SampleStruct中设定好
/// 编译完成后，在MD中打开工具，即可得到对应的json字符串
/// 复制该字符串作为配置用的填写格式给策划
/// 然后通过配置解释模块（各种ConfigReader）即可读出完全对应的struct-struct
/// </summary>
public class StructToJsonEdit : EditorWindow {
	// Data about.
	DataPhaser dataPhaser = new DataPhaser();
	[MenuItem("MD/配置用复杂数据结处理工具")]
	static void Init () {
		StructToJsonEdit window = (StructToJsonEdit)EditorWindow.GetWindow (typeof(StructToJsonEdit));
		window.Show ();
	}

	void OnGUI() {
		EditorGUILayout.BeginVertical ();
		this.MkTitle ();
		this.MkJsonString ();
		EditorGUILayout.EndVertical ();
	}

	private void MkTitle() {
		EditorGUILayout.BeginVertical ();
		EditorGUILayout.LabelField ("当配置表格需要复杂结构时，拿着这个编辑工具找程序.");
		EditorGUILayout.EndVertical ();
	}

	private void MkJsonString() {
		EditorGUILayout.BeginVertical ();
		LitJson.JsonData jData = this.dataPhaser.ObjectToJson (new SampleStruct (), "");
		string _str = jData.ToJson ();
		//_str.Replace ('"', ' ');
		EditorGUILayout.SelectableLabel (_str);
		EditorGUILayout.EndVertical ();
	}
}