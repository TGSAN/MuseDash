////////////////////////////////////////////////////////////////////////////////
//  
// @module IOS Native Plugin for Unity3D 
// @author Osipov Stanislav (Stan's Assets) 
// @support stans.assets@gmail.com 
//
////////////////////////////////////////////////////////////////////////////////



using UnityEngine;
using System.Collections;

public class IOSStoreKitResult : ISN_Result  {

	private string _ProductIdentifier  = string.Empty;
	private InAppPurchaseState _State = InAppPurchaseState.Failed;
	private string _Receipt  = string.Empty;
	private string _TransactionIdentifier = string.Empty;
	private string _ApplicationUsername = string.Empty;


	//--------------------------------------
	// Initialize
	//--------------------------------------

	public IOSStoreKitResult(string productIdentifier, ISN_Error e): base(e) {
		_ProductIdentifier = productIdentifier;
		_State = InAppPurchaseState.Failed;
	}

	public IOSStoreKitResult(string productIdentifier, InAppPurchaseState state, string applicationUsername = "", string receipt = "", string transactionIdentifier = ""):base(true) {
		_ProductIdentifier = productIdentifier;
		_State = state;
		_Receipt = receipt;
		_TransactionIdentifier = transactionIdentifier;
		_ApplicationUsername = applicationUsername;
	}


	//--------------------------------------
	// Get / Set
	//--------------------------------------


	public IOSTransactionErrorCode TransactionErrorCode {
		get {
			if(_Error != null) {
				return (IOSTransactionErrorCode) _Error.Code;
			} else {
				return IOSTransactionErrorCode.SKErrorNone;
			}
			
		}
	}

	public InAppPurchaseState State {
		get {
			return _State;
		}
	}

	public string ProductIdentifier {
		get {
			return _ProductIdentifier;
		}
	}

	//An opaque identifier for the user’s account on your system. 
	//This is used to help the store detect irregular activity. 
	//For example, in a game, it would be unusual for dozens of different iTunes Store accounts making purchases on behalf of the same in-game character.
	//The recommended implementation is to use a one-way hash of the user’s account name to calculate the value for this property.
	public string ApplicationUsername {
		get {
			return _ApplicationUsername;
		}
	}

	public string Receipt {
		get {
			return _Receipt;
		}
	}

	public string TransactionIdentifier {
		get {
			return _TransactionIdentifier;
		}
	}
}
