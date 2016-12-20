////////////////////////////////////////////////////////////////////////////////
//  
// @module IOS Native Plugin for Unity3D 
// @author Osipov Stanislav (Stan's Assets) 
// @support stans.assets@gmail.com 
//
////////////////////////////////////////////////////////////////////////////////



using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class iCloudData  {


	private string _key;
	private string _val;

	private bool _IsEmpty = false;

	
	//--------------------------------------
	// INITIALIZE
	//--------------------------------------

	public iCloudData(string k, string v) {
		_key = k;
		_val = v;

		if(_val.Equals("null")) {
			if(!IOSNativeSettings.Instance.DisablePluginLogs) 
				Debug.Log ("empty set");
			_IsEmpty = true;
		}
	}

	//--------------------------------------
	//  PUBLIC METHODS
	//--------------------------------------
	
	//--------------------------------------
	//  GET/SET
	//--------------------------------------

	public string key {
		get {
			return _key;
		}
	}

	public string stringValue {
		get {
			if(_IsEmpty) {
				return null;
			}

			return _val;
		}
	}

	public float floatValue {
		get {

			if(_IsEmpty) {
				return 0f;
			}

			return System.Convert.ToSingle (_val);
		}
	}

	public byte[] bytesValue {
		get {

			if(_IsEmpty) {
				return null;
			}

			string[] array;
			array = _val.Split("," [0]);

			List<byte> l = new List<byte> ();
			foreach(string s in array) {
				l.Add (System.Convert.ToByte(s));
			}

			return l.ToArray ();
		}
	}


	public bool IsEmpty {
		get {
			return _IsEmpty;
		}
	}

	
	//--------------------------------------
	//  EVENTS
	//--------------------------------------
	
	//--------------------------------------
	//  PRIVATE METHODS
	//--------------------------------------
	
	//--------------------------------------
	//  DESTROY
	//--------------------------------------

}
