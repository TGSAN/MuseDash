using UnityEngine;
using System.Collections;
[ExecuteInEditMode]
[AddComponentMenu("NGUI/Interaction/ExCs/SpriteFillAmount")]
public class UISpriteFillAmount : MonoBehaviour {
	public UISprite _HandleCs=null;

	public float _Value = 1.0f;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (_HandleCs != null)
			_HandleCs.fillAmount = _Value;
	}
}
