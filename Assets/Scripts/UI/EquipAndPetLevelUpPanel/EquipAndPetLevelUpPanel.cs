using UnityEngine;
using System.Collections;
using FormulaBase;
using System.Collections.Generic;
public enum EquipAndPetLevelUpPanelState
{
	SHOWEXPUP_SKILLUP=1,			//经验技能升级					
	SHOWEXPUP						//经验升级
}
public class EquipAndPetLevelUpPanel : UIPanelBase {


	EquipAndPetLevelUpPanelState  m_EquipAndPetLevelUpPanelState;
	public TweenPosition m_LetfTris;			//三角群
	public TweenPosition m_rightTris;
	public TweenPosition m_Leftrhombus;			//中间菱形
	public TweenPosition m_Rightrhombus;

	public UILabel m_LevelUplabel;				//升级字样
	public TweenScale m_LabelScale;				//升级字样缩放
	public LevelUpCell m_LevelUpCell;			//升级物品
	public TweenScale m_SpriteCellScale;		//升级物品缩放
	public TweenScale m_BallScale;				//篮球的缩放
//	public TweenScale m_UpAndLevelLabel;
	public TweenScale m_ItemLevelUp;
	public TweenScale m_PanelScale;				//面板的缩放
	public GameObject m_LevelUpObject;
	public UIPlayTween m_LevelUpFinishAniMation;//升级动画的结束动画

	FormulaHost m_host;
	#region 吞噬的动画
	public UISprite []m_ChoseObject=new UISprite[4];
	public UISprite []m_ChoseObjectQuility=new UISprite[4];
	public GameObject m_UseItemObject;
	public Animator m_Animation;
	public LevelUpCell m_UseItemLevelUp;
	public GameObject m_ItemParticle;

	/// <summary>
	/// 使用物品动画放完
	/// </summary>
	public void FinishUseItemAnimation()
	{
		m_UseItemObject.gameObject.SetActive(false);
		m_LevelUpObject.gameObject.SetActive(true);
		ShowGradeUp();
	}
	#endregion 
	//public TweenAlpha
	/// <summary>
	/// 设置选择物品的ICon
	/// </summary>
	public void SetChoseData()
	{
		List<FormulaHost> tempList=ItemManageComponent.Instance.GetChosedItem;
		int i=0;
			for(int j=0,max=tempList.Count;j<max;j++)
			{
				for(int k=0,max2=tempList[j].GetDynamicIntByKey(SignKeys.CHOSED);k<max2;k++)
				{
					m_ChoseObject[i].spriteName=tempList[j].GetDynamicIntByKey("ID").ToString();
					i++;
				}
			}
		while(i<4)
		{
			m_ChoseObject[i].spriteName="";
			m_ChoseObjectQuility[i].spriteName="";
			i++;
		}
	}
	/// <summary>
	/// 设置升级动画界面的显示
	/// </summary>
	public void SetLevelUpData()
	{
		m_UseItemLevelUp.SetData(m_host);
		m_LevelUpCell.SetData(m_host);
	}
	public static string BroadEquipAnPetAnimationFinish="BroadEquipAnPetAnimationFinish";
	public static string BroadEquipAnPetAnimationFinish2="BroadEquipAnPetAnimationFinish2";
	/// <summary>
	/// Shows the panel.
	/// </summary>
	/// <param name="_state">State.</param>
	/// <param name="_host">Host.</param>
	/// <param name="_Layer">Layer.</param>
	public override void ShowPanel(int _state=1,FormulaHost _host=null,int _Layer=-1)
	{

		m_host=_host;

		m_EquipAndPetLevelUpPanelState=(EquipAndPetLevelUpPanelState)_state;
		SetChoseData();
		SetLevelUpData();
		m_Animation.Play(ItemManageComponent.Instance.GetChoseCount()+"item");

	}
	/// <summary>
	/// 显示等级提升
	/// </summary>
	public void ShowGradeUp()
	{
		m_LetfTris.PlayForward();
		m_rightTris.PlayForward();

		SetLabel("Upgrade\nSuccessful!!!");
		StartCoroutine("PlaySkillUp");
		StartCoroutine("PlayHidPanel");
//		if(m_EquipAndPetLevelUpPanelState==EquipAndPetLevelUpPanelState.SHOWEXPUP_SKILLUP)
//		{
//			StartCoroutine("PlaySkillUp");
//		}
//
//		else 
//		{
//			StartCoroutine("PlayHidPanel");
//		}
//		else 
//		{
//			StartCoroutine("PlayHidPanel");
//		}
	
	}
	public void ShowStarUp()
	{
		
	}
	/// <summary>
	/// 设置显示的文字
	/// </summary>
	/// <param name="_str">String.</param>
	/// <param name="color">Color.</param>
	void SetLabel(string _str,int color=0)
	{
		m_LabelScale.ResetToBeginning();
		m_LabelScale.PlayForward();
		m_LevelUplabel.text=_str;
		m_Leftrhombus.ResetToBeginning();
		m_Leftrhombus.PlayForward();
		m_Rightrhombus.ResetToBeginning();
		m_Rightrhombus.PlayForward();
		m_SpriteCellScale.ResetToBeginning();
		m_SpriteCellScale.PlayForward();
		m_BallScale.ResetToBeginning();
		m_BallScale.PlayForward();
		m_ItemLevelUp.ResetToBeginning();
		m_ItemLevelUp.PlayForward();
	}
	/// <summary>
	/// 播放技能升级的动画
	/// </summary>
	/// <returns>The skill up.</returns>
	IEnumerator PlaySkillUp()
	{
		yield return new WaitForSeconds(0.8f);

		SetLabel("Sikll up");

	
	}
	/// <summary>
	/// 播放升级动画的退出动画
	/// </summary>
	/// <returns>The hid panel.</returns>
	IEnumerator PlayHidPanel()
	{
		yield return new WaitForSeconds(1.7f);
		CloseUIAnimation();
//		if(m_EquipAndPetLevelUpPanelState==EquipAndPetLevelUpPanelState.SHOWEXPUP_SKILLUP)
//		{
//			yield return new WaitForSeconds(1.6f);
//		}
//		else 
//		{
//			yield return new WaitForSeconds(0.8f);
//		}

	
	}
	/// <summary>
	/// 关闭动画
	/// </summary>
	public void CloseUIAnimation()
	{
		//m_LevelUpFinishAniMation.resetOnPlay;
		m_LevelUpFinishAniMation.Play(true);
		m_ItemParticle.gameObject.SetActive(false);
	//	m_PanelScale.PlayForward();
		
	}
	/// <summary>
	/// 关闭动画放完
	/// </summary>
	public void CloseUIAnimationFinish()
	{
		Debug.Log("CloseUIAnimationFinishFinish>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>");
		Messenger.Broadcast(BroadEquipAnPetAnimationFinish);
		Messenger.Broadcast(ItemInfoPanel.BroadUseItemtolevelupBallAni);
		//Messenger.Broadcast(BroadEquipAnPetAnimationFinish2);
		UIManageSystem.g_Instance.RomoveUI();
	
	}
	void OnEnable()
	{
	//	Messenger.MarkAsPermanent()
	
	}
	void OnDisable()
	{
		
	}
}
