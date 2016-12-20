using UnityEngine;
using System.Collections;

public class CloudKitUseExample : BaseIOSFeaturePreview {





	void OnGUI() {

		UpdateToStartPos();

		GUI.Label(new Rect(StartX, StartY, Screen.width, 40), "Cloud Kit", style);

		StartY+= YLableStep;
		if(GUI.Button(new Rect(StartX, StartY, buttonWidth, buttonHeight), "Create Record")) {

			CK_RecordID recordId =  new CK_RecordID("1");

			CK_Record record =  new CK_Record(recordId, "Post");
			record.SetObject("PostText", "Sample point of interest");
			record.SetObject("PostTitle", "My favorite point of interest");


			CK_Database database = ISN_CloudKit.Instance.PublicDB;
			database.SaveRecrod(record);

			database.ActionRecordSaved += Database_ActionRecordSaved;
		}


		StartX += XButtonStep;
		if(GUI.Button(new Rect(StartX, StartY, buttonWidth, buttonHeight), "Delete Record")) {
			CK_RecordID recordId =  new CK_RecordID("1");
			CK_Database database = ISN_CloudKit.Instance.PublicDB;

			database.DeleteRecordWithID(recordId);
			database.ActionRecordDeleted += Database_ActionRecordDeleted;

		}

		StartX += XButtonStep;
		if(GUI.Button(new Rect(StartX, StartY, buttonWidth, buttonHeight), "Fetch Record")) {
			CK_RecordID recordId =  new CK_RecordID("1");
			CK_Database database = ISN_CloudKit.Instance.PublicDB;

			database.FetchRecordWithID(recordId);
			database.ActionRecordFetchComplete += Database_ActionRecordFetchComplete;
		}


	}

	void Database_ActionRecordFetchComplete (CK_RecordResult res) {
		res.Database.ActionRecordFetchComplete -= Database_ActionRecordFetchComplete;

		if(res.IsSucceeded) {
			Debug.Log("Database_ActionRecordFetchComplete:");
			Debug.Log("Post Title: "  + res.Record.GetObject("PostTitle"));
		} else {
			Debug.Log("Database_ActionRecordFetchComplete, Error: " + res.Error.Description);
		}
	}

	void Database_ActionRecordDeleted (CK_RecordDeleteResult res) {
		res.Database.ActionRecordDeleted -= Database_ActionRecordDeleted;

		if(res.IsSucceeded) {
			Debug.Log("Database_ActionRecordDeleted, Success: ");
		} else {
			Debug.Log("Database_ActionRecordDeleted, Error: " + res.Error.Description);
		}
	}

	void Database_ActionRecordSaved (CK_RecordResult res) {

		res.Database.ActionRecordSaved -= Database_ActionRecordSaved;

		if(res.IsSucceeded) {
			Debug.Log("Database_ActionRecordSaved:");
			Debug.Log("Post Title: "  + res.Record.GetObject("PostTitle"));
		} else {
			Debug.Log("Database_ActionRecordSaved, Error: " + res.Error.Description);
		}
	}

}
