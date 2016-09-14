using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class LevelAudioQuan : MonoBehaviour {
	public GameObject _UIPrefab=null;
	public List<GameObject> _Pool = new List<GameObject> ();
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () 
	{
//		if (Input.GetKeyDown (KeyCode.B)) 
//		{
//			CreateQuan ();
//		}
	}

	public void CreateQuan()
	{
		if (_UIPrefab != null) {
			if (_Pool.Count > 0) {
				GameObject ui = _Pool [0];
				_Pool.RemoveAt (0);
				ui.SetActive (true);
				ui.GetComponent<TweenScale> ().ResetToBeginning ();
				ui.GetComponent<TweenScale> ().PlayForward ();

				ui.GetComponent<TweenAlpha> ().ResetToBeginning ();
				ui.GetComponent<TweenAlpha> ().PlayForward ();
			} 
			else 
			{
				GameObject ui = Instantiate (_UIPrefab) as GameObject;
				ui.SetActive (true);
				ui.transform.parent = gameObject.transform;
				ui.transform.localScale = Vector3.one;
				ui.transform.localPosition = Vector3.zero;
				ui.GetComponent<TweenScale> ().ResetToBeginning ();
				ui.GetComponent<TweenScale> ().PlayForward ();

				ui.GetComponent<TweenAlpha> ().ResetToBeginning ();
				ui.GetComponent<TweenAlpha> ().PlayForward ();
			}
		} 
	}

	public void ReturnUI(GameObject _ui)
	{
		_Pool.Add (_ui);
		_ui.SetActive (false);
	}

	private float _OldValue=1f;
	public float  _CreateValue=0.3f;
	public void SetAudioValue(float _value)
	{
		//Debug.Log (_value.ToString());
		if (_value - _OldValue >= _CreateValue||_OldValue-_value>=_CreateValue) 
		{
			CreateQuan ();
			_OldValue = _value;
		} else 
		{
			_OldValue = _value;
		}
	}
}
