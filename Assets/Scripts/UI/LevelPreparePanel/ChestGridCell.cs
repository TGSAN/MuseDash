//EDIT: SF 宝箱栏位

using UnityEngine;
using System.Collections;
using FormulaBase;

public enum ChestGirdState
{
	CHESTGRID_EMPTY,		//空的
	CHESTGRID_SOMETHING,	//有东西的
	CHESTGRID_LOCKED,		//锁定的
	CHSETGRID_FINISHED		//倒计时完成
}
public class ChestGridCell : MonoBehaviour {
	public static string BroadcastCutDownTime="BroadcastCutDownTime";

	GameObject Empty;		//箱子栏为空
	GameObject SomeThing;	//箱子栏有东西
	GameObject Locked;		//箱子栏锁定
	ChestGirdState m_ChestState;	//当前状态栏的状态
	FormulaHost m_host;
	public int Index;


	 TweenPosition m_ChestMove;//宝箱图片的移动

	//有东西的状态1
	public UILabel m_QueueLabel;		//在队列的文字
	//public UILabel m_FinishLabel;		//完成的文字

	public UILabel m_RemindTime;		//剩余的时间
	public UISprite m_RemindTimeSprite; //剩余时间的图标
	public UILabel m_UseDamond;			//一次开启的需要的钻石数
	public UISprite m_LockedDamondSprite;		//锁定的钻石的图标
//	public UISprite m_TimeSprite;		//时间图标


	public GameObject m_SomeThingFinish;
	public GameObject m_SomeThing;
	//锁定的状态
	public  UILabel m_OpenLockedMoney;			//解锁宝箱栏
	public GameObject Scanning;
	public GameObject m_ScanningBox;

	Show3DCube ShowCube=null;


	public Show3DCube m_ShowCube
	{
		get
		{
			if(ShowCube==null)
			{
				ShowCube=GameObject.Find("3D UI/3D UI Camera"+Index).GetComponent<Show3DCube>();
			}
			return ShowCube;
		}
	}	//显示3D Cube

	public UILabel m_testLabel;			//测试Label

	void Awake()
	{
		InitObjet();
	}
	/// <summary>
	/// 初始化物品
	/// </summary>
	void InitObjet()
	{
		if(Empty==null)
		{
		Empty=transform.FindChild("Empty").gameObject;
		SomeThing=transform.FindChild("Something").gameObject;
		Locked=transform.FindChild("Locked").gameObject;
		}
	}
	/// <summary>
	/// 设置宝箱栏位为空
	/// </summary>
	public void SetEmpty()
	{
		m_ChestState=ChestGirdState.CHESTGRID_EMPTY;
		if(Empty==null)
		{
			InitObjet();
		}
		SomeThing.SetActive(false);
		Empty.SetActive(true);
		Locked.SetActive(false);
		m_ShowCube.ShowCube(-1);
//		if(m_ChestMove==null)
//		{
//			m_ChestMove=transform.FindChild("Something/Chest").gameObject.GetComponent<TweenPosition>();
//		}
//		m_ChestMove.ResetToBeginning();
		if(Index==0)
		{
			ShowFirstChestGird(false);

		}
	}

	int  QualityChest=1;
	/// <summary>
	/// 设置宝箱栏位有什么东西
	/// </summary>
	/// <param name="_host">Host.</param>
	public void SetSomething(FormulaHost _host)
	{
//		Transform tem=transform.FindChild("Something/Chest").transform;
//		tem.localPosition=tem.gameObject.GetComponent<TweenPosition>().from;
		m_host=_host;
		m_ChestState=ChestGirdState.CHESTGRID_SOMETHING;
	//	ChestManageComponent.Instance.GetChestAllTime(_host);
		m_host.Result(FormulaKeys.FORMULA_90);
		QualityChest=(int)m_host.GetDynamicDataByKey(SignKeys.QUALITY);
		//m_QueueLabel.gameObject.SetActive(true);
		//m_QueueLabel.text=Index.ToString()+"Queue:"+m_host.GetDynamicIntByKey(SignKeys.CHESTQUEUE);
		if(Index==0)//一号栏位
		{
			m_ShowCube.ShowCube((int)m_host.GetDynamicDataByKey(SignKeys.QUALITY));
			m_QueueLabel.gameObject.SetActive(false);
			if(m_host.GetDynamicIntByKey(SignKeys.CHESTREMAINING_TIME)==0)
			{
				ShowFirstChestGird(false);
			}
			else 
			{
				ShowFirstChestGird(true);
			}
			string tempTime=ChestManageComponent.Instance.GetChestAllTime(m_host);

			m_testLabel.text=tempTime;
			if(tempTime.Length<=0)//宝箱倒计时结束
			{
				Debug.Log("宝箱倒计时有问题");
				m_SomeThingFinish.gameObject.SetActive(true);
				m_ChestState=ChestGirdState.CHSETGRID_FINISHED;
				m_SomeThing.SetActive(false);
				m_ShowCube.ShowLocked(QualityChest);
			}
			else 
			{
				SetRemindTime();
				m_SomeThingFinish.gameObject.SetActive(false);
				m_SomeThing.SetActive(true);
				m_ShowCube.ShowCube(QualityChest);
			}
			m_QueueLabel.gameObject.SetActive(false);
		}
		else //其他栏位
		{

			if(ChestManageComponent.Instance.GetChestAllTime(m_host).Length==0)
			{
				m_QueueLabel.gameObject.SetActive(false);	
				m_SomeThingFinish.gameObject.SetActive(true);
				m_ShowCube.ShowLocked(QualityChest);
			}
			else 
			{
				m_SomeThingFinish.gameObject.SetActive(false);
				//m_QueueLabel.gameObject.SetActive(true);
			//	m_QueueLabel.text=Index.ToString()+"Queue:"+m_host.GetDynamicIntByKey(SignKeys.CHESTQUEUE);
				m_ShowCube.ShowCube(QualityChest);
			}

		}
		if(Empty==null)
		{
			InitObjet();
		}
		Debug.LogWarning("QualityChest:"+QualityChest);
		SomeThing.SetActive(true);
		Empty.SetActive(false);
		Locked.SetActive(false);
	}
	/// <summary>
	/// Sets the locked.
	/// </summary>
	/// <param name="_showMoney">是否显示消耗的钻石.</param>
	public void SetLocked(bool _showMoney)
	{
		m_ChestState=ChestGirdState.CHESTGRID_LOCKED;
		if(Empty==null)
		{
			InitObjet();
		}
		SomeThing.SetActive(false);
		Empty.SetActive(false);
		m_ShowCube.ShowCube(-1);
		if(_showMoney)
		{
			Locked.SetActive(true);
			m_OpenLockedMoney.text=((int)SundryManageComponent.Instance.GetVaule(Index+5)).ToString();
			m_LockedDamondSprite.gameObject.SetActive(true);
		}
		else 
		{
			Locked.SetActive(false);
			m_OpenLockedMoney.text="";
			m_LockedDamondSprite.gameObject.SetActive(false);
		}
	}

	// Use this for initialization
	void Start () {
	
	}
	// Update is called once per frame
	void Update () {
		if(Index==0&&m_host!=null)
			m_testLabel.text=m_host.GetRealTimeCountDownNow().ToString();
	}

	/// <summary>
	/// 点击宝箱栏位
	/// </summary>
	public void ClickButton()
	{
		switch(m_ChestState)
		{
		case ChestGirdState.CHESTGRID_EMPTY:
			ShowEmptyBox();
			break;
		case ChestGirdState.CHESTGRID_LOCKED:
			ShowLockBox();
			break;
		case ChestGirdState.CHESTGRID_SOMETHING:
			ShowSomeThingBox();
			break;
		case ChestGirdState.CHSETGRID_FINISHED:
			ShowSomeThingBox();
			break;
		}
	}

	public void MonveFinish()
	{
//		Debug.Log("MonveFinish");
//		CommonPanel.GetInstance().ShowRewardCubePanel((int)m_host.GetDynamicDataByKey(SignKeys.QUALITY),ShowGetReward);
//		//m_ShowCube.PlayCubeInAniMation((int)m_host.GetDynamicDataByKey(SignKeys.QUALITY),ShowGetReward);
	}

	/// <summary>
	/// 点击后显示获取的奖励
	/// </summary>
	public void ShowGetReward()
	{
		/*
		Debug.Log("ShowGetReward");
		CommonPanel.GetInstance().m_ChestShow.m_RewardPanel.ShowGetItem(m_host);
		*/
	}

	/// <summary>
	/// 点击后先是为空
	/// </summary>
	void ShowEmptyBox()
	{
		CommonPanel.GetInstance().ShowText("Chest is empty");
//		Debug.Log("Click CHestButton");
//		//显示背包的宝箱页
//		bagPanel2 BagPanel=GameObject.Find("UI Root").gameObject.transform.FindChild("BagPanel").GetComponent<bagPanel2>();
//		UIPerfabsManage.g_Instan.GetUIState=UIPerfabsManage.GameUIState.BAG_CHESTFIELDTOBAG;
//		BagPanel.ShowTypeBag(UIPerfabsManage.g_Instan.GetUIState);
	//	UIManageSystem.g_Instance.AddUI(UIManageSystem.UIBAGPANEL,(int)BagPanel2State.BagPanel2_ShowCube*10);

	}
	/// <summary>
	/// 点击后显示为锁定栏位
	/// </summary>
	void ShowLockBox()
	{
		int count=AccountManagerComponent.Instance.GetChestGirdNumber();
		int MoneyNumber=(int)SundryManageComponent.Instance.GetVaule(5+count);
		if(Index>count)
			return ;
		CommonPanel.GetInstance().ShowUseResBox("花费钻石扩充",MoneyNumber,1,ClickOkutton);
	}
	/// <summary>
	/// 解锁点击了OK
	/// </summary>
	public void ClickOkutton() {
		int count = AccountManagerComponent.Instance.GetChestGirdNumber ();
		int MoneyNumber = (int)SundryManageComponent.Instance.GetVaule (5 + count);
		Debug.Log ("点击了 扩充OK按钮 cost : " + MoneyNumber);

		bool _result = AccountCrystalManagerComponent.Instance.ChangeDiamond (-MoneyNumber, true, new HttpResponseDelegate (((bool result) => {
			if (!result) {
				CommonPanel.GetInstance ().ShowTextLackDiamond ();
				return;
			}

			AccountManagerComponent.Instance.SetChestGirdNumber (1, new HttpResponseDelegate (ClickOKButtonCallBack));
			ChestManageComponent.Instance.AddGridMoveChest ();
			ChestManageComponent.Instance.ChestBagToGrid ();
			Messenger.Broadcast<int> (LevelPrepaerPanel.BraodCast_ChestMissAni, 10);
		})));

		if (!_result) {
			CommonPanel.GetInstance ().ShowTextLackDiamond ();
		}
	}

	/// <summary>
	/// 解锁点击OK的回调
	/// </summary>
	/// <param name="_success">If set to <c>true</c> success.</param>
	void ClickOKButtonCallBack(bool _success) {
		Debug.Log ("回调函数");
		if (_success) {
			CommonPanel.GetInstance ().ShowWaittingPanel (false);
			//s	Messenger.Broadcast(LevelPrepaerPanel.BraodCast_RestChestEmpty);
		} else {
			CommonPanel.GetInstance ().ShowText ("connect is fail");
		}
	}

	/// <summary>
	/// 点击了 有宝箱的栏位
	/// </summary>
	void ShowSomeThingBox()
	{
		LevelPrepaerPanel.g_ClickChestIndex=Index;
		AudioClipDefine.AudioClipManager.Get().SetAudioVolme();
		if(m_host.GetDynamicIntByKey(SignKeys.CHESTREMAINING_TIME)==0)
		{
			Debug.Log("点击了完成宝箱");
			CommonPanel.GetInstance().ShowRewardCubePanel((int)m_host.GetDynamicDataByKey(SignKeys.QUALITY),null,m_host);
		//	CommonPanel.GetInstance().SetBlurSub(this.GetComponent<UIPanel>());
		}
		else 
		{
		//	UIPerfabsManage.g_Instan.GetUIState=UIPerfabsManage.GameUIState.BAG_CHESTFIELDTOBAG;
			CommonPanel.GetInstance().ShowChestEquip(m_host,true,Index+1);
		}
	}

	void OnEnable()
	{
		Messenger.MarkAsPermanent(BroadcastCutDownTime);
		Messenger.AddListener(BroadcastCutDownTime,ChangeChestTime);
	}

	/// <summary>
	/// 设置总的剩余时间
	/// </summary>
	void SetRemindTime()
	{


		FormulaHost thost=ChestManageComponent.Instance.GetQueue1();
		if(thost!=null)
		{
			string tempTime=ChestManageComponent.Instance.GetChestAllTime(thost);
			if(tempTime.Length<=0)
			{

				if(LevelPrepaerPanel.g_ClickChestIndex==0)
				{
					CommonPanel.GetInstance().HidEnSureSalePanel();
				}
				//AnimationEventPlay1005.DestroyObject;
				//Messenger.Broadcast(LevelPrepaerPanel.BraodCast_ChestMissAni);
				MoveChestToEnd();
			}
			else 
			{
				m_UseDamond.gameObject.SetActive(true);	
				m_RemindTime.gameObject.SetActive(true);
				m_UseDamond.text=ChestManageComponent.Instance.GetOpenChestMoney(thost).ToString();
				m_RemindTimeSprite.fillAmount=ChestManageComponent.Instance.GetTimePercentage(thost);

			}

			m_RemindTime.text=tempTime;
		}
		else 
		{
		//	Debug.Log("没有宝箱在开启");
		}
	
	}

	/// <summary>
	/// 移动宝箱到末尾
	/// </summary>
	public void MoveChestToEnd()
	{
		FormulaHost thost=ChestManageComponent.Instance.GetQueue1();
		if(!ChestManageComponent.Instance.MoveFinishedToEnd(thost));
		{
			//关闭流光 关闭 外框 设置总时间  关闭当前时间
			//Scanning.SetActive(false);
			ShowFirstChestGird(false);
		//	SomeThing.SetActive(false);	//箱子栏有东西
			m_UseDamond.gameObject.SetActive(false);	
			m_RemindTime.gameObject.SetActive(false);
		}
	}
//	bool tttt=true;
	/// <summary>
	/// 改变宝箱倒计时
	/// </summary>
	void ChangeChestTime()
	{
		if(m_host!=null&&Index==0)
		{

			
//			Debug.Log("Change Time");
//			int temp=m_host.GetDynamicDataByKey(SignKeys.CHESTREMAINING_TIME);
//			temp--;
//			m_host.SetDynamicData(SignKeys.CHESTREMAINING_TIME,temp);
			SetRemindTime();

		}
		else 
		{
			return ;
		}
	}
	void OnDisable()
	{
		Messenger.RemoveListener(BroadcastCutDownTime,ChangeChestTime);
	}
	/// <summary>
	/// 一号栏位的流光 和一号栏位的外边框
	/// </summary>
	/// <param name="_show">If set to <c>true</c> show.</param>
	void ShowFirstChestGird(bool _show=true)
	{
		Scanning.SetActive(_show);
		m_ScanningBox.SetActive(_show);

	}

//
//	void OnApplicationQuit()//退出保存 宝箱时间
//	{
//		if(m_host!=null&&Index==0)
//		{
//			m_host.Save();
//		}
//	}
}
