using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;


public class SpineModeInspector : EditorWindow {
	private static string SAVE_BUTTON = "Save";
	private static string ADD_BUTTON = "+";
	private static string DEL_BUTTON = "-";

	[MenuItem("RHY/Spine扩展/node类型模式编辑")]
	static void Init () {
		SpineModeInspector window = (SpineModeInspector)EditorWindow.GetWindow (typeof(SpineModeInspector));
		window.Show ();
	}

	private void MkDesItem() {
		EditorGUILayout.BeginHorizontal ();
		EditorGUILayout.LabelField ("Mode Descript");
		if (GUILayout.Button (ADD_BUTTON)) {
			string emptyDes = "";
			EditorData.Instance.AddStringListItem (ref emptyDes, ref EditorData.Instance.SpineModeDes);
			EditorData.Instance.AddStringListItem (ref emptyDes, ref EditorData.Instance.SpineModeName);
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
		EditorData.Instance.SpineModeDes [idx] = EditorGUILayout.TextField ("", EditorData.Instance.SpineModeDes [idx]);

		if (idx >= EditorData.Instance.SpineModeName.Length) {
			EditorData.Instance.AddStringListItem (ref emptyDes, ref EditorData.Instance.SpineModeName);
		}
		EditorData.Instance.SpineModeName [idx] = EditorGUILayout.TextField ("", EditorData.Instance.SpineModeName [idx]);
		if (GUILayout.Button (DEL_BUTTON)) {
			EditorData.Instance.DelStringListItem (idx, ref EditorData.Instance.SpineModeDes);
			EditorData.Instance.DelStringListItem (idx, ref EditorData.Instance.SpineModeName);
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
		EditorApplication.SaveAssets ();
		EditorData.Instance.AfterSave ();
	}

	void OnGUI() {
		EditorGUILayout.BeginVertical ();
		this.MkDesItem ();
		if (EditorData.Instance.SpineModeDes != null) {
			for (int i = 0; i < EditorData.Instance.SpineModeDes.Length; i++) {
				this.MkKeyItem (i);
			}
		}

		EditorGUILayout.EndVertical ();
	}
}