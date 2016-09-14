using UnityEngine;
using System.Collections;
public class ComboNumberShow : MonoBehaviour {
	public enum FontType
	{
		FONT_1=0,
		FONT_2,
		FONT_3,
		FONT_4,
		FONT_5,
		FONT_6,
		FONT_7
	}
	public UIFont[] _Fonts=null;
	public UILabel  _TextLabel=null;
	public Animation _NumAnimation=null;

	public string _Animation01str = string.Empty;
	public string _Animation02str =string.Empty;
	public string _Animation03str= string.Empty;
	public string _Animation04str=	string.Empty;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () 
	{
//		if (Input.GetKeyDown (KeyCode.A)) 
//		{
//			SetText ("+-123", FontType.FONT_1);
//		}
//
//		if (Input.GetKeyDown (KeyCode.B)) 
//		{
//			SetText ("+-123", FontType.FONT_2);
//		}
//
//		if (Input.GetKeyDown (KeyCode.C)) 
//		{
//			SetText ("+-123", FontType.FONT_3);
//		}
//
//		if (Input.GetKeyDown (KeyCode.D)) 
//		{
//			SetText ("+-123", FontType.FONT_4);
//		}
//		if (Input.GetKeyDown (KeyCode.E)) 
//		{
//			SetText ("+-123", FontType.FONT_5);
//		}
	}
	public void SetText(string _str,FontType _fontype,int FontSize=47)
	{
		_TextLabel.bitmapFont=_Fonts[(int)_fontype];
		_TextLabel.text = _str;
		_TextLabel.fontSize = FontSize;

		switch (_fontype) 
		{
		case FontType.FONT_1:
		case FontType.FONT_2:
		case FontType.FONT_3:
			_NumAnimation.Stop ();
			_NumAnimation.Play (_Animation01str);
			break;
		case FontType.FONT_4:
			_NumAnimation.Stop ();
			_NumAnimation.Play (_Animation02str);
			break;
		case FontType.FONT_5:
			_NumAnimation.Stop ();
			_NumAnimation.Play (_Animation03str);
			break;
		case FontType.FONT_6:
		case FontType.FONT_7:
			_NumAnimation.Stop ();
			_NumAnimation.Play (_Animation04str);
			break;
		}

	}
}
