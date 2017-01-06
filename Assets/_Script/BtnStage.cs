using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class BtnStage : MonoBehaviour {

	private Button m_Btn;

		// Use this for initialization
	void Awake () {
		var index = transform.GetSiblingIndex ();
		m_Btn = gameObject.GetComponent<Button> ();
		m_Btn.onClick.AddListener (() => {
			Debug.Log (index);
		});
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
