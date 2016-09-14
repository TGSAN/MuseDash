using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using LitJson;
using FormulaBase;
public enum NowUI
{
	ADVENTURE=1,		//主界面
	CHARACTOR=2,		//人物
	GACHA=3,			//背包
	FRIENDS=4,			//好友
	OTHER=5				//其他
} 



public class MainMenuPanel : MonoBehaviour {
	
//	ushort m_NowUI=1;
//	GameObject m_NowObg=null;

//	public GameObject m_AdButton;				  //冒险界面
//	public GameObject m_CharactorButton;		  //角色界面
//	public GameObject m_GachaButton; 	 		  //。。。
//	public GameObject m_FriendPanelButton;	  	  //朋友界面
//	public GameObject m_OtherPanelButton;		  //其他界面

	public GameObject[] m_ArrButton=new GameObject[5];
	public static string BroadcastChangeMoney="BroadcastChangeMoney";
	public static string BroadcastChangeDiamond="BroadcastChangeDiamond";
	public static string BroadcastChangePhysical="BroadcastChangePhysical";
	public static string Broadcast_BagHaveNewItem="Broadcast_BagHaveNewItem";
	public static string Broadcast_MainMenuChangeMoney="Broadcast_MainMenuChangeMoney";
	public static string Broadcast_MainMenuChangeDiamond="Broadcast_MainMenuChangeDiamond";
	public static string Broadcast_MainMenuChangePhysical="Broadcast_MainMenuChangePhysical";
//	public  UIToggle m_AdToggle;		  //冒险界面的Toggle
	GameObject m_ExitButton=null;		  //退出按钮
	//public GameObject m_ItemInfonPanel;   //道具信息界面
	//public GameObject m_BagPanel;		  //背包界面
	public UISprite m_PhysicalRecoverTime;//体力恢复时间

	public UILabel m_money;			//钱
	public UILabel m_diamond;		//钻石
	public UILabel m_Physical; 		//体力
	public UISlider m_ExpLabel;		//EXP

	public GameObject m_Mask;// 挡住连点带来的BUG 
	public TweenPosition m_ExitTween;
	public AudioClip temp222;
	public GameObject m_BagNewLable;

	public TweenScale m_MoneyIcon;			//钱模型的缩放
	public ParticleSystem m_MoneyPartic;

	public TweenScale m_DiamondIcon;			//钱模型的缩放
	public ParticleSystem m_DiamondPartic;

	public TweenScale m_PhyscialIcon;			//钱模型的缩放
	public ParticleSystem m_PhyscialPartic;
	// Use this for initialization
	void Start () {
		SetMoney();
		SetDiamond();
		SetPhysical();
		GameObject uimanage=GameObject.Find("UIManage(Clone)");
		UIState temp=uimanage.GetComponent<UIManage>().GetUIState;
		if(temp==UIState.UISTATE_CLEAR||temp==UIState.UISTATE_FBEXIT||temp==UIState.UISTATE_FBFAIL)
		{
			SetExitButtonShow(true);
		}
		#region 新的UI 管理
		UIManageSystem.g_Instance.AddUI (UIManageSystem.UIADVENTUREPANEL);

		if (UIManageSystem.g_Instance.HaveUI () > 1) {//战斗出关卡的情况
			UIManageSystem.g_Instance.RomoveUI ();
			UIManageSystem.g_Instance.ShowUI ((int)LevelPrepaerPanelState.LevelPrepaerPanel_DontReSetLevel);
		}
		#endregion
		Debug.Log ("StartMainMenupanel------------------------------------------");
	}
	public void UnEnableMask()
	{
		m_Mask.SetActive (false);
	}

	public GameObject m_PhysicalModel;
	public GameObject m_CoinModel;
	public GameObject m_DiamondModel;
	public void SetModelLayer(bool _blur)
	{
		if(_blur)
		{
			m_PhysicalModel.layer=17;
			m_CoinModel.layer=17;
			m_DiamondModel.layer=17;
		}
		else 
		{
			m_PhysicalModel.layer=5;
			m_CoinModel.layer=5;
			m_DiamondModel.layer=5;
		}
		
	}
	void OnDisable()
	{
		Messenger.RemoveListener(BroadcastChangeMoney, SetMoney );
		Messenger.RemoveListener(MainMenuPanel.BroadcastChangeDiamond, SetDiamond );
		Messenger.RemoveListener(MainMenuPanel.BroadcastChangePhysical, SetPhysical );
//		Messenger.RemoveListener(MainMenuPanel.BroadcastChangePhysical, PlayMoneyChangeAnimation );
//		Messenger.RemoveListener(MainMenuPanel.BroadcastChangePhysical, PlayDiamondChangeAnimation );
//		Messenger.RemoveListener(MainMenuPanel.BroadcastChangePhysical, PlayPhysicalChangeAnimation );
		m_Mask.SetActive(true);
		if (CommonPanel.GetInstance () != null) {
			CommonPanel.GetInstance ().ShowGMRobot (false);
		}
	}
	public void SetBagNewLabel()
	{
		if (m_BagNewLable != null) {
			m_BagNewLable.gameObject.SetActive(	BagManageComponent.Instance.GetBagNewItem());
		}

	
	}
	void OnEnable()
	{
		CommonPanel.GetInstance ().ShowGMRobot ();
		Messenger.MarkAsPermanent (BroadcastChangeMoney);
		Messenger.MarkAsPermanent (MainMenuPanel.BroadcastChangeDiamond);
		Messenger.MarkAsPermanent (MainMenuPanel.BroadcastChangePhysical);
		Messenger.AddListener (BroadcastChangeMoney, SetMoney);
		Messenger.AddListener (MainMenuPanel.BroadcastChangeDiamond, SetDiamond);
		Messenger.AddListener (MainMenuPanel.BroadcastChangePhysical, SetPhysical);
		Messenger.MarkAsPermanent (Broadcast_BagHaveNewItem);
		Messenger.AddListener (Broadcast_BagHaveNewItem, SetBagNewLabel);
		Messenger.MarkAsPermanent (Broadcast_MainMenuChangeMoney);
		Messenger.AddListener (Broadcast_MainMenuChangeMoney, PlayMoneyChangeAnimation);
		Messenger.MarkAsPermanent (Broadcast_MainMenuChangeDiamond);
		Messenger.AddListener (Broadcast_MainMenuChangeDiamond, PlayDiamondChangeAnimation);
		Messenger.MarkAsPermanent (Broadcast_MainMenuChangePhysical);
		Messenger.AddListener (Broadcast_MainMenuChangePhysical, PlayPhysicalChangeAnimation);
		Time.timeScale = 1;
		Invoke ("UnEnableMask", 1);
		MoveIdelTime = 0f;
		playMove = false;
		if (m_ExitButton == null) {
			//m_ExitButton = this.transform.FindChild ("ExitButton").gameObject;
		}
		m_ExpLabel.value = 0.5f;
		m_ArrButton [0].SetActive (true);//设置冒险 高亮

	}

	public void SetExitButtonShow(bool _show)
	{
		/*
		TweenAlpha temp;
		if(_show)
		{
			m_ExitTween.Play(true);
		}
		else
		{
			m_ExitTween.Play(false);
		}
		*/
	}

	void SetMoney()
	{
		m_money.text = AccountGoldManagerComponent.Instance.GetMoney ().ToString ();
	}
	void SetDiamond() {
		int d = AccountCrystalManagerComponent.Instance.GetDiamond ();
		Debug.Log ("设置玩家钻石:" + d);
		//PlayDiamondChangeAnimation();
		m_diamond.text = d.ToString ();
	}
	void SetPhysical()
	{
		m_Physical.text=AccountPhysicsManagerComponent.Instance.GetPhysical().ToString()+"/"+AccountPhysicsManagerComponent.Instance.GetMaxPhysical().ToString();
	}
	/// <summary>
	/// Sets the recover physical.
	/// </summary>
	/// <param name="_value">Value.</param>
	void SetRecoverPhysical(float _value)
	{
		Debug.LogWarning("设置恢复体力百分比");
		m_PhysicalRecoverTime.fillAmount=_value;
	}

	bool playMove=false;
	// Update is called once per frame
	void Update () {
		MoveIdelTime+=Time.deltaTime;
		if(MoveIdelTime>30)
		{
			playMove=true;
			//Handheld.PlayFullScreenMovie("op.mp4", Color.black, FullScreenMovieControlMode.CancelOnInput);
			MoveIdelTime=0;
		}
		if(Input.GetMouseButtonDown(0))
		{
			MoveIdelTime=0f;
		}
	}
	/// <summary>
	/// 显示隐藏 Toggle  因为要做动画 不用toggle 做
	/// </summary>
	/// <param name="_index">Index.</param>
	void HideAndShowBUtton(int _index)
	{
		for(int i=0,max=m_ArrButton.Length;i<max;i++)
		{
			if(i==_index)
			{
				m_ArrButton[i].SetActive(true);	
			}
			else 
			{
				if (m_ArrButton [i].activeSelf) 
				{
					AudioClipDefine.AudioClipManager.Get ().ExitUI ();
				}
				m_ArrButton[i].SetActive(false);
			}
		}
		
	}
//	public Bool
	public void ClickAdventure()
	{
	//	Debug.Log("点击冒险按钮");
		//m_AdButton.SetActive(true);
		if(m_ArrButton[0].activeSelf)
			return ;
		HideAndShowBUtton(0);
		UIManageSystem.g_Instance.RemoveToRoot();
		UIManageSystem.g_Instance.AddUI(UIManageSystem.UIADVENTUREPANEL);
		SetExitButtonShow(false);

	}
	public void ClickCharactor()
	{
//		if(!_chose)
//			return;
		//m_AdToggle.value=false;
		if(m_ArrButton[1].activeSelf)
			return ;
		HideAndShowBUtton(1);
		UIManageSystem.g_Instance.RemoveToRoot();
		UIManageSystem.g_Instance.AddUI(UIManageSystem.UICHARACTORPANEL);
		SetExitButtonShow(false);
	}
	public void ClickBag()
	{
//		if(!_chose)
//			return;
		//m_AdToggle.value=false;
		if(m_ArrButton[2].activeSelf)
			return ;
		HideAndShowBUtton(2);
		UIManageSystem.g_Instance.RemoveToRoot();
		UIManageSystem.g_Instance.AddUI(UIManageSystem.UIBAGPANEL,(int)BagPanel2State.BagPanel2_ShowAll*10);
		SetExitButtonShow(false);

	}
	public void ClickStore()
	{
		if(m_ArrButton[3].activeSelf)
			return ;
		HideAndShowBUtton(3);
		UIManageSystem.g_Instance.RemoveToRoot();
		UIManageSystem.g_Instance.AddUI(UIManageSystem.UISTOREPANEL);
		SetExitButtonShow(false);
	}

	public void ClickOtherButton()
	{
		if(m_ArrButton[4].activeSelf)
			return ;
		HideAndShowBUtton(4);
		UIManageSystem.g_Instance.RemoveToRoot();
		UIManageSystem.g_Instance.AddUI(UIManageSystem.UIOTERHPANEL);
		SetExitButtonShow(false);
	}

//	public void CloseInfoPanel()
//	{
//		if(m_ItemInfonPanel.activeSelf)
//		{
//			m_ItemInfonPanel.SetActive(false);
//		}
//	}
//	public GameObject m_GoalPanel;
	/// <summary>
	/// 点击主菜单的BacK按钮
	/// </summary>
	/// <param name="_BigChange">If set to <c>true</c> 就是大界面的切换.</param>
	public void ClickCancel(bool _BigChange=false)
	{
		Debug.Log("ClickBackButton-----------------------------------back");
		#region UI管理器逻辑
		UIManageSystem.g_Instance.ClickBackButton();
		return ;
		#endregion
	}

	#region
	float MoveIdelTime=0f;

	public void CLickTestButton()
	{
		GameObject CommonPanel=GameObject.Find("CommonPanel");
		CommonPanel.GetComponent<CommonPanel>().ShowText("");
	}
	#endregion
	public  void ClickTaskButton()
	{

		UIManageSystem.g_Instance.AddUI(UIManageSystem.UIDAILYTASHPANEL);
		UIManageSystem.g_Instance.SetMainMenuBackButton(true);
		Debug.Log("点击活跃任务");
	}
	public void PlayMoneyChangeAnimation()
	{
		if (m_money == null) {
			return;
		}

		m_MoneyIcon.ResetToBeginning();
		m_MoneyIcon.PlayForward();
		m_MoneyPartic.Play();
		//钱模型的缩放
	//	public ParticleSystem m_MoneyPartic;
	}
	public void PlayDiamondChangeAnimation()
	{
		if (m_DiamondIcon == null) {
			return;
		}

		m_DiamondIcon.ResetToBeginning();
		m_DiamondIcon.PlayForward();
		m_DiamondPartic.Play();
		//钱模型的缩放
		//	public ParticleSystem m_MoneyPartic;
	}
	public void PlayPhysicalChangeAnimation()
	{
		if (m_PhyscialIcon == null) {
			return;
		}

		m_PhyscialIcon.ResetToBeginning();
		m_PhyscialIcon.PlayForward();
		m_PhyscialPartic.Play();
		//钱模型的缩放
		//	public ParticleSystem m_MoneyPartic;
	}
}
