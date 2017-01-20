using UnityEditor;
using UnityEngine;
using System.Collections;

// Example script with properties.
public class EditorExample : MonoBehaviour {

	public GameObject gameObject;
	public int m_int;

	// ...other code...
}


// Custom Editor the "old" way by modifying the script variables directly.
// No handling of multi-object editing, undo, and prefab overrides!
[CustomEditor (typeof(EditorExample))]
public class ExampleEditor : Editor {

	public override void OnInspectorGUI () {
		EditorExample mp = (EditorExample)target;

		mp.m_int = EditorGUILayout.IntSlider ("IntSlider", mp.m_int, 0, 100);
		ProgressBar (mp.m_int / 100.0f, "ProgressBar");

		bool allowSceneObjects = !EditorUtility.IsPersistent (target);
		mp.gameObject = (GameObject)EditorGUILayout.ObjectField ("GameObject", mp.gameObject, typeof(GameObject), allowSceneObjects);
	}

	// Custom GUILayout progress bar.
	void ProgressBar (float value, string label) {
		// Get a rect for the progress bar using the same margins as a textfield:
		Rect rect = GUILayoutUtility.GetRect (18, 18, "TextField");
		EditorGUI.ProgressBar (rect, value, label);
		EditorGUILayout.Space ();
	}
}