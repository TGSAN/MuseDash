using UnityEngine;
using System.Collections;

public enum IOSTransactionErrorCode  {

	SKErrorUnknown = 0,
	SKErrorClientInvalid = 1,               // client is not allowed to issue the request, etc.
	SKErrorPaymentCancelled = 2,            // user cancelled the request, etc.
	SKErrorPaymentInvalid = 3,              // purchase identifier was invalid, etc.
	SKErrorPaymentNotAllowed = 4,           // this device is not allowed to make the payment
	SKErrorStoreProductNotAvailable = 5,    // Product is not available in the current storefront
	SKErrorPaymentNoPurchasesToRestore = 6,  // No purchases to restore"
	SKErrorPaymentServiceNotInitialized = 7,  //StoreKit initialization required
	SKErrorNone = 8 //No error occurred
}
