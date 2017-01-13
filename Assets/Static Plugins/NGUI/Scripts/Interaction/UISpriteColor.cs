using UnityEngine;
using System.Collections;
[ExecuteInEditMode]
[AddComponentMenu("NGUI/Interaction/ExCs/SpriteColor")]
public class UISpriteColor : MonoBehaviour {
	public UISprite _HandleSpr = null;

	public Color _UseColor = Color.white;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (_HandleSpr != null)
			_HandleSpr.color = _UseColor;
	}

	void OnEnable()
	{
		if (_HandleSpr != null)
			_HandleSpr.color = _UseColor;
	}
}
