using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;


public class SpineActionInspector : EditorWindow {
	private static string SAVE_BUTTON = "Save";
	private static string ADD_BUTTON = "+";
	private static string DEL_BUTTON = "-";

	[MenuItem("MD/Spine扩展/动作关键字编辑")]
	static void Init () {
		SpineActionInspector window = (SpineActionInspector)EditorWindow.GetWindow (typeof(SpineActionInspector));
		window.Show ();
	}

	private void MkDesItem() {
		EditorGUILayout.BeginHorizontal ();
		EditorGUILayout.LabelField ("Action Key");
		EditorGUILayout.LabelField ("Descript");
		if (GUILayout.Button (ADD_BUTTON)) {
			string empty = "";
			string emptyDes = "";
			EditorData.Instance.AddStringListItem (ref empty, ref EditorData.Instance.SpineActionKeys);
			EditorData.Instance.AddStringListItem (ref emptyDes, ref EditorData.Instance.SpineActionDes);
			this.Repaint ();
			return;
		}

		if (GUILayout.Button (SAVE_BUTTON)) {
			this.Save ();
			return;
		}

		EditorGUILayout.EndHorizontal ();
	}

	private void MkKeyItem(int idx) {
		EditorGUILayout.BeginHorizontal ();
		EditorData.Instance.SpineActionKeys [idx] = EditorGUILayout.TextField ("", EditorData.Instance.SpineActionKeys [idx]);
		EditorData.Instance.SpineActionDes [idx] = EditorGUILayout.TextField ("", EditorData.Instance.SpineActionDes [idx]);
		if (GUILayout.Button (DEL_BUTTON)) {
			EditorData.Instance.DelStringListItem(idx, ref EditorData.Instance.SpineActionKeys);
			EditorData.Instance.DelStringListItem(idx, ref EditorData.Instance.SpineActionDes);
			this.Repaint ();
			return;
		}

		EditorGUILayout.EndHorizontal ();
	}

	private void Save() {
		GameObject dataObject = EditorData.GetDataObject ();
		if (dataObject == null) {
			Debug.Log ("Editor Data prefab is null.");
			return;
		}
		
		//PrefabUtility.ReplacePrefab (this.gameObject, dataObject);
		PropertyModification[] temp = new PropertyModification[1];
		temp [0] = new PropertyModification ();
		temp [0].target = dataObject;
		PrefabUtility.SetPropertyModifications (dataObject, temp);
		AssetDatabase.SaveAssets ();
		EditorData.Instance.AfterSave ();
	}

	void OnGUI() {
		EditorGUILayout.BeginVertical ();
		this.MkDesItem ();
		if (EditorData.Instance.SpineActionKeys != null) {
			for (int i = 0; i < EditorData.Instance.SpineActionKeys.Length; i++) {
				this.MkKeyItem (i);
			}
		}

		EditorGUILayout.EndVertical ();
	}
}