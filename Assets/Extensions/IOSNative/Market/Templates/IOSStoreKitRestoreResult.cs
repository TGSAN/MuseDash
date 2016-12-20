using UnityEngine;
using System.Collections;

public class IOSStoreKitRestoreResult : ISN_Result {


	//--------------------------------------
	// Initialize
	//--------------------------------------


	public  IOSStoreKitRestoreResult(ISN_Error e) : base(e) {
	
	}

	public IOSStoreKitRestoreResult(bool IsResultSucceeded) : base(IsResultSucceeded)  {
	
	}

	public IOSTransactionErrorCode TransactionErrorCode {
		get {
			if(_Error != null) {
				return (IOSTransactionErrorCode) _Error.Code;
			} else {
				return IOSTransactionErrorCode.SKErrorNone;
			}

		}
	}

}
