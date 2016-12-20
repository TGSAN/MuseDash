using UnityEngine;
using System.Collections;


#if UNITY_4_6 || UNITY_5_0 || UNITY_5_1 || UNITY_5_2
#else
using UnityEngine.SceneManagement;
#endif


public class IOSNativePreviewBackButton : BaseIOSFeaturePreview {


	private string initialSceneName = "scene";

	public static IOSNativePreviewBackButton Create() {
		return new GameObject("BackButton").AddComponent<IOSNativePreviewBackButton>();
	} 


	void Awake() {
		DontDestroyOnLoad(gameObject);
		initialSceneName = loadedLevelName;
	}


	void OnGUI() {
		float bw = 120;
		float x = Screen.width - bw * 1.2f ;
		float y = bw * 0.2f;


		if(!loadedLevelName.Equals(initialSceneName)) {
			Color customColor = GUI.color;
			GUI.color = Color.green;

			if(GUI.Button(new Rect(x, y, bw, bw * 0.4f), "Back")) {
				LoadLevel(initialSceneName);
			}

			GUI.color = customColor;

		}
	}

	public string loadedLevelName {
		get {
			#if UNITY_4_6 || UNITY_5_0 || UNITY_5_1 || UNITY_5_2
			return Application.loadedLevelName;
			#else
			return SceneManager.GetActiveScene().name;
			#endif

		}
	}
	 


}
