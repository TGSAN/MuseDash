using UnityEngine;
using System.Collections;

public class CK_RecordDeleteResult : ISN_Result {


	private CK_RecordID _RecordID;
	private CK_Database _Database;


	public CK_RecordDeleteResult(int recordId):base(true) {
		_RecordID = CK_RecordID.GetRecordIdByInternalId(recordId);
	}


	public CK_RecordDeleteResult(string errorData):base(errorData) {

	}

	public void SetDatabase(CK_Database database) {
		_Database = database;
	}



	public CK_Database Database {
		get {
			return _Database;
		}
	}

	public CK_RecordID RecordID {
		get {
			return _RecordID;
		}
	}
}
