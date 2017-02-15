using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class ChoseHeroPanel : UIPanelBase {
	// Use this for initialization
	void Start () 
	{
		
		//ShowPanel ();

	}
	
	// Update is called once per frame
	void Update () 
	{
		
	}
		
	public override void PanelClickBack()
	{
		
	}
	private int ActorIndex = -1;
	public override void ShowPanel(int _state=1,FormulaBase.FormulaHost _host=null,int _Layer=0)
	{
		ChoseHeroManager.Get ().InitLoadModle ();
		ChoseHeroManager.Get ().UI = gameObject;
		ChoseHeroDefine.PLAYR_TYPE _PlayerType = ChoseHeroManager.Get ().GetPlayerActer ();
		InitUI (_PlayerType);
	}
	/// <summary>
	/// 设置对应角色的名字+描述
	/// </summary>
	public UILabel  _NameLabel=null;
	public UILabel  _PlayeTextLabel=null;
	public void SetActorText(string PlayerName,string PlayeText)
	{
		if (_NameLabel != null) 
		{
			_NameLabel.text = PlayerName;
		}
		if (_PlayeTextLabel != null) 
		{
			_PlayeTextLabel.text = PlayeText;
		}
	}
	private Dictionary<ChoseHeroDefine.PLAYR_TYPE,GameObject> _PlayerDic = new Dictionary<ChoseHeroDefine.PLAYR_TYPE, GameObject> ();
	public GameObject _ActiveObj=null;
	public void SetPlayerIcon(ChoseHeroDefine.PLAYR_TYPE _tYpe)
	{
		if (_PlayerDic.ContainsKey (_tYpe)) {
			
			if (_ActiveObj != null)
				_ActiveObj.SetActive (false);
			_ActiveObj = _PlayerDic [_tYpe];
			_PlayerDic [_tYpe].SetActive (true);
		} else 
		{
			//加载该模型的对象
			GameObject _LoadObj=ChoseHeroManager.Get().LoadPlayerActor(_tYpe);
			if (_LoadObj != null) {
				GameObject ui = Instantiate<GameObject> (_LoadObj);
				ui.transform.parent = gameObject.transform.FindChild ("GameObject/PlayerPanel/PlayerPot");
				ui.transform.localScale = Vector3.one;
				ui.transform.localPosition = Vector3.zero;

				_PlayerDic.Add (_tYpe, ui);
				if(_ActiveObj!=null)
					_ActiveObj.SetActive (false);
				_ActiveObj = ui;
				ui.SetActive (true);
			} else {
				Debug.LogError ("加载未获得对象");
			}
		}
	}
	public GameObject _BackButton = null;
	/// <summary>
	/// 返回按钮点击处理
	/// </summary>
	/// <param name="button">Button.</param>
	public void BackButtonOnclick(GameObject button)
	{
		ChoseHeroManager.Get ().UI = null;
		UIManageSystem.g_Instance.RomoveUI ();
		UIManageSystem.g_Instance.ShowUI ();
	}
	/// <summary>
	/// 初始化UI数据
	/// </summary>
	public void InitUI(ChoseHeroDefine.PLAYR_TYPE _Type)
	{
		ActorIndex = (int)_Type;
		UIEventListener.Get (_BackButton).onClick = BackButtonOnclick;
		UIEventListener.Get (_AddButton).onClick = AddButtonOnclick;
		UIEventListener.Get (_CubeButton).onClick = CubeButtonOnclick;
		//设置描述
		SetActorText(ChoseHeroManager.Get().GetActorName(_Type),ChoseHeroManager.Get().GetActorNameText(_Type));
		//设置展示图
		SetPlayerIcon(_Type);
		//设置属性
		SetActerAttr(_Type);
		//设置底板
		SetBottomPanel(_Type);
		//设置皮肤
		SetActorUI(_Type);
	}
	public GameObject _AddButton=null;
	/// <summary>
	/// 添加的按钮的点击处理
	/// </summary>
	/// <param name="button">Button.</param>
	public void AddButtonOnclick(GameObject button)
	{
		if (ActorIndex == (int)ChoseHeroDefine.PLAYR_TYPE.PLAYER_MAX-1) 
		{
			ActorIndex = 0;
		}
		else
			ActorIndex += 1;
		InitUI ((ChoseHeroDefine.PLAYR_TYPE)ActorIndex);
		//ChoseHeroManager.Get ().SetChoseHero ((ChoseHeroDefine.PLAYR_TYPE)ActorIndex);
	}

	public  GameObject _CubeButton = null;
	/// <summary>
	/// 减少按钮的点就处理
	/// </summary>
	/// <param name="button">Button.</param>
	public void CubeButtonOnclick(GameObject button)
	{
		if (ActorIndex == 0) 
		{
			ActorIndex = (int)ChoseHeroDefine.PLAYR_TYPE.PLAYER_MAX-1;
		}
		else
			ActorIndex -= 1;
		InitUI ((ChoseHeroDefine.PLAYR_TYPE)ActorIndex);
		//ChoseHeroManager.Get ().SetChoseHero ((ChoseHeroDefine.PLAYR_TYPE)ActorIndex);
	}
	public ChoseHeroCsItem[] _CosUI = null;
	/// <summary>
	/// 设置角色的皮肤数据
	/// </summary>
	public void SetActorUI(ChoseHeroDefine.PLAYR_TYPE _Type)
	{
		int[] CosIDs = ChoseHeroManager.Get ().GetCosIDS (_Type);
		for (int i = 0; i < 4; i++) 
		{
			_CosUI [i].SetUI (CosIDs [i],"cs_cos_buro_2");
		}
	}
	public ChoseHeroActerAtrrItem[] _AtrrItems=null;
	public void SetActerAttr(ChoseHeroDefine.PLAYR_TYPE _Type)
	{
		//设置等级上限
		ChoseHeroDefine.PlayerAttr[] _ReturnData=ChoseHeroManager.Get().GetActorAtrrs(_Type);
		_AtrrItems [0].SetData (_ReturnData [0]);
		_AtrrItems [1].SetData (_ReturnData [1]);
		_AtrrItems [2].SetData (_ReturnData [2]);
		_AtrrItems [3].SetData (_ReturnData [3]);
	}
	public UIToggle[] _PotToggles=null;
	public GameObject _BuyValueUI=null;
	public GameObject _BuyButton=null;
	public UILabel    _BuyValueLabel=null;
	public UILabel    _BuyButtonTextLabel=null;
	public GameObject _HaveUseUI=null;
	/// <summary>
	/// 设置底部的UI数据
	/// </summary>
	public void SetBottomPanel(ChoseHeroDefine.PLAYR_TYPE _Type)
	{
		///设置对应的红点显示
		_PotToggles [(int)_Type].value=true;

		ChoseHeroDefine.RESULT_EQUIP IsCanBuy = ChoseHeroManager.Get ().GetActorIsBuy (_Type);
		switch (IsCanBuy) 
		{
		case ChoseHeroDefine.RESULT_EQUIP.BUY_GET:
				//已经购买到了 但未装备
			_HaveUseUI.SetActive (false);
			_BuyButtonTextLabel.text = "CHOSE";
			_BuyValueUI.SetActive (false);
			_BuyButton.SetActive (true);
			UIEventListener.Get (_BuyButton).onClick = EquipOnclick;
				break;
		case ChoseHeroDefine.RESULT_EQUIP.EQUIP_GET:
				//已经购买 并且已经装备
			_BuyButton.SetActive (false);
			_HaveUseUI.SetActive (true);
			_BuyValueUI.SetActive (false);
				break;
		case ChoseHeroDefine.RESULT_EQUIP.NO_GET:
				//未获得
			_HaveUseUI.SetActive (false);
			_BuyButtonTextLabel.text = "PURCHASE";
			_BuyValueUI.SetActive (true);
			_BuyButton.SetActive (true);
			UIEventListener.Get (_BuyButton).onClick = BuyButtonOnclick;
			_BuyValueLabel.text = ChoseHeroManager.Get ().GetActorBuyDemond (_Type).ToString();
				break;
			default:
				break;
		}
	}

	public void EquipOnclick(GameObject button)
	{
		//Debug.LogError ("装备对应的角色");
		ChoseHeroManager.Get ().SetChoseHero ((ChoseHeroDefine.PLAYR_TYPE)ActorIndex,()=>
			{
				InitUI((ChoseHeroDefine.PLAYR_TYPE)ActorIndex);
			});
	}

	public void BuyButtonOnclick(GameObject button)
	{
		ChoseHeroManager.Get ().BuyHero ((ChoseHeroDefine.PLAYR_TYPE)ActorIndex);
	}
}
