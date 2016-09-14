using UnityEngine;
using System.Collections;
[ExecuteInEditMode]
public class LableColor : MonoBehaviour {

	public UILabel _HandleLabel = null;

	public Color _UseColor = Color.white;
	// Use this for initialization
	void Start () {

	}

	// Update is called once per frame
	void Update () 
	{
		if (_HandleLabel != null)
			_HandleLabel.color = _UseColor;
	}

	void OnEnable()
	{
		if (_HandleLabel != null)
			_HandleLabel.color = _UseColor;
	}
}
