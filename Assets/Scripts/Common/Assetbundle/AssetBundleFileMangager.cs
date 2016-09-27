using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System;

public class AssetBundleFileMangager {
	/// <summary>
	/// 网络的地址URL
	/// </summary>
	public readonly string WebUIRl = "http://172.16.1.100:23333";

	/// <summary>
	/// 资源路径访问头
	/// </summary>
	public static readonly string FILE_HEAD = "file://";

	/// <summary>
	/// 本地资源的URL地址
	/// </summary>
	public static readonly string ResURL = FILE_HEAD + Application.streamingAssetsPath;
	/// <summary>
	/// 文件创建位置
	/// </summary>
#if UNITY_EDITOR || UNITY_EDITOR_OSX || UNITY_EDITOR_64
	public static readonly string FileCreateResPath = Application.streamingAssetsPath;
#elif UNITY_IOS
	public static readonly string FileCreateResPath = Application.persistentDataPath;
#else
	public static readonly string FileCreateResPath = Application.persistentDataPath;
#endif

	/// <summary>
	/// 本地资源加载目录
	/// </summary>
	public static readonly string FileLoadResPath = FILE_HEAD + FileCreateResPath;

	/// <summary>
	/// 网络的MD5文件信息
	/// </summary>
	public Dictionary<string,string> _WebServiceVersionDic = new Dictionary<string, string>();
	/// <summary>
	/// 本地的MD5文件信息
	/// </summary>
	public Dictionary<string,string> _LocalVersionDic = new Dictionary<string, string>();
	/// <summary>
	/// 保存的需要下载的文件
	/// </summary>
	public List<string> _NeedLoadFiles = new List<string>();
	/// <summary>
	/// 是否需要更新
	/// </summary>
	public bool NeedUpdateLocalVersionFile = false;

	public int needUpdateCount = 0;

	private bool isLoadAll = false;

	public string GetWebServiceGameEditionStr() {
		return "1.0.0.0";
	}

	public bool IsLoadAll {
		get {
			return this.isLoadAll;
		}
	}

	private static AssetBundleFileMangager S_Instance = null;

	static public AssetBundleFileMangager Get() {
		if (S_Instance == null) {
			S_Instance = new AssetBundleFileMangager ();
		}

		return S_Instance;
	}

	/// <summary>
	/// 获得本地游戏的配置信息
	/// </summary>
	public void InitGameConfig() {
		this.isLoadAll = false;
		///加载网络的Version文本
		AssetBundleLoad.Get ().wwwLoadAssetBundleInterface (WebUIRl + "/version.txt", (wwwLoad) => {
			///得到网络服务器的资源MD5文件
			this.SaveVersionTxtToDic (wwwLoad.text, this._WebServiceVersionDic);
			///加载本地的VersionMD5文件
			AssetBundleLoad.Get ().wwwLoadAssetBundleInterface (ResURL + "/version.txt", (LocalwwwVersion) => {
				if (LocalwwwVersion == null) {
					this._LocalVersionDic = new Dictionary<string, string> ();
				} else {
					this.SaveVersionTxtToDic (LocalwwwVersion.text, this._LocalVersionDic);
				}
				///进行本地的文件的校对 确定是否需要进行资源的更新
				this.CompareVersion ();
				///进行更新
				this.UpdateRes ();
			});
		});
	}

	/// <summary>
	/// 保存数据到内存字典中
	/// </summary>
	/// <param name="content">Content.</param>
	/// <param name="_SaveDic">Save dic.</param>
	private void SaveVersionTxtToDic(string content, Dictionary<string,string> _SaveDic) {
		if (content == null || content.Length == 0) {
			return;
		}

		string[] items = content.Split (new char[] { '\n' });
		foreach (string item in items) {
			string[] info = item.Split (new char[] { ',' });
			if (info != null && info.Length == 2) {
				_SaveDic.Add (info [0], info [1]);
			}
		}
	}

	/// <summary>
	/// 更新本地的文件资源
	/// </summary>
	private void CompareVersion() {
		foreach (var version in this._WebServiceVersionDic) {
			string fileName = version.Key;
			string serverMd5 = version.Value;
			//新增的资源
			if (!this._LocalVersionDic.ContainsKey (fileName)) {
				///新添加的资源
				this._NeedLoadFiles.Add (fileName);
			} else {
				//需要替换的资源
				string localMd5 = string.Empty;
				this._LocalVersionDic.TryGetValue (fileName, out localMd5);
				if (!serverMd5.Equals (localMd5)) {
					this._NeedLoadFiles.Add (fileName);
				}
			}
		}

		//本次有更新，同时更新本地的version.txt
		this.needUpdateCount = this._NeedLoadFiles.Count;
		this.NeedUpdateLocalVersionFile = this._NeedLoadFiles.Count > 0;
	}

	private void UpdateRes() {
		if (this._NeedLoadFiles.Count <= 0) {
			this.UpdateLocalVersion ();
			return;
		}

		string file = this._NeedLoadFiles [0];
		///向服务器下载文件资源
		AssetBundleLoad.Get ().wwwLoadAssetBundleInterface (WebUIRl + file, (wwwLoadRes) => {
			//if (wwwLoadRes == null || !wwwLoadRes.isDone) {
				//Debug.Log("--->" + file + " " + wwwLoadRes.progress);
				//this.UpdateRes ();
				//return;
			//}

			Debug.Log ("Load file ok : " + file + "  " + (this.needUpdateCount - this._NeedLoadFiles.Count + 1) + "/" + this.needUpdateCount);
			this._NeedLoadFiles.RemoveAt (0);
			this.ReplaceLoadRes (file, wwwLoadRes.bytes);
			this.UpdateRes ();
		});
	}

	private void ReplaceLoadRes(string fileName, byte[] datas) {
		string filePath = FileCreateResPath + fileName;
		string[] paths = filePath.Split ('/');
		string _path = string.Join ("/", paths, 0, paths.Length - 1);
		// Debug.Log ("_path is : " + _path);

		FileStream stream = null;
		if (File.Exists (filePath)) {
			stream = new FileStream (filePath, FileMode.Create);
		} else {
			DirectoryInfo dInfo = new DirectoryInfo (_path);
			if (!dInfo.Exists) {
				dInfo.Create ();
			}

			stream = new FileStream (filePath, FileMode.CreateNew);
		}

		stream.Write (datas, 0, datas.Length);
		stream.Flush ();
		stream.Close ();
	}

	/// <summary>
	/// 更新本地的MD5文件
	/// </summary>
	private void UpdateLocalVersion() {
		if (this.NeedUpdateLocalVersionFile) {
			StringBuilder versions = new StringBuilder ();
			foreach (var item in this._WebServiceVersionDic) {  
				versions.Append (item.Key).Append (",").Append (item.Value).Append ("\n");
			}

			FileStream stream = new FileStream (FileCreateResPath + "/version.txt", FileMode.Create);
			byte[] data = Encoding.UTF8.GetBytes (versions.ToString ());
			stream.Write (data, 0, data.Length);
			stream.Flush ();
			stream.Close ();
			this.isLoadAll = true;
			Debug.Log ("Up-to-date Finished.");
		} else {
			this.isLoadAll = true;
		}
	}

	/// <summary
	/// 对文件生成MD5
	/// </summary>
	/// <returns>The d5 file.</returns>
	/// <param name="file">File.</param>
	public static string MD5File(string file) {
		try {
			FileStream fs = new FileStream (file, FileMode.Open);
			System.Security.Cryptography.MD5 md5 = new System.Security.Cryptography.MD5CryptoServiceProvider ();
			byte[] retVal = md5.ComputeHash (fs);
			fs.Close ();
			StringBuilder sb = new StringBuilder ();
			for (int i = 0; i < retVal.Length; i++) {
				sb.Append (retVal [i].ToString ("x2"));
			}

			Debug.LogWarning (sb.ToString ());
			return sb.ToString ();
		} catch (Exception ex) {
			throw new Exception ("md5file() fail, error:" + ex.Message);
		}
	}

	/// <summary>
	/// 生成更新的文件信息  文本的名字 + 文本的MD5值
	/// </summary>
	/// <returns><c>true</c>, if version text was created, <c>false</c> otherwise.</returns>
	public bool CreateVersionText(string FilePath) {
		// 获取Res文件夹下所有文件的相对路径和MD5值
		string[] files = Directory.GetFiles (FilePath, "*", SearchOption.AllDirectories);
		StringBuilder versions = new StringBuilder ();
		for (int i = 0, len = files.Length; i < len; i++) {
			string filePath = files [i];
			int _dotIdx = files [i].LastIndexOf (".");
			if (_dotIdx < 0) {
				Debug.Log ("File " + filePath + " has no file type for example .ab");
			}

			string extension = filePath.Substring (_dotIdx);
			if (extension == ".ab" || extension == ".ogg") {
				string relativePath = filePath.Replace (FilePath, "").Replace ("\\", "/");
				string md5 = MD5File (filePath);
				versions.Append (relativePath).Append (",").Append (md5).Append ("\n");
			}
		}

		// 生成配置文件
		FileStream stream = new FileStream (Application.dataPath + "/Assetbundle/version.txt", FileMode.Create);
		byte[] data = Encoding.UTF8.GetBytes (versions.ToString ());
		stream.Write (data, 0, data.Length);
		stream.Flush ();
		stream.Close ();

		return true;
	}
}