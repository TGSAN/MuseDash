using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;


public class SpineEventInspector : EditorWindow {
	private static string SAVE_BUTTON = "Save";
	private static string ADD_BUTTON = "+";
	private static string DEL_BUTTON = "-";

	[MenuItem("MD/Spine扩展/动作事件编辑")]
	static void Init () {
		SpineEventInspector window = (SpineEventInspector)EditorWindow.GetWindow (typeof(SpineEventInspector));
		window.Show ();
	}

	private void MkDesItem() {
		EditorGUILayout.BeginHorizontal ();
		EditorGUILayout.LabelField ("Event Descript");
		if (GUILayout.Button (ADD_BUTTON)) {
			string emptyDes = "";
			EditorData.Instance.AddStringListItem (ref emptyDes, ref EditorData.Instance.SpineEventDes);
			EditorData.Instance.AddStringListItem (ref emptyDes, ref EditorData.Instance.SpineEventName);
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
		string emptyDes = "";
		EditorGUILayout.BeginHorizontal ();
		EditorData.Instance.SpineEventDes [idx] = EditorGUILayout.TextField ("", EditorData.Instance.SpineEventDes [idx]);
		if (idx >= EditorData.Instance.SpineEventName.Length) {
			EditorData.Instance.AddStringListItem (ref emptyDes, ref EditorData.Instance.SpineEventName);
		}
		EditorData.Instance.SpineEventName [idx] = EditorGUILayout.TextField ("", EditorData.Instance.SpineEventName [idx]);
		if (GUILayout.Button (DEL_BUTTON)) {
			EditorData.Instance.DelStringListItem(idx, ref EditorData.Instance.SpineEventDes);
			EditorData.Instance.DelStringListItem(idx, ref EditorData.Instance.SpineEventName);
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
		if (EditorData.Instance.SpineEventDes != null) {
			for (int i = 0; i < EditorData.Instance.SpineEventDes.Length; i++) {
				this.MkKeyItem (i);
			}
		}

		EditorGUILayout.EndVertical ();
	}
}