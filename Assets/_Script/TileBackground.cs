using UnityEngine;
using System.Collections;

public class TileBackground : MonoBehaviour {

	public static float PixelToUnit = 100f;
	public int TextureSize = 128;

	// Use this for initialization
	void Start () {
		var NewWidth = Mathf.Ceil (Screen.width / (TextureSize * PixelPerfectCamera.scale));
		transform.localScale = new Vector3 ((NewWidth * TextureSize) / PixelToUnit, 1, 1);
		GetComponent<Renderer> ().material.mainTextureScale = new Vector3 (NewWidth, 1, 1);
	}

}
