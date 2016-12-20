//#define CLOUD_KIT
using UnityEngine;
using System;
using System.Collections;
#if (UNITY_IPHONE && !UNITY_EDITOR && CLOUD_KIT) || SA_DEBUG_MODE
using System.Runtime.InteropServices;
#endif


public class ISN_CloudKit : ISN_Singleton<ISN_CloudKit> {

	#if (UNITY_IPHONE && !UNITY_EDITOR && CLOUD_KIT) || SA_DEBUG_MODE
	[DllImport ("__Internal")]
	private static extern void _ISN_CreateRecordId_Object(int recordId, string name);

	[DllImport ("__Internal")]
	private static extern void _ISN_UpdateRecord_Object(int ID, string charType, string keys, string values, int recordId);

	[DllImport ("__Internal")]
	private static extern void _ISN_SaveRecord(int dbId, int recordId);

	[DllImport ("__Internal")]
	private static extern void _ISN_DeleteRecord(int dbId, int recordId);

	[DllImport ("__Internal")]
	private static extern void _ISN_FetchRecord(int dbId, int recordId);





	#endif

	private CK_Database _PrivateDB = null;
	private CK_Database _PublicDB = null;


	private const int PUBLIC_DB_KEY = -1;
	private const int PRIVATE_DB_KEY = -2;


	void Awake() {
		DontDestroyOnLoad(gameObject);

		_PublicDB =  new CK_Database(PUBLIC_DB_KEY);
		_PrivateDB =  new CK_Database(PRIVATE_DB_KEY);

	}



	//--------------------------------------
	// Get / Set
	//--------------------------------------


	/// <summary>
	/// The database containing the user’s private data.
	/// 
	/// The database in this property is available only if the device has an active iCloud account. 
	/// Access to the database is limited to the user of that iCloud account by default. 
	/// The current user owns all content in the private database and is allowed to read and write that content. 
	/// Data in the private database is not visible in the developer portal or to any other users.
	/// 
	/// Data stored in the private database counts against the storage quota of the current user’s iCloud account.
	/// 
	/// If there is no active iCloud account on the user’s device, this property still returns a valid database object, 
	/// but attempts to use that object will return errors. 
	/// </summary>
	public CK_Database PrivateDB {
		get {
			return _PrivateDB;
		}
	}

	/// <summary>
	/// The database containing the data shared by all users.
	/// 
	/// The database in this property is available regardless of whether the user’s device has an active iCloud account. 
	/// The contents of the public database are readable by all users of the app, and users 
	/// have write access to the records (and other data objects) they create. 
	/// Data in the public database is also visible in the developer portal, 
	/// where you can assign roles to users and restrict access as needed.
	/// 
	/// Data stored in the public database counts against your app’s iCloud storage 
	/// quota and not against the quota of any single user.
	/// </summary>
	public CK_Database PublicDB {
		get {
			return _PublicDB;
		}
	}


	//--------------------------------------
	// Internal Events
	//--------------------------------------



	/*
	 * Save
	 */

	private void OnSaveRecordSuccess(string data) {
		string[] values =  IOSNative.ParseArray(data);
		int dbId = System.Convert.ToInt32(values[0]);
		int recordId = System.Convert.ToInt32(values[1]);

		CK_Database db = CK_Database.GetDatabaseByInternalId(dbId);

		CK_RecordResult result =  new CK_RecordResult(recordId);

		db.FireSaveRecordResult(result);
	}

	private void OnSaveRecordFailed(string data) {
		string[] DataArray = data.Split(new string[] { IOSNative.DATA_SPLITTER2 }, StringSplitOptions.None);


		int dbId = System.Convert.ToInt32(DataArray[0]);
		CK_Database db = CK_Database.GetDatabaseByInternalId(dbId);


		string errorData = DataArray[1];
		CK_RecordResult result =  new CK_RecordResult(errorData);

		db.FireSaveRecordResult(result);
	}


	/*
	 * Delete
	 */

	private void OnDeleteRecordSuccess(string data) {
		string[] values =  IOSNative.ParseArray(data);
		int dbId = System.Convert.ToInt32(values[0]);
		int recordId = System.Convert.ToInt32(values[1]);

		CK_Database db = CK_Database.GetDatabaseByInternalId(dbId);

		CK_RecordDeleteResult result =  new CK_RecordDeleteResult(recordId);

		db.FireDeleteRecordResult(result);
	}

	private void OnDeleteRecordFailed(string data) {
		string[] DataArray = data.Split(new string[] { IOSNative.DATA_SPLITTER2 }, StringSplitOptions.None);


		int dbId = System.Convert.ToInt32(DataArray[0]);
		CK_Database db = CK_Database.GetDatabaseByInternalId(dbId);


		string errorData = DataArray[1];
		CK_RecordDeleteResult result =  new CK_RecordDeleteResult(errorData);

		db.FireDeleteRecordResult(result);
	}

	/*
	 * Fetch
	 */

	private void OnFetchRecordSuccess(string data) {
		string[] DataArray = data.Split(new string[] { IOSNative.DATA_SPLITTER2 }, StringSplitOptions.None);
		int dbId = System.Convert.ToInt32(DataArray[0]);

		CK_Database db = CK_Database.GetDatabaseByInternalId(dbId);

		string recordData = DataArray[1];
		CK_Record record =  new CK_Record(recordData);
		CK_RecordResult result =  new CK_RecordResult(record.Internal_Id);

		db.FireFetchRecordResult(result);
	}

	private void OnFetchRecordFailed(string data) {
		string[] DataArray = data.Split(new string[] { IOSNative.DATA_SPLITTER2 }, StringSplitOptions.None);


		int dbId = System.Convert.ToInt32(DataArray[0]);
		CK_Database db = CK_Database.GetDatabaseByInternalId(dbId);


		string errorData = DataArray[1];
		CK_RecordResult result =  new CK_RecordResult(errorData);

		db.FireFetchRecordResult(result);
	}






	//--------------------------------------
	// Internal Use Only
	//--------------------------------------


	public static void CreateRecordId_Object(int recordId, string name) {
		#if (UNITY_IPHONE && !UNITY_EDITOR && CLOUD_KIT) || SA_DEBUG_MODE
		_ISN_CreateRecordId_Object(recordId, name);
		#endif
	}


	public static void UpdateRecord_Object(int ID, string type, string keys, string values, int recordId) {
		#if (UNITY_IPHONE && !UNITY_EDITOR && CLOUD_KIT) || SA_DEBUG_MODE
		_ISN_UpdateRecord_Object(ID, type, keys, values, recordId);
		#endif
	}

	public static void SaveRecord(int dbId, int recordId) {
		#if (UNITY_IPHONE && !UNITY_EDITOR && CLOUD_KIT) || SA_DEBUG_MODE
		_ISN_SaveRecord(dbId, recordId);
		#endif
	}

	public static void DeleteRecord(int dbId, int recordId) {
		#if (UNITY_IPHONE && !UNITY_EDITOR && CLOUD_KIT) || SA_DEBUG_MODE
		_ISN_DeleteRecord(dbId, recordId);
		#endif
	}

	public static void FetchRecord(int dbId, int recordId) {
		#if (UNITY_IPHONE && !UNITY_EDITOR && CLOUD_KIT) || SA_DEBUG_MODE
		_ISN_FetchRecord(dbId, recordId);
		#endif
	}







}
