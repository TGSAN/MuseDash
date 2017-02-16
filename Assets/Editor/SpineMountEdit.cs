using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using Spine;
using Spine.Unity;

[CustomEditor(typeof(SpineMountController))]
public class SpineMountEdit : Editor {
	private string[] sklNames;
	private GameObject obj;
	private string STR_EMPTY = "";
	private string DEC = "-";

	public override void OnInspectorGUI() {
		this.obj = null;
		if (target != null) {
			this.obj = ((SpineMountController)target).gameObject;
		}

		if (this.obj == null) {
			return;
		}

		SpineMountController spa = this.obj.GetComponent<SpineMountController> ();
		if (spa == null) {
			return;
		}

		this.InitSkeletonName ();

		EditorGUILayout.BeginHorizontal ();
		GUILayout.Label ("SpineMountController Settings", EditorStyles.boldLabel);
		if (GUILayout.Button ("Add Mount")) {
			this.AddMount ();
			return;
		}
		
		EditorGUILayout.EndHorizontal ();

		this.InitMountList ();
	}

	private void InitMountList() {
		SpineMountController spa = this.obj.GetComponent<SpineMountController> ();
		if (spa == null) {
			return;
		}

		EditorGUILayout.BeginVertical ();

		for (int i = 0; i < spa.DataCount(); i++) {
			this.InitMountItem (i);
		}

		EditorGUILayout.EndVertical ();
	}

	private void InitMountItem(int idx) {
		SpineMountController spa = this.obj.GetComponent<SpineMountController> ();
		if (spa == null) {
			return;
		}

		GUILayoutOption opt = GUILayout.MaxWidth (150);
		SkeletonMountData data = spa.GetData (idx);
		EditorGUILayout.BeginHorizontal ();
		// bone list
		data.actionId = EditorGUILayout.Popup (STR_EMPTY, data.actionId, this.sklNames, opt);
		// perfabs instance
		data.instance = EditorGUILayout.ObjectField (STR_EMPTY, data.instance, typeof(GameObject), true, opt) as GameObject;
		// init module
		data.moduleId = EditorGUILayout.Popup (STR_EMPTY, data.moduleId, EditorData.Instance.SpineModeDes, opt);
		// del button
		if (GUILayout.Button (DEC)) {
			this.DelMount (idx);
			return;
		}

		spa.SetData (idx, data);
		EditorGUILayout.EndHorizontal ();
	}

	private void AddMount() {
		SpineMountController spa = this.obj.GetComponent<SpineMountController> ();
		if (spa == null) {
			return;
		}

		spa.AddData ();
		this.Repaint ();
	}

	private void DelMount(int idx) {
		SpineMountController spa = this.obj.GetComponent<SpineMountController> ();
		if (spa == null) {
			return;
		}

		spa.DelData (idx);
		this.Repaint ();
	}


	private void InitSkeletonName() {
		ExposedList<Bone> temp = this.obj.GetComponent<SkeletonAnimation> ().skeleton.Bones;
		this.sklNames = new string[temp.Count];
		for (int i = 0, max = temp.Count; i < max; i++) {
			this.sklNames [i] = temp.Items [i].Data.Name;
		}
	}
}