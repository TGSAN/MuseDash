using System;
using System.Collections;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using LitJson;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using UnityEngine;
using cn.bmob.json;

namespace FormulaBase {
	public class FormulaBase {
		public const int DATA_SOURCE_NONE = 0;
		// 数据来源类型
		/// <summary>
		/// Dynamic value is index of config, from sum of condiction.
		/// </summary>
		public const int DATA_SOURCE_CONFIG = 1;
		/// <summary>
		/// Dynamic value is result value of dynamic formula.
		/// </summary>
		public const int DATA_SOURCE_QUOTE_FORMULA = 2;
		/// <summary>
		/// Dynamic value is sum of condiction.
		/// </summary>
		public const int DATA_SOURCE_DYNAMIC = 3;
		/// <summary>
		/// Dynamic value is base value + sum of condiction.
		/// </summary>
		public const int DATA_SOURCE_BASE_VALUE = 4;
		/// <summary>
		/// Config read with param.
		/// </summary>
		public const int DATA_SOURCE_PARAM_CONFIG = 5;

		public const int CONDICTION_TYPE_NONE = 0;
		public const int CONDICTION_TYPE_SIGN = 1;
		public const int CONDICTION_TYPE_OVER_VALUE = 2;
		public const int CONDICTION_TYPE_BASE_VALUE = 3;
		public const int CONDICTION_TYPE_EQUAL_VALUE = 4;
		public const int CONDICTION_TYPE_OVER_RATE = 5;
		public const int CONDICTION_TYPE_RATE = 6;
		public const int CONDICTION_TYPE_OVER_FORMULA = 7;
		public const int CONDICTION_TYPE_OVER_FIX_VALUE = 8;
		public const int CONDICTION_TYPE_CFG_VALUE = 9;

		public const char SPLITE_UNIT = ' ';
		public const string SIGN_KEY_HOST_NAME = "fileName";

		// 倒计时相关
		public const string SING_KEY_REAL_TIME_START = "SING_KEY_REAL_TIME_START";
		public const string SING_KEY_REAL_TIME_END = "SING_KEY_REAL_TIME_END";

		// 数据被更改
		public const string SING_KEY_DATA_MODIFIED = "SING_KEY_DATA_MODIFIED";

		private const string CHINESE_MATCH = @"^[\u4e00-\u9fa5]+$";
		private const string DIG_MATCH = @"^\d+\.\d+$";

		public static bool IsChinese(string str) {
			if (str == null) {
				return false;
			}

			return Regex.IsMatch (str, CHINESE_MATCH);
		}

		public static bool IsDig (string str) {
			if (str == null) {
				return false;
			}

			return Regex.IsMatch (str, DIG_MATCH);
		}

		public static bool IsSignMatch(string str) {
			if (str == null) {
				return false;
			}

			if (str.Length < 1) {
				return false;
			}

			for (int i = 0; i < FormulaData.Instance.DynamicParams.Length; i++) {
				string s = FormulaData.Instance.DynamicParams [i];
				if (s == str) {
					return true;
				}
			}

			return false;
		}

		/// <summary>
		/// 将c# DateTime时间格式转换为Unix时间戳格式
		/// </summary>
		/// <param name="time">时间</param>
		/// <returns>long</returns>
		public static long ConvertDateTimeInt(System.DateTime time) {
			var start = TimeZone.CurrentTimeZone.ToLocalTime (new System.DateTime (1970, 1, 1, 0, 0, 0, 0));
			long t = Convert.ToInt64 ((time - start).TotalSeconds);
			return t;
		}

		/// <summary>
		/// Gets the real time now.
		/// 
		/// 获得以服务器时间为基础的当前真实时间
		/// </summary>
		/// <returns>The real time now.</returns>
		public static int GetRealTimeNow() {
			int gameTime = (int)(Time.realtimeSinceStartup - ExpandBmobLogin.ServerTimeRefleshAt);
			int realNow = ExpandBmobLogin.ServerTimeStamp.Get () + gameTime;
			return realNow;
		}
	}

	/// <summary>
	/// Fomula host manager.
	/// </summary>
	public class FomulaHostManager {
		public const string NAME_SPACE = "FormulaBase.";
		public const string SAVE_FILE_NAME = "rhyhostdatalocal";
		//private static Dictionary<string, System.Type> TYPE_POLL;
		//private static System.Reflection.Assembly assembly = System.Reflection.Assembly.Load("Assembly-CSharp");

		private FormulaHost[] catchObjects;

		private Dictionary<string, UIPhaseHelper> catchUiHelper;

		/// <summary>
		/// The host pool.
		/// </summary>
		private Dictionary<string, Dictionary<string, FormulaHost>> hostPool;
		
		private static FomulaHostManager instance;
		public static FomulaHostManager Instance {
			get {
				if (instance == null) {
					instance = new FomulaHostManager ();
				}
				
				return instance;
			}
		}

		public FomulaHostManager () {
			if (FormulaData.Instance.Hosts == null || FormulaData.Instance.Hosts.Length <= 0) {
				return;
			}
			
			this.catchObjects = new FormulaHost[FormulaData.Instance.Hosts.Length];
			for (int i = 0; i < FormulaData.Instance.Hosts.Length; i++) {
				this.catchObjects [i] = new FormulaHost (i);
			}
		}

		public int Count {
			get {
				if (this.hostPool == null) {
					return 0;
				}

				int c = 0;
				foreach (string k in this.hostPool.Keys) {
					if (k == null) {
						continue;
					}

					c += this.hostPool [k].Count;
				}

				return c;
			}
		}
		/*
		public CustomComponentBase GetCustomComponentObj(string name) {
			System.Type t = this.GetHostModuleType (name);
			if (t == null) {
				return null;
			}

			CustomComponentBase obj = (CustomComponentBase)t.Assembly.CreateInstance (t.ToString ());
			return obj;
		}

		private System.Type GetHostModuleType(string name) {
			System.Type t = null;
			if (TYPE_POLL.ContainsKey (name)) {
				t = TYPE_POLL [name];
				if (t != null) {
					return t;
				}
			}

			t = assembly.GetType (NAME_SPACE + name);
			TYPE_POLL [name] = t;
			return t;
		}*/

		/// <summary>
		/// Creates the formula.
		/// Get a new host object by idx.
		/// </summary>
		/// <returns>The Host.</returns>
		/// <param name="idx">Index.</param>
		public FormulaHost CreateHost(int idx) {
			/*
			if (idx >= this.catchObjects.Length) {
				return null;
			}

			FormulaHost cObj = null;
			FormulaHost catchObj = this.catchObjects [idx];
			if (catchObj != null) {
				cObj = catchObj.Clone ();
				return cObj;
			}
			
			catchObj = new FormulaHost (idx);
			this.catchObjects [idx] = catchObj;
			
			cObj = catchObj.Clone ();

			return cObj;
			*/
			FormulaHost host = new FormulaHost (idx);

			return host;
		}

		/// <summary>
		/// Creates the host.
		/// Get a new host object by name.
		/// </summary>
		/// <returns>The Host.</returns>
		/// <param name="name">Name.</param>
		public FormulaHost CreateHost(string name) {
			int _idx = -1;
			for (int i = 0; i < FormulaData.Instance.Hosts.Length; i++) {
				FormulaHostStruct fs = FormulaData.Instance.Hosts [i];
				if (name == fs.name || name == fs.fileName) {
					_idx = i;
					break;
				}
			}
			
			if (_idx < 0) {
				return null;
			}
			
			return this.CreateHost (_idx);
		}

		/// <summary>
		/// Loads the host.
		/// 
		/// load from hostPool or create new host.
		/// </summary>
		/// <returns>The host.</returns>
		/// <param name="name">Name.</param>
		public FormulaHost LoadHost(string fileName, string oid = null) {
			for (int i = 0; i < FormulaData.Instance.Hosts.Length; i++) {
				FormulaHostStruct fs = FormulaData.Instance.Hosts [i];
				if (fileName == fs.fileName && this.hostPool.ContainsKey(fileName)) {
					Dictionary<string, FormulaHost> _list = this.hostPool[fileName];
					if (_list != null && oid != null && _list.ContainsKey(oid)) {
						return _list[oid];
					} else {
						foreach(string _oid in _list.Keys) {
							FormulaHost _defaultHost = _list[_oid];
							return _defaultHost;
						}
					}
				}
			}

			return this.CreateHost(fileName);
		}

		public void InitHostPool() {
			if (this.hostPool != null) {
				return;
			}

			this.hostPool = new Dictionary<string, Dictionary<string, FormulaHost>> ();
		}

		public FormulaHost GetHost(string oid) {
			foreach (Dictionary<string, FormulaHost> hosts in this.hostPool.Values) {
				if (hosts.ContainsKey (oid)) {
					return hosts [oid];
				}
			}
			return null;
		}

		/// <summary>
		/// Loads the host.
		/// 
		/// load from hostPool or create new host.
		/// </summary>
		/// <returns>The host.</returns>
		/// <param name="name">Type.</param>
		public FormulaHost LoadHost(int hostType, string oid = null) {
			if (hostType >= FormulaData.Instance.Hosts.Length) {
				Debug.Log ("No such host type " + hostType);
				return null;
			}

			FormulaHostStruct fs = FormulaData.Instance.Hosts [hostType];
			string fileName = fs.fileName;
			if (this.hostPool != null && this.hostPool.ContainsKey (fileName)) {
				Dictionary<string, FormulaHost> _list = this.hostPool [fileName];
				if (_list != null && oid != null && _list.ContainsKey (oid)) {
					return _list [oid];
				} else {
					foreach (string _oid in _list.Keys) {
						FormulaHost _defaultHost = _list [_oid];
						return _defaultHost;
					}
				}
			}

			return this.CreateHost (hostType);
		}

		public string GetFileNameByHostType(int hostType) {
			if (hostType >= FormulaData.Instance.Hosts.Length) {
				Debug.Log ("No such host type " + hostType);
				return null;
			}

			FormulaHostStruct fs = FormulaData.Instance.Hosts [hostType];
			return fs.fileName;
		}

		public int GetHostTypeByFileName(string fileName) {
			if (FormulaData.Instance.Hosts == null) {
				return -1;
			}

			for (int i = 0; i < FormulaData.Instance.Hosts.Length; i++) {
				FormulaHostStruct fd = FormulaData.Instance.Hosts [i];
				if (fd.fileName == fileName) {
					return i;
				}
			}
			return -1;
		}

		/// <summary>
		/// Creates the host.
		/// 
		/// Get a new host directly by data.
		/// Maily for test.
		/// </summary>
		/// <returns>The host.</returns>
		/// <param name="hostData">Host data.</param>
		public FormulaHost CreateHost(FormulaHostStruct hostData) {
			FormulaHost host = new FormulaHost (hostData);
			return host;
		}

		public Dictionary<string, FormulaHost> GetHostListByFileName(string filename) {
			if (this.hostPool == null) {
				this.InitHostPool ();
			}

			if (!this.hostPool.ContainsKey (filename)) {
				return this.hostPool [filename] = new Dictionary<string, FormulaHost> ();
			}

			return this.hostPool [filename];
		}

		public Dictionary<string, FormulaHost> AddHostListByFileName(string filename) {
			if (this.hostPool == null) {
				return null;
			}
			
			if (this.hostPool.ContainsKey (filename)) {
				return this.hostPool [filename];
			}

			this.hostPool [filename] = new Dictionary<string, FormulaHost> ();
			return this.hostPool [filename];
		}

		/// <summary>
		/// Loads all host.
		/// 
		/// Load all host from server or local data,
		/// then creat all host by data.
		/// </summary>
		public void LoadAllHost(HttpEndResponseDelegate _rsp = null) {
			string uid = ExpandBmobCommon.GetUid ();
			if (uid == null) {
				if (GameLogic.GameGlobal.ENABLE_LOCAL_SAVE) {
					JsonData loadData = GameLogic.GameGlobal.gConfigLoader.LoadPrefs (SAVE_FILE_NAME);
					this.HttpLoadDelegate (loadData);
					if (_rsp != null) {
						_rsp (null);
					}
				}
			} else {
				HttpEndResponseDelegate rsp = new HttpEndResponseDelegate (this.HttpLoadDelegate);
				if (_rsp != null) {
					rsp += _rsp;
				}

				ExpandBmobData.Instance.LoadAll (uid, rsp);
			}
		}

		public void AddHost(FormulaHost host) {
			this.InitHostPool ();
			if (host.objectID == null) {
				return;
			}

			string fileName = host.GetFileName ();
			if (!this.hostPool.ContainsKey (fileName)) {
				this.hostPool [fileName] = new Dictionary<string, FormulaHost> ();
			}

			host.RemoveDynamicData (FormulaBase.SING_KEY_DATA_MODIFIED);
			this.hostPool [fileName] [host.objectID] = host;
		}

		public void DeleteAllHost(HttpEndResponseDelegate rsp = null) {
			this.hostPool.Clear ();
			string uid = ExpandBmobCommon.GetUid ();
			if (uid == null) {
				if (GameLogic.GameGlobal.ENABLE_LOCAL_SAVE) {
				}

				PlayerPrefs.DeleteAll ();
			} else {
				ExpandBmobData.Instance.DeleteAll (uid, rsp);
			}
		}

		public void DeleteHost(FormulaHost host, HttpResponseDelegate rsp = null) {
			if (host.objectID == null) {
				return;
			}
			string fileName = host.GetFileName ();
			if (!this.hostPool.ContainsKey (fileName)) {
				return;
			}

			Dictionary<string, FormulaHost> _data = this.hostPool [fileName];
			if (!_data.ContainsKey (host.objectID)) {
				return;
			}
			string uid = ExpandBmobCommon.GetUid ();
			if (uid == null) {
				this.RemoveHostFromPool (host);
				// local delete
				if (GameLogic.GameGlobal.ENABLE_LOCAL_SAVE) {
					JsonData loadData = GameLogic.GameGlobal.gConfigLoader.LoadPrefs (SAVE_FILE_NAME);
					if (!loadData.Keys.Contains (host.objectID)) {
						return;
					}

					loadData [host.objectID] = null;
					GameLogic.GameGlobal.gConfigLoader.SavePrefs (SAVE_FILE_NAME, loadData);
				}
			} else {
				// server delete
				ExpandBmobData.Instance.Delete (uid, host, rsp);
			}
		}

		public void RemoveHostFromPool(FormulaHost host) {
			if (host.objectID == null) {
				return;
			}

			string fileName = host.GetFileName();
			if (!this.hostPool.ContainsKey (fileName)) {
				return;
			}

			Dictionary<string, FormulaHost> _data = this.hostPool [fileName];
			if (!_data.ContainsKey (host.objectID)) {
				return;
			}

			_data.Remove (host.objectID);
		}

		public void SetNotifyUiHost(FormulaHost host) {
			if (host == null) {
				return;
			}

			if (this.catchUiHelper == null) {
				return;
			}

			foreach (UIPhaseHelper uph in this.catchUiHelper.Values) {
				if (uph == null) {
					continue;
				}

				uph.SetActiveHost (host);
				uph.SetLabelHost (host);
				uph.SetSliderHost (host);
			}
		}

		public void SetNotifyUiHelper(string uid, UIPhaseHelper uph) {
			if (uid == null || uph == null) {
				return;
			}

			if (this.catchUiHelper == null) {
				this.catchUiHelper = new Dictionary<string, UIPhaseHelper> ();
			}

			this.catchUiHelper [uid] = uph;
		}

		public void ClearNotifyUiHelper() {
			if (this.catchUiHelper == null) {
				return;
			}

			this.catchUiHelper.Clear ();
		}

		public void NotifyDynamicDataChange(FormulaHost host, string key, object value) {
			if (host == null || key == null) {
				return;
			}

			if (this.catchUiHelper == null) {
				return;
			}

			foreach (UIPhaseHelper uph in this.catchUiHelper.Values) {
				if (uph == null) {
					continue;
				}

				uph.OnNotifyDynamicDataChange (host, key, value);
			}
		}

		public void RemoveNotifyUiHelper(string uid) {
			if (uid == null) {
				return;
			}

			if (this.catchUiHelper == null) {
				return;
			}

			if (!this.catchUiHelper.ContainsKey (uid)) {
				return;
			}

			this.catchUiHelper.Remove (uid);
		}

		/// <summary>
		/// Https the load delegate.
		/// 
		/// After load all JsonData from server or local,
		/// turn into FormulaHost obj and save by fileType.
		/// </summary>
		/// <param name="response">Response.</param>
		private void HttpLoadDelegate(JsonData response) {
			this.InitHostPool ();
			if (response != null) {
				JsonData _data = response;
				/*
				while (_data.Keys.Contains(HttpModule.FUNC_KEY_DATA)) {
					_data = _data [HttpModule.FUNC_KEY_DATA];
				}
				*/
				if (_data.GetJsonType() == JsonType.None) {
					return;
				}
				if (_data != null && _data.Count > 0) {
					foreach (string oid in _data.Keys) {
						if (oid == null) {
							continue;
						}

						JsonData _hostData = _data [oid];
						Debug.Log("存储数据:" + oid + " / " + _hostData.ToJson());
						if (_hostData == null || _hostData.Count == 0) {
							continue;
						}
						if (!_hostData.Keys.Contains (FormulaBase.SIGN_KEY_HOST_NAME)) {
							Debug.Log ("Http load all host data with no host type data : " + _hostData.ToJson ());
							continue;
						}
						string hostFileName = _hostData [FormulaBase.SIGN_KEY_HOST_NAME].ToString ();
						if (!this.hostPool.ContainsKey (hostFileName)) {
							this.hostPool [hostFileName] = new Dictionary<string, FormulaHost> ();
						}

						FormulaHost host = this.CreateHost (hostFileName);
						if (host == null) {
							Debug.Log("Faile to create host with type : " + hostFileName);
							continue;
						}

						host.objectID = oid;
						host.JsonToSign (_hostData);
						this.hostPool [hostFileName] [oid] = host;
					}
				}

				Debug.Log ("FomulaHostManager.HttpLoadDelegate load host data complete.");
			}
		}

		private void HttpLoadDelegate(cn.bmob.response.EndPointCallbackData<Hashtable> resp) {
			this.InitHostPool ();
			if (resp == null) {
				return;
			}

			SimpleJson.JsonArray _resultList = resp.data ["results"] as SimpleJson.JsonArray;
			if (_resultList == null || _resultList.Count == 0) {
				return;
			}

			foreach (SimpleJson.JsonObject _result in _resultList) {
				String _oid = _result ["objectId"] as String;
				if (_oid == null) {
					continue;
				}

				String _fileName = _result ["fileName"] as String;
				String _createAt = _result ["createdAt"] as String;
				string _updateAt = _result ["updatedAt"] as String;
				String _data = _result ["data"] as String;

				FormulaHost host = this.CreateHost (_fileName.ToString ());
				if (host == null) {
					Debug.Log ("Formula host type " + _fileName.ToString () + " has no config!");
					continue;
				}

				host.objectID = _oid;
				host.createAt = _createAt;
				host.updateAt = _updateAt;

				SimpleJson.JsonObject _objData = JsonAdapter.JSON.ToObject (_data) as SimpleJson.JsonObject;
				host.JsonToSign (_objData);

				if (!this.hostPool.ContainsKey (_fileName)) {
					this.hostPool [_fileName] = new Dictionary<string, FormulaHost> ();
				}

				this.hostPool [_fileName] [_oid] = host;
				Debug.Log ("db存储数据 (" + host.objectID + " update at : " + host.updateAt + ") : " + _data);
			}
		}
	}

	/// <summary>
	/// Formula manager.
	/// </summary>
	public class FormulaManager {
		private FormulaObject[] catchObjects;

		private static FormulaManager instance;
		public static FormulaManager Instance {
			get {
				if (instance == null) {
					instance = new FormulaManager ();
				}
				
				return instance;
			}
		}

		public FormulaManager () {
			if (FormulaData.Instance.Formulas == null || FormulaData.Instance.Formulas.Length <= 0) {
				return;
			}

			this.catchObjects = new FormulaObject[FormulaData.Instance.Formulas.Length];
			for (int i = 0; i < FormulaData.Instance.Formulas.Length; i++) {
				this.catchObjects [i] = new FormulaObject (i);
			}
		}

		/// <summary>
		/// Creates the formula.
		/// Get a new fromula object by idx.
		/// </summary>
		/// <returns>The formula.</returns>
		/// <param name="idx">Index.</param>
		public FormulaObject CreateFormula(FormulaHost host, int idx) {
			/*
			FormulaObject cObj = null;
			FormulaObject catchObj = this.catchObjects [idx];
			if (catchObj != null) {
				cObj = catchObj.Clone ();
				cObj.SetHost (host);
				return cObj;
			}

			catchObj = new FormulaObject (idx);
			this.catchObjects [idx] = catchObj;

			cObj = catchObj.Clone ();
			cObj.SetHost (host);

			return cObj;
			*/

			FormulaObject fObj = new FormulaObject (idx);
			fObj.SetHost (host);
			return fObj;
		}

		/// <summary>
		/// Creates the formula.
		/// Get a new fromula object by name.
		/// </summary>
		/// <returns>The formula.</returns>
		/// <param name="name">Name.</param>
		public FormulaObject CreateFormula(FormulaHost host, string name) {
			int _idx = -1;
			for (int i = 0; i < FormulaData.Instance.Formulas.Length; i++) {
				FormulaStruct fs = FormulaData.Instance.Formulas [i];
				if (name == fs.name) {
					_idx = fs.idx;
					break;
				}
			}

			if (_idx < 0) {
				return null;
			}

			return this.CreateFormula (host, _idx);
		}
	}
}