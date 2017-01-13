using UnityEngine;
using System.Collections;

public class ChoseHeroCsItem : MonoBehaviour {
	public UISprite _CosIcon = null;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void SetUI(int CosID,string _SpriteName=null)
	{
		if (CosID==-1) 
		{
			gameObject.SetActive (false);
			return;
		}
		//string SpriteName = string.Empty;
		//UIAtlas UseAtlas = null;

		//IconManager.Get ().GetIcon (ref UseAtlas, ref SpriteName, CosID);
		//_CosIcon.atlas = UseAtlas;
		_CosIcon.spriteName = _SpriteName;
		_CosIcon.MakePixelPerfect ();

		UIEventListener.Get (gameObject).onClick = CosButtonOnclick;
	}

	public void CosButtonOnclick(GameObject button)
	{
		Debug.LogError ("未添加 皮肤点击处理");
	}
}
