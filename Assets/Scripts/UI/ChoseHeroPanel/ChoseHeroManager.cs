using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using FormulaBase;
public class ChoseHeroManager
{
	static ChoseHeroManager S_Instance = null;

	public static ChoseHeroManager Get()
	{
		if (S_Instance == null) 
		{
			S_Instance = new ChoseHeroManager ();
		}
		return S_Instance;
	}
	/// <summary>
	/// 返回角色的名字
	/// </summary>
	/// <returns>The player name.</returns>
	/// <param name="_tYpe">T ype.</param>xx
	public string GetActorName(ChoseHeroDefine.PLAYR_TYPE _tYpe)
	{
		
		return RoleManageComponent.Instance.GetName((int)_tYpe);
	}
	/// <summary>
	/// 返回角色的名字描述
	/// </summary>
	/// <returns>The player name text.</returns>
	/// <param name="_tYpe">T ype.</param>xx
	public string GetActorNameText(ChoseHeroDefine.PLAYR_TYPE _tYpe)
	{
		
		return RoleManageComponent.Instance.GetDes((int)_tYpe);
	}
	/// <summary>
	/// 返回角色的对应的属性能力  xxx
	/// </summary>
	/// <returns>The player atrrs.</returns>
	/// <param name="_tYpe">T ype.</param>  
	public ChoseHeroDefine.PlayerAttr[] GetActorAtrrs(ChoseHeroDefine.PLAYR_TYPE _tYpe)
	{
		ChoseHeroDefine.PlayerAttr[] _ReturnData = new ChoseHeroDefine.PlayerAttr[4]
		{ 
			new ChoseHeroDefine.PlayerAttr(),
			new ChoseHeroDefine.PlayerAttr(),
			new ChoseHeroDefine.PlayerAttr(),
			new ChoseHeroDefine.PlayerAttr()
		};

		_ReturnData [0]._Name = "等级上限";

		_ReturnData [0]._tYpe = ChoseHeroDefine.PLAYER_ATTRITE.LEVEL_MAX;
		//_ReturnData [0]._Value	 = 100;

		_ReturnData [1]._Name = "暴击概率";
		_ReturnData [1]._tYpe = ChoseHeroDefine.PLAYER_ATTRITE.BAOJI_P;
		//_ReturnData [1]._Value = 100;

		_ReturnData [2]._Name = "金币获取";
		_ReturnData [2]._tYpe = ChoseHeroDefine.PLAYER_ATTRITE.GOLD_GET;
	//	_ReturnData [2]._Value = 100;

		_ReturnData [3]._Name = "体力上限";
		_ReturnData [3]._tYpe = ChoseHeroDefine.PLAYER_ATTRITE.TILI_MAX;
	//	_ReturnData [3]._Value = 100;
		RoleManageComponent.Instance.SetHeroAttribute((int)_tYpe,ref _ReturnData [0]._Value,ref _ReturnData [1]._Value,ref _ReturnData [2]._Value, ref _ReturnData [3]._Value);
		return _ReturnData;
	}
	/// <summary>
	/// 返回角色的皮肤ID
	/// </summary>
	/// <returns>The cos identifier.</returns>
	/// <param name="_tYpe">T ype.</param>
	public int[] GetCosIDS(ChoseHeroDefine.PLAYR_TYPE _tYpe)
	{
		return new int[]{10001,1002,1003,1004};
	}
	/// <summary>
	/// 返回角色的购买钻石
	/// </summary>
	/// <returns>The player buy demond.</returns>
	/// <param name="_tYpe">T ype.</param>xx
	public int GetActorBuyDemond(ChoseHeroDefine.PLAYR_TYPE _tYpe)
	{
		int ttype=0;//类型 1=钱 2=钻石
		int tCost=0;//消耗
		RoleManageComponent.Instance.GetUnLockRoleMoeny((int)_tYpe,ref ttype,ref tCost );

		return tCost;
	}
	/// <summary>
	/// 返回角色的是否已经获得
	/// </summary>
	/// <returns><c>true</c>, if actor is buy was gotten, <c>false</c> otherwise.</returns>
	/// <param name="_tYpe">T ype.</param>xx
	public ChoseHeroDefine.RESULT_EQUIP GetActorIsBuy(ChoseHeroDefine.PLAYR_TYPE _tYpe)
	{
		return RoleManageComponent.Instance.GetRoleState((int)_tYpe);
	}

	//private ChoseHeroDefine.PLAYR_TYPE _ChosePlayer=ChoseHeroDefine.PLAYR_TYPE.PLAYER_LUMI;
	/// <summary>
	/// 返回玩家装备的角色
	/// </summary>
	/// <returns>The player acter.</returns>xx
	public ChoseHeroDefine.PLAYR_TYPE GetPlayerActer()
	{
		
		return (ChoseHeroDefine.PLAYR_TYPE)RoleManageComponent.Instance.GetFightGirlIndex();
	}
	/// <summary>
	/// 设置玩家选择的角色
	/// </summary>
	/// <returns><c>true</c>, if chose herp was set, <c>false</c> otherwise.</returns>XX
	public  void SetChoseHero(ChoseHeroDefine.PLAYR_TYPE _type,Callback _callBack=null)
	{
		RoleManageComponent.Instance.SetFightGirlIndex((int)_type,_callBack);
	//	_ChosePlayer=_type;
	}
	private Dictionary<ChoseHeroDefine.PLAYR_TYPE,GameObject> _PlayerModleDic = new Dictionary<ChoseHeroDefine.PLAYR_TYPE, GameObject> ();
	public GameObject LoadPlayerActor(ChoseHeroDefine.PLAYR_TYPE _type)
	{
		if (_PlayerModleDic.ContainsKey (_type)) 
		{
			return _PlayerModleDic[_type];
		}
		///进行加载 对应的预制体

		return null;
	}

	public GameObject UI=null;
	/// <summary>
	/// 购买角色
	/// </summary>
	/// <param name="_type">Type.</param>
	public void BuyHero(ChoseHeroDefine.PLAYR_TYPE _type)
	{
		RoleManageComponent.Instance.UnlockRole ((int)_type, () => 
			{
				if(UI!=null)
				{
					UI.GetComponent<ChoseHeroPanel>().InitUI(_type);
				}
			});
	}
	/// <summary>
	/// 预加载对象的预制体模型
	/// </summary>
	public void  InitLoadModle()
	{
		
		GameObject Malya = Resources.Load<GameObject> ("UIResource/Model/UIMinPrefab/Malya");
		if (Malya != null) 
		{
			if(!_PlayerModleDic.ContainsKey(ChoseHeroDefine.PLAYR_TYPE.PLAYER_MALYA))
				_PlayerModleDic.Add (ChoseHeroDefine.PLAYR_TYPE.PLAYER_MALYA, Malya);
		}

		GameObject Buro = Resources.Load<GameObject> ("UIResource/Model/UIMinPrefab/Buro");
		if (Buro != null) 
		{
			if(!_PlayerModleDic.ContainsKey(ChoseHeroDefine.PLAYR_TYPE.PLAYER_BURO))
				_PlayerModleDic.Add (ChoseHeroDefine.PLAYR_TYPE.PLAYER_BURO, Buro);
		}

		GameObject Lumi = Resources.Load<GameObject> ("UIResource/Model/UIMinPrefab/Lumi");
		if (Lumi != null) 
		{
			if(!_PlayerModleDic.ContainsKey(ChoseHeroDefine.PLAYR_TYPE.PLAYER_LUMI))
				_PlayerModleDic.Add (ChoseHeroDefine.PLAYR_TYPE.PLAYER_LUMI, Lumi);
		}
	}
}
