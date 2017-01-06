using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

[AddComponentMenu("HaaaqiTool/SenceSwitcher")]
public class SceneSwitcher : MonoBehaviour {

	public string TargetSence;

	void Awake () {
		GetComponent<Button> ().onClick.AddListener (() => {
			SceneManager.LoadScene(TargetSence);
		});
	}
}