using FormulaBase;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//public enum BagState
//{
//	BAG_SHOW,//普通显示背包
//	BAG_ROLELVSHOW//角色升级背包
//}
public enum BagStyle
{
    BAG_SHOWALL,        //所有道具
    BAG_SHOWEQUIP,      //Equip装备页：武器-衣服-饰品放置处
    BAG_SHOWSTRFF,      //（2）material素材页：装备升级、升星的相关吞噬素材，人物升级、升星的相关吞噬材料
    BAG_SHOWSERVAMT,    //（4）Pet宠物页：宠物放置处
    BAG_SHOWCUBE        //（3）Cube宝箱页：宝箱放置处。在背包点击宝箱弹出宝箱信息弹窗
}

public enum SortItemStyel
{
    ITEM_TYPESORT = 1,      //类型排序
    ITEM_TIMESORT = 2,      //时间排序
    ITEM_QUALITY = 3            //品质排序
}

public enum EquipShowStyel
{
    EQUIPSHOW_EQUIP = 1,        //zhuangbei
    EQUIPSHOW_DISCHARGE = 2,      //xiezai
    EQUIPSHOW_EQUIPSALE = 3,      //chushou
    EQUIPSHOW_UPGRADE,
    EQUIPSHOW_EVOLUTION,
    NONE = 4
}

public enum BagPanel2State
{
    BagPanel2_ShowAll = 1,                  //显示所有物品
    BagPanel2_ShowWeapon = 2,                   //显示武器
    BagPanel2_ShowChip = 3,                 //显示饰品
    BagPanel2_SHowPet = 4,                  //显示宠物
    BagPanel2_ShowFood = 5,                 //显示食物
    BagPanel2_ShowCube = 6,                 //显示宝箱
    BagPanel2_ShowEquip = 7					//显示装备
}

public enum BagPanelState2
{
    BagPanelState2_Normal = 0,              //普通状态
    BagPanelState2_Sale = 1,                    //出售状态
    BagPanelState2_Replace = 2,             //替换状态
    BagPanelState2_Chosed = 3,              //选择状态
    BagPanelState2_Equiped = 4				//装备物品
}

public enum BagPanelEquipPlace
{
    BagPanelEquipPlace_None,                //空的状态
    BagPanelEquipPlace_Pet1 = 11,               //宠物1
    BagPanelEquipPlace_Pet2 = 12,               //宠物2
    BagPanelEquipPlace_Pet3 = 13,               //宠物3
    BagPanelEquipPlace_Equip1 = 21,         //装备1
    BagPanelEquipPlace_Equip2 = 22,         //装备2
    BagPanelEquipPlace_Equip3 = 23,         //装备3
    BagPanelEquipPlace_Equip4 = 24			//装备4
}

public class bagPanel2 : UIPanelBase
{
    public UITable m_EquipTable;                                    //装备的Table
    public GameObject BagEquipCell;                                 //单个背包物品的实例
    public UILabel m_BagSize;
    public GameObject m_PointOb;                                    //指定类型时隐藏的物品
    public GameObject m_ChoseItemNumberShow;                        //选择个数的面板
    public UILabel ChosedLabel;                                     //选择的物品数量
    public static FormulaHost m_MysticalHost = null;

    ////因为从物品信息进入背包 需要删除显示的物品
    public List<UIToggle> m_listType = new List<UIToggle>();            //不同类型的Toggle

    public static string BroadcastBagChoseItem = "BAGHEROLVUP_CHANGECOST";
    public static string BroadcastBagUpSize = "BAGSIZEUP";          //背包容量改变
    public static string BroadcastBagEmptyReSet = "BAGEMPTYRESET";  //重新设置背包空格
    public static string BroadcastBagItem = "BAGERESETITEM";            //重新设置背包物品
    public static string BroadcastClearChose = "BAGCLEARCHOSEDITEM";    //重新清空背包

    public UILabel m_SortLabel;
    public SaleBoxPanel m_SaleBoxPanel;
    private BagStyle m_BagTable = BagStyle.BAG_SHOWALL;                     //背包显示类型
    private SortItemStyel m_ItemSort = SortItemStyel.ITEM_TIMESORT;         //排序方式
    private List<List<ushort>> m_ComposeList = new List<List<ushort>>();        //组合链表
    private List<GameObject> m_listbagEmpty = new List<GameObject>();           //背包空位

    private void Awake()
    {
    }

    //刷新背包 空格数
    public void RefreshBagGrid()
    {
        int count = BagManageComponent.Instance.GetBagSize();
        //int count=24;
        if (m_listbagEmpty.Count == count)
            return;
        Debug.LogWarning("背包空格数为：" + count);
        for (int i = m_listbagEmpty.Count; i < count; i++)
        {
            GameObject temp = Instantiate(BagEquipCell) as GameObject;
            temp.transform.parent = BagEquipCell.transform.parent.transform;
            temp.transform.localScale = BagEquipCell.transform.localScale;
            temp.transform.localPosition = new Vector3(0, 0, 0);
            temp.SetActive(true);
            m_listbagEmpty.Add(temp);
        }
        m_EquipTable.Reposition();
    }

    #region 点击Tab

    public void ClickAllTab(bool _value)
    {
        if (!_value)
            return;
        m_BagTable = BagStyle.BAG_SHOWALL;
        SortItem();
        LoadPos();
    }

    public void ClickEquip(bool _value)
    {
        if (!_value)
            return;
        m_BagTable = BagStyle.BAG_SHOWEQUIP;
        SortItem(); LoadPos();
    }

    public void ClickStrff(bool _value)
    {
        if (!_value)
            return;
        m_BagTable = BagStyle.BAG_SHOWSTRFF;
        SortItem(); LoadPos();
    }

    public void ClickServant(bool _value)
    {
        if (!_value)
            return;
        m_BagTable = BagStyle.BAG_SHOWSERVAMT;
        SortItem(); LoadPos();
    }

    public void ClickCube(bool _value)
    {
        if (!_value)
            return;
        m_BagTable = BagStyle.BAG_SHOWCUBE;
        SortItem(); LoadPos();
    }

    #endregion 点击Tab

    /// <summary>
    /// 色泽背包物品信息
    /// </summary>
    /// <param name="temp">Temp.</param>
    private void SetEmpty(List<FormulaHost> temp)
    {
        if (m_BagPanel2State == BagPanel2State.BagPanel2_ShowAll)
        {
            for (int i = 0, max = BagManageComponent.Instance.GetBagSize(), allEq = temp.Count; i < max; i++)
            {
                if (m_listbagEmpty == null || i >= m_listbagEmpty.Count)
                {
                    continue;
                }

                if (!m_listbagEmpty[i].activeSelf)
                    m_listbagEmpty[i].SetActive(true);
                if (i < allEq)
                {
                    m_listbagEmpty[i].GetComponent<bagItemCell>().SetUI(temp[i]);
                }
                else
                {
                    m_listbagEmpty[i].GetComponent<bagItemCell>().SetUI(null);
                }
            }
        }
        else
        {
            for (int i = 0, j = 0, max = BagManageComponent.Instance.GetBagSize(), allEq = temp.Count; i < max; i++, j++)
            {
                if (j < allEq)
                {
                    if (m_MysticalHost != null)//当背包物品进入 升级界面
                    {
                        //	Debug.Log("m_IgnoreHost");
                        if (temp[j] == m_MysticalHost)
                        {
                            i--;
                            continue;
                        }
                    }
                    m_listbagEmpty[i].SetActive(true);
                    m_listbagEmpty[i].GetComponent<bagItemCell>().SetUI(temp[j]);
                }
                else
                {
                    m_listbagEmpty[i].SetActive(false);
                    //m_listbagEmpty[i].GetComponent<bagItemCell>().SetUI(null);
                }
            }
        }
    }

    #region 点击排序按钮

    /// <summary>
    /// 重置Toggle
    /// </summary>
    public void ResetToggle()
    {
        for (int i = 0, max = m_listType.Count; i < max; i++)
        {
            if (i == 0)
            {
                m_listType[i].Set(true);
            }
            else
            {
                m_listType[i].Set(false);
            }
        }
    }

    /// <summary>
    /// 点击排序按钮
    /// </summary>
    public void ClickSortButton()
    {
        m_ItemSort++;
        if (m_ItemSort == (SortItemStyel)4)
        {
            m_ItemSort = (SortItemStyel)1;
        }
        SortItem();
    }

    private void SortItem()
    {
        switch (m_ItemSort)
        {
            case SortItemStyel.ITEM_TYPESORT:
                ClickType();
                m_SortLabel.text = "TYPE";
                break;

            case SortItemStyel.ITEM_TIMESORT:
                ClickTime();
                m_SortLabel.text = "TIME";
                break;

            case SortItemStyel.ITEM_QUALITY:
                ClickQuarlity();
                m_SortLabel.text = "QUARLITY";
                break;
        }
    }

    /// <summary>
    /// 点击时间排序
    /// </summary>
    public void ClickTime()
    {
        List<FormulaHost> All = new List<FormulaHost>();
        List<FormulaHost> temp = new List<FormulaHost>();
        if (m_BagPanel2State == BagPanel2State.BagPanel2_ShowChip)
        {
            All = ItemManageComponent.Instance.GetAppointItem(2);
            ItemManageComponent.Instance.SortTime(All);
            SetEmpty(All);
        }
        else if (m_BagPanel2State == BagPanel2State.BagPanel2_ShowCube)
        {
            ItemManageComponent.Instance.SortTime(ItemManageComponent.Instance.GetChestList);
            SetEmpty(ItemManageComponent.Instance.GetChestList);
        }
        else if (m_BagPanel2State == BagPanel2State.BagPanel2_ShowEquip)
        {
            temp = ItemManageComponent.Instance.GetAppointItem(1);
            All.AddRange(temp);
            temp = ItemManageComponent.Instance.GetAppointItem(2);
            All.AddRange(temp);
            temp = ItemManageComponent.Instance.GetAppointItem(3);
            All.AddRange(temp);
            ItemManageComponent.Instance.SortTime(All);
            SetEmpty(All);
        }
        else if (m_BagPanel2State == BagPanel2State.BagPanel2_ShowFood)
        {
            temp = ItemManageComponent.Instance.GetAppointItem(5, 1);
            All.AddRange(temp);
            temp = ItemManageComponent.Instance.GetAppointItem(5, 2);
            All.AddRange(temp);
            temp = ItemManageComponent.Instance.GetAppointItem(5, 3);
            All.AddRange(temp);
            ItemManageComponent.Instance.SortTime(All);
            SetEmpty(All);
        }
        else if (m_BagPanel2State == BagPanel2State.BagPanel2_SHowPet)
        {
            All = ItemManageComponent.Instance.GetAppointItem(4, 5);
            ItemManageComponent.Instance.SortTime(All);
            SetEmpty(All);
        }
        else if (m_BagPanel2State == BagPanel2State.BagPanel2_ShowWeapon)
        {
            All = ItemManageComponent.Instance.GetAppointItem(1);
            ItemManageComponent.Instance.SortTime(All);
            SetEmpty(All);
        }
        else if (m_BagPanel2State == BagPanel2State.BagPanel2_ShowAll)
        {
            switch (m_BagTable)
            {
                case BagStyle.BAG_SHOWEQUIP:
                    ItemManageComponent.Instance.SortTime(ItemManageComponent.Instance.GetEquipList);
                    SetEmpty(ItemManageComponent.Instance.GetEquipList);
                    break;//.BAG_ROLELVSHOW
                case BagStyle.BAG_SHOWSTRFF:
                    ItemManageComponent.Instance.SortTime(ItemManageComponent.Instance.GetMaterialList);
                    SetEmpty(ItemManageComponent.Instance.GetMaterialList);
                    break;

                case BagStyle.BAG_SHOWSERVAMT:
                    ItemManageComponent.Instance.SortTime(ItemManageComponent.Instance.GetPetList);
                    SetEmpty(ItemManageComponent.Instance.GetPetList);
                    break;

                case BagStyle.BAG_SHOWCUBE:
                    ItemManageComponent.Instance.SortTime(ItemManageComponent.Instance.GetChestList);
                    SetEmpty(ItemManageComponent.Instance.GetChestList);
                    break;

                case BagStyle.BAG_SHOWALL:
                    SetEmpty(ItemManageComponent.Instance.SortAllTime());
                    break;
            }
        }
    }

    /// <summary>
    /// 点击类型排序
    /// </summary>
    public void ClickType()
    {
        List<FormulaHost> All = new List<FormulaHost>();
        List<FormulaHost> temp = new List<FormulaHost>();
        if (m_BagPanel2State == BagPanel2State.BagPanel2_ShowChip)
        {
            All = ItemManageComponent.Instance.GetAppointItem(3);
            ItemManageComponent.Instance.SortType(All);
            SetEmpty(All);
        }
        else if (m_BagPanel2State == BagPanel2State.BagPanel2_ShowCube)
        {
            ItemManageComponent.Instance.SortType(ItemManageComponent.Instance.GetChestList);
            SetEmpty(ItemManageComponent.Instance.GetChestList);
        }
        else if (m_BagPanel2State == BagPanel2State.BagPanel2_ShowEquip)
        {
            temp = ItemManageComponent.Instance.GetAppointItem(1);
            All.AddRange(temp);
            temp = ItemManageComponent.Instance.GetAppointItem(2);
            All.AddRange(temp);
            temp = ItemManageComponent.Instance.GetAppointItem(3);
            All.AddRange(temp);
            ItemManageComponent.Instance.SortType(All);
            SetEmpty(All);
        }
        else if (m_BagPanel2State == BagPanel2State.BagPanel2_ShowFood)
        {
            temp = ItemManageComponent.Instance.GetAppointItem(5, 1);
            All.AddRange(temp);
            temp = ItemManageComponent.Instance.GetAppointItem(5, 2);
            All.AddRange(temp);
            temp = ItemManageComponent.Instance.GetAppointItem(5, 3);
            All.AddRange(temp);
            ItemManageComponent.Instance.SortType(All);
            SetEmpty(All);
        }
        else if (m_BagPanel2State == BagPanel2State.BagPanel2_SHowPet)
        {
            All = ItemManageComponent.Instance.GetAppointItem(4, 5);
            ItemManageComponent.Instance.SortType(All);
            SetEmpty(All);
        }
        else if (m_BagPanel2State == BagPanel2State.BagPanel2_ShowWeapon)
        {
            All = ItemManageComponent.Instance.GetAppointItem(1);
            ItemManageComponent.Instance.SortType(All);
            SetEmpty(All);
        }
        else if (m_BagPanel2State == BagPanel2State.BagPanel2_ShowAll)
        {
            switch (m_BagTable)
            {
                case BagStyle.BAG_SHOWEQUIP:
                    ItemManageComponent.Instance.SortType(ItemManageComponent.Instance.GetEquipList);
                    SetEmpty(ItemManageComponent.Instance.GetEquipList);
                    break;//.BAG_ROLELVSHOW
                case BagStyle.BAG_SHOWSTRFF:
                    ItemManageComponent.Instance.SortType(ItemManageComponent.Instance.GetMaterialList);
                    SetEmpty(ItemManageComponent.Instance.GetMaterialList);
                    break;

                case BagStyle.BAG_SHOWSERVAMT:
                    ItemManageComponent.Instance.SortType(ItemManageComponent.Instance.GetPetList);
                    SetEmpty(ItemManageComponent.Instance.GetPetList);
                    break;

                case BagStyle.BAG_SHOWCUBE:
                    ItemManageComponent.Instance.SortType(ItemManageComponent.Instance.GetChestList);
                    SetEmpty(ItemManageComponent.Instance.GetChestList);
                    break;

                case BagStyle.BAG_SHOWALL:
                    SetEmpty(ItemManageComponent.Instance.SortAllType());
                    break;
            }
        }
    }

    /// <summary>
    /// 点击品质排序
    /// </summary>
    public void ClickQuarlity()
    {
        List<FormulaHost> All = new List<FormulaHost>();
        List<FormulaHost> temp = new List<FormulaHost>();
        if (m_BagPanel2State == BagPanel2State.BagPanel2_ShowChip)
        {
            All = ItemManageComponent.Instance.GetAppointItem(3);
            ItemManageComponent.Instance.SotrQuality(All);
            SetEmpty(All);
        }
        else if (m_BagPanel2State == BagPanel2State.BagPanel2_ShowCube)
        {
            ItemManageComponent.Instance.SotrQuality(ItemManageComponent.Instance.GetChestList);
            SetEmpty(ItemManageComponent.Instance.GetChestList);
        }
        else if (m_BagPanel2State == BagPanel2State.BagPanel2_ShowEquip)
        {
            temp = ItemManageComponent.Instance.GetAppointItem(1);
            All.AddRange(temp);
            temp = ItemManageComponent.Instance.GetAppointItem(2);
            All.AddRange(temp);
            temp = ItemManageComponent.Instance.GetAppointItem(3);
            All.AddRange(temp);
            ItemManageComponent.Instance.SotrQuality(All);
            SetEmpty(All);
        }
        else if (m_BagPanel2State == BagPanel2State.BagPanel2_ShowFood)
        {
            temp = ItemManageComponent.Instance.GetAppointItem(5, 1);
            All.AddRange(temp);
            temp = ItemManageComponent.Instance.GetAppointItem(5, 2);
            All.AddRange(temp);
            temp = ItemManageComponent.Instance.GetAppointItem(5, 3);
            All.AddRange(temp);
            ItemManageComponent.Instance.SotrQuality(All);
            SetEmpty(All);
        }
        else if (m_BagPanel2State == BagPanel2State.BagPanel2_SHowPet)
        {
            All = ItemManageComponent.Instance.GetAppointItem(4, 5);
            ItemManageComponent.Instance.SotrQuality(All);
            SetEmpty(All);
        }
        else if (m_BagPanel2State == BagPanel2State.BagPanel2_ShowWeapon)
        {
            All = ItemManageComponent.Instance.GetAppointItem(1);
            ItemManageComponent.Instance.SotrQuality(All);
            SetEmpty(All);
        }
        else if (m_BagPanel2State == BagPanel2State.BagPanel2_ShowAll)
        {
            switch (m_BagTable)
            {
                case BagStyle.BAG_SHOWEQUIP:
                    ItemManageComponent.Instance.SotrQuality(ItemManageComponent.Instance.GetEquipList);
                    SetEmpty(ItemManageComponent.Instance.GetEquipList);
                    break;//.BAG_ROLELVSHOW
                case BagStyle.BAG_SHOWSTRFF:
                    ItemManageComponent.Instance.SotrQuality(ItemManageComponent.Instance.GetMaterialList);
                    SetEmpty(ItemManageComponent.Instance.GetMaterialList);
                    break;

                case BagStyle.BAG_SHOWSERVAMT:
                    ItemManageComponent.Instance.SotrQuality(ItemManageComponent.Instance.GetPetList);
                    SetEmpty(ItemManageComponent.Instance.GetPetList);
                    break;

                case BagStyle.BAG_SHOWCUBE:
                    ItemManageComponent.Instance.SotrQuality(ItemManageComponent.Instance.GetChestList);
                    SetEmpty(ItemManageComponent.Instance.GetChestList);
                    break;

                case BagStyle.BAG_SHOWALL:
                    SetEmpty(ItemManageComponent.Instance.SortAllQuality());
                    break;
            }
        }
    }

    #endregion 点击排序按钮

    /// <summary>
    /// 判断是什么组合
    /// </summary>
    /// <returns>The compose.</returns>
    //	int JudgeCompose()
    //	{
    //		for(int i=0;i<m_ComposeList.Count;i++)
    //		{
    //			for(int j=0;j<m_ComposeList[i].Count;j++)
    //			{
    //				bool have=false;
    //				for(int k=0;k<ItemManageComponent.Instance.GetChosedItem.Count;k++)
    //				{
    //					if(ItemManageComponent.Instance.GetChosedItem[k].GetDynamicDataByKey("ID")==m_ComposeList[i][j])
    //					{
    //						have=true;
    //						break;
    //					}
    //				}
    //				if(have)
    //				{
    //					if(j==m_ComposeList[i].Count-1)
    //					{
    //						Debug.Log("特殊组合");
    //						return i;//返回第几个组合
    //					}
    //				}
    //				else
    //				{
    //					break;
    //				}
    //			}
    //		}
    //		return -1;
    //	}
    private void OnEnable()
    {
        InitBag();
        LoadPos();
        Messenger.MarkAsPermanent(BroadcastBagChoseItem);
        Messenger.MarkAsPermanent(BroadcastBagUpSize);
        Messenger.MarkAsPermanent(BroadcastBagEmptyReSet);
        Messenger.MarkAsPermanent(BroadcastBagItem);
        Messenger.MarkAsPermanent(BroadcastClearChose);
        Messenger.AddListener(BroadcastBagChoseItem, ChangeCost);
        Messenger.AddListener(BroadcastBagUpSize, ShowBagSize);
        Messenger.AddListener(BroadcastBagEmptyReSet, RefreshBagGrid);
        Messenger.AddListener(BroadcastBagItem, SortItem);
        Messenger.AddListener(BroadcastClearChose, ClearChoseItem);
    }

    /// <summary>
    /// 清空背包选择项
    /// </summary>
    public void ClearBagChosedItem()
    {
        for (int i = 0; i < ItemManageComponent.Instance.GetChosedItem.Count; i++)
        {
            ItemManageComponent.Instance.GetChosedItem[i].SetDynamicData(SignKeys.CHOSED, 0);
        }
        ItemManageComponent.Instance.GetChosedItem.Clear();
    }

    private void InitBag()
    {
        ShowBagSize();
        if (ItemManageComponent.Instance.GetChosedItem.Count == 0)
        {
            Messenger.Broadcast(bagItemCell.BraodCast_HideChoseTag);
            Messenger.Broadcast(bagItemCell.BroadCast_HideMask);
        }
    }

    private void OnDisable()
    {
        BagManageComponent.Instance.SetBagHaveNew(0);
        ResetToggle();
        Messenger.RemoveListener(BroadcastBagChoseItem, ChangeCost);
        Messenger.RemoveListener(BroadcastBagUpSize, ShowBagSize);
        Messenger.RemoveListener(BroadcastBagEmptyReSet, RefreshBagGrid);
        Messenger.RemoveListener(BroadcastBagItem, SortItem);
        Messenger.RemoveListener(BroadcastClearChose, ClearChoseItem);
    }

    #region 出售物品

    public GameObject SaleMenu;//下面出售菜单
    public GameObject Normal;  //通常情况  扩充背包
    public UILabel GetMoney;   //获取的金钱
                               //public List<FormulaHost> m_lisChosedItem=new List<FormulaHost>();			//背包选择的物品

    /// <summary>
    /// 获得选择物品的数量
    /// </summary>
    /// <returns>The all chose item number.</returns>
    public int GetAllChoseItemNumber()
    {
        int NUmber = 0;
        for (int i = 0, max = ItemManageComponent.Instance.GetChosedItem.Count; i < max; i++)
        {
            NUmber += ItemManageComponent.Instance.GetChosedItem[i].GetDynamicIntByKey(SignKeys.CHOSED);
        }
        return NUmber;
    }

    //	void InitSaleItemMenu()
    //	{
    ////		SaleMenu.SetActive(false);
    ////		Normal.SetActive(true);
    //	//	ClearChoseItem();
    //	}
    /// <summary>
    /// 清空出售面板
    /// </summary>
    private void ClearChoseItem()
    {
        if (m_BagPanel2State2 == BagPanelState2.BagPanelState2_Sale)
        {
            ChosedLabel.text = "0/20";
            GetMoney.text = "0";
            for (int i = 0, max = ItemManageComponent.Instance.GetChosedItem.Count; i < max; i++)
            {
                ItemManageComponent.Instance.GetChosedItem[i].SetDynamicData("CHOSED", 0);
            }
            ItemManageComponent.Instance.GetChosedItem.Clear();
        }
        else if (m_BagPanel2State2 == BagPanelState2.BagPanelState2_Chosed)
        {
            ChosedLabel.text = "0/4";
            GetMoney.text = "0";
            for (int i = 0, max = ItemManageComponent.Instance.GetChosedItem.Count; i < max; i++)
            {
                ItemManageComponent.Instance.GetChosedItem[i].SetDynamicData("CHOSED", 0);
            }
            ItemManageComponent.Instance.GetChosedItem.Clear();
        }
        //	Messenger.Broadcast(UIPerfabsManage.BraodCast_BagSaleItemPluralNumber);
    }

    /// <summary>
    /// 设置出售的钱
    /// </summary>
    public void SetMoney()
    {
        int temp = 0;
        for (int i = 0, max = ItemManageComponent.Instance.GetChosedItem.Count; i < max; i++)
        {
            if (ItemManageComponent.Instance.GetChosedItem == null)
            {
                Debug.LogError("MISSS null");
            }
            int ChosedNumber = ItemManageComponent.Instance.GetChosedItem[i].GetDynamicIntByKey(SignKeys.CHOSED);
            Debug.Log("Chose itEM nUMBER " + ChosedNumber);
            temp += (int)ItemManageComponent.Instance.GetChosedItem[i].GetDynamicIntByKey(SignKeys.SOLD) * ChosedNumber;
        }
        GetMoney.text = temp.ToString();
    }

    /// <summary>
    /// 设置选择的数量
    /// </summary>
    public void SetChoseNb()
    {
        if (m_BagPanel2State2 == BagPanelState2.BagPanelState2_Sale)
        {
            ChosedLabel.text = GetAllChoseItemNumber() + "/20";
        }
        else if (m_BagPanel2State2 == BagPanelState2.BagPanelState2_Chosed)
        {
            ChosedLabel.text = GetAllChoseItemNumber() + "/4";
        }
        else
        {
            m_ChoseItemNumberShow.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// 设置出售信息
    /// </summary>
    public void SetSaleInfo()
    {
        SetChoseNb();
        SetMoney();
    }

    /// <summary>
    /// 点击出售按钮
    /// </summary>
    public void ClickSaleButton()
    {
        if (m_BagPanel2State2 == BagPanelState2.BagPanelState2_Sale)
            return;

        m_BagPanel2State2 = BagPanelState2.BagPanelState2_Sale;
        UIManageSystem.g_Instance.SetMainMenuBackButton(true);
        SaleMenu.SetActive(true);
        ShowSale();
        Messenger.Broadcast<bool>(bagItemCell.BraodCast_BagSaleItemPluralNumber, true);
    }

    //显示出售
    private void ShowSale()
    {
        SaleMenu.SetActive(true);
        m_ChoseItemNumberShow.SetActive(true);
        Normal.SetActive(false);
        ClearChoseItem();
    }

    /// <summary>
    /// 隐藏出售面板
    /// </summary>
    public void HideSale()
    {
        ClearChoseItem();
        Messenger.Broadcast(bagItemCell.BraodCast_HideChoseTag);
        Messenger.Broadcast(bagItemCell.BraodCast_BagSaleItemPluralNumber, true);
        SaleMenu.SetActive(false);
        m_ChoseItemNumberShow.SetActive(false);
        Normal.SetActive(true);
        UIPerfabsManage.g_Instan.GetUIState = UIPerfabsManage.GameUIState.SuitCase_Main;
    }

    /// <summary>
    /// 点击确定按钮
    /// </summary>
    public void ClickSuerSale()
    {
        if (m_BagPanel2State == BagPanel2State.BagPanel2_ShowAll)
        {
            //CommonPanel.GetInstance().SetBlurSub(null,false);
            //CommonPanel.GetInstance().ShowEnSureSalePanel(int.Parse(GetMoney.text), OKEnsureSaleItemListCallBack, CancelEnsureSaleItemListCallBack, ItemManageComponent.Instance.CheckChoseItemQuility());
        }
        else if (m_BagPanel2State == BagPanel2State.BagPanel2_ShowFood)
        {
            if (LevelUpPanel.m_LevelUpPanelState == LevelUpPanelState.LevelUpPanelState_ChosePetFood)
            {
                Debug.Log("选择升级的食物");

                UIManageSystem.g_Instance.RomoveUI();

                UIManageSystem.g_Instance.ShowUI();
                Messenger.Broadcast<bool>(ItemInfoPanel.ItemInfoChoseItem, false);
                //CommonPanel.GetInstance().SetBlurSub(null, true);
            }
            else if (LevelUpPanel.m_LevelUpPanelState == LevelUpPanelState.LevelUpPanelState_ChoseHeroFood)
            {
                UIManageSystem.g_Instance.RomoveUI();
                UIManageSystem.g_Instance.ShowUI();
                Messenger.Broadcast<bool>(ItemInfoPanel.ItemInfoChoseItem, false);
            }
        }
        else if (m_BagPanel2State == BagPanel2State.BagPanel2_ShowEquip)
        {
            Debug.Log("选择升级的食物");

            UIManageSystem.g_Instance.RomoveUI();

            UIManageSystem.g_Instance.ShowUI();

            Messenger.Broadcast<bool>(ItemInfoPanel.ItemInfoChoseItem, false);

            //CommonPanel.GetInstance().SetBlurSub(null, true);
        }
    }

    /// <summary>
    /// 确定出售的回调
    /// </summary>
    public void OKEnsureSaleItemListCallBack()
    {
        //CommonPanel.GetInstance().CloseBlur(null);
        //CommonPanel.GetInstance().HidEnSureSalePanel(true);
        ItemManageComponent.Instance.UseItemList(ItemManageComponent.Instance.GetChosedItem);
        Debug.Log("all count:" + ItemManageComponent.Instance.GetAllItemCount());

        int Money = int.Parse(GetMoney.text);
        AccountGoldManagerComponent.Instance.ChangeMoney(Money);

        ItemManageComponent.Instance.GetChosedItem.Clear();
        this.ClearChoseItem();
        this.SortItem();
        //Messenger.Broadcast (MainMenuPanel.BroadcastChangeMoney);
        Messenger.Broadcast(bagItemCell.BroadCast_HideMask);
        Messenger.Broadcast<bool>(bagItemCell.BraodCast_BagSaleItemPluralNumber, true);
        this.ShowBagSize();
    }

    /// <summary>
    /// 取消出售的回调
    /// </summary>
    /// <returns><c>true</c> if this instance cancel ensure sale item list call back; otherwise, <c>false</c>.</returns>
    public void CancelEnsureSaleItemListCallBack()
    {
        //CommonPanel.GetInstance().CloseBlur(null);
        //CommonPanel.GetInstance().HidEnSureSalePanel(true);
    }

    #endregion 出售物品

    /// <summary>
    /// 添加背包格子
    /// </summary>
    public void AddBagSpace()
    {
        m_SaleBoxPanel.ShowAddSpace();
        ShowBagSize();
    }

    /// <summary>
    /// 改变选择物品的价钱
    /// </summary>
    private void ChangeCost()
    {
        SetChoseNb();
        int AllMoney = 0;
        int AllExp = 0;
        for (int i = 0, max = ItemManageComponent.Instance.GetChosedItem.Count; i < max; i++)
        {
            if (ItemManageComponent.Instance.GetChosedItem == null)
            {
                Debug.LogError("MISSS null");
            }
            int ChosedNumber = ItemManageComponent.Instance.GetChosedItem[i].GetDynamicIntByKey(SignKeys.CHOSED);
            Debug.Log("Chose itEM nUMBER " + ChosedNumber);
            //AllMoney+=(int)ItemManageComponent.Instance.GetChosedItem[i].Result(FormulaKeys.FORMULA_40)*ChosedNumber;
            //AllExp+=(int)ItemManageComponent.Instance.GetChosedItem[i].Result(FormulaKeys.FORMULA_41)*ChosedNumber;
        }
    }

    //
    private List<GameObject> m_BagItem = new List<GameObject>();                 //UI显示的Ob 所有的物品

    /// <summary>
    /// 消耗的物品
    /// </summary>
    public void UseChosedItem()
    {
    }

    private void ShowBagSize()
    {
        //		Debug.LogWarning(BagManageComponent.Instance.GetBagSize());
        m_BagSize.text = string.Format("{0}/{1}", ItemManageComponent.Instance.GetAllItemCount(), BagManageComponent.Instance.GetBagSize());
    }

    /// <summary>
    /// 当选满时其余物品半透明
    /// </summary>
    public void AlphaItem(bool _show)
    {
        if (_show)
        {
            for (int i = 0; i < m_listbagEmpty.Count; i++)
            {
                if (ItemManageComponent.Instance.GetChosedItem.Contains(m_listbagEmpty[i].GetComponent<bagItemCell>().GetFormulaHost()))
                {
                    continue;
                }
                TweenAlpha.Begin(m_listbagEmpty[i], 0f, 1f);
                m_listbagEmpty[i].GetComponent<BoxCollider>().enabled = false;
            }
        }
        else
        {
            for (int i = 0; i < m_listbagEmpty.Count; i++)
            {
                if (ItemManageComponent.Instance.GetChosedItem.Contains(m_listbagEmpty[i].GetComponent<bagItemCell>().GetFormulaHost()))
                {
                    continue;
                }
                TweenAlpha.Begin(m_listbagEmpty[i], 0f, 0.4f);
                m_listbagEmpty[i].GetComponent<BoxCollider>().enabled = true;
            }
        }
    }

    /// <summary>
    /// 点击确定按钮在人物升级界面
    /// </summary>
    public void ClickConfirm()
    {
        Debug.LogWarning("choosed count:" + ItemManageComponent.Instance.GetChosedItem.Count);
        this.gameObject.SetActive(false);
        UIPerfabsManage.g_Instan.GetUIState = UIPerfabsManage.GameUIState.CHARACTOR_HEROLEVELUp;
        //bagPanel2.
    }

    // Use this for initialization
    private void Start()
    {
        SavePos();              //保存起始位置
    }

    // Update is called once per frame
    private void Update()
    {
    }

    #region 切换Tab后物品置顶

    public UIPanel ScroViewPanel;//背包滑动
    private Vector3 pos;
    private Vector2 offSet;

    private void SavePos()
    {
        pos = ScroViewPanel.gameObject.transform.localPosition;
        offSet = ScroViewPanel.clipOffset;
    }

    private void LoadPos()
    {
        if (pos == Vector3.zero)
            return;
        ScroViewPanel.gameObject.transform.localPosition = pos;
        ScroViewPanel.clipOffset = offSet;
    }

    #endregion 切换Tab后物品置顶

    public static BagPanel2State m_BagPanel2State = BagPanel2State.BagPanel2_ShowAll;

    public static BagPanel2State GetBagPanelState()
    {
        return m_BagPanel2State;
    }

    public static BagPanelEquipPlace m_BagPanelEquipPlace = BagPanelEquipPlace.BagPanelEquipPlace_None;

    public static BagPanelEquipPlace GetBagPanelEquipPlace()
    {
        return m_BagPanelEquipPlace;
    }

    public static BagPanelState2 m_BagPanel2State2 = BagPanelState2.BagPanelState2_Normal;

    public static BagPanelState2 GetBagPanelState2()
    {
        return m_BagPanel2State2;
    }

    /// <summary>
    /// 显示界面
    /// </summary>
    /// <param name="_state">State.</param>
    /// <param name="_host">Host.</param>
    /// <param name="_Layer">Layer.</param>
    public override void ShowPanel(int _state = 1, FormulaBase.FormulaHost _host = null, int _Layer = -1)
    {
        //InitCellArr();
        this.gameObject.SetActive(true);
        m_MysticalHost = _host;
        SetPanelLayer(_Layer);
        if (_Layer != -1)
        {
            ScroViewPanel.depth = _Layer * 5 + 1;
            ScroViewPanel.sortingOrder = _Layer * 5 + 1;
        }
        RefreshBagGrid();                   //显示所有空格位
        SetSaleInfo();
        m_BagPanel2State = (BagPanel2State)(_state / 10);
        m_BagPanel2State2 = (BagPanelState2)(_state % 10);
        if (m_BagPanel2State == BagPanel2State.BagPanel2_ShowAll)
        {
            ItemManageComponent.Instance.ClearChosedItem();
        }
        ShowItem();
        if (m_BagPanel2State2 == BagPanelState2.BagPanelState2_Chosed)
        {
            if (ItemManageComponent.Instance.GetChosedItem.Count == 4)
            {
                Messenger.Broadcast(bagItemCell.BroadCast_ShowMask);
            }
        }
    }

    /// <summary>
    /// 显示物品
    /// </summary>
    public void ShowItem()
    {
        if (m_BagPanel2State == BagPanel2State.BagPanel2_ShowEquip
         || m_BagPanel2State == BagPanel2State.BagPanel2_ShowFood
        )
        {
            m_ChoseItemNumberShow.SetActive(true);
            SetChoseNb();
        }
        else
        {
            m_ChoseItemNumberShow.SetActive(false);
        }

        if (m_BagPanel2State == BagPanel2State.BagPanel2_ShowAll)
        {
            ShowBagSize();
            m_PointOb.SetActive(true);
        }
        else
        {
            m_PointOb.SetActive(false);
            //	SortItem();						//这地方隐藏了TAb  不自动进行一次 选择 手动刷一下物品
        }
        SortItem();
    }

    /// <summary>
    /// 点击回退按钮
    /// </summary>
    public override void PanelClickBack()
    {
        if (m_BagPanel2State == BagPanel2State.BagPanel2_ShowAll)
        {
            //			if(m_BagPanel2State2==BagPanelState2.BagPanelState2_Sale)
            //			{
            HideSale();
            UIManageSystem.g_Instance.SetMainMenuBackButton();
            Debug.Log("隐藏出售的东西");
            m_BagPanel2State2 = BagPanelState2.BagPanelState2_Normal;
            Messenger.Broadcast<bool>(bagItemCell.BraodCast_BagSaleItemPluralNumber, true);
            ItemManageComponent.Instance.ClearChosedItem();
            Messenger.Broadcast(bagItemCell.BroadCast_HideMask);
            //			}
        }
        else
        {
            if (m_BagPanel2State2 == BagPanelState2.BagPanelState2_Chosed)
            {
                ItemManageComponent.Instance.ClearChosedItem();
                Messenger.Broadcast(bagItemCell.BroadCast_HideMask);
                //	CommonPanel.GetInstance().SetBlurSub(null);

                if (LevelUpPanel.m_LevelUpPanelState == LevelUpPanelState.LevelUpPanelState_ChosePetFood)
                {
                    Messenger.Broadcast<bool>(ItemInfoPanel.ItemInfoChoseItem, false);
                    //CommonPanel.GetInstance().SetBlurSub(null, true);
                }
                else if (LevelUpPanel.m_LevelUpPanelState == LevelUpPanelState.LevelUpPanelState_ChoseEquip)
                {
                    Messenger.Broadcast<bool>(ItemInfoPanel.ItemInfoChoseItem, false);
                    //CommonPanel.GetInstance().SetBlurSub(null, true);
                }
                else if (LevelUpPanel.m_LevelUpPanelState == LevelUpPanelState.LevelUpPanelState_ChoseHeroFood)
                {
                    Messenger.Broadcast<bool>(ItemInfoPanel.ItemInfoChoseItem, false);
                }
            }
            UIManageSystem.g_Instance.RomoveUI();
            //m_PointOb.SetActive();
        }
    }

    #region 新的背包物品移动

    public void InitCellArr()
    {
        m_listAllItemCell.Add(m_Arr1);
        m_listAllItemCell.Add(m_Arr2);
        m_listAllItemCell.Add(m_Arr3);
        m_listAllItemCell.Add(m_Arr4);
        m_listAllItemCell.Add(m_Arr5);
    }

    public List<GameObject> m_Arr1 = new List<GameObject>();
    public List<GameObject> m_Arr2 = new List<GameObject>();
    public List<GameObject> m_Arr3 = new List<GameObject>();
    public List<GameObject> m_Arr4 = new List<GameObject>();
    public List<GameObject> m_Arr5 = new List<GameObject>();                                        //几个物体
    public List<GameObject> m_listAllItemCellTable = new List<GameObject>();                        //物体的父节点
    public List<List<GameObject>> m_listAllItemCell = new List<List<GameObject>>();             //2纬数组
    public UIWrapContent m_CellWrapContent;

    /// <summary>
    /// 设置新的关卡行列
    /// </summary>
    public void SetnewCellRow(List<FormulaHost> _list)
    {
        int MaxCell = BagManageComponent.Instance.GetBagSize();
        int Row = (MaxCell - 1) / 6 + 1;
        int Line = (MaxCell - 1) % 6;
        if (Row <= 5)
        {
            m_CellWrapContent.enabled = false;
            for (int i = 0; i < m_listAllItemCell.Count; i++)
            {
                if (i < Row)
                {
                    m_listAllItemCellTable[i].SetActive(true);
                    if (i == Row - 1)
                    {
                        for (int j = 0; j < 6; j++)
                        {
                            if (j <= Line)
                            {
                                m_listAllItemCell[i][j].SetActive(true);
                            }
                            else
                            {
                                m_listAllItemCell[i][j].SetActive(false);
                            }
                        }
                        Debug.Log("判断咧");
                    }
                }
                else
                {
                    m_listAllItemCellTable[i].SetActive(false);
                }
            }
        }
        else
        {
            for (int i = 0; i < 5; i++)
            {
                for (int j = 0; j < 6; j++)
                {
                    m_listAllItemCell[i][j].SetActive(true);
                }
            }
        }
    }

    public int RealRow = 0;

    /// <summary>
    /// 设置物品的数据
    /// </summary>
    public void SetRowData(List<FormulaHost> _temp)
    {
        RealRow = 0;
        int MaxCell = _temp.Count;
        int Row = (MaxCell - 1) / 6 + 1;
        int Line = (MaxCell - 1) % 6;
        for (int i = 0; i < 5; i++)
        {
            for (int j = 0; j < 6; j++)
            {
                if (i < Row)
                {
                    if (i == Row - 1)
                    {
                    }
                    else
                    {
                        //m_listAllItemCell[i][j].GetComponent<bagItemCell>().SetUI(_temp[(Row-1)*6]);
                    }
                }
            }
        }
    }

    public List<FormulaHost> m_ShowHost;

    //for(int if=0;i++)

    #endregion 新的背包物品移动
}