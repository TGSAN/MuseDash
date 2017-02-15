using UnityEngine;
using System.Collections;
using AssetBundleDefine;
public class AssetBundleLoad : MonoBehaviour {
	public delegate AudioClip AudioLoadCallBack (AudioClip Res,string Path) ;
	public static AssetBundleLoad S_Instance=null;
	// Use this for initialization
	void Start () 
	{
		
	}

	//void Awake()
	//{
		//LoadAudioRes ();
	//}
	// Update is called once per frame
	//void Update () 
	//{
	//	
	//}

	public static AssetBundleLoad Get() {
		if (S_Instance == null) {
			GameObject LoadResTool = new GameObject ();
			S_Instance = LoadResTool.AddComponent<AssetBundleLoad> ();
			GameObject.DontDestroyOnLoad (LoadResTool);
		}

		return S_Instance;
	}

	public void wwwLoadAssetBundleInterface(string url, wwwLoadCallBack<WWW> wwwCallBack) {
		this.StartCoroutine (this.wwwLoadAssetBundle (url, wwwCallBack));
	}

	private IEnumerator wwwLoadAssetBundle(string url, wwwLoadCallBack<WWW> wwwCallBack) {
		WWW __loadwww = new WWW (url);
		yield return __loadwww;

		if (__loadwww.error != null) {
			if (wwwCallBack != null)
				wwwCallBack (null);
			Debug.LogWarning (__loadwww.error);
		} else {
			if (wwwCallBack != null)
				wwwCallBack (__loadwww);
		}
	}
}