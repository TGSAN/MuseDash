////////////////////////////////////////////////////////////////////////////////
//  
// @module Android Native Plugin for Unity3D 
// @author Osipov Stanislav (Stan's Assets) 
// @support stans.assets@gmail.com 
//
////////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections;
using System.Collections.Generic;


#if UNITY_4_6 || UNITY_5_0 || UNITY_5_1 || UNITY_5_2
#else
using UnityEngine.SceneManagement;
#endif



//Attach the script to the empty gameobject on your sceneS
public class iAdIOSBanner : MonoBehaviour {
	
	public TextAnchor anchor = TextAnchor.LowerCenter;


	private static Dictionary<string, iAdBanner> _registeredBanners = null;


	// --------------------------------------
	// Unity Events
	// --------------------------------------


	void Start() {
		ShowBanner();
	}

	void OnDestroy() {
		HideBanner();
	}


	// --------------------------------------
	// PUBLIC METHODS
	// --------------------------------------

	public void ShowBanner() {
		iAdBanner banner;

		if(registeredBanners.ContainsKey(sceneBannerId)) {
			banner = registeredBanners[sceneBannerId];
		}  else {
			banner = iAdBannerController.instance.CreateAdBanner(anchor);
			registeredBanners.Add(sceneBannerId, banner);
		}

		if(banner.IsLoaded && !banner.IsOnScreen) {
			banner.Show();
		}
	}

	public void HideBanner() {
		if(registeredBanners.ContainsKey(sceneBannerId)) {
			iAdBanner banner = registeredBanners[sceneBannerId];
			if(banner.IsLoaded) {
				if(banner.IsOnScreen) {
					banner.Hide();
				}
			} else {
				banner.ShowOnLoad = false;
			}
		}
	}

	// --------------------------------------
	// GET / SET
	// --------------------------------------


	public static Dictionary<string, iAdBanner> registeredBanners {
		get {
			if(_registeredBanners == null) {
				_registeredBanners = new Dictionary<string, iAdBanner>();
			}

			return _registeredBanners;
		}
	}

	public string sceneBannerId {
		get {
			#if UNITY_4_6 || UNITY_5_0 || UNITY_5_1 || UNITY_5_2
			return Application.loadedLevelName + "_" + this.gameObject.name;
			#else
			return SceneManager.GetActiveScene().name + "_" + this.gameObject.name;
			#endif



		}
	}

	
}
