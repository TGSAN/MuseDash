using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SpineSynchroObjects : MonoBehaviour {
	[SerializeField]
	public List<GameObject> synchroObjects;

	void Start () {
		SpineActionController sac = this.gameObject.GetComponent<SpineActionController> ();
		if (sac == null) {
			return;
		}

		sac.SetSynchroObjects (this.synchroObjects);
	}
}
