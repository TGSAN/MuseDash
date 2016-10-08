using UnityEngine;
using System.Collections;
using FormulaBase;
public enum ItemInfoPanelState
{

	ItemInfoPanel_LevelUp=1,	//普通信息面板
	ItemInfoPanel_Replace=2,	//显示替换的
	ItemInfoPanel_ChoseItem=3,	//选择物品的界面
	ItemInfoPanel_Normal=4,		//普通信息面板
	ITEMINFOPANEL_LEVELUP,		//升级
	ITEMINFOPANEL_ADVANCE,		//建议？
	ITEMINFOPANEL_BAG			//背包


}
public class ItemInfoPanel : UIPanelBase {
	public static  ItemInfoPanelState m_ItemInfoPanelState=ItemInfoPanelState.ItemInfoPanel_LevelUp;

	public static string ItemRefreshItemInfo="ItemRefreshItemInfo";
	public static string ItemInfoChoseItem="ItemInfoChoseItem";
	public static string BroadUseItemtolevelup="BroadUseItemtolevelup";
	public static string BroadUseItemtolevelupBallAni="BroadUseItemtolevelupBallAni";
	public static bool g_levelUp=false;

	public GameObject m_UpgradeButton;			//5个Button
	public GameObject m_LockButton;
	public GameObject m_ReplaceButton;
	public GameObject m_SaleButton;
	public GameObject m_SaleButton2;			//第二个出售素材
	public GameObject m_RemoveButton;

	public GameObject m_Hpob;//4个属性的OB 用于不同的显示
	public GameObject m_Defob;
	public GameObject m_Attob;
	public GameObject m_Crtob;
	public GameObject m_Addob;		//附加属性的Object

	public GameObject m_Star;
	public UISprite m_Exp;			  //物品经验
	public UISprite m_LevelUpExp;	  //物品升级经验
	public UILabel m_ItemName;		  //物品信息	
	public UITable m_InfoTable;
	public UILabel m_Hp;			  //HP 数值
	public UILabel m_Def;			  //防御数值
	public UILabel m_Att;			  //攻击数值
	public UILabel m_Crt;			  //暴击数值
	public UITable m_StarsTable;

	public UILabel m_Level;			  //等级数值
	public UILabel m_Des;			  //描述文本
	public UILabel m_Effect1Des1;	  //效果1标题
	public UILabel m_Effect1;		  //效果描述1
	public UILabel m_Effect1Des2;	  //效果2标题
	public UILabel m_effect2;		  //效果描述2	
	public UILabel m_AddNumberLaber;  //附加属性的值
	public bagItemCell m_bagItemCell; //Icon显示
	public GameObject[]listBlckStars;
	public GameObject[]stars;
	public UISprite m_sprite;
	public UISprite m_Lock;
	public UIPlayTween m_LevelUpEffect;			//升级的效果

	public TweenScale m_HpBall;
	public TweenScale m_DFBall;
	public TweenScale m_ATTBall;
	public TweenScale m_CrtBall;
	public TweenScale m_LvBall;

	public UIPlayTween m_DesPlayTween;

//	CallBackFun SaleCallBack;
	FormulaHost m_host;
	string FontBase="[907BB0FF]";				//显示面板的两种字体颜色
	string FontLevel="[2BFC7AFF]";
	#region 升级相关
	public TweenPosition m_Buttons;			//所有Button
	//public TweenPosition m_LevelUpPanel;	//物品升级的板子
	#endregion
	LevelUpPanel m_LevelUpPanel=null;
	LevelUpPanel GetLevelUpPanel
	{
		get
		{
			if(m_LevelUpPanel==null)
				m_LevelUpPanel=GameObject.Find("UI Root").gameObject.transform.FindChild("LevelUpPanel").GetComponent<LevelUpPanel>();
			return m_LevelUpPanel;
		}
	}
	void OnEnable()
	{
		Messenger.MarkAsPermanent(ItemRefreshItemInfo);
		Messenger.AddListener<FormulaHost>(ItemRefreshItemInfo,SetItemInfoData);
		Messenger.MarkAsPermanent(ItemInfoChoseItem);
		Messenger.AddListener<bool>(ItemInfoChoseItem,HideAnimation);
		Messenger.MarkAsPermanent(BroadUseItemtolevelupBallAni);
		Messenger.AddListener(BroadUseItemtolevelupBallAni,PlayAddBallAnimationMess);
//		Messenger.MarkAsPermanent(EquipAndPetLevelUpPanel.BroadEquipAnPetAnimationFinish2);
//		Messenger.AddListener(EquipAndPetLevelUpPanel.BroadEquipAnPetAnimationFinish2,LevelUpAnimationFinish2);
	}
//	public void LevelUpAnimationFinish2()
//	{
//		//m_LevelUpEffect.Play(true);
//	}
	void OnDisable()
	{
		Messenger.RemoveListener<FormulaHost>(ItemRefreshItemInfo,SetItemInfoData);
		CommonPanel.GetInstance().CloseBlur(this.gameObject);
		Messenger.RemoveListener<bool>(ItemInfoChoseItem,HideAnimation);
		Messenger.RemoveListener(BroadUseItemtolevelupBallAni,PlayAddBallAnimationMess);
		//Messenger.RemoveListener(EquipAndPetLevelUpPanel.BroadEquipAnPetAnimationFinish2,LevelUpAnimationFinish2);
		//ResetButtonsAndPanelPos();
	//	GetLevelUpPanel.gameObject.SetActive(false);
		//GameObject.Find("MainMenuPanel").GetComponent<MainMenuPanel>().SetExitButtonShow(false);
	}
//	public void InitPanel()
//	{
//		//m_ItemInfoPanelState=ItemInfoPanelState.ITEMINFOPANEL_ADVANCE;
//	//	SetButton();
//	}
	public void Update()
	{

	}

//	public void SetData(string _string)
//	{
//		m_sprite.spriteName=_string;
//	}
	/// <summary>
	/// 设置升级按钮或是升星按钮
	/// </summary>
	void SetButton()
	{
		if(m_ItemInfoPanelState==ItemInfoPanelState.ITEMINFOPANEL_ADVANCE)
		{
			m_UpgradeButton.SetActive(false);
		}
		else if(m_ItemInfoPanelState==ItemInfoPanelState.ITEMINFOPANEL_LEVELUP)
		{
			m_UpgradeButton.SetActive(true);
		}
	}
	public delegate void CallBackFun();
	/// <summary>
	/// Sets the item info data.
	/// </summary>
	/// <param name="_host">为Null时 就死沿用上次的数据</param>
	/// <param name="_SaleCallBack">Sale call back.</param>
	public void SetItemInfoData(FormulaHost _host=null)
	{
	//	this.gameObject.SetActive(true);
		if(_host!=null)
		{
			m_host=_host;
		}
	//	SaleCallBack=_SaleCallBack;												//出售的回调
		m_bagItemCell.SetUI(m_host);												//设置ICon  上的东西
		m_ItemName.text=m_host.GetDynamicStrByKey(SignKeys.NAME);				//物品的名字
		int locked =(int)m_host.GetDynamicDataByKey(SignKeys.LOCKED);			///设置 锁的状态
		if(locked==0)
		{
			m_Lock.gameObject.SetActive(false);
		}
		else 
		{
			m_Lock.gameObject.SetActive(true);
		}

		//设置星级
//		Debug.Log("星级"+"now:"+NowStars+"Max:"+MaxStars);


		string temp=m_host.GetFileName();
	//	Debug.Log("Data name:"+temp);
		switch(temp)
		{
		case "Equip":		
			//Debug.Log("EquipInfo");
			ShowEquip(m_host);break;
		case "Material":	
		//	Debug.Log("MaterialInfo");
			ShowMaterial(m_host);break;
		case "Pet":			
			//Debug.Log("PetInfo");
			ShowPet(m_host);break;
		}

	}
//	/// <summary>
//	/// 点击回退在物品升级界面
//	/// </summary>
//	public void ClickBackInItemLevelUp()
//	{
//		
//		
//	}
	#region 显示选定物品信息
	void ShowItemInfo(int _hp=0,int _def=0,int _att=0,int _crt=0,int addhp=0,int adddef=0,int addatt=0,int addcrt=0)
	{
		if(_hp==0)
		{
			m_Hpob.SetActive(false);
		}
		else 
		{
			m_Hpob.SetActive(true);
			if(addhp==0)
				m_Hp.text=FontBase+_hp.ToString()+"[-]";
			else 
			{
				m_Hp.text=FontBase+_hp.ToString()+"[-]"+FontLevel+"-->"+addhp.ToString()+"[-]";
			}
		}

		if(_def==0)
		{
			m_Defob.SetActive(false);
		}
		else 
		{
			m_Defob.SetActive(true);
			if(adddef==0)
				m_Def.text=FontBase+_def.ToString()+"[-]";
			else 
			{
				m_Def.text=FontBase+_def.ToString()+"[-]"+FontLevel+"-->"+adddef.ToString()+"[-]";
			}
		}
		if(_att==0)
		{
			m_Attob.SetActive(false);
		}
		else 
		{
			m_Attob.SetActive(true);
			if(addatt==0)
				m_Att.text=FontBase+_att.ToString()+"[-]";
			else 
			{
				m_Att.text=FontBase+_att.ToString()+"[-]"+FontLevel+"-->"+addatt.ToString()+"[-]";
			}

		}
		if(_crt==0)
		{
			m_Crtob.SetActive(false);
		}
		else 
		{
			m_Crtob.SetActive(true);
			if(addcrt==0)
			{
				m_Crt.text=FontBase+_crt.ToString()+"[-]";
			}
			else 
			{
				m_Crt.text=FontBase+_crt.ToString()+"[-]"+FontLevel+"-->"+addcrt.ToString()+"[-]";
			}
		}
		m_InfoTable.Reposition();

	}
	void ShowEquip(FormulaHost _host)
	{
		int nowLevel=(int)_host.GetDynamicDataByKey(SignKeys.LEVEL);			//等级
		Debug.Log("差个经验值");
		int nowHp=(int)_host.Result(FormulaKeys.FORMULA_26);					//Hp
		int nowDef=(int)_host.Result(FormulaKeys.FORMULA_32);					//Def
		int nowAtt=(int)_host.Result(FormulaKeys.FORMULA_29);					//Att
		int nowCrt=(int)_host.Result(FormulaKeys.FORMULA_35);					//Crt

		int maxLevel=(int)_host.Result(FormulaKeys.FORMULA_23);
		m_Addob.SetActive(true);
		int type=(int)_host.Result(FormulaKeys.FORMULA_147);
		float BaseVaule=(float)_host.Result(FormulaKeys.FORMULA_148);
		string AddType="";
		switch(type)
		{
		case 1:AddType="攻击";break;
		case 2:AddType="生命";break;
		case 3:AddType="防御";break;
		case 4:AddType="暴击";break;
		default:AddType="error";break;
		}


		int MaxStars=(int)_host.Result(FormulaKeys.FORMULA_20);
		int NowStars=(int)_host.Result(FormulaKeys.FORMULA_24);

		for(int i=0,max=listBlckStars.Length;i<max;i++)
		{
			if(i<MaxStars)
			{
				listBlckStars[i].SetActive(true);
			}
			else
			{
				listBlckStars[i].SetActive(false);
			}
		}
		for(int i=0,max=stars.Length;i<max;i++)
		{
			if(i<NowStars)
			{
				stars[i].SetActive(true);
			}
			else
			{
				stars[i].SetActive(false);
			}
		}
		m_StarsTable.Reposition();
//		public UILabel m_Des;			  //描述文本
//		public UILabel m_Effect1Des1;	  //效果1标题
//		public UILabel m_Effect1;		  //效果描述1
//		public UILabel m_Effect1Des2;	  //效果2标题
//		public UILabel m_effect2;		  //效果描述2	
		m_Effect1Des1.text="装备效果";
		m_Effect1Des2.text="套装效果";
		m_Effect1Des2.gameObject.SetActive(true);
		m_Des.text=	(string)_host.GetDynamicStrByKey(SignKeys.DESCRIPTION);

		//#region 面板属性显示

		if(ItemManageComponent.Instance.GetChosedItem.Count!=0)
		{

			Debug.Log("显示而外的属性");
			FormulaHost tempHost=EquipManageComponent.Instance.GetLevelUpHost(_host);
			int tHp=(int)tempHost.Result(FormulaKeys.FORMULA_26);					//Hp
			int tDef=(int)tempHost.Result(FormulaKeys.FORMULA_32);					//Def
			int tAtt=(int)tempHost.Result(FormulaKeys.FORMULA_29);					//Att
			int tCrt=(int)tempHost.Result(FormulaKeys.FORMULA_35);					//Crt
			int tLevel=tempHost.GetDynamicIntByKey(SignKeys.LEVEL);
			m_LevelUpExp.gameObject.SetActive(true);
			m_Level.text="LV."+nowLevel.ToString()+"-->"+tLevel.ToString();

			ShowItemInfo(nowHp,nowDef,nowAtt,nowCrt,tHp,tDef,tAtt,tCrt);
			m_AddNumberLaber.text=string.Format("角色{0}额外提升{1}%",AddType,BaseVaule*100);
			m_Effect1.text=(string)_host.GetDynamicStrByKey(SignKeys.ITEMEFFECT1);
			m_effect2.text=(string)_host.GetDynamicStrByKey(SignKeys.ITEMEFFECT2)+"\n"+(string)_host.GetDynamicStrByKey(SignKeys.ITEMEFFECT3);
			m_Exp.fillAmount=_host.GetDynamicIntByKey(SignKeys.EXP)/_host.Result(FormulaKeys.FORMULA_155);
			m_LevelUpExp.fillAmount=tempHost.GetDynamicIntByKey(SignKeys.EXP)/tempHost.Result(FormulaKeys.FORMULA_155);

			if(_host.GetDynamicIntByKey(SignKeys.LEVEL)<tLevel)
			{
				g_levelUp=true;
			}
			else 
			{
				g_levelUp=false;
			}
			GetLevelUpPanel.SetClickSureFun(()=>{
				_host.SetDynamicData(SignKeys.LEVEL,tLevel);
				_host.SetDynamicData(SignKeys.EXP,tempHost.GetDynamicIntByKey(SignKeys.EXP));
				_host.Save(new HttpResponseDelegate(ClickLevelUpCallBack));
				ItemManageComponent.Instance.UseItemList(ItemManageComponent.Instance.GetChosedItem);
				CommonPanel.GetInstance().ShowWaittingPanel(true);
			}
			);
		}
		else 
		{
			m_LevelUpExp.gameObject.SetActive(false);
			m_Level.text="LV."+nowLevel.ToString();
			ShowItemInfo(nowHp,nowDef,nowAtt,nowCrt);
			m_AddNumberLaber.text=string.Format("角色{0}额外提升{1}%",AddType,BaseVaule*100);
			m_Effect1.text=(string)_host.GetDynamicStrByKey(SignKeys.ITEMEFFECT1);
			m_effect2.text=(string)_host.GetDynamicStrByKey(SignKeys.ITEMEFFECT2)+"\n"+(string)_host.GetDynamicStrByKey(SignKeys.ITEMEFFECT3);
			m_Exp.fillAmount=_host.GetDynamicIntByKey(SignKeys.EXP)/_host.Result(FormulaKeys.FORMULA_155);
		}
//		m_Level.text="LV."+nowLevel.ToString();
//		ShowItemInfo(nowHp,nowDef,nowAtt,nowCrt);
//		m_AddNumberLaber.text=string.Format("角色{0}额外提升{1}%",AddType,BaseVaule*100);
//		m_Effect1.text=(string)_host.GetDynamicStrByKey(SignKeys.ITEMEFFECT1);
//		m_effect2.text=(string)_host.GetDynamicStrByKey(SignKeys.ITEMEFFECT2)+"\n"+(string)_host.GetDynamicStrByKey(SignKeys.ITEMEFFECT3);
//		m_Exp.fillAmount=_host.GetDynamicIntByKey(SignKeys.EXP)/_host.Result(FormulaKeys.FORMULA_155);
		//#endregion


		//m_Effect1.text=_host.GetDynamicStrByKey(SignKeys)

	//	Debug.Log
		if(maxLevel==nowLevel)					//判断是否可升级
		{
			FullLevelEquip();
		}
		else 
		{
			NoFullLevelEquip();
		}

//		if(bagPanel2.GetBagPanelState()==BagPanel2State.BagPanel2_ShowAll)
//		{
//			m_RemoveButton.SetActive(false);
//			m_ReplaceButton.SetActive(false);
//			m_UpgradeButton.SetActive(true);
//			m_LockButton.SetActive(true);
//			m_SaleButton.SetActive(true);
//		}
//		else if(UIPerfabsManage.g_Instan.GetUIState==UIPerfabsManage.GameUIState.BAG_CHARACTERTTOBAG
//			||UIPerfabsManage.g_Instan.GetUIState==UIPerfabsManage.GameUIState.CHARACTOR_ITMESHOW
//		)
//		{
//			m_RemoveButton.SetActive(true);
//			m_ReplaceButton.SetActive(true);
//			m_UpgradeButton.SetActive(true);
//			m_LockButton.SetActive(false);
//			m_SaleButton.SetActive(false);
//		}
	}
	/// <summary>
	/// 判断装备是否满级
	/// </summary>
	void FullLevelEquip()
	{
		m_UpgradeButton.SetActive(false);
	}
	void NoFullLevelEquip()
	{
		m_UpgradeButton.SetActive(true);
	}
	/// <summary>
	/// 设置素材
	/// </summary>
	/// <param name="_host">Host.</param>
	void ShowMaterial(FormulaHost _host)
	{
			m_Level.text="LV.MAX";			//等级
			m_Addob.SetActive(false);
			m_Hpob.SetActive(false);
			m_Defob.SetActive(false);
			m_Attob.SetActive(false);
			m_Crtob.SetActive(false);


			int MaxStars=(int)m_host.Result(FormulaKeys.FORMULA_132);
			int NowStars=(int)m_host.Result(FormulaKeys.FORMULA_131);

			for(int i=0,max=listBlckStars.Length;i<max;i++)
			{
				if(i<MaxStars)
				{
					listBlckStars[i].SetActive(true);
				}
				else
				{
					listBlckStars[i].SetActive(false);
				}
			}
			for(int i=0,max=stars.Length;i<max;i++)
			{
				if(i<NowStars)
				{
					stars[i].SetActive(true);
				}
				else
				{
					stars[i].SetActive(false);
				}
			}
			m_StarsTable.Reposition();

			m_Effect1Des1.text="道具效果";
			m_Effect1Des2.gameObject.SetActive(false);
			m_Des.text=	(string)_host.GetDynamicStrByKey(SignKeys.DESCRIPTION);
			m_Effect1.text=(string)_host.GetDynamicStrByKey(SignKeys.ITEMEFFECT1);
			m_effect2.text="";
			m_Exp.fillAmount=1f;
	}
	/// <summary>
	/// 设置宠物
	/// </summary>
	/// <param name="_host">Host.</param>
	void ShowPet(FormulaHost _host)
	{
		int type=(int)_host.GetDynamicDataByKey(SignKeys.SMALLlTYPE);
		m_Addob.SetActive(false);
		if(type==5)//宠物 
		{
			int nowLevel=(int)_host.GetDynamicDataByKey(SignKeys.LEVEL);			//等级

			int maxLevel=(int)_host.Result(FormulaKeys.FORMULA_157);
			int nowHp=(int)_host.Result(FormulaKeys.FORMULA_137);					//Hp
			int nowDef=(int)_host.Result(FormulaKeys.FORMULA_141);					//Def
			int nowAtt=(int)_host.Result(FormulaKeys.FORMULA_139);					//Att
			int nowCrt=(int)_host.Result(FormulaKeys.FORMULA_143);					//Crt
			m_Effect1Des1.text="激活技能";
			m_Effect1Des2.gameObject.SetActive(true);
			int MaxStars=(int)m_host.Result(FormulaKeys.FORMULA_136);
			int NowStars=(int)m_host.Result(FormulaKeys.FORMULA_135);
			#region 宠物信息

			if(ItemManageComponent.Instance.GetChosedItem.Count!=0)
			{
				Debug.Log("显示而外的属性");
				m_Des.text=	(string)_host.GetDynamicStrByKey(SignKeys.DESCRIPTION);
				m_Effect1.text=(string)_host.GetDynamicStrByKey(SignKeys.ITEMEFFECT1);
				m_effect2.text=(string)_host.GetDynamicStrByKey(SignKeys.ITEMEFFECT2);
				m_Effect1Des1.text="被动技能";
				m_Level.text="LV."+nowLevel.ToString();
				ShowItemInfo(nowHp,nowDef,nowAtt,nowCrt);
				m_Exp.fillAmount=_host.GetDynamicIntByKey(SignKeys.EXP)/_host.Result(FormulaKeys.FORMULA_141);


				FormulaHost tempHost=PetManageComponent.Instance.GetLevelUpHost(_host);
				int tHp=(int)tempHost.Result(FormulaKeys.FORMULA_137);					//Hp
				int tDef=(int)tempHost.Result(FormulaKeys.FORMULA_141);					//Def
				int tAtt=(int)tempHost.Result(FormulaKeys.FORMULA_139);					//Att
				int tCrt=(int)tempHost.Result(FormulaKeys.FORMULA_143);					//Crt
				int tLevel=tempHost.GetDynamicIntByKey(SignKeys.LEVEL);
				m_LevelUpExp.gameObject.SetActive(true);
				m_Level.text="LV."+nowLevel.ToString()+"-->"+tLevel.ToString();
//
				ShowItemInfo(nowHp,nowDef,nowAtt,nowCrt,tHp,tDef,tAtt,tCrt);

				m_Exp.fillAmount=_host.GetDynamicIntByKey(SignKeys.EXP)/_host.Result(FormulaKeys.FORMULA_141);
				m_LevelUpExp.fillAmount=tempHost.GetDynamicIntByKey(SignKeys.EXP)/tempHost.Result(FormulaKeys.FORMULA_141);
				//			if(_host.GetDynamicIntByKey(SignKeys.LEVEL)<tLevel)
				if(_host.GetDynamicIntByKey(SignKeys.LEVEL)<tLevel)
				{
					g_levelUp=true;
				}
				else 
				{
					g_levelUp=false;
				}
				GetLevelUpPanel.SetClickSureFun(()=>{
//
					_host.SetDynamicData(SignKeys.LEVEL,tLevel);
					_host.SetDynamicData(SignKeys.EXP,tempHost.GetDynamicIntByKey(SignKeys.EXP));
					_host.Save(new HttpResponseDelegate(ClickLevelUpCallBack));
					ItemManageComponent.Instance.UseItemList(ItemManageComponent.Instance.GetChosedItem);
					CommonPanel.GetInstance().ShowWaittingPanel(true);


				}
				);
				
			}
			else 
			{	

				m_LevelUpExp.gameObject.SetActive(false);
				m_Des.text=	(string)_host.GetDynamicStrByKey(SignKeys.DESCRIPTION);
				m_Effect1.text=(string)_host.GetDynamicStrByKey(SignKeys.ITEMEFFECT1);
				m_effect2.text=(string)_host.GetDynamicStrByKey(SignKeys.ITEMEFFECT2);
				m_Effect1Des1.text="被动技能";
				m_Level.text="LV."+nowLevel.ToString();
				ShowItemInfo(nowHp,nowDef,nowAtt,nowCrt);
				m_Exp.fillAmount=_host.GetDynamicIntByKey(SignKeys.EXP)/_host.Result(FormulaKeys.FORMULA_141);
			}


			#endregion
			for(int i=0,max=listBlckStars.Length;i<max;i++)
			{
				if(i<MaxStars)
					{
						listBlckStars[i].SetActive(true);
					}
					else
					{
						listBlckStars[i].SetActive(false);
					}
				}
				for(int i=0,max=stars.Length;i<max;i++)
				{
					if(i<NowStars)
					{
						stars[i].SetActive(true);
					}
					else
					{
						stars[i].SetActive(false);
					}
				}


				m_StarsTable.Reposition();
				m_Hpob.SetActive(true);
				m_Defob.SetActive(true);
				m_Attob.SetActive(true);
				m_Crtob.SetActive(true);
				m_InfoTable.Reposition();
			Debug.Log(">>>>>>>>>>>>>>>>>>>>>>>"+nowLevel+"/"+maxLevel);
			if(maxLevel==nowLevel)					//判断是否可升级
			{
				FullLevelEquip();
			}
			else 
			{
				NoFullLevelEquip();
			}
		}
			else if(type==6)//碎片
			{

				m_Exp.fillAmount=1f;
				m_Level.text="MAX";			//等级

				m_Hpob.SetActive(false);
				m_Defob.SetActive(false);
				m_Attob.SetActive(false);
				m_Crtob.SetActive(false);

				int MaxStars=(int)m_host.Result(FormulaKeys.FORMULA_136);
				int NowStars=(int)m_host.Result(FormulaKeys.FORMULA_135);

				for(int i=0,max=listBlckStars.Length;i<max;i++)
				{
					if(i<MaxStars)
					{
						listBlckStars[i].SetActive(true);
					}
					else
					{
						listBlckStars[i].SetActive(false);
					}
				}
				for(int i=0,max=stars.Length;i<max;i++)
				{
					if(i<NowStars)
					{
						stars[i].SetActive(true);
					}
					else
					{
						stars[i].SetActive(false);
					}
				}
				m_StarsTable.Reposition();

				m_Des.text=	(string)_host.GetDynamicStrByKey(SignKeys.DESCRIPTION);
				m_Effect1.text=(string)_host.GetDynamicStrByKey(SignKeys.ITEMEFFECT1);
				m_effect2.text=(string)_host.GetDynamicStrByKey(SignKeys.ITEMEFFECT2);
			}
	}
	/// <summary>
	/// 点击升级按钮的回调
	/// </summary>
	void ClickLevelUpCallBack(bool _success)
	{
		if(_success)
		{
			ItemManageComponent.Instance.GetChosedItem.Clear();
			SetItemInfoData();
			GetLevelUpPanel.SetData();
			CommonPanel.GetInstance().ShowWaittingPanel(false);
		}
		else 
		{
			CommonPanel.GetInstance().ShowText("Conect is fail");
		}
		
	}
	void FullLevelPet()
	{

	}
	void NoFullLevelPet()
	{

	}
//	void ShowChest(FormulaHost _host)
//	{
//	}
	#endregion
	// Use this for initialization
	void Start () {
	
	}

//	public void ClickAdvanceButton()
//	{
//	//	m_ItemInfoPanelState=ItemInfoPanelState.ITEMINFOPANEL_LEVELUP;
//		//SetButton();
//	}
	/// <summary>
	/// 点击升级按钮
	/// </summary>
	public void  CLickGradeUpButton()
	{
		Debug.Log("点击GradeUp Button");

//		m_Buttons.ResetToBeginning();
//		m_Buttons.PlayForward();			//所有Button
		m_RightInfoTween.PlayReverse();
		//GetLevelUpPanel.ShowLevelUpPanel(true,m_host);//ResetToBeginning();
		string temp=m_host.GetFileName();

		if(temp=="Equip")
		{
			UIManageSystem.g_Instance.AddUI(UIManageSystem.UILEVELUPPANEL,21,m_host);
		}
		else 
		{
			UIManageSystem.g_Instance.AddUI(UIManageSystem.UILEVELUPPANEL,11,m_host);
		}
		m_ItemInfoPanelState=ItemInfoPanelState.ItemInfoPanel_ChoseItem;
	}
	/// <summary>
	/// 重设按钮和升级Panel 的位置
	/// </summary>
	public void ResetButtonsAndPanelPos()
	{
		m_RightInfoTween.transform.localPosition=m_RightInfoTween.from;
	//	GetLevelUpPanel.ShowLevelUpPanel();//ResetToBeginning();
	}
	/// <summary>
	/// 在物品信息升级的界面点Back
	/// </summary>
	public void ClickBackButtonInInfoLevel()
	{
		Debug.Log("清空选择");
		//清空选择链表  BagCell d额Tag  个数 选择数
		ItemManageComponent.Instance.ClearChosedItem();
		Messenger.Broadcast(bagItemCell.BraodCast_HideChoseTag);
		Messenger.Broadcast(bagItemCell.BraodCast_BagSaleItemPluralNumber,true);


		//m_Buttons.PlayReverse();			//所有Button
		m_RightInfoTween.PlayReverse();
		SetItemInfoData();
		GetLevelUpPanel.HideLevelUpPanel();
	}
	/// <summary>
	/// 点击出售按钮
	/// </summary>
	public void ClickSaleButton()
	{
		Debug.Log("点击出售按钮");
		if(m_host.GetDynamicIntByKey(SignKeys.LOCKED)!=0)
		{
			CommonPanel.GetInstance().ShowText("Sale Something is Locked(5001)",null,false);
			return;
		}
		CommonPanel.GetInstance().SetPanelBlur(this.gameObject.GetComponent<UIPanel>(),false);
		CommonPanel.GetInstance().SetMainMenuBlur(true);
		CommonPanel.GetInstance().ShowEnSureSalePanel(ItemManageComponent.Instance.GetItemMoney(m_host),EnSureOKCallBack,EnsureCancelCallBack,m_host.GetDynamicDataByKey(SignKeys.QUALITY)>1,true);
//		m_RightInfoTween.PlayReverse();
//		m_LeftInfoTween.PlayReverse();
	}
	/// <summary>
	/// 出售确定的回调
	/// </summary>
	public void EnSureOKCallBack()
	{
		CommonPanel.GetInstance().SetPanelBlur(this.gameObject.GetComponent<UIPanel>(),true);
		CommonPanel.GetInstance().SetMainMenuBlur(false);
		ItemManageComponent.Instance.SaleItem(m_host);
		CommonPanel.GetInstance().HidEnSureSalePanel();
		UIManageSystem.g_Instance.RomoveUI();

		//PanelClickBack();
				
	}
	/// <summary>
	/// 出售取消的回调
	/// </summary>
	public void EnsureCancelCallBack()
	{
		CommonPanel.GetInstance().SetPanelBlur(this.gameObject.GetComponent<UIPanel>(),true);
		CommonPanel.GetInstance().SetMainMenuBlur(false);
		CommonPanel.GetInstance().HidEnSureSalePanel(true);
		
	}
	/// <summary>
	/// 点击锁定按钮
	/// </summary>
	public void ClickLockButton()
	{
		m_Lock.gameObject.SetActive(!m_Lock.gameObject.activeSelf);
		ItemManageComponent.Instance.LockItem(m_host,!m_Lock.gameObject.activeSelf);
	}
	/// <summary>
	/// 点击替换按钮
	/// </summary>
	public void ClickReplace()
	{
		CommonPanel.GetInstance().CloseBlur(null);
		//string filmname=m_host.GetFileName();
		if(bagPanel2.GetBagPanelEquipPlace()==BagPanelEquipPlace.BagPanelEquipPlace_Equip1)
		{
			UIManageSystem.g_Instance.AddUI(UIManageSystem.UIBAGPANEL,(int)BagPanel2State.BagPanel2_ShowWeapon*10+(int)BagPanelState2.BagPanelState2_Replace);
		}
		else if(
			bagPanel2.GetBagPanelEquipPlace()==BagPanelEquipPlace.BagPanelEquipPlace_Equip2||
			bagPanel2.GetBagPanelEquipPlace()==BagPanelEquipPlace.BagPanelEquipPlace_Equip3||
			bagPanel2.GetBagPanelEquipPlace()==BagPanelEquipPlace.BagPanelEquipPlace_Equip4
		)
		{
			UIManageSystem.g_Instance.AddUI(UIManageSystem.UIBAGPANEL,(int)BagPanel2State.BagPanel2_ShowChip*10+(int)BagPanelState2.BagPanelState2_Replace);
		}
		else if(
			bagPanel2.GetBagPanelEquipPlace()==BagPanelEquipPlace.BagPanelEquipPlace_Pet1||
			bagPanel2.GetBagPanelEquipPlace()==BagPanelEquipPlace.BagPanelEquipPlace_Pet2||
			bagPanel2.GetBagPanelEquipPlace()==BagPanelEquipPlace.BagPanelEquipPlace_Pet3
		)
		{
			UIManageSystem.g_Instance.AddUI(UIManageSystem.UIBAGPANEL,(int)BagPanel2State.BagPanel2_SHowPet*10+(int)BagPanelState2.BagPanelState2_Replace);
		}

		Debug.Log("点击替换按钮");
	}
	/// <summary>
	/// 点击卸载按钮
	/// </summary>
	public void ClickRemove()
	{

		string temp=m_host.GetFileName();
		if(temp=="Equip")
		{
			m_host.SetDynamicData(SignKeys.EQUIPEDQUEUE,0);
			EquipManageComponent.Instance.GetEquipedEquipList.Remove(m_host);
			ItemManageComponent.Instance.GetEquipList.Add(m_host);
		}
		else if(temp=="Pet")
		{
			m_host.SetDynamicData(SignKeys.EQUIPEDQUEUE,0);
			PetManageComponent.Instance.GetListEquipedPetHosts.Remove(m_host);
			ItemManageComponent.Instance.GetPetList.Add(m_host);
		}
		m_host.Save(new HttpResponseDelegate(ClickRemoveCallBack));
		Debug.Log("点击移除按钮");
	}
	/// <summary>
	/// 卸载的回调
	/// </summary>
	/// <param name="_Success">If set to <c>true</c> success.</param>
	public void ClickRemoveCallBack(bool _Success)
	{
		if(_Success)
		{
			Debug.Log("调用Remove 的回调");
			Messenger.Broadcast(PetCell.BroadCastRefreshEquipedPetUI);
			Messenger.Broadcast(EquipCell.BroadCastRefreshEquipedEquipUI);
			Messenger.Broadcast(CharactorPanel2.BroadCast_SetHeroData);
			UIManageSystem.g_Instance.RomoveUI();
		//	ClosePanel();
		}
		else 
		{
			CommonPanel.GetInstance().ShowText("connect is fail");
		}
	}
	/// <summary>
	/// 关闭界面
	/// </summary>
	public void ClosePanel()
	{
		this.gameObject.SetActive(false);
	}
	public override void ShowPanel(int _state=1,FormulaHost _host=null,int _Layer=-1)
	{
		Debug.Log("显示InfoPanel");
		if(_host!=null)
		{
			m_host=_host;
		}
		this.gameObject.SetActive(true);
		if(_Layer!=-1)
		{
			SetPanelLayer(_Layer);
			CommonPanel.GetInstance().SetBlurSub(this.GetComponent<UIPanel>(),true);
			m_LeftInfoTween.Play(true);
			m_DesPlayTween.Play(true);
			m_RightInfoTween.Play(true);
		}
		UIManageSystem.g_Instance.SetMainMenuBackButton(true);
		SetItemInfoData(m_host);

		if(m_ItemInfoPanelState==ItemInfoPanelState.ItemInfoPanel_ChoseItem)
		{
			m_ItemInfoPanelState=(ItemInfoPanelState)_state;
			m_RightInfoTween.Play(true);
		}
		Debug.Log((ItemInfoPanelState)_state);
		switch((ItemInfoPanelState)_state)
		{
		case ItemInfoPanelState.ItemInfoPanel_LevelUp:
			m_RemoveButton.SetActive(false);
			m_ReplaceButton.SetActive(false);

			m_LockButton.SetActive(true);
			m_SaleButton.SetActive(true);
			m_SaleButton2.SetActive(false);
			break;
		case ItemInfoPanelState.ItemInfoPanel_Replace:
			m_RemoveButton.SetActive(true);
			m_ReplaceButton.SetActive(true);
			m_LockButton.SetActive(false);
			m_SaleButton.SetActive(false);
			m_SaleButton2.SetActive(false);
			break;
		}
		string temp=m_host.GetFileName();
		int ttype=(int)m_host.GetDynamicDataByKey(SignKeys.SMALLlTYPE);
		switch(temp)
		{
		case "Equip":
			m_UpgradeButton.SetActive(true);
			break;
		case "Pet":
			if(ttype==6)
			{
				m_UpgradeButton.SetActive(false);
				m_SaleButton.SetActive(false);
				m_SaleButton2.SetActive(true);
			}
			else 
			{
				//m_UpgradeButton.SetActive(true);
			}
			break;
		case "Material":
			m_UpgradeButton.SetActive(false);
			m_SaleButton.SetActive(false);
			m_SaleButton2.SetActive(true);
			break;
		}
	}
	/// <summary>
	/// 每个界面自己的Back事件 Example：背包界面出售按钮显示Back
	/// </summary>
	public override void PanelClickBack()
	{
		m_LeftInfoTween.Play(false);
		m_RightInfoTween.Play(false);
		m_DesPlayTween.Play(false);
		m_LeftInfoTween.AddOnFinished(new EventDelegate(ExitAnimationFinish));
	}
	/// <summary>
	/// 退场动画播放完
	/// </summary>
	public void ExitAnimationFinish()
	{
		UIManageSystem.g_Instance.RomoveUI();
		UIManageSystem.g_Instance.ShowUI();
		m_LeftInfoTween.RemoveOnFinished(new EventDelegate(ExitAnimationFinish));
		Debug.Log("FinishCount"+m_LeftInfoTween.onFinished.Count);
	}

	#region 动画
	public TweenPosition m_LeftInfoTween;
	public TweenPosition m_RightInfoTween;
	/// <summary>
	/// 物品信息隐藏的动画 --- 点击选择升级材料
	/// </summary>
	public void HideAnimation(bool _hide)
	{

		m_LeftInfoTween.onFinished.Clear();

		m_LeftInfoTween.Play(!_hide);
		m_DesPlayTween.Play(!_hide);
		if(_hide)
		{
			m_LeftInfoTween.AddOnFinished(new EventDelegate(HideBlur));
		}
		else 
		{
			m_LeftInfoTween.AddOnFinished(new EventDelegate(ShowBlur));
		}
	}
	/// <summary>
	/// 隐藏毛玻璃
	/// </summary>
	void HideBlur()
	{
		Debug.Log("HideBlur>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>");
		CommonPanel.GetInstance().CloseBlur(null);

	}
	/// <summary>
	/// 显示毛玻璃
	/// </summary>
	void ShowBlur()
	{
		Debug.Log("ShowBlur>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>");
		CommonPanel.GetInstance().SetBlurSub(null);
	}
	/// <summary>
	/// 点击物品的描述位置 
	/// </summary>
	public void ClickDes()
	{
		PanelClickBack();
	}

//	public void  LevelUpAnimationFinish()
//	{
//		CommonPanel.GetInstance().SetPanelBlur(this.GetComponent<UIPanel>(),true);
//	}

//	public TweenPosition m_
	/// <summary>
	/// 显示升级动画光球
	/// </summary>
	public void PlayAddBallAnimation()
	{
		m_HpBall.gameObject.SetActive(true);
		m_DFBall.gameObject.SetActive(true);
		m_ATTBall.gameObject.SetActive(true);
		m_CrtBall.gameObject.SetActive(true);
		m_LvBall.gameObject.SetActive(true);
		m_HpBall.ResetToBeginning();
		m_HpBall.PlayForward();
		m_DFBall.ResetToBeginning();
		m_DFBall.PlayForward();
		m_ATTBall.ResetToBeginning();
		m_ATTBall.PlayForward();
		m_CrtBall.ResetToBeginning();
		m_CrtBall.PlayForward();
		m_LvBall.ResetToBeginning();
		m_LvBall.PlayForward();
	}
	public void PlayAddBallAnimationMess()
	{
		Debug.Log("播放完成球动画");
		if(g_levelUp)
		{
			StartCoroutine("PlayLevelUpFinishAnimation");
			//();
			g_levelUp=false;
		}

	}
	IEnumerator PlayLevelUpFinishAnimation()
	{
		yield return new WaitForSeconds(0.2f);			//0.2s 后隐藏掉球

		PlayAddBallAnimation();
	}
	public void AnimationFinish()
	{
		m_HpBall.gameObject.SetActive(false);
		m_DFBall.gameObject.SetActive(false);
		m_ATTBall.gameObject.SetActive(false);
		m_CrtBall.gameObject.SetActive(false);
		m_LvBall.gameObject.SetActive(false);
	}
	#endregion
}
