using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using FormulaBase;


//public enum CharactorPanel2State
//{
//	CHARACTOR_ENTERHEROUP,	//进入角色升级
//	CHARACTOR_HEROLVMAX,	//角色等级满级
//	CHARACTOR_HEROLEVELUp,	//英雄升级
//	CHARACTOR_ITMESHOW,     //展示装备信息
//	CHARACTOR_LVUPMATERIAL	//升级材料选择
//
//	
//}
using GameLogic;


public enum CharactorPanel2State
{
	CharactorPanel2State_Normal=1,				//普通状态
	CharactorPanel2State_LevelUp				//升级界面
}
public enum LevelUpPic
{
	LEVELUPPIC_NORMAL,			//普通
	LEVELUPPIC_MID,				//中等
	LEVELUPPIC_CRITICAL			//高级

}
public class CharactorPanel2 : UIPanelBase {

	CharactorPanel2State m_CharactorPanel2State=CharactorPanel2State.CharactorPanel2State_Normal;
	//public static string RefreshCharactorPanel2="RefreshCharactorPanel2";
	string Infofont="[907BB0FF]";			//装备升级字体的两个颜色
	string InfoAddfont="[2BFC7AFF]";		//字体升级的颜色
	#region Data
	public UILabel m_HP;
	public UILabel m_DF;
	public UILabel m_AP;
	public UILabel m_DR;
	public UILabel m_ToHp;
	public UILabel m_ToDF;
	public UILabel m_ToAP;
	public UILabel m_ToDR;


	public UILabel m_Lv;
	public UILabel m_CostMoney;
	public List<GameObject> m_ListStars=new List<GameObject>();					//星星的链表			
	public GameObject LvelcritePanel;
	LevelUpPic m_levelUpPic=LevelUpPic.LEVELUPPIC_NORMAL;		//人物升级的爆率 
	public TweenAlpha m_ShowLevel;
	public TweenPosition m_EquipShow;
	public  static string BroadCast_SetHeroData="BROADCAST_SETHERODATA";
	public UISprite m_Exp;					//角色经验
	public UISprite m_addExp;				//角色增加的经验
	public UILabel m_HeroName;				//角色名字
	public List<GameObject> m_listHero=new List<GameObject>();

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
	#endregion
	#region 人物升级
//	public void SetLevelUpInfo()
//	{
//	}

	/// <summary>
	/// 升级按钮
	/// </summary>
	public void ClickUpGrade()
	{

		UIManageSystem.g_Instance.AddUI(UIManageSystem.UILEVELUPPANEL,30);
	//	GetLevelUpPanel.ShowLevelUpPanel(true,null,false);//ResetToBeginning();	GetLevelUpPanel.ShowLevelUpPanel(true,m_host);//ResetToBeginning();
		m_EquipShow.Play(true);

		m_CharactorPanel2State=CharactorPanel2State.CharactorPanel2State_LevelUp;
	//	GameObject.Find("MainMenuPanel").GetComponent<MainMenuPanel>().SetExitButtonShow(true);
	//	UIPerfabsManage.g_Instan.GetUIState=UIPerfabsManage.GameUIState.CHARACTOR_HEROLEVELUp;
	}
	#endregion

	void OnEnable()
	{
		//GetLevelUpPanel.ReSetPanelPos();
		m_EquipShow.transform.localPosition=m_EquipShow.from;
		Debug.Log("初始化玩家装备界面");

		Messenger.MarkAsPermanent(BroadCast_SetHeroData);
		Messenger.AddListener(BroadCast_SetHeroData,SetRoleData);
		UIPerfabsManage.g_Instan.GetUIState=UIPerfabsManage.GameUIState.Character_Main;



	}
	void OnDisable()
	{//	GetLevelUpPanel.gameObject.SetActive(false);
		Messenger.RemoveListener(BroadCast_SetHeroData,SetRoleData);
		//GetLevelUpPanel.ReSetPanelPos();
	}
	/// <summary>
	/// 点击回退按钮在升级界面
	/// </summary>
	public void ClickBackButtonInLevelUp()
	{
		Debug.Log("接受BACk 按钮的回调");
		//m_bg1.GetComponent<TweenScale>().PlayReverse();
		m_EquipShow.Play(false);
		UIManageSystem.g_Instance.SetMainMenuBackButton(false);
		GetLevelUpPanel.HideLevelUpPanel();
		ItemManageComponent.Instance.ClearChosedItem();
		SetRoleData();
	}
	/// <summary>
	/// 设置面板上的人物属性
	/// </summary>
	/// <param name="_host">Host.</param>
	public void SetRoleData()
	{

		FormulaHost thost=RoleManageComponent.Instance.GetRole(RoleManageComponent.Instance.GetFightGirlIndex());
	
		int hp= (int)thost.Result(FormulaKeys.FORMULA_38);
		int df= (int)thost.Result(FormulaKeys.FORMULA_40);
		int ap= (int)thost.Result(FormulaKeys.FORMULA_39);
		//int dr= (int)thost.Result(FormulaKeys.FORMULA_189);
		m_Exp.fillAmount=thost.GetDynamicIntByKey(SignKeys.EXP)/thost.Result(FormulaKeys.FORMULA_12);
		int star=thost.GetDynamicIntByKey(SignKeys.LEVEL_STAR);
		for(int i=0,max=m_ListStars.Count;i<max;i++)
		{
			if(i<star)
			{
				m_ListStars[i].SetActive(true);
			}
			else 
			{
				m_ListStars[i].SetActive(false);
			}
		}
			if(ItemManageComponent.Instance.GetChosedItem.Count>0)
			{
				Debug.Log("显示升级信息 ");
				FormulaHost Uphost=RoleManageComponent.Instance.GetLevelUpHost(thost);
				int thp= (int)Uphost.Result(FormulaKeys.FORMULA_38);
				int tdf= (int)Uphost.Result(FormulaKeys.FORMULA_40);
				int tap= (int)Uphost.Result(FormulaKeys.FORMULA_39);
				//int tdr= (int)Uphost.Result(FormulaKeys.FORMULA_189);
			//	EquipManageComponent.Instance.GetAllEquipedEquip(ref hp,ref df,ref ap,ref dr,  ref thp,ref df,ref ap,ref dr );
				//ShowAttributeInfo(hp,df,ap,dr,thp,tdf,tap,tdr);
				m_addExp.gameObject.SetActive(true);
				m_addExp.fillAmount=Uphost.GetDynamicIntByKey(SignKeys.EXP)/Uphost.Result(FormulaKeys.FORMULA_12);
				m_Lv.text="LV.:"+thost.GetDynamicIntByKey(SignKeys.LEVEL)+"->"+Uphost.GetDynamicIntByKey(SignKeys.LEVEL);
				GetLevelUpPanel.SetClickSureFun(()=>{

//				_host.SetDynamicData(SignKeys.LEVEL,tLevel);
//				_host.SetDynamicData(SignKeys.EXP,tempHost.GetDynamicIntByKey(SignKeys.EXP));
//				_host.Save(new HttpResponseDelegate(ClickLevelUpCallBack));
//				ItemManageComponent.Instance.UseItemList(ItemManageComponent.Instance.GetChosedItem);
//				CommonPanel.GetInstance().ShowWaittingPanel(true);


				thost.SetDynamicData(SignKeys.LEVEL,Uphost.GetDynamicIntByKey(SignKeys.LEVEL));
				thost.SetDynamicData(SignKeys.EXP,Uphost.GetDynamicIntByKey(SignKeys.EXP));
				thost.Save(new HttpResponseDelegate(ClickLevelUpCallBack));
				ItemManageComponent.Instance.UseItemList(ItemManageComponent.Instance.GetChosedItem);
				CommonPanel.GetInstance().ShowWaittingPanel(true);
				});
			}
			else 
			{
				//EquipManageComponent.Instance.GetAllEquipedEquip(ref hp,ref df,ref ap,ref dr);
				//ShowAttributeInfo(hp,df,ap,dr);
				m_addExp.gameObject.SetActive(false);
				m_Lv.text="LV.:"+thost.GetDynamicIntByKey(SignKeys.LEVEL);
			}
	}
	/// <summary>
	/// 点击升级的回调
	/// </summary>
	/// <param name="_success">If set to <c>true</c> success.</param>
	public void ClickLevelUpCallBack(bool _success)
	{
		if(_success)
		{
			
			ItemManageComponent.Instance.GetChosedItem.Clear();
			//GetLevelUpPanel.SetData();
			SetRoleData();
			GetLevelUpPanel.SetData(false);
			CommonPanel.GetInstance().ShowWaittingPanel(false);
		}
		else 
		{
			CommonPanel.GetInstance().ShowText("conect is fail");
		}
	}
	/// <summary>
	/// 设置玩家的面板属性
	/// </summary>
	/// <param name="_hp">Hp.</param>
	/// <param name="_df">Df.</param>
	/// <param name="_ap">Ap.</param>
	/// <param name="_dr">Dr.</param>
	/// <param name="addhp">Addhp.</param>
	/// <param name="adddf">Adddf.</param>
	/// <param name="addap">Addap.</param>
	/// <param name="adddr">Adddr.</param>
	void ShowAttributeInfo(int _hp,int _df,int _ap,int _dr,int addhp=0,int adddf=0,int addap=0,int adddr=0)
	{
//		public UILabel m_ToHp;
//		public UILabel m_ToDF;
//		public UILabel m_ToAP;
//		public UILabel m_ToDR;



		if(addhp==0)
		{
			if(m_ToHp.gameObject.activeSelf)
			{
				m_ToHp.GetComponent<TweenScale>().PlayReverse();
			}
		}
		else 
		{
			m_ToHp.GetComponent<TweenScale>().PlayForward();
			m_ToHp.text="  +"+InfoAddfont+(addhp-_hp).ToString()+"[-]";
		}

	
		if(adddf==0)
		{
			m_ToDF.GetComponent<TweenScale>().PlayReverse();
		}
		else 
		{	
			m_ToDF.GetComponent<TweenScale>().PlayForward();
			m_ToDF.text="  +"+InfoAddfont+(adddf-_df).ToString()+"[-]";
		}

		if(addap==0)
		{
			m_ToAP.GetComponent<TweenScale>().PlayReverse();
		}
		else 
		{
			m_ToAP.GetComponent<TweenScale>().PlayForward();
			m_ToAP.text="  +"+InfoAddfont+(addap-_ap).ToString()+"[-]";
		}

		if(adddr==0)
		{
			m_ToDR.GetComponent<TweenScale>().PlayReverse();
		}
		else 
		{
			m_ToDR.GetComponent<TweenScale>().PlayForward();
			m_ToDR.text="  +"+InfoAddfont+(adddr-_dr).ToString()+"[-]";
		}
		EquipManageComponent.Instance.GetAllEquipedEquip(ref _hp,ref _df,ref _ap,ref _dr);
		PetManageComponent.Instance.GetAllEquipedPet(ref _hp,ref _df,ref _ap,ref _dr);
		m_HP.text = Infofont+ _hp.ToString ()+"[-]";
		m_DF.text = Infofont+ _df.ToString ()+"[-]";
		m_AP.text = Infofont+ _ap.ToString ()+"[-]";
		m_DR.text = Infofont+ _dr.ToString ()+"[-]";
	}
	void SetRoleData(ushort _index,FormulaHost _tHost=null)
	{
		
//		m_nowIndex = _index;
//		if (tempRole != null) {
//
//			if(UIPerfabsManage.g_Instan.GetUIState==UIPerfabsManage.GameUIState.CHARACTOR_ENTERHEROUP||UIPerfabsManage.g_Instan.GetUIState==UIPerfabsManage.GameUIState.CHARACTOR_HEROLVMAX)
//			{
//
//			}
//			else if(UIPerfabsManage.g_Instan.GetUIState==UIPerfabsManage.GameUIState.CHARACTOR_HEROLEVELUp)
//			{
//				
//				float hp= tempRole.Result(FormulaKeys.FORMULA_0);
//				float df= tempRole.Result(FormulaKeys.FORMULA_1);
//				float ap= tempRole.Result(FormulaKeys.FORMULA_13);
//				float dr= tempRole.Result(FormulaKeys.FORMULA_2);
//				int lv=RoleLevelComponent.Instance.GetLevel(tempRole);
//
//				if(_tHost!=null)
//				{
//					m_tHost=_tHost;
//					float tohp= _tHost.Result(FormulaKeys.FORMULA_0);
//					float todf= _tHost.Result(FormulaKeys.FORMULA_1);
//					float toap= _tHost.Result(FormulaKeys.FORMULA_13);
//					float todr= _tHost.Result(FormulaKeys.FORMULA_2);
//					int tolv=RoleLevelComponent.Instance.GetLevel(_tHost);
//					//tempRole.Result(FormulaKeys.
//					//float dr=tempRole.Result(FormulaKeys.FORMULA_2);
//					//	Debug.Log(tempRole.Result(FormulaKeys.f))
//					if(RoleLevelComponent.Instance.GetLevel(_tHost)==RoleLevelComponent.Instance.GetLevel(tempRole))
//					{
//						Debug.Log("未升级");
//						m_HP.text =	"[907BB0FF]"+ ((int)hp).ToString ();//+"[-]  [2BFC7AFF]+"+_tHost.Result(FormulaKeys.FORMULA_0)-hp+"999[-]";
//						m_DF.text = "[907BB0FF]"+ ((int)df).ToString ();//+"[-]  [2BFC7AFF]+"+"999[-]";
//						m_AP.text = "[907BB0FF]"+ ((int)ap).ToString ();//+"[-]  [2BFC7AFF]+"+"999[-]";
//						m_DR.text = "[907BB0FF]"+ ((int)dr).ToString ();//+"[-]  [2BFC7AFF]+"+"999[-]";
//						m_Lv.text="LV.:"+((int)RoleLevelComponent.Instance.GetLevel (this.tempRole)).ToString();
//
//					}
//					else//升级的
//					{
//						m_HP.text =	"[907BB0FF]"+ ((int)hp).ToString ()+"[-]  [2BFC7AFF]+"+(tohp-hp).ToString()+"[-]";
//						m_DF.text = "[907BB0FF]"+ ((int)df).ToString ()+"[-]  [2BFC7AFF]+"+(todf-df).ToString()+"[-]";
//						m_AP.text = "[907BB0FF]"+ ((int)ap).ToString ()+"[-]  [2BFC7AFF]+"+(toap-ap).ToString()+"[-]";
//						m_DR.text = "[907BB0FF]"+ ((int)dr).ToString ()+"[-]  [2BFC7AFF]+"+(todr-dr).ToString()+"[-]";
//
//						m_Lv.text="LV.:"+((int)RoleLevelComponent.Instance.GetLevel (this.tempRole)).ToString()+"[-]  [2BFC7AFF]>>> "+tolv.ToString()+"[-]";
//
//					}
//				}
//				else
//				{
//					float hp2= tempRole.Result(FormulaKeys.FORMULA_0);
//					float df2= tempRole.Result(FormulaKeys.FORMULA_1);
//					float ap2= tempRole.Result(FormulaKeys.FORMULA_13);
//					float dr2= tempRole.Result(FormulaKeys.FORMULA_2);
//					int lv2=RoleLevelComponent.Instance.GetLevel(tempRole);
//							Debug.Log("未升级");
//							m_HP.text =	"[907BB0FF]"+ ((int)hp2).ToString ();//+"[-]  [2BFC7AFF]+"+_tHost.Result(FormulaKeys.FORMULA_0)-hp+"999[-]";
//							m_DF.text = "[907BB0FF]"+ ((int)df2).ToString ();//+"[-]  [2BFC7AFF]+"+"999[-]";
//							m_AP.text = "[907BB0FF]"+ ((int)ap2).ToString ();//+"[-]  [2BFC7AFF]+"+"999[-]";
//							m_DR.text = "[907BB0FF]"+ ((int)dr2).ToString ();//+"[-]  [2BFC7AFF]+"+"999[-]";
//					m_Lv.text="LV.:"+((int)RoleLevelComponent.Instance.GetLevel (this.tempRole)).ToString();
//				}
//			}
//		}
	}

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {

	}
	/// <summary>
	/// 点击人物替换按钮
	/// </summary>
	public void ClickReplaceButton()
	{

		this.gameObject.SetActive(false);
		UIManageSystem.g_Instance.AddUI(UIManageSystem.UICHOSEHEROPANEL);
		UIManageSystem.g_Instance.SetMainMenuBackButton(true);

		//CommonPanel.GetInstance().ShowText(".....");
	}
	public SkeletonAnimation m_HeroAnimation; 
	/// <summary>
	/// 显示界面
	/// </summary>
	/// <param name="_state">State.</param>
	/// <param name="_host">Host.</param>
	/// <param name="_Layer">Layer.</param>
	public override void ShowPanel(int _state=1,FormulaBase.FormulaHost _host=null,int _Layer=-1)
	{
		this.gameObject.SetActive(true);
		SetRoleData();
		if(_Layer!=-1)
		{
			m_HeroAnimation.GetComponent<Renderer>().sortingOrder=_Layer*5+1;
			SetPanelLayer(_Layer);
		}

		if(m_CharactorPanel2State==CharactorPanel2State.CharactorPanel2State_LevelUp)
		{
			m_EquipShow.Play(false);
			m_CharactorPanel2State=CharactorPanel2State.CharactorPanel2State_Normal;

		}
		ShowSelectHero();
		//Debug.LogError(" use Father fun");
	}

	public Material _LumiMat=null;
	/// <summary>
	/// 设置选择女孩的模型
	/// </summary>
	public void ShowSelectHero()
	{
		int index = RoleManageComponent.Instance.GetFightGirlIndex ();
		m_HeroName.text = ConfigPool.Instance.GetConfigStringValue ("char_info", index.ToString (), "name");
		for (int i = 0; i < m_listHero.Count; i++) {
			if (i == 0) {
				_LumiMat.SetFloat ("_TextureColor", 0f);
			}
			if (index == i) {
				m_listHero [i].SetActive (true);
			} else {
				m_listHero [i].SetActive (false);
			}
		}
	}
}
