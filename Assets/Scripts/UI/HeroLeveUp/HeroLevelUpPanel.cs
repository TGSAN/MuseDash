using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using FormulaBase;
public enum LevelUpCritState
{
	HeroLevelUp_One,			//一倍爆率
	HeroLevelUp_OnePointFive,	//1.5被爆率
	HeroLevelUp_Two				//2被爆率
}
public class HeroLevelUpPanel : UIPanelBase {
	
	public GameObject m_AniMation1;
	public UISprite [] m_ArrChoseItem=new UISprite[4];
	public UIPlayTween m_EatFoodAnimation;
	public TweenPosition m_MosterMove;
	public TweenPosition m_TopBlack;
	public TweenPosition m_BottomBlack;
	public List<GameObject> m_listLevelUpFont=new List<GameObject>(); 

	//动画第二部分
//public GameObject 
	public GameObject m_AniMation2;
	public TweenAlpha m_WhiteMask;//开场白色遮罩

	public TweenPosition m_BackFont;//背后字体
	public TweenPosition m_HeroMove;//角色的移动
	public TweenAlpha m_AlphaHero;//角色透明
	public TweenScale m_AlphaHeroScale;//透明角色的缩放

	public UISprite m_HeroSprite;		//升级的小人物
	public UITexture m_HeroSprite2;
	public UISprite m_Bg;				//升级的背景
	public UISprite m_HeroSpriteBig;	//升级的大人物
	public UITexture m_HeroSpriteBig2;
	public TweenAlpha m_BlackMask;
	public static LevelUpCritState g_LevelUpCritStage=LevelUpCritState.HeroLevelUp_One;
//	ItemInfoPanel m_ItemInfoPanel=null;
//	public  ItemInfoPanel GetItemInfoPanel
//	{
//		get
//		{
//			if(m_ItemInfoPanel==null)
//			{
//				m_ItemInfoPanel=this.transform.parent.Find("ItemInfoPanel").gameObject.GetComponent<ItemInfoPanel>();
//			}
//			return m_ItemInfoPanel;
//
//		}
//	}
//
	/// <summary>
	/// 设置吞噬的物品图片
	/// </summary>
	public void SetUseFood()
	{


		for(int j=0,i=0,AllChosecount=ItemManageComponent.Instance.GetChoseCount();i<4;j++)
		{
			if(j<AllChosecount)
			{
				for(int k=0,max=ItemManageComponent.Instance.GetChosedItem[j].GetDynamicIntByKey(SignKeys.CHOSED);k<max;k++)
				{
					ItemManageComponent.Instance.GetChosedItem[j].Result(FormulaKeys.FORMULA_26);

					m_ArrChoseItem[i].spriteName=ItemManageComponent.Instance.GetChosedItem[j].GetDynamicStrByKey("ID");
					i++;
				}
			}
			else 
			{
				m_ArrChoseItem[i].gameObject.SetActive(false);
				i++;
			}
		}
	}
	public void Update()
	{
		//Messenger.Broadcast(ItemInfoPanel.BroadUseItemtolevelupBallAni);
	}
	/// <summary>
	/// Shows the panel.
	/// </summary>
	/// <param name="_state">State.</param>
	/// <param name="_host">Host.</param>
	/// <param name="_Layer">Layer.</param>
	public override void ShowPanel(int _state=1,FormulaHost _host=null,int _Layer=-1)
	{

		g_LevelUpCritStage=(LevelUpCritState)_state;
		//获取当前角色的索引
		//获取当前暴击倍数
		m_AniMation1.SetActive(true);
		m_MosterMove.ResetToBeginning();
		m_MosterMove.onFinished.Add(new EventDelegate(EatFoodAnimationFinish));
		m_MosterMove.PlayForward();
		m_TopBlack.PlayForward();
		m_BottomBlack.PlayForward();
		m_EatFoodAnimation.tweenGroup=1;
		m_EatFoodAnimation.Play(true);
		m_EatFoodAnimation.tweenGroup=2;
		m_EatFoodAnimation.Play(true);
		SetUseFood();



//		Debug.LogError(" use Father fun");
//
//		SetPanelLayer(_Layer);
		//this.gameObject.GetComponent<UIPanel>().depth;
		//this.gameObject.GetComponent<UIPanel>().sortingOrder;
	}
	//buro_cool buro_great buro_perfect
	//lumi_cool lumi_great lumi_perfect
	//malya_cool malya_great malya_perfect
	/// <summary>
	/// 吃食物动画播放完
	/// </summary>
	public void EatFoodAnimationFinish()
	{
		m_MosterMove.onFinished.Clear();
		SetFunDiffent();
		PlayAnimation2();
	}
	public void SetFunDiffent()
	{
		//设置弹出的妹子
		switch(ItemManageComponent.Instance.GetMultiplying)
		{
		case ItemManageComponent.ItemCritStyel.CRITE_ONE:
			switch(RoleManageComponent.Instance.GetFightGirlIndex())
			{
			case 0:
				m_HeroSprite.spriteName="lumi_cool";
				break;
			case 1:
				m_HeroSprite.spriteName="malya_cool";
				break;
			case 2:
				m_HeroSprite.spriteName="buro_cool";
				break;
			}
			break;
		case ItemManageComponent.ItemCritStyel.CRITE_ONEPOINTFIVE:
			switch(RoleManageComponent.Instance.GetFightGirlIndex())
			{
			case 0:
				m_HeroSprite.spriteName="lumi_great";
				break;
			case 1:
				m_HeroSprite.spriteName="malya_great";
				break;
			case 2:
				m_HeroSprite.spriteName="buro_great";
				break;
			}
			break;
		case ItemManageComponent.ItemCritStyel.CRITE_TWO:
			switch(RoleManageComponent.Instance.GetFightGirlIndex())
			{
			case 0:
				m_HeroSprite.spriteName="lumi_perfect";
				break;
			case 1:
				m_HeroSprite.spriteName="malya_perfect";
				break;
			case 2:
				m_HeroSprite.spriteName="buro_perfect";
				break;
			}
			break;
		}
		m_HeroSpriteBig.spriteName=m_HeroSprite.spriteName;
		//设置背景
		//switch(ItemManageComponent.Instance.GetMultiplying)
		{
		//case ItemManageComponent.ItemCritStyel.CRITE_ONE:
	//		m_Bg.spriteName="lumi_cool_bg";
	//		Debug.Log("111");
	//		break;
	//	case  ItemManageComponent.ItemCritStyel.CRITE_ONEPOINTFIVE:
	//		m_Bg.spriteName="lumi_great_bg";
	//		Debug.Log("222");
	//		break;
	//	case  ItemManageComponent.ItemCritStyel.CRITE_TWO:
	//		m_Bg.spriteName="lumi_perfect_bg";
	//		Debug.Log("333");
	//		break;
		}
		int tindex=RoleManageComponent.Instance.GetFightGirlIndex();

		for(int i=0;i<m_listLevelUpFont.Count;i++)
		{
			m_listLevelUpFont[i].SetActive(false);
		}
		m_listLevelUpFont[tindex*3+(int)ItemManageComponent.Instance.GetMultiplying].SetActive(true);
	}
	public void PlayAnimation2()
	{
		//GetItemInfoPanel.PlayAddBallAnimationMess();
		m_AniMation1.SetActive(false);
		m_AniMation2.SetActive(true);
		m_WhiteMask.PlayForward();
		m_BackFont.PlayForward();//背后字体
		m_HeroMove.PlayForward();//角色的移动
		m_AlphaHero.PlayForward();//角色透明
		m_AlphaHeroScale.PlayForward();//透明角色的缩放
	//	m_BackFont.onFinished.Add(new EventDelegate(Animation2Finish));
		//m_WhiteMask.ResetToBeginning();
		m_BlackMask.PlayForward();
		m_BlackMask.onFinished.Add(new EventDelegate(Animation2Finish));

		//StartCoroutine("CloseUI");
	}
	/// <summary>
	/// 动画播放完成
	/// </summary>
	public void Animation2Finish()
	{
		m_BlackMask.onFinished.Clear();
	
		UIManageSystem.g_Instance.RomoveUI();
	}
//	IEnumerator CloseUI()
//	{
//		yield return new WaitForSeconds(0.5f);
//		UIManageSystem.g_Instance.RomoveUI();
//	}
//
}
