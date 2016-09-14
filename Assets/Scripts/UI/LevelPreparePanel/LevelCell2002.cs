using UnityEngine;
using System.Collections;

public class LevelCell2002 : MonoBehaviour {
	public GameObject[] _Sprites = null;

	public float _CDTime = 0.2f;

	private float _KeepTime = 0f;

	private bool  _AnimationState=false;
	private int   _Index=0;
	private int _UseIndex = 0;
	// Use this for initialization
	void Start ()
	{
	
	}

	
	// Update is called once per frame
	void Update () 
	{
//		if (Input.GetKeyDown (KeyCode.B))
//			PlayAnimaiton (20);
		Logic ();
	}

	public void PlayAnimaiton(int value)
	{
		if (value > _Sprites.Length) 
		{
			value = _Sprites.Length;
		}
		_UseIndex = value-1;
		for (int i = 0; i < _Sprites.Length; i++) 
		{
			_Sprites [i].SetActive (false);
		}
		_AnimationState = true;
		_Index = -1;
	}

	void Logic()
	{
		if (_AnimationState)
		{
			_KeepTime += Time.deltaTime;
			if (_KeepTime > _CDTime) 
			{
				_Index++;
				if (_Index >_UseIndex)
				{
					_AnimationState = false;
					_KeepTime = 0f;
					_UseIndex = 0;
					_Index = 0;
					return;
				}
				_Sprites [_Index].SetActive (true);
				_KeepTime = 0f;
			}
		}
	}
}
