using UnityEngine;
using System.Collections;

public class ChoseHeroActerAtrrItem : MonoBehaviour {
	public UILabel _AtrrNameLable = null;

	public UILabel _AtrrValueLabel=null;

	// Use this for initialization
	void Start () 
	{
	
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}

	public void SetData(ChoseHeroDefine.PlayerAttr _data)
	{
		_AtrrNameLable.text = _data._Name;
		switch (_data._tYpe) 
		{
		case ChoseHeroDefine.PLAYER_ATTRITE.BAOJI_P:
			_AtrrValueLabel.text = "+" + _data._Value.ToString () + "%";
			break;
		case ChoseHeroDefine.PLAYER_ATTRITE.GOLD_GET:
			_AtrrValueLabel.text = "+" + _data._Value.ToString () + "%";
			break;
		case ChoseHeroDefine.PLAYER_ATTRITE.LEVEL_MAX:
			_AtrrValueLabel.text = _data._Value.ToString();
			break;
		case ChoseHeroDefine.PLAYER_ATTRITE.TILI_MAX:
			_AtrrValueLabel.text =_data._Value.ToString ();
			break;
		default:
			break;
		}
	}
}
