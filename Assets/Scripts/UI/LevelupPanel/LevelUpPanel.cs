using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using FormulaBase;

public enum LevelUpPanelState
{
	LevelUpPanelState_ChosePetFood=1,
	LevelUpPanelState_ChoseEquip=2,
	LevelUpPanelState_ChoseHeroFood=3,
		
}
public class LevelUpPanel : UIPanelBase {
	public List<StrengthenItem> m_listShowLvUpEquipCell=new List<StrengthenItem>();		//显示选择的物品
	public UILabel m_CostMoney;
	FormulaHost m_ingoreHost=null;

	public UISprite m_smallBg;				//小的界面背景
	public UISprite m_bigBg;				//大的界面背景
	public FormulaHost m_host;				//进入界面时的选择
	public static LevelUpPanelState m_LevelUpPanelState=LevelUpPanelState.LevelUpPanelState_ChosePetFood;
	public TweenPosition m_Animation1;
	// Use this for initialization
	public GameObject m_InfoPanel=null;
	public GameObject GetInfoPanel
	{
		get
		{
			if(m_InfoPanel==null)
			{
				m_InfoPanel=GameObject.Find("UI Root/ItemInfoPanel");
			}
//			if(m_InfoPanel==null)
//			{
//				Debug.LogError("the fun is error");
//			}
			return m_InfoPanel;
		}
		
	}
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
                                                       
	}

	void OnEnable()
	{
		Debug.Log("注册----------------------------------》》》》"+"ItemInfoChoseItem");
		//Messenger.MarkAsPermanent(ItemInfoPanel.ItemInfoChoseItem);
		//Messenger.AddListener<bool>(ItemInfoPanel.ItemInfoChoseItem,ChoseItem);

		//Messenger.AddListener<bool>(ItemInfoPanel.BroadUseItemtolevelup,SetSpecialBlur);

		Messenger.MarkAsPermanent(EquipAndPetLevelUpPanel.BroadEquipAnPetAnimationFinish);
		Messenger.AddListener(EquipAndPetLevelUpPanel.BroadEquipAnPetAnimationFinish,LevelUpAnimationFinish);
		//Messenger.MarkAsPermanent(HideLevelUp)	
		
	}
	void OnDisable()
	{
		Debug.Log("删除----------------------------------》》》》"+"ItemInfoChoseItem");
		//Messenger.RemoveListener<bool>(ItemInfoPanel.ItemInfoChoseItem,ChoseItem);
		//Messenger.RemoveListener<bool>(ItemInfoPanel.BroadUseItemtolevelup,SetSpecialBlur);
		Messenger.RemoveListener(EquipAndPetLevelUpPanel.BroadEquipAnPetAnimationFinish,LevelUpAnimationFinish);
	}
	/// <summary>
	/// 进场动画
	/// </summary>
	void PlayInAnimation()
	{
		m_Animation1.onFinished.Clear();
		m_Animation1.PlayForward();
	}
	public void ReSetPanelPos()
	{
		this.gameObject.transform.localPosition=GetComponent<TweenPosition>().from;
	}
//	public void ShowLevelUpPanel(bool _playAni=true,FormulaHost _ignorehost=null,bool _big=true)
//	{
//		m_ingoreHost=_ignorehost;
//		this.gameObject.SetActive(true);
//		SetData(_big);
//
//		if(_playAni)
//		{
//			PlayInAnimation();
//		}
//
//	}
	/// <summary>
	/// 升级动画的退场动画
	/// </summary>
	public void HideLevelUpPanel()
	{
		PlayOutAnimation();
	}
	/// <summary>
	/// 出场动画
	/// </summary>
	void PlayOutAnimation()
	{
		m_Animation1.onFinished.Clear();
		m_Animation1.AddOnFinished(new EventDelegate(FinishOutAnimation));
		m_Animation1.PlayReverse();

	}
	/// <summary>
	/// 刷新数据
	/// </summary>
	public void RefreshData()
	{
		
	}
	/// <summary>
	/// 设置数据
	/// </summary>
	/// <param name="_big">If set to <c>true</c> big.</param>
	public void  SetData(bool _big=true)
	{
//		if(_big)
//		{
//			m_bigBg.gameObject.SetActive(true);
//			m_smallBg.gameObject.SetActive(false);
//		}
//		else 
//		{
//			m_bigBg.gameObject.SetActive(false);
//			m_smallBg.gameObject.SetActive(true);
//		}
		List<FormulaHost> temp=ItemManageComponent.Instance.GetChosedItem;
		int j=0;
		int CostMoney=0;
		int GetExp=0;
	//	if(UIPerfabsManage.g_Instan.GetUIState==UIPerfabsManage.GameUIState.STRENGTHERNITEN_FOODLEVELUP)
	//	{
			for(int i=0,max=temp.Count;i<max;i++)
			{
				for(int m=0,max2=temp[i].GetDynamicIntByKey(SignKeys.CHOSED);m<max2;m++)
				{
					m_listShowLvUpEquipCell[j].SetItem(temp[i],m_ingoreHost);
//					CostMoney+=(int)temp[i].Result(FormulaKeys.FORMULA_41);
//					GetExp+=(int)temp[i].Result(FormulaKeys.FORMULA_40);
					j++;
				}
			}
	//	}
	//	else if(UIPerfabsManage.g_Instan.GetUIState==UIPerfabsManage.GameUIState.STRENGTHERNITEN_EQUIPLEVELUP)
	//	{
		//	for(int i=0,max=temp.Count;i<max;i++)
	//		{

	//			m_listShowLvUpEquipCell[j].SetItem(temp[i],m_ingoreHost);
//				CostMoney+=(int)temp[i].Result(FormulaKeys.FORMULA_37);
//				GetExp+=(int)temp[i].Result(FormulaKeys.FORMULA_36)+temp[i].GetDynamicIntByKey(SignKeys.EXP);
	//			j++;
	//		}
	//	}

		while(j<4)
		{
			m_listShowLvUpEquipCell[j].SetItem(null,m_ingoreHost);
			j++;
		}
		SetCostMoneyAndExp();
		//SetExp();
	}
	/// <summary>
	/// 设置金钱的消耗 和经验的获取
	/// </summary>
	void SetCostMoneyAndExp()
	{
		int tempMoney=0;
		int Exp=0;
		if(m_LevelUpPanelState==LevelUpPanelState.LevelUpPanelState_ChoseHeroFood||m_LevelUpPanelState==LevelUpPanelState.LevelUpPanelState_ChosePetFood)
		{
			materialManageComponent.Instance.GetExpAndCost(ref Exp,ref tempMoney);
		}
		else if(m_LevelUpPanelState==LevelUpPanelState.LevelUpPanelState_ChoseEquip)
		{
			EquipManageComponent.Instance.GetExpAndCost(ref Exp,ref tempMoney);
		}
		Debug.Log("设置消耗的金钱");
		m_CostMoney.text=tempMoney.ToString();
		Debug.Log("获取的经验为："+Exp);
	}
	/// <summary>
	/// 点击确定按钮
	/// </summary>
	public void CLickSureButton()
	{
		Debug.Log("点击确定按钮");
		if(ItemManageComponent.Instance.GetChosedItem.Count==0)
		{
			CommonPanel.GetInstance().ShowText("请选择物品",null,false);
			return ;
		}
		if(ClickSure==null)
		{
			Debug.Log("点击确定失败");
		}
		else 
		{
			if(m_LevelUpPanelState==LevelUpPanelState.LevelUpPanelState_ChoseEquip||m_LevelUpPanelState==LevelUpPanelState.LevelUpPanelState_ChosePetFood)
			{
				SetSpecialBlur(false);
				SetInfoPanelBlur(false);
				CommonPanel.GetInstance().SetMainMenuBlur(true);
				UIManageSystem.g_Instance.AddUI(UIManageSystem.UIEQUIPANDPET_LEVELUPPANEL,1,m_host);
				//CommonPanel.GetInstance().CloseBlur();
				ClickSure();
				Debug.Log("宠物装备确定升级");
			}
			else if(m_LevelUpPanelState==LevelUpPanelState.LevelUpPanelState_ChoseHeroFood)
			{
				UIManageSystem.g_Instance.AddUI(UIManageSystem.UI_HEROLEVEUP_PANEL,1,m_host);
				ClickSure();
			}
		}
	}

	UIPerfabsManage.CallBackFun ClickSure=null;
	/// <summary>
	/// 设置点击确定的回调
	/// </summary>
	/// <param name="_clickSure">Click sure.</param>
	public void SetClickSureFun(UIPerfabsManage.CallBackFun _clickSure)
	{
		ClickSure=_clickSure;
	}
	#region 各种操作
	public override void ShowPanel(int _state=-1,FormulaHost _host=null,int _Layer=-1)
	{
		m_LevelUpPanelState=(LevelUpPanelState)(_state/10);
		if(_Layer==-1)
		{
			SetData();
			switch(m_LevelUpPanelState)
			{
			case LevelUpPanelState.LevelUpPanelState_ChoseEquip:
				//Messenger.Broadcast<FormulaHost>(ItemInfoPanel.ItemRefreshItemInfo,null);

				break;
			case  LevelUpPanelState.LevelUpPanelState_ChoseHeroFood:
				//Messenger.Broadcast(CharactorPanel2.BroadCast_SetHeroData);
				break;
			case  LevelUpPanelState.LevelUpPanelState_ChosePetFood:
				//Messenger.Broadcast<FormulaHost>(ItemInfoPanel.ItemRefreshItemInfo,null);
				break;
			}
//			Messenger.Broadcast<FormulaHost>(ItemInfoPanel.ItemRefreshItemInfo,null);
//			Messenger.Broadcast(CharactorPanel2.BroadCast_SetHeroData);
			return ;
		}
	

		switch(_state%10)
		{
		case 0:
			this.gameObject.layer=5;
			break;
		case 1:
			this.gameObject.layer=17;
			break;
		}
		m_host=_host;
		this.gameObject.SetActive(true);
		SetPanelLayer(_Layer);
		UIManageSystem.g_Instance.SetMainMenuBackButton(true);
		PlayInAnimation();

		switch(m_LevelUpPanelState)
		{
		case LevelUpPanelState.LevelUpPanelState_ChoseEquip:
			m_bigBg.gameObject.SetActive(true);
			m_smallBg.gameObject.SetActive(false);
			break;
		case LevelUpPanelState.LevelUpPanelState_ChoseHeroFood:
			m_bigBg.gameObject.SetActive(false);
			m_smallBg.gameObject.SetActive(true);
			break;
		case LevelUpPanelState.LevelUpPanelState_ChosePetFood:
			m_bigBg.gameObject.SetActive(true);
			m_smallBg.gameObject.SetActive(false);
			break;
		}
	}

//	public override void PanelClickBack()
//	{
//		UIManageSystem.g_Instance.RomoveUI();
//
//	}
	/// <summary>
	/// 每个界面自己的Back事件 Example：背包界面出售按钮显示Back
	/// </summary>
	public override void PanelClickBack()
	{
		PlayOutAnimation();

	}
	/// <summary>
	/// 退场动画的结束
	/// </summary>
	public void FinishOutAnimation()
	{
		Debug.Log("Delete UI"+"LevelUpPanel");
		m_Animation1.onFinished.Clear();
		ItemManageComponent.Instance.ClearChosedItem();
		UIManageSystem.g_Instance.RomoveUI();
		UIManageSystem.g_Instance.ShowUI();
	}

	#endregion

	#region 动画
	public void ChoseItem(bool _hide)
	{
//		m_Animation1.onFinished.Clear();
//		if(m_Animation1==null)
//		{
//			m_Animation1=this.gameObject.GetComponent<TweenPosition>();
//		}
		m_Animation1.Play(!_hide);
	}
	#endregion


	/// <summary>
	/// 设置指定层的  用于毛玻璃
	/// </summary>
	/// <param name="_layer">If set to <c>true</c> layer.</param>
	public void SetSpecialBlur(bool _layer)
	{
		if(_layer)
		{
			this.gameObject.layer=17;
		}
		else 
		{
			this.gameObject.layer=5;
		}
	}
	/// <summary>
	/// 设置info的毛玻璃
	/// </summary>
	/// <param name="_layer">If set to <c>true</c> layer.</param>
	public void SetInfoPanelBlur(bool _layer)
	{
		if(_layer)
		{
			GetInfoPanel.layer=17;
		}
		else 
		{
			GetInfoPanel.layer=5;
		}

	}
	/// <summary>
	/// 升级动画播放完成
	/// </summary>
	public void LevelUpAnimationFinish()
	{
		SetSpecialBlur(true);
		SetInfoPanelBlur(true);
		CommonPanel.GetInstance().SetMainMenuBlur(false);
		StartCoroutine("StartAnimationReverse");

	//	m_Animation1.onFinished.Clear();
	//	m_Animation1.AddOnFinished(new EventDelegate(FinishOutAnimation));
	//	m_Animation1.PlayReverse();
	//	CommonPanel.GetInstance().SetPanelBlur(this.GetComponent<UIPanel>(),true);
	}
	/// <summary>
	/// 开始退场动画
	/// </summary>
	/// <returns>The animation reverse.</returns>
	IEnumerator StartAnimationReverse()
	{
		yield return 0;
		PanelClickBack();
	}
}
