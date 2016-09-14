using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;
using System.IO;


public class AssetBundleCreat: EditorWindow {
	public static string AsbSavePath = "Assets/Assetbundle";

	[MenuItem("RHY/AssetBundle/自动设置音频(测试)")]
	static void AutoSetBundleId() {
		string _path = "Resources/Audio/Musics/";
		string _assImportPath = "Assets/Resources/Audio/Musics/";
		string _assPath = Application.dataPath + "/" + _path;

		DirectoryInfo dInfo = new DirectoryInfo (_assPath);
		FileInfo[] fns = dInfo.GetFiles ();
		foreach (FileInfo _fn in fns) {
			if (_fn.Name [0] == '.') {
				continue;
			}

			if (_fn.Name.Contains (".meta")) {
				continue;
			}

			string _assetPath = _assImportPath + _fn.Name;
			AssetImporter assetImporter = AssetImporter.GetAtPath (_assetPath);
			string _fbundle = _path + _fn.Name.Replace (".ogg", "");
			//assetImporter.assetBundleName = _fbundle;
			//assetImporter.assetBundleVariant = "ogg";
			assetImporter.assetBundleName = null;
			assetImporter.assetBundleVariant = null;
			Debug.Log ("Set file bundle " + _fn.Name + " : " + _fbundle);
		}
	}

	[MenuItem("RHY/AssetBundle/打包")]
	static void CreateAssetBundleThemelves() {
		BuildPipeline.BuildAssetBundles (AsbSavePath, BuildAssetBundleOptions.UncompressedAssetBundle | BuildAssetBundleOptions.CollectDependencies, BuildTarget.iOS);
		AssetDatabase.Refresh ();

		FileInfo fInfoOld = new FileInfo (Application.dataPath + "/Assetbundle/Assetbundle.ab");
		if (fInfoOld != null) {
			fInfoOld.Delete ();
		}

		FileInfo fInfo = new FileInfo (Application.dataPath + "/Assetbundle/Assetbundle");
		fInfo.CopyTo ("Assets/Assetbundle/Assetbundle.ab");
		fInfo.Delete ();
		//fInfo.MoveTo ("Assetbundle.ab");

		//AssetImporter assetImporter = AssetImporter.GetAtPath (Application.dataPath + "/Assetbundle/Assetbundle");
		//assetImporter.name += ".ab";
		//assetImporter.SaveAndReimport ();

		CreateAssetBundleVersion ();
	}

	[MenuItem("RHY/AssetBundle/获取perfab的关联")]
	static void CreateGetDependObjs() {
		List<Object> _DependAsset = new List<Object> ();
		Object HanldeObject = Selection.activeObject;
		Object[] dependObjs = EditorUtility.CollectDependencies (new Object[]{ HanldeObject });

		for (int i = 0, max = dependObjs.Length; i < max; i++) {
			if (dependObjs [i].GetType () != typeof(MonoScript)) {
				_DependAsset.Add (dependObjs [i]);
				Debug.Log (dependObjs [i].name + ":" + dependObjs [i].GetType ());
			}
		}

		Selection.objects = _DependAsset.ToArray ();
	}

	[MenuItem("RHY/AssetBundle/生成版本文档")]
	static void CreateAssetBundleVersion() {
		AssetBundleFileMangager.Get ().CreateVersionText (Application.dataPath + "/Assetbundle");
		///修改一个文件的后缀名字
		AssetDatabase.Refresh ();
	}


//	[MenuItem("AssetBundle/AudioClip/Create One AudioClip")]
//	static void CreateAudioAssetBundles()
//	{
//
////		List<AssetBundleCreatePath> Config= AssetBundleCreateConfig.Get ()._OneAudioClipsConfig;
////
////		for (int i = 0, max = Config.Count; i < max; i++) 
////		{
////			AssetBundleCreatePath TepCreate=Config[i];
////			// Create the array of bundle build details.
////			AssetBundleBuild[] buildMap = new AssetBundleBuild[1];
////
////			buildMap[0].assetBundleName = TepCreate.AsbName;
////
////			string[] enemyAssets = TepCreate._ResPathList;
////			buildMap[0].assetNames = enemyAssets;
////
////			BuildPipeline.BuildAssetBundles(TepCreate.AsbPath, buildMap, BuildAssetBundleOptions.None, BuildTarget.iOS);
////
////		}
////		AssetDatabase.Refresh ();
//	}

//	[MenuItem("AssetBundle/TestCreate")]
//	static void CreateAssetBundleBySelectObjs()
//	{
////		AssetBundleBuild[] buildMap = new AssetBundleBuild[1];
////		buildMap[0].assetBundleName = "test.asb";
////
////		Object[] SelectObject = Selection.objects;
////		string[] _Assets = new string[SelectObject.Length];
////		for (int i = 0, max = SelectObject.Length; i < max; i++) 
////		{
////			_Assets[i]= AssetDatabase.GetAssetPath(SelectObject[i]);
////		}
////		buildMap[0].assetNames = _Assets;
////
////		BuildPipeline.BuildAssetBundles("Assets/StreamingAssets/Asb", buildMap, BuildAssetBundleOptions.None, BuildTarget.iOS);
//	}
}

/// <summary>
/// 单个ASB的配置信息
/// </summary>
public class AssetBundleCreatePath {
	/// <summary>
	/// 生成的ASB名字
	/// </summary>
	public string AsbName = string.Empty;
	/// <summary>
	/// 生成的ASBID
	/// </summary>
	public string[] AsbID = null;
	/// <summary>
	/// ASB包含的资源项
	/// </summary>
	public string[] _ResPathList =null;
	/// <summary>
	/// asb的位置
	/// </summary>
	public string AsbPath = string.Empty;
}

/// <summary>
/// 各个类型的ASB的打包配置
/// </summary>
public class AssetBundleCreateConfig {
	/// <summary>
	/// 单个音频的打包配置信息
	/// </summary>
	public List<AssetBundleCreatePath> _OneAudioClipsConfig = new List<AssetBundleCreatePath> ();
	/// <summary>
	/// 杂项的音频的统一打包的配置信息
	/// </summary>
	public List<AssetBundleCreatePath> _MaxAudioClipsConfig = new List<AssetBundleCreatePath> ();
	/// <summary>
	/// 
	/// </summary>
	public List<AssetBundleCreatePath> _TexturesConfig = new List<AssetBundleCreatePath> ();

	static AssetBundleCreateConfig S_Instance = null;

	public static AssetBundleCreateConfig Get() {
		if (S_Instance == null) {
			S_Instance = new AssetBundleCreateConfig ();
			S_Instance.Init ();
		}

		return S_Instance;
	}

	/// <summary>
	/// 初始化配置
	/// </summary>
	public void Init() {
		//初始化各个音频文件的打包配置
//		_OneAudioClipsConfig.Clear();
//
//		AssetBundleCreatePath _OneClip1 = new AssetBundleCreatePath ();
//
//		_OneClip1.AsbName = "AudioCip1001.ab";
//		_OneClip1.AsbPath = "Assets/StreamingAssets/Asb/Audio";
//		_OneClip1._ResPathList = new string[1]
//		{
//			"Assets/Resources/Audio/Musics/ama_-Amacha.mp3",
//		};
//		_OneClip1.AsbID = new string[1]
//		{ 
//			"Audio1001",
//		};
//
//		AssetBundleCreatePath _OneClip2 = new AssetBundleCreatePath ();
//		_OneClip2.AsbID = new string[1]
//		{ 
//			"Audio1002",
//		};
//		_OneClip2.AsbName = "AudioCip1002.ab";
//		_OneClip2.AsbPath = "Assets/StreamingAssets/Asb/Audio";
//		_OneClip2._ResPathList = new string[1]
//		{
//			"Assets/Resources/Audio/Musics/bgm_maoudamashii_cyber12.mp3",
//		};
//
//		AssetBundleCreatePath _OneClip3 = new AssetBundleCreatePath ();
//		_OneClip2.AsbID = new string[1] {
//			"UIAudio1001",
//		};
//		_OneClip3.AsbName = "AudioCipOther.ab";
//		_OneClip3.AsbPath = "Assets/StreamingAssets/Asb/Audio";
//		_OneClip3._ResPathList = new string[1]
//		{
//			"Assets/Resources/Audio/UIMusic/bg_music_01.mp3",
//		};
//
//		_OneAudioClipsConfig.Add (_OneClip1);
//		_OneAudioClipsConfig.Add (_OneClip2);
//		_OneAudioClipsConfig.Add (_OneClip3);
		///整个音频的打包处理
	}
}
