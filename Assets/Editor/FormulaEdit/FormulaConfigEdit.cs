using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Formula edit.
/// </summary>
public class FormulaConfigEdit : EditorWindow
{
    private Vector2 scorllVecAction;
    private bool isSaving = false;

    [MenuItem("RHY/配置表管理")]
    private static void FindAllConfig()
    {
        /*#if UNITY_STANDALONE_WIN
                        if (true) return;
        #endif*/

        EditorSettings.serializationMode = SerializationMode.ForceText;
        //string path = AssetDatabase.GetAssetPath (Selection.activeObject);
        string path = Application.dataPath;
        if (string.IsNullOrEmpty(path))
        {
            return;
        }

        Debug.Log("Find all .json in " + path);
        string guid = AssetDatabase.AssetPathToGUID(path);
        List<string> withoutExtensions = new List<string>() { ".json" };
        string[] files = Directory.GetFiles(Application.dataPath, "*.*", SearchOption.AllDirectories).Where(s => withoutExtensions.Contains(Path.GetExtension(s).ToLower())).ToArray();
        int startIndex = 0;

        Dictionary<string, string> _nameToPath = new Dictionary<string, string>();
        EditorApplication.update = delegate ()
        {
            string file = files[startIndex];

            bool isCancel = EditorUtility.DisplayCancelableProgressBar("匹配资源中", file, (float)startIndex / (float)files.Length);
            if (Regex.IsMatch(File.ReadAllText(file), guid))
            {
                string[] _pn = file.Split('/');
                string _fname = _pn[_pn.Length - 1];
                string _fShortName = _fname.Split('.')[0];
                if (_nameToPath.ContainsKey(_fShortName))
                {
                    Debug.Log("有重名配置" + _fShortName + ", 请修改配置名");
                }
                else
                {
                    string _shortPath = file.Replace(path, "");
                    _shortPath = _shortPath.Replace("/Resources/", "");
                    _shortPath = _shortPath.Replace(_fname, "");
                    var _finalPath = _shortPath + _fShortName;
#if UNITY_STANDALONE_WIN
                    _finalPath = _shortPath.Replace("\\Resources\\", "");
                    _finalPath = _finalPath.Replace("\\", "/");
                    _finalPath = _finalPath.Split('.')[0];
                    var splitPath = _finalPath.Split('/');
                    _fShortName = splitPath[splitPath.Length - 1];
#endif
                    try
                    {
                        Debug.Log(_finalPath);
                        TextAsset txt = (TextAsset)Resources.Load(_finalPath);
                        if (txt != null)
                        {
                            // 带 spine 内容的直接当它骨骼动画过滤掉
                            if (!txt.text.Contains("spine"))
                            {
                                var myPath = _finalPath;
#if UNITY_STANDALONE_WIN
                                myPath = "";
                                var tmpSplit = _finalPath.Split('/');
                                for (int i = 0; i < tmpSplit.Length - 1; i++)
                                {
                                    myPath += tmpSplit[i] + "/";
                                }
#else
                                //myPath = myPath.Replace(_fShortName, "");
                                int _ridx = myPath.LastIndexOf('/');
                                myPath = myPath.Substring(0, _ridx + 1);
#endif
                                _nameToPath[_fShortName] = myPath;
                                Debug.Log(file, AssetDatabase.LoadAssetAtPath<Object>(GetRelativeAssetsPath(file)));
                            }
                        }
                    }
                    catch (System.InvalidCastException)
                    {
                        //Debug.Log ("配置" + _fShortName + "分析有误");
                    }
                }
            }

            startIndex++;
            if (isCancel || startIndex >= files.Length)
            {
                EditorUtility.ClearProgressBar();
                EditorApplication.update = null;
                startIndex = 0;
                Debug.Log("配置搜索完毕，生成运行用文件");
                FormulaGenerator.WriteConfigKey(_nameToPath);
                Debug.Log("配置处理完毕");
            }
        };
    }

    static private string GetRelativeAssetsPath(string path)
    {
        return "Assets" + Path.GetFullPath(path).Replace(Path.GetFullPath(Application.dataPath), "").Replace('\\', '/');
    }

    /*
	private void MkSignSet() {
		if (FormulaData.Instance.ConfigNames == null || FormulaData.Instance.ConfigNames.Length <= 0) {
			return;
		}

		this.scorllVecAction = EditorGUILayout.BeginScrollView (this.scorllVecAction);
		EditorGUILayout.BeginVertical ();

		for (int i = 0; i < FormulaData.Instance.ConfigNames.Length; i++) {
			EditorGUILayout.BeginHorizontal ();
			FormulaData.Instance.ConfigNames[i] = EditorGUILayout.TextField (FormulaData.Instance.ConfigNames[i]);
			//if (GUILayout.Button ("-")) {
			//}
			EditorGUILayout.EndHorizontal ();
		}

		EditorGUILayout.EndVertical ();

		EditorGUILayout.EndScrollView ();
	}

	private void AddSign() {
		string str = "config";
		EditorData.Instance.AddStringListItem (ref str, ref FormulaData.Instance.ConfigNames);
		EditorCommon.SaveFormula ();
	}
	*/
}