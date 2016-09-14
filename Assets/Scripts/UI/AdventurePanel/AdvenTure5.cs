using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using FormulaBase;
using GameLogic;


public class AdvenTure5 : UIPanelBase {

	MainMenuPanel m_MainMenuPanel=null;
	MainMenuPanel GetMainMenuPanel
	{
		get
		{
			if(m_MainMenuPanel==null)
			{
				m_MainMenuPanel=GameObject.Find("MainMenuPanel").GetComponent<MainMenuPanel>();
			}
			return m_MainMenuPanel;
		}
	}
	public static string AdvenTure5BraodChangeHero="AdvenTure5BraodChangeHero";
	//public GameObject m_HeroParent;
	//GameObject m_nowHeroObject;
	public List<GameObject> m_ListMode=new List<GameObject>();
	/// <summary>
	/// 播放视频的按钮
	/// </summary>
	public void ClickPlayAudio()
	{
		//Handheld.PlayFullScreenMovie("op.mp4", Color.black, FullScreenMovieControlMode.CancelOnInput);
	}
	/// <summary>
	/// 点击战斗的按钮
	/// </summary>
	public void CLickBattleButton()
	{
		UIManageSystem.g_Instance.SetMainMenuBackButton(true);
		UIManageSystem.g_Instance.SetTopPanel(false);
		UIManageSystem.g_Instance.AddUI(UIManageSystem.UILEVELPREPAREPANEL,(int)LevelPrepaerPanelState.LevelPrepaerPanel_DontReSetLevel);
	}
	/// <summary>
	/// 重载显示界面的函数
	/// </summary>
	/// <param name="_state">State.</param>
	/// <param name="_host">Host.</param>
	/// <param name="_Layer">Layer.</param>
	public override void ShowPanel(int _state=1,FormulaBase.FormulaHost _host=null,int _Layer=-1)
	{
		this.gameObject.SetActive(true);
		SetPanelLayer(_Layer);
		ShowHero();
	}
	public void ShowHero()
	{
		int showIndex=RoleManageComponent.Instance.GetFightGirlIndex();
		Debug.Log("ShowHero"+showIndex);
		for(int i=0;i<RoleManageComponent.GirlNumber;i++)
		{
			if(i==showIndex)
			{
				m_ListMode[i].SetActive(true);
			}
			else 
			{
				m_ListMode[i].SetActive(false);
			}
		}
	}

	void OnEnable()
	{
		Messenger.MarkAsPermanent(AdvenTure5BraodChangeHero);
		Messenger.AddListener(AdvenTure5BraodChangeHero,ShowHero);
	}
	void OnDisable()
	{
		Messenger.RemoveListener(AdvenTure5BraodChangeHero,ShowHero);
	}
	void Awake()
	{

	}
	void Start () {

	}
	// Update is called once per frame
	void Update () {
	
	}
	/// <summary>
	/// 点击事件BUTTON
	/// </summary>
	public  void ClickEventsButton()
	{
		CommonPanel.GetInstance().ShowText("研发中!");
	}
	/// <summary>
	/// 点击替换按钮
	/// </summary>
	public void ClickReplaceButton()
	{
		UIManageSystem.g_Instance.AddUI(UIManageSystem.UICHOSEHEROPANEL);
		UIManageSystem.g_Instance.SetMainMenuBackButton(true);
		this.gameObject.SetActive(false);
		//CommonPanel.GetInstance().ShowText("研发中!");
	}
	/// <summary>
	/// 点击英雄
	/// </summary>
	public void ClickHero()
	{
		int HeroIndex = RoleManageComponent.Instance.GetFightGirlIndex ();
		string name = ConfigPool.Instance.GetConfigStringValue ("character_info", (RoleManageComponent.RoleIndexToId (HeroIndex)).ToString (), "name");
		Debug.Log ("点击英雄>>>>>>>>>>>>>>>>>>>" + HeroIndex + " " + name);
		SoundEffectComponent.Instance.Say (name, GameGlobal.SOUND_TYPE_MAIN_BOARD_TOUCH);
		SkeletonAnimation temp=m_ListMode[HeroIndex].GetComponent<SkeletonAnimation>();
		temp.state.ClearTracks();
		temp.state.SetAnimation(0, "1touch", false);
		temp.state.AddAnimation(0,"1stand", true, 0f);
	}
}
