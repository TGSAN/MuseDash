#define INAPP_API_ENABLED
////////////////////////////////////////////////////////////////////////////////
//  
// @module IOS Native Plugin for Unity3D 
// @author Osipov Stanislav (Stan's Assets) 
// @support stans.assets@gmail.com 
//
////////////////////////////////////////////////////////////////////////////////
/// 
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
#if (UNITY_IPHONE && !UNITY_EDITOR && INAPP_API_ENABLED) || SA_DEBUG_MODE
using System.Runtime.InteropServices;
#endif


public class IOSStoreProductView {

	public event Action Loaded = delegate {};
	public event Action LoadFailed = delegate {};
	public event Action Appeared = delegate {};
	public event Action Dismissed = delegate {};




	#if (UNITY_IPHONE && !UNITY_EDITOR && INAPP_API_ENABLED) || SA_DEBUG_MODE
	[DllImport ("__Internal")]
	private static extern void _createProductView(int viewId, string productsId);
	
	[DllImport ("__Internal")]
	private static extern void _showProductView(int viewId);
	#endif


	private List<string> _ids =  new List<string>();

	private int _id;


	//--------------------------------------
	// INITIALIZE
	//--------------------------------------


	public IOSStoreProductView() {
		foreach(string pid in IOSNativeSettings.Instance.DefaultStoreProductsView) {
			addProductId(pid);
		}

		IOSInAppPurchaseManager.Instance.RegisterProductView(this);
	}

	public IOSStoreProductView(params string[] ids) {
		foreach(string pid in ids) {
			addProductId(pid);
		}

		IOSInAppPurchaseManager.Instance.RegisterProductView(this);
	}


	//--------------------------------------
	// PUBLIC METHODS
	//--------------------------------------

	public void addProductId(string productId) {
		if(_ids.Contains(productId)) {
			return;
		}
		
		_ids.Add(productId);
	}

	

	public void Load() {
		#if (UNITY_IPHONE && !UNITY_EDITOR && INAPP_API_ENABLED) || SA_DEBUG_MODE
			string ids = "";
			int len = _ids.Count;
			for(int i = 0; i < len; i++) {
				if(i != 0) {
					ids += ",";
				}
				
				ids += _ids[i];
			}

			_createProductView(id, ids);
		#endif
	}

	public void Show() {
		#if (UNITY_IPHONE && !UNITY_EDITOR  && INAPP_API_ENABLED) || SA_DEBUG_MODE
			_showProductView(id);
		#endif
	}

	
	//--------------------------------------
	// GET / SET
	//--------------------------------------

	public int id {
		get {
			return _id;
		}
	}


	//--------------------------------------
	// EVENTS
	//--------------------------------------

	public void OnProductViewAppeard() {
		Appeared();
	}

	public void OnProductViewDismissed() {
		Dismissed();
	}

	public void OnContentLoaded() {
		Show();
		Loaded();
	}

	public void OnContentLoadFailed() {
		LoadFailed();
	}

	//--------------------------------------
	// PRIVATE METHODS
	//--------------------------------------

	public void SetId(int viewId) {
		_id = viewId;
	}



}
