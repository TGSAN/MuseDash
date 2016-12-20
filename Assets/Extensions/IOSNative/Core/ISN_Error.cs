using UnityEngine;
using System.Collections;

public class ISN_Error {

	protected int _Code;
	protected string _Description;


	public ISN_Error(int code, string description) {
		_Code = code;
		_Description = description;
	} 

	public ISN_Error(string errorData) {
		string[] data = errorData.Split(IOSNative.DATA_SPLITTER);

		_Code = System.Convert.ToInt32(data[0]);
		_Description = data[1];
	}


	public int Code {
		get {
			return _Code;
		}
	}

	public string Description {
		get {
			return _Description;
		}
	}
}
