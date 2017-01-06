using UnityEngine;
using System.Collections;

public class Note : MonoBehaviour {

	// Use this for initialization
	void Start () {
		var inputMnger = FindObjectOfType<InputManager> ();
		inputMnger.notes.Add (gameObject);
		gameObject.name = inputMnger.notes.Count.ToString();
	}

	private void OnDestroy()
	{
		var inputMnger = FindObjectOfType<InputManager> ();
		inputMnger.notes.Remove(gameObject);
	}

}
