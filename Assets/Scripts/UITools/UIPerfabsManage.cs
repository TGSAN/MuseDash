//EDIT :SF 留打包用
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using FormulaBase;

/// <summary>
/// 奖池物品数据
/// </summary>

public class UIPerfabsManage :System.Object {


	public delegate void CallBackFun();

	public FormulaHost NowChoseChesGrid;

	public enum GameUIState		//游戏当前状态的管理类
	{
		NONE,						//什么都不设置的情况
		AdvenTure_Main,				//冒险关卡主界面
		Character_Main,				//人物主界面
		SuitCase_Main,				//背包主界面
		SuitCase_Sale,				//背包出售界面
		SuitCase_ItemInfo,			//背包物品显示界面
		Store_Main,					//什么的主界面
		Other_Main,					//其他的主界面

		//CHARACTOR_ENTERHEROUP,		//进入角色升级
		CHARACTOR_HEROLVMAX,		//角色等级满级
		CHARACTOR_HEROLEVELUp,		//英雄升级
		CHARACTOR_HEROLEVELUPTOBAG,	//英雄升级到背包
		//CHARACTOR_ITMEINFO,     	//展示人物界面的装备信息
		CHARACTOR_ITMESHOW,     	//展示装备信息
		CHARACTOR_ITEMLEVELUPEQUIP,		//展示装备升级界面
		CHARACTOR_ITEMLEVELUPFOOD,		//展示装备升级界面
		CHARACTOR_LVUPMATERIAL,		//升级材料选择

		BAG_CHESTFIELDTOBAG,		    		 //宝箱栏位到背包
		BAG_CHESTFIELDREPLACETOBAG,	     		 //宝箱替换到背包
		BAG_CHARACTERTTOBAG,		     		 //人物属性界面到背包
		BAG_CHARACTER_WEAPON_TOBAG,	  	 		 //人物武器到背包
		BAG_CHARACTER_WEAPONREPLACE_TOBAG, 		 //人物武器替换到背包
		BAG_CHARACTER_SUIT_TOBAG,		 	 	 //人物衣服到背包
		BAG_CHARACTER_SUITREPLACE_TOBAG,	 	 //人物衣服替换到背包
		BAG_CHARACTER_CHIP0_TOBAG,		     	 //人物饰品0到背包
		BAG_CHARACTER_CHIP0REPLACE_TOBAG,   	 //人物饰品0替换到背包
		BAG_CHARACTER_CHIP1_TOBAG,		     	 //人物饰品1到背包
		BAG_CHARACTER_CHIP1REPLACE_TOBAG,   	 //人物饰品1替换到背包
		BAG_CHARACTER_CHIP2_TOBAG,		     	 //人物饰品2到背包
		BAG_CHARACTER_CHIP2REPLACE_TOBAG,   	 //人物饰品2替换到背包
		BAG_CHARACTER_PET0_TOBAG,		     	 //人物宠物0到背包
		BAG_CHARACTER_PET0REPLACE_TOBAG,   	 	 //人物宠物0替换到背包
		BAG_CHARACTER_PET1_TOBAG,		     	 //人物宠物0到背包
		BAG_CHARACTER_PET1REPLACE_TOBAG,   	 	 //人物宠物0替换到背包
		BAG_CHARACTER_PET2_TOBAG,		     	 //人物宠物0到背包
		BAG_CHARACTER_PET2REPLACE_TOBAG,   	 	 //人物宠物0替换到背包

		BAG_CHARACTER_HEROLEVELUP_TOBAG,   		 //人物升级到背包

		STRENGTHERNITEN_FOODLEVELUP,			 //吃食物的物品升级
		STRENGTHERNITEN_FOODREPLACELEVELUP,		 //物品升级_替换食品材料

		STRENGTHERNITEN_EQUIPLEVELUP,			 //吃食物的物品升级
		STRENGTHERNITEN_EQUIPREPLACELEVELUP,	 //物品升级_替换食品材料

		STRENGTHERNITEN_STARUP,					 //物品升星
		ADVENTURE_MAINMENU,			//主界面
		ADVENTURE_PREPARE,			//关卡准备界面
		ADVENTURE_GOALS,			//关卡成就界面
		//BAG_ANYWHERETOBAG,		//各种地方进入背包
		//各种进入临时背包
//		TEMPITEM_HEORLEVELUP,		//角色升级
//		TEMPITEM_EQUIPUPGRADE,		//装备点击
//		TEMPITEM_PETUPGRADE,		//宠物点击
//		TEMPITEM_CHESTADD			//宝箱点击加号
	}
	GameUIState m_GameUIState=GameUIState.AdvenTure_Main;

	public GameUIState GetUIState
	{
		get
		{
			return m_GameUIState;
		}
		set
		{
			m_GameUIState=value;
		}
	}
////
//	public UIPerfabsManage()
//	{
//		Messenger.AddListener(BroadcastCutDownTime)
//	}

//s	public void 
	static UIPerfabsManage m_Instan;
	public static UIPerfabsManage g_Instan
	{
		get
		{
			if(m_Instan==null)
			{
				m_Instan=new UIPerfabsManage();
			}
			return m_Instan;
		}
	}
	int m_offLineTime;
	public int OffLineTime
	{
		get
		{
			return m_offLineTime;
		}
		set
		{
			m_offLineTime=value;
		}
	}
	public void SetOffLineTime(int _offLineTime)
	{

		m_offLineTime=_offLineTime;
	}

//	// Use this for initialization
//	void Start () {
//	
//	}
//	
//	// Update is called once per frame
//	void Update () {
//	
//	}
}
