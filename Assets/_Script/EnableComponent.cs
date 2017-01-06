using UnityEngine;
using System.Collections;

public class EnableComponent : MonoBehaviour {

	public Light _Light;

	// Update is called once per frame
	void Update () {
		if(Input.GetKeyDown(KeyCode.Space))
		_Light.enabled = !_Light.enabled;
	}
}
