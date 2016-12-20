using UnityEngine;
using System.Collections;

public class ISN_Result  {


	protected ISN_Error _Error = null;
	protected bool _IsSucceeded = true;


	//--------------------------------------
	// Initialize
	//--------------------------------------

	public ISN_Result(bool IsResultSucceeded) {
		_IsSucceeded = IsResultSucceeded;
	}

	public ISN_Result (ISN_Error e) {
		SetError(e);
	}

	public ISN_Result (string errorData) {
		ISN_Error e =  new ISN_Error(errorData);
		SetError(e);
	}


	//--------------------------------------
	// Public Methods (internal use only)
	//--------------------------------------






	//--------------------------------------
	// Get / Set
	//--------------------------------------


	public bool IsSucceeded {
		get {
			return _IsSucceeded;
		}
	}

	public bool IsFailed {
		get {
			return !_IsSucceeded;
		}
	}

	public ISN_Error Error {
		get {
			return _Error;
		}
	}


	//--------------------------------------
	// Private Methods
	//--------------------------------------

	public void SetError(ISN_Error e) {
		_Error = e;
		_IsSucceeded = false;
	}
}
