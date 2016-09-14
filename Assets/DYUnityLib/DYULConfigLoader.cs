using UnityEngine;
using System.Collections;
using LitJson;

/*
 * 使用方法：
工程中需要使用LitJson库；
创建ConfigLoader对象后（建议作为全局对象）；
通过LoadAsJsonData方法，获得JsonData（类似mapping）
在通过JsonFromExcelParse方法获取数据块
参数是存放在Resources/Config/中的配置文件名


 * */
namespace DYUnityLib {
	public class ConfigLoader {
		// Use this for initialization
		//#if UNITY_EDITOR 
		private static string DEFAULT_PATH = "config/";
//		#elif UNITY_ANDROID
//		"jar:file://" + Application.dataPath + "!/assets/";
//		#elif UNITY_IPHONE
//		Application.dataPath + "/Raw/";
//		#elif UNITY_STANDALONE_WIN 
//		"file://" + Application.dataPath + "/StreamingAssets/";
//		#else 
//		string.Empty;
//		#endif


		//	
		TextAsset Load (string fileName, string path = null) {
			string _p = path;
			if (_p == null) {
				_p = DEFAULT_PATH;
			}

			try {
				TextAsset txt = (TextAsset)Resources.Load (_p + fileName);
				if (txt == null) {
					Debug.Log ("DYULConfigLoader no such file " + _p + fileName);
					return txt;
				}

				return txt;

			} catch (System.InvalidCastException) {
				//Debug.Log ("配置" + fileName + "分析有误");
				return null;
			}

			return null;
		}

		//void testtest(){
		//	JsonData jd = new JsonData ();
		//	jd ["vvv"] = "fuck you very much.";
		//	SavePrefs ("kkk", jd);
		//
		//	JsonData jd2 = LoadPrefs ("kkk");
		//	Debug.Log ("---------------------------->>>>> " + jd2 ["vvv"]);
		//}

		/*
		 * 各平台保存路径

			On Mac OS X PlayerPrefs are stored in ~/Library/Preferences folder, in a file named unity.[company name].[product name].plist, where company and product names are the names set up in Project Settings. The same .plist file is used for both Projects run in the Editor and standalone players.

			On Windows, PlayerPrefs are stored in the registry under HKCUSoftware[company name][product name] key, where company and product names are the names set up in Project Settings.

			On Linux, PlayerPrefs can be found in ~/.config/unity3d/[CompanyName]/[ProductName] again using the company and product names specified in the Project Settings.

			On Windows Store Apps, Player Prefs can be found in %userprofile%AppDataLocalPackages[ProductPackageId]>LocalStateplayerprefs.dat

			On Windows Phone 8, Player Prefs can be found in application's local folder, See Also: Windows.Directory.localFolder

			WebPlayer

			On Web players, PlayerPrefs are stored in binary files in the following locations:

			Mac OS X: ~/Library/Preferences/Unity/WebPlayerPrefs

			Windows: %APPDATA%UnityWebPlayerPrefs
		 */
		public JsonData LoadPrefs (string fileName){
			string str = PlayerPrefs.GetString (fileName);
			if (str == null) {
				return null;
			}

			return (JsonData)JsonMapper.ToObject (str);
		}
		public string LoadPrefs (string fileName,bool _string){
			string str = PlayerPrefs.GetString (fileName);
			return str;
		}

		public void SavePrefs (string fileName, JsonData data){
			string saveStr = JsonMapper.ToJson (data);
			PlayerPrefs.SetString (fileName, saveStr);
		}
		public void SavePrefs (string fileName, string _string){
			//string saveStr = JsonMapper.ToJson (data);
			PlayerPrefs.SetString (fileName, _string);
		}
		public void ClearPrefs () {
			PlayerPrefs.DeleteAll ();
		}

		public JsonData LoadAsJsonData(string fileName, string path = null){
			TextAsset txt = this.Load (fileName, path);
			if (txt == null) {
				return null;
			}

			JsonReader jrd = new JsonReader (txt.text);
			JsonData jd = JsonMapper.ToObject (jrd);

			return jd;
			/*
			Debug.Log ("------------------------>>>");
			foreach (string k in jd.Keys) {
				Debug.Log(k + " " + jd[k]);
			}
			Debug.Log ("------>>>");
			*/
		}

		///真心没看懂sheet 是干什么的 分页？也不是 *下一行会报错 不知道为什么
		//重载函数在下面 附带一个可以辨别从第几行开始 （以防以后表头增多）
		public JsonData JsonFromExcelParse(JsonData config, int sheet = 0)
		{
			JsonData sheetData = config ["Workbook"] ["Worksheet"];
			if (sheetData == null || sheetData.Count <= 0 || sheetData.Count < sheet) {
				Debug.Log("Config has no sheet");
				return null;
			}
			//IList rowData = (IList)sheetData ["Table"] ["Row"];
			IList rowData = (IList)sheetData [sheet] ["Table"] ["Row"];
			if (rowData == null || rowData.Count <= 1) {
				Debug.Log("Config sheet " + sheet + " has no data.");
				return null;
			}

			JsonData datas = new JsonData ();
			IList keys = (IList)((JsonData)rowData [0]) ["Cell"];
			int keyCount = keys.Count;

			for (int i = 1; i < rowData.Count; i++) {
				JsonData _rd = (JsonData)rowData[i];
				if (_rd == null || _rd.IsString || !_rd.Keys.Contains("Cell")){
					Debug.Log("Config at row " + i + " has fucking no data.");
					continue;
				}

				IList _d = (IList)_rd["Cell"];
				if (_d == null){

					continue;
				}

				JsonData _tmp = new JsonData();
				for(int _i = 0; _i < keyCount; _i++){
					if (_i >= _d.Count){
						continue;
					}
					string __key = (string)((JsonData)keys[_i])["Data"];
					JsonData __rd = (JsonData)_d[_i];
					if (__rd == null || __rd.IsString || !__rd.Keys.Contains("Data")){
						continue;
					}

					JsonData __value = __rd["Data"];
					_tmp[__key] = __value;
				//	Debug.Log("kEY"+__key+"VALUE"+__value);
				}

				datas.Add(_tmp);
			}

			return datas;
		}
		/// <summary>
		/// //重载函数 Edit： SF 2015/12/22
		/// </summary>
		/// <returns>The from excel parse.</returns>
		/// <param name="config"></param>
		/// <param name="_BeginRow">文件数据开始的行数 默认为第二行</param>
		/// <param name="sheet">迷之参数</param>
		public JsonData JsonFromExcelParse(JsonData config,int _BeginRow, int sheet = 0){
			
			JsonData sheetData = config ["Workbook"] ["Worksheet"];
			if (sheetData == null || sheetData.Count <= 0 || sheetData.Count < sheet) {
				Debug.Log("Config has no sheet");
				return null;
			}
			//IList rowData = (IList)sheetData ["Table"] ["Row"];
			IList rowData = (IList)sheetData ["Table"] ["Row"];
			if (rowData == null || rowData.Count <= 1) {
				Debug.Log("Config sheet " + sheet + " has no data.");
				return null;
			}
			
			JsonData datas = new JsonData ();
			IList keys = (IList)((JsonData)rowData [0]) ["Cell"];
			int keyCount = keys.Count;
			
			for (int i = _BeginRow-1; i < rowData.Count; i++) {
				JsonData _rd = (JsonData)rowData[i];
				if (_rd == null || _rd.IsString || !_rd.Keys.Contains("Cell")){
					Debug.Log("Config at row " + i + " has fucking no data.");
					continue;
				}
				
				IList _d = (IList)_rd["Cell"];
				if (_d == null){
					
					continue;
				}
				
				JsonData _tmp = new JsonData();
				for(int _i = 0; _i < keyCount; _i++){
					if (_i >= _d.Count){
						continue;
					}
					string __key = (string)((JsonData)keys[_i])["Data"];
					JsonData __rd = (JsonData)_d[_i];
					if (__rd == null || __rd.IsString || !__rd.Keys.Contains("Data")){
						continue;
					}
					
					JsonData __value = __rd["Data"];
					_tmp[__key] = __value;
					//Debug.Log("kEY"+__key+"VALUE"+__value);
				}
				
				datas.Add(_tmp);
			}
			
			return datas;
		}

	}
}
