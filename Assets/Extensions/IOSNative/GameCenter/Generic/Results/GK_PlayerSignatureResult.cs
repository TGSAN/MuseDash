using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class GK_PlayerSignatureResult : ISN_Result {
	
	public string _PublicKeyUrl;
	public byte[] _Signature;
	public byte[] _Salt;
	public long _Timestamp;


	public GK_PlayerSignatureResult(ISN_Error er):base(false) {
		_Error = er;
	}


	public GK_PlayerSignatureResult(string publicKeyUrl, string signature, string salt, string timestamp):base(true) {
		_PublicKeyUrl = publicKeyUrl;


		string[] array;
		array = signature.Split(',');
		
		List<byte> l = new List<byte> ();
		foreach(string s in array) {
			l.Add (System.Convert.ToByte(s));
		}
		
		_Signature = l.ToArray ();



		array = salt.Split(',');
		
		l = new List<byte> ();
		foreach(string s in array) {
			l.Add (System.Convert.ToByte(s));
		}
		_Salt = l.ToArray ();


		_Timestamp = System.Convert.ToInt64(timestamp);


	}

	//The URL for the public encryption key.
	public string PublicKeyUrl {
		get {
			return _PublicKeyUrl;
		}
	}

	//The verification signature data generated.
	public byte[] Signature {
		get {
			return _Signature;
		}
	}


	//A random string used to compute the hash and keep it randomized.
	public byte[] Salt {
		get {
			return _Salt;
		}
	}

	//The date and time that the signature was created.
	public long Timestamp {
		get {
			return _Timestamp;
		}
	}
}
