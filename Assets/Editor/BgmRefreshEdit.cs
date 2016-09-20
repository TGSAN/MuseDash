using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;
using System.Collections;
using System.Text.RegularExpressions;
using System.IO;

/// <summary>
/// Formula edit.
/// </summary>
public class BgmRefreshEdit : EditorWindow {
	private const string PATH = "路径";
	private const string DES1 = "是否后台加载";

	private bool isLoadInBackground = true;
	private string path = "stage/";


	[MenuItem("RHY/stage bgm 统一刷新")]
	static void Init () {
		BgmRefreshEdit window = (BgmRefreshEdit)EditorWindow.GetWindow (typeof(BgmRefreshEdit));
		window.Show ();
	}

	void OnGUI() {
		EditorGUILayout.BeginVertical ();

		this.path = EditorGUILayout.TextField (PATH, this.path);
		this.isLoadInBackground = EditorGUILayout.Toggle (DES1, this.isLoadInBackground);

		if (GUILayout.Button ("Reflesh")) {
			this.RefleshAsset ();
		}

		EditorGUILayout.EndVertical ();
	}

	private void __RefleshAsset(string path) {
		AudioImporter sda = AudioImporter.GetAtPath (path) as AudioImporter;
		if (sda == null) {
			Debug.Log ("Null ogg path : " + path);
			return;
		}

		if (sda.loadInBackground == this.isLoadInBackground) {
			return;
		}

		sda.loadInBackground = this.isLoadInBackground;
		sda.SaveAndReimport ();

		EditorUtility.SetDirty(sda);

		//EditorApplication.SaveAssets ();
	}

	private void RefleshAsset() {
		EditorSettings.serializationMode = SerializationMode.ForceText;
		//string path = AssetDatabase.GetAssetPath (Selection.activeObject);
		string path = Application.dataPath + "/Resources/" + this.path;
		if (string.IsNullOrEmpty (path)) {
			return;
		}

		Debug.Log ("Reflesh all ogg in " + path);

		int startIndex = 0;
		string guid = AssetDatabase.AssetPathToGUID (path);
		List<string> withoutExtensions = new List<string> (){ ".ogg" };
		string[] files = Directory.GetFiles (path, "*.*", SearchOption.AllDirectories).Where (s => withoutExtensions.Contains (Path.GetExtension (s).ToLower ())).ToArray ();

		Dictionary<string, string> _nameToPath = new Dictionary<string, string> ();
		EditorApplication.update = delegate() {
			string file = files [startIndex];

			bool isCancel = EditorUtility.DisplayCancelableProgressBar ("匹配资源中", file, (float)startIndex / (float)files.Length);
			if (Regex.IsMatch (File.ReadAllText (file), guid)) {
				string[] _pn = file.Split ('/');
				string _fname = _pn [_pn.Length - 1];
				string _fShortName = _fname.Split ('.') [0];

				string _shortPath = file.Replace (path, "");
				_shortPath = _shortPath.Replace ("/Resources/", "");
				_shortPath = _shortPath.Replace (_fname, "");
				/*
				SkeletonDataAsset sda;
				sda.GetAnimationStateData().DefaultMix = 0f;
				sda.scale = _scale;
				*/
				string _resPath = "Assets/Resources/" + this.path + _shortPath + _fShortName + ".ogg";
				this.__RefleshAsset(_resPath);
				Debug.Log (file, AssetDatabase.LoadAssetAtPath<Object> (GetRelativeAssetsPath (file)));
			}

			startIndex++;
			if (isCancel || startIndex >= files.Length) {
				EditorUtility.ClearProgressBar ();
				EditorApplication.update = null;
				startIndex = 0;
				Debug.Log (".ogg搜索完毕");
			}
		};
	}

	static private string GetRelativeAssetsPath(string path) {
		return "Assets" + Path.GetFullPath (path).Replace (Path.GetFullPath (Application.dataPath), "").Replace ('\\', '/');
	}
}