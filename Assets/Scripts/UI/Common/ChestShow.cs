using FormulaBase;
using System.Collections;
using UnityEngine;

public class ChestShow : MonoBehaviour
{
    //宝箱状态 此处链表只存放 在开启栏的宝箱
    //1.宝箱正在开启
    //2.宝箱在队列中
    //3.宝箱可以开启
    public UILabel m_ChestName;         //宝箱名字

    public UILabel m_RewardMoney;       //宝箱获取的钱
    public UILabel m_Grading;           //宝箱分级描述
    public UILabel m_ChestLeftTime;     //宝箱队列的时间
    public UILabel m_CostMoney;         //开启宝箱需要的钱
    public UILabel m_RightButton;       //这JB 功能太多 不知道怎么命名	框中右下角的文字
    public UISprite m_RightMoneyIcon;   //按钮右边的图标
    public GameObject ExitMask;
    public UILabel m_Temp;
    public UIButton LeftButton;
    public UIButton RightButton;
    public GameObject tempObject;       //临时的动画位置
    public Show3DCube m_Show3DCube;     //显示3D的Cube

    public Animator m_ShowChestAnimator;    //显示面板的动画
    private FormulaHost m_host;

    //FormulaHost m_host2;
    private void OnEnable()
    {
        //CommonPanel.GetInstance().SetBlurSub(this.GetComponent<UIPanel>());

        Debug.LogWarning(this.gameObject.name);
        m_ShowChestAnimator.Play("Ge_UI_small_start");

        //Messenger.MarkAsPermanent(ChestGridCell.BroadcastCutDownTime);
        //Messenger.AddListener(ChestGridCell.BroadcastCutDownTime,SetChestRemindTime);
        //	StartCoroutine("PlayInAniMation");
        //BUG ....会掉帧
    }

    public void SetChestRemindTime()
    {
        if (m_host.GetDynamicIntByKey(SignKeys.CHESTREMAINING_TIME) == 0)
        {
            ClickExitMask(null);
        }
        else
        {
            m_ChestLeftTime.text = ChestManageComponent.Instance.GetChestAllTime(m_host);
        }
    }

    #region 临时解决方案

    private IEnumerator PlayInAniMation()
    {
        yield return 0;
        m_ShowChestAnimator.Play("Ge_UI_small_start");
    }

    #endregion 临时解决方案

    private void OnDisable()
    {
        //Messenger.RemoveListener (ChestGridCell.BroadcastCutDownTime, SetChestRemindTime);
    }

    // Use this for initialization
    private void Start()
    {
        //UIEventListener.Get(ExitMask).onClick=ClickExitMask;
    }

    public void CloseObject()
    {
        if (m_CloseBlur)
        {
            //CommonPanel.GetInstance().CloseBlur(null);
        }
        this.gameObject.SetActive(false);
    }

    public void ClickOtherPlease()
    {
        Debug.Log("ClickOtherPlease");
        //AudioClipDefine.AudioClipManager.Get().SetAudioVolme(1);
        ClickExitMask(null, true);
    }

    private bool m_CloseBlur = true;

    public void ClickExitMask(GameObject _ob, bool _CloseBlur = true)
    {
        //AudioClipDefine.AudioClipManager.Get().SetAudioVolme();
        m_CloseBlur = _CloseBlur;
        m_ShowChestAnimator.Play("Ge_UI_small_out");

        //this.gameObject.SetActive(false);
    }

    // Update is called once per frame
    //void Update () {
    //}

    public void SetData(FormulaBase.FormulaHost _host, bool _Queue = false, int _QueueIndex = 0, GameObject _ob = null)
    {
        Transform tem = transform.FindChild("Ge_UI_small_anim/ChestShow/Chest").transform;
        tem.localPosition = tem.gameObject.GetComponent<TweenPosition>().from;

        //TweenPosition tem=transform.FindChild("Chest").gameObject.GetComponent<TweenPosition>();

        tempObject = _ob;
        m_host = _host;
        //m_host2=_host;

        this.gameObject.SetActive(true);
        m_ChestName.text = (string)_host.GetDynamicStrByKey(SignKeys.NAME);
        //m_Temp.text=ChestManageComponent.Instance.GetChestAllTime(_host)+"\n"+_host.GetDynamicDataByKey(SignKeys.CHESTREMAINING_TIME);
        //设置空闲那句话
        //m_RightMoneyIcon.gameObject.SetActive(false);
        //在队列中进入
        m_ChestLeftTime.text = ChestManageComponent.Instance.GetChestAllTime(m_host);
        //	m_RightButton.text="Sale";
        //RightButton.onClick.Add(new EventDelegate( SaleChestInGridCell));
        if (_Queue)
        {
            if (_QueueIndex == 1)
            {//队列一
                RightButton.onClick.Clear();
                RightButton.onClick.Add(new EventDelegate(SaleChestInGridCell));
                m_Show3DCube.ShowCube((int)m_host.GetDynamicDataByKey(SignKeys.QUALITY));
            }
            else
            { //队列后续
                RightButton.onClick.Clear();
                RightButton.onClick.Add(new EventDelegate(SaleChestInGridCell));
                m_Show3DCube.ShowCube((int)m_host.GetDynamicDataByKey(SignKeys.QUALITY));
            }
        }
        else
        {
            m_RightButton.text = "Sale";
            m_RightMoneyIcon.gameObject.SetActive(false);
            RightButton.onClick.Clear();
            RightButton.gameObject.layer = 17;
            RightButton.onClick.Add(new EventDelegate(SaleChest));
            //			if(ChestManageComponent.Instance.GetChestList.Count==0)//非队列 队列空
            //			{
            //				RightButton.onClick.Clear();
            //				RightButton.onClick.Add(new EventDelegate(SaleChest));
            //			}
            //			else if(AccountManagerComponent.Instance.GetChestGirdNumber()>ChestManageComponent.Instance.GetChestList.Count)//有空位
            //			{
            //			//	m_ChestLeftTime.text="另一个宝箱正在解锁。。。";
            //				m_RightButton.text="Sale";
            //				RightButton.onClick.Clear();
            //				RightButton.onClick.Add(new EventDelegate( SaleChest));
            //
            //			}
            //			else if(AccountManagerComponent.Instance.GetChestGirdNumber()<6)	//没空位  满位小于最大位
            //			{
            //			//	m_ChestLeftTime.text="[FB1E94FF]栏位已满![-]";
            //
            //				int count=AccountManagerComponent.Instance.GetChestGirdNumber();
            //				int MoneyNumber=(int)SundryManageComponent.Instance.GetVaule(5+count);
            //
            //			//	m_RightButton.text="ADD FIELD\n"+MoneyNumber.ToString();
            //				m_RightButton.text="Sale";
            //				m_RightMoneyIcon.gameObject.SetActive(false);
            //				RightButton.onClick.Clear();
            //				RightButton.onClick.Add(new EventDelegate( SaleChest));
            //
            //			}
            //			else  if(AccountManagerComponent.Instance.GetChestGirdNumber()==6)
            //			{
            //			//	m_ChestLeftTime.text="[FB1E94FF]栏位已满![-]";
            //				m_RightButton.text="Sale";
            //				m_RightMoneyIcon.gameObject.SetActive(false);
            //				RightButton.onClick.Clear();
            //				RightButton.onClick.Add(new EventDelegate( SaleChest));
            ////				m_RightButton.text="Busy";
            ////				RightButton.onClick.Clear();
            //			}
            m_Show3DCube.ShowCube((int)m_host.GetDynamicDataByKey(SignKeys.QUALITY));
        }
        //剩余时间换金钱
        m_CostMoney.text = ChestManageComponent.Instance.GetOpenChestMoney(m_host).ToString();
        //m_Show3DCube.ShowCube((int)m_host.GetDynamicDataByKey(SignKeys.QUALITY));
        Debug.Log("设置宝箱的信息");
    }

    /// <summary>
    /// 花钱购买物品栏
    /// </summary>
    public void ClicikUseMoneyBuyGrid()
    {
        //		Debug.Log("Use Money");
        //
        //		int count=AccountManagerComponent.Instance.GetChestGirdNumber();
        //
        //		int MoneyNumber=(int)SundryManageComponent.Instance.GetVaule(5+count);
        //
        //		if(AccountManagerComponent.Instance.ChangeCrystal(-MoneyNumber))
        //		{
        //			CommonPanel.GetInstance().ShowWaittingPanel(true);
        //			AccountManagerComponent.Instance.SetChestGirdNumber(1,new HttpResponseDelegate(ClicikUseMoneyBuyGridCallBack));
        //		}
        //		else
        //		{
        //			Debug.Log("Money is not Enough");
        //		}
    }

    private void ClicikUseMoneyBuyGridCallBack(bool _success)
    {
        Debug.Log("使用钱的反馈");
        //ChestManageComponent.Instance

        //	this.gameObject.SetActive(false);
        m_host.SetDynamicData(SignKeys.CHESTQUEUE, ChestManageComponent.Instance.GetOwnedChestNumber() + 1);
        ChestManageComponent.Instance.GetChestList.Add(m_host);
        ItemManageComponent.Instance.GetChestList.Remove(m_host);
        m_host.Save(new HttpResponseDelegate(ClickCostMoneyBuyGridCallBack));
    }

    private void ClickCostMoneyBuyGridCallBack(bool _success)
    {
        //CommonPanel.GetInstance().CloseBlur(this.gameObject);
        ClickExitMask(null);
        if (_success)
        {
            //Messenger.Broadcast(LevelPrepaerPanel.BraodCast_RestChestEmpty);
            Messenger.Broadcast(bagPanel2.BroadcastBagItem);
            //Messenger.Broadcast (MainMenuPanel.BroadcastChangeDiamond);
            Debug.Log("删除UI");
            UIManageSystem.g_Instance.RomoveUI();
            CommonPanel.GetInstance().ShowWaittingPanel(false);
            //	Messenger.Broadcast(bagPanel2.BroadcastBagItem);
        }
        else
        {
            CommonPanel.GetInstance().ShowText("connect is fail");
        }
    }

    public void SaleChest()
    {
        ClickExitMask(null, false);
        //		m_host.Result (FormulaKeys.FORMULA_90);
        //CommonPanel.GetInstance().ShowEnSureSalePanel(m_host.GetDynamicIntByKey(SignKeys.SOLD), EnSureSaleChest, CancelSaleChest, m_host.GetDynamicIntByKey(SignKeys.QUALITY) > 1, true);
    }

    public void EnSureSaleChest()
    {
        Debug.Log("卖出宝箱");
        //CommonPanel.GetInstance().CloseBlur(null);
        this.gameObject.SetActive(false);
        //	ClickExitMask(this.gameObject);
        ItemManageComponent.Instance.SaleItem(m_host, SaleChestCallBack);
    }

    public void CancelSaleChest()
    {
        /*
		CommonPanel.GetInstance().CloseBlur(null);
		CommonPanel.GetInstance().m_EnSureSalePanel.gameObject.SetActive(false);
		this.gameObject.SetActive(false);
		//ClickExitMask(null);
		*/
    }

    public void SaleChestCallBack(bool _success)
    {
        CommonPanel.GetInstance().ShowWaittingPanel(false);
        if (true)
        {
            //	UIManageSystem.g_Instance.RomoveUI();
            //刷新背包
            Messenger.Broadcast(bagPanel2.BroadcastBagItem);
            //Messenger.Broadcast (MainMenuPanel.BroadcastChangeMoney);
            Messenger.Broadcast(bagPanel2.BroadcastBagUpSize);
        }
        else
        {
            NGUIDebug.Log("connet is fail");
        }
    }

    public void SaleChestInGridCell()
    {
        Debug.Log("Sale Chest");

        ClickExitMask(null, false);
        //		m_host.Result (FormulaKeys.FORMULA_90);
        //CommonPanel.GetInstance().ShowEnSureSalePanel(m_host.GetDynamicIntByKey(SignKeys.SOLD), EntureSaleChestInGrid, CancelSaleChestInGrid, m_host.GetDynamicIntByKey(SignKeys.QUALITY) > 1, true);
    }

    public void EntureSaleChestInGrid()
    {
        /*
		ChestManageComponent.Instance.SaleChestInGird(m_host);
		CommonPanel.GetInstance().m_EnSureSalePanel.gameObject.SetActive(false);
		CommonPanel.GetInstance().CloseBlur(null);
		this.gameObject.SetActive(false);
		//ClickExitMask(null);
*/
    }

    public void CancelSaleChestInGrid()
    {
        /*
		CommonPanel.GetInstance().CloseBlur(null);
		CommonPanel.GetInstance().m_EnSureSalePanel.gameObject.SetActive(false);
		this.gameObject.SetActive(false);
		//ClickExitMask(null);
		*/
    }

    public void EntureSaleChestInGridCell()
    {
    }

    /// <summary>
    /// 点击替换按钮
    /// </summary>
    //	public void ReplaceButton()
    //	{
    //		Debug.Log("点击替换按钮");
    //		//UIPerfabsManage.g_Instan.NowChoseChesGrid=m_host;
    //		//显示背包的宝箱页
    //		//bagPanel2 BagPanel=GameObject.Find("UI Root").gameObject.transform.FindChild("BagPanel").GetComponent<bagPanel2>();
    //	//	UIPerfabsManage.g_Instan.GetUIState=UIPerfabsManage.GameUIState.BAG_CHESTFIELDREPLACETOBAG;
    //		//BagPanel.ShowTypeBag(UIPerfabsManage.g_Instan.GetUIState);
    //		CommonPanel.GetInstance().CloseBlur(this.gameObject);
    //		ClickExitMask(null);
    //		UIManageSystem.g_Instance.AddUI(UIManageSystem.UIBAGPANEL,(int)BagPanel2State.BagPanel2_ShowCube*10+(int)BagPanelState2.BagPanelState2_Replace,m_host);
    //		//this.gameObject.SetActive(false);
    //	}
    //	public void MoveToQueue()
    //	{
    //		CommonPanel.GetInstance().CloseBlur(this.gameObject);
    //		ClickExitMask(null);
    //		m_host.SetDynamicData(SignKeys.CHESTQUEUE,ChestManageComponent.Instance.GetOwnedChestNumber()+1);
    //		if(ChestManageComponent.Instance.GetOwnedChestNumber()==0)
    //		{
    //			m_host.SetRealTimeCountDown((int)(m_host.Result(FormulaKeys.FORMULA_94)));
    //		}
    //		ChestManageComponent.Instance.GetChestList.Add(m_host);
    //		ItemManageComponent.Instance.GetChestList.Remove(m_host);
    //		m_host.Save(new HttpResponseDelegate(ChestMoveCallBack));
    //
    //	}
    public void ChestMoveCallBack(bool _success)
    {
        if (_success)
        {
            Messenger.Broadcast(bagPanel2.BroadcastBagItem);
            //CommonPanel.GetInstance().CloseBlur(this.gameObject);
            //CommonPanel.GetInstance().CloseBlur(null);
            Debug.Log("删除UI");
            UIManageSystem.g_Instance.RomoveUI();
        }
        else
        {
            CommonPanel.GetInstance().ShowText("Connect is fail");
        }
    }

    public RewardPanel m_RewardPanel;

    public void CLickOPenNowButton()
    {
        //int money2=(int)ChestManageComponent.Instance.GetOpenChestMoney(m_host2);
        int money = (int)ChestManageComponent.Instance.GetOpenChestMoney(m_host);
        //=_host;
        bool _result = AccountCrystalManagerComponent.Instance.ChangeCrystal(-money, true, new HttpResponseDelegate(((bool result) =>
        {
            if (!result)
            {
                CommonPanel.GetInstance().ShowText("钻石不足 请充值");
                return;
            }
            this.gameObject.SetActive(false);
        })));
    }

    public void MonveFInish()
    {
        //.//m_Show3DCube.PlayCubeInAniMation((int)m_host.GetDynamicDataByKey(SignKeys.QUALITY),ShowItem);
        //StartCoroutine("ShowItem");
    }

    public void ShowItem()
    {
        m_RewardPanel.ShowGetItem(m_host);
        this.gameObject.SetActive(false);
    }

    #region 动画

    //public void  Playe

    #endregion 动画
}