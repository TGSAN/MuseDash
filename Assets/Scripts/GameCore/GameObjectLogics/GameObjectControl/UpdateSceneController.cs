using UnityEngine;
using System.Collections;

public class UpdateSceneController : MonoBehaviour {
	[SerializeField]
	public string JumpSceneAfterUpdate;
	// Use this for initialization
	void Start () {
		AssetBundleFileMangager.Get ().InitGameConfig ();
	}
	
	// Update is called once per frame
	void Update () {
		if (AssetBundleFileMangager.Get ().IsLoadAll) {
			Application.LoadLevel (this.JumpSceneAfterUpdate);
		}
	}
}
