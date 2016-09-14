// EDIT:SF 关卡准备
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using FormulaBase;
using GameLogic;
enum LevelPrepaerPanelState
{
	LevelPrepaerPanel_ReSetLevel=1,			//重置关卡位置

	LevelPrepaerPanel_DontReSetLevel=2,		//不重置关卡位置

	LevelPrepaerPanel_ChosedChapter=3,		//章节选择界面

	LevelPrepaerPanel_GameToMenu=4			//关卡到主界面
}

public class LevelPrepaerPanel : UIPanelBase {
	public bool  NEWCELLTEMP=true; 

	GameObject m_levelCell;
	public GameObject m_table;
	List<GameObject> m_listLevel=new List<GameObject>();
//
//	GameObject m_Achievement11;			//目标1
//	GameObject m_Achievement12;			//目标2
//	GameObject m_Achievement13;			//目标3

	GameObject m_GetMoney;		
	LevelPrepaerPanelState m_LevelPrepaerPanelState=LevelPrepaerPanelState.LevelPrepaerPanel_ChosedChapter;
	public static string BraodCast_RestChestEmpty="BraodCast_RestChestEmpty";
	public static string BraodCast_ChestMissAni="BraodCast_ChestMissAni";			//宝箱消失动画
	//public static string BraodCast_ChestAppearAni="BraodCast_ChestAppearAni";		//宝箱出现动画

	public UIPanel m_ChapterPanel;
	public UILabel m_AllChestTime;
	public GameObject m_AllChestTimeDes;		 //总时间的描述文本
	#region UI管理系统
	public static int NowLevel=0;	   			 //界面当前选择关卡
	public static int NowChapterIndex=0;	     //界面当前选择关卡
	public static int oldChapterIndex=0;		 //原来老的章节选择
	public UIPlayTween m_ChestMissTween;		 //宝箱MIss的动画
	//public UIPlayTween m_ChestApperTween;		 //宝箱出现的动画i
	public static int g_ClickChestIndex=10;
	public  GameObject AniMationMask; 
	#region 动画--宝箱

	/// <summary>
	/// 宝箱隐藏动画
	/// </summary>
	/// <param name="_group">Group.</param>
	public void PlayMissaAnimation(int _group=10)
	{
//		Messenger.Broadcast(LevelPrepaerPanel.BraodCast_RestChestEmpty);
//		return ;
		AniMationMask.SetActive(true);
		m_ChestMissTween.tweenGroup=_group;
		if(ChestManageComponent.Instance.GetOwnedChestNumber()==0)
		{
			Debug.Log("没物品时的动画");
			//m_listChestGrid[0].SetActive(true);

			m_ChestMissTween.tweenGroup=12;
			StartCoroutine("StartempShowChest");
		}
		else 
		{
			m_ChestMissTween.Play(true);
		}
		if(_group==12)
		{
			Debug.Log(">>>>>>>>>>>>>>>>>>>>>开始播放宝箱消失动画");
		}
 	}
	//动画间隔得隔一帧 不然出BUG
	IEnumerator StartempShowChest()
	{
		yield return 0;
		Messenger.Broadcast(LevelPrepaerPanel.BraodCast_RestChestEmpty);
		m_ChestMissTween.Play(true);
	}
	/// <summary>
	/// 宝箱消失动画播放完
	/// </summary>
	public void MissAnimationFinish()
	{
		Debug.Log("jock-------------?"+m_ChestMissTween.tweenGroup);
		if(m_ChestMissTween.tweenGroup==10)
		{
			Messenger.Broadcast(LevelPrepaerPanel.BraodCast_RestChestEmpty);
			PlayMissaAnimation(12);
			Debug.Log("MissAnimation");
		}
		else if(m_ChestMissTween.tweenGroup==12)
		{
			Debug.Log("AppearAnimation");
			AniMationMask.SetActive(false);
		}

	}

	#endregion

//	public GameObject m_Scanning;				//一号栏位流光
//	public GameObject m_GirdOneBox;				//一号栏位的外边框

	/// <summary>
	/// Shows the panel.
	/// </summary>
	/// <param name="_statge">Statge.</param>
	/// <param name="_host">Host.</param>
	/// <param name="_Layer">Layer.</param>
	public override void ShowPanel (int _statge,FormulaBase.FormulaHost _host=null,int _Layer=-1)
	{
		this.gameObject.SetActive(true);
	
		if(_Layer!=-1)
		{
			SetPanelLayer(_Layer);
			m_ChapterPanel.depth=_Layer*5+1;
			m_ChapterPanel.sortingOrder=_Layer*5+1;
		}
		Debug.Log("LevelPrepaerPanel  statge is-->"+_statge);
		m_LevelPrepaerPanelState=(LevelPrepaerPanelState)_statge;
		switch((LevelPrepaerPanelState)_statge)
		{
		case LevelPrepaerPanelState.LevelPrepaerPanel_DontReSetLevel:
			m_ChoseChapter.SetActive(false);
			SetPanelLayer(UIManageSystem.g_Instance.HaveUI());
			m_ChapterPanel.depth=UIManageSystem.g_Instance.HaveUI()*5+1;
			m_ChapterPanel.sortingOrder=UIManageSystem.g_Instance.HaveUI()*5+1;
			RefreshNewCell();	
			AudioClipDefine.AudioClipManager.Get().OpenUI();
			m_LevelChangeChapter.SetActive(true);
			break;
		case LevelPrepaerPanelState.LevelPrepaerPanel_GameToMenu:
			m_ChoseChapter.SetActive(false);
			SetPanelLayer(UIManageSystem.g_Instance.HaveUI());
			m_ChapterPanel.depth=UIManageSystem.g_Instance.HaveUI()*5+1;
			m_ChapterPanel.sortingOrder=UIManageSystem.g_Instance.HaveUI()*5+1;
			RefreshNewCell();	
			AudioClipDefine.AudioClipManager.Get().OpenUI();
			m_LevelChangeChapter.SetActive(true);
			Debug.Log(">>>>>>>>>>>>>>>>>>>>>>>>>>>>>>关卡出来要做的事");
			break;
		case LevelPrepaerPanelState.LevelPrepaerPanel_ReSetLevel:
			NowLevel=0;
			//m_ChoseLevel.SetActive(true);
		
			m_ChoseChapter.SetActive(false);
			RefreshNewCell();	
			AudioClipDefine.AudioClipManager.Get().OpenUI();
			m_LevelChangeChapter.SetActive(true);
			break;
		case LevelPrepaerPanelState.LevelPrepaerPanel_ChosedChapter:
			//m_ChoseLevel.SetActive(false);
		
		//	m_ChoseChapter.SetActive(true);	
			AudioClipDefine.AudioClipManager.Get().ExitUI();
		//	m_LevelChangeChapter.SetActive(false);
			break;
		}
		SetChapterDes();

		ChestManageComponent.Instance.ChestBagToGrid();

		SetEmpty();
		SetAllTime();
	}
	#endregion


	#region 宝箱相关
	public List<GameObject> m_listChestGrid=new List<GameObject>();
	//public GameObject

	/// <summary>
	/// 设置宝箱上空格的位置 
	/// </summary>
	public void SetEmpty()
	{
		int MaxGrid=AccountManagerComponent.Instance.GetChestGirdNumber();
		Debug.Log("玩家最大箱子L:"+MaxGrid+"玩家拥有的箱子数:"+ChestManageComponent.Instance.GetOwnedChestNumber());
		for(int i =0,max=6,haveChest=ChestManageComponent.Instance.GetOwnedChestNumber();i<max;i++)
		{
			if(i<MaxGrid)
			{
				m_listChestGrid[i].GetComponent<ChestGridCell>().SetEmpty();
			}
			else 
			{
				if(i<MaxGrid+1)
				{
					m_listChestGrid[i].GetComponent<ChestGridCell>().SetLocked(true);
				}
				else 
				{
					m_listChestGrid[i].GetComponent<ChestGridCell>().SetLocked(false);
				}
			}
		}
		for(int i=0,max=ChestManageComponent.Instance.GetChestList.Count;i<max;i++)
		{
			int tindex=ChestManageComponent.Instance.GetChestList[i].GetDynamicIntByKey(SignKeys.CHESTQUEUE)-1;
			Debug.Log("tindex-------------------->>>>>>>>>"+tindex);
			m_listChestGrid[tindex].GetComponent<ChestGridCell>().SetSomething(ChestManageComponent.Instance.GetChestList[i]);
		}

		for(int i=0,max=ChestManageComponent.Instance.GetTimeDownChest.Count;i<max;i++)
		{
			int tindex=ChestManageComponent.Instance.GetTimeDownChest[i].GetDynamicIntByKey(SignKeys.CHESTQUEUE)-1;
//			Debug.Log("tindex-------------------->>>>>>>>>"+tindex);
		//	Debug.Log("XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX");
			if (tindex >= m_listChestGrid.Count) {
				continue;
			}

			m_listChestGrid[tindex].GetComponent<ChestGridCell>().SetSomething(ChestManageComponent.Instance.GetTimeDownChest[i]);
		}
	}
	/// <summary>
	/// 设置所有宝箱的剩余时间
	/// </summary>
	public void SetAllTime() {
		if (this.m_AllChestTime == null) {
			return;
		}

		string temp = ChestManageComponent.Instance.GetAllChestTime ();
		if (temp == null || temp.Length == 0) {
			m_AllChestTime.text = "空闲中";
			m_AllChestTimeDes.SetActive (false);
			//m_AllChestTime.transform.parent.gameObject.SetActive(false);
		} else {
			//m_AllChestTime.transform.parent.gameObject.SetActive(true);	
			m_AllChestTime.text = temp;
			m_AllChestTimeDes.SetActive (true);
		}
	}

//	public void 

	/// <summary>
	/// 初始化宝箱队列
	/// </summary>
	public void InitChestGrid()
	{
		SetEmpty();
	}
//	public void 
	#endregion

	#region FB 出来的东西
	/// <summary>
	/// 显示获取钱和经验
	/// </summary>
	public void ShowGetMoney()
	{
		if(m_GetMoney==null)
		{
			m_GetMoney=transform.FindChild("GetMoneyPanel").gameObject;
		}
		m_GetMoney.SetActive(true);
	}
	#endregion

	#region 章节的移动

	public List<GameObject> m_listChapterCell=new List<GameObject>();
	public UIPlayTween m_PlayChapterTween;

	public UILabel m_ChapterDes;

	/// <summary>
	/// 设置章节的描述
	/// </summary>
	void SetChapterDes()
	{
		if(m_LevelPrepaerPanelState==LevelPrepaerPanelState.LevelPrepaerPanel_ChosedChapter)
		{
			m_ChapterDes.text="RANK MASTER";
		}
		else
		{
			m_ChapterDes.text="Chapter "+(NowChapterIndex+1);
		}

	}
	#endregion

	#region 圆盘缩放的关卡移动

	public  List<GameObject> m_listLevelCell2=new List<GameObject>();

	public UIPlayTween m_PlayTween;



	//------------------
	//		//m_textRate.text = levelData.GetFinRate ().ToString () + "%";
	//		m_textScore.text = FormulaBase.StageBattleComponent.Instance.GetSorceBest ().ToString ();

//	-		if(m_label==null)
//		-		{
//		-			m_label=transform.FindChild("SongLabel").GetComponent<UILabel>();
//		-		}
//	+//		if(m_label==null)
//	+//		{
//	+//			m_label=transform.FindChild("SongLabel").GetComponent<UILabel>();
//	+//		}
//	m_id=_index;
//	FormulaBase.StageBattleComponent.Instance.SetStageId ((uint)(m_id+1));
//	//m_label.text="STAGE "+(_index+1).ToString();
//	m_sprite.spriteName=FormulaBase.StageBattleComponent.Instance.GetStageIconName();
//	-	//	FormulaBase.StageBattleComponent.Instance.GetStage().Result(FormulaKeys.FORMULA_37)geResult(.);
//	-
//	-		m_label.text=FormulaBase.StageBattleComponent.Instance.GetStageName();
//
//	+		//m_label.text=FormulaBase.StageBattleComponent.Instance.GetStageName();
//
//	-		FormulaBase.FormulaHost host = FormulaBase.StageBattleComponent.Instance.GetStage((m_id+1));
//	-		m_PhysicalLabel.text=(host.Result(FormulaBase.FormulaKeys.FORMULA_72)).ToString();
//	-	//	bug.LogWarning("TRUE id:"+(m_id+1));
//	-		//Debug.LogWarning(host.Result(FormulaBase.FormulaKeys.FORMULA_68));
//	-		//m_sprite.spriteName=nametttt[_index%nametttt.Length];
//	-		//Debug.Log("ddddddddddddddddddddd"+nametttt[_index%nametttt.Length]);
//
//	+	//	FormulaBase.FormulaHost host = FormulaBase.StageBattleComponent.Instance.GetStage((m_id+1));
//	+	//	m_PhysicalLabel.text=(host.Result(FormulaBase.FormulaKeys.FORMULA_72)).ToString();

//}

	//--------------------
	//public int i=10;
	/// <summary>
	/// 刷新当前章节的歌曲
	/// </summary>
	public void RefreshNewCell()
	{
		//		int index=LevelPrepaerPanel.NowLevel;
		//		List<int> ListStageIndex=StageBattleComponent.Instance.GetStageIdsInChapter(NowChapterIndex+1);
		//
		//		int OpendeChapter=AccountManagerComponent.Instance.GetOpenedChapter();
		//		for(int i=0,max=m_listLevelCell2.Count;i<max;i++)
		//		{
		//			if(index+i-2<0)
		//			{
		//				m_listLevelCell2[i].SetActive(false);
		//			}
		//			else if(index+i-2>ListStageIndex.Count-1)
		//			{
		//				m_listLevelCell2[i].SetActive(false);
		//			}
		//			else 
		//			{
		//				m_listLevelCell2[i].SetActive(true);
		//				m_listLevelCell2[i].GetComponent<LevelCell3>().SetData(ListStageIndex[index+i-2]);
		//			}
		//		}
		//		ushort stageId=(ushort)(m_listLevelCell2[2].GetComponent<LevelCell3>().m_id);
		//		StageBattleComponent.Instance.SetStageId ((uint)(stageId));
		//		SetSongInfo(stageId);
		bool isNextLock = true;
		List<AudioClipDefine.ChapterSongData> temp = new List<AudioClipDefine.ChapterSongData> ();
		//int index=LevelPrepaerPanel.NowLevel;
		int NowChapter = LevelPrepaerPanel.NowChapterIndex;
		bool isLashChapterClear = StageBattleComponent.Instance.IsChapterCleared (NowChapterIndex);
		if (isLashChapterClear) {
			Debug.Log ("章节" + NowChapterIndex + "全通过");
		} else {
			Debug.Log ("章节" + NowChapterIndex + "未通过");
		}

		List<int> ListStageIndex = StageBattleComponent.Instance.GetStageIdsInChapter (NowChapterIndex + 1);
		for (int i = 0, max = ListStageIndex.Count; i < max; i++) {
			FormulaBase.StageBattleComponent.Instance.SetStageId ((uint)(ListStageIndex [i]));

			AudioClipDefine.ChapterSongData tLevelCell = new AudioClipDefine.ChapterSongData ();
			tLevelCell.SongID = ListStageIndex [i];

			// Lock state check
			if (GameGlobal.IS_UNLOCK_ALL_STAGE) {
				tLevelCell.IsLock = false;
			} else {
				TaskStageTarget.Instance.Init (tLevelCell.SongID);
				tLevelCell.IsLock = TaskStageTarget.Instance.IsLock ();
				if (tLevelCell.IsLock && !isNextLock) {
					tLevelCell.IsLock = false;
				}

				if (i == 0 && isLashChapterClear) {
					tLevelCell.IsLock = false;
				}

				isNextLock = TaskStageTarget.Instance.IsNextLock ();
			}

			//StageBattleComponent.Instance.GetId();
			tLevelCell.SongName = StageBattleComponent.Instance.GetStageDesName ();
			tLevelCell.IconSpriteName = StageBattleComponent.Instance.GetStageIconName ();
			tLevelCell.CostTili = StageBattleComponent.Instance.GetStagePhysical ();
			tLevelCell.SongDiffcult = 22;
			tLevelCell.SongText = StageBattleComponent.Instance.GetSatageAuthor ();
			tLevelCell.SongIndex = i;

			string musicPath = AudioManager.MUSIC_PATH + StageBattleComponent.Instance.GetMusicName ();
			tLevelCell._AudioClp = musicPath;

			temp.Add (tLevelCell);
			//Debug.Log("ID>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>"+tLevelCell.SongID);
		}

		AudioClipDefine.AudioClipManager.Get ().InitUI (m_LevelChangeChapter, temp);
	}
	/// <summary>
	/// 刷新章节
	/// </summary>
	public void RefreshChapterNewCell()
	{
		/*
		int index=LevelPrepaerPanel.NowChapterIndex;
		for(int i=0,max=m_listChapterCell.Count;i<max;i++)
		{
			if(index+i-2<0)
			{
				m_listChapterCell[i].SetActive(false);
			}
			else if(index+i-2>ChapterManageComponent.Instance.GetChapterCount()-1)
			{
				m_listChapterCell[i].SetActive(false);
			}
			else 
			{
				m_listChapterCell[i].SetActive(true);
				m_listChapterCell[i].GetComponent<ChapterCell>().SetData(index+i-2);
			}
		}
*/
	}

	#endregion
	UIManage uimanage;
	void Awake()
	{
	}
	public void ClickMasterButton()
	{
		GameObject.Find("CommonPanel").GetComponent<CommonPanel>().ShowText("SetEmpty");
	}
	void OnEnable()
	{
		//m_table.GetComponent<UICenterOnChild>().onCenter=onCenter;
		Debug.Log("注册------------------------------------------------");
		Messenger.MarkAsPermanent(BraodCast_RestChestEmpty);
		Messenger.AddListener(BraodCast_RestChestEmpty,SetEmpty);
		Messenger.MarkAsPermanent(ChestGridCell.BroadcastCutDownTime);
		Messenger.AddListener(ChestGridCell.BroadcastCutDownTime,SetAllTime);

		Messenger.MarkAsPermanent(BraodCast_ChestMissAni);
		Messenger.AddListener<int>(BraodCast_ChestMissAni,PlayMissaAnimation);
//		Messenger.MarkAsPermanent(BraodCast_ChestAppearAni);
//		Messenger.AddListener(BraodCast_ChestAppearAni,PlayApperAnimation);
//		public static string BraodCast_ChestMissAni="BraodCast_ChestMissAni";			//宝箱消失动画
//		public static string BraodCast_ChestAppearAni="BraodCast_ChestAppearAni";		//宝箱出现动画
		ItemManageComponent.Instance.CheckChestTime();
	//	CheckChestTime()
	//	InitChestGrid();
	}

	void OnDisable()
	{
		Messenger.RemoveListener(BraodCast_RestChestEmpty,SetEmpty);
		Messenger.RemoveListener(ChestGridCell.BroadcastCutDownTime,SetAllTime);
		Messenger.RemoveListener<int>(BraodCast_ChestMissAni,PlayMissaAnimation);
	}
	public UILabel m_SongName;
	public UILabel m_AuthorName;
	void SetSongInfo(int _index)
	{
		m_SongName.text=FormulaBase.StageBattleComponent.Instance.GetStageName();
	}
	void onCenter(GameObject _ob)
	{
//		if(nowChoseed==_ob)
//		{
//			return;
//		}
//		nowChoseed=_ob;
//		ushort stageId=(ushort)(nowChoseed.GetComponent<LevelCell3>().m_id+1);
//		FormulaBase.StageBattleComponent.Instance.SetStageId ((uint)(stageId));
//		SetSongInfo(stageId);
	}

	GameObject nowChoseed=null;

//	public void ClickStartButton()
//	{
////		int sid;
////		sid=m_listLevelCell2[2].GetComponent<LevelCell3>().m_id+1;
////		FormulaBase.StageBattleComponent.Instance.SetStageId ((uint)sid);
////		UIManageSystem.g_Instance.AddUI(UIManageSystem.UILEVELGOALSPANEL,1,StageBattleComponent.Instance.GetStage(sid));
//	}
	// Use this for initialization
	void Start () {

		RefreshChapterNewCell();

	//	RefreshNewCell();
	}
	// Update is called once per frame
	void Update () {

	}
	#region Test
	public GameObject m_temp1;
	public void ClickReplace()
	{
	}
	/// <summary>
	/// 实际为开始关卡按钮  改了面板不想改
	/// </summary>
//	public void ShowGoldsPanel()
//	{
//	//	ClickStartButton();
//	}

	public void ShowEvents()
	{
//		if(ChestManageComponent.Instance.ChestBagToGrid())
//		{
//				SetEmpty();
//		}

		ChestManageComponent.Instance.ChestBagToGrid();

		//GameObject.Find("CommonPanel").GetComponent<CommonPanel>().ShowText("");
	}
	#endregion
	float MinTimeInterval=0.2f; //最小时间间隔
	float TimeCountBegin=0f;
	float TimeCountEnd=0f;
	/// <summary>
	/// 左移动
	/// </summary>
	public void ClickRightButton()
	{

		//控制最小时间
		TimeCountEnd=RealTime.time;
		if(TimeCountEnd-TimeCountBegin>MinTimeInterval)
		{
			TimeCountBegin=TimeCountEnd;
		}
		else 
		{
			return ;
		}
		if(m_LevelPrepaerPanelState==LevelPrepaerPanelState.LevelPrepaerPanel_ChosedChapter)
		{
			/*
			if(NowChapterIndex+1>ChapterManageComponent.Instance.GetChapterCount()-1)
				return ;
			if(NowChapterIndex+1>AccountManagerComponent.Instance.GetOpenedChapter())
				return ;
				*/
			NowChapterIndex+=1;
			RefreshChapterNewCell();//逆向移动 得先刷下 XXXXXXXXXXXXXX
			m_PlayChapterTween.Play(false);
		}
//		if(NEWCELLTEMP)
//		{			
//			if(NowLevel+1>StageBattleComponent.Instance.GetStageIdsInChapter(NowChapterIndex+1).Count-1)
//				return;
//			NowLevel+=1;
//			//RefreshNewCell();
//			m_PlayTween.Play(true);
//		}
//		else 
//		{
//			Debug.Log("ClickLeft");
//			uint sid=(uint)(nowChoseed.GetComponent<LevelCell3>().m_id);
//			if(sid==0)
//				return ;
//			m_table.GetComponent<UICenterOnChild>().CenterOn(m_listLevel[(int)sid-1].transform);
//		}
	}
	bool forward=false;
	/// <summary>
	/// 右移动
	/// </summary>
	public void ClickLeftButton()
	{
		TimeCountEnd=RealTime.time;
		if(TimeCountEnd-TimeCountBegin>MinTimeInterval)
		{
			TimeCountBegin=TimeCountEnd;
		}
		else 
		{
			return ;
		}

		if(m_LevelPrepaerPanelState==LevelPrepaerPanelState.LevelPrepaerPanel_ChosedChapter)
		{
			if(NowChapterIndex<1)
				return ;
			NowChapterIndex-=1;
		//	RefreshChapterNewCell();
			forward=true;
			m_PlayChapterTween.Play(true);
		}
//		if(NEWCELLTEMP)
//		{
////			Debug.Log("ClickRight");
//			if(NowLevel<1)
//				return ;
//
//			NowLevel-=1;
//
//			RefreshNewCell();
//			m_PlayTween.Play(false);
//		}
//		else 
//		{
//			uint sid=(uint)(nowChoseed.GetComponent<LevelCell3>().m_id);
//			if(sid==m_listLevel.Count-1)
//				return ;
//			m_table.GetComponent<UICenterOnChild>().CenterOn(m_listLevel[(int)sid+1].transform);
//		}
	}

	/// <summary>
	/// 
	/// </summary>
	public void MovwFInsih()
	{
		for(int i=0,max=m_listLevelCell2.Count;i<max;i++)
		{
			m_listLevelCell2[i].transform.localPosition=m_listLevelCell2[i].GetComponent<TweenTransform>().from.transform.localPosition;
			m_listLevelCell2[i].transform.localScale=m_listLevelCell2[i].GetComponent<TweenTransform>().from.transform.localScale;
		}
		RefreshNewCell();
	}
	public void ChapterMoveFinish()
	{
		for(int i=0,max=m_listChapterCell.Count;i<max;i++)
		{
			m_listChapterCell[i].transform.localPosition=m_listChapterCell[i].GetComponent<TweenTransform>().from.transform.localPosition;
			m_listChapterCell[i].transform.localScale=m_listChapterCell[i].GetComponent<TweenTransform>().from.transform.localScale;
		}
//		if(forward)
//		{
//			forward=false;
//			RefreshChapterNewCell();
//		}
//		//
		RefreshChapterNewCell();
	}
	/// <summary>
	/// Moves the left finish.
	/// </summary>
//	public void MoveLeftFinish()
//	{
//
//		Debug.Log("移动结束");
//		//uimanage.GetNowLevel+=1;
//		if(m_LevelPrepaerPanelState==LevelPrepaerPanelState.LevelPrepaerPanel_ChosedChapter)
//		{
//			Debug.Log("移动结束");
//		}
//		else 
//		{
//			RefreshNewCell();
//		}
//
//	}

	//public  GameObject m_ChoseLevel;
	public GameObject m_ChoseChapter;
	/// <summary>
	/// 点击选择章节
	/// </summary>
	public void CLickTestChoseChapter()
	{
		
		m_LevelChangeChapter.SetActive(true);
		m_ChoseChapter.SetActive(false);
		m_LevelPrepaerPanelState=LevelPrepaerPanelState.LevelPrepaerPanel_ReSetLevel;
		NowLevel=0;
		RefreshNewCell();
		SetChapterDes();
		AudioClipDefine.AudioClipManager.Get().OpenUI();
	}
	/// <summary>
	/// 点击章节按钮
	/// </summary>
	public void ChoseChapterButton()
	{

		if(m_ChoseChapter.activeSelf)
		{
			m_LevelChangeChapter.SetActive(true);
			m_ChoseChapter.SetActive(false);
			m_LevelPrepaerPanelState=LevelPrepaerPanelState.LevelPrepaerPanel_DontReSetLevel;
			//RefreshNewCell();
			SetChapterDes();
	//		AudioClipDefine.AudioClipManager.Get().OpenUI();
		}
		else 
		{
			m_LevelChangeChapter.SetActive(false);
			m_ChoseChapter.SetActive(true);
			m_LevelPrepaerPanelState=LevelPrepaerPanelState.LevelPrepaerPanel_ChosedChapter;

			oldChapterIndex=NowChapterIndex;

			RefreshChapterNewCell();
			SetChapterDes();
		}

		//m_LevelPrepaerPanelState=LevelPrepaerPanelState.LevelPrepaerPanel_ReSetLevel;
		//NowLevel=0;
		//RefreshNewCell();
	}
	/// <summary>
	/// 每个界面自己的Back事件 Example：背包界面出售按钮显示Back
	/// </summary>
	public override void PanelClickBack()
	{
		if(m_LevelPrepaerPanelState==LevelPrepaerPanelState.LevelPrepaerPanel_ChosedChapter)
		{
			NowChapterIndex=oldChapterIndex;
			m_LevelChangeChapter.SetActive(true);
			m_ChoseChapter.SetActive(false);
			m_LevelPrepaerPanelState=LevelPrepaerPanelState.LevelPrepaerPanel_DontReSetLevel;	
			SetChapterDes();
			Debug.Log(">>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>重新播放关卡音乐");
		}
		else 
		{
			AudioClipDefine.AudioClipManager.Get().ExitUI();
			UIManageSystem.g_Instance.RomoveUI();
			UIManageSystem.g_Instance.ShowUI();
		}

	}
	#region xiaobo
	/// <summary>
	/// 关卡选择的东西
	/// </summary>
	public GameObject m_LevelChangeChapter;

	#endregion
}
